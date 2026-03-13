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
	public class PropertyPageObjectControlComboBox : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxType;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonValueConversion0;
		private System.Windows.Forms.RadioButton radioButtonValueConversion1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox checkBoxStyleSort;
		private System.Windows.Forms.CheckBox checkBoxStyleNoIntegralHeight;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		MultiSelectComboBox multiSelectType = new MultiSelectComboBox();
		MultiSelectRadioButton multiSelectValueConversion = new MultiSelectRadioButton();
		MultiSelectCheckBox multiSelectStyleSort = new MultiSelectCheckBox();
		MultiSelectCheckBox multiSelectStyleNoIntegralHeight = new MultiSelectCheckBox();

		public PropertyPageObjectControlComboBox()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			multiSelectType.Add(this.comboBoxType);
			multiSelectValueConversion.Add(this.radioButtonValueConversion0, this.radioButtonValueConversion1);
			multiSelectStyleSort.Add(this.checkBoxStyleSort);
			multiSelectStyleNoIntegralHeight.Add(this.checkBoxStyleNoIntegralHeight);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectControlComboBox));
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonValueConversion1 = new System.Windows.Forms.RadioButton();
            this.radioButtonValueConversion0 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxStyleNoIntegralHeight = new System.Windows.Forms.CheckBox();
            this.checkBoxStyleSort = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // comboBoxType
            // 
            this.comboBoxType.AccessibleDescription = null;
            this.comboBoxType.AccessibleName = null;
            resources.ApplyResources(this.comboBoxType, "comboBoxType");
            this.comboBoxType.BackgroundImage = null;
            this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxType.Font = null;
            this.comboBoxType.Items.AddRange(new object[] {
            resources.GetString("comboBoxType.Items"),
            resources.GetString("comboBoxType.Items1"),
            resources.GetString("comboBoxType.Items2")});
            this.comboBoxType.Name = "comboBoxType";
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.radioButtonValueConversion1);
            this.groupBox1.Controls.Add(this.radioButtonValueConversion0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonValueConversion1
            // 
            this.radioButtonValueConversion1.AccessibleDescription = null;
            this.radioButtonValueConversion1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonValueConversion1, "radioButtonValueConversion1");
            this.radioButtonValueConversion1.BackgroundImage = null;
            this.radioButtonValueConversion1.Font = null;
            this.radioButtonValueConversion1.Name = "radioButtonValueConversion1";
            // 
            // radioButtonValueConversion0
            // 
            this.radioButtonValueConversion0.AccessibleDescription = null;
            this.radioButtonValueConversion0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonValueConversion0, "radioButtonValueConversion0");
            this.radioButtonValueConversion0.BackgroundImage = null;
            this.radioButtonValueConversion0.Font = null;
            this.radioButtonValueConversion0.Name = "radioButtonValueConversion0";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.checkBoxStyleNoIntegralHeight);
            this.groupBox2.Controls.Add(this.checkBoxStyleSort);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // checkBoxStyleNoIntegralHeight
            // 
            this.checkBoxStyleNoIntegralHeight.AccessibleDescription = null;
            this.checkBoxStyleNoIntegralHeight.AccessibleName = null;
            resources.ApplyResources(this.checkBoxStyleNoIntegralHeight, "checkBoxStyleNoIntegralHeight");
            this.checkBoxStyleNoIntegralHeight.BackgroundImage = null;
            this.checkBoxStyleNoIntegralHeight.Font = null;
            this.checkBoxStyleNoIntegralHeight.Name = "checkBoxStyleNoIntegralHeight";
            // 
            // checkBoxStyleSort
            // 
            this.checkBoxStyleSort.AccessibleDescription = null;
            this.checkBoxStyleSort.AccessibleName = null;
            resources.ApplyResources(this.checkBoxStyleSort, "checkBoxStyleSort");
            this.checkBoxStyleSort.BackgroundImage = null;
            this.checkBoxStyleSort.Font = null;
            this.checkBoxStyleSort.Name = "checkBoxStyleSort";
            // 
            // PropertyPageObjectControlComboBox
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboBoxType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Icon = null;
            this.Name = "PropertyPageObjectControlComboBox";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		public void SetObjectArgs(ObjectArgsControlComboBox args) 
		{
			int val;

			if((args.dwWindowStyle & EnumWindowStyleFlags.CBS_DROPDOWNLIST) == EnumWindowStyleFlags.CBS_DROPDOWNLIST) 
				val = 2;
			else if((args.dwWindowStyle & EnumWindowStyleFlags.CBS_DROPDOWN) == EnumWindowStyleFlags.CBS_DROPDOWN) 
				val = 1;
			else
				val = 0;

			multiSelectType.Set(val);
			multiSelectValueConversion.Set(args.nValueConvert);

			val = ((args.dwWindowStyle & EnumWindowStyleFlags.CBS_SORT) > 0) ? 1 : 0;
			multiSelectStyleSort.Set(val);
			val = ((args.dwWindowStyle & EnumWindowStyleFlags.CBS_NOINTEGRALHEIGHT) > 0) ? 1 : 0;
			multiSelectStyleNoIntegralHeight.Set(val);
		}

		public ObjectArgsControlComboBox GetObjectArgs(ObjectArgsControlComboBox org) 
		{
			ObjectArgsControlComboBox args = (ObjectArgsControlComboBox)Tools.CopyObject(org);

			EnumWindowStyleFlags flags = 0;
			int val;

			if((args.dwWindowStyle & EnumWindowStyleFlags.CBS_DROPDOWNLIST) == EnumWindowStyleFlags.CBS_DROPDOWNLIST) 
				val = 2;
			else if((args.dwWindowStyle & EnumWindowStyleFlags.CBS_DROPDOWN) == EnumWindowStyleFlags.CBS_DROPDOWN)
				val = 1;
			else
				val = 0;
			multiSelectType.Get(ref val);

			if(val == 1) 
				flags |= EnumWindowStyleFlags.CBS_DROPDOWN;
			else if(val == 2) 
				flags |= EnumWindowStyleFlags.CBS_DROPDOWNLIST;
			else 
				flags |= EnumWindowStyleFlags.CBS_SIMPLE;

			multiSelectValueConversion.Get(ref args.nValueConvert);

			val = ((args.dwWindowStyle & EnumWindowStyleFlags.CBS_SORT) > 0) ? 1 : 0;
			multiSelectStyleSort.Get(ref val);
			if(val == 1)	flags |= EnumWindowStyleFlags.CBS_SORT;

			val = ((args.dwWindowStyle & EnumWindowStyleFlags.CBS_NOINTEGRALHEIGHT) > 0) ? 1 : 0;
			multiSelectStyleNoIntegralHeight.Get(ref val);
			if(val == 1)	flags |= EnumWindowStyleFlags.CBS_NOINTEGRALHEIGHT;

			args.dwWindowStyle = flags;

			return args;
		}
	}
}
