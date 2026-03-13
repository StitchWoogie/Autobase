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
	/// Summary description for PropertyPageObjectAnalogString.
	/// </summary>
	public class PropertyPageObjectStringString : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonBorder0;
		private System.Windows.Forms.RadioButton radioButtonBorder1;
		private System.Windows.Forms.RadioButton radioButtonBorder2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButtonHorzAlign2;
		private System.Windows.Forms.RadioButton radioButtonHorzAlign1;
		private System.Windows.Forms.RadioButton radioButtonHorzAlign0;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton radioButtonVertAlign2;
		private System.Windows.Forms.RadioButton radioButtonVertAlign1;
		private System.Windows.Forms.RadioButton radioButtonVertAlign0;
		private System.Windows.Forms.RadioButton radioButtonBorder3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		MultiSelectRadioButton multiSelectBorder = new MultiSelectRadioButton();
		MultiSelectRadioButton multiSelectHorzAlign = new MultiSelectRadioButton();
		MultiSelectRadioButton multiSelectVertAlign = new MultiSelectRadioButton();

		public PropertyPageObjectStringString()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			multiSelectBorder.Add(	this.radioButtonBorder0, 
									this.radioButtonBorder1, 
									this.radioButtonBorder2, 
									this.radioButtonBorder3);
			multiSelectHorzAlign.Add(this.radioButtonHorzAlign0,
									this.radioButtonHorzAlign1,
									this.radioButtonHorzAlign2);
			multiSelectVertAlign.Add(this.radioButtonVertAlign0,
									this.radioButtonVertAlign1,
									this.radioButtonVertAlign2);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectStringString));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonBorder3 = new System.Windows.Forms.RadioButton();
            this.radioButtonBorder2 = new System.Windows.Forms.RadioButton();
            this.radioButtonBorder1 = new System.Windows.Forms.RadioButton();
            this.radioButtonBorder0 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonHorzAlign2 = new System.Windows.Forms.RadioButton();
            this.radioButtonHorzAlign1 = new System.Windows.Forms.RadioButton();
            this.radioButtonHorzAlign0 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonVertAlign2 = new System.Windows.Forms.RadioButton();
            this.radioButtonVertAlign1 = new System.Windows.Forms.RadioButton();
            this.radioButtonVertAlign0 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.radioButtonBorder3);
            this.groupBox1.Controls.Add(this.radioButtonBorder2);
            this.groupBox1.Controls.Add(this.radioButtonBorder1);
            this.groupBox1.Controls.Add(this.radioButtonBorder0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonBorder3
            // 
            this.radioButtonBorder3.AccessibleDescription = null;
            this.radioButtonBorder3.AccessibleName = null;
            resources.ApplyResources(this.radioButtonBorder3, "radioButtonBorder3");
            this.radioButtonBorder3.BackgroundImage = null;
            this.radioButtonBorder3.Font = null;
            this.radioButtonBorder3.Name = "radioButtonBorder3";
            // 
            // radioButtonBorder2
            // 
            this.radioButtonBorder2.AccessibleDescription = null;
            this.radioButtonBorder2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonBorder2, "radioButtonBorder2");
            this.radioButtonBorder2.BackgroundImage = null;
            this.radioButtonBorder2.Font = null;
            this.radioButtonBorder2.Name = "radioButtonBorder2";
            // 
            // radioButtonBorder1
            // 
            this.radioButtonBorder1.AccessibleDescription = null;
            this.radioButtonBorder1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonBorder1, "radioButtonBorder1");
            this.radioButtonBorder1.BackgroundImage = null;
            this.radioButtonBorder1.Font = null;
            this.radioButtonBorder1.Name = "radioButtonBorder1";
            // 
            // radioButtonBorder0
            // 
            this.radioButtonBorder0.AccessibleDescription = null;
            this.radioButtonBorder0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonBorder0, "radioButtonBorder0");
            this.radioButtonBorder0.BackgroundImage = null;
            this.radioButtonBorder0.Font = null;
            this.radioButtonBorder0.Name = "radioButtonBorder0";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.radioButtonHorzAlign2);
            this.groupBox2.Controls.Add(this.radioButtonHorzAlign1);
            this.groupBox2.Controls.Add(this.radioButtonHorzAlign0);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonHorzAlign2
            // 
            this.radioButtonHorzAlign2.AccessibleDescription = null;
            this.radioButtonHorzAlign2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonHorzAlign2, "radioButtonHorzAlign2");
            this.radioButtonHorzAlign2.BackgroundImage = null;
            this.radioButtonHorzAlign2.Font = null;
            this.radioButtonHorzAlign2.Name = "radioButtonHorzAlign2";
            // 
            // radioButtonHorzAlign1
            // 
            this.radioButtonHorzAlign1.AccessibleDescription = null;
            this.radioButtonHorzAlign1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonHorzAlign1, "radioButtonHorzAlign1");
            this.radioButtonHorzAlign1.BackgroundImage = null;
            this.radioButtonHorzAlign1.Font = null;
            this.radioButtonHorzAlign1.Name = "radioButtonHorzAlign1";
            // 
            // radioButtonHorzAlign0
            // 
            this.radioButtonHorzAlign0.AccessibleDescription = null;
            this.radioButtonHorzAlign0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonHorzAlign0, "radioButtonHorzAlign0");
            this.radioButtonHorzAlign0.BackgroundImage = null;
            this.radioButtonHorzAlign0.Font = null;
            this.radioButtonHorzAlign0.Name = "radioButtonHorzAlign0";
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.radioButtonVertAlign2);
            this.groupBox3.Controls.Add(this.radioButtonVertAlign1);
            this.groupBox3.Controls.Add(this.radioButtonVertAlign0);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // radioButtonVertAlign2
            // 
            this.radioButtonVertAlign2.AccessibleDescription = null;
            this.radioButtonVertAlign2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonVertAlign2, "radioButtonVertAlign2");
            this.radioButtonVertAlign2.BackgroundImage = null;
            this.radioButtonVertAlign2.Font = null;
            this.radioButtonVertAlign2.Name = "radioButtonVertAlign2";
            this.radioButtonVertAlign2.CheckedChanged += new System.EventHandler(this.radioButtonVertAlign2_CheckedChanged);
            // 
            // radioButtonVertAlign1
            // 
            this.radioButtonVertAlign1.AccessibleDescription = null;
            this.radioButtonVertAlign1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonVertAlign1, "radioButtonVertAlign1");
            this.radioButtonVertAlign1.BackgroundImage = null;
            this.radioButtonVertAlign1.Font = null;
            this.radioButtonVertAlign1.Name = "radioButtonVertAlign1";
            // 
            // radioButtonVertAlign0
            // 
            this.radioButtonVertAlign0.AccessibleDescription = null;
            this.radioButtonVertAlign0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonVertAlign0, "radioButtonVertAlign0");
            this.radioButtonVertAlign0.BackgroundImage = null;
            this.radioButtonVertAlign0.Font = null;
            this.radioButtonVertAlign0.Name = "radioButtonVertAlign0";
            // 
            // PropertyPageObjectStringString
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Icon = null;
            this.Name = "PropertyPageObjectStringString";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		public void SetObjectArgs(ObjectArgsStringString args) 
		{
			multiSelectBorder.Set(args.nBoxUse); 
		}

		public ObjectArgsStringString GetObjectArgs(ObjectArgsStringString org) 
		{
			ObjectArgsStringString args = (ObjectArgsStringString)Tools.CopyObject(org);

			multiSelectBorder.Get(ref args.nBoxUse); 

			return args;
		}

		public void SetTextAlign(TEXT_ALIGN args) 
		{
			multiSelectHorzAlign.Set(args.x); 
			multiSelectVertAlign.Set(args.y); 
		}

		public TEXT_ALIGN GetTextAlign(TEXT_ALIGN org) 
		{
			TEXT_ALIGN args = (TEXT_ALIGN)Tools.CopyObject(org);

			multiSelectHorzAlign.Get(ref args.x); 
			multiSelectVertAlign.Get(ref args.y); 

			return args;
		}

		private void groupBox3_Enter(object sender, System.EventArgs e)
		{
		
		}

		private void radioButtonVertAlign2_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

		/*
		public ObjectArgsStringString ObjectArgs 
		{
			set 
			{
				this.radioButtonBorder0.Checked = (value.nBoxUse == 0);
				this.radioButtonBorder1.Checked = (value.nBoxUse == 1);
				this.radioButtonBorder2.Checked = (value.nBoxUse == 2);
				this.radioButtonBorder3.Checked = (value.nBoxUse == 3);
			}
			get 
			{
				ObjectArgsStringString args = new ObjectArgsStringString();
				
				if(radioButtonBorder0.Checked)		args.nBoxUse = 0;
				else if(radioButtonBorder1.Checked)	args.nBoxUse = 1;
				else if(radioButtonBorder2.Checked)	args.nBoxUse = 2;
				else if(radioButtonBorder3.Checked)	args.nBoxUse = 3;
				else								args.nBoxUse = 0;

				return args;
			}
		}

		public TEXT_ALIGN TextAlign 
		{
			set 
			{
				this.radioButtonHorzAlign0.Checked = (value.x == 0);
				this.radioButtonHorzAlign1.Checked = (value.x == 1);
				this.radioButtonHorzAlign2.Checked = (value.x == 2);

				this.radioButtonVertAlign0.Checked = (value.y == 0);
				this.radioButtonVertAlign1.Checked = (value.y == 1);
				this.radioButtonVertAlign2.Checked = (value.y == 2);
			}

			get 
			{
				TEXT_ALIGN align = new TEXT_ALIGN();

				if(this.radioButtonHorzAlign0.Checked)		align.x = 0;
				else if(this.radioButtonHorzAlign1.Checked)	align.x = 1;
				else if(this.radioButtonHorzAlign2.Checked)	align.x = 2;
				else										align.x = 0;

				if(this.radioButtonVertAlign0.Checked)		align.y = 0;
				else if(this.radioButtonVertAlign1.Checked)	align.y = 1;
				else if(this.radioButtonVertAlign2.Checked)	align.y = 2;
				else										align.y = 0;

				return align;
			}
			
		}
		*/
	}
}
