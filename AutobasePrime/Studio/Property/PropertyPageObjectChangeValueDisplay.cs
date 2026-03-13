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
	/// Summary description for PropertyPageObjectChangeValueDisplay.
	/// </summary>
	public class PropertyPageObjectChangeValueDisplay : System.Windows.Forms.Form
	{
		private System.Windows.Forms.NumericUpDown numericUpDownListCount;
		private System.Windows.Forms.GroupBox groupBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		MultiSelectNumericUpDown multiSelectListCount = new MultiSelectNumericUpDown();

		public PropertyPageObjectChangeValueDisplay()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.multiSelectListCount.Add(this.numericUpDownListCount);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectChangeValueDisplay));
            this.numericUpDownListCount = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownListCount)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // numericUpDownListCount
            // 
            this.numericUpDownListCount.AccessibleDescription = null;
            this.numericUpDownListCount.AccessibleName = null;
            resources.ApplyResources(this.numericUpDownListCount, "numericUpDownListCount");
            this.numericUpDownListCount.Font = null;
            this.numericUpDownListCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownListCount.Name = "numericUpDownListCount";
            this.numericUpDownListCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.numericUpDownListCount);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // PropertyPageObjectChangeValueDisplay
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox1);
            this.Icon = null;
            this.Name = "PropertyPageObjectChangeValueDisplay";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownListCount)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		public void SetObjectArgs(ObjectArgsChangeValueDisplay args) 
		{
			multiSelectListCount.Set(args.nListCount);
		}

		public ObjectArgsChangeValueDisplay GetObjectArgs(ObjectArgsChangeValueDisplay org) 
		{
			ObjectArgsChangeValueDisplay args = (ObjectArgsChangeValueDisplay)Tools.CopyObject(org);

			multiSelectListCount.Get(ref args.nListCount);

			return args;
		}

		/*
		public ObjectArgsChangeValueDisplay ObjectArgs 
		{
			set 
			{
				this.numericUpDownListCount.Value = value.nListCount;
			}
			get 
			{
				ObjectArgsChangeValueDisplay args = new ObjectArgsChangeValueDisplay();

				args.nListCount = ConvertTool.ToInt32(this.numericUpDownListCount.Value);

				return args;
			}
		}
		*/
	}
}
