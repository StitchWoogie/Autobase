using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using DatabaseConnection;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageObjectDatabaseTrend.
	/// </summary>
	public class PropertyPageObjectMultiGraph : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numericUpDownShowUnit;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown numericUpDownTimeDevide;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.NumericUpDown numericUpDownGuideDevide;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownDataTime;
		private System.Windows.Forms.CheckBox checkBoxRightToLeft;
		private System.Windows.Forms.CheckBox checkBoxDisplayRealTime;
        private GroupBox groupBox2;
        private NumericUpDown numericUpDownLogarithmicScaleBase;
        private Label label5;
        private CheckBox checkBoxLogarithmicScaleUse;
		private System.Windows.Forms.Label label12;

		public PropertyPageObjectMultiGraph()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectMultiGraph));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownDataTime = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownShowUnit = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownTimeDevide = new System.Windows.Forms.NumericUpDown();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.numericUpDownGuideDevide = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.checkBoxRightToLeft = new System.Windows.Forms.CheckBox();
            this.checkBoxDisplayRealTime = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericUpDownLogarithmicScaleBase = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBoxLogarithmicScaleUse = new System.Windows.Forms.CheckBox();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDataTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownShowUnit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeDevide)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGuideDevide)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLogarithmicScaleBase)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.numericUpDownDataTime);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.numericUpDownShowUnit);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
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
            // numericUpDownDataTime
            // 
            this.numericUpDownDataTime.AccessibleDescription = null;
            this.numericUpDownDataTime.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownDataTime, "numericUpDownDataTime");
            this.numericUpDownDataTime.Font = null;
            this.numericUpDownDataTime.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.numericUpDownDataTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownDataTime.Name = "numericUpDownDataTime";
            this.numericUpDownDataTime.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // numericUpDownShowUnit
            // 
            this.numericUpDownShowUnit.AccessibleDescription = null;
            this.numericUpDownShowUnit.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownShowUnit, "numericUpDownShowUnit");
            this.numericUpDownShowUnit.Font = null;
            this.numericUpDownShowUnit.Maximum = new decimal(new int[] {
            36000,
            0,
            0,
            0});
            this.numericUpDownShowUnit.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownShowUnit.Name = "numericUpDownShowUnit";
            this.numericUpDownShowUnit.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AccessibleDescription = null;
            this.label6.AccessibleName = null;
            resources.ApplyResources(this.label6, "label6");
            this.label6.Font = null;
            this.label6.Name = "label6";
            // 
            // label7
            // 
            this.label7.AccessibleDescription = null;
            this.label7.AccessibleName = null;
            resources.ApplyResources(this.label7, "label7");
            this.label7.Font = null;
            this.label7.Name = "label7";
            // 
            // numericUpDownTimeDevide
            // 
            this.numericUpDownTimeDevide.AccessibleDescription = null;
            this.numericUpDownTimeDevide.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownTimeDevide, "numericUpDownTimeDevide");
            this.numericUpDownTimeDevide.Font = null;
            this.numericUpDownTimeDevide.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownTimeDevide.Name = "numericUpDownTimeDevide";
            // 
            // groupBox6
            // 
            this.groupBox6.AccessibleDescription = null;
            this.groupBox6.AccessibleName = null;
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.BackgroundImage = null;
            this.groupBox6.Controls.Add(this.numericUpDownGuideDevide);
            this.groupBox6.Controls.Add(this.label11);
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.numericUpDownTimeDevide);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Font = null;
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // numericUpDownGuideDevide
            // 
            this.numericUpDownGuideDevide.AccessibleDescription = null;
            this.numericUpDownGuideDevide.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownGuideDevide, "numericUpDownGuideDevide");
            this.numericUpDownGuideDevide.Font = null;
            this.numericUpDownGuideDevide.Name = "numericUpDownGuideDevide";
            // 
            // label11
            // 
            this.label11.AccessibleDescription = null;
            this.label11.AccessibleName = null;
            resources.ApplyResources(this.label11, "label11");
            this.label11.Font = null;
            this.label11.Name = "label11";
            // 
            // label12
            // 
            this.label12.AccessibleDescription = null;
            this.label12.AccessibleName = null;
            resources.ApplyResources(this.label12, "label12");
            this.label12.Font = null;
            this.label12.Name = "label12";
            // 
            // checkBoxRightToLeft
            // 
            this.checkBoxRightToLeft.AccessibleDescription = null;
            this.checkBoxRightToLeft.AccessibleName = null;
            resources.ApplyResources(this.checkBoxRightToLeft, "checkBoxRightToLeft");
            this.checkBoxRightToLeft.BackgroundImage = null;
            this.checkBoxRightToLeft.Font = null;
            this.checkBoxRightToLeft.Name = "checkBoxRightToLeft";
            // 
            // checkBoxDisplayRealTime
            // 
            this.checkBoxDisplayRealTime.AccessibleDescription = null;
            this.checkBoxDisplayRealTime.AccessibleName = null;
            resources.ApplyResources(this.checkBoxDisplayRealTime, "checkBoxDisplayRealTime");
            this.checkBoxDisplayRealTime.BackgroundImage = null;
            this.checkBoxDisplayRealTime.Font = null;
            this.checkBoxDisplayRealTime.Name = "checkBoxDisplayRealTime";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.numericUpDownLogarithmicScaleBase);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.checkBoxLogarithmicScaleUse);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // numericUpDownLogarithmicScaleBase
            // 
            this.numericUpDownLogarithmicScaleBase.AccessibleDescription = null;
            this.numericUpDownLogarithmicScaleBase.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownLogarithmicScaleBase, "numericUpDownLogarithmicScaleBase");
            this.numericUpDownLogarithmicScaleBase.DecimalPlaces = 1;
            this.numericUpDownLogarithmicScaleBase.Font = null;
            this.numericUpDownLogarithmicScaleBase.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownLogarithmicScaleBase.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownLogarithmicScaleBase.Name = "numericUpDownLogarithmicScaleBase";
            this.numericUpDownLogarithmicScaleBase.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.Font = null;
            this.label5.Name = "label5";
            // 
            // checkBoxLogarithmicScaleUse
            // 
            this.checkBoxLogarithmicScaleUse.AccessibleDescription = null;
            this.checkBoxLogarithmicScaleUse.AccessibleName = null;
            resources.ApplyResources(this.checkBoxLogarithmicScaleUse, "checkBoxLogarithmicScaleUse");
            this.checkBoxLogarithmicScaleUse.BackgroundImage = null;
            this.checkBoxLogarithmicScaleUse.Font = null;
            this.checkBoxLogarithmicScaleUse.Name = "checkBoxLogarithmicScaleUse";
            this.checkBoxLogarithmicScaleUse.UseVisualStyleBackColor = true;
            this.checkBoxLogarithmicScaleUse.CheckedChanged += new System.EventHandler(this.checkBoxLogarithmicScaleUse_CheckedChanged);
            // 
            // PropertyPageObjectMultiGraph
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.checkBoxDisplayRealTime);
            this.Controls.Add(this.checkBoxRightToLeft);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox3);
            this.Icon = null;
            this.Name = "PropertyPageObjectMultiGraph";
            this.Load += new System.EventHandler(this.PropertyPageObjectDatabaseTrend_Load);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDataTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownShowUnit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTimeDevide)).EndInit();
            this.groupBox6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGuideDevide)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLogarithmicScaleBase)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void PropertyPageObjectDatabaseTrend_Load(object sender, System.EventArgs e)
		{
			//listDsn.ConnectionStringLoad();
			//DbTool.FillComboBox(this.comboBoxDsn, listDsn); 

            EnableLogarithmicScale();
		}

		public ObjectArgsMultiGraph ObjectArgs 
		{
			set 
			{
				//this.radioButtonDataCycle0.Checked = (value.wTimeSelectOption == 0);
				//this.radioButtonDataCycle1.Checked = (value.wTimeSelectOption == 1);
				//this.radioButtonDataCycle2.Checked = (value.wTimeSelectOption == 2);
				//this.radioButtonDataCycle3.Checked = (value.wTimeSelectOption == 3);
				//this.radioButtonDataCycle4.Checked = (value.wTimeSelectOption == 4);
				//this.radioButtonDataCycle5.Checked = (value.wTimeSelectOption == 5);
				//this.radioButtonDataCycle6.Checked = (value.wTimeSelectOption == 6);

				this.numericUpDownShowUnit.Value = value.wShowUnit;
				this.numericUpDownDataTime.Value = value.nDataTime;
				this.numericUpDownTimeDevide.Value = value.wTimeDevide;
				this.numericUpDownGuideDevide.Value = value.wLevelDevide;
				//this.numericUpDownradioButtonDataCycleUnit.Value = value.nDataCycle;

				//this.comboBoxDsn.Text = value.sDsn;
				//this.textBoxTable.Text = value.sTable;
				//this.textBoxTimeColumn.Text = value.sColumnTime;
				
				//this.textBoxMilliSecColumn.Text = value.sColumnMilli;

				//this.radioButtonDateTimeType0.Checked = (value.nDateColumnType == 0);
				//this.radioButtonDateTimeType1.Checked = (value.nDateColumnType == 1);

				//this.numericUpDownBasicSpaceLeft.Value = value.nBasicSpaceLeft;
				//this.numericUpDownBasicSpaceRight.Value = value.nBasicSpaceRight;

				this.checkBoxRightToLeft.Checked = (value.bTimeDirToLeft == 1);
				this.checkBoxDisplayRealTime.Checked = (value.bDisplayByTime == 1);

                this.checkBoxLogarithmicScaleUse.Checked = value.logarithmicScale.bUse;
                this.numericUpDownLogarithmicScaleBase.Value = Convert.ToDecimal(value.logarithmicScale.fBase);
			}
			get 
			{
				ObjectArgsMultiGraph args = new ObjectArgsMultiGraph();

				//if(this.radioButtonDataCycle0.Checked)	args.wTimeSelectOption = 0;
				//else if(this.radioButtonDataCycle1.Checked)	args.wTimeSelectOption = 1;
				//else if(this.radioButtonDataCycle2.Checked)	args.wTimeSelectOption = 2;
				//else if(this.radioButtonDataCycle3.Checked)	args.wTimeSelectOption = 3;
				//else if(this.radioButtonDataCycle4.Checked)	args.wTimeSelectOption = 4;
				//else if(this.radioButtonDataCycle5.Checked)	args.wTimeSelectOption = 5;
				//else if(this.radioButtonDataCycle6.Checked)	args.wTimeSelectOption = 6;
				//else										args.wTimeSelectOption = 2;

				args.wShowUnit = ConvertTool.ToInt32(this.numericUpDownShowUnit.Value);
				args.nDataTime = ConvertTool.ToInt32(this.numericUpDownDataTime.Value);
				args.wTimeDevide = ConvertTool.ToInt32(this.numericUpDownTimeDevide.Value);
				args.wLevelDevide = ConvertTool.ToInt32(this.numericUpDownGuideDevide.Value);

				args.bDisplayByTime = this.checkBoxDisplayRealTime.Checked ? (sbyte)1 : (sbyte)0;
				args.bTimeDirToLeft = this.checkBoxRightToLeft.Checked ? (sbyte)1 : (sbyte)0;

				
				//args.nDataCycle = ConvertTool.ToInt32(this.numericUpDownradioButtonDataCycleUnit.Value);

				//args.sDsn = this.comboBoxDsn.Text;
				//args.sTable = this.textBoxTable.Text;
				//args.sColumnTime = this.textBoxTimeColumn.Text;
				//args.sColumnMilli = this.textBoxMilliSecColumn.Text;

				//if(this.radioButtonDateTimeType0.Checked)		args.nDateColumnType = 0;
				//else if(this.radioButtonDateTimeType1.Checked)	args.nDateColumnType = 1;
				//else											args.nDateColumnType = 0;

				//args.nBasicSpaceLeft = ConvertTool.ToInt32(this.numericUpDownBasicSpaceLeft.Value);
				//args.nBasicSpaceRight = ConvertTool.ToInt32(this.numericUpDownBasicSpaceRight.Value);

                args.logarithmicScale.bUse = this.checkBoxLogarithmicScaleUse.Checked;
                args.logarithmicScale.fBase = Convert.ToDouble(this.numericUpDownLogarithmicScaleBase.Value);

				return args;
			}
		}

        void EnableLogarithmicScale()
        {
            bool flag = this.checkBoxLogarithmicScaleUse.Checked;

            this.numericUpDownLogarithmicScaleBase.Enabled = flag;
        }

        private void checkBoxLogarithmicScaleUse_CheckedChanged(object sender, EventArgs e)
        {
            EnableLogarithmicScale();
        }
	}
}
