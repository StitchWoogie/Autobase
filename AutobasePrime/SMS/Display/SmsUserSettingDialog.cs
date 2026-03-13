using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using SMS.SmsFunc;

namespace SMS.Display
{
	/// <summary>
	/// Summary description for SmsUserSettingDialog.
	/// </summary>
	public class SmsUserSettingDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox textBox_UserName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBox_PhoneNo;
		private System.Windows.Forms.Label label_PhoneNo;
		private System.Windows.Forms.CheckBox checkBox_Active;
		private System.Windows.Forms.Button button_PrevUser;
		private System.Windows.Forms.Button button_NextUser;
		private System.Windows.Forms.Button button_OK; 
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButton_AlarmPriority;
		private System.Windows.Forms.RadioButton radioButton_AlarmPlusTag;
		private System.Windows.Forms.TextBox textBox_DiTag;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button_TagFind;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.ListBox listBox_AlarmPriority;
		private System.Windows.Forms.Button button_SelectAll;
		private System.Windows.Forms.Button button_UnSelectAll;
		public	int		nUserPos = 0;
		private System.Windows.Forms.Label label_No;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton radioButtonSendType0;
        private RadioButton radioButtonSendType2;
        private TextBox textBox_ChatID;
        private Label label4;
		private System.Windows.Forms.RadioButton radioButtonSendType1;

