using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using AutoLib;
using DialogHoliday;
using PublicStudioLocalMain.Schedule;
using AutoLibLocal;

namespace PublicStudioLocalMain.Schedule
{
	/// <summary>
	/// Summary description for FormConfigScheduleAdditionalAdd.
	/// </summary>
	public class FormConfigScheduleAdditionalAdd : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxTitle;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonType0;
		private System.Windows.Forms.RadioButton radioButtonType1;
		private System.Windows.Forms.RadioButton radioButtonType2;
		private System.Windows.Forms.RadioButton radioButtonType3;
		private System.Windows.Forms.RadioButton radioButtonType4;
		private System.Windows.Forms.Button buttonSpecial;
		private System.Windows.Forms.Button buttonHoliday;
		private System.Windows.Forms.TextBox textBoxUserType;
		private System.Windows.Forms.Button buttonUserType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBoxModel;
		private System.Windows.Forms.Button buttonModelConfig;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigScheduleAdditionalAdd()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigScheduleAdditionalAdd));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonUserType = new System.Windows.Forms.Button();
            this.textBoxUserType = new System.Windows.Forms.TextBox();
            this.buttonHoliday = new System.Windows.Forms.Button();
            this.buttonSpecial = new System.Windows.Forms.Button();
            this.radioButtonType4 = new System.Windows.Forms.RadioButton();
            this.radioButtonType3 = new System.Windows.Forms.RadioButton();
            this.radioButtonType2 = new System.Windows.Forms.RadioButton();
            this.radioButtonType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonType0 = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxModel = new System.Windows.Forms.ComboBox();
            this.buttonModelConfig = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.AccessibleDescription = null;
            this.buttonOK.AccessibleName = null;
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.BackgroundImage = null;
            this.buttonOK.Font = null;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.AccessibleDescription = null;
            this.buttonCancel.AccessibleName = null;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.BackgroundImage = null;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = null;
            this.buttonCancel.Name = "buttonCancel";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.AccessibleDescription = null;
            this.textBoxTitle.AccessibleName = null;
            resources.ApplyResources(this.textBoxTitle, "textBoxTitle");
            this.textBoxTitle.BackgroundImage = null;
            this.textBoxTitle.Font = null;
            this.textBoxTitle.Name = "textBoxTitle";
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.buttonUserType);
            this.groupBox1.Controls.Add(this.textBoxUserType);
            this.groupBox1.Controls.Add(this.buttonHoliday);
            this.groupBox1.Controls.Add(this.buttonSpecial);
            this.groupBox1.Controls.Add(this.radioButtonType4);
            this.groupBox1.Controls.Add(this.radioButtonType3);
            this.groupBox1.Controls.Add(this.radioButtonType2);
            this.groupBox1.Controls.Add(this.radioButtonType1);
            this.groupBox1.Controls.Add(this.radioButtonType0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // buttonUserType
            // 
            this.buttonUserType.AccessibleDescription = null;
            this.buttonUserType.AccessibleName = null;
            resources.ApplyResources(this.buttonUserType, "buttonUserType");
            this.buttonUserType.BackgroundImage = null;
            this.buttonUserType.Font = null;
            this.buttonUserType.Name = "buttonUserType";
            this.buttonUserType.Click += new System.EventHandler(this.buttonUserType_Click);
            // 
            // textBoxUserType
            // 
            this.textBoxUserType.AccessibleDescription = null;
            this.textBoxUserType.AccessibleName = null;
            resources.ApplyResources(this.textBoxUserType, "textBoxUserType");
            this.textBoxUserType.BackgroundImage = null;
            this.textBoxUserType.Font = null;
            this.textBoxUserType.Name = "textBoxUserType";
            this.textBoxUserType.ReadOnly = true;
            // 
            // buttonHoliday
            // 
            this.buttonHoliday.AccessibleDescription = null;
            this.buttonHoliday.AccessibleName = null;
            resources.ApplyResources(this.buttonHoliday, "buttonHoliday");
            this.buttonHoliday.BackgroundImage = null;
            this.buttonHoliday.Font = null;
            this.buttonHoliday.Name = "buttonHoliday";
            this.buttonHoliday.Click += new System.EventHandler(this.buttonHoliday_Click);
            // 
            // buttonSpecial
            // 
            this.buttonSpecial.AccessibleDescription = null;
            this.buttonSpecial.AccessibleName = null;
            resources.ApplyResources(this.buttonSpecial, "buttonSpecial");
            this.buttonSpecial.BackgroundImage = null;
            this.buttonSpecial.Font = null;
            this.buttonSpecial.Name = "buttonSpecial";
            this.buttonSpecial.Click += new System.EventHandler(this.buttonSpecial_Click);
            // 
            // radioButtonType4
            // 
            this.radioButtonType4.AccessibleDescription = null;
            this.radioButtonType4.AccessibleName = null;
            resources.ApplyResources(this.radioButtonType4, "radioButtonType4");
            this.radioButtonType4.BackgroundImage = null;
            this.radioButtonType4.Font = null;
            this.radioButtonType4.Name = "radioButtonType4";
            this.radioButtonType4.CheckedChanged += new System.EventHandler(this.radioButtonType4_CheckedChanged);
            // 
            // radioButtonType3
            // 
            this.radioButtonType3.AccessibleDescription = null;
            this.radioButtonType3.AccessibleName = null;
            resources.ApplyResources(this.radioButtonType3, "radioButtonType3");
            this.radioButtonType3.BackgroundImage = null;
            this.radioButtonType3.Font = null;
            this.radioButtonType3.Name = "radioButtonType3";
            this.radioButtonType3.CheckedChanged += new System.EventHandler(this.radioButtonType3_CheckedChanged);
            // 
            // radioButtonType2
            // 
            this.radioButtonType2.AccessibleDescription = null;
            this.radioButtonType2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonType2, "radioButtonType2");
            this.radioButtonType2.BackgroundImage = null;
            this.radioButtonType2.Font = null;
            this.radioButtonType2.Name = "radioButtonType2";
            this.radioButtonType2.CheckedChanged += new System.EventHandler(this.radioButtonType2_CheckedChanged);
            // 
            // radioButtonType1
            // 
            this.radioButtonType1.AccessibleDescription = null;
            this.radioButtonType1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonType1, "radioButtonType1");
            this.radioButtonType1.BackgroundImage = null;
            this.radioButtonType1.Font = null;
            this.radioButtonType1.Name = "radioButtonType1";
            this.radioButtonType1.CheckedChanged += new System.EventHandler(this.radioButtonType1_CheckedChanged);
            // 
            // radioButtonType0
            // 
            this.radioButtonType0.AccessibleDescription = null;
            this.radioButtonType0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonType0, "radioButtonType0");
            this.radioButtonType0.BackgroundImage = null;
            this.radioButtonType0.Font = null;
            this.radioButtonType0.Name = "radioButtonType0";
            this.radioButtonType0.CheckedChanged += new System.EventHandler(this.radioButtonType0_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // comboBoxModel
            // 
            this.comboBoxModel.AccessibleDescription = null;
            this.comboBoxModel.AccessibleName = null;
            resources.ApplyResources(this.comboBoxModel, "comboBoxModel");
            this.comboBoxModel.BackgroundImage = null;
            this.comboBoxModel.Font = null;
            this.comboBoxModel.Name = "comboBoxModel";
            // 
            // buttonModelConfig
            // 
            this.buttonModelConfig.AccessibleDescription = null;
            this.buttonModelConfig.AccessibleName = null;
            resources.ApplyResources(this.buttonModelConfig, "buttonModelConfig");
            this.buttonModelConfig.BackgroundImage = null;
            this.buttonModelConfig.Font = null;
            this.buttonModelConfig.Name = "buttonModelConfig";
            this.buttonModelConfig.Click += new System.EventHandler(this.buttonModelConfig_Click);
            // 
            // FormConfigScheduleAdditionalAdd
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonModelConfig);
            this.Controls.Add(this.comboBoxModel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxTitle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigScheduleAdditionalAdd";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigScheduleAdditionalAdd_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public SCHEDULE_ADDITIONAL additional = new SCHEDULE_ADDITIONAL();

        ArrayList blockModel = new ArrayList();

		private void FormConfigScheduleAdditionalAdd_Load(object sender, System.EventArgs e)
		{
            blockModel = ScheduleLib.ModelLoad();

			this.radioButtonType0.Checked = (additional.type == 0);
			this.radioButtonType1.Checked = (additional.type == 1);
			this.radioButtonType2.Checked = (additional.type == 2);
			this.radioButtonType3.Checked = (additional.type == 3);
			this.radioButtonType4.Checked = (additional.type == 4);

			FillModelBox();

			string buf;
			Holiday.MakeString(out buf, additional.user);
			this.textBoxUserType.Text = buf;
			this.textBoxTitle.Text = additional.title;

			this.comboBoxModel.Text = additional.model;

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                this.radioButtonType4.Visible = false;
            }

			EnableDisable();
		}

        ArrayList arraySchedule = null;
        int indexSchedule = -1;

        public void Set(ArrayList array_schedule, int index)
        {
            arraySchedule = array_schedule;
            indexSchedule = index;
        }

		private void buttonUserType_Click(object sender, System.EventArgs e)
		{
			additional.user.title = "Title";

			string title;
			if(Tools.IsLangKorean())		title = "사용자 지정 스케쥴 수정";
			else if(Tools.IsLangChinese())	title = "修改自定义计划表";
			else							title = "User Defined Schedule Modify";

			additional.user.title = title;

			if(FormConfigHoliday.ConfigHolidayModify(this, title, additional.user, null, -1)) 
			{
				string text;
				Holiday.MakeString(out text, additional.user);
				this.textBoxUserType.Text = text;
			}	
		}

		private void buttonHoliday_Click(object sender, System.EventArgs e)
		{
			if(Holiday.ConfigHoliday(this, ref Holiday.arrayHoliday)) 
			{
				//FormSchedule.OnScheduleStructChanged();
			}	
		}

		private void buttonSpecial_Click(object sender, System.EventArgs e)
		{
			if(Holiday.ConfigSpecialDay(this, ref Holiday.arraySpecialDay)) 
			{
				//FormSchedule.OnScheduleStructChanged();
			}	
		}

		int GetRadioType()
		{

			int type;
			if(this.radioButtonType0.Checked)		type = 0;
			else if(this.radioButtonType1.Checked)	type = 1;
			else if(this.radioButtonType2.Checked)	type = 2;
            else if (this.radioButtonType3.Checked) type = 3;
			else 									type = 0;

			return type;
		}

		private void buttonOK_Click(object sender, System.EventArgs e) 
		{
            if (TextBoxTool.CheckTextBoxLimitOver(textBoxTitle, TextBoxLimit.MAX_ScheduleTitle)) return;

            this.textBoxTitle.Text = this.textBoxTitle.Text.Trim(); // 추가스케쥴 제목 공백 방지를 위하여 추가. hsjeong 25-03-11

			if(this.textBoxTitle.Text.Length == 0) 
			{
				if(Tools.IsLangKorean()) 
					MessageBox.Show("제목을 입력해야 합니다..", "입력 오류");
				else if(Tools.IsLangChinese()) 
					MessageBox.Show("请输入标题。", "输入错误");
				else
					MessageBox.Show("You must input the Title.", "Input Error");
				return;
			}

			additional.model = this.comboBoxModel.Text;

			additional.type = GetRadioType();

			additional.title = this.textBoxTitle.Text;

			if(additional.model.Length == 0) 
			{
				if(Tools.IsLangKorean())
					MessageBox.Show("모델을 선택해야 합니다.", "모델선택 오류");
				else if(Tools.IsLangChinese())
					MessageBox.Show("请选1个模型。", "选择错误");
				else
					MessageBox.Show("You must select the Model.", "Selection Error");
				return;
			}

            SCHEDULE_ADDITIONAL sa;

            for (int i = 0; i < arraySchedule.Count; i++)
            {
                if (i == indexSchedule) continue;

                sa = (SCHEDULE_ADDITIONAL)arraySchedule[i];
                if (String.Compare(sa.title, this.textBoxTitle.Text, true) == 0)
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("같은 제목의 스케쥴이 이미 추가되어 있습니다.", "제목 중복");
                    else
                        MessageBox.Show("Same title already exists.", "Title exists");

                    return;
                }
            }

			DialogResult = DialogResult.OK;
			Close();
		}

		void EnableDisable()
		{
			int type = GetRadioType();

			bool flag = false;
			if(type == 2)	flag = true;
			else			flag = false;

			//this.textBoxUserType.Enabled = flag;
			this.buttonUserType.Enabled = flag;
		}

		private void radioButtonType0_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		private void radioButtonType1_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		private void radioButtonType2_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		private void radioButtonType3_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		private void radioButtonType4_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		void FillModelBox()
		{
			int l;
			SCHEDULE_MODEL_STRUCT model;

			this.comboBoxModel.Items.Clear();

			for(l = 0; l < blockModel.Count; l++) 
			{
				model = (SCHEDULE_MODEL_STRUCT)blockModel[l];
				this.comboBoxModel.Items.Add(model.title);
			}
		}

		private void buttonModelConfig_Click(object sender, System.EventArgs e)
		{
            FormConfigModel dialog = new FormConfigModel();
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				//FormSchedule.OnScheduleStructChanged();
                blockModel = ScheduleLib.ModelLoad();
                string title;
				title = this.comboBoxModel.Text;
				FillModelBox();
                //this.comboBoxModel.SelectedText = title; 
                // selected text 로 했을 때 button을 누르자마자 selectedtext == null 이 되면서
                // 기존 선택 text(title)가 1번 더 붙어서 똑같은 이름이 2번 써지는 버그 발생하여서 수정함. hsjeong 25-03-12

                this.comboBoxModel.Text = title;
			}
		}

	}
}


