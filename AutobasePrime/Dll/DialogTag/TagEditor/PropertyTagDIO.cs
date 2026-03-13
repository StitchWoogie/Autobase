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
	public class PropertyTagDIO : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
        private System.ComponentModel.Container components = null;
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
		private System.Windows.Forms.Label label47;
		private MyNumericUpDown numericUpDownDoPulseTime;
		private System.Windows.Forms.Label label48;

        MultiSelectCheckBox multiDoUseAsOutput = new MultiSelectCheckBox();

		MultiSelectTextBox multiDoStation = new MultiSelectTextBox();
		MultiSelectTextBox multiDoAddress = new MultiSelectTextBox(true);
		MultiSelectTextBox multiDoExtra1 = new MultiSelectTextBox();
		MultiSelectTextBox multiDoExtra2 = new MultiSelectTextBox();
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
        private CheckBox checkBoxUseAsOutput;
		MultiSelectNumericUpDown multiDoOffDelay = new MultiSelectNumericUpDown();

		public PropertyTagDIO()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			// DO
            multiDoUseAsOutput.Add(this.checkBoxUseAsOutput);
			multiDoStation.Add(this.textBoxDoStation);
			multiDoAddress.Add(this.textBoxDoAddress);
			multiDoExtra1.Add(this.textBoxDoExtra1);
			multiDoExtra2.Add(this.textBoxDoExtra2);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyTagDIO));
            this.groupBoxPlcScan = new System.Windows.Forms.GroupBox();
            this.textBoxDoExtra2 = new System.Windows.Forms.TextBox();
            this.label57 = new System.Windows.Forms.Label();
            this.textBoxDoExtra1 = new System.Windows.Forms.TextBox();
            this.label58 = new System.Windows.Forms.Label();
            this.textBoxDoStation = new System.Windows.Forms.TextBox();
            this.label61 = new System.Windows.Forms.Label();
            this.textBoxDoAddress = new System.Windows.Forms.TextBox();
            this.label62 = new System.Windows.Forms.Label();
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
            this.checkBoxUseAsOutput = new System.Windows.Forms.CheckBox();
            this.groupBoxPlcScan.SuspendLayout();
            this.groupBoxRelayType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOffDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOnDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDoPulseTime)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxPlcScan
            // 
            this.groupBoxPlcScan.Controls.Add(this.textBoxDoExtra2);
            this.groupBoxPlcScan.Controls.Add(this.label57);
            this.groupBoxPlcScan.Controls.Add(this.textBoxDoExtra1);
            this.groupBoxPlcScan.Controls.Add(this.label58);
            this.groupBoxPlcScan.Controls.Add(this.textBoxDoStation);
            this.groupBoxPlcScan.Controls.Add(this.label61);
            this.groupBoxPlcScan.Controls.Add(this.textBoxDoAddress);
            this.groupBoxPlcScan.Controls.Add(this.label62);
            resources.ApplyResources(this.groupBoxPlcScan, "groupBoxPlcScan");
            this.groupBoxPlcScan.Name = "groupBoxPlcScan";
            this.groupBoxPlcScan.TabStop = false;
            // 
            // textBoxDoExtra2
            // 
            resources.ApplyResources(this.textBoxDoExtra2, "textBoxDoExtra2");
            this.textBoxDoExtra2.Name = "textBoxDoExtra2";
            // 
            // label57
            // 
            resources.ApplyResources(this.label57, "label57");
            this.label57.Name = "label57";
            // 
            // textBoxDoExtra1
            // 
            resources.ApplyResources(this.textBoxDoExtra1, "textBoxDoExtra1");
            this.textBoxDoExtra1.Name = "textBoxDoExtra1";
            // 
            // label58
            // 
            resources.ApplyResources(this.label58, "label58");
            this.label58.Name = "label58";
            // 
            // textBoxDoStation
            // 
            resources.ApplyResources(this.textBoxDoStation, "textBoxDoStation");
            this.textBoxDoStation.Name = "textBoxDoStation";
            // 
            // label61
            // 
            resources.ApplyResources(this.label61, "label61");
            this.label61.Name = "label61";
            // 
            // textBoxDoAddress
            // 
            resources.ApplyResources(this.textBoxDoAddress, "textBoxDoAddress");
            this.textBoxDoAddress.Name = "textBoxDoAddress";
            // 
            // label62
            // 
            resources.ApplyResources(this.label62, "label62");
            this.label62.Name = "label62";
            // 
            // groupBoxRelayType
            // 
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
            resources.ApplyResources(this.groupBoxRelayType, "groupBoxRelayType");
            this.groupBoxRelayType.Name = "groupBoxRelayType";
            this.groupBoxRelayType.TabStop = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // numericUpDownOffDelay
            // 
            resources.ApplyResources(this.numericUpDownOffDelay, "numericUpDownOffDelay");
            this.numericUpDownOffDelay.Name = "numericUpDownOffDelay";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // numericUpDownOnDelay
            // 
            resources.ApplyResources(this.numericUpDownOnDelay, "numericUpDownOnDelay");
            this.numericUpDownOnDelay.Name = "numericUpDownOnDelay";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label48
            // 
            resources.ApplyResources(this.label48, "label48");
            this.label48.Name = "label48";
            // 
            // numericUpDownDoPulseTime
            // 
            resources.ApplyResources(this.numericUpDownDoPulseTime, "numericUpDownDoPulseTime");
            this.numericUpDownDoPulseTime.Name = "numericUpDownDoPulseTime";
            // 
            // label47
            // 
            resources.ApplyResources(this.label47, "label47");
            this.label47.Name = "label47";
            // 
            // radioButtonDoRelayType1
            // 
            resources.ApplyResources(this.radioButtonDoRelayType1, "radioButtonDoRelayType1");
            this.radioButtonDoRelayType1.Name = "radioButtonDoRelayType1";
            this.radioButtonDoRelayType1.CheckedChanged += new System.EventHandler(this.radioButtonDoRelayType1_CheckedChanged);
            // 
            // radioButtonDoRelayType0
            // 
            resources.ApplyResources(this.radioButtonDoRelayType0, "radioButtonDoRelayType0");
            this.radioButtonDoRelayType0.Name = "radioButtonDoRelayType0";
            this.radioButtonDoRelayType0.CheckedChanged += new System.EventHandler(this.radioButtonDoRelayType0_CheckedChanged);
            // 
            // checkBoxUseAsOutput
            // 
            resources.ApplyResources(this.checkBoxUseAsOutput, "checkBoxUseAsOutput");
            this.checkBoxUseAsOutput.Name = "checkBoxUseAsOutput";
            this.checkBoxUseAsOutput.UseVisualStyleBackColor = true;
            this.checkBoxUseAsOutput.CheckedChanged += new System.EventHandler(this.checkBoxUseAsOutput_CheckedChanged);
            // 
            // PropertyTagDIO
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.checkBoxUseAsOutput);
            this.Controls.Add(this.groupBoxPlcScan);
            this.Controls.Add(this.groupBoxRelayType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertyTagDIO";
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

        public void SetTagDO(TagDiClass di)
		{
            multiDoUseAsOutput.Set(di.bUseAsOutput);
			multiDoStation.Set(di.station);
			multiDoAddress.Set(di.writeDo.address);
            multiDoExtra1.Set(di.writeDo.sExtraAddr);
            multiDoExtra2.Set(di.writeDo.wExtraAddr);
            multiDoRelayType.Set(di.writeDo.cRelayType);
            multiDoPulseTime.Set(di.writeDo.wRelaySecTarget);

            multiDoOnDelay.Set(di.writeDo.nDelaySecON);
            multiDoOffDelay.Set(di.writeDo.nDelaySecOFF);
		}

        public void GetTagDO(TagDiClass di)
		{
            multiDoUseAsOutput.Get(ref di.bUseAsOutput);
			multiDoStation.Get(ref di.station);
            multiDoAddress.Get(ref di.writeDo.address);
            multiDoExtra1.Get(ref di.writeDo.sExtraAddr);
            multiDoExtra2.Get(ref di.writeDo.wExtraAddr);
            multiDoRelayType.Get(ref di.writeDo.cRelayType);
            multiDoPulseTime.Get(ref di.writeDo.wRelaySecTarget);

            multiDoOnDelay.Get(ref di.writeDo.nDelaySecON);
            multiDoOffDelay.Get(ref di.writeDo.nDelaySecOFF);
		}

		int nConnectionType;

		public void EnableDisableConnectionType(int type)
		{
			nConnectionType = type;
			bool flag_plcscan = true;
			bool flag_relaytype = true;

            if (this.checkBoxUseAsOutput.Checked)
            {
                if (type == 0) // plc_scan
                {

                }
                else if (type == 1) // dde
                {
                    flag_plcscan = false;
                }
                else if (type == 2) // 메모리 (사용안함)
                {
                    flag_plcscan = false;
                    //flag_relaytype = false; //20250709 PSU 메모리태그도 지연출력 사용
                }
                else if (type == 3) // 간접
                {
                    flag_plcscan = false;
                    flag_relaytype = false;
                }
                else if (type == 4) // 시스템
                {
                    flag_plcscan = false;
                    flag_relaytype = false;
                }
                else if (type == 5) // OPC
                {
                    flag_plcscan = false;
                }
            }
            else
            {
                flag_plcscan = false;
                flag_relaytype = false;
            }
			
			this.groupBoxPlcScan.Enabled = flag_plcscan;
			this.groupBoxRelayType.Enabled = flag_relaytype;

            if (this.checkBoxUseAsOutput.Checked)
            {
                EnableDisableRelayType();
            }
		}

		void EnableDisableRelayType()
		{
			bool flag_latch = true;
			bool flag_pulse = true;

            //if(nConnectionType == 2 || nConnectionType == 3 || nConnectionType == 4) 
            if (nConnectionType == 3 || nConnectionType == 4)   //20250709 PSU 메모리태그도 지연출력 사용
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

        private void checkBoxUseAsOutput_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableConnectionType(nConnectionType);
        }


	}
}
