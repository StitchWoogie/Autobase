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
	/// Summary description for PropertyPageObjectControlCheckBox.
	/// </summary>
	public class PropertyPageObjectControlCheckBox : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBoxTitle;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		MultiSelectTextBox multiSelectTitle = new MultiSelectTextBox();

		public PropertyPageObjectControlCheckBox()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			multiSelectTitle.Add(this.textBoxTitle);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectControlCheckBox));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.textBoxTitle);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.AccessibleDescription = null;
            this.textBoxTitle.AccessibleName = null;
            resources.ApplyResources(this.textBoxTitle, "textBoxTitle");
            this.textBoxTitle.BackgroundImage = null;
            this.textBoxTitle.Font = null;
            this.textBoxTitle.Name = "textBoxTitle";
            // 
            // PropertyPageObjectControlCheckBox
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox1);
            this.Icon = null;
            this.Name = "PropertyPageObjectControlCheckBox";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		public void SetObjectArgs(ObjectArgsControlCheckBox args) 
		{
			multiSelectTitle.Set(args.sTitle);
		}

		public ObjectArgsControlCheckBox GetObjectArgs(ObjectArgsControlCheckBox org) 
		{
			ObjectArgsControlCheckBox args = (ObjectArgsControlCheckBox)Tools.CopyObject(org);

			multiSelectTitle.Get(ref args.sTitle);

			return args;
		}

		/*
		public ObjectArgsControlCheckBox ObjectArgs 
		{
			set 
			{
				this.textBoxTitle.Text = value.sTitle;
			}
			get 
			{
				ObjectArgsControlCheckBox args = new ObjectArgsControlCheckBox();
				args.sTitle = this.textBoxTitle.Text;
				return args;
			}
		}
		*/
	}
}
