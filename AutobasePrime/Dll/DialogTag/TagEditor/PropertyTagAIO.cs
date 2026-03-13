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
	public class PropertyTagAIO : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
        private System.ComponentModel.Container components = null;
		private System.Windows.Forms.RadioButton radioButtonAoCalcFilter1;
        private System.Windows.Forms.RadioButton radioButtonAoCalcFilter0;
		private System.Windows.Forms.TextBox textBoxAoAddress;
        private System.Windows.Forms.Label label44;
		private System.Windows.Forms.TextBox textBoxAoStation;
		private System.Windows.Forms.Label label31;
		private System.Windows.Forms.TextBox textBoxAoExtra1;
		private System.Windows.Forms.Label label39;
		private System.Windows.Forms.TextBox textBoxAoExtra2;
		private System.Windows.Forms.Label label40;
		private System.Windows.Forms.RadioButton radioButtonAoDdeData1;
		private System.Windows.Forms.RadioButton radioButtonAoDdeData0;

        MultiSelectCheckBox multiAoUseAsOutput = new MultiSelectCheckBox();

		MultiSelectTextBox multiAoStation = new MultiSelectTextBox();
		MultiSelectTextBox multiAoAddress = new MultiSelectTextBox(true);
		MultiSelectTextBox multiAoExtra1 = new MultiSelectTextBox();
		MultiSelectTextBox multiAoExtra2 = new MultiSelectTextBox();
		
		MultiSelectRadioButton multiAoFilter = new MultiSelectRadioButton();
        private System.Windows.Forms.GroupBox groupBoxCalcFilter;
		private System.Windows.Forms.GroupBox groupBoxPlcScan;
		private System.Windows.Forms.GroupBox groupBoxDdeData;
        private CheckBox checkBoxUseAsOutput;
        private RadioButton radioButtonAoCalcFilter2;
        private GroupBox groupBox2;
        private TextBox textBoxCalcScript;
		MultiSelectRadioButton multiAoDdeData = new MultiSelectRadioButton();

        MultiSelectTextBox multiAoCalcScript = new MultiSelectTextBox();

        public PropertyTagAIO()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			// AO
            multiAoUseAsOutput.Add(this.checkBoxUseAsOutput);
			multiAoStation.Add(this.textBoxAoStation);
			multiAoAddress.Add(this.textBoxAoAddress);
			multiAoExtra1.Add(this.textBoxAoExtra1);
			multiAoExtra2.Add(this.textBoxAoExtra2);
			multiAoFilter.Add(this.radioButtonAoCalcFilter0, radioButtonAoCalcFilter1, radioButtonAoCalcFilter2);
			multiAoDdeData.Add(this.radioButtonAoDdeData0, radioButtonAoDdeData1);
            multiAoCalcScript.Add(this.textBoxCalcScript);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyTagAIO));
            this.groupBoxCalcFilter = new System.Windows.Forms.GroupBox();
            this.radioButtonAoCalcFilter2 = new System.Windows.Forms.RadioButton();
            this.radioButtonAoCalcFilter1 = new System.Windows.Forms.RadioButton();
            this.radioButtonAoCalcFilter0 = new System.Windows.Forms.RadioButton();
            this.groupBoxPlcScan = new System.Windows.Forms.GroupBox();
            this.textBoxAoExtra2 = new System.Windows.Forms.TextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.textBoxAoExtra1 = new System.Windows.Forms.TextBox();
            this.label39 = new System.Windows.Forms.Label();
            this.textBoxAoStation = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.textBoxAoAddress = new System.Windows.Forms.TextBox();
            this.label44 = new System.Windows.Forms.Label();
            this.groupBoxDdeData = new System.Windows.Forms.GroupBox();
            this.radioButtonAoDdeData1 = new System.Windows.Forms.RadioButton();
            this.radioButtonAoDdeData0 = new System.Windows.Forms.RadioButton();
            this.checkBoxUseAsOutput = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxCalcScript = new System.Windows.Forms.TextBox();
            this.groupBoxCalcFilter.SuspendLayout();
            this.groupBoxPlcScan.SuspendLayout();
            this.groupBoxDdeData.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxCalcFilter
            // 
            this.groupBoxCalcFilter.AccessibleDescription = null;
            this.groupBoxCalcFilter.AccessibleName = null;
            resources.ApplyResources(this.groupBoxCalcFilter, "groupBoxCalcFilter");
            this.groupBoxCalcFilter.BackgroundImage = null;
            this.groupBoxCalcFilter.Controls.Add(this.radioButtonAoCalcFilter2);
            this.groupBoxCalcFilter.Controls.Add(this.radioButtonAoCalcFilter1);
            this.groupBoxCalcFilter.Controls.Add(this.radioButtonAoCalcFilter0);
            this.groupBoxCalcFilter.Font = null;
            this.groupBoxCalcFilter.Name = "groupBoxCalcFilter";
            this.groupBoxCalcFilter.TabStop = false;
            // 
            // radioButtonAoCalcFilter2
            // 
            this.radioButtonAoCalcFilter2.AccessibleDescription = null;
            this.radioButtonAoCalcFilter2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAoCalcFilter2, "radioButtonAoCalcFilter2");
            this.radioButtonAoCalcFilter2.BackgroundImage = null;
            this.radioButtonAoCalcFilter2.Font = null;
            this.radioButtonAoCalcFilter2.Name = "radioButtonAoCalcFilter2";
            this.radioButtonAoCalcFilter2.CheckedChanged += new System.EventHandler(this.radioButtonAoCalcFilter2_CheckedChanged);
            // 
            // radioButtonAoCalcFilter1
            // 
            this.radioButtonAoCalcFilter1.AccessibleDescription = null;
            this.radioButtonAoCalcFilter1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAoCalcFilter1, "radioButtonAoCalcFilter1");
            this.radioButtonAoCalcFilter1.BackgroundImage = null;
            this.radioButtonAoCalcFilter1.Font = null;
            this.radioButtonAoCalcFilter1.Name = "radioButtonAoCalcFilter1";
            this.radioButtonAoCalcFilter1.CheckedChanged += new System.EventHandler(this.radioButtonAoCalcFilter1_CheckedChanged);
            // 
            // radioButtonAoCalcFilter0
            // 
            this.radioButtonAoCalcFilter0.AccessibleDescription = null;
            this.radioButtonAoCalcFilter0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAoCalcFilter0, "radioButtonAoCalcFilter0");
            this.radioButtonAoCalcFilter0.BackgroundImage = null;
            this.radioButtonAoCalcFilter0.Font = null;
            this.radioButtonAoCalcFilter0.Name = "radioButtonAoCalcFilter0";
            this.radioButtonAoCalcFilter0.CheckedChanged += new System.EventHandler(this.radioButtonAoCalcFilter0_CheckedChanged);
            // 
            // groupBoxPlcScan
            // 
            this.groupBoxPlcScan.AccessibleDescription = null;
            this.groupBoxPlcScan.AccessibleName = null;
            resources.ApplyResources(this.groupBoxPlcScan, "groupBoxPlcScan");
            this.groupBoxPlcScan.BackgroundImage = null;
            this.groupBoxPlcScan.Controls.Add(this.textBoxAoExtra2);
            this.groupBoxPlcScan.Controls.Add(this.label40);
            this.groupBoxPlcScan.Controls.Add(this.textBoxAoExtra1);
            this.groupBoxPlcScan.Controls.Add(this.label39);
            this.groupBoxPlcScan.Controls.Add(this.textBoxAoStation);
            this.groupBoxPlcScan.Controls.Add(this.label31);
            this.groupBoxPlcScan.Controls.Add(this.textBoxAoAddress);
            this.groupBoxPlcScan.Controls.Add(this.label44);
            this.groupBoxPlcScan.Font = null;
            this.groupBoxPlcScan.Name = "groupBoxPlcScan";
            this.groupBoxPlcScan.TabStop = false;
            // 
            // textBoxAoExtra2
            // 
            this.textBoxAoExtra2.AccessibleDescription = null;
            this.textBoxAoExtra2.AccessibleName = null;
            resources.ApplyResources(this.textBoxAoExtra2, "textBoxAoExtra2");
            this.textBoxAoExtra2.BackgroundImage = null;
            this.textBoxAoExtra2.Font = null;
            this.textBoxAoExtra2.Name = "textBoxAoExtra2";
            // 
            // label40
            // 
            this.label40.AccessibleDescription = null;
            this.label40.AccessibleName = null;
            resources.ApplyResources(this.label40, "label40");
            this.label40.Font = null;
            this.label40.Name = "label40";
            // 
            // textBoxAoExtra1
            // 
            this.textBoxAoExtra1.AccessibleDescription = null;
            this.textBoxAoExtra1.AccessibleName = null;
            resources.ApplyResources(this.textBoxAoExtra1, "textBoxAoExtra1");
            this.textBoxAoExtra1.BackgroundImage = null;
            this.textBoxAoExtra1.Font = null;
            this.textBoxAoExtra1.Name = "textBoxAoExtra1";
            // 
            // label39
            // 
            this.label39.AccessibleDescription = null;
            this.label39.AccessibleName = null;
            resources.ApplyResources(this.label39, "label39");
            this.label39.Font = null;
            this.label39.Name = "label39";
            // 
            // textBoxAoStation
            // 
            this.textBoxAoStation.AccessibleDescription = null;
            this.textBoxAoStation.AccessibleName = null;
            resources.ApplyResources(this.textBoxAoStation, "textBoxAoStation");
            this.textBoxAoStation.BackgroundImage = null;
            this.textBoxAoStation.Font = null;
            this.textBoxAoStation.Name = "textBoxAoStation";
            // 
            // label31
            // 
            this.label31.AccessibleDescription = null;
            this.label31.AccessibleName = null;
            resources.ApplyResources(this.label31, "label31");
            this.label31.Font = null;
            this.label31.Name = "label31";
            // 
            // textBoxAoAddress
            // 
            this.textBoxAoAddress.AccessibleDescription = null;
            this.textBoxAoAddress.AccessibleName = null;
            resources.ApplyResources(this.textBoxAoAddress, "textBoxAoAddress");
            this.textBoxAoAddress.BackgroundImage = null;
            this.textBoxAoAddress.Font = null;
            this.textBoxAoAddress.Name = "textBoxAoAddress";
            // 
            // label44
            // 
            this.label44.AccessibleDescription = null;
            this.label44.AccessibleName = null;
            resources.ApplyResources(this.label44, "label44");
            this.label44.Font = null;
            this.label44.Name = "label44";
            // 
            // groupBoxDdeData
            // 
            this.groupBoxDdeData.AccessibleDescription = null;
            this.groupBoxDdeData.AccessibleName = null;
            resources.ApplyResources(this.groupBoxDdeData, "groupBoxDdeData");
            this.groupBoxDdeData.BackgroundImage = null;
            this.groupBoxDdeData.Controls.Add(this.radioButtonAoDdeData1);
            this.groupBoxDdeData.Controls.Add(this.radioButtonAoDdeData0);
            this.groupBoxDdeData.Font = null;
            this.groupBoxDdeData.Name = "groupBoxDdeData";
            this.groupBoxDdeData.TabStop = false;
            // 
            // radioButtonAoDdeData1
            // 
            this.radioButtonAoDdeData1.AccessibleDescription = null;
            this.radioButtonAoDdeData1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAoDdeData1, "radioButtonAoDdeData1");
            this.radioButtonAoDdeData1.BackgroundImage = null;
            this.radioButtonAoDdeData1.Font = null;
            this.radioButtonAoDdeData1.Name = "radioButtonAoDdeData1";
            // 
            // radioButtonAoDdeData0
            // 
            this.radioButtonAoDdeData0.AccessibleDescription = null;
            this.radioButtonAoDdeData0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonAoDdeData0, "radioButtonAoDdeData0");
            this.radioButtonAoDdeData0.BackgroundImage = null;
            this.radioButtonAoDdeData0.Font = null;
            this.radioButtonAoDdeData0.Name = "radioButtonAoDdeData0";
            // 
            // checkBoxUseAsOutput
            // 
            this.checkBoxUseAsOutput.AccessibleDescription = null;
            this.checkBoxUseAsOutput.AccessibleName = null;
            resources.ApplyResources(this.checkBoxUseAsOutput, "checkBoxUseAsOutput");
            this.checkBoxUseAsOutput.BackgroundImage = null;
            this.checkBoxUseAsOutput.Font = null;
            this.checkBoxUseAsOutput.Name = "checkBoxUseAsOutput";
            this.checkBoxUseAsOutput.UseVisualStyleBackColor = true;
            this.checkBoxUseAsOutput.CheckedChanged += new System.EventHandler(this.checkBoxUseAsOutput_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.textBoxCalcScript);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // textBoxCalcScript
            // 
            this.textBoxCalcScript.AccessibleDescription = null;
            this.textBoxCalcScript.AccessibleName = null;
            resources.ApplyResources(this.textBoxCalcScript, "textBoxCalcScript");
            this.textBoxCalcScript.BackgroundImage = null;
            this.textBoxCalcScript.Font = null;
            this.textBoxCalcScript.Name = "textBoxCalcScript";
            // 
            // PropertyTagAIO
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.checkBoxUseAsOutput);
            this.Controls.Add(this.groupBoxDdeData);
            this.Controls.Add(this.groupBoxCalcFilter);
            this.Controls.Add(this.groupBoxPlcScan);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertyTagAIO";
            this.ShowInTaskbar = false;
            this.groupBoxCalcFilter.ResumeLayout(false);
            this.groupBoxPlcScan.ResumeLayout(false);
            this.groupBoxPlcScan.PerformLayout();
            this.groupBoxDdeData.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion


		public void SetTagAO(TagAiClass ai)
		{
            multiAoUseAsOutput.Set(ai.bUseAsOutput);
			//multiAoPort.Set(ao.port);
			multiAoStation.Set(ai.station);
			multiAoAddress.Set(ai.writeAo.address);
			multiAoExtra1.Set(ai.writeAo.sExtraAddr);
			multiAoExtra2.Set(ai.writeAo.wExtraAddr);
			//multiAoFull.Set(ao.fFull);
			//multiAoBase.Set(ao.fBase);
			//multiAoPlcFull.Set(ao.plc_full);
			//multiAoPlcBase.Set(ao.plc_base);
			//multiAoUnit.Set(ao.unit);

			//multiAoBcd.Set(ao.bBcdValue);
            multiAoFilter.Set(ai.writeAo.nCalculateFilter);
			//multiAoCut.Set(ao.bCutOverValue);
            multiAoDdeData.Set(ai.writeAo.cDdeDataFormat);
            multiAoCalcScript.Set(ai.writeAo.sCalcScript);
		}

		public void GetTagAO(TagAiClass ai)
		{
            multiAoUseAsOutput.Get(ref ai.bUseAsOutput);
			//multiAoPort.Get(ref ao.port);

            multiAoStation.Get(ref ai.station);
            multiAoAddress.Get(ref ai.writeAo.address);
            multiAoExtra1.Get(ref ai.writeAo.sExtraAddr);
            multiAoExtra2.Get(ref ai.writeAo.wExtraAddr);
			//multiAoFull.Get(ref ao.fFull);
			//multiAoBase.Get(ref ao.fBase);
			//multiAoPlcFull.Get(ref ao.plc_full);
			//multiAoPlcBase.Get(ref ao.plc_base);
			//multiAoUnit.Get(ref ao.unit);

			//multiAoBcd.Get(ref ao.bBcdValue);
			multiAoFilter.Get(ref ai.writeAo.nCalculateFilter);
			//multiAoCut.Get(ref ao.bCutOverValue);
            multiAoDdeData.Get(ref ai.writeAo.cDdeDataFormat);
            multiAoCalcScript.Get(ref ai.writeAo.sCalcScript);
		}

        int nSaveTagType = 0;

		public void EnableDisableConnectionType(int type)
		{
            nSaveTagType = type;

			bool flag_plcscan = true;
			bool flag_calcfilter = true;
			bool flag_ddedata = true;

            if (this.checkBoxUseAsOutput.Checked)
            {
                if (type == 0)
                {
                    flag_ddedata = false;
                }
                else if (type == 1) //dde
                {
                    flag_plcscan = false;
                }
                else if (type == 2)
                {
                    flag_plcscan = false;
                    flag_calcfilter = false;
                    flag_ddedata = false;
                }
                else if (type == 3)
                {
                    flag_plcscan = false;
                    flag_calcfilter = false;
                    flag_ddedata = false;
                }
                else if (type == 4)
                {
                    flag_plcscan = false;
                    flag_calcfilter = false;
                    flag_ddedata = false;
                }
                else if (type == 5) // opc
                {
                    flag_plcscan = false;
                    //flag_calcfilter = false;
                    flag_ddedata = false;
                }
            }
            else
            {
                flag_plcscan = false;
                flag_calcfilter = false;
                flag_ddedata = false;
            }
			
			this.groupBoxPlcScan.Enabled = flag_plcscan;
			this.groupBoxCalcFilter.Enabled = flag_calcfilter;
			this.groupBoxDdeData.Enabled = flag_ddedata;
		}

        private void checkBoxUseAsOutput_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableConnectionType(nSaveTagType);
        }

        void EnableDisableCalcScript()
        {
            this.textBoxCalcScript.Enabled = this.radioButtonAoCalcFilter2.Checked;
        }

        private void radioButtonAoCalcFilter0_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableCalcScript();
        }

        private void radioButtonAoCalcFilter1_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableCalcScript();
        }

        private void radioButtonAoCalcFilter2_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableCalcScript();
        }


	}
}