		public SmsUserSettingDialog()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SmsUserSettingDialog));
            this.textBox_UserName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label_No = new System.Windows.Forms.Label();
            this.button_NextUser = new System.Windows.Forms.Button();
            this.button_PrevUser = new System.Windows.Forms.Button();
            this.checkBox_Active = new System.Windows.Forms.CheckBox();
            this.textBox_ChatID = new System.Windows.Forms.TextBox();
            this.textBox_PhoneNo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label_PhoneNo = new System.Windows.Forms.Label();
            this.button_OK = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton_AlarmPlusTag = new System.Windows.Forms.RadioButton();
            this.radioButton_AlarmPriority = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_DiTag = new System.Windows.Forms.TextBox();
            this.button_TagFind = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button_UnSelectAll = new System.Windows.Forms.Button();
            this.listBox_AlarmPriority = new System.Windows.Forms.ListBox();
            this.button_SelectAll = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButtonSendType2 = new System.Windows.Forms.RadioButton();
            this.radioButtonSendType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonSendType0 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_UserName
            // 
            this.textBox_UserName.AccessibleDescription = null;
            this.textBox_UserName.AccessibleName = null;
            resources.ApplyResources(this.textBox_UserName, "textBox_UserName");
            this.textBox_UserName.BackgroundImage = null;
            this.textBox_UserName.Font = null;
            this.textBox_UserName.Name = "textBox_UserName";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.label_No);
            this.groupBox1.Controls.Add(this.button_NextUser);
            this.groupBox1.Controls.Add(this.button_PrevUser);
            this.groupBox1.Controls.Add(this.checkBox_Active);
            this.groupBox1.Controls.Add(this.textBox_ChatID);
            this.groupBox1.Controls.Add(this.textBox_PhoneNo);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label_PhoneNo);
            this.groupBox1.Controls.Add(this.textBox_UserName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label_No
            // 
            this.label_No.AccessibleDescription = null;
            this.label_No.AccessibleName = null;
            resources.ApplyResources(this.label_No, "label_No");
            this.label_No.Font = null;
            this.label_No.Name = "label_No";
            // 
            // button_NextUser
            // 
            this.button_NextUser.AccessibleDescription = null;
            this.button_NextUser.AccessibleName = null;
            resources.ApplyResources(this.button_NextUser, "button_NextUser");
            this.button_NextUser.BackgroundImage = null;
            this.button_NextUser.Font = null;
            this.button_NextUser.Name = "button_NextUser";
            this.button_NextUser.Click += new System.EventHandler(this.button_NextUser_Click);
            // 
            // button_PrevUser
            // 
            this.button_PrevUser.AccessibleDescription = null;
            this.button_PrevUser.AccessibleName = null;
            resources.ApplyResources(this.button_PrevUser, "button_PrevUser");
            this.button_PrevUser.BackgroundImage = null;
            this.button_PrevUser.Font = null;
            this.button_PrevUser.Name = "button_PrevUser";
            this.button_PrevUser.Click += new System.EventHandler(this.button_PrevUser_Click);
            // 
            // checkBox_Active
            // 
            this.checkBox_Active.AccessibleDescription = null;
            this.checkBox_Active.AccessibleName = null;
            resources.ApplyResources(this.checkBox_Active, "checkBox_Active");
            this.checkBox_Active.BackgroundImage = null;
            this.checkBox_Active.Font = null;
            this.checkBox_Active.Name = "checkBox_Active";
            this.checkBox_Active.Click += new System.EventHandler(this.checkBox_Active_Click);
            // 
            // textBox_ChatID
            // 
            this.textBox_ChatID.AccessibleDescription = null;
            this.textBox_ChatID.AccessibleName = null;
            resources.ApplyResources(this.textBox_ChatID, "textBox_ChatID");
            this.textBox_ChatID.BackgroundImage = null;
            this.textBox_ChatID.Font = null;
            this.textBox_ChatID.Name = "textBox_ChatID";
            // 
            // textBox_PhoneNo
            // 
            this.textBox_PhoneNo.AccessibleDescription = null;
            this.textBox_PhoneNo.AccessibleName = null;
            resources.ApplyResources(this.textBox_PhoneNo, "textBox_PhoneNo");
            this.textBox_PhoneNo.BackgroundImage = null;
            this.textBox_PhoneNo.Font = null;
            this.textBox_PhoneNo.Name = "textBox_PhoneNo";
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // label_PhoneNo
            // 
            this.label_PhoneNo.AccessibleDescription = null;
            this.label_PhoneNo.AccessibleName = null;
            resources.ApplyResources(this.label_PhoneNo, "label_PhoneNo");
            this.label_PhoneNo.Font = null;
            this.label_PhoneNo.Name = "label_PhoneNo";
            // 
            // button_OK
            // 
            this.button_OK.AccessibleDescription = null;
            this.button_OK.AccessibleName = null;
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.BackgroundImage = null;
            this.button_OK.Font = null;
            this.button_OK.Name = "button_OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.AccessibleDescription = null;
            this.button_Cancel.AccessibleName = null;
            resources.ApplyResources(this.button_Cancel, "button_Cancel");
            this.button_Cancel.BackgroundImage = null;
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Font = null;
            this.button_Cancel.Name = "button_Cancel";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.radioButton_AlarmPlusTag);
            this.groupBox2.Controls.Add(this.radioButton_AlarmPriority);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textBox_DiTag);
            this.groupBox2.Controls.Add(this.button_TagFind);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButton_AlarmPlusTag
            // 
            this.radioButton_AlarmPlusTag.AccessibleDescription = null;
            this.radioButton_AlarmPlusTag.AccessibleName = null;
            resources.ApplyResources(this.radioButton_AlarmPlusTag, "radioButton_AlarmPlusTag");
            this.radioButton_AlarmPlusTag.BackgroundImage = null;
            this.radioButton_AlarmPlusTag.Font = null;
            this.radioButton_AlarmPlusTag.Name = "radioButton_AlarmPlusTag";
            this.radioButton_AlarmPlusTag.Click += new System.EventHandler(this.radioButton_AlarmPlusTag_Click);
            // 
            // radioButton_AlarmPriority
            // 
            this.radioButton_AlarmPriority.AccessibleDescription = null;
            this.radioButton_AlarmPriority.AccessibleName = null;
            resources.ApplyResources(this.radioButton_AlarmPriority, "radioButton_AlarmPriority");
            this.radioButton_AlarmPriority.BackgroundImage = null;
            this.radioButton_AlarmPriority.Font = null;
            this.radioButton_AlarmPriority.Name = "radioButton_AlarmPriority";
            this.radioButton_AlarmPriority.Click += new System.EventHandler(this.radioButton_AlarmPriority_Click);
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // textBox_DiTag
            // 
            this.textBox_DiTag.AccessibleDescription = null;
            this.textBox_DiTag.AccessibleName = null;
            resources.ApplyResources(this.textBox_DiTag, "textBox_DiTag");
            this.textBox_DiTag.BackgroundImage = null;
            this.textBox_DiTag.Font = null;
            this.textBox_DiTag.Name = "textBox_DiTag";
            // 
            // button_TagFind
            // 
            this.button_TagFind.AccessibleDescription = null;
            this.button_TagFind.AccessibleName = null;
            resources.ApplyResources(this.button_TagFind, "button_TagFind");
            this.button_TagFind.BackgroundImage = null;
            this.button_TagFind.Font = null;
            this.button_TagFind.Name = "button_TagFind";
            this.button_TagFind.Click += new System.EventHandler(this.button_TagFind_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.button_UnSelectAll);
            this.groupBox3.Controls.Add(this.listBox_AlarmPriority);
            this.groupBox3.Controls.Add(this.button_SelectAll);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // button_UnSelectAll
            // 
            this.button_UnSelectAll.AccessibleDescription = null;
            this.button_UnSelectAll.AccessibleName = null;
            resources.ApplyResources(this.button_UnSelectAll, "button_UnSelectAll");
            this.button_UnSelectAll.BackgroundImage = null;
            this.button_UnSelectAll.Font = null;
            this.button_UnSelectAll.Name = "button_UnSelectAll";
            this.button_UnSelectAll.Click += new System.EventHandler(this.button_UnSelectAll_Click);
            // 
            // listBox_AlarmPriority
            // 
            this.listBox_AlarmPriority.AccessibleDescription = null;
            this.listBox_AlarmPriority.AccessibleName = null;
            resources.ApplyResources(this.listBox_AlarmPriority, "listBox_AlarmPriority");
            this.listBox_AlarmPriority.BackgroundImage = null;
            this.listBox_AlarmPriority.Font = null;
            this.listBox_AlarmPriority.MultiColumn = true;
            this.listBox_AlarmPriority.Name = "listBox_AlarmPriority";
            this.listBox_AlarmPriority.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            // 
            // button_SelectAll
            // 
            this.button_SelectAll.AccessibleDescription = null;
            this.button_SelectAll.AccessibleName = null;
            resources.ApplyResources(this.button_SelectAll, "button_SelectAll");
            this.button_SelectAll.BackgroundImage = null;
            this.button_SelectAll.Font = null;
            this.button_SelectAll.Name = "button_SelectAll";
            this.button_SelectAll.Click += new System.EventHandler(this.button_SelectAll_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.AccessibleDescription = null;
            this.groupBox4.AccessibleName = null;
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.BackgroundImage = null;
            this.groupBox4.Controls.Add(this.radioButtonSendType2);
            this.groupBox4.Controls.Add(this.radioButtonSendType1);
            this.groupBox4.Controls.Add(this.radioButtonSendType0);
            this.groupBox4.Font = null;
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // radioButtonSendType2
            // 
            this.radioButtonSendType2.AccessibleDescription = null;
            this.radioButtonSendType2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonSendType2, "radioButtonSendType2");
            this.radioButtonSendType2.BackgroundImage = null;
            this.radioButtonSendType2.Font = null;
            this.radioButtonSendType2.Name = "radioButtonSendType2";
            this.radioButtonSendType2.CheckedChanged += new System.EventHandler(this.radioButtonSendType2_CheckedChanged);
            // 
            // radioButtonSendType1
            // 
            this.radioButtonSendType1.AccessibleDescription = null;
            this.radioButtonSendType1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonSendType1, "radioButtonSendType1");
            this.radioButtonSendType1.BackgroundImage = null;
            this.radioButtonSendType1.Font = null;
            this.radioButtonSendType1.Name = "radioButtonSendType1";
            // 
            // radioButtonSendType0
            // 
            this.radioButtonSendType0.AccessibleDescription = null;
            this.radioButtonSendType0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonSendType0, "radioButtonSendType0");
            this.radioButtonSendType0.BackgroundImage = null;
            this.radioButtonSendType0.Font = null;
            this.radioButtonSendType0.Name = "radioButtonSendType0";
            // 
            // SmsUserSettingDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SmsUserSettingDialog";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.SmsUserSettingDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void SmsUserSettingDialog_Load(object sender, System.EventArgs e)
		{
			for(int i = 0; i < 1000; i++) 
			{
				this.listBox_AlarmPriority.Items.Add(String.Format("{0,3:d03}", i));
			}
			setInitValue();
		}

		void setInitValue()
		{
			this.label_No.Text = String.Format("( No = {0} )", nUserPos+1);			
			if(SmsBasic.arrUserSetting.Count <= nUserPos) return;
			smsUserConfig	user;
			user = (smsUserConfig)SmsBasic.arrUserSetting[nUserPos];
			
			this.checkBox_Active.Checked = user.bActive;
			this.textBox_UserName.Text = user.userName;
			this.textBox_PhoneNo.Text = user.phoneNo;
			this.textBox_DiTag.Text = user.diTag;
			if(user.nSendCondition == 0) radioButton_AlarmPriority.Checked = true;
			else						 radioButton_AlarmPlusTag.Checked = true;
			for(int i = 0; i < 1000; i++)
			{
				listBox_AlarmPriority.SetSelected(i, user.bPriority[i]);
			}

			this.radioButtonSendType0.Checked = (user.nSmsType == 0);
			this.radioButtonSendType1.Checked = (user.nSmsType == 1);
            this.radioButtonSendType2.Checked = (user.nSmsType == 2);

            //20241111 PSU 텔레그램
            this.textBox_ChatID.Text = user.ChatID;

			EnableDisableAllItem();
		}

		void EnableDisableAllItem()
		{
			bool		flag = this.checkBox_Active.Checked;

			//this.button_PrevUser.Enabled = flag;
			//this.button_NextUser.Enabled = flag;
			this.button_SelectAll.Enabled = flag;
			this.button_UnSelectAll.Enabled = flag;
			this.button_TagFind.Enabled = flag;
			this.radioButton_AlarmPlusTag.Enabled = flag;
			this.radioButton_AlarmPriority.Enabled = flag;
			this.textBox_DiTag.Enabled = flag;
			this.textBox_PhoneNo.Enabled = flag;
			this.textBox_UserName.Enabled = flag;
			this.listBox_AlarmPriority.Enabled = flag;

			this.radioButtonSendType0.Enabled = flag;
			//this.radioButtonSendType1.Enabled = flag;
            this.radioButtonSendType2.Enabled = flag;

			if(flag) EnableDisableTagItem();

            //20241111 PSU 텔레그램
            textBox_ChatID.Enabled = flag;
            if (radioButtonSendType2.Checked)
            {
                label_PhoneNo.Text = "BotToken :";
            }
            else
            {
                textBox_ChatID.Enabled = false;
                label_PhoneNo.Text = "Phone No :";
            }
		}

		
		private void checkBox_Active_Click(object sender, System.EventArgs e)
		{
			EnableDisableAllItem();
		}

		void EnableDisableTagItem()
		{
			bool		flag = this.radioButton_AlarmPlusTag.Checked;

			this.button_TagFind.Enabled = flag;
			this.textBox_DiTag.Enabled = flag;	
		}

		private void radioButton_AlarmPlusTag_Click(object sender, System.EventArgs e)
		{
			EnableDisableTagItem();
		}

		private void radioButton_AlarmPriority_Click(object sender, System.EventArgs e)
		{
			EnableDisableTagItem();
		}

		private void button_SelectAll_Click(object sender, System.EventArgs e)
		{
			for(int i = 0; i < 1000; i++)
				this.listBox_AlarmPriority.SetSelected(i, true);
		}

		private void button_UnSelectAll_Click(object sender, System.EventArgs e)
		{
			for(int i = 0; i < 1000; i++)
				this.listBox_AlarmPriority.SetSelected(i, false);
		}

		bool checkBlank()
		{
			if(this.textBox_UserName.Text.Length <= 0) 
			{
				MessageBox.Show("User Name is Blank. Please Input User Name.");
				return true;
			}
			if(this.textBox_PhoneNo.Text.Length <= 0) 
			{
				MessageBox.Show("Phone number is Blank. Please Input Phone Number.");
				return true;
			}
			if(this.radioButton_AlarmPlusTag.Checked && this.textBox_DiTag.Text.Length <= 0) 
			{
				MessageBox.Show("DI Tag Name is Blank. Please Input DI Tag Name.");
				return true;
			}
			return false;
		}

		void getCurrentDialogDataAndSave()
		{
			if(checkBlank()) return;

			smsUserConfig	user;			
			user = (smsUserConfig)SmsBasic.arrUserSetting[nUserPos];

			
			user.bActive = this.checkBox_Active.Checked;
			user.userName = this.textBox_UserName.Text;
			user.phoneNo = this.textBox_PhoneNo.Text;
			user.diTag = this.textBox_DiTag.Text;

            //20241111 PSU 텔레그램
            user.ChatID = this.textBox_ChatID.Text;
            
            if(radioButton_AlarmPriority.Checked) user.nSendCondition = 0;
			else								  user.nSendCondition = 1;
			for(int i = 0; i < 1000; i++)
			{
				user.bPriority[i] = listBox_AlarmPriority.GetSelected(i);
			}

			if(this.radioButtonSendType0.Checked)		user.nSmsType = 0;
			else if(this.radioButtonSendType1.Checked)	user.nSmsType = 1;
            else if (this.radioButtonSendType2.Checked) user.nSmsType = 2;
			else										user.nSmsType = 0;

			SmsFileDataLoadSave.SaveOneUserData(nUserPos);

			if(SmsMainDisplayForm.formThis != null)					// 사용자 표시 MDI 화면을 다시 그린다
                SmsMainDisplayForm.formThis.CallByThreadReDrawUserInfo(nUserPos);
		}

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			if(SmsBasic.arrUserSetting.Count <= nUserPos) 
			{
				Close();
				return;
			}
			if(checkBlank()) return;
			getCurrentDialogDataAndSave();
			this.DialogResult = DialogResult.OK;
			Close();
		}

		private void button_TagFind_Click(object sender, System.EventArgs e)
		{
			string	sTag, sDes;
			if(DialogTag.SelectTag.SelectDi(this, out sTag, out sDes) == DialogResult.OK) 
			{
				this.textBox_DiTag.Text = sTag;
			}
		}

		void reLoadUserData(int pos)
		{
			if(checkBlank()) return;
			getCurrentDialogDataAndSave();			
			nUserPos = pos;
			setInitValue();
		}

		void changePrevNextUser(bool bNext)
		{
			smsUserConfig	user;
			int				pos;

			for(int i = 1; i < SmsBasic.arrUserSetting.Count; i++)
			{
				if(bNext) pos = (nUserPos+i) % 256;
				else	  pos = (nUserPos+256-i) % 256;
				if(pos >= SmsBasic.arrUserSetting.Count) continue;
				user = (smsUserConfig)SmsBasic.arrUserSetting[pos];
				if(user.bActive == false) continue;
				reLoadUserData(pos);
				break;
			}
		}

		private void button_PrevUser_Click(object sender, System.EventArgs e)
		{
			changePrevNextUser(false);
		}

		private void button_NextUser_Click(object sender, System.EventArgs e)
		{
			changePrevNextUser(true);
		}

        //20241111 PSU 텔레그램
        private void radioButtonSendType2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonSendType2.Checked)
            {
                textBox_ChatID.Enabled = true;
                label_PhoneNo.Text = "BotToken :";

            }
            else
            {
                textBox_ChatID.Enabled = false;
                label_PhoneNo.Text = "Phone No :";
            }
        }
		
		
	}
}
