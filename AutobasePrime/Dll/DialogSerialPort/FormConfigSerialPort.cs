using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;

namespace DialogSerialPort
{
	/// <summary>
	/// Summary description for FormConfigSerialPort.
	/// </summary>
	public class FormConfigSerialPort : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ComboBox comboBoxPort;
		private System.Windows.Forms.ComboBox comboBoxBaud;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonStop0;
		private System.Windows.Forms.RadioButton radioButtonStop1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButtonData1;
		private System.Windows.Forms.RadioButton radioButtonData0;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton radioButtonParity1;
		private System.Windows.Forms.RadioButton radioButtonParity0;
		private System.Windows.Forms.RadioButton radioButtonParity2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public int nPort = 1;
		public int nParity = 0;
		public int nBaud = 19200;
		public int nData = 8;
		private System.Windows.Forms.CheckBox checkBoxRts;
		private System.Windows.Forms.CheckBox checkBoxDtr;
		public int nStop = 1;
		public int nRts = 1;	// 0 = OFF, 1 = ON, 2 = AUTO
		public int nDtr = 1;	// 0 = OFF, 1 = ON, 2 = AUTO

		public FormConfigSerialPort()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigSerialPort));
            this.comboBoxPort = new System.Windows.Forms.ComboBox();
            this.comboBoxBaud = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonStop1 = new System.Windows.Forms.RadioButton();
            this.radioButtonStop0 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonData1 = new System.Windows.Forms.RadioButton();
            this.radioButtonData0 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonParity2 = new System.Windows.Forms.RadioButton();
            this.radioButtonParity1 = new System.Windows.Forms.RadioButton();
            this.radioButtonParity0 = new System.Windows.Forms.RadioButton();
            this.checkBoxRts = new System.Windows.Forms.CheckBox();
            this.checkBoxDtr = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxPort
            // 
            this.comboBoxPort.AccessibleDescription = null;
            this.comboBoxPort.AccessibleName = null;
            resources.ApplyResources(this.comboBoxPort, "comboBoxPort");
            this.comboBoxPort.BackgroundImage = null;
            this.comboBoxPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPort.Font = null;
            this.comboBoxPort.Name = "comboBoxPort";
            // 
            // comboBoxBaud
            // 
            this.comboBoxBaud.AccessibleDescription = null;
            this.comboBoxBaud.AccessibleName = null;
            resources.ApplyResources(this.comboBoxBaud, "comboBoxBaud");
            this.comboBoxBaud.BackgroundImage = null;
            this.comboBoxBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBaud.Font = null;
            this.comboBoxBaud.Name = "comboBoxBaud";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
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
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.radioButtonStop1);
            this.groupBox1.Controls.Add(this.radioButtonStop0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonStop1
            // 
            this.radioButtonStop1.AccessibleDescription = null;
            this.radioButtonStop1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonStop1, "radioButtonStop1");
            this.radioButtonStop1.BackgroundImage = null;
            this.radioButtonStop1.Font = null;
            this.radioButtonStop1.Name = "radioButtonStop1";
            // 
            // radioButtonStop0
            // 
            this.radioButtonStop0.AccessibleDescription = null;
            this.radioButtonStop0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonStop0, "radioButtonStop0");
            this.radioButtonStop0.BackgroundImage = null;
            this.radioButtonStop0.Font = null;
            this.radioButtonStop0.Name = "radioButtonStop0";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.radioButtonData1);
            this.groupBox2.Controls.Add(this.radioButtonData0);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonData1
            // 
            this.radioButtonData1.AccessibleDescription = null;
            this.radioButtonData1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonData1, "radioButtonData1");
            this.radioButtonData1.BackgroundImage = null;
            this.radioButtonData1.Font = null;
            this.radioButtonData1.Name = "radioButtonData1";
            // 
            // radioButtonData0
            // 
            this.radioButtonData0.AccessibleDescription = null;
            this.radioButtonData0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonData0, "radioButtonData0");
            this.radioButtonData0.BackgroundImage = null;
            this.radioButtonData0.Font = null;
            this.radioButtonData0.Name = "radioButtonData0";
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.radioButtonParity2);
            this.groupBox3.Controls.Add(this.radioButtonParity1);
            this.groupBox3.Controls.Add(this.radioButtonParity0);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // radioButtonParity2
            // 
            this.radioButtonParity2.AccessibleDescription = null;
            this.radioButtonParity2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonParity2, "radioButtonParity2");
            this.radioButtonParity2.BackgroundImage = null;
            this.radioButtonParity2.Font = null;
            this.radioButtonParity2.Name = "radioButtonParity2";
            // 
            // radioButtonParity1
            // 
            this.radioButtonParity1.AccessibleDescription = null;
            this.radioButtonParity1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonParity1, "radioButtonParity1");
            this.radioButtonParity1.BackgroundImage = null;
            this.radioButtonParity1.Font = null;
            this.radioButtonParity1.Name = "radioButtonParity1";
            // 
            // radioButtonParity0
            // 
            this.radioButtonParity0.AccessibleDescription = null;
            this.radioButtonParity0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonParity0, "radioButtonParity0");
            this.radioButtonParity0.BackgroundImage = null;
            this.radioButtonParity0.Font = null;
            this.radioButtonParity0.Name = "radioButtonParity0";
            // 
            // checkBoxRts
            // 
            this.checkBoxRts.AccessibleDescription = null;
            this.checkBoxRts.AccessibleName = null;
            resources.ApplyResources(this.checkBoxRts, "checkBoxRts");
            this.checkBoxRts.BackgroundImage = null;
            this.checkBoxRts.Font = null;
            this.checkBoxRts.Name = "checkBoxRts";
            // 
            // checkBoxDtr
            // 
            this.checkBoxDtr.AccessibleDescription = null;
            this.checkBoxDtr.AccessibleName = null;
            resources.ApplyResources(this.checkBoxDtr, "checkBoxDtr");
            this.checkBoxDtr.BackgroundImage = null;
            this.checkBoxDtr.Font = null;
            this.checkBoxDtr.Name = "checkBoxDtr";
            // 
            // FormConfigSerialPort
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.checkBoxDtr);
            this.Controls.Add(this.checkBoxRts);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxBaud);
            this.Controls.Add(this.comboBoxPort);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigSerialPort";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormConfigSerialPort_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void FormConfigSerialPort_Load(object sender, System.EventArgs e)
		{
			string buf;
			for(int i = 0; i < 256; i++) 
			{
				buf = String.Format("COM{0}", i+1);
				this.comboBoxPort.Items.Add(buf);
			}

			this.comboBoxPort.SelectedIndex = nPort-1;

			this.comboBoxBaud.Items.Add("1200");
			this.comboBoxBaud.Items.Add("2400");
			this.comboBoxBaud.Items.Add("4800");
			this.comboBoxBaud.Items.Add("9600");
			this.comboBoxBaud.Items.Add("19200");
			this.comboBoxBaud.Items.Add("38400");
			this.comboBoxBaud.Items.Add("57600");
			this.comboBoxBaud.Items.Add("115200");

			this.comboBoxBaud.Text = nBaud.ToString();

			this.radioButtonData0.Checked = (nData == 8);
			this.radioButtonData1.Checked = (nData == 7);

			this.radioButtonParity0.Checked = (nParity == 0);
			this.radioButtonParity1.Checked = (nParity == 1);
			this.radioButtonParity2.Checked = (nParity == 2);

			this.radioButtonStop0.Checked = (nStop == 1);
			this.radioButtonStop1.Checked = (nStop == 2);

			this.checkBoxRts.Checked = (nRts == 1);
			this.checkBoxDtr.Checked = (nDtr == 1);
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			this.nPort = this.comboBoxPort.SelectedIndex+1;
			this.nBaud = ConvertTool.ToInt32(this.comboBoxBaud.Text);
			
			if(this.radioButtonStop1.Checked)	nStop = 2;
			else								nStop = 1;

			if(this.radioButtonData1.Checked)	nData = 7;
			else								nData = 8;

			if(this.radioButtonParity1.Checked)		nParity = 1;
			else if(this.radioButtonParity2.Checked)nParity = 2;
			else									nParity = 0;

			if(this.checkBoxRts.Checked)		nRts = 1;
			else								nRts = 0;

			if(this.checkBoxDtr.Checked)		nDtr = 1;
			else								nDtr = 0;

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
