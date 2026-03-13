using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using AutoLibLocal;

namespace DataEdit
{
	/// <summary>
	/// Summary description for DialogDataEditConfig.
	/// </summary>
	public class DialogDataEditConfig : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button button_CANCEL;
		private System.Windows.Forms.Button button_OK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown numericUpDown_Minute;
		private System.Windows.Forms.NumericUpDown numericUpDown_Hour;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown numericUpDown_Day;
		private System.Windows.Forms.NumericUpDown numericUpDown_Month;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown numericUpDown_Year;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button button_SetToCurrentTime;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.RadioButton radioButton3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label_Font;
		private System.Windows.Forms.Button button_Font;
		private System.Windows.Forms.CheckBox checkBox_SaveReason;
		Font		fImsiFont = new Font("±¼¸²", 9.75F);
		public	bool	bFontChang = false;

		public DialogDataEditConfig()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogDataEditConfig));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_SaveReason = new System.Windows.Forms.CheckBox();
            this.button_CANCEL = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_SetToCurrentTime = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDown_Minute = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_Hour = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDown_Day = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_Month = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown_Year = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button_Font = new System.Windows.Forms.Button();
            this.label_Font = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Minute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Hour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Day)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Month)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Year)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_SaveReason);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // checkBox_SaveReason
            // 
            resources.ApplyResources(this.checkBox_SaveReason, "checkBox_SaveReason");
            this.checkBox_SaveReason.Name = "checkBox_SaveReason";
            // 
            // button_CANCEL
            // 
            this.button_CANCEL.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button_CANCEL, "button_CANCEL");
            this.button_CANCEL.Name = "button_CANCEL";
            // 
            // button_OK
            // 
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_SetToCurrentTime);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.numericUpDown_Minute);
            this.groupBox2.Controls.Add(this.numericUpDown_Hour);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.numericUpDown_Day);
            this.groupBox2.Controls.Add(this.numericUpDown_Month);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.numericUpDown_Year);
            this.groupBox2.Controls.Add(this.label4);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // button_SetToCurrentTime
            // 
            resources.ApplyResources(this.button_SetToCurrentTime, "button_SetToCurrentTime");
            this.button_SetToCurrentTime.Name = "button_SetToCurrentTime";
            this.button_SetToCurrentTime.Click += new System.EventHandler(this.button_SetToCurrentTime_Click);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // numericUpDown_Minute
            // 
            resources.ApplyResources(this.numericUpDown_Minute, "numericUpDown_Minute");
            this.numericUpDown_Minute.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numericUpDown_Minute.Name = "numericUpDown_Minute";
            // 
            // numericUpDown_Hour
            // 
            resources.ApplyResources(this.numericUpDown_Hour, "numericUpDown_Hour");
            this.numericUpDown_Hour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.numericUpDown_Hour.Name = "numericUpDown_Hour";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // numericUpDown_Day
            // 
            resources.ApplyResources(this.numericUpDown_Day, "numericUpDown_Day");
            this.numericUpDown_Day.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.numericUpDown_Day.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_Day.Name = "numericUpDown_Day";
            this.numericUpDown_Day.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numericUpDown_Month
            // 
            resources.ApplyResources(this.numericUpDown_Month, "numericUpDown_Month");
            this.numericUpDown_Month.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.numericUpDown_Month.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_Month.Name = "numericUpDown_Month";
            this.numericUpDown_Month.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_Month.ValueChanged += new System.EventHandler(this.numericUpDown_Month_ValueChanged);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // numericUpDown_Year
            // 
            resources.ApplyResources(this.numericUpDown_Year, "numericUpDown_Year");
            this.numericUpDown_Year.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown_Year.Name = "numericUpDown_Year";
            this.numericUpDown_Year.Value = new decimal(new int[] {
            2005,
            0,
            0,
            0});
            this.numericUpDown_Year.ValueChanged += new System.EventHandler(this.numericUpDown_Year_ValueChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButton3);
            this.groupBox3.Controls.Add(this.radioButton2);
            this.groupBox3.Controls.Add(this.radioButton1);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // radioButton3
            // 
            resources.ApplyResources(this.radioButton3, "radioButton3");
            this.radioButton3.Name = "radioButton3";
            // 
            // radioButton2
            // 
            resources.ApplyResources(this.radioButton2, "radioButton2");
            this.radioButton2.Name = "radioButton2";
            // 
            // radioButton1
            // 
            resources.ApplyResources(this.radioButton1, "radioButton1");
            this.radioButton1.Name = "radioButton1";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button_Font);
            this.groupBox4.Controls.Add(this.label_Font);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // button_Font
            // 
            resources.ApplyResources(this.button_Font, "button_Font");
            this.button_Font.Name = "button_Font";
            this.button_Font.Click += new System.EventHandler(this.button_Font_Click);
            // 
            // label_Font
            // 
            resources.ApplyResources(this.label_Font, "label_Font");
            this.label_Font.Name = "label_Font";
            // 
            // DialogDataEditConfig
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button_CANCEL);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogDataEditConfig";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.DialogDataEditConfig_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Minute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Hour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Day)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Month)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Year)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		
		void setDateTimeValue(DateTime d)
		{
			this.numericUpDown_Year.Value = d.Year;
			this.numericUpDown_Month.Value = d.Month;
			this.numericUpDown_Day.Value = d.Day;
			this.numericUpDown_Hour.Value = d.Hour;
			this.numericUpDown_Minute.Value = d.Minute;
		}

		private void DialogDataEditConfig_Load(object sender, System.EventArgs e)
		{
			this.checkBox_SaveReason.Checked = DataEditConfig.bSaveWriteReason;
			setDateTimeValue(DataEditConfig.dTime);
			setMaxDayLimit();
			if(DataEditConfig.nBasicEditMode == 1) this.radioButton2.Checked = true;
			else if(DataEditConfig.nBasicEditMode == 2) this.radioButton3.Checked = true;
			else this.radioButton1.Checked = true;
			this.fImsiFont = DataEditConfig.fEditFont;
			this.label_Font.Text = string.Format("{0}, {1, 0:f0}", fImsiFont.FontFamily.Name, fImsiFont.Size);
		}

		void readCurrentDateTime()
		{
			int		year = (int)this.numericUpDown_Year.Value;
			int		month = (int)this.numericUpDown_Month.Value;
			int		day = (int)this.numericUpDown_Day.Value;
			int		hour = (int)this.numericUpDown_Hour.Value;
			int		minute = (int)this.numericUpDown_Minute.Value;
			DataEditConfig.dTime = new DateTime(year, month, day, hour, minute, 0);
		}
		
		
		private void button_OK_Click(object sender, System.EventArgs e)
		{
			DataEditConfig.bSaveWriteReason = this.checkBox_SaveReason.Checked;
			readCurrentDateTime();
			if(this.radioButton2.Checked) DataEditConfig.nBasicEditMode = 1;
			else if(this.radioButton3.Checked) DataEditConfig.nBasicEditMode = 2;
			else DataEditConfig.nBasicEditMode = 0;
			if(DataEditConfig.fEditFont != this.fImsiFont) this.bFontChang = true;
			DataEditConfig.fEditFont = this.fImsiFont;
			
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		void setMaxDayLimit()
		{
			int		year = (int)this.numericUpDown_Year.Value;
			int		month = (int)this.numericUpDown_Month.Value;
			int		day = (int)this.numericUpDown_Day.Value;
			int		limit = TimeUtil.getmonthlimit(year, month);
			
			if(day < 1) day = 1;
			if(day > limit) day = limit;
			this.numericUpDown_Day.Value = day;
			this.numericUpDown_Day.Maximum = limit;
		}

		private void numericUpDown_Year_ValueChanged(object sender, System.EventArgs e)
		{
			setMaxDayLimit();
		}

		private void numericUpDown_Month_ValueChanged(object sender, System.EventArgs e)
		{
			setMaxDayLimit();
		}

		private void button_SetToCurrentTime_Click(object sender, System.EventArgs e)
		{
			setDateTimeValue(DateTime.Now);
		}		

		private void button_Font_Click(object sender, System.EventArgs e)
		{
			FontDialog	font = new FontDialog();
			font.Font = fImsiFont;
			if(font.ShowDialog() == DialogResult.OK) 
			{
				fImsiFont = font.Font;
				this.label_Font.Text = string.Format("{0}, {1, 0:f0}", fImsiFont.FontFamily.Name, fImsiFont.Size);
			}
		}
		
		

		
	}
}
