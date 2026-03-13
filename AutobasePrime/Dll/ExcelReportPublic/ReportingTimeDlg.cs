using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ExcelReportData
{
	/// <summary>
	/// Summary description for ReportingTimeDlg.
	/// </summary>
	public class ReportingTimeDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox_show_time;
		private System.Windows.Forms.Label label4;
		public System.Windows.Forms.NumericUpDown numericUpDown_hour;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.NumericUpDown numericUpDown_day;
		private System.Windows.Forms.Label label2;
		public System.Windows.Forms.NumericUpDown numericUpDown_month;
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.NumericUpDown numericUpDown_year;
		private System.Windows.Forms.Button button_CANCEL;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Button button_Curr_Time;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ReportingTimeDlg()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportingTimeDlg));
            this.groupBox_show_time = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDown_hour = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDown_day = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown_month = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown_year = new System.Windows.Forms.NumericUpDown();
            this.button_CANCEL = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.button_Curr_Time = new System.Windows.Forms.Button();
            this.groupBox_show_time.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_day)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_month)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_year)).BeginInit();
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
            this.groupBox_show_time.Controls.Add(this.label1);
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
            this.numericUpDown_month.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // numericUpDown_year
            // 
            resources.ApplyResources(this.numericUpDown_year, "numericUpDown_year");
            this.numericUpDown_year.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDown_year.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_year.Name = "numericUpDown_year";
            this.numericUpDown_year.Value = new decimal(new int[] {
            2002,
            0,
            0,
            0});
            // 
            // button_CANCEL
            // 
            this.button_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button_CANCEL, "button_CANCEL");
            this.button_CANCEL.Name = "button_CANCEL";
            // 
            // button_OK
            // 
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // button_Curr_Time
            // 
            resources.ApplyResources(this.button_Curr_Time, "button_Curr_Time");
            this.button_Curr_Time.Name = "button_Curr_Time";
            this.button_Curr_Time.Click += new System.EventHandler(this.button_Curr_Time_Click);
            // 
            // ReportingTimeDlg
            // 
            this.AcceptButton = this.button_OK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.button_CANCEL;
            this.Controls.Add(this.button_Curr_Time);
            this.Controls.Add(this.button_CANCEL);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.groupBox_show_time);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReportingTimeDlg";
            this.groupBox_show_time.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_day)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_month)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_year)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion


		private int getmonthlimit(int year, int month)
		{
			short[] limit = new short[12];
			
			limit[0] = 31;
			limit[1] = 28;
			limit[2] = 31;
			limit[3] = 30;
			limit[4] = 31;
			limit[5] = 30;
			limit[6] = 31;
			limit[7] = 31;
			limit[8] = 30;
			limit[9] = 31;
			limit[10] = 30;
			limit[11] = 31;

			if(month <= 0 || month > 12) return 0;
			if(month != 2)	return(limit[month-1]);

			if(year%400 == 0)	return 29;
			if(year%100 == 0)	return 28;
			if(year%4 == 0)	return 29;
			return 28;
		}

		private void checkMonthLimit()
		{
			int			day;
			
			day = getmonthlimit((int)numericUpDown_year.Value, (int)numericUpDown_month.Value);
			if((int)numericUpDown_day.Value > day) numericUpDown_day.Value = day;
		}

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			checkMonthLimit();			
		}

		private void button_Curr_Time_Click(object sender, System.EventArgs e)
		{
			DateTime dt = DateTime.Now;

			numericUpDown_year.Value = dt.Year;
			numericUpDown_month.Value = dt.Month;
			numericUpDown_day.Value = dt.Day;
			numericUpDown_hour.Value = dt.Hour;			

			checkMonthLimit();			
		}
	}
}
