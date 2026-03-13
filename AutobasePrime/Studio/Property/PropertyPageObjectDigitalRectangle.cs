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
	/// Summary description for PropertyPageDigitalCircle.
	/// </summary>
	public class PropertyPageObjectDigitalRectangle : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonColorOn;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button buttonColorOff;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton radioButtonDisplayMethod0;
		private System.Windows.Forms.RadioButton radioButtonDisplayMethod1;
		private System.Windows.Forms.RadioButton radioButtonDisplayMethod2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		MultiSelectColorButton multiSelectColorOn = new MultiSelectColorButton();
		MultiSelectColorButton multiSelectColorOff = new MultiSelectColorButton();
		MultiSelectRadioButton multiSelectDisplayMethod = new MultiSelectRadioButton();

		/*
		public int DisplayMethod 
		{
			get 
			{
				int val = 0;
				if(radioButtonDisplayMethod0.Checked)		val = 0;
				else if(radioButtonDisplayMethod1.Checked)	val = 1;
				else if(radioButtonDisplayMethod2.Checked)	val = 2;
				else										val = 0;
				return val;
			}
			set 
			{
				radioButtonDisplayMethod0.Checked = (value == 0);
				radioButtonDisplayMethod1.Checked = (value == 1);
				radioButtonDisplayMethod2.Checked = (value == 2);
			}
		}
		*/

		public PropertyPageObjectDigitalRectangle()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			multiSelectColorOn.Add(this.buttonColorOn);
			multiSelectColorOff.Add(this.buttonColorOff);
			multiSelectDisplayMethod.Add(this.radioButtonDisplayMethod0, this.radioButtonDisplayMethod1, this.radioButtonDisplayMethod2);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectDigitalRectangle));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonColorOn = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonColorOff = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonDisplayMethod2 = new System.Windows.Forms.RadioButton();
            this.radioButtonDisplayMethod1 = new System.Windows.Forms.RadioButton();
            this.radioButtonDisplayMethod0 = new System.Windows.Forms.RadioButton();
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
            this.groupBox1.Controls.Add(this.buttonColorOn);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // buttonColorOn
            // 
            this.buttonColorOn.AccessibleDescription = null;
            this.buttonColorOn.AccessibleName = null;
            resources.ApplyResources(this.buttonColorOn, "buttonColorOn");
            this.buttonColorOn.BackgroundImage = null;
            this.buttonColorOn.Font = null;
            this.buttonColorOn.Name = "buttonColorOn";
            this.buttonColorOn.Click += new System.EventHandler(this.buttonColorOn_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.buttonColorOff);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // buttonColorOff
            // 
            this.buttonColorOff.AccessibleDescription = null;
            this.buttonColorOff.AccessibleName = null;
            resources.ApplyResources(this.buttonColorOff, "buttonColorOff");
            this.buttonColorOff.BackgroundImage = null;
            this.buttonColorOff.Font = null;
            this.buttonColorOff.Name = "buttonColorOff";
            this.buttonColorOff.Click += new System.EventHandler(this.buttonColorOff_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.AccessibleDescription = null;
            this.groupBox3.AccessibleName = null;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.BackgroundImage = null;
            this.groupBox3.Controls.Add(this.radioButtonDisplayMethod2);
            this.groupBox3.Controls.Add(this.radioButtonDisplayMethod1);
            this.groupBox3.Controls.Add(this.radioButtonDisplayMethod0);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // radioButtonDisplayMethod2
            // 
            this.radioButtonDisplayMethod2.AccessibleDescription = null;
            this.radioButtonDisplayMethod2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDisplayMethod2, "radioButtonDisplayMethod2");
            this.radioButtonDisplayMethod2.BackgroundImage = null;
            this.radioButtonDisplayMethod2.Font = null;
            this.radioButtonDisplayMethod2.Name = "radioButtonDisplayMethod2";
            // 
            // radioButtonDisplayMethod1
            // 
            this.radioButtonDisplayMethod1.AccessibleDescription = null;
            this.radioButtonDisplayMethod1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDisplayMethod1, "radioButtonDisplayMethod1");
            this.radioButtonDisplayMethod1.BackgroundImage = null;
            this.radioButtonDisplayMethod1.Font = null;
            this.radioButtonDisplayMethod1.Name = "radioButtonDisplayMethod1";
            this.radioButtonDisplayMethod1.CheckedChanged += new System.EventHandler(this.radioButtonDisplayMethod1_CheckedChanged);
            // 
            // radioButtonDisplayMethod0
            // 
            this.radioButtonDisplayMethod0.AccessibleDescription = null;
            this.radioButtonDisplayMethod0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonDisplayMethod0, "radioButtonDisplayMethod0");
            this.radioButtonDisplayMethod0.BackgroundImage = null;
            this.radioButtonDisplayMethod0.Font = null;
            this.radioButtonDisplayMethod0.Name = "radioButtonDisplayMethod0";
            // 
            // PropertyPageObjectDigitalRectangle
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Icon = null;
            this.Name = "PropertyPageObjectDigitalRectangle";
            this.Load += new System.EventHandler(this.PropertyPageObjectDigitalRectangle_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void buttonColorOn_Click(object sender, System.EventArgs e)
		{
			FormColorDialog dialog = new FormColorDialog();


			dialog.SetSelectedColor(buttonColorOn.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				buttonColorOn.BackColor = dialog.GetSelectedColor();
			}
		}

		private void buttonColorOff_Click(object sender, System.EventArgs e)
		{
			FormColorDialog dialog = new FormColorDialog();


			dialog.SetSelectedColor(buttonColorOff.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				buttonColorOff.BackColor = dialog.GetSelectedColor();
			}
		}

		public void SetObjectArgs(ObjectArgsDigitalRectangle args) 
		{
			multiSelectColorOn.Set(args.colorOn);
			multiSelectColorOff.Set(args.colorOff);
			multiSelectDisplayMethod.Set(args.nDisplayMethod);
		}

		public ObjectArgsDigitalRectangle GetObjectArgs(ObjectArgsDigitalRectangle org) 
		{
			ObjectArgsDigitalRectangle args = (ObjectArgsDigitalRectangle)Tools.CopyObject(org);

			multiSelectColorOn.Get(ref args.colorOn);
			multiSelectColorOff.Get(ref args.colorOff);
			multiSelectDisplayMethod.Get(ref args.nDisplayMethod);

			return args;
		}

		private void radioButtonDisplayMethod1_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

        private void PropertyPageObjectDigitalRectangle_Load(object sender, EventArgs e)
        {

        }
	}
}
