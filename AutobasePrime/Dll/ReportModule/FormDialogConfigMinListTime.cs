using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ReportBasicLib;
using NetTools;
using AutoLibLocal;

namespace ReportModule
{
	/// <summary>
	/// Summary description for FormDialogConfigMinListTime.
	/// </summary>
	public class FormDialogConfigMinListTime : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonSetToCurrentTime;
		private System.Windows.Forms.NumericUpDown numericUpDownFrYear;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownFrMonth;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown numericUpDownFrDay;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numericUpDownFrHour;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown numericUpDownFrMinute;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown numericUpDownToMinute;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown numericUpDownToHour;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown numericUpDownToDay;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.NumericUpDown numericUpDownToMonth;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.NumericUpDown numericUpDownToYear;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormDialogConfigMinListTime()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDialogConfigMinListTime));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownFrMinute = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownFrHour = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownFrDay = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownFrMonth = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownFrYear = new System.Windows.Forms.NumericUpDown();
            this.buttonSetToCurrentTime = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDownToMinute = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownToHour = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numericUpDownToDay = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.numericUpDownToMonth = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.numericUpDownToYear = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrMinute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrHour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrMonth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrYear)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToMinute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToHour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToMonth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToYear)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.numericUpDownFrMinute);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.numericUpDownFrHour);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numericUpDownFrDay);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numericUpDownFrMonth);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericUpDownFrYear);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // numericUpDownFrMinute
            // 
            resources.ApplyResources(this.numericUpDownFrMinute, "numericUpDownFrMinute");
            this.numericUpDownFrMinute.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numericUpDownFrMinute.Name = "numericUpDownFrMinute";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // numericUpDownFrHour
            // 
            resources.ApplyResources(this.numericUpDownFrHour, "numericUpDownFrHour");
            this.numericUpDownFrHour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.numericUpDownFrHour.Name = "numericUpDownFrHour";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // numericUpDownFrDay
            // 
            resources.ApplyResources(this.numericUpDownFrDay, "numericUpDownFrDay");
            this.numericUpDownFrDay.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.numericUpDownFrDay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownFrDay.Name = "numericUpDownFrDay";
            this.numericUpDownFrDay.Value = new decimal(new int[] {
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
            // numericUpDownFrMonth
            // 
            resources.ApplyResources(this.numericUpDownFrMonth, "numericUpDownFrMonth");
            this.numericUpDownFrMonth.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDownFrMonth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownFrMonth.Name = "numericUpDownFrMonth";
            this.numericUpDownFrMonth.Value = new decimal(new int[] {
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
            // numericUpDownFrYear
            // 
            resources.ApplyResources(this.numericUpDownFrYear, "numericUpDownFrYear");
            this.numericUpDownFrYear.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownFrYear.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownFrYear.Name = "numericUpDownFrYear";
            this.numericUpDownFrYear.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // buttonSetToCurrentTime
            // 
            resources.ApplyResources(this.buttonSetToCurrentTime, "buttonSetToCurrentTime");
            this.buttonSetToCurrentTime.Name = "buttonSetToCurrentTime";
            this.buttonSetToCurrentTime.Click += new System.EventHandler(this.buttonSetToCurrentTime_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.numericUpDownToMinute);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.numericUpDownToHour);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.numericUpDownToDay);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.numericUpDownToMonth);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.numericUpDownToYear);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // numericUpDownToMinute
            // 
            resources.ApplyResources(this.numericUpDownToMinute, "numericUpDownToMinute");
            this.numericUpDownToMinute.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numericUpDownToMinute.Name = "numericUpDownToMinute";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // numericUpDownToHour
            // 
            resources.ApplyResources(this.numericUpDownToHour, "numericUpDownToHour");
            this.numericUpDownToHour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.numericUpDownToHour.Name = "numericUpDownToHour";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // numericUpDownToDay
            // 
            resources.ApplyResources(this.numericUpDownToDay, "numericUpDownToDay");
            this.numericUpDownToDay.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.numericUpDownToDay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownToDay.Name = "numericUpDownToDay";
            this.numericUpDownToDay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // numericUpDownToMonth
            // 
            resources.ApplyResources(this.numericUpDownToMonth, "numericUpDownToMonth");
            this.numericUpDownToMonth.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDownToMonth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownToMonth.Name = "numericUpDownToMonth";
            this.numericUpDownToMonth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // numericUpDownToYear
            // 
            resources.ApplyResources(this.numericUpDownToYear, "numericUpDownToYear");
            this.numericUpDownToYear.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownToYear.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownToYear.Name = "numericUpDownToYear";
            this.numericUpDownToYear.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // FormDialogConfigMinListTime
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonSetToCurrentTime);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDialogConfigMinListTime";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormDialogConfigMinListTime_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrMinute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrHour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrMonth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrYear)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToMinute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToHour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToMonth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToYear)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void FormDialogConfigMinListTime_Load(object sender, System.EventArgs e)
		{
			USER_SELECT_TIME time_fr = new USER_SELECT_TIME();
			USER_SELECT_TIME time_to = new USER_SELECT_TIME();
			ReportConfig.GetMinListTimeFr(time_fr);
			ReportConfig.GetMinListTimeTo(time_to);

			this.numericUpDownFrYear.Value = time_fr.year;
			this.numericUpDownFrMonth.Value = time_fr.mon;
			this.numericUpDownFrDay.Value = time_fr.day;
			this.numericUpDownFrHour.Value = time_fr.hour;
			this.numericUpDownFrMinute.Value = time_fr.min;

			this.numericUpDownToYear.Value = time_to.year;
			this.numericUpDownToMonth.Value = time_to.mon;
			this.numericUpDownToDay.Value = time_to.day;
			this.numericUpDownToHour.Value = time_to.hour;
			this.numericUpDownToMinute.Value = time_to.min;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			USER_SELECT_TIME time_fr = new USER_SELECT_TIME();
			USER_SELECT_TIME time_to = new USER_SELECT_TIME();

			time_fr.year = ConvertTool.ToInt32(this.numericUpDownFrYear.Value);
			time_fr.mon = ConvertTool.ToInt32(this.numericUpDownFrMonth.Value);
			time_fr.day = ConvertTool.ToInt32(this.numericUpDownFrDay.Value);
			time_fr.hour = ConvertTool.ToInt32(this.numericUpDownFrHour.Value);
			time_fr.min = ConvertTool.ToInt32(this.numericUpDownFrMinute.Value);

			time_to.year = ConvertTool.ToInt32(this.numericUpDownToYear.Value);
			time_to.mon = ConvertTool.ToInt32(this.numericUpDownToMonth.Value);
			time_to.day = ConvertTool.ToInt32(this.numericUpDownToDay.Value);
			time_to.hour = ConvertTool.ToInt32(this.numericUpDownToHour.Value);
			time_to.min = ConvertTool.ToInt32(this.numericUpDownToMinute.Value);

            DateTime t1 = new DateTime(time_fr.year, time_fr.mon, time_fr.day, time_fr.hour, time_fr.min, 0);
            DateTime t2 = new DateTime(time_to.year, time_to.mon, time_to.day, time_to.hour, time_to.min, 0);

            if (t1 > t2)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("끝 시간이 시작 시간보다 작습니다.", "시간 설정 오류");
                else
                    MessageBox.Show("Start Time must be greater then the End Time.", "Time error");

                return;
            }

			ReportConfig.SetMinListTimeFr(time_fr);
			ReportConfig.SetMinListTimeTo(time_to);

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonSetToCurrentTime_Click(object sender, System.EventArgs e)
		{
            DateTime t = DateTimeServer.Now;

			this.numericUpDownFrYear.Value = t.Year;
			this.numericUpDownFrMonth.Value = t.Month;
			this.numericUpDownFrDay.Value = t.Day;
			this.numericUpDownFrHour.Value = 0;
			this.numericUpDownFrMinute.Value = 0;

			this.numericUpDownToYear.Value = t.Year;
			this.numericUpDownToMonth.Value = t.Month;
			this.numericUpDownToDay.Value = t.Day;
			this.numericUpDownToHour.Value = 23;
			this.numericUpDownToMinute.Value = 59;
		}
	}
}
