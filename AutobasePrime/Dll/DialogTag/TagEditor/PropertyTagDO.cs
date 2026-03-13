using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AutoLibLocal;
using DialogTag;
using NetTools;
using System.IO;

namespace DialogTag.TagEditor
{
	/// <summary>
	/// Summary description for FormTagProperty.
	/// </summary>
	public class PropertyTagDO : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.CheckBox checkBoxDoReverse;
		private System.Windows.Forms.TextBox textBoxDoOffDes;
		private System.Windows.Forms.Label label43;
		private System.Windows.Forms.TextBox textBoxDoOnDes;
		private System.Windows.Forms.Label label46;
		private System.Windows.Forms.RadioButton radioButtonDoRelayType1;
		private System.Windows.Forms.RadioButton radioButtonDoRelayType0;
		private System.Windows.Forms.TextBox textBoxDoExtra2;
		private System.Windows.Forms.Label label57;
		private System.Windows.Forms.TextBox textBoxDoExtra1;
		private System.Windows.Forms.Label label58;
		private System.Windows.Forms.TextBox textBoxDoStation;
		private System.Windows.Forms.Label label61;
		private System.Windows.Forms.TextBox textBoxDoAddress;
		private System.Windows.Forms.Label label62;
		private System.Windows.Forms.TextBox textBoxDoPort;
		private System.Windows.Forms.Label label63;
		private System.Windows.Forms.Label label47;
		private MyNumericUpDown numericUpDownDoPulseTime;
		private System.Windows.Forms.Label label48;

		MultiSelectTextBox multiDoPort = new MultiSelectTextBox();
		MultiSelectTextBox multiDoStation = new MultiSelectTextBox();
		MultiSelectTextBox multiDoAddress = new MultiSelectTextBox(true);
		MultiSelectTextBox multiDoExtra1 = new MultiSelectTextBox();
		MultiSelectTextBox multiDoExtra2 = new MultiSelectTextBox();
		MultiSelectTextBox multiDoOnDes = new MultiSelectTextBox();
		MultiSelectTextBox multiDoOffDes = new MultiSelectTextBox();
		MultiSelectCheckBox multiDoReverse = new MultiSelectCheckBox();
		MultiSelectRadioButton multiDoRelayType = new MultiSelectRadioButton();
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private MyNumericUpDown numericUpDownOnDelay;
		private MyNumericUpDown numericUpDownOffDelay;
		private System.Windows.Forms.GroupBox groupBoxPlcScan;
		private System.Windows.Forms.GroupBox groupBoxRelayType;
		MultiSelectNumericUpDown multiDoPulseTime = new MultiSelectNumericUpDown();
		MultiSelectNumericUpDown multiDoOnDelay = new MultiSelectNumericUpDown();
		MultiSelectNumericUpDown multiDoOffDelay = new MultiSelectNumericUpDown();

