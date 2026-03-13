using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using AutoLibLocal;
using PublicStudioLocalMain.Schedule;

namespace PublicStudioLocalMain.Schedule
{
	/// <summary>
	/// Summary description for FormConfigScheduleWeek.
	/// </summary>
	public class FormConfigScheduleWeek : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxSun;
		private System.Windows.Forms.ComboBox comboBoxMon;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBoxTue;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBoxWed;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox comboBoxThu;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox comboBoxFri;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox comboBoxSat;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox comboBoxHoliday;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox comboBoxSpecial;
		private System.Windows.Forms.Label label9;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigScheduleWeek()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigScheduleWeek));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxSpecial = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxHoliday = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBoxSat = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxFri = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxThu = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxWed = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxTue = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxMon = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxSun = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
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
            this.groupBox1.Controls.Add(this.comboBoxSpecial);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.comboBoxHoliday);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.comboBoxSat);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.comboBoxFri);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.comboBoxThu);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.comboBoxWed);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.comboBoxTue);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBoxMon);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.comboBoxSun);
            this.groupBox1.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // comboBoxSpecial
            // 
            this.comboBoxSpecial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxSpecial, "comboBoxSpecial");
            this.comboBoxSpecial.Name = "comboBoxSpecial";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // comboBoxHoliday
            // 
            this.comboBoxHoliday.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxHoliday, "comboBoxHoliday");
            this.comboBoxHoliday.Name = "comboBoxHoliday";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // comboBoxSat
            // 
            this.comboBoxSat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxSat, "comboBoxSat");
            this.comboBoxSat.Name = "comboBoxSat";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // comboBoxFri
            // 
            this.comboBoxFri.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxFri, "comboBoxFri");
            this.comboBoxFri.Name = "comboBoxFri";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // comboBoxThu
            // 
            this.comboBoxThu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxThu, "comboBoxThu");
            this.comboBoxThu.Name = "comboBoxThu";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // comboBoxWed
            // 
            this.comboBoxWed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxWed, "comboBoxWed");
            this.comboBoxWed.Name = "comboBoxWed";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // comboBoxTue
            // 
            this.comboBoxTue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxTue, "comboBoxTue");
            this.comboBoxTue.Name = "comboBoxTue";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // comboBoxMon
            // 
            this.comboBoxMon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxMon, "comboBoxMon");
            this.comboBoxMon.Name = "comboBoxMon";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // comboBoxSun
            // 
            this.comboBoxSun.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxSun, "comboBoxSun");
            this.comboBoxSun.Name = "comboBoxSun";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // FormConfigScheduleWeek
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigScheduleWeek";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigScheduleWeek_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

        SCHEDULE_WEEK_STRUCT[] scheduleWeek;
        ArrayList blockScheduleFixed;

		private void FormConfigScheduleWeek_Load(object sender, System.EventArgs e)
		{
            scheduleWeek = ScheduleLib.ScheduleLoadWeek();
            blockScheduleFixed = ScheduleLib.ScheduleLoadFixed();

			FillComboBox(this.comboBoxSun, scheduleWeek[0].title);
			FillComboBox(this.comboBoxMon, scheduleWeek[1].title);
			FillComboBox(this.comboBoxTue, scheduleWeek[2].title);
			FillComboBox(this.comboBoxWed, scheduleWeek[3].title);
			FillComboBox(this.comboBoxThu, scheduleWeek[4].title);
			FillComboBox(this.comboBoxFri, scheduleWeek[5].title);
			FillComboBox(this.comboBoxSat, scheduleWeek[6].title);
			FillComboBox(this.comboBoxHoliday, scheduleWeek[7].title);
			FillComboBox(this.comboBoxSpecial, scheduleWeek[8].title);

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN && !AutoLib.SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_SCHEDULE_SETUP))
                this.buttonOK.Enabled = false;
		}

		void FillComboBox(ComboBox combo, string text)
		{
			int l;
			SCHEDULE_STRUCT sc;

			for(l = 0; l < blockScheduleFixed.Count; l++) 
			{
				sc = (SCHEDULE_STRUCT)blockScheduleFixed[l];
				combo.Items.Add(sc.title);
				if(sc.title == text) 
				{
					combo.SelectedIndex = combo.Items.Count-1;
				}
			}
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			scheduleWeek[0].title = this.comboBoxSun.Text;
			scheduleWeek[1].title = this.comboBoxMon.Text;
			scheduleWeek[2].title = this.comboBoxTue.Text;
			scheduleWeek[3].title = this.comboBoxWed.Text;
			scheduleWeek[4].title = this.comboBoxThu.Text;
			scheduleWeek[5].title = this.comboBoxFri.Text;
			scheduleWeek[6].title = this.comboBoxSat.Text;
			scheduleWeek[7].title = this.comboBoxHoliday.Text;
			scheduleWeek[8].title = this.comboBoxSpecial.Text;

			ScheduleSaveWeek();		
			//FindFixedScheduleAtWeek();
			//FormSchedule.OnScheduleStructChanged();

			DialogResult = DialogResult.OK;
		}

		void ScheduleSaveWeek()
		{
			string filename;
			TextWriter writer;
			int i;

			filename = String.Format("{0}\\SCHEDULE", TotalConfig.sDirWorkProject);
			Directory.CreateDirectory(filename);
			filename = String.Format("{0}\\SCHEDULE\\WEEK.LSTX", TotalConfig.sDirWorkProject);
			writer = new StreamWriter(filename);
			if(writer == null)	return;

			for(i = 0; i < ScheduleLib.MAX_SCHEDULE_WEEK; i++) 
			{
				writer.Write("{0},", i);
				writer.Write("{0},", scheduleWeek[i].title);
				writer.WriteLine();
			}
			writer.Close();
		}

		

		private void label3_Click(object sender, System.EventArgs e)
		{
		
		}
	}
}

