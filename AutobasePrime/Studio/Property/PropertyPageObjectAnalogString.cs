using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using NetTools;
using AutoLibLocal;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageObjectAnalogString.
	/// </summary>
	public class PropertyPageObjectAnalogString : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonBorder0;
		private System.Windows.Forms.RadioButton radioButtonBorder1;
		private System.Windows.Forms.RadioButton radioButtonBorder2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButtonDisplayValue1;
		private System.Windows.Forms.RadioButton radioButtonDisplayValue0;
		private System.Windows.Forms.RadioButton radioButtonBorder3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		MultiSelectRadioButton multiSelectBorder = new MultiSelectRadioButton();
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox textBoxDisplayFormat;
		MultiSelectRadioButton multiSelectDisplayValue = new MultiSelectRadioButton();
        private CheckBox checkBoxDisplayUnit;
        private GroupBox groupBox4;
        private RadioButton radioButtonHorzAlign2;
        private RadioButton radioButtonHorzAlign1;
        private RadioButton radioButtonHorzAlign0;
		MultiSelectTextBox multiSelectDisplayFormat = new MultiSelectTextBox();
        MultiSelectRadioButton multiSelectHorzAlign = new MultiSelectRadioButton();
        MultiSelectCheckBox multiSelectDisplayUnit = new MultiSelectCheckBox();

		public PropertyPageObjectAnalogString()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			multiSelectBorder.Add(this.radioButtonBorder0,
									this.radioButtonBorder1,
									this.radioButtonBorder2,
									this.radioButtonBorder3);
			
			multiSelectDisplayValue.Add(this.radioButtonDisplayValue0,
										this.radioButtonDisplayValue1);

			multiSelectDisplayFormat.Add(this.textBoxDisplayFormat);

            multiSelectDisplayUnit.Add(this.checkBoxDisplayUnit);
            multiSelectHorzAlign.Add(this.radioButtonHorzAlign0,
                                        this.radioButtonHorzAlign1,
                                        this.radioButtonHorzAlign2);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectAnalogString));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonBorder3 = new System.Windows.Forms.RadioButton();
            this.radioButtonBorder2 = new System.Windows.Forms.RadioButton();
            this.radioButtonBorder1 = new System.Windows.Forms.RadioButton();
            this.radioButtonBorder0 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonDisplayValue1 = new System.Windows.Forms.RadioButton();
            this.radioButtonDisplayValue0 = new System.Windows.Forms.RadioButton();
            this.textBoxDisplayFormat = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxDisplayUnit = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButtonHorzAlign2 = new System.Windows.Forms.RadioButton();
            this.radioButtonHorzAlign1 = new System.Windows.Forms.RadioButton();
            this.radioButtonHorzAlign0 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonBorder3);
            this.groupBox1.Controls.Add(this.radioButtonBorder2);
            this.groupBox1.Controls.Add(this.radioButtonBorder1);
            this.groupBox1.Controls.Add(this.radioButtonBorder0);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // radioButtonBorder3
            // 
            resources.ApplyResources(this.radioButtonBorder3, "radioButtonBorder3");
            this.radioButtonBorder3.Name = "radioButtonBorder3";
            this.radioButtonBorder3.CheckedChanged += new System.EventHandler(this.radioButtonBorder3_CheckedChanged);
            // 
            // radioButtonBorder2
            // 
            resources.ApplyResources(this.radioButtonBorder2, "radioButtonBorder2");
            this.radioButtonBorder2.Name = "radioButtonBorder2";
            this.radioButtonBorder2.CheckedChanged += new System.EventHandler(this.radioButtonBorder2_CheckedChanged);
            // 
            // radioButtonBorder1
            // 
            resources.ApplyResources(this.radioButtonBorder1, "radioButtonBorder1");
            this.radioButtonBorder1.Name = "radioButtonBorder1";
            this.radioButtonBorder1.CheckedChanged += new System.EventHandler(this.radioButtonBorder1_CheckedChanged);
            // 
            // radioButtonBorder0
            // 
            resources.ApplyResources(this.radioButtonBorder0, "radioButtonBorder0");
            this.radioButtonBorder0.Name = "radioButtonBorder0";
            this.radioButtonBorder0.CheckedChanged += new System.EventHandler(this.radioButtonBorder0_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonDisplayValue1);
            this.groupBox2.Controls.Add(this.radioButtonDisplayValue0);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonDisplayValue1
            // 
            resources.ApplyResources(this.radioButtonDisplayValue1, "radioButtonDisplayValue1");
            this.radioButtonDisplayValue1.Name = "radioButtonDisplayValue1";
            // 
            // radioButtonDisplayValue0
            // 
            resources.ApplyResources(this.radioButtonDisplayValue0, "radioButtonDisplayValue0");
            this.radioButtonDisplayValue0.Name = "radioButtonDisplayValue0";
            // 
            // textBoxDisplayFormat
            // 
            resources.ApplyResources(this.textBoxDisplayFormat, "textBoxDisplayFormat");
            this.textBoxDisplayFormat.Name = "textBoxDisplayFormat";
            this.textBoxDisplayFormat.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxDisplayFormat);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // checkBoxDisplayUnit
            // 
            resources.ApplyResources(this.checkBoxDisplayUnit, "checkBoxDisplayUnit");
            this.checkBoxDisplayUnit.Name = "checkBoxDisplayUnit";
            this.checkBoxDisplayUnit.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButtonHorzAlign2);
            this.groupBox4.Controls.Add(this.radioButtonHorzAlign1);
            this.groupBox4.Controls.Add(this.radioButtonHorzAlign0);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // radioButtonHorzAlign2
            // 
            resources.ApplyResources(this.radioButtonHorzAlign2, "radioButtonHorzAlign2");
            this.radioButtonHorzAlign2.Name = "radioButtonHorzAlign2";
            // 
            // radioButtonHorzAlign1
            // 
            resources.ApplyResources(this.radioButtonHorzAlign1, "radioButtonHorzAlign1");
            this.radioButtonHorzAlign1.Name = "radioButtonHorzAlign1";
            // 
            // radioButtonHorzAlign0
            // 
            resources.ApplyResources(this.radioButtonHorzAlign0, "radioButtonHorzAlign0");
            this.radioButtonHorzAlign0.Name = "radioButtonHorzAlign0";
            // 
            // PropertyPageObjectAnalogString
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.checkBoxDisplayUnit);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "PropertyPageObjectAnalogString";
            this.Load += new System.EventHandler(this.PropertyPageObjectAnalogString_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        bool bPageLoading = false;

		public void SetObjectArgs(ObjectArgsAnalogString args) 
		{
            bPageLoading = true;
			multiSelectBorder.Set(args.nBoxUse);
			multiSelectDisplayValue.Set(args.nDisplayValue);
			multiSelectDisplayFormat.Set(args.sDisplayFormat);
            multiSelectDisplayUnit.Set(args.bDisplayUnit);
            multiSelectHorzAlign.Set(args.nHorzAlign);
            bPageLoading = false;

            
		}

		public ObjectArgsAnalogString GetObjectArgs(ObjectArgsAnalogString org) 
		{
			ObjectArgsAnalogString args = (ObjectArgsAnalogString)Tools.CopyObject(org);
			
			multiSelectBorder.Get(ref args.nBoxUse);
			multiSelectDisplayValue.Get(ref args.nDisplayValue);
			multiSelectDisplayFormat.Get(ref args.sDisplayFormat);
            multiSelectDisplayUnit.Get(ref args.bDisplayUnit);
            multiSelectHorzAlign.Get(ref args.nHorzAlign);

			return args;
		}

		private void groupBox1_Enter(object sender, System.EventArgs e)
		{
		
		}

		private void textBox1_TextChanged(object sender, System.EventArgs e)
		{
		
		}

		private void PropertyPageObjectAnalogString_Load(object sender, System.EventArgs e)
		{
            
		}

        void EnableDisableBackColorTab()
        {
            if (TotalConfig.eOemType != EnumOemType.MBSENGSCADA) return;    // 혹시 동작이 잘 안될수도 있어서 당분간 MBSENGSCADA 만 적용한다.

            if (bPageLoading) return;

            int type;

            if (radioButtonBorder0.Checked) type = 0;
            else if (radioButtonBorder1.Checked) type = 1;
            else if (radioButtonBorder2.Checked) type = 2;
            else if (radioButtonBorder3.Checked) type = 3;
            else type = 0;

            bool flag;
            flag = (type == 3) ? false : true;

            ClassEditProperty.propertySheet.EnableDisableTabPage("BackColor", flag);
        }

        private void radioButtonBorder0_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableBackColorTab();
        }

        private void radioButtonBorder1_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableBackColorTab();
        }

        private void radioButtonBorder2_CheckedChanged(object sender, System.EventArgs e)
        {
            EnableDisableBackColorTab();
        }

        private void radioButtonBorder3_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableBackColorTab();
        }

        public void AfterSheetRun()
        {
            EnableDisableBackColorTab();
        }
	}
}