		public PropertyTagDO()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			// DO
			multiDoPort.Add(this.textBoxDoPort);
			multiDoStation.Add(this.textBoxDoStation);
			multiDoAddress.Add(this.textBoxDoAddress);
			multiDoExtra1.Add(this.textBoxDoExtra1);
			multiDoExtra2.Add(this.textBoxDoExtra2);
			multiDoOnDes.Add(this.textBoxDoOnDes);
			multiDoOffDes.Add(this.textBoxDoOffDes);
			multiDoReverse.Add(this.checkBoxDoReverse);
			multiDoRelayType.Add(this.radioButtonDoRelayType0, radioButtonDoRelayType1);
			multiDoPulseTime.Add(this.numericUpDownDoPulseTime);
			multiDoOnDelay.Add(this.numericUpDownOnDelay);
			multiDoOffDelay.Add(this.numericUpDownOffDelay);

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyTagDO));
            this.groupBoxPlcScan = new System.Windows.Forms.GroupBox();
            this.textBoxDoExtra2 = new System.Windows.Forms.TextBox();
            this.label57 = new System.Windows.Forms.Label();
            this.textBoxDoExtra1 = new System.Windows.Forms.TextBox();
            this.label58 = new System.Windows.Forms.Label();
            this.textBoxDoStation = new System.Windows.Forms.TextBox();
            this.label61 = new System.Windows.Forms.Label();
            this.textBoxDoAddress = new System.Windows.Forms.TextBox();
            this.label62 = new System.Windows.Forms.Label();
            this.textBoxDoPort = new System.Windows.Forms.TextBox();
            this.label63 = new System.Windows.Forms.Label();
            this.checkBoxDoReverse = new System.Windows.Forms.CheckBox();
            this.textBoxDoOffDes = new System.Windows.Forms.TextBox();
            this.label43 = new System.Windows.Forms.Label();
            this.textBoxDoOnDes = new System.Windows.Forms.TextBox();
            this.label46 = new System.Windows.Forms.Label();
            this.groupBoxRelayType = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownOffDelay = new MyNumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownOnDelay = new MyNumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.numericUpDownDoPulseTime = new MyNumericUpDown();
            this.label47 = new System.Windows.Forms.Label();
            this.radioButtonDoRelayType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonDoRelayType0 = new System.Windows.Forms.RadioButton();
            this.groupBoxPlcScan.SuspendLayout();
            this.groupBoxRelayType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOffDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOnDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDoPulseTime)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxPlcScan
            // 
            this.groupBoxPlcScan.AccessibleDescription = null;
            this.groupBoxPlcScan.AccessibleName = null;
            resources.ApplyResources(this.groupBoxPlcScan, "groupBoxPlcScan");
            this.groupBoxPlcScan.BackgroundImage = null;
            this.groupBoxPlcScan.Controls.Add(this.textBoxDoExtra2);
            this.groupBoxPlcScan.Controls.Add(this.label57);
            this.groupBoxPlcScan.Controls.Add(this.textBoxDoExtra1);
            this.groupBoxPlcScan.Controls.Add(this.label58);
            this.groupBoxPlcScan.Controls.Add(this.textBoxDoStation);
            this.groupBoxPlcScan.Controls.Add(this.label61);
            this.groupBoxPlcScan.Controls.Add(this.textBoxDoAddress);
            this.groupBoxPlcScan.Controls.Add(this.label62);
            this.groupBoxPlcScan.Controls.Add(this.textBoxDoPort);
            this.groupBoxPlcScan.Controls.Add(this.label63);
            this.groupBoxPlcScan.Font = null;
            this.groupBoxPlcScan.Name = "groupBoxPlcScan";
            this.groupBoxPlcScan.TabStop = false;
            // 
            // textBoxDoExtra2
            // 
            this.textBoxDoExtra2.AccessibleDescription = null;
            this.textBoxDoExtra2.AccessibleName = null;
            resources.ApplyResources(this.textBoxDoExtra2, "textBoxDoExtra2");
            this.textBoxDoExtra2.BackgroundImage = null;
            this.textBoxDoExtra2.Font = null;
            this.textBoxDoExtra2.Name = "textBoxDoExtra2";
            // 
            // label57
            // 
            this.label57.AccessibleDescription = null;
            this.label57.AccessibleName = null;
            resources.ApplyResources(this.label57, "label57");
            this.label57.Font = null;
            this.label57.Name = "label57";
            // 
            // textBoxDoExtra1
            // 
            this.textBoxDoExtra1.AccessibleDescription = null;
            this.textBoxDoExtra1.AccessibleName = null;
            resources.ApplyResources(this.textBoxDoExtra1, "textBoxDoExtra1");
            this.textBoxDoExtra1.BackgroundImage = null;
            this.textBoxDoExtra1.Font = null;
            this.textBoxDoExtra1.Name = "textBoxDoExtra1";
            // 
            // label58
            // 
            this.label58.AccessibleDescription = null;
            this.label58.AccessibleName = null;
            resources.ApplyResources(this.label58, "label58");
            this.label58.Font = null;
            this.label58.Name = "label58";
            // 
            // textBoxDoStation
            // 
            this.textBoxDoStation.AccessibleDescription = null;
            this.textBoxDoStation.AccessibleName = null;
            resources.ApplyResources(this.textBoxDoStation, "textBoxDoStation");
            this.textBoxDoStation.BackgroundImage = null;
            this.textBoxDoStation.Font = null;
            this.textBoxDoStation.Name = "textBoxDoStation";
            // 
            // label61
            // 
            this.label61.AccessibleDescription = null;
            this.label61.AccessibleName = null;
            resources.ApplyResources(this.label61, "label61");
            this.label61.Font = null;
            this.label61.Name = "label61";
            // 
            // textBoxDoAddress
            // 
            this.textBoxDoAddress.AccessibleDescription = null;
            this.textBoxDoAddress.AccessibleName = null;
            resources.ApplyResources(this.textBoxDoAddress, "textBoxDoAddress");
            this.textBoxDoAddress.BackgroundImage = null;
            this.textBoxDoAddress.Font = null;
            this.textBoxDoAddress.Name = "textBoxDoAddress";
            // 
            // label62
            // 
            this.label62.AccessibleDescription = null;
            this.label62.AccessibleName = null;
            resources.ApplyResources(this.label62, "label62");
            this.label62.Font = null;
            this.label62.Name = "label62";
            // 
            // textBoxDoPort
            // 
            this.textBoxDoPort.AccessibleDescription = null;
            this.textBoxDoPort.AccessibleName = null;
            resources.ApplyResources(this.textBoxDoPort, "textBoxDoPort");
            this.textBoxDoPort.BackgroundImage = null;
            this.textBoxDoPort.Font = null;
            this.textBoxDoPort.Name = "textBoxDoPort";
            // 
            // label63
            // 
            this.label63.AccessibleDescription = null;
            this.label63.AccessibleName = null;
            resources.ApplyResources(this.label63, "label63");
            this.label63.Font = null;
            this.label63.Name = "label63";
            // 
            // checkBoxDoReverse
            // 
            this.checkBoxDoReverse.AccessibleDescription = null;
            this.checkBoxDoReverse.AccessibleName = null;
            resources.ApplyResources(this.checkBoxDoReverse, "checkBoxDoReverse");
            this.checkBoxDoReverse.BackgroundImage = null;
            this.checkBoxDoReverse.Font = null;
            this.checkBoxDoReverse.Name = "checkBoxDoReverse";
            // 
            // textBoxDoOffDes
            // 
            this.textBoxDoOffDes.AccessibleDescription = null;
            this.textBoxDoOffDes.AccessibleName = null;
            resources.ApplyResources(this.textBoxDoOffDes, "textBoxDoOffDes");
            this.textBoxDoOffDes.BackgroundImage = null;
            this.textBoxDoOffDes.Font = null;
            this.textBoxDoOffDes.Name = "textBoxDoOffDes";
            // 
            // label43
            // 
            this.label43.AccessibleDescription = null;
            this.label43.AccessibleName = null;
            resources.ApplyResources(this.label43, "label43");
            this.label43.Font = null;
            this.label43.Name = "label43";
            // 
            // textBoxDoOnDes
            // 
            this.textBoxDoOnDes.AccessibleDescription = null;
            this.textBoxDoOnDes.AccessibleName = null;
            resources.ApplyResources(this.textBoxDoOnDes, "textBoxDoOnDes");
            this.textBoxDoOnDes.BackgroundImage = null;
            this.textBoxDoOnDes.Font = null;
            this.textBoxDoOnDes.Name = "textBoxDoOnDes";
            // 
            // label46
            // 
            this.label46.AccessibleDescription = null;
            this.label46.AccessibleName = null;
            resources.ApplyResources(this.label46, "label46");
            this.label46.Font = null;
            this.label46.Name = "label46";
            // 
            // groupBoxRelayType
            // 
            this.groupBoxRelayType.AccessibleDescription = null;
            this.groupBoxRelayType.AccessibleName = null;
            resources.ApplyResources(this.groupBoxRelayType, "groupBoxRelayType");
            this.groupBoxRelayType.BackgroundImage = null;
            this.groupBoxRelayType.Controls.Add(this.label3);
            this.groupBoxRelayType.Controls.Add(this.numericUpDownOffDelay);
            this.groupBoxRelayType.Controls.Add(this.label4);
            this.groupBoxRelayType.Controls.Add(this.label1);
            this.groupBoxRelayType.Controls.Add(this.numericUpDownOnDelay);
            this.groupBoxRelayType.Controls.Add(this.label2);
            this.groupBoxRelayType.Controls.Add(this.label48);
            this.groupBoxRelayType.Controls.Add(this.numericUpDownDoPulseTime);
            this.groupBoxRelayType.Controls.Add(this.label47);
            this.groupBoxRelayType.Controls.Add(this.radioButtonDoRelayType1);
            this.groupBoxRelayType.Controls.Add(this.radioButtonDoRelayType0);
            this.groupBoxRelayType.Font = null;
            this.groupBoxRelayType.Name = "groupBoxRelayType";
            this.groupBoxRelayType.TabStop = false;
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // numericUpDownOffDelay
            // 
            this.numericUpDownOffDelay.AccessibleDescription = null;
            this.numericUpDownOffDelay.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownOffDelay, "numericUpDownOffDelay");
            this.numericUpDownOffDelay.Font = null;
            this.numericUpDownOffDelay.Name = "numericUpDownOffDelay";
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // numericUpDownOnDelay
            // 
            this.numericUpDownOnDelay.AccessibleDescription = null;
            this.numericUpDownOnDelay.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownOnDelay, "numericUpDownOnDelay");
            this.numericUpDownOnDelay.Font = null;
            this.numericUpDownOnDelay.Name = "numericUpDownOnDelay";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // label48
            // 
            this.label48.AccessibleDescription = null;
            this.label48.AccessibleName = null;
            resources.ApplyResources(this.label48, "label48");
            this.label48.Font = null;
            this.label48.Name = "label48";
            // 
            // numericUpDownDoPulseTime
            // 
            this.numericUpDownDoPulseTime.AccessibleDescription = null;
            this.numericUpDownDoPulseTime.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownDoPulseTime, "numericUpDownDoPulseTime");
            this.numericUpDownDoPulseTime.Font = null;
            this.numericUpDownDoPulseTime.Name = "numericUpDownDoPulseTime";
            // 
            // label47
            // 
            this.label47.AccessibleDescription = null;
            this.label47.AccessibleName = null;
            resources.ApplyResources(this.label47, "label47");
            this.label47.Font = null;
            this.label47.Name = "label47";
            // 
            // radioButtonDoRelayType1
            // 
            this.radioButtonDoRelayType1.AccessibleDescription = null;
            this.radioButtonDoRelayType1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDoRelayType1, "radioButtonDoRelayType1");
            this.radioButtonDoRelayType1.BackgroundImage = null;
            this.radioButtonDoRelayType1.Font = null;
            this.radioButtonDoRelayType1.Name = "radioButtonDoRelayType1";
            this.radioButtonDoRelayType1.CheckedChanged += new System.EventHandler(this.radioButtonDoRelayType1_CheckedChanged);
            // 
            // radioButtonDoRelayType0
            // 
            this.radioButtonDoRelayType0.AccessibleDescription = null;
            this.radioButtonDoRelayType0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDoRelayType0, "radioButtonDoRelayType0");
            this.radioButtonDoRelayType0.BackgroundImage = null;
            this.radioButtonDoRelayType0.Font = null;
            this.radioButtonDoRelayType0.Name = "radioButtonDoRelayType0";
            this.radioButtonDoRelayType0.CheckedChanged += new System.EventHandler(this.radioButtonDoRelayType0_CheckedChanged);
            // 
            // PropertyTagDO
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.textBoxDoOnDes);
            this.Controls.Add(this.textBoxDoOffDes);
            this.Controls.Add(this.label46);
            this.Controls.Add(this.groupBoxPlcScan);
            this.Controls.Add(this.label43);
            this.Controls.Add(this.groupBoxRelayType);
            this.Controls.Add(this.checkBoxDoReverse);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertyTagDO";
            this.ShowInTaskbar = false;
            this.groupBoxPlcScan.ResumeLayout(false);
            this.groupBoxPlcScan.PerformLayout();
            this.groupBoxRelayType.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOffDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOnDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDoPulseTime)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public void SetTagDO(TagDoClass dout)
		{
			multiDoPort.Set(dout.port);
			multiDoStation.Set(dout.station);
			multiDoAddress.Set(dout.address);
			multiDoExtra1.Set(dout.sExtraAddr);
			multiDoExtra2.Set(dout.wExtraAddr);
			multiDoOnDes.Set(dout.desON);
			multiDoOffDes.Set(dout.desOFF);
			multiDoReverse.Set(dout.bReverse);
			multiDoRelayType.Set(dout.cRelayType);
			multiDoPulseTime.Set(dout.wRelaySecTarget);

			multiDoOnDelay.Set(dout.nDelaySecON);
			multiDoOffDelay.Set(dout.nDelaySecOFF);
		}

		public void GetTagDO(TagDoClass dout)
		{
			multiDoPort.Get(ref dout.port);
			multiDoStation.Get(ref dout.station);
			multiDoAddress.Get(ref dout.address);
			multiDoExtra1.Get(ref dout.sExtraAddr);
			multiDoExtra2.Get(ref dout.wExtraAddr);
			multiDoOnDes.Get(ref dout.desON);
			multiDoOffDes.Get(ref dout.desOFF);
			multiDoReverse.Get(ref dout.bReverse);
			multiDoRelayType.Get(ref dout.cRelayType);
			multiDoPulseTime.Get(ref dout.wRelaySecTarget);

			multiDoOnDelay.Get(ref dout.nDelaySecON);
			multiDoOffDelay.Get(ref dout.nDelaySecOFF);
		}

		int nConnectionType;

		public void EnableDisableConnectionType(int type)
		{
			nConnectionType = type;
			bool flag_plcscan = true;
			bool flag_relaytype = true;

			if(type == 0) // plc_scan
			{
				
			}
			else if(type == 1) // dde
			{
				flag_plcscan = false;
			}
			else if(type == 2) // 메모리 (사용안함)
			{
				flag_plcscan = false;
				//flag_relaytype = false; //20250709 PSU 메모리태그도 지연 설정 가능하도록 수정.
			}
			else if(type == 3) // 간접
			{
				flag_plcscan = false;	
				flag_relaytype = false;
			}
			else if(type == 4) // 시스템
			{
				flag_plcscan = false;
				flag_relaytype = false;
			}
			else if(type == 5) // OPC
			{
				flag_plcscan = false;
			}
			
			this.groupBoxPlcScan.Enabled = flag_plcscan;
			this.groupBoxRelayType.Enabled = flag_relaytype;

			EnableDisableRelayType();
		}

		void EnableDisableRelayType()
		{
			bool flag_latch = true;
			bool flag_pulse = true;

            //if(nConnectionType == 2 || nConnectionType == 3 || nConnectionType == 4) 
            if (nConnectionType == 3 || nConnectionType == 4) //20250709 PSU 메모리태그도 사용할 수 있도록 수정.
            {
				flag_latch = false;
				flag_pulse = false;
			}
			else 
			{
				if(this.radioButtonDoRelayType0.Checked)		flag_pulse = false;
				else if(this.radioButtonDoRelayType1.Checked)	flag_latch = false;
				else {}
			}

			this.numericUpDownDoPulseTime.Enabled = flag_pulse;
			this.numericUpDownOffDelay.Enabled = flag_latch;
			this.numericUpDownOnDelay.Enabled = flag_latch;
		}

		private void radioButtonDoRelayType0_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisableRelayType();
		}

		private void radioButtonDoRelayType1_CheckedChanged(object sender, System.EventArgs e)
		{
			EnableDisableRelayType();
		}


	}
}
