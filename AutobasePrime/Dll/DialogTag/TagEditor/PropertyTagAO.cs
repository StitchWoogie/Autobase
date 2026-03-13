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
	public class PropertyTagAO : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
        private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label label32;
		private System.Windows.Forms.TextBox textBoxAoPlcBase;
		private System.Windows.Forms.Label label34;
		private System.Windows.Forms.TextBox textBoxAoPlcFull;
		private System.Windows.Forms.Label label36;
		private System.Windows.Forms.TextBox textBoxAoBase;
		private System.Windows.Forms.Label label37;
		private System.Windows.Forms.TextBox textBoxAoFull;
		private System.Windows.Forms.Label label38;
		private System.Windows.Forms.RadioButton radioButtonAoCalcFilter1;
		private System.Windows.Forms.RadioButton radioButtonAoCalcFilter0;
		private System.Windows.Forms.RadioButton radioButtonAoCut1;
		private System.Windows.Forms.RadioButton radioButtonAoCut0;
		private System.Windows.Forms.TextBox textBoxAoAddress;
		private System.Windows.Forms.Label label44;
		private System.Windows.Forms.TextBox textBoxAoPort;
		private System.Windows.Forms.Label label45;
		private System.Windows.Forms.CheckBox checkBoxAoBcd;
		private System.Windows.Forms.TextBox textBoxAoStation;
		private System.Windows.Forms.Label label31;
		private System.Windows.Forms.TextBox textBoxAoExtra1;
		private System.Windows.Forms.Label label39;
		private System.Windows.Forms.TextBox textBoxAoExtra2;
		private System.Windows.Forms.Label label40;
		private System.Windows.Forms.RadioButton radioButtonAoDdeData1;
		private System.Windows.Forms.RadioButton radioButtonAoDdeData0;

		MultiSelectTextBox multiAoPort = new MultiSelectTextBox();
		MultiSelectTextBox multiAoStation = new MultiSelectTextBox();
		MultiSelectTextBox multiAoAddress = new MultiSelectTextBox(true);
		MultiSelectTextBox multiAoExtra1 = new MultiSelectTextBox();
		MultiSelectTextBox multiAoExtra2 = new MultiSelectTextBox();
		MultiSelectTextBox multiAoFull = new MultiSelectTextBox();
		MultiSelectTextBox multiAoBase = new MultiSelectTextBox();
		MultiSelectTextBox multiAoPlcFull = new MultiSelectTextBox();
		MultiSelectTextBox multiAoPlcBase = new MultiSelectTextBox();
        MultiSelectComboBox multiAoUnit = new MultiSelectComboBox();
		
		MultiSelectCheckBox multiAoBcd = new MultiSelectCheckBox();
		MultiSelectRadioButton multiAoFilter = new MultiSelectRadioButton();
		MultiSelectRadioButton multiAoCut = new MultiSelectRadioButton();

		private System.Windows.Forms.GroupBox groupBoxConversion;
		private System.Windows.Forms.GroupBox groupBoxCalcFilter;
		private System.Windows.Forms.GroupBox groupBoxOverflow;
		private System.Windows.Forms.GroupBox groupBoxPlcScan;
		private System.Windows.Forms.GroupBox groupBoxDdeData;
        private RadioButton radioButtonAoCalcFilter2;
        private GroupBox groupBox2;
        private TextBox textBoxCalcScript;
		MultiSelectRadioButton multiAoDdeData = new MultiSelectRadioButton();
        private RadioButton radioButtonAoCut2;
        private ComboBox comboBoxAoUnit;
        MultiSelectTextBox multiAoCalcScript = new MultiSelectTextBox();

		public PropertyTagAO()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			// AO
			multiAoPort.Add(this.textBoxAoPort);
			multiAoStation.Add(this.textBoxAoStation);
			multiAoAddress.Add(this.textBoxAoAddress);
			multiAoExtra1.Add(this.textBoxAoExtra1);
			multiAoExtra2.Add(this.textBoxAoExtra2);
			multiAoFull.Add(this.textBoxAoFull);
			multiAoBase.Add(this.textBoxAoBase);
			multiAoPlcFull.Add(this.textBoxAoPlcFull);
			multiAoPlcBase.Add(this.textBoxAoPlcBase);
			multiAoUnit.Add(this.comboBoxAoUnit);
			multiAoBcd.Add(this.checkBoxAoBcd);
			multiAoFilter.Add(this.radioButtonAoCalcFilter0, radioButtonAoCalcFilter1, radioButtonAoCalcFilter2);
			multiAoCut.Add(this.radioButtonAoCut0, radioButtonAoCut1, radioButtonAoCut2);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyTagAO));
            this.checkBoxAoBcd = new System.Windows.Forms.CheckBox();
            this.label32 = new System.Windows.Forms.Label();
            this.groupBoxConversion = new System.Windows.Forms.GroupBox();
            this.textBoxAoPlcBase = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.textBoxAoPlcFull = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.textBoxAoBase = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.textBoxAoFull = new System.Windows.Forms.TextBox();
            this.label38 = new System.Windows.Forms.Label();
            this.groupBoxCalcFilter = new System.Windows.Forms.GroupBox();
            this.radioButtonAoCalcFilter2 = new System.Windows.Forms.RadioButton();
            this.radioButtonAoCalcFilter1 = new System.Windows.Forms.RadioButton();
            this.radioButtonAoCalcFilter0 = new System.Windows.Forms.RadioButton();
            this.groupBoxOverflow = new System.Windows.Forms.GroupBox();
            this.radioButtonAoCut2 = new System.Windows.Forms.RadioButton();
            this.radioButtonAoCut1 = new System.Windows.Forms.RadioButton();
            this.radioButtonAoCut0 = new System.Windows.Forms.RadioButton();
            this.groupBoxPlcScan = new System.Windows.Forms.GroupBox();
            this.textBoxAoExtra2 = new System.Windows.Forms.TextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.textBoxAoExtra1 = new System.Windows.Forms.TextBox();
            this.label39 = new System.Windows.Forms.Label();
            this.textBoxAoStation = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.textBoxAoAddress = new System.Windows.Forms.TextBox();
            this.label44 = new System.Windows.Forms.Label();
            this.textBoxAoPort = new System.Windows.Forms.TextBox();
            this.label45 = new System.Windows.Forms.Label();
            this.groupBoxDdeData = new System.Windows.Forms.GroupBox();
            this.radioButtonAoDdeData1 = new System.Windows.Forms.RadioButton();
            this.radioButtonAoDdeData0 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxCalcScript = new System.Windows.Forms.TextBox();
            this.comboBoxAoUnit = new System.Windows.Forms.ComboBox();
            this.groupBoxConversion.SuspendLayout();
            this.groupBoxCalcFilter.SuspendLayout();
            this.groupBoxOverflow.SuspendLayout();
            this.groupBoxPlcScan.SuspendLayout();
            this.groupBoxDdeData.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxAoBcd
            // 
            resources.ApplyResources(this.checkBoxAoBcd, "checkBoxAoBcd");
            this.checkBoxAoBcd.Name = "checkBoxAoBcd";
            // 
            // label32
            // 
            resources.ApplyResources(this.label32, "label32");
            this.label32.Name = "label32";
            // 
            // groupBoxConversion
            // 
            this.groupBoxConversion.Controls.Add(this.textBoxAoPlcBase);
            this.groupBoxConversion.Controls.Add(this.label34);
            this.groupBoxConversion.Controls.Add(this.textBoxAoPlcFull);
            this.groupBoxConversion.Controls.Add(this.label36);
            this.groupBoxConversion.Controls.Add(this.textBoxAoBase);
            this.groupBoxConversion.Controls.Add(this.label37);
            this.groupBoxConversion.Controls.Add(this.textBoxAoFull);
            this.groupBoxConversion.Controls.Add(this.label38);
            resources.ApplyResources(this.groupBoxConversion, "groupBoxConversion");
            this.groupBoxConversion.Name = "groupBoxConversion";
            this.groupBoxConversion.TabStop = false;
            // 
            // textBoxAoPlcBase
            // 
            resources.ApplyResources(this.textBoxAoPlcBase, "textBoxAoPlcBase");
            this.textBoxAoPlcBase.Name = "textBoxAoPlcBase";
            // 
            // label34
            // 
            resources.ApplyResources(this.label34, "label34");
            this.label34.Name = "label34";
            // 
            // textBoxAoPlcFull
            // 
            resources.ApplyResources(this.textBoxAoPlcFull, "textBoxAoPlcFull");
            this.textBoxAoPlcFull.Name = "textBoxAoPlcFull";
            // 
            // label36
            // 
            resources.ApplyResources(this.label36, "label36");
            this.label36.Name = "label36";
            // 
            // textBoxAoBase
            // 
            resources.ApplyResources(this.textBoxAoBase, "textBoxAoBase");
            this.textBoxAoBase.Name = "textBoxAoBase";
            // 
            // label37
            // 
            resources.ApplyResources(this.label37, "label37");
            this.label37.Name = "label37";
            // 
            // textBoxAoFull
            // 
            resources.ApplyResources(this.textBoxAoFull, "textBoxAoFull");
            this.textBoxAoFull.Name = "textBoxAoFull";
            // 
            // label38
            // 
            resources.ApplyResources(this.label38, "label38");
            this.label38.Name = "label38";
            // 
            // groupBoxCalcFilter
            // 
            this.groupBoxCalcFilter.Controls.Add(this.radioButtonAoCalcFilter2);
            this.groupBoxCalcFilter.Controls.Add(this.radioButtonAoCalcFilter1);
            this.groupBoxCalcFilter.Controls.Add(this.radioButtonAoCalcFilter0);
            resources.ApplyResources(this.groupBoxCalcFilter, "groupBoxCalcFilter");
            this.groupBoxCalcFilter.Name = "groupBoxCalcFilter";
            this.groupBoxCalcFilter.TabStop = false;
            // 
            // radioButtonAoCalcFilter2
            // 
            resources.ApplyResources(this.radioButtonAoCalcFilter2, "radioButtonAoCalcFilter2");
            this.radioButtonAoCalcFilter2.Name = "radioButtonAoCalcFilter2";
            this.radioButtonAoCalcFilter2.CheckedChanged += new System.EventHandler(this.radioButtonAoCalcFilter2_CheckedChanged);
            // 
            // radioButtonAoCalcFilter1
            // 
            resources.ApplyResources(this.radioButtonAoCalcFilter1, "radioButtonAoCalcFilter1");
            this.radioButtonAoCalcFilter1.Name = "radioButtonAoCalcFilter1";
            this.radioButtonAoCalcFilter1.CheckedChanged += new System.EventHandler(this.radioButtonAoCalcFilter1_CheckedChanged);
            // 
            // radioButtonAoCalcFilter0
            // 
            resources.ApplyResources(this.radioButtonAoCalcFilter0, "radioButtonAoCalcFilter0");
            this.radioButtonAoCalcFilter0.Name = "radioButtonAoCalcFilter0";
            this.radioButtonAoCalcFilter0.CheckedChanged += new System.EventHandler(this.radioButtonAoCalcFilter0_CheckedChanged);
            // 
            // groupBoxOverflow
            // 
            this.groupBoxOverflow.Controls.Add(this.radioButtonAoCut2);
            this.groupBoxOverflow.Controls.Add(this.radioButtonAoCut1);
            this.groupBoxOverflow.Controls.Add(this.radioButtonAoCut0);
            resources.ApplyResources(this.groupBoxOverflow, "groupBoxOverflow");
            this.groupBoxOverflow.Name = "groupBoxOverflow";
            this.groupBoxOverflow.TabStop = false;
            // 
            // radioButtonAoCut2
            // 
            resources.ApplyResources(this.radioButtonAoCut2, "radioButtonAoCut2");
            this.radioButtonAoCut2.Name = "radioButtonAoCut2";
            // 
            // radioButtonAoCut1
            // 
            resources.ApplyResources(this.radioButtonAoCut1, "radioButtonAoCut1");
            this.radioButtonAoCut1.Name = "radioButtonAoCut1";
            // 
            // radioButtonAoCut0
            // 
            resources.ApplyResources(this.radioButtonAoCut0, "radioButtonAoCut0");
            this.radioButtonAoCut0.Name = "radioButtonAoCut0";
            // 
            // groupBoxPlcScan
            // 
            this.groupBoxPlcScan.Controls.Add(this.textBoxAoExtra2);
            this.groupBoxPlcScan.Controls.Add(this.label40);
            this.groupBoxPlcScan.Controls.Add(this.textBoxAoExtra1);
            this.groupBoxPlcScan.Controls.Add(this.label39);
            this.groupBoxPlcScan.Controls.Add(this.textBoxAoStation);
            this.groupBoxPlcScan.Controls.Add(this.label31);
            this.groupBoxPlcScan.Controls.Add(this.textBoxAoAddress);
            this.groupBoxPlcScan.Controls.Add(this.label44);
            this.groupBoxPlcScan.Controls.Add(this.textBoxAoPort);
            this.groupBoxPlcScan.Controls.Add(this.label45);
            resources.ApplyResources(this.groupBoxPlcScan, "groupBoxPlcScan");
            this.groupBoxPlcScan.Name = "groupBoxPlcScan";
            this.groupBoxPlcScan.TabStop = false;
            // 
            // textBoxAoExtra2
            // 
            resources.ApplyResources(this.textBoxAoExtra2, "textBoxAoExtra2");
            this.textBoxAoExtra2.Name = "textBoxAoExtra2";
            // 
            // label40
            // 
            resources.ApplyResources(this.label40, "label40");
            this.label40.Name = "label40";
            // 
            // textBoxAoExtra1
            // 
            resources.ApplyResources(this.textBoxAoExtra1, "textBoxAoExtra1");
            this.textBoxAoExtra1.Name = "textBoxAoExtra1";
            // 
            // label39
            // 
            resources.ApplyResources(this.label39, "label39");
            this.label39.Name = "label39";
            // 
            // textBoxAoStation
            // 
            resources.ApplyResources(this.textBoxAoStation, "textBoxAoStation");
            this.textBoxAoStation.Name = "textBoxAoStation";
            // 
            // label31
            // 
            resources.ApplyResources(this.label31, "label31");
            this.label31.Name = "label31";
            // 
            // textBoxAoAddress
            // 
            resources.ApplyResources(this.textBoxAoAddress, "textBoxAoAddress");
            this.textBoxAoAddress.Name = "textBoxAoAddress";
            // 
            // label44
            // 
            resources.ApplyResources(this.label44, "label44");
            this.label44.Name = "label44";
            // 
            // textBoxAoPort
            // 
            resources.ApplyResources(this.textBoxAoPort, "textBoxAoPort");
            this.textBoxAoPort.Name = "textBoxAoPort";
            // 
            // label45
            // 
            resources.ApplyResources(this.label45, "label45");
            this.label45.Name = "label45";
            // 
            // groupBoxDdeData
            // 
            this.groupBoxDdeData.Controls.Add(this.radioButtonAoDdeData1);
            this.groupBoxDdeData.Controls.Add(this.radioButtonAoDdeData0);
            resources.ApplyResources(this.groupBoxDdeData, "groupBoxDdeData");
            this.groupBoxDdeData.Name = "groupBoxDdeData";
            this.groupBoxDdeData.TabStop = false;
            // 
            // radioButtonAoDdeData1
            // 
            resources.ApplyResources(this.radioButtonAoDdeData1, "radioButtonAoDdeData1");
            this.radioButtonAoDdeData1.Name = "radioButtonAoDdeData1";
            // 
            // radioButtonAoDdeData0
            // 
            resources.ApplyResources(this.radioButtonAoDdeData0, "radioButtonAoDdeData0");
            this.radioButtonAoDdeData0.Name = "radioButtonAoDdeData0";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxCalcScript);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // textBoxCalcScript
            // 
            resources.ApplyResources(this.textBoxCalcScript, "textBoxCalcScript");
            this.textBoxCalcScript.Name = "textBoxCalcScript";
            // 
            // comboBoxAoUnit
            // 
            this.comboBoxAoUnit.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxAoUnit, "comboBoxAoUnit");
            this.comboBoxAoUnit.Name = "comboBoxAoUnit";
            // 
            // PropertyTagAO
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.comboBoxAoUnit);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label32);
            this.Controls.Add(this.groupBoxConversion);
            this.Controls.Add(this.groupBoxDdeData);
            this.Controls.Add(this.groupBoxCalcFilter);
            this.Controls.Add(this.groupBoxOverflow);
            this.Controls.Add(this.groupBoxPlcScan);
            this.Controls.Add(this.checkBoxAoBcd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertyTagAO";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.PropertyTagAO_Load);
            this.groupBoxConversion.ResumeLayout(false);
            this.groupBoxConversion.PerformLayout();
            this.groupBoxCalcFilter.ResumeLayout(false);
            this.groupBoxOverflow.ResumeLayout(false);
            this.groupBoxPlcScan.ResumeLayout(false);
            this.groupBoxPlcScan.PerformLayout();
            this.groupBoxDdeData.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion


		public void SetTagAO(TagAoClass ao)
		{
			multiAoPort.Set(ao.port);
			multiAoStation.Set(ao.station);
			multiAoAddress.Set(ao.address);
			multiAoExtra1.Set(ao.sExtraAddr);
			multiAoExtra2.Set(ao.wExtraAddr);
			multiAoFull.Set(ao.fFull);
			multiAoBase.Set(ao.fBase);
			multiAoPlcFull.Set(ao.plc_full);
			multiAoPlcBase.Set(ao.plc_base);
			multiAoUnit.Set(ao.unit);

			multiAoBcd.Set(ao.bBcdValue);
			multiAoFilter.Set(ao.nCalculateFilter);
			multiAoCut.Set(ao.bCutOverValue);
			multiAoDdeData.Set(ao.cDdeDataFormat);
            multiAoCalcScript.Set(ao.sCalcScript);

		}

		public void GetTagAO(TagAoClass ao)
		{
			multiAoPort.Get(ref ao.port);
			multiAoStation.Get(ref ao.station);
			multiAoAddress.Get(ref ao.address);
			multiAoExtra1.Get(ref ao.sExtraAddr);
			multiAoExtra2.Get(ref ao.wExtraAddr);
			multiAoFull.Get(ref ao.fFull);
			multiAoBase.Get(ref ao.fBase);
			multiAoPlcFull.Get(ref ao.plc_full);
			multiAoPlcBase.Get(ref ao.plc_base);
			multiAoUnit.Get(ref ao.unit);

			multiAoBcd.Get(ref ao.bBcdValue);
			multiAoFilter.Get(ref ao.nCalculateFilter);
			multiAoCut.Get(ref ao.bCutOverValue);
			multiAoDdeData.Get(ref ao.cDdeDataFormat);
            multiAoCalcScript.Get(ref ao.sCalcScript);
		}

		public void EnableDisableConnectionType(int type)
		{
			bool flag_plcscan = true;
			bool flag_calcfilter = true;
			bool flag_ddedata = true;

			if(type == 0) 
			{
				flag_ddedata = false;
			}
			else if(type == 1) 
			{
				flag_plcscan = false;
			}
			else if(type == 2) 
			{
				flag_plcscan = false;
				flag_calcfilter = false;
				flag_ddedata = false;
			}
			else if(type == 3) 
			{
				flag_plcscan = false;	
				flag_calcfilter = false;
				flag_ddedata = false;
			}
			else if(type == 4) 
			{
				flag_plcscan = false;
				flag_calcfilter = false;
				flag_ddedata = false;
			}
			else if(type == 5) 
			{
				flag_plcscan = false;
				flag_calcfilter = false;
				flag_ddedata = false;
			}
			
			this.groupBoxPlcScan.Enabled = flag_plcscan;
			this.groupBoxCalcFilter.Enabled = flag_calcfilter;
			this.groupBoxConversion.Enabled = flag_calcfilter;
			this.groupBoxDdeData.Enabled = flag_ddedata;
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

        private void PropertyTagAO_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < PropertyTagAI.sSampleUnit.Length; i++)
            {
                this.comboBoxAoUnit.Items.Add(PropertyTagAI.sSampleUnit[i]);
            }
        }

	}
}
