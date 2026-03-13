using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using System.Drawing.Printing;

namespace LinePrinter
{
	/// <summary>
	/// Summary description for FormConfigBasic.
	/// </summary>
	public class FormConfigBasic : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBoxEnable;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButtonDate0;
		private System.Windows.Forms.RadioButton radioButtonDate1;
		private System.Windows.Forms.RadioButton radioButtonDate2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown numericUpDownTimer;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.TextBox textBoxSerial;
		private System.Windows.Forms.Button buttonSerial;
		private System.Windows.Forms.RadioButton radioButtonPortType1;
		private System.Windows.Forms.RadioButton radioButtonPortType0;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.RadioButton radioButtonLpt3;
		private System.Windows.Forms.RadioButton radioButtonLpt2;
		private System.Windows.Forms.RadioButton radioButtonLpt1;
        private RadioButton radioButtonPortType2;
        private GroupBox groupBoxPrinter;
        private ComboBox comboBoxDriver;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormConfigBasic()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigBasic));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxEnable = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonDate2 = new System.Windows.Forms.RadioButton();
            this.radioButtonDate1 = new System.Windows.Forms.RadioButton();
            this.radioButtonDate0 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonPortType2 = new System.Windows.Forms.RadioButton();
            this.radioButtonPortType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonPortType0 = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownTimer = new System.Windows.Forms.NumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.buttonSerial = new System.Windows.Forms.Button();
            this.textBoxSerial = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.radioButtonLpt3 = new System.Windows.Forms.RadioButton();
            this.radioButtonLpt2 = new System.Windows.Forms.RadioButton();
            this.radioButtonLpt1 = new System.Windows.Forms.RadioButton();
            this.groupBoxPrinter = new System.Windows.Forms.GroupBox();
            this.comboBoxDriver = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimer)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBoxPrinter.SuspendLayout();
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
            this.groupBox1.Controls.Add(this.checkBoxEnable);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // checkBoxEnable
            // 
            resources.ApplyResources(this.checkBoxEnable, "checkBoxEnable");
            this.checkBoxEnable.Name = "checkBoxEnable";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonDate2);
            this.groupBox2.Controls.Add(this.radioButtonDate1);
            this.groupBox2.Controls.Add(this.radioButtonDate0);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonDate2
            // 
            resources.ApplyResources(this.radioButtonDate2, "radioButtonDate2");
            this.radioButtonDate2.Name = "radioButtonDate2";
            // 
            // radioButtonDate1
            // 
            resources.ApplyResources(this.radioButtonDate1, "radioButtonDate1");
            this.radioButtonDate1.Name = "radioButtonDate1";
            // 
            // radioButtonDate0
            // 
            resources.ApplyResources(this.radioButtonDate0, "radioButtonDate0");
            this.radioButtonDate0.Name = "radioButtonDate0";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButtonPortType2);
            this.groupBox3.Controls.Add(this.radioButtonPortType1);
            this.groupBox3.Controls.Add(this.radioButtonPortType0);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // radioButtonPortType2
            // 
            resources.ApplyResources(this.radioButtonPortType2, "radioButtonPortType2");
            this.radioButtonPortType2.Name = "radioButtonPortType2";
            this.radioButtonPortType2.CheckedChanged += new System.EventHandler(this.radioButtonPortType2_CheckedChanged);
            // 
            // radioButtonPortType1
            // 
            resources.ApplyResources(this.radioButtonPortType1, "radioButtonPortType1");
            this.radioButtonPortType1.Name = "radioButtonPortType1";
            this.radioButtonPortType1.CheckedChanged += new System.EventHandler(this.radioButtonPortType1_CheckedChanged);
            // 
            // radioButtonPortType0
            // 
            resources.ApplyResources(this.radioButtonPortType0, "radioButtonPortType0");
            this.radioButtonPortType0.Name = "radioButtonPortType0";
            this.radioButtonPortType0.CheckedChanged += new System.EventHandler(this.radioButtonPortType0_CheckedChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // numericUpDownTimer
            // 
            resources.ApplyResources(this.numericUpDownTimer, "numericUpDownTimer");
            this.numericUpDownTimer.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.numericUpDownTimer.Name = "numericUpDownTimer";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.numericUpDownTimer);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.buttonSerial);
            this.groupBox5.Controls.Add(this.textBoxSerial);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // buttonSerial
            // 
            resources.ApplyResources(this.buttonSerial, "buttonSerial");
            this.buttonSerial.Name = "buttonSerial";
            this.buttonSerial.Click += new System.EventHandler(this.buttonSerial_Click);
            // 
            // textBoxSerial
            // 
            resources.ApplyResources(this.textBoxSerial, "textBoxSerial");
            this.textBoxSerial.Name = "textBoxSerial";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.radioButtonLpt3);
            this.groupBox6.Controls.Add(this.radioButtonLpt2);
            this.groupBox6.Controls.Add(this.radioButtonLpt1);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // radioButtonLpt3
            // 
            resources.ApplyResources(this.radioButtonLpt3, "radioButtonLpt3");
            this.radioButtonLpt3.Name = "radioButtonLpt3";
            // 
            // radioButtonLpt2
            // 
            resources.ApplyResources(this.radioButtonLpt2, "radioButtonLpt2");
            this.radioButtonLpt2.Name = "radioButtonLpt2";
            // 
            // radioButtonLpt1
            // 
            resources.ApplyResources(this.radioButtonLpt1, "radioButtonLpt1");
            this.radioButtonLpt1.Name = "radioButtonLpt1";
            // 
            // groupBoxPrinter
            // 
            this.groupBoxPrinter.Controls.Add(this.comboBoxDriver);
            resources.ApplyResources(this.groupBoxPrinter, "groupBoxPrinter");
            this.groupBoxPrinter.Name = "groupBoxPrinter";
            this.groupBoxPrinter.TabStop = false;
            // 
            // comboBoxDriver
            // 
            this.comboBoxDriver.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDriver.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxDriver, "comboBoxDriver");
            this.comboBoxDriver.Name = "comboBoxDriver";
            // 
            // FormConfigBasic
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBoxPrinter);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigBasic";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigBasic_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimer)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBoxPrinter.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		void MakeComString()
		{
			string text;

			text = String.Format("COM{0},{1},{2},{3},{4}", nPort, nBaud, nParity, nData, nStop);
			this.textBoxSerial.Text = text;
		}

		private void FormConfigBasic_Load(object sender, System.EventArgs e)
		{
			this.checkBoxEnable.Checked = Config.flag;
			this.radioButtonDate0.Checked = (Config.day_type == 0);
			this.radioButtonDate1.Checked = (Config.day_type == 1);
			this.radioButtonDate2.Checked = (Config.day_type == 2);
			this.numericUpDownTimer.Value = Config.timer;

			this.radioButtonPortType0.Checked = (Config.port_type == 0);
			this.radioButtonPortType1.Checked = (Config.port_type == 1);
            this.radioButtonPortType2.Checked = (Config.port_type == 2);

			this.radioButtonLpt1.Checked = (Config.lpt_no == 1);
			this.radioButtonLpt2.Checked = (Config.lpt_no == 2);
			this.radioButtonLpt3.Checked = (Config.lpt_no == 3);

			nPort = Config.com_no;
			nBaud = Config.baud;
			nParity = Config.parity;
			nData = Config.data;
			nStop = Config.stop;
			
			MakeComString();

            PrinterSettings s = new PrinterSettings();

            foreach (String printer in PrinterSettings.InstalledPrinters)
            {
                comboBoxDriver.Items.Add(printer);
            }

            comboBoxDriver.Text = Config.sPrinterName;

			EnableDisable();
		}

		private void radioButtonPortType0_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

		private void radioButtonPortType1_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisable();
		}

        private void radioButtonPortType2_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

		int GetRadioPortType()
		{
			int type;

			if(this.radioButtonPortType0.Checked)		type = 0;
			else if(this.radioButtonPortType1.Checked)	type = 1;
            else if (this.radioButtonPortType2.Checked) type = 2;
			else										type = 0;

			return type;
		}

		void EnableDisable()
		{
			int type = GetRadioPortType();

			bool flag_lpt;
			bool flag_com;
            bool flag_printer = false;

			if(type == 0)
			{
				flag_lpt = true;
				flag_com = false;
			}
            else if (type == 1)
            {
                flag_lpt = false;
                flag_com = true;
            }
            else
            {
                flag_lpt = false;
                flag_com = false;
                flag_printer = true;
            }

			this.radioButtonLpt1.Enabled = flag_lpt;
			this.radioButtonLpt2.Enabled = flag_lpt;
			this.radioButtonLpt3.Enabled = flag_lpt;

			this.textBoxSerial.Enabled = flag_com;
			this.buttonSerial.Enabled = flag_com;

            this.groupBoxPrinter.Enabled = flag_printer;
		}

		int nPort;
		int nBaud;
		int nParity;
		int nData;
		int nStop;

		private void buttonSerial_Click(object sender, System.EventArgs e)
		{
			DialogSerialPort.FormConfigSerialPort dialog = new DialogSerialPort.FormConfigSerialPort();
		
			dialog.nPort = nPort;
			dialog.nBaud = nBaud;
			dialog.nParity = nParity;
			dialog.nData = nData;
			dialog.nStop = nStop;

            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				nPort = dialog.nPort;
				nBaud = dialog.nBaud;
				nParity = dialog.nParity;
				nData = dialog.nData;
				nStop = dialog.nStop;

				MakeComString();
			}
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			Config.flag = this.checkBoxEnable.Checked;

			if(this.radioButtonDate0.Checked)		Config.day_type = 0;
			else if(this.radioButtonDate1.Checked)	Config.day_type = 1;
			else if(this.radioButtonDate2.Checked)	Config.day_type = 2;
			else									Config.day_type = 0;

			Config.timer = ConvertTool.ToInt32(this.numericUpDownTimer.Value);

			Config.port_type = GetRadioPortType();

			if(this.radioButtonLpt1.Checked)		Config.lpt_no = 1;
			else if(this.radioButtonLpt2.Checked)	Config.lpt_no = 2;
			else if(this.radioButtonLpt3.Checked)	Config.lpt_no = 3;
			else									Config.lpt_no = 1;

			Config.com_no = nPort;
			Config.baud = nBaud;
			Config.parity = nParity;
			Config.data = nData;
			Config.stop = nStop;

            Config.sPrinterName = this.comboBoxDriver.Text;

			Config.Save();

			DialogResult = DialogResult.OK;

			Close();
		}

        
	}
}

