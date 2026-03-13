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
	/// Summary description for PropertyPageObjectRectangle.
	/// </summary>
	public class PropertyPageObjectRoundRectangle : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.NumericUpDown numericUpDownRoundX;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericUpDownRoundY;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		MultiSelectNumericUpDown multiSelectRoundX = new MultiSelectNumericUpDown();
		MultiSelectNumericUpDown multiSelectRoundY = new MultiSelectNumericUpDown();

		public PropertyPageObjectRoundRectangle()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			multiSelectRoundX.Add(this.numericUpDownRoundX);
			multiSelectRoundY.Add(this.numericUpDownRoundY);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectRoundRectangle));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownRoundY = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownRoundX = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRoundY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRoundX)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numericUpDownRoundY);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericUpDownRoundX);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // numericUpDownRoundY
            // 
            this.numericUpDownRoundY.AccessibleDescription = null;
            this.numericUpDownRoundY.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownRoundY, "numericUpDownRoundY");
            this.numericUpDownRoundY.Font = null;
            this.numericUpDownRoundY.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownRoundY.Name = "numericUpDownRoundY";
            this.numericUpDownRoundY.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // numericUpDownRoundX
            // 
            this.numericUpDownRoundX.AccessibleDescription = null;
            this.numericUpDownRoundX.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownRoundX, "numericUpDownRoundX");
            this.numericUpDownRoundX.Font = null;
            this.numericUpDownRoundX.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownRoundX.Name = "numericUpDownRoundX";
            this.numericUpDownRoundX.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // PropertyPageObjectRoundRectangle
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox1);
            this.Icon = null;
            this.Name = "PropertyPageObjectRoundRectangle";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRoundY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRoundX)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		public void SetObjectArgs(ObjectArgsRoundRectangle args) 
		{
			this.multiSelectRoundX.Set(args.round_x);
			this.multiSelectRoundY.Set(args.round_y);
		}

		public ObjectArgsRoundRectangle GetObjectArgs(ObjectArgsRoundRectangle org) 
		{
			ObjectArgsRoundRectangle args = (ObjectArgsRoundRectangle)Tools.CopyObject(org);
			this.multiSelectRoundX.Get(ref args.round_x);
			this.multiSelectRoundY.Get(ref args.round_y);

			return args;
		}
	}
}
