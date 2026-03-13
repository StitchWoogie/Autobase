using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLib;
using NetTools;
using AutoLibLocal;
using System.Security.Principal;

namespace BasicScreen
{
	/// <summary>
	/// Summary description for ViewAnalogInputHandInputDlg.
	/// </summary>
	public class ViewAnalogInputHandInputDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_CANCEL;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.GroupBox groupBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.CheckBox checkBox_HandInput;
		private System.Windows.Forms.TextBox textBox_InputValue;

		TagAiClass	ai;
		bool		bOldHandInputFlag;
		public		bool	bElementChanged = false;

		public ViewAnalogInputHandInputDlg(TagAiClass tag)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			ai = tag;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewAnalogInputHandInputDlg));
            this.button_CANCEL = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.checkBox_HandInput = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_InputValue = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
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
            // checkBox_HandInput
            // 
            resources.ApplyResources(this.checkBox_HandInput, "checkBox_HandInput");
            this.checkBox_HandInput.Name = "checkBox_HandInput";
            this.checkBox_HandInput.CheckedChanged += new System.EventHandler(this.checkBox_HandInput_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_InputValue);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBox_InputValue
            // 
            resources.ApplyResources(this.textBox_InputValue, "textBox_InputValue");
            this.textBox_InputValue.Name = "textBox_InputValue";
            // 
            // ViewAnalogInputHandInputDlg
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBox_HandInput);
            this.Controls.Add(this.button_CANCEL);
            this.Controls.Add(this.button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewAnalogInputHandInputDlg";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ViewAnalogInputHandInputDlg_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void ViewAnalogInputHandInputDlg_Load(object sender, System.EventArgs e)
		{
			if((ai.wProtectFlags & EnumProtectFlag.SCAN) != 0) this.checkBox_HandInput.Checked = true;
			else this.checkBox_HandInput.Checked = false;
			bOldHandInputFlag = this.checkBox_HandInput.Checked;		// 이전의 수동기입 상태
			this.textBox_InputValue.Text = ai.curr.ToString();
			EnableDisable();
		}

		void EnableDisable()
		{
			if(this.checkBox_HandInput.Checked) this.textBox_InputValue.Enabled = true;
			else this.textBox_InputValue.Enabled = false;
            if (!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_HAND_INPUT)) this.button_OK.Enabled = false;
		}

		private void checkBox_HandInput_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		void makeAndSetValueChangeEvent()
		{
            /*
			COMM_EVENT_STRUCT	tagEvent = new COMM_EVENT_STRUCT();
			tagEvent.tag = ai.tag;
			tagEvent.tag_type = EnumTagType.AI;
			tagEvent.message_type = 0;			// 0 - 현재치, 1 - 적산치.
			tagEvent.tp = ai;*/
			SharedViewMain.EventGoTagChanged(ai);
		}

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			string			msg;
			float			val = 0.0F;

			try 
			{
				val = ConvertTool.ToSingle(this.textBox_InputValue.Text);
			}
			catch {}


			if(!bOldHandInputFlag && this.checkBox_HandInput.Checked) 
			{
				if(Tools.IsLangKorean()) msg = string.Format("수동 기입 모드 작동.  설정값={0,3:F2}", val);
				else msg = string.Format("Hand Input Mode Start.  Value={0,3:F2}", val);
				SharedViewMain.AlarmDisplayAI(ai, msg, EnumAlarmType.HAND_INPUT, true, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);
			}
			if(bOldHandInputFlag && !this.checkBox_HandInput.Checked) 
			{
				if(Tools.IsLangKorean()) msg = string.Format("수동 기입 모드 해제.  해제값={0,3:F2}", ai.curr);
				else msg = string.Format("Hand Input Mode Stop.  Value={0,3:F2}", ai.curr);
                SharedViewMain.AlarmDisplayAI(ai, msg, EnumAlarmType.HAND_INPUT, false, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);
			}
			if(this.checkBox_HandInput.Checked) 
			{
				if(ai.curr != val) 
				{
					if(Tools.IsLangKorean()) msg = string.Format("수동 기입값 변경 {0,3:F2} -> {1,3:F2}", ai.curr, val);
                    else msg = string.Format("Hand Input Value Changed {0,3:F2} -> {1,3:F2}", ai.curr, val);
                    SharedViewMain.AlarmDisplayAI(ai, msg, EnumAlarmType.HAND_INPUT, true, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);
					ai.curr = val;
					bElementChanged = true;
					makeAndSetValueChangeEvent();
				}
			}
			
			if(bOldHandInputFlag != this.checkBox_HandInput.Checked)
			{
				if(this.checkBox_HandInput.Checked)	ai.wProtectFlags |= EnumProtectFlag.SCAN;
				else ai.wProtectFlags &= (EnumProtectFlag)(0xFFFF-(ushort)EnumProtectFlag.SCAN);
				bElementChanged = true;
				SharedViewMain.EventGoTagPropertyChanged((TagPublicClass)ai);
				LibComNetServer.SendCommandProtectFlagChange(ai.tag, (int)ai.wProtectFlags);
			}

			if(this.checkBox_HandInput.Checked) 
			{
				LibComNetServer.SendCommandTagValueChanged(ai.tag, val);
			}
		}




	}
}
