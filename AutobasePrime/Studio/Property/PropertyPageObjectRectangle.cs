using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using GraphicModule;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageObjectRectangle.
	/// </summary>
	public class PropertyPageObjectRectangle : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.RadioButton radioButtonOption4;
		private System.Windows.Forms.RadioButton radioButtonOption2;
		private System.Windows.Forms.RadioButton radioButtonOption0;
		private System.Windows.Forms.RadioButton radioButtonOption3;
		private System.Windows.Forms.RadioButton radioButtonOption1;

		MultiSelectRadioButton multiSelectBorderStyle = new MultiSelectRadioButton();

		public PropertyPageObjectRectangle()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			multiSelectBorderStyle.Add(radioButtonOption0, radioButtonOption1, radioButtonOption2, radioButtonOption3, radioButtonOption4);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectRectangle));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonOption4 = new System.Windows.Forms.RadioButton();
            this.radioButtonOption2 = new System.Windows.Forms.RadioButton();
            this.radioButtonOption0 = new System.Windows.Forms.RadioButton();
            this.radioButtonOption3 = new System.Windows.Forms.RadioButton();
            this.radioButtonOption1 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.radioButtonOption4);
            this.groupBox1.Controls.Add(this.radioButtonOption2);
            this.groupBox1.Controls.Add(this.radioButtonOption0);
            this.groupBox1.Controls.Add(this.radioButtonOption3);
            this.groupBox1.Controls.Add(this.radioButtonOption1);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonOption4
            // 
            this.radioButtonOption4.AccessibleDescription = null;
            this.radioButtonOption4.AccessibleName = null;
            resources.ApplyResources(this.radioButtonOption4, "radioButtonOption4");
            this.radioButtonOption4.BackgroundImage = null;
            this.radioButtonOption4.Font = null;
            this.radioButtonOption4.Name = "radioButtonOption4";
            // 
            // radioButtonOption2
            // 
            this.radioButtonOption2.AccessibleDescription = null;
            this.radioButtonOption2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonOption2, "radioButtonOption2");
            this.radioButtonOption2.BackgroundImage = null;
            this.radioButtonOption2.Font = null;
            this.radioButtonOption2.Name = "radioButtonOption2";
            // 
            // radioButtonOption0
            // 
            this.radioButtonOption0.AccessibleDescription = null;
            this.radioButtonOption0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonOption0, "radioButtonOption0");
            this.radioButtonOption0.BackgroundImage = null;
            this.radioButtonOption0.Font = null;
            this.radioButtonOption0.Name = "radioButtonOption0";
            // 
            // radioButtonOption3
            // 
            this.radioButtonOption3.AccessibleDescription = null;
            this.radioButtonOption3.AccessibleName = null;
            resources.ApplyResources(this.radioButtonOption3, "radioButtonOption3");
            this.radioButtonOption3.BackgroundImage = null;
            this.radioButtonOption3.Font = null;
            this.radioButtonOption3.Name = "radioButtonOption3";
            // 
            // radioButtonOption1
            // 
            this.radioButtonOption1.AccessibleDescription = null;
            this.radioButtonOption1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonOption1, "radioButtonOption1");
            this.radioButtonOption1.BackgroundImage = null;
            this.radioButtonOption1.Font = null;
            this.radioButtonOption1.Name = "radioButtonOption1";
            // 
            // PropertyPageObjectRectangle
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox1);
            this.Icon = null;
            this.Name = "PropertyPageObjectRectangle";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		public void SetObjectArgs(ObjectArgsRectangle args) 
		{
			this.multiSelectBorderStyle.Set(args.nBorderStyle);
		}

		public ObjectArgsRectangle GetObjectArgs(ObjectArgsRectangle org) 
		{
			ObjectArgsRectangle args = (ObjectArgsRectangle)Tools.CopyObject(org);

			this.multiSelectBorderStyle.Get(ref args.nBorderStyle);

			return args;
		}
	}
}
