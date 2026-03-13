using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ExportLib;

namespace ExportGlofaCnet
{
	/// <summary>
	/// Summary description for FormProtocolOption.
	/// </summary>
	public class FormProtocolOption : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.CheckBox checkBoxUseStation;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.NumericUpDown numericUpDownStation;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormProtocolOption()
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

		public void SetParameters(string option)
		{
			CommaBlockString comma = new CommaBlockString();

			int imsi = 0;

			comma.Set(option);
			comma.GetInt(ref imsi);
			this.checkBoxUseStation.Checked = (imsi == 1);
			comma.GetInt(ref imsi);
			this.numericUpDownStation.Value = imsi;
		}

		public void GetParameters(ref string option)
		{
			option = "";
			option += String.Format("{0},", this.checkBoxUseStation.Checked ? 1 : 0);
			option += String.Format("{0},", this.numericUpDownStation.Value);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.checkBoxUseStation = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.numericUpDownStation = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStation)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(240, 16);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(240, 48);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			// 
			// checkBoxUseStation
			// 
			this.checkBoxUseStation.Location = new System.Drawing.Point(8, 16);
			this.checkBoxUseStation.Name = "checkBoxUseStation";
			this.checkBoxUseStation.TabIndex = 2;
			this.checkBoxUseStation.Text = "Use station";
			this.checkBoxUseStation.CheckedChanged += new System.EventHandler(this.checkBoxUseStation_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.numericUpDownStation);
			this.groupBox1.Controls.Add(this.checkBoxUseStation);
			this.groupBox1.Location = new System.Drawing.Point(16, 16);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 88);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Station";
			// 
			// numericUpDownStation
			// 
			this.numericUpDownStation.Location = new System.Drawing.Point(16, 48);
			this.numericUpDownStation.Maximum = new System.Decimal(new int[] {
																				 255,
																				 0,
																				 0,
																				 0});
			this.numericUpDownStation.Name = "numericUpDownStation";
			this.numericUpDownStation.Size = new System.Drawing.Size(56, 21);
			this.numericUpDownStation.TabIndex = 3;
			this.numericUpDownStation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(80, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 23);
			this.label1.TabIndex = 4;
			this.label1.Text = "0~255";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormProtocolOption
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(330, 273);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormProtocolOption";
			this.ShowInTaskbar = false;
			this.Text = "FormProtocolOption";
			this.Load += new System.EventHandler(this.FormProtocolOption_Load);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStation)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		void EnableDisableStation()
		{
			this.numericUpDownStation.Enabled = this.checkBoxUseStation.Checked;
		}

		private void checkBoxUseStation_CheckedChanged(object sender, System.EventArgs e)
		{
			this.EnableDisableStation();
		}

		private void FormProtocolOption_Load(object sender, System.EventArgs e)
		{
			this.EnableDisableStation();
		}
	}
}
