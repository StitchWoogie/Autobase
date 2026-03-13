using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using AutoLibLocal;

namespace DialogHoliday
{
	/// <summary>
	/// Summary description for FormConfigHolidayAdd.
	/// </summary>
	public class FormConfigHolidayAdd : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label1;
		public  System.Windows.Forms.TextBox textBoxTitle;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonType0;
		private System.Windows.Forms.RadioButton radioButtonType1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBoxMonth;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBoxDay;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox comboBoxWeek;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox comboBoxWeekday;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.RadioButton radioButtonSolar;
		private System.Windows.Forms.RadioButton radioButtonLunar;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public sbyte m_type;
		public string m_sYear = "매";
		public sbyte m_nMon;
		public sbyte m_nDay;
		public sbyte m_solar_lunar;
		public sbyte m_nWeek;
		private System.Windows.Forms.ComboBox m_comboYear;
		public sbyte m_nWeekDay;

		public FormConfigHolidayAdd()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigHolidayAdd));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonType0 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonLunar = new System.Windows.Forms.RadioButton();
            this.radioButtonSolar = new System.Windows.Forms.RadioButton();
            this.comboBoxWeekday = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxWeek = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBoxDay = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxMonth = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.m_comboYear = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBoxTitle
            // 
            resources.ApplyResources(this.textBoxTitle, "textBoxTitle");
            this.textBoxTitle.Name = "textBoxTitle";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonType1);
            this.groupBox1.Controls.Add(this.radioButtonType0);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonType1
            // 
            resources.ApplyResources(this.radioButtonType1, "radioButtonType1");
            this.radioButtonType1.Name = "radioButtonType1";
            this.radioButtonType1.CheckedChanged += new System.EventHandler(this.radioButtonType1_CheckedChanged);
            // 
            // radioButtonType0
            // 
            resources.ApplyResources(this.radioButtonType0, "radioButtonType0");
            this.radioButtonType0.Name = "radioButtonType0";
            this.radioButtonType0.CheckedChanged += new System.EventHandler(this.radioButtonType0_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonLunar);
            this.groupBox2.Controls.Add(this.radioButtonSolar);
            this.groupBox2.Controls.Add(this.comboBoxWeekday);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.comboBoxWeek);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.comboBoxDay);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.comboBoxMonth);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.m_comboYear);
            this.groupBox2.Controls.Add(this.label2);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonLunar
            // 
            resources.ApplyResources(this.radioButtonLunar, "radioButtonLunar");
            this.radioButtonLunar.Name = "radioButtonLunar";
            // 
            // radioButtonSolar
            // 
            resources.ApplyResources(this.radioButtonSolar, "radioButtonSolar");
            this.radioButtonSolar.Name = "radioButtonSolar";
            // 
            // comboBoxWeekday
            // 
            this.comboBoxWeekday.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxWeekday, "comboBoxWeekday");
            this.comboBoxWeekday.Name = "comboBoxWeekday";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // comboBoxWeek
            // 
            this.comboBoxWeek.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxWeek, "comboBoxWeek");
            this.comboBoxWeek.Name = "comboBoxWeek";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // comboBoxDay
            // 
            this.comboBoxDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxDay, "comboBoxDay");
            this.comboBoxDay.Name = "comboBoxDay";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // comboBoxMonth
            // 
            this.comboBoxMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxMonth, "comboBoxMonth");
            this.comboBoxMonth.Name = "comboBoxMonth";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // m_comboYear
            // 
            this.m_comboYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.m_comboYear, "m_comboYear");
            this.m_comboYear.Name = "m_comboYear";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // FormConfigHolidayAdd
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxTitle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigHolidayAdd";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigHolidayAdd_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        ArrayList arraySchedule = null;
        int indexSchedule = -1;

        public void Set(ArrayList array_schedule, int index)
        {
            arraySchedule = array_schedule;
            indexSchedule = index;
        }

		private void FormConfigHolidayAdd_Load(object sender, System.EventArgs e)
		{
			this.radioButtonSolar.Checked = (m_solar_lunar == 0);
			this.radioButtonLunar.Checked = (m_solar_lunar == 1);

			this.radioButtonType0.Checked = (m_type == 0);
			this.radioButtonType1.Checked = (m_type == 1);

			EnableDisable();

			DateTime t;
			string buf;
			int i;

            t = DateTimeServer.Now;

			if(Tools.IsLangKorean())
				m_comboYear.Items.Add("매");
			else if(Tools.IsLangJapanese())
				m_comboYear.Items.Add("每");
			else if(Tools.IsLangChinese())
				m_comboYear.Items.Add("按");
			else
				m_comboYear.Items.Add("Every");

			for(i = 0; i < 50; i++) 
			{
				buf = String.Format("{0}", i+t.Year);
				m_comboYear.Items.Add(buf);
			}	

			if(Tools.IsLangKorean())
				this.comboBoxMonth.Items.Add("매");
			else if(Tools.IsLangJapanese())
				this.comboBoxMonth.Items.Add("每");
			else if(Tools.IsLangChinese())
				this.comboBoxMonth.Items.Add("按");
			else
				this.comboBoxMonth.Items.Add("Every");

			for(i = 1; i <= 12; i++) 
			{
				buf = String.Format("{0}", i);
				comboBoxMonth.Items.Add(buf);
			}	

			for(i = 1; i <= 31; i++) 
			{
				buf = String.Format("{0}", i);
				comboBoxDay.Items.Add(buf);
			}	

			if(Tools.IsLangKorean())
				this.comboBoxDay.Items.Add("말");
			else if(Tools.IsLangJapanese())
				this.comboBoxDay.Items.Add("末");
			else if(Tools.IsLangChinese())
				this.comboBoxDay.Items.Add("最后一");
			else
				this.comboBoxDay.Items.Add("End");

			if(Tools.IsLangKorean()) 
			{
				this.comboBoxWeek.Items.Add("매");
				this.comboBoxWeek.Items.Add("첫째");
				this.comboBoxWeek.Items.Add("둘째");
				this.comboBoxWeek.Items.Add("셋째");
				this.comboBoxWeek.Items.Add("넷째");
				this.comboBoxWeek.Items.Add("다섯째");
				this.comboBoxWeek.Items.Add("여섯째");
			}
			else if(Tools.IsLangChinese()) 
			{
				this.comboBoxWeek.Items.Add("按");
				this.comboBoxWeek.Items.Add("第一");
				this.comboBoxWeek.Items.Add("第二");
				this.comboBoxWeek.Items.Add("第三");
				this.comboBoxWeek.Items.Add("第四");
				this.comboBoxWeek.Items.Add("第五");
				this.comboBoxWeek.Items.Add("第六");
			}
			else 
			{
				this.comboBoxWeek.Items.Add("Every");
				this.comboBoxWeek.Items.Add("1st");
				this.comboBoxWeek.Items.Add("2nd");
				this.comboBoxWeek.Items.Add("3th");
				this.comboBoxWeek.Items.Add("4th");
				this.comboBoxWeek.Items.Add("5th");
				this.comboBoxWeek.Items.Add("6th");
			}

			if(Tools.IsLangKorean()) 
			{
				this.comboBoxWeekday.Items.Add("일");
				this.comboBoxWeekday.Items.Add("월");
				this.comboBoxWeekday.Items.Add("화");
				this.comboBoxWeekday.Items.Add("수");
				this.comboBoxWeekday.Items.Add("목");
				this.comboBoxWeekday.Items.Add("금");
				this.comboBoxWeekday.Items.Add("토");
			}
			else if(Tools.IsLangJapanese()) 
			{
				this.comboBoxWeekday.Items.Add("日");
				this.comboBoxWeekday.Items.Add("月");
				this.comboBoxWeekday.Items.Add("火");
				this.comboBoxWeekday.Items.Add("水");
				this.comboBoxWeekday.Items.Add("木");
				this.comboBoxWeekday.Items.Add("金");
				this.comboBoxWeekday.Items.Add("土");
			}
			else if(Tools.IsLangChinese()) 
			{
				this.comboBoxWeekday.Items.Add("星期天");
				this.comboBoxWeekday.Items.Add("星期一");
				this.comboBoxWeekday.Items.Add("星期二");
				this.comboBoxWeekday.Items.Add("星期三");
				this.comboBoxWeekday.Items.Add("星期四");
				this.comboBoxWeekday.Items.Add("星期五");
				this.comboBoxWeekday.Items.Add("星期六");
			}
			else 
			{
				this.comboBoxWeekday.Items.Add("Sun");
				this.comboBoxWeekday.Items.Add("Mon");
				this.comboBoxWeekday.Items.Add("Tue");
				this.comboBoxWeekday.Items.Add("Wed");
				this.comboBoxWeekday.Items.Add("Thu");
				this.comboBoxWeekday.Items.Add("Fri");
				this.comboBoxWeekday.Items.Add("Sat");
			}


			this.m_comboYear.Text = this.m_sYear;
			this.comboBoxMonth.SelectedIndex = this.m_nMon;
			this.comboBoxDay.SelectedIndex = this.m_nDay;
			this.comboBoxWeek.SelectedIndex = this.m_nWeek;
			this.comboBoxWeekday.SelectedIndex = this.m_nWeekDay;
		}

		sbyte GetRadioType()
		{
			sbyte type;

			if(this.radioButtonType0.Checked)		type = 0;
			else if(this.radioButtonType1.Checked)	type = 1;
			else 									type = 2;

			return type;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
            if (TextBoxTool.CheckTextBoxLimitOver(textBoxTitle, 30)) return;

            if(this.radioButtonSolar.Checked)		this.m_solar_lunar = 0;
			else if(this.radioButtonLunar.Checked)	this.m_solar_lunar = 1;
			else									this.m_solar_lunar = 0; 

			m_type = GetRadioType();

            this.textBoxTitle.Text = this.textBoxTitle.Text.Trim(); // 제목 공백 방지를 위하여 추가. hsejong 25-03-11

			if(this.textBoxTitle.Text.Length == 0) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("제목을 입력해야 합니다.", "입력오류");
				else if(Tools.IsLangChinese()) 
					MessageBox.Show("请输入标题。", "输入错误");
				else 
					MessageBox.Show("You must input the Title.", "Error");	
				return;
			}
			m_sYear = this.m_comboYear.Text;
			m_nMon = (sbyte)this.comboBoxMonth.SelectedIndex;
			m_nDay = (sbyte)this.comboBoxDay.SelectedIndex;
			m_nWeek = (sbyte)this.comboBoxWeek.SelectedIndex;
			m_nWeekDay = (sbyte)this.comboBoxWeekday.SelectedIndex;

			if(m_sYear.Length == 0) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("년을 입력해야 합니다.", "입력오류");
				else 
					MessageBox.Show("You must select the Year.", "Error");

				return;
			}	
			if(m_nMon == -1) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("월을 입력해야 합니다.", "입력오류");
				else
					MessageBox.Show("You must select the Month.", "Error");
				return;
			}

			if(m_type == 0) 
			{
				if(m_nDay == -1) 
				{
					if(Tools.IsLangKorean())
						MessageBox.Show("일을 입력해야 합니다.", "입력오류");
					else
						MessageBox.Show("You must select the Day.", "Error");
					return;
				}
			}
			else 
			{
				if(m_nWeek == -1) 
				{
					if(Tools.IsLangKorean())
						MessageBox.Show("주을 입력해야 합니다.", "입력오류");
					else
						MessageBox.Show("You must select the Week.", "Error");
					return;
				}
				if(m_nWeekDay == -1) 
				{
					if(Tools.IsLangKorean())
						MessageBox.Show("요일을 입력해야 합니다.", "입력오류");
					else
						MessageBox.Show("You must select the WeekDay.", "Error");
					return;
				}
			}

            if (arraySchedule != null)
            {
                HOLIDAY_LIST hl;

                for (int i = 0; i < arraySchedule.Count; i++)
                {
                    if (i == indexSchedule) continue;

                    hl = (HOLIDAY_LIST)arraySchedule[i];

                    if (String.Compare(hl.title, this.textBoxTitle.Text, true) == 0)
                    {
                        if (Tools.IsLangKorean())
                            MessageBox.Show("같은 제목의 항목이 이미 추가되어 있습니다.", "제목 중복");
                        else
                            MessageBox.Show("Same title already exists.", "Title exists");

                        return;
                    }

                    // 같은 날짜가 존재하는 체크해도 좋을 듯 하다. 나중에 지원
                }
            }
	
			DialogResult = DialogResult.OK;
			Close();
		}

		void EnableDisable()
		{
			int type = GetRadioType();

			bool flag_day  = false;
			bool flag_week = false;

			if(type == 1) 
			{
				flag_week  = true;
			}
			else 
			{
				flag_day = true;
			}

			this.comboBoxDay.Enabled = flag_day;
			this.radioButtonLunar.Enabled = flag_day;
			this.radioButtonSolar.Enabled = flag_day;

			this.comboBoxWeek.Enabled = flag_week;
			this.comboBoxWeekday.Enabled = flag_week;
		}

		private void radioButtonType0_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		private void radioButtonType1_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}
	}
}


