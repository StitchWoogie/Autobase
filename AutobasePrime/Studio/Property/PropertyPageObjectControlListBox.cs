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
	public class PropertyPageObjectControlListBox : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonValueConversion0;
		private System.Windows.Forms.RadioButton radioButtonValueConversion1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox checkBoxStyleSort;
		private System.Windows.Forms.CheckBox checkBoxStyoeVScroll;
		private System.Windows.Forms.CheckBox checkBoxStyleNoIntegralHeight;
		private System.Windows.Forms.CheckBox checkBoxStyleBorder;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		MultiSelectRadioButton multiSelectValueConversion = new MultiSelectRadioButton();
		MultiSelectCheckBox multiSelectStyleSort = new MultiSelectCheckBox();
		MultiSelectCheckBox multiSelectStyleVScroll = new MultiSelectCheckBox();
		MultiSelectCheckBox multiSelectStyleNoIntegralHeight = new MultiSelectCheckBox();
        private Label label1;
        private ComboBox comboBoxSelectionMode;
		MultiSelectCheckBox multiSelectStyleBorder = new MultiSelectCheckBox();
        MultiSelectComboBox multiSelectSelectionMode = new MultiSelectComboBox();

		public PropertyPageObjectControlListBox()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			multiSelectValueConversion.Add(this.radioButtonValueConversion0, this.radioButtonValueConversion1);
			multiSelectStyleSort.Add(this.checkBoxStyleSort);
			multiSelectStyleVScroll.Add(this.checkBoxStyoeVScroll);
			multiSelectStyleNoIntegralHeight.Add(this.checkBoxStyleNoIntegralHeight);
			multiSelectStyleBorder.Add(this.checkBoxStyleBorder);
            multiSelectSelectionMode.Add(this.comboBoxSelectionMode);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectControlListBox));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonValueConversion1 = new System.Windows.Forms.RadioButton();
            this.radioButtonValueConversion0 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxStyleBorder = new System.Windows.Forms.CheckBox();
            this.checkBoxStyleNoIntegralHeight = new System.Windows.Forms.CheckBox();
            this.checkBoxStyoeVScroll = new System.Windows.Forms.CheckBox();
            this.checkBoxStyleSort = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxSelectionMode = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
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
            this.groupBox2.Controls.Add(this.checkBoxStyleBorder);
            this.groupBox2.Controls.Add(this.checkBoxStyleNoIntegralHeight);
            this.groupBox2.Controls.Add(this.checkBoxStyoeVScroll);
            this.groupBox2.Controls.Add(this.checkBoxStyleSort);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
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
            // checkBoxStyleNoIntegralHeight
            // 
            this.checkBoxStyleNoIntegralHeight.AccessibleDescription = null;
            this.checkBoxStyleNoIntegralHeight.AccessibleName = null;
            resources.ApplyResources(this.checkBoxStyleNoIntegralHeight, "checkBoxStyleNoIntegralHeight");
            this.checkBoxStyleNoIntegralHeight.BackgroundImage = null;
            this.checkBoxStyleNoIntegralHeight.Font = null;
            this.checkBoxStyleNoIntegralHeight.Name = "checkBoxStyleNoIntegralHeight";
            // 
            // checkBoxStyoeVScroll
            // 
            this.checkBoxStyoeVScroll.AccessibleDescription = null;
            this.checkBoxStyoeVScroll.AccessibleName = null;
            resources.ApplyResources(this.checkBoxStyoeVScroll, "checkBoxStyoeVScroll");
            this.checkBoxStyoeVScroll.BackgroundImage = null;
            this.checkBoxStyoeVScroll.Font = null;
            this.checkBoxStyoeVScroll.Name = "checkBoxStyoeVScroll";
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
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // comboBoxSelectionMode
            // 
            this.comboBoxSelectionMode.AccessibleDescription = null;
            this.comboBoxSelectionMode.AccessibleName = null;
            resources.ApplyResources(this.comboBoxSelectionMode, "comboBoxSelectionMode");
            this.comboBoxSelectionMode.BackgroundImage = null;
            this.comboBoxSelectionMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSelectionMode.Font = null;
            this.comboBoxSelectionMode.FormattingEnabled = true;
            this.comboBoxSelectionMode.Items.AddRange(new object[] {
            resources.GetString("comboBoxSelectionMode.Items"),
            resources.GetString("comboBoxSelectionMode.Items1"),
            resources.GetString("comboBoxSelectionMode.Items2"),
            resources.GetString("comboBoxSelectionMode.Items3")});
            this.comboBoxSelectionMode.Name = "comboBoxSelectionMode";
            // 
            // PropertyPageObjectControlListBox
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.comboBoxSelectionMode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Icon = null;
            this.Name = "PropertyPageObjectControlListBox";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public void SetObjectArgs(ObjectArgsControlListBox args) 
		{
			int val;

			multiSelectValueConversion.Set(args.nValueConvert);

			val = ((args.dwWindowStyle & EnumWindowStyleFlags.LBS_SORT) > 0) ? 1 : 0;
			multiSelectStyleSort.Set(val);
			val = ((args.dwWindowStyle & EnumWindowStyleFlags.WS_VSCROLL) > 0) ? 1 : 0;
			multiSelectStyleVScroll.Set(val);
			val = ((args.dwWindowStyle & EnumWindowStyleFlags.LBS_NOINTEGRALHEIGHT) > 0) ? 1 : 0;
			multiSelectStyleNoIntegralHeight.Set(val);
			val = ((args.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0) ? 1 : 0;
			multiSelectStyleBorder.Set(val);

            multiSelectSelectionMode.Set(args.nSelectionMode);
		}

		public ObjectArgsControlListBox GetObjectArgs(ObjectArgsControlListBox org) 
		{
			ObjectArgsControlListBox args = (ObjectArgsControlListBox)Tools.CopyObject(org);

			EnumWindowStyleFlags flags = 0;
			int val;

			multiSelectValueConversion.Get(ref args.nValueConvert);

			val = ((args.dwWindowStyle & EnumWindowStyleFlags.LBS_SORT) > 0) ? 1 : 0;
			multiSelectStyleSort.Get(ref val);
			if(val == 1)	flags |= EnumWindowStyleFlags.LBS_SORT;

			val = ((args.dwWindowStyle & EnumWindowStyleFlags.WS_VSCROLL) > 0) ? 1 : 0;
			multiSelectStyleVScroll.Get(ref val);
			if(val == 1)	flags |= EnumWindowStyleFlags.WS_VSCROLL;

			val = ((args.dwWindowStyle & EnumWindowStyleFlags.LBS_NOINTEGRALHEIGHT) > 0) ? 1 : 0;
			multiSelectStyleNoIntegralHeight.Get(ref val);
			if(val == 1)	flags |= EnumWindowStyleFlags.LBS_NOINTEGRALHEIGHT;

			val = ((args.dwWindowStyle & EnumWindowStyleFlags.WS_BORDER) > 0) ? 1 : 0;
			multiSelectStyleBorder.Get(ref val);
			if(val == 1)	flags |= EnumWindowStyleFlags.WS_BORDER;

			args.dwWindowStyle = flags;

            multiSelectSelectionMode.Get(ref args.nSelectionMode);

			return args;
		}
		
	}
}
