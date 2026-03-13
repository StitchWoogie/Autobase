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
	/// Summary description for PropertyPageObjectCircle.
	/// </summary>
	public class PropertyPageObjectCircle : System.Windows.Forms.Form
	{
        private GroupBox groupBox1;
        private RadioButton radioButtonType2;
        private RadioButton radioButtonType1;
        private RadioButton radioButtonType0;
        private GroupBox groupBoxAngle;
        private Label label1;
        private NumericUpDown numericUpDownSweepAngle;
        private Label label2;
        private NumericUpDown numericUpDownStartAngle;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        MultiSelectRadioButton multiSelectType = new MultiSelectRadioButton();
        MultiSelectNumericUpDown multiSelectStartAngle = new MultiSelectNumericUpDown();
        MultiSelectNumericUpDown multiSelectSweepAngle = new MultiSelectNumericUpDown();
        
		public PropertyPageObjectCircle()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            multiSelectType.Add(radioButtonType0, radioButtonType1, radioButtonType2);
            multiSelectStartAngle.Add(numericUpDownStartAngle);
            multiSelectSweepAngle.Add(numericUpDownSweepAngle);

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectCircle));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonType2 = new System.Windows.Forms.RadioButton();
            this.radioButtonType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonType0 = new System.Windows.Forms.RadioButton();
            this.groupBoxAngle = new System.Windows.Forms.GroupBox();
            this.numericUpDownSweepAngle = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownStartAngle = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBoxAngle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSweepAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartAngle)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.radioButtonType2);
            this.groupBox1.Controls.Add(this.radioButtonType1);
            this.groupBox1.Controls.Add(this.radioButtonType0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonType2
            // 
            this.radioButtonType2.AccessibleDescription = null;
            this.radioButtonType2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonType2, "radioButtonType2");
            this.radioButtonType2.BackgroundImage = null;
            this.radioButtonType2.Font = null;
            this.radioButtonType2.Name = "radioButtonType2";
            this.radioButtonType2.TabStop = true;
            this.radioButtonType2.UseVisualStyleBackColor = true;
            this.radioButtonType2.CheckedChanged += new System.EventHandler(this.radioButtonType2_CheckedChanged);
            // 
            // radioButtonType1
            // 
            this.radioButtonType1.AccessibleDescription = null;
            this.radioButtonType1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonType1, "radioButtonType1");
            this.radioButtonType1.BackgroundImage = null;
            this.radioButtonType1.Font = null;
            this.radioButtonType1.Name = "radioButtonType1";
            this.radioButtonType1.TabStop = true;
            this.radioButtonType1.UseVisualStyleBackColor = true;
            this.radioButtonType1.CheckedChanged += new System.EventHandler(this.radioButtonType1_CheckedChanged);
            // 
            // radioButtonType0
            // 
            this.radioButtonType0.AccessibleDescription = null;
            this.radioButtonType0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonType0, "radioButtonType0");
            this.radioButtonType0.BackgroundImage = null;
            this.radioButtonType0.Font = null;
            this.radioButtonType0.Name = "radioButtonType0";
            this.radioButtonType0.TabStop = true;
            this.radioButtonType0.UseVisualStyleBackColor = true;
            this.radioButtonType0.CheckedChanged += new System.EventHandler(this.radioButtonType0_CheckedChanged);
            // 
            // groupBoxAngle
            // 
            this.groupBoxAngle.AccessibleDescription = null;
            this.groupBoxAngle.AccessibleName = null;
            resources.ApplyResources(this.groupBoxAngle, "groupBoxAngle");
            this.groupBoxAngle.BackgroundImage = null;
            this.groupBoxAngle.Controls.Add(this.numericUpDownSweepAngle);
            this.groupBoxAngle.Controls.Add(this.label2);
            this.groupBoxAngle.Controls.Add(this.numericUpDownStartAngle);
            this.groupBoxAngle.Controls.Add(this.label1);
            this.groupBoxAngle.Font = null;
            this.groupBoxAngle.Name = "groupBoxAngle";
            this.groupBoxAngle.TabStop = false;
            // 
            // numericUpDownSweepAngle
            // 
            this.numericUpDownSweepAngle.AccessibleDescription = null;
            this.numericUpDownSweepAngle.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownSweepAngle, "numericUpDownSweepAngle");
            this.numericUpDownSweepAngle.DecimalPlaces = 1;
            this.numericUpDownSweepAngle.Font = null;
            this.numericUpDownSweepAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numericUpDownSweepAngle.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.numericUpDownSweepAngle.Name = "numericUpDownSweepAngle";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // numericUpDownStartAngle
            // 
            this.numericUpDownStartAngle.AccessibleDescription = null;
            this.numericUpDownStartAngle.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownStartAngle, "numericUpDownStartAngle");
            this.numericUpDownStartAngle.DecimalPlaces = 1;
            this.numericUpDownStartAngle.Font = null;
            this.numericUpDownStartAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numericUpDownStartAngle.Name = "numericUpDownStartAngle";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // PropertyPageObjectCircle
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBoxAngle);
            this.Controls.Add(this.groupBox1);
            this.Icon = null;
            this.Name = "PropertyPageObjectCircle";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxAngle.ResumeLayout(false);
            this.groupBoxAngle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSweepAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartAngle)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

        public void SetObjectArgs(ObjectArgsCircle args)
        {
            this.multiSelectType.Set(args.type);
            this.multiSelectStartAngle.Set((decimal)args.fStartAngle);
            this.multiSelectSweepAngle.Set((decimal)args.fSweepAngle);
        }

        public ObjectArgsCircle GetObjectArgs(ObjectArgsCircle org)
        {
            ObjectArgsCircle args = (ObjectArgsCircle)Tools.CopyObject(org);

            this.multiSelectType.Get(ref args.type);
            this.multiSelectStartAngle.Get(ref args.fStartAngle);
            this.multiSelectSweepAngle.Get(ref args.fSweepAngle);

            return args;
        }

        void EnableAngle()
        {
            bool flag;

            if (this.radioButtonType1.Checked || this.radioButtonType2.Checked)
                flag = true;
            else
                flag = false;

            this.groupBoxAngle.Enabled = flag;
        }

        private void radioButtonType0_CheckedChanged(object sender, EventArgs e)
        {
            EnableAngle();
        }

        private void radioButtonType1_CheckedChanged(object sender, EventArgs e)
        {
            EnableAngle();
        }

        private void radioButtonType2_CheckedChanged(object sender, EventArgs e)
        {
            EnableAngle();
        }
	}
}
