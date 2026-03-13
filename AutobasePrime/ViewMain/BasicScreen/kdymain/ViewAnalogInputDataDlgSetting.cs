using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using AutoLibLocal;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewAnalogInputDataDlgSetting.
	/// </summary>
	public class ViewAnalogInputDataDlgSetting : AnalogDigitalCommonDrawClass
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		public System.Windows.Forms.CheckBox checkBox_guide_line;
		public System.Windows.Forms.CheckBox checkBox_alarm_line;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Button button_CANCEL;
		public System.Windows.Forms.ComboBox comboBox_show_method;
		public System.Windows.Forms.ComboBox comboBox_dot_type;
		private System.Windows.Forms.GroupBox groupBox_view;
		public MyNumericUpDown numericUpDown_view_full;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label_view_full;
		public MyNumericUpDown numericUpDown_view_base;
		private System.Windows.Forms.GroupBox groupBox3;
		public MyNumericUpDown numericUpDown_sum_total;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		public MyNumericUpDown numericUpDown_sum_part;
		private System.Windows.Forms.GroupBox groupBox_show_time;
		private System.Windows.Forms.Label label4;
		public MyNumericUpDown numericUpDown_hour;
		private System.Windows.Forms.Label label5;
		public MyNumericUpDown numericUpDown_day;
		private System.Windows.Forms.Label label6;
		public MyNumericUpDown numericUpDown_month;
		private System.Windows.Forms.Label label7;
		public MyNumericUpDown numericUpDown_year;
		public MyNumericUpDown numericUpDown_min;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		public MyNumericUpDown numericUpDown_min_part;
		public MyNumericUpDown numericUpDown_hour_part;
		public MyNumericUpDown numericUpDown_day_part;
		public MyNumericUpDown numericUpDown_month_part;
		public MyNumericUpDown numericUpDown_year_part;
		public	DateTime	tSumTotal = new DateTime(2004, 12, 1);
		public	DateTime	tSumPart = new DateTime(2004, 12, 1);
        public CheckBox checkBoxWhiteBackground;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ViewAnalogInputDataDlgSetting(Form parent)
            
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewAnalogInputDataDlgSetting));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxWhiteBackground = new System.Windows.Forms.CheckBox();
            this.comboBox_show_method = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_alarm_line = new System.Windows.Forms.CheckBox();
            this.checkBox_guide_line = new System.Windows.Forms.CheckBox();
            this.comboBox_dot_type = new System.Windows.Forms.ComboBox();
            this.button_OK = new System.Windows.Forms.Button();
            this.button_CANCEL = new System.Windows.Forms.Button();
            this.groupBox_view = new System.Windows.Forms.GroupBox();
            this.numericUpDown_view_full = new AutoLibLocal.MyNumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label_view_full = new System.Windows.Forms.Label();
            this.numericUpDown_view_base = new AutoLibLocal.MyNumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox_show_time = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDown_min = new AutoLibLocal.MyNumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDown_hour = new AutoLibLocal.MyNumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown_day = new AutoLibLocal.MyNumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDown_month = new AutoLibLocal.MyNumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDown_year = new AutoLibLocal.MyNumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown_sum_part = new AutoLibLocal.MyNumericUpDown();
            this.numericUpDown_sum_total = new AutoLibLocal.MyNumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDown_min_part = new AutoLibLocal.MyNumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.numericUpDown_hour_part = new AutoLibLocal.MyNumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.numericUpDown_day_part = new AutoLibLocal.MyNumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.numericUpDown_month_part = new AutoLibLocal.MyNumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.numericUpDown_year_part = new AutoLibLocal.MyNumericUpDown();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox_view.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_view_full)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_view_base)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox_show_time.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_min)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_day)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_month)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_year)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_sum_part)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_sum_total)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_min_part)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hour_part)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_day_part)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_month_part)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_year_part)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxWhiteBackground);
            this.groupBox1.Controls.Add(this.comboBox_show_method);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // checkBoxWhiteBackground
            // 
            resources.ApplyResources(this.checkBoxWhiteBackground, "checkBoxWhiteBackground");
            this.checkBoxWhiteBackground.Name = "checkBoxWhiteBackground";
            // 
            // comboBox_show_method
            // 
            resources.ApplyResources(this.comboBox_show_method, "comboBox_show_method");
            this.comboBox_show_method.Items.AddRange(new object[] {
            resources.GetString("comboBox_show_method.Items"),
            resources.GetString("comboBox_show_method.Items1"),
            resources.GetString("comboBox_show_method.Items2")});
            this.comboBox_show_method.Name = "comboBox_show_method";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_alarm_line);
            this.groupBox2.Controls.Add(this.checkBox_guide_line);
            this.groupBox2.Controls.Add(this.comboBox_dot_type);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
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
            // button_OK
            // 
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // button_CANCEL
            // 
            this.button_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button_CANCEL, "button_CANCEL");
            this.button_CANCEL.Name = "button_CANCEL";
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
            99999999,
            0,
            0,
            0});
            this.numericUpDown_view_full.Minimum = new decimal(new int[] {
            99999999,
            0,
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox_show_time);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.numericUpDown_sum_part);
            this.groupBox3.Controls.Add(this.numericUpDown_sum_total);
            this.groupBox3.Controls.Add(this.groupBox4);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // groupBox_show_time
            // 
            this.groupBox_show_time.Controls.Add(this.label8);
            this.groupBox_show_time.Controls.Add(this.numericUpDown_min);
            this.groupBox_show_time.Controls.Add(this.label4);
            this.groupBox_show_time.Controls.Add(this.numericUpDown_hour);
            this.groupBox_show_time.Controls.Add(this.label5);
            this.groupBox_show_time.Controls.Add(this.numericUpDown_day);
            this.groupBox_show_time.Controls.Add(this.label6);
            this.groupBox_show_time.Controls.Add(this.numericUpDown_month);
            this.groupBox_show_time.Controls.Add(this.label7);
            this.groupBox_show_time.Controls.Add(this.numericUpDown_year);
            resources.ApplyResources(this.groupBox_show_time, "groupBox_show_time");
            this.groupBox_show_time.Name = "groupBox_show_time";
            this.groupBox_show_time.TabStop = false;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // numericUpDown_min
            // 
            resources.ApplyResources(this.numericUpDown_min, "numericUpDown_min");
            this.numericUpDown_min.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numericUpDown_min.Name = "numericUpDown_min";
            this.numericUpDown_min.SampleProperty = 0;
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
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
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
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
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
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
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
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // numericUpDown_sum_part
            // 
            this.numericUpDown_sum_part.DecimalPlaces = 2;
            resources.ApplyResources(this.numericUpDown_sum_part, "numericUpDown_sum_part");
            this.numericUpDown_sum_part.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.numericUpDown_sum_part.Minimum = new decimal(new int[] {
            99999999,
            0,
            0,
            -2147483648});
            this.numericUpDown_sum_part.Name = "numericUpDown_sum_part";
            this.numericUpDown_sum_part.SampleProperty = 0;
            // 
            // numericUpDown_sum_total
            // 
            this.numericUpDown_sum_total.DecimalPlaces = 2;
            resources.ApplyResources(this.numericUpDown_sum_total, "numericUpDown_sum_total");
            this.numericUpDown_sum_total.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.numericUpDown_sum_total.Minimum = new decimal(new int[] {
            99999999,
            0,
            0,
            -2147483648});
            this.numericUpDown_sum_total.Name = "numericUpDown_sum_total";
            this.numericUpDown_sum_total.SampleProperty = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.numericUpDown_min_part);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.numericUpDown_hour_part);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.numericUpDown_day_part);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.numericUpDown_month_part);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.numericUpDown_year_part);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // numericUpDown_min_part
            // 
            resources.ApplyResources(this.numericUpDown_min_part, "numericUpDown_min_part");
            this.numericUpDown_min_part.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numericUpDown_min_part.Name = "numericUpDown_min_part";
            this.numericUpDown_min_part.SampleProperty = 0;
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // numericUpDown_hour_part
            // 
            resources.ApplyResources(this.numericUpDown_hour_part, "numericUpDown_hour_part");
            this.numericUpDown_hour_part.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.numericUpDown_hour_part.Name = "numericUpDown_hour_part";
            this.numericUpDown_hour_part.SampleProperty = 0;
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // numericUpDown_day_part
            // 
            resources.ApplyResources(this.numericUpDown_day_part, "numericUpDown_day_part");
            this.numericUpDown_day_part.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.numericUpDown_day_part.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_day_part.Name = "numericUpDown_day_part";
            this.numericUpDown_day_part.SampleProperty = 0;
            this.numericUpDown_day_part.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // numericUpDown_month_part
            // 
            resources.ApplyResources(this.numericUpDown_month_part, "numericUpDown_month_part");
            this.numericUpDown_month_part.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDown_month_part.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_month_part.Name = "numericUpDown_month_part";
            this.numericUpDown_month_part.SampleProperty = 0;
            this.numericUpDown_month_part.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // numericUpDown_year_part
            // 
            resources.ApplyResources(this.numericUpDown_year_part, "numericUpDown_year_part");
            this.numericUpDown_year_part.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDown_year_part.Name = "numericUpDown_year_part";
            this.numericUpDown_year_part.SampleProperty = 0;
            this.numericUpDown_year_part.Value = new decimal(new int[] {
            2002,
            0,
            0,
            0});
            // 
            // ViewAnalogInputDataDlgSetting
            // 
            this.AcceptButton = this.button_OK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.button_CANCEL;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox_view);
            this.Controls.Add(this.button_CANCEL);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewAnalogInputDataDlgSetting";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ViewAnalogInputDataDlgSetting_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox_view.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_view_full)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_view_base)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox_show_time.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_min)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_day)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_month)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_year)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_sum_part)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_sum_total)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_min_part)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hour_part)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_day_part)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_month_part)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_year_part)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			try 
			{
				int 	year = tSumTotal.Year, month = tSumTotal.Month, day = tSumTotal.Day, hour = tSumTotal.Hour, min = tSumTotal.Minute;

				year = (int)this.numericUpDown_year.Value;
				month = (int)this.numericUpDown_month.Value;
				day = (int)this.numericUpDown_day.Value;
				hour = (int)this.numericUpDown_hour.Value;
				min = (int)this.numericUpDown_min.Value;
				this.tSumTotal = new DateTime(year, month, day, hour, min, 0);

				year = (int)this.numericUpDown_year_part.Value;
				month = (int)this.numericUpDown_month_part.Value;
				day = (int)this.numericUpDown_day_part.Value;
				hour = (int)this.numericUpDown_hour_part.Value;
				min = (int)this.numericUpDown_min_part.Value;
				this.tSumPart = new DateTime(year, month, day, hour, min, 0);
			}
			catch {}

			Close();
		}

		private void ViewAnalogInputDataDlgSetting_Load(object sender, System.EventArgs e)
		{
			try 
			{
				this.numericUpDown_year.Value = this.tSumTotal.Year;
				this.numericUpDown_month.Value = this.tSumTotal.Month;
				this.numericUpDown_day.Value = this.tSumTotal.Day;
				this.numericUpDown_hour.Value = this.tSumTotal.Hour;
				this.numericUpDown_min.Value = this.tSumTotal.Minute;

				this.numericUpDown_year_part.Value = this.tSumPart.Year;
				this.numericUpDown_month_part.Value = this.tSumPart.Month;
				this.numericUpDown_day_part.Value = this.tSumPart.Day;
				this.numericUpDown_hour_part.Value = this.tSumPart.Hour;
				this.numericUpDown_min_part.Value = this.tSumPart.Minute;
			}
			catch {}
		}
	}
}
