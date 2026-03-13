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
	public class PropertyPageObjectControlEditBox : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox checkBoxStyoeUppercase;
		private System.Windows.Forms.CheckBox checkBoxStylePassword;
		private System.Windows.Forms.CheckBox checkBoxStyleLowercase;
		private System.Windows.Forms.CheckBox checkBoxStyleBorder;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		MultiSelectCheckBox multiSelectStyleUppercase = new MultiSelectCheckBox();
		MultiSelectCheckBox multiSelectStylePassword = new MultiSelectCheckBox();
		MultiSelectCheckBox multiSelectStyleLowercase = new MultiSelectCheckBox();
        private GroupBox groupBox1;
        private RadioButton radioButtonHorzAlign2;
        private RadioButton radioButtonHorzAlign1;
        private RadioButton radioButtonHorzAlign0;
		MultiSelectCheckBox multiSelectStyleBorder = new MultiSelectCheckBox();
        private CheckBox checkBoxStyleReadOnly;
        MultiSelectRadioButton multiSelectHorzAlign = new MultiSelectRadioButton();

        MultiSelectCheckBox multiSelectStyleReadOnly= new MultiSelectCheckBox();

		public PropertyPageObjectControlEditBox()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			multiSelectStyleUppercase.Add(this.checkBoxStyoeUppercase);
			multiSelectStylePassword.Add(this.checkBoxStylePassword);
			multiSelectStyleLowercase.Add(this.checkBoxStyleLowercase);
			multiSelectStyleBorder.Add(this.checkBoxStyleBorder);

            multiSelectHorzAlign.Add(this.radioButtonHorzAlign0,
                                    this.radioButtonHorzAlign1,
                                    this.radioButtonHorzAlign2);

            multiSelectStyleReadOnly.Add(this.checkBoxStyleReadOnly);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectControlEditBox));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxStyleReadOnly = new System.Windows.Forms.CheckBox();
            this.checkBoxStyleBorder = new System.Windows.Forms.CheckBox();
            this.checkBoxStyleLowercase = new System.Windows.Forms.CheckBox();
            this.checkBoxStyoeUppercase = new System.Windows.Forms.CheckBox();
            this.checkBoxStylePassword = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonHorzAlign2 = new System.Windows.Forms.RadioButton();
            this.radioButtonHorzAlign1 = new System.Windows.Forms.RadioButton();
            this.radioButtonHorzAlign0 = new System.Windows.Forms.RadioButton();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.checkBoxStyleReadOnly);
            this.groupBox2.Controls.Add(this.checkBoxStyleBorder);
            this.groupBox2.Controls.Add(this.checkBoxStyleLowercase);
            this.groupBox2.Controls.Add(this.checkBoxStyoeUppercase);
            this.groupBox2.Controls.Add(this.checkBoxStylePassword);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // checkBoxStyleReadOnly
            // 
            this.checkBoxStyleReadOnly.AccessibleDescription = null;
            this.checkBoxStyleReadOnly.AccessibleName = null;
            resources.ApplyResources(this.checkBoxStyleReadOnly, "checkBoxStyleReadOnly");
            this.checkBoxStyleReadOnly.BackgroundImage = null;
            this.checkBoxStyleReadOnly.Font = null;
            this.checkBoxStyleReadOnly.Name = "checkBoxStyleReadOnly";
            // 
            // checkBoxStyleBorder
            // 
            this.checkBoxStyleBorder.AccessibleDescription = null;
            this.checkBoxStyleBorder.AccessibleName = null;
            resources.ApplyResources(this.checkBoxStyleBorder, "checkBoxStyleBorder");
            this.checkBoxStyleBorder.BackgroundImage = null;
            this.checkBoxStyleBorder.Font = null;
            this.checkBoxStyleBorder.Name = "checkBoxStyleBorder";
            // 
            // checkBoxStyleLowercase
            // 
            this.checkBoxStyleLowercase.AccessibleDescription = null;
            this.checkBoxStyleLowercase.AccessibleName = null;
            resources.ApplyResources(this.checkBoxStyleLowercase, "checkBoxStyleLowercase");
            this.checkBoxStyleLowercase.BackgroundImage = null;
            this.checkBoxStyleLowercase.Font = null;
            this.checkBoxStyleLowercase.Name = "checkBoxStyleLowercase";
            this.checkBoxStyleLowercase.CheckedChanged += new System.EventHandler(this.checkBoxStyleLowercase_CheckedChanged);
            // 
            // checkBoxStyoeUppercase
            // 
            this.checkBoxStyoeUppercase.AccessibleDescription = null;
            this.checkBoxStyoeUppercase.AccessibleName = null;
            resources.ApplyResources(this.checkBoxStyoeUppercase, "checkBoxStyoeUppercase");
            this.checkBoxStyoeUppercase.BackgroundImage = null;
            this.checkBoxStyoeUppercase.Font = null;
            this.checkBoxStyoeUppercase.Name = "checkBoxStyoeUppercase";
            this.checkBoxStyoeUppercase.CheckedChanged += new System.EventHandler(this.checkBoxStyoeUppercase_CheckedChanged);
            // 
            // checkBoxStylePassword
            // 
            this.checkBoxStylePassword.AccessibleDescription = null;
            this.checkBoxStylePassword.AccessibleName = null;
            resources.ApplyResources(this.checkBoxStylePassword, "checkBoxStylePassword");
            this.checkBoxStylePassword.BackgroundImage = null;
            this.checkBoxStylePassword.Font = null;
            this.checkBoxStylePassword.Name = "checkBoxStylePassword";
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.radioButtonHorzAlign2);
            this.groupBox1.Controls.Add(this.radioButtonHorzAlign1);
            this.groupBox1.Controls.Add(this.radioButtonHorzAlign0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
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
            // PropertyPageObjectControlEditBox
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Icon = null;
            this.Name = "PropertyPageObjectControlEditBox";
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion


		public void SetObjectArgs(ObjectArgsControlEditBox args) 
		{
			int val;

			val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_UPPERCASE) > 0) ? 1 : 0;
			multiSelectStyleUppercase.Set(val);
			val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_PASSWORD) > 0) ? 1 : 0;
			multiSelectStylePassword.Set(val);
			val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_LOWERCASE) > 0) ? 1 : 0;
			multiSelectStyleLowercase.Set(val);
			val = ((args.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0) ? 1 : 0;
			multiSelectStyleBorder.Set(val);

            val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_READONLY) > 0) ? 1 : 0;
            multiSelectStyleReadOnly.Set(val);

            multiSelectHorzAlign.Set(args.nHorzAlign);
		}

		public ObjectArgsControlEditBox GetObjectArgs(ObjectArgsControlEditBox org) 
		{
			ObjectArgsControlEditBox args = (ObjectArgsControlEditBox)Tools.CopyObject(org);

			EnumWindowStyleFlags flags = 0;
			int val;

			val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_UPPERCASE) > 0) ? 1 : 0;
			multiSelectStyleUppercase.Get(ref val);
			if(val == 1)	flags |= EnumWindowStyleFlags.ES_UPPERCASE;

			val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_PASSWORD) > 0) ? 1 : 0;
			multiSelectStylePassword.Get(ref val);
			if(val == 1)	flags |= EnumWindowStyleFlags.ES_PASSWORD;

			val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_LOWERCASE) > 0) ? 1 : 0;
			multiSelectStyleLowercase.Get(ref val);
			if(val == 1)	flags |= EnumWindowStyleFlags.ES_LOWERCASE;

			val = ((args.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0) ? 1 : 0;
			multiSelectStyleBorder.Get(ref val);
			if(val == 1)	flags |= EnumWindowStyleFlags.WS_BORDER;

            val = ((args.dwWindowStyle & EnumWindowStyleFlags.ES_READONLY) > 0) ? 1 : 0;
            multiSelectStyleReadOnly.Get(ref val);
            if (val == 1) flags |= EnumWindowStyleFlags.ES_READONLY;

			args.dwWindowStyle = flags;

            multiSelectHorzAlign.Get(ref args.nHorzAlign);

			return args;
		}

		bool bIng = false;

		private void checkBoxStyoeUppercase_CheckedChanged(object sender, System.EventArgs e)
		{
			if(bIng)	return;
			bIng = true;
			if(this.checkBoxStyoeUppercase.Checked)
				this.checkBoxStyleLowercase.Checked = false;
			bIng = false;
		}

		private void checkBoxStyleLowercase_CheckedChanged(object sender, System.EventArgs e)
		{
			if(bIng)	return;
			bIng = true;
			if(this.checkBoxStyleLowercase.Checked)
				this.checkBoxStyoeUppercase.Checked = false;
			bIng = false;
		}


		/*

		public ObjectArgsControlEditBox ObjectArgs 
		{
			set 
			{
				this.checkBoxStyleAutoHScroll.Checked = ((value.dwWindowStyle & EnumWindowStyleFlags.ES_AUTOHSCROLL) > 0);
				this.checkBoxStyleBorder.Checked = ((value.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0);
				this.checkBoxStyleLowercase.Checked = ((value.dwWindowStyle & EnumWindowStyleFlags.ES_LOWERCASE) > 0);
				this.checkBoxStylePassword.Checked = ((value.dwWindowStyle & EnumWindowStyleFlags.ES_PASSWORD) > 0);
				this.checkBoxStyoeUppercase.Checked = ((value.dwWindowStyle & EnumWindowStyleFlags.ES_UPPERCASE) > 0);
			}
			get 
			{
				ObjectArgsControlEditBox args = new ObjectArgsControlEditBox();

				args.dwWindowStyle = 0;

				if(this.checkBoxStyleAutoHScroll.Checked)
					args.dwWindowStyle |= EnumWindowStyleFlags.ES_AUTOHSCROLL;
				if(this.checkBoxStyleBorder.Checked)
					args.dwWindowStyle |= EnumWindowStyleFlags.WS_BORDER;
				if(this.checkBoxStyleLowercase.Checked)
					args.dwWindowStyle |= EnumWindowStyleFlags.ES_LOWERCASE;
				if(this.checkBoxStylePassword.Checked)
					args.dwWindowStyle |= EnumWindowStyleFlags.ES_PASSWORD;
				if(this.checkBoxStyoeUppercase.Checked)
					args.dwWindowStyle |= EnumWindowStyleFlags.ES_UPPERCASE;
				
				return args;
			}
		}
		*/
	}
}
