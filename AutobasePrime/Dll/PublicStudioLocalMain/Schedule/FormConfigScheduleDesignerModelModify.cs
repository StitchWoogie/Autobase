using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using AutoLibLocal;
using PublicStudioLocalMain.Schedule;
using NetTools.OldDefine;
using DialogTag;
using DialogHoliday;

namespace PublicStudioLocalMain.Schedule
{
	/// <summary>
	/// Summary description for FormConfigScheduleDesignerModelModify.
	/// </summary>
	public class FormConfigScheduleDesignerModelModify : System.Windows.Forms.Form
    {
        private System.ComponentModel.Container components = null;

		public FormConfigScheduleDesignerModelModify()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigScheduleDesignerModelModify));
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.AccessibleDescription = null;
            this.textBoxDescription.AccessibleName = null;
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.BackgroundImage = null;
            this.textBoxDescription.Font = null;
            this.textBoxDescription.Name = "textBoxDescription";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
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
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
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
            // buttonOK
            // 
            this.buttonOK.AccessibleDescription = null;
            this.buttonOK.AccessibleName = null;
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.BackgroundImage = null;
            this.buttonOK.Font = null;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click_1);
            // 
            // FormConfigScheduleDesignerModelModify
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxTitle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigScheduleDesignerModelModify";
            this.Load += new System.EventHandler(this.FormConfigScheduleDesignerModelModify_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        private TextBox textBoxDescription;
        private Label label2;
        private TextBox textBoxTitle;
        private Label label1;
        private Button buttonCancel;
        private Button buttonOK;



        //ConfigModel

        public SCHEDULE_MODEL_STRUCT model = new SCHEDULE_MODEL_STRUCT();

        private void FormConfigScheduleDesignerModelModify_Load(object sender, EventArgs e)
        {
            this.textBoxTitle.Text = model.title;
            this.textBoxDescription.Text = model.description;
        }

        ArrayList arraySchedule = null;
        int indexSchedule = -1;

        public void Set(ArrayList array_schedule, int index)
        {
            arraySchedule = array_schedule;
            indexSchedule = index;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (this.textBoxTitle.Text.Length == 0)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("제목을 입력해야 합니다.", "입력 오류");
                else if (Tools.IsLangChinese())
                    MessageBox.Show("请输入标题。", "输入错误");
                else
                    MessageBox.Show("You must input the Title", "Input Error");
                return;
            }

            SCHEDULE_MODEL_STRUCT sms;

            for (int i = 0; i < arraySchedule.Count; i++)
            {
                if (i == indexSchedule) continue;

                sms = (SCHEDULE_MODEL_STRUCT)arraySchedule[i];
                if (String.Compare(sms.title, this.textBoxTitle.Text, true) == 0)
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("같은 모델명이 이미 추가되어 있습니다.", "모델명 중복");
                    else
                        MessageBox.Show("Same title already exists.", "Title exists");

                    return;
                }
            }

            model.title = this.textBoxTitle.Text;
            model.description = this.textBoxDescription.Text;

            //TotalConfig.AutoBaseMainListCtrlConfigSave(m_list, "DllDialogConfigModelOne");

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonOK_Click_1(object sender, EventArgs e)
        {
            this.textBoxTitle.Text = this.textBoxTitle.Text.Trim(); //모델이름 공백방지를 위하여 추가. hsjeong 25-03-11

            if (this.textBoxTitle.Text.Length == 0)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("제목을 입력해야 합니다.", "입력 오류");
                else if (Tools.IsLangChinese())
                    MessageBox.Show("请输入标题。", "输入错误");
                else
                    MessageBox.Show("You must input the Title", "Input Error");
                return;
            }

            SCHEDULE_MODEL_STRUCT sms;

            for (int i = 0; i < arraySchedule.Count; i++)
            {
                if (i == indexSchedule) continue;

                sms = (SCHEDULE_MODEL_STRUCT)arraySchedule[i];
                if (String.Compare(sms.title, this.textBoxTitle.Text, true) == 0)
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show("같은 모델명이 이미 추가되어 있습니다.", "모델명 중복");
                    else
                        MessageBox.Show("Same title already exists.", "Title exists");

                    return;
                }
            }

            model.title = this.textBoxTitle.Text;
            model.description = this.textBoxDescription.Text;

            //TotalConfig.AutoBaseMainListCtrlConfigSave(m_list, "DllDialogConfigModelOne");

            DialogResult = DialogResult.OK;
            Close();
        }

        
        

		

	}
}

