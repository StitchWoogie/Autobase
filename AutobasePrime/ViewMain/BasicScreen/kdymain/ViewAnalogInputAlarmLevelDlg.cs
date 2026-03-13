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
	/// Summary description for ViewAnalogInputAlarmLevelDlg.
	/// </summary>
	public class ViewAnalogInputAlarmLevelDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_CANCEL;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox textBox_HiHi;
		private System.Windows.Forms.TextBox textBox_High;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox_Low;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBox_LoLo;
		private System.Windows.Forms.Label label4;

		TagAiClass	ai;

		public ViewAnalogInputAlarmLevelDlg(TagAiClass tag)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewAnalogInputAlarmLevelDlg));
            this.button_CANCEL = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_HiHi = new System.Windows.Forms.TextBox();
            this.textBox_High = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Low = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_LoLo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
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
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBox_HiHi
            // 
            resources.ApplyResources(this.textBox_HiHi, "textBox_HiHi");
            this.textBox_HiHi.Name = "textBox_HiHi";
            // 
            // textBox_High
            // 
            resources.ApplyResources(this.textBox_High, "textBox_High");
            this.textBox_High.Name = "textBox_High";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBox_Low
            // 
            resources.ApplyResources(this.textBox_Low, "textBox_Low");
            this.textBox_Low.Name = "textBox_Low";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // textBox_LoLo
            // 
            resources.ApplyResources(this.textBox_LoLo, "textBox_LoLo");
            this.textBox_LoLo.Name = "textBox_LoLo";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // ViewAnalogInputAlarmLevelDlg
            // 
            this.AcceptButton = this.button_OK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.button_CANCEL;
            this.Controls.Add(this.textBox_LoLo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_Low);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_High);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_HiHi);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_CANCEL);
            this.Controls.Add(this.button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewAnalogInputAlarmLevelDlg";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ViewAnalogInputAlarmLevelDlg_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void ViewAnalogInputAlarmLevelDlg_Load(object sender, System.EventArgs e)
		{
			this.textBox_HiHi.Text = ai.hihi.ToString();
			this.textBox_High.Text = ai.high.ToString();
			this.textBox_Low.Text = ai.low.ToString();
			this.textBox_LoLo.Text = ai.lolo.ToString();
            if (!SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_TAG_MEMBER_ALARM_LEVEL)) this.button_OK.Enabled = false;
		}

		private void button_OK_Click(object sender, System.EventArgs e)
		{
			TagAiClass save = (TagAiClass)Tools.CopyObject(ai);

            double[] saveVal = new double[4];

			saveVal[0] = ai.hihi;
			saveVal[1] = ai.high;
			saveVal[2] = ai.low;
			saveVal[3] = ai.lolo;

			try 
			{
				ai.hihi = ConvertTool.ToDouble(this.textBox_HiHi.Text);
			}
			catch {}
			try 
			{
				ai.high = ConvertTool.ToDouble(this.textBox_High.Text);
			}
			catch {}
			try 
			{
				ai.low = ConvertTool.ToDouble(this.textBox_Low.Text);
			}
			catch {}
			try 
			{
				ai.lolo = ConvertTool.ToDouble(this.textBox_LoLo.Text);
			}
			catch {}

			if(saveVal[0] == ai.hihi && saveVal[1] == ai.high && saveVal[2] == ai.low && saveVal[3] == ai.lolo) return;
			TagLib.bChangedByLocalMain = true;		// 태그속성이 바뀌었다, 프로그램 종료 시 등에 태그를 저장하기 위해
			ai.bTagChangeFlag = true;
			LibComNetServer.CheckChangedMember(ai, save);	
		}
	}
}
