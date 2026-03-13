using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using AutoLib;
using NetTools;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ViewAnalogInputDetailDlgSetting.
	/// </summary>
	public class ViewAnalogInputDetailDlgSetting : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button_cancel;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.GroupBox groupBox_data_read_period;
		public System.Windows.Forms.ComboBox comboBox_data_read_period;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		public MyNumericUpDown numericUpDown_total_time;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxAlarmHiHi;
		private System.Windows.Forms.TextBox textBoxAlarmHigh;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBoxAlarmLow;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textBoxAlarmLoLo;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textBoxViewBase;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textBoxViewFull;
		private System.Windows.Forms.Label label11;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ViewAnalogInputDetailDlgSetting()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewAnalogInputDetailDlgSetting));
            this.button_cancel = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox_data_read_period = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_data_read_period = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown_total_time = new AutoLibLocal.MyNumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxAlarmLoLo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxAlarmLow = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxAlarmHigh = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxAlarmHiHi = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxViewBase = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxViewFull = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox_data_read_period.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_total_time)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_cancel
            // 
            this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button_cancel, "button_cancel");
            this.button_cancel.Name = "button_cancel";
            // 
            // button_OK
            // 
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // groupBox_data_read_period
            // 
            this.groupBox_data_read_period.Controls.Add(this.label4);
            this.groupBox_data_read_period.Controls.Add(this.label3);
            this.groupBox_data_read_period.Controls.Add(this.comboBox_data_read_period);
            this.groupBox_data_read_period.Controls.Add(this.label2);
            this.groupBox_data_read_period.Controls.Add(this.numericUpDown_total_time);
            resources.ApplyResources(this.groupBox_data_read_period, "groupBox_data_read_period");
            this.groupBox_data_read_period.Name = "groupBox_data_read_period";
            this.groupBox_data_read_period.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // comboBox_data_read_period
            // 
            this.comboBox_data_read_period.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_data_read_period.DropDownWidth = 72;
            resources.ApplyResources(this.comboBox_data_read_period, "comboBox_data_read_period");
            this.comboBox_data_read_period.Items.AddRange(new object[] {
            resources.GetString("comboBox_data_read_period.Items"),
            resources.GetString("comboBox_data_read_period.Items1"),
            resources.GetString("comboBox_data_read_period.Items2"),
            resources.GetString("comboBox_data_read_period.Items3"),
            resources.GetString("comboBox_data_read_period.Items4"),
            resources.GetString("comboBox_data_read_period.Items5"),
            resources.GetString("comboBox_data_read_period.Items6"),
            resources.GetString("comboBox_data_read_period.Items7"),
            resources.GetString("comboBox_data_read_period.Items8"),
            resources.GetString("comboBox_data_read_period.Items9")});
            this.comboBox_data_read_period.Name = "comboBox_data_read_period";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // numericUpDown_total_time
            // 
            resources.ApplyResources(this.numericUpDown_total_time, "numericUpDown_total_time");
            this.numericUpDown_total_time.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDown_total_time.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_total_time.Name = "numericUpDown_total_time";
            this.numericUpDown_total_time.SampleProperty = 10;
            this.numericUpDown_total_time.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxAlarmLoLo);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBoxAlarmLow);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textBoxAlarmHigh);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBoxAlarmHiHi);
            this.groupBox1.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBoxAlarmLoLo
            // 
            resources.ApplyResources(this.textBoxAlarmLoLo, "textBoxAlarmLoLo");
            this.textBoxAlarmLoLo.Name = "textBoxAlarmLoLo";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // textBoxAlarmLow
            // 
            resources.ApplyResources(this.textBoxAlarmLow, "textBoxAlarmLow");
            this.textBoxAlarmLow.Name = "textBoxAlarmLow";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // textBoxAlarmHigh
            // 
            resources.ApplyResources(this.textBoxAlarmHigh, "textBoxAlarmHigh");
            this.textBoxAlarmHigh.Name = "textBoxAlarmHigh";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // textBoxAlarmHiHi
            // 
            resources.ApplyResources(this.textBoxAlarmHiHi, "textBoxAlarmHiHi");
            this.textBoxAlarmHiHi.Name = "textBoxAlarmHiHi";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxViewBase);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.textBoxViewFull);
            this.groupBox2.Controls.Add(this.label11);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // textBoxViewBase
            // 
            resources.ApplyResources(this.textBoxViewBase, "textBoxViewBase");
            this.textBoxViewBase.Name = "textBoxViewBase";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // textBoxViewFull
            // 
            resources.ApplyResources(this.textBoxViewFull, "textBoxViewFull");
            this.textBoxViewFull.Name = "textBoxViewFull";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // ViewAnalogInputDetailDlgSetting
            // 
            this.AcceptButton = this.button_OK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.button_cancel;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox_data_read_period);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewAnalogInputDetailDlgSetting";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ViewAnalogInputDetailDlgSetting_Load);
            this.groupBox_data_read_period.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_total_time)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		public void SetValue(TagAiClass ai)
		{
			this.textBoxAlarmHigh.Text = TagUtil.AiValueToStringOnlyPoint(ai, ai.high);
			this.textBoxAlarmHiHi.Text = TagUtil.AiValueToStringOnlyPoint(ai, ai.hihi);
			this.textBoxAlarmLow.Text = TagUtil.AiValueToStringOnlyPoint(ai, ai.low);
			this.textBoxAlarmLoLo.Text = TagUtil.AiValueToStringOnlyPoint(ai, ai.lolo);

			this.textBoxViewBase.Text = TagUtil.AiValueToStringOnlyPoint(ai, ai.view_base);
			this.textBoxViewFull.Text = TagUtil.AiValueToStringOnlyPoint(ai, ai.view_full);
		}

		public void GetValue(TagAiClass ai)
		{
			ai.hihi = ConvertTool.ToSingle(this.textBoxAlarmHiHi.Text);
			ai.high = ConvertTool.ToSingle(this.textBoxAlarmHigh.Text);
			ai.low = ConvertTool.ToSingle(this.textBoxAlarmLow.Text);
			ai.lolo = ConvertTool.ToSingle(this.textBoxAlarmLoLo.Text);

			ai.view_base = ConvertTool.ToSingle(this.textBoxViewBase.Text);
			ai.view_full = ConvertTool.ToSingle(this.textBoxViewFull.Text);
		}

		private void ViewAnalogInputDetailDlgSetting_Load(object sender, System.EventArgs e)
		{
			bool flag;

            flag = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_TAG_MEMBER_ALARM_LEVEL);

			this.textBoxAlarmHigh.Enabled = flag;
			this.textBoxAlarmHiHi.Enabled = flag;
			this.textBoxAlarmLow.Enabled = flag;
			this.textBoxAlarmLoLo.Enabled = flag;

            flag = SharedData.userInfo.IsHaveRight(EnumUserRights.RIGHT_TAG_MEMBER_VIEW_RANGE);

			this.textBoxViewBase.Enabled = flag;
			this.textBoxViewFull.Enabled = flag;
		}

        private void button_OK_Click(object sender, EventArgs e)
        {
            TagLib.bChangedByLocalMain = true;  // 2012-9-13 Ãß°¡
        }
	}
}
