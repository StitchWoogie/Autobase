using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ReportBasicLib;
using NetTools;

namespace ReportModule
{
	/// <summary>
	/// Summary description for PropertyPageObjectMoment.
	/// </summary>
	public class PropertyPageObjectMoment : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown numericUpDownDay;
		private System.Windows.Forms.NumericUpDown numericUpDownHour;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownMinute;
		private System.Windows.Forms.Label label3;
        private Label label4;
        private ComboBox comboBoxDataType;
        private Label label5;
        private NumericUpDown numericUpDownSharpSharpValue;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertyPageObjectMoment()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectMoment));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericUpDownMinute = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownHour = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownDay = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxDataType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownSharpSharpValue = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSharpSharpValue)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numericUpDownMinute);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numericUpDownHour);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numericUpDownDay);
            this.groupBox1.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // numericUpDownMinute
            // 
            resources.ApplyResources(this.numericUpDownMinute, "numericUpDownMinute");
            this.numericUpDownMinute.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numericUpDownMinute.Name = "numericUpDownMinute";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // numericUpDownHour
            // 
            resources.ApplyResources(this.numericUpDownHour, "numericUpDownHour");
            this.numericUpDownHour.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.numericUpDownHour.Name = "numericUpDownHour";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // numericUpDownDay
            // 
            resources.ApplyResources(this.numericUpDownDay, "numericUpDownDay");
            this.numericUpDownDay.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.numericUpDownDay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownDay.Name = "numericUpDownDay";
            this.numericUpDownDay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // comboBoxDataType
            // 
            this.comboBoxDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDataType.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxDataType, "comboBoxDataType");
            this.comboBoxDataType.Name = "comboBoxDataType";
            this.comboBoxDataType.SelectedIndexChanged += new System.EventHandler(this.comboBoxDataType_SelectedIndexChanged);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // numericUpDownSharpSharpValue
            // 
            resources.ApplyResources(this.numericUpDownSharpSharpValue, "numericUpDownSharpSharpValue");
            this.numericUpDownSharpSharpValue.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDownSharpSharpValue.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownSharpSharpValue.Name = "numericUpDownSharpSharpValue";
            this.numericUpDownSharpSharpValue.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // PropertyPageObjectMoment
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.numericUpDownSharpSharpValue);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboBoxDataType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Name = "PropertyPageObjectMoment";
            this.Load += new System.EventHandler(this.PropertyPageObjectMoment_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSharpSharpValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public void SetItem(OBJECT_MOMENT_DATA obj)
		{
			NetTools.Tools.SetNumericUpDownValue(this.numericUpDownDay, obj.day);
			NetTools.Tools.SetNumericUpDownValue(this.numericUpDownHour, obj.hour);
			NetTools.Tools.SetNumericUpDownValue(this.numericUpDownMinute, obj.min);

            this.comboBoxDataType.Items.Add("Moment");
            this.comboBoxDataType.Items.Add("##MinuteAve");
            this.comboBoxDataType.Items.Add("##MinuteAve_Max");

            this.comboBoxDataType.Text = obj.data_type;

            if (this.comboBoxDataType.SelectedIndex == -1)
            {
                this.comboBoxDataType.SelectedIndex = 0;
            }

            NetTools.Tools.SetNumericUpDownValue(this.numericUpDownSharpSharpValue, obj.nSharpSharpValue);
		}

		public void GetItem(OBJECT_MOMENT_DATA obj)
		{
			obj.day = ConvertTool.ToInt32(this.numericUpDownDay.Value);
			obj.hour = ConvertTool.ToInt32(this.numericUpDownHour.Value);
			obj.min = ConvertTool.ToInt32(this.numericUpDownMinute.Value);

            obj.data_type = this.comboBoxDataType.Text;

            obj.nSharpSharpValue = ConvertTool.ToInt32(this.numericUpDownSharpSharpValue.Value);
		}

        int nTimeType = 0;

        void EnableDisable()
        {
            bool flag_min = false;
            bool flag_hour = false;
            bool flag_day = false;
            bool flag_sharpsharpvalue = false;

            string data_type = this.comboBoxDataType.Text;

            if (data_type == "##MinuteAve")
            {
                flag_sharpsharpvalue = true;

                if (nTimeType == 1)
                {
                    flag_min = true;
                }
                else if (nTimeType == 2)
                {
                    flag_min = true;
                    flag_hour = true;
                }
                else if (nTimeType == 4)
                {
                    flag_min = true;
                    flag_hour = true;
                    flag_day = true;
                }
            }
            else if (data_type == "##MinuteAve_Max")
            {
                flag_sharpsharpvalue = true;
            }
            else
            {
                if (nTimeType == 1)
                {
                    flag_min = true;
                }
                else if (nTimeType == 2)
                {
                    flag_min = true;
                    flag_hour = true;
                }
                else if (nTimeType == 4)
                {
                    flag_min = true;
                    flag_hour = true;
                    flag_day = true;
                }
            }

            this.numericUpDownDay.Enabled = flag_day;
            this.numericUpDownHour.Enabled = flag_hour;
            this.numericUpDownMinute.Enabled = flag_min;

            this.numericUpDownSharpSharpValue.Enabled = flag_sharpsharpvalue;
        }

		public void OnEventTimeType(int type)
		{
            nTimeType = type;

            EnableDisable();
            /*
			bool flag_min = false;
			bool flag_hour = false;
			bool flag_day = false;

			if(type == 1) 
			{
				flag_min = true;
			}
			else if(type == 2) 
			{
				flag_min = true;
				flag_hour = true;
			}
			else if(type == 4) 
			{
				flag_min = true;
				flag_hour = true;
				flag_day = true;
			}

			this.numericUpDownDay.Enabled = flag_day;
			this.numericUpDownHour.Enabled = flag_hour;
			this.numericUpDownMinute.Enabled = flag_min;
            */
		}

        private void PropertyPageObjectMoment_Load(object sender, EventArgs e)
        {

        }

        private void comboBoxDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }
	}
}
