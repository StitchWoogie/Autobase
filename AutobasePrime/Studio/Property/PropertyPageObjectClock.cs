using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using GraphicModule;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageButtonModule3D.
	/// </summary>
	public class PropertyPageObjectClock : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonType0;
		private System.Windows.Forms.RadioButton radioButtonType1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		MultiSelectRadioButton multiSelectType = new MultiSelectRadioButton();


		public PropertyPageObjectClock()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			multiSelectType.Add(this.radioButtonType0, this.radioButtonType1);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectClock));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonType0 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.radioButtonType1);
            this.groupBox1.Controls.Add(this.radioButtonType0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonType1
            // 
            this.radioButtonType1.AccessibleDescription = null;
            this.radioButtonType1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonType1, "radioButtonType1");
            this.radioButtonType1.BackgroundImage = null;
            this.radioButtonType1.Font = null;
            this.radioButtonType1.Name = "radioButtonType1";
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
            // 
            // PropertyPageObjectClock
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox1);
            this.Icon = null;
            this.Name = "PropertyPageObjectClock";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		
		public void SetObjectArgs(ObjectArgsClock args) 
		{
			multiSelectType.Set(args.type);
		}

		public ObjectArgsClock GetObjectArgs(ObjectArgsClock org) 
		{
			ObjectArgsClock args = (ObjectArgsClock)Tools.CopyObject(org);

			multiSelectType.Get(ref args.type);

			return args;
		}

		private void radioButtonType1_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

		/*
		public ObjectArgsClock ObjectArgs 
		{
			set 
			{
				this.radioButtonType0.Checked = (value.type == 0);
				this.radioButtonType1.Checked = (value.type == 1);
			}
			get 
			{
				ObjectArgsClock args = new ObjectArgsClock();

				if(this.radioButtonType0.Checked)	args.type = 0;
				else								args.type = 1;

				return args;
			}
		}
		*/
	}
}
