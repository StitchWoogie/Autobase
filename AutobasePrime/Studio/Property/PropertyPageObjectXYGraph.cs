using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageObjectXYGraph.
	/// </summary>
	public class PropertyPageObjectXYGraph : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label ShowUnit;
		private System.Windows.Forms.NumericUpDown numericUpDownShowUnit;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown numericUpDownDataTime;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown numericUpDownDevideY;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown numericUpDownDevideX;
		private System.Windows.Forms.Label label8;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PropertyPageObjectXYGraph()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectXYGraph));
            this.ShowUnit = new System.Windows.Forms.Label();
            this.numericUpDownShowUnit = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownDataTime = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownDevideY = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownDevideX = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownShowUnit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDataTime)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDevideY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDevideX)).BeginInit();
            this.SuspendLayout();
            // 
            // ShowUnit
            // 
            this.ShowUnit.AccessibleDescription = null;
            this.ShowUnit.AccessibleName = null;
            resources.ApplyResources(this.ShowUnit, "ShowUnit");
            this.ShowUnit.Font = null;
            this.ShowUnit.Name = "ShowUnit";
            // 
            // numericUpDownShowUnit
            // 
            this.numericUpDownShowUnit.AccessibleDescription = null;
            this.numericUpDownShowUnit.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownShowUnit, "numericUpDownShowUnit");
            this.numericUpDownShowUnit.Font = null;
            this.numericUpDownShowUnit.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownShowUnit.Name = "numericUpDownShowUnit";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
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
            1,
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
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.numericUpDownDevideY);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.numericUpDownDevideX);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.Font = null;
            this.label5.Name = "label5";
            // 
            // numericUpDownDevideY
            // 
            this.numericUpDownDevideY.AccessibleDescription = null;
            this.numericUpDownDevideY.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownDevideY, "numericUpDownDevideY");
            this.numericUpDownDevideY.Font = null;
            this.numericUpDownDevideY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownDevideY.Name = "numericUpDownDevideY";
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
            // numericUpDownDevideX
            // 
            this.numericUpDownDevideX.AccessibleDescription = null;
            this.numericUpDownDevideX.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownDevideX, "numericUpDownDevideX");
            this.numericUpDownDevideX.Font = null;
            this.numericUpDownDevideX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownDevideX.Name = "numericUpDownDevideX";
            // 
            // label8
            // 
            this.label8.AccessibleDescription = null;
            this.label8.AccessibleName = null;
            resources.ApplyResources(this.label8, "label8");
            this.label8.Font = null;
            this.label8.Name = "label8";
            // 
            // PropertyPageObjectXYGraph
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericUpDownDataTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericUpDownShowUnit);
            this.Controls.Add(this.ShowUnit);
            this.Icon = null;
            this.Name = "PropertyPageObjectXYGraph";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownShowUnit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDataTime)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDevideY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDevideX)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		public ObjectArgsXYGraph ObjectArgs 
		{
			set 
			{
				this.numericUpDownDataTime.Value = value.nDataTime;
				this.numericUpDownDevideX.Value = value.wTimeDevide;
				this.numericUpDownDevideY.Value = value.leveldevide;
				this.numericUpDownShowUnit.Value = value.showunit;
			}
			get 
			{
				ObjectArgsXYGraph args = new ObjectArgsXYGraph();

				args.nDataTime = ConvertTool.ToInt32(this.numericUpDownDataTime.Value);
				args.wTimeDevide = ConvertTool.ToInt32(this.numericUpDownDevideX.Value);
				args.leveldevide = ConvertTool.ToInt32(this.numericUpDownDevideY.Value);
				args.showunit = ConvertTool.ToInt32(this.numericUpDownShowUnit.Value);

				return args;
			}
		}
	}
}
