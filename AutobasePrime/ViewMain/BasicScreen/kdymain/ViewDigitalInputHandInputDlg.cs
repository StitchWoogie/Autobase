using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLib;
using NetTools;
using AutoLibLocal;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewDigitalInputHandInputDlg.
	/// </summary>
	public class ViewDigitalInputHandInputDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBox_HandInput;
		private System.Windows.Forms.Button button_CANCEL;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.RadioButton radioButton_Off;
		private System.Windows.Forms.RadioButton radioButton_On;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		TagDiClass	di;
		bool		bOldHandInputFlag;
		public		bool	bElementChanged = false;

		public ViewDigitalInputHandInputDlg(TagDiClass	tag)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			di = tag;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewDigitalInputHandInputDlg));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton_On = new System.Windows.Forms.RadioButton();
            this.radioButton_Off = new System.Windows.Forms.RadioButton();
            this.checkBox_HandInput = new System.Windows.Forms.CheckBox();
            this.button_CANCEL = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton_On);
            this.groupBox1.Controls.Add(this.radioButton_Off);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButton_On
            // 
            resources.ApplyResources(this.radioButton_On, "radioButton_On");
            this.radioButton_On.Name = "radioButton_On";
            // 
            // radioButton_Off
            // 
            resources.ApplyResources(this.radioButton_Off, "radioButton_Off");
            this.radioButton_Off.Name = "radioButton_Off";
            // 
            // checkBox_HandInput
            // 
            resources.ApplyResources(this.checkBox_HandInput, "checkBox_HandInput");
            this.checkBox_HandInput.Name = "checkBox_HandInput";
            this.checkBox_HandInput.CheckedChanged += new System.EventHandler(this.checkBox_HandInput_CheckedChanged);
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
            // ViewDigitalInputHandInputDlg
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBox_HandInput);
            this.Controls.Add(this.button_CANCEL);
            this.Controls.Add(this.button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewDigitalInputHandInputDlg";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ViewDigitalInputHandInputDlg_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void ViewDigitalInputHandInputDlg_Load(object sender, System.EventArgs e)
		{
			if((di.wProtectFlags & EnumProtectFlag.SCAN) > 0) this.checkBox_HandInput.Checked = true;
			else this.checkBox_HandInput.Checked = false;
			bOldHandInputFlag = this.checkBox_HandInput.Checked;		// 이전의 수동기입 상태
			if(di.curr == 1) this.radioButton_On.Checked = true;
			else this.radioButton_Off.Checked = true;
			EnableDisable();
		}

		void EnableDisable()
		{
			if(this.checkBox_HandInput.Checked) 
			{
				this.radioButton_On.Enabled = true;
				this.radioButton_Off.Enabled = true;
			}
			else 
			{
				this.radioButton_On.Enabled = false;
				this.radioButton_Off.Enabled = false;
			}
            if (!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_HAND_INPUT)) this.button_OK.Enabled = false;
		}

		private void checkBox_HandInput_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		string GetDigitalStatusString(sbyte status)
		{
			
			if(status == 1) 
			{
				if(di.desON.Length <= 0) return "ON";
				return di.desON;
			}
			else 
			{
				if(di.desOFF.Length <= 0) return "OFF";
				return di.desOFF;
			}
		}


		void makeAndSetValueChangeEvent()
		{
			//COMM_EVENT_STRUCT	tagEvent = new COMM_EVENT_STRUCT();
			//tagEvent.tag = di.tag;
			//tagEvent.tag_type = EnumTagType.DI;
			//tagEvent.message_type = 0;			// 0 - 현재치, 1 - 적산치.
			//tagEvent.tp = di;
			SharedViewMain.EventGoTagChanged(di);
		}

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			string			msg, onOffBuf = "";
			sbyte			val = 0;			

			if(this.radioButton_On.Checked) val = 1;
			else val = 0;			
			if(!bOldHandInputFlag && this.checkBox_HandInput.Checked) 
			{
				onOffBuf = GetDigitalStatusString(val);
				if(Tools.IsLangKorean()) msg = string.Format("수동 기입 모드 작동.  설정값={0}", onOffBuf);
				else msg = string.Format("Hand Input Mode Start.  Value={0}", onOffBuf);
                SharedViewMain.AlarmDisplayDI(di, msg, EnumAlarmType.HAND_INPUT, true, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);
			}
			if(bOldHandInputFlag && !this.checkBox_HandInput.Checked) 
			{
				onOffBuf = GetDigitalStatusString(di.curr);
				if(Tools.IsLangKorean()) msg = string.Format("수동 기입 모드 해제.  해제값={0}", onOffBuf);
				else msg = string.Format("Hand Input Mode Stop.  Value={0}", onOffBuf);
                SharedViewMain.AlarmDisplayDI(di, msg, EnumAlarmType.HAND_INPUT, false, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);
			}
			if(this.checkBox_HandInput.Checked) 
			{
				onOffBuf = GetDigitalStatusString(di.curr);
				if(di.curr != val) 
				{
					if(Tools.IsLangKorean()) msg = string.Format("수동 기입값 변경 {0} -> {1}", onOffBuf, GetDigitalStatusString(val));
					else msg = string.Format("Hand Input Value Changed {0} -> {1}", onOffBuf, GetDigitalStatusString(val));
                    SharedViewMain.AlarmDisplayDI(di, msg, EnumAlarmType.HAND_INPUT, true, SharedData.userInfo.sUsername, "localhost", TotalConfig.sCurrentComputer);
					bElementChanged = true;
					di.curr = val;
					makeAndSetValueChangeEvent();
				}
			}

			if(bOldHandInputFlag != this.checkBox_HandInput.Checked)
			{
				if(this.checkBox_HandInput.Checked)	di.wProtectFlags |= EnumProtectFlag.SCAN;
				else								di.wProtectFlags &= (EnumProtectFlag)(0xFFFF-(int)EnumProtectFlag.SCAN);
				bElementChanged = true;
				SharedViewMain.EventGoTagPropertyChanged((TagPublicClass)di);
				LibComNetServer.SendCommandProtectFlagChange(di.tag, (int)di.wProtectFlags);
			}

			if(this.checkBox_HandInput.Checked) 
			{
				LibComNetServer.SendCommandTagValueChanged(di.tag, val);
			}
		}





	}
}
