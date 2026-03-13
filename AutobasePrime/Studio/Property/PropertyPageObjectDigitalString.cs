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
	public class PropertyPageObjectDigitalString : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonColorOn;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button buttonColorOff;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button buttonColorBack;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		MultiSelectColorButton multiSelectColorOn = new MultiSelectColorButton();
		MultiSelectColorButton multiSelectColorOff = new MultiSelectColorButton();
		MultiSelectColorButton multiSelectColorBack = new MultiSelectColorButton();

		public PropertyPageObjectDigitalString()
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
			multiSelectColorBack.Add(this.buttonColorBack);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectDigitalString));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonColorOn = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonColorOff = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonColorBack = new System.Windows.Forms.Button();
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
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
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
            this.groupBox3.Controls.Add(this.buttonColorBack);
            this.groupBox3.Font = null;
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // buttonColorBack
            // 
            this.buttonColorBack.AccessibleDescription = null;
            this.buttonColorBack.AccessibleName = null;
            resources.ApplyResources(this.buttonColorBack, "buttonColorBack");
            this.buttonColorBack.BackgroundImage = null;
            this.buttonColorBack.Font = null;
            this.buttonColorBack.Name = "buttonColorBack";
            this.buttonColorBack.Click += new System.EventHandler(this.buttonColorBack_Click);
            // 
            // PropertyPageObjectDigitalString
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Icon = null;
            this.Name = "PropertyPageObjectDigitalString";
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

		private void buttonColorBack_Click(object sender, System.EventArgs e)
		{
			FormColorDialog dialog = new FormColorDialog();


			dialog.SetSelectedColor(buttonColorBack.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				buttonColorBack.BackColor = dialog.GetSelectedColor();
			}
		}

		public void SetObjectArgs(ObjectArgsDigitalString args) 
		{
			multiSelectColorOn.Set(args.colorOn);
			multiSelectColorOff.Set(args.colorOff);
			multiSelectColorBack.Set(args.colorBack.basic_color);
		}

		public ObjectArgsDigitalString GetObjectArgs(ObjectArgsDigitalString org) 
		{
			ObjectArgsDigitalString args = (ObjectArgsDigitalString)Tools.CopyObject(org);

			multiSelectColorOn.Get(ref args.colorOn);
			multiSelectColorOff.Get(ref args.colorOff);
			multiSelectColorBack.Get(ref args.colorBack.basic_color);

			return args;
		}

		private void groupBox2_Enter(object sender, System.EventArgs e)
		{
		
		}
	}
}
