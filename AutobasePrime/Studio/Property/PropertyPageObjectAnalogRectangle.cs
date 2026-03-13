using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using GraphicModule;
using NetTools;
using AutoLibLocal;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageObjectAnalogRectangle.
	/// </summary>
	public class PropertyPageObjectAnalogRectangle : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.RadioButton radioButtonGuideDisplay0;
		private System.Windows.Forms.RadioButton radioButtonGuideDisplay1;
		private System.Windows.Forms.RadioButton radioButtonGuideDisplay2;
		private System.Windows.Forms.NumericUpDown numericUpDownGuideBig;
		private System.Windows.Forms.Button buttonGuideBigColor;
		private System.Windows.Forms.Button buttonGuideSmallColor;
		private System.Windows.Forms.NumericUpDown numericUpDownGuideSmall;
		private System.Windows.Forms.NumericUpDown numericUpDownGuideSize;
		private System.Windows.Forms.CheckBox checkBoxDisplayNumber;
		private System.Windows.Forms.RadioButton radioButtonBarDir0;
		private System.Windows.Forms.RadioButton radioButtonBarDir1;
		private System.Windows.Forms.RadioButton radioButtonBarDir2;
		private System.Windows.Forms.RadioButton radioButtonBarDir3;
		private System.Windows.Forms.CheckBox checkBoxUseNewRange;
		private System.Windows.Forms.TextBox textBoxNewRangeFull;
		private System.Windows.Forms.TextBox textBoxNewRangeBase;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		MultiSelectRadioButton multiSelectBarDir = new MultiSelectRadioButton();
		MultiSelectRadioButton multiSelectGuideDisplay = new MultiSelectRadioButton();
		MultiSelectNumericUpDown multiSelectGuideBig = new MultiSelectNumericUpDown();
		MultiSelectNumericUpDown multiSelectGuideSmall = new MultiSelectNumericUpDown();
		MultiSelectNumericUpDown multiSelectGuideSize = new MultiSelectNumericUpDown();
		MultiSelectCheckBox multiSelectDisplayNumber = new MultiSelectCheckBox();
		MultiSelectCheckBox multiSelectUseNewRange = new MultiSelectCheckBox();
		MultiSelectTextBox multiSelectNewRangeFull = new MultiSelectTextBox();
		MultiSelectTextBox multiSelectNewRangeBase = new MultiSelectTextBox();
		MultiSelectColorButton multiSelectGuideBigColor = new MultiSelectColorButton();
        private NumericUpDown numericUpDownCornerRadius;
        private Label label8;
		MultiSelectColorButton multiSelectGuideSmallColor = new MultiSelectColorButton();

		public PropertyPageObjectAnalogRectangle()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			multiSelectBarDir.Add(	radioButtonBarDir0,
									radioButtonBarDir1,
									radioButtonBarDir2,
									radioButtonBarDir3);

			multiSelectGuideDisplay.Add(	radioButtonGuideDisplay0,
											this.radioButtonGuideDisplay1,
											this.radioButtonGuideDisplay2);

			multiSelectGuideBig.Add(this.numericUpDownGuideBig);
			multiSelectGuideSmall.Add(this.numericUpDownGuideSmall);
			multiSelectGuideSize.Add(this.numericUpDownGuideSize);
			multiSelectDisplayNumber.Add(this.checkBoxDisplayNumber);
			multiSelectUseNewRange.Add(this.checkBoxUseNewRange);
			multiSelectNewRangeFull.Add(this.textBoxNewRangeFull);
			multiSelectNewRangeBase.Add(this.textBoxNewRangeBase);
			multiSelectGuideBigColor.Add(this.buttonGuideBigColor);
			multiSelectGuideSmallColor.Add(this.buttonGuideSmallColor);

            if (TotalConfigProject.eProjectPlatform == EnumProjectPlatform.CE) this.numericUpDownCornerRadius.Enabled = false;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectAnalogRectangle));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericUpDownCornerRadius = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownGuideSize = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonGuideSmallColor = new System.Windows.Forms.Button();
            this.numericUpDownGuideSmall = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonGuideBigColor = new System.Windows.Forms.Button();
            this.numericUpDownGuideBig = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonGuideDisplay2 = new System.Windows.Forms.RadioButton();
            this.radioButtonGuideDisplay1 = new System.Windows.Forms.RadioButton();
            this.radioButtonGuideDisplay0 = new System.Windows.Forms.RadioButton();
            this.checkBoxDisplayNumber = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonBarDir3 = new System.Windows.Forms.RadioButton();
            this.radioButtonBarDir2 = new System.Windows.Forms.RadioButton();
            this.radioButtonBarDir1 = new System.Windows.Forms.RadioButton();
            this.radioButtonBarDir0 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxNewRangeBase = new System.Windows.Forms.TextBox();
            this.textBoxNewRangeFull = new System.Windows.Forms.TextBox();
            this.checkBoxUseNewRange = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCornerRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGuideSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGuideSmall)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGuideBig)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numericUpDownCornerRadius);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.numericUpDownGuideSize);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.buttonGuideSmallColor);
            this.groupBox1.Controls.Add(this.numericUpDownGuideSmall);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.buttonGuideBigColor);
            this.groupBox1.Controls.Add(this.numericUpDownGuideBig);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radioButtonGuideDisplay2);
            this.groupBox1.Controls.Add(this.radioButtonGuideDisplay1);
            this.groupBox1.Controls.Add(this.radioButtonGuideDisplay0);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // numericUpDownCornerRadius
            // 
            resources.ApplyResources(this.numericUpDownCornerRadius, "numericUpDownCornerRadius");
            this.numericUpDownCornerRadius.Name = "numericUpDownCornerRadius";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // numericUpDownGuideSize
            // 
            resources.ApplyResources(this.numericUpDownGuideSize, "numericUpDownGuideSize");
            this.numericUpDownGuideSize.Name = "numericUpDownGuideSize";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // buttonGuideSmallColor
            // 
            resources.ApplyResources(this.buttonGuideSmallColor, "buttonGuideSmallColor");
            this.buttonGuideSmallColor.Name = "buttonGuideSmallColor";
            this.buttonGuideSmallColor.Click += new System.EventHandler(this.buttonGuideSmallColor_Click);
            // 
            // numericUpDownGuideSmall
            // 
            resources.ApplyResources(this.numericUpDownGuideSmall, "numericUpDownGuideSmall");
            this.numericUpDownGuideSmall.Name = "numericUpDownGuideSmall";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // buttonGuideBigColor
            // 
            resources.ApplyResources(this.buttonGuideBigColor, "buttonGuideBigColor");
            this.buttonGuideBigColor.Name = "buttonGuideBigColor";
            this.buttonGuideBigColor.Click += new System.EventHandler(this.buttonGuideBigColor_Click);
            // 
            // numericUpDownGuideBig
            // 
            resources.ApplyResources(this.numericUpDownGuideBig, "numericUpDownGuideBig");
            this.numericUpDownGuideBig.Name = "numericUpDownGuideBig";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // radioButtonGuideDisplay2
            // 
            resources.ApplyResources(this.radioButtonGuideDisplay2, "radioButtonGuideDisplay2");
            this.radioButtonGuideDisplay2.Name = "radioButtonGuideDisplay2";
            // 
            // radioButtonGuideDisplay1
            // 
            resources.ApplyResources(this.radioButtonGuideDisplay1, "radioButtonGuideDisplay1");
            this.radioButtonGuideDisplay1.Name = "radioButtonGuideDisplay1";
            // 
            // radioButtonGuideDisplay0
            // 
            resources.ApplyResources(this.radioButtonGuideDisplay0, "radioButtonGuideDisplay0");
            this.radioButtonGuideDisplay0.Name = "radioButtonGuideDisplay0";
            // 
            // checkBoxDisplayNumber
            // 
            resources.ApplyResources(this.checkBoxDisplayNumber, "checkBoxDisplayNumber");
            this.checkBoxDisplayNumber.Name = "checkBoxDisplayNumber";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonBarDir3);
            this.groupBox2.Controls.Add(this.radioButtonBarDir2);
            this.groupBox2.Controls.Add(this.radioButtonBarDir1);
            this.groupBox2.Controls.Add(this.radioButtonBarDir0);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonBarDir3
            // 
            resources.ApplyResources(this.radioButtonBarDir3, "radioButtonBarDir3");
            this.radioButtonBarDir3.Name = "radioButtonBarDir3";
            // 
            // radioButtonBarDir2
            // 
            resources.ApplyResources(this.radioButtonBarDir2, "radioButtonBarDir2");
            this.radioButtonBarDir2.Name = "radioButtonBarDir2";
            // 
            // radioButtonBarDir1
            // 
            resources.ApplyResources(this.radioButtonBarDir1, "radioButtonBarDir1");
            this.radioButtonBarDir1.Name = "radioButtonBarDir1";
            this.radioButtonBarDir1.CheckedChanged += new System.EventHandler(this.radioButtonBarDir1_CheckedChanged);
            // 
            // radioButtonBarDir0
            // 
            resources.ApplyResources(this.radioButtonBarDir0, "radioButtonBarDir0");
            this.radioButtonBarDir0.Name = "radioButtonBarDir0";
            this.radioButtonBarDir0.CheckedChanged += new System.EventHandler(this.radioButtonBarDir0_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxNewRangeBase);
            this.groupBox3.Controls.Add(this.textBoxNewRangeFull);
            this.groupBox3.Controls.Add(this.checkBoxUseNewRange);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label7);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // textBoxNewRangeBase
            // 
            resources.ApplyResources(this.textBoxNewRangeBase, "textBoxNewRangeBase");
            this.textBoxNewRangeBase.Name = "textBoxNewRangeBase";
            // 
            // textBoxNewRangeFull
            // 
            resources.ApplyResources(this.textBoxNewRangeFull, "textBoxNewRangeFull");
            this.textBoxNewRangeFull.Name = "textBoxNewRangeFull";
            // 
            // checkBoxUseNewRange
            // 
            resources.ApplyResources(this.checkBoxUseNewRange, "checkBoxUseNewRange");
            this.checkBoxUseNewRange.Name = "checkBoxUseNewRange";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // PropertyPageObjectAnalogRectangle
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.checkBoxDisplayNumber);
            this.Controls.Add(this.groupBox1);
            this.Name = "PropertyPageObjectAnalogRectangle";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCornerRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGuideSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGuideSmall)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGuideBig)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void label6_Click(object sender, System.EventArgs e)
		{
		
		}

		private void buttonGuideBigColor_Click(object sender, System.EventArgs e)
		{
			FormColorDialog dialog = new FormColorDialog();

			dialog.SetSelectedColor(buttonGuideBigColor.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				buttonGuideBigColor.BackColor = dialog.GetSelectedColor();
			}
		}

		private void buttonGuideSmallColor_Click(object sender, System.EventArgs e)
		{
			FormColorDialog dialog = new FormColorDialog();

			dialog.SetSelectedColor(buttonGuideSmallColor.BackColor, true);
            dialog.StartPosition = FormStartPosition.CenterParent;

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				buttonGuideSmallColor.BackColor = dialog.GetSelectedColor();
			}
		}

		public void SetViewRange(VIEW_RANGE_STRUCT args)
		{
			multiSelectUseNewRange.Set(args.flag);
			multiSelectNewRangeFull.Set(args.fFull);
			multiSelectNewRangeBase.Set(args.fBase);
		}

		public VIEW_RANGE_STRUCT GetViewRange(VIEW_RANGE_STRUCT org)
		{
			VIEW_RANGE_STRUCT args = (VIEW_RANGE_STRUCT)Tools.CopyObject(org);

			multiSelectUseNewRange.Get(ref args.flag);
			multiSelectNewRangeFull.Get(ref args.fFull);
			multiSelectNewRangeBase.Get(ref args.fBase);

			return args;
		}

		public void SetGuideLine(GUIDE_LINE_STRUCT args)
		{
			multiSelectDisplayNumber.Set(args.bLevelString);
			multiSelectGuideBigColor.Set(args.colorBig);
			multiSelectGuideSmallColor.Set(args.colorSmall);
			multiSelectGuideBig.Set(args.devideBig);
			multiSelectGuideSmall.Set(args.devideSmall);
			multiSelectGuideSize.Set(args.line_length);
			multiSelectGuideDisplay.Set(args.method);
		}

		public GUIDE_LINE_STRUCT GetGuideLine(GUIDE_LINE_STRUCT org)
		{
			GUIDE_LINE_STRUCT args = (GUIDE_LINE_STRUCT)Tools.CopyObject(org);

			multiSelectDisplayNumber.Get(ref args.bLevelString);
			multiSelectGuideBigColor.Get(ref args.colorBig);
			multiSelectGuideSmallColor.Get(ref args.colorSmall);
			multiSelectGuideBig.Get(ref args.devideBig);
			multiSelectGuideSmall.Get(ref args.devideSmall);
			multiSelectGuideSize.Get(ref args.line_length);
			multiSelectGuideDisplay.Get(ref args.method);

			return args;
		}

		public void SetObjectArgs(ObjectArgsAnalogRectangle args) 
		{
			multiSelectBarDir.Set(args.nBarDir);

            // cornerRadius 값을 0~100 범위로 제한
            if (args.cornerRadius < 0)
            {
                args.cornerRadius = 0;
            }
            else if (args.cornerRadius > 100)
            {
                args.cornerRadius = 100;
            }
            this.numericUpDownCornerRadius.Value = args.cornerRadius;   //20250204 PSU 추가
		}

		public ObjectArgsAnalogRectangle GetObjectArgs(ObjectArgsAnalogRectangle org) 
		{
			ObjectArgsAnalogRectangle args = (ObjectArgsAnalogRectangle)Tools.CopyObject(org);
			multiSelectBarDir.Get(ref args.nBarDir);
            args.cornerRadius = ConvertTool.ToInt32(this.numericUpDownCornerRadius.Value);  //20250204 PSU 추가

			return args;
		}

		private void radioButtonBarDir0_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

		private void radioButtonBarDir1_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}

		/*
		 * 
		 * 
		
		public VIEW_RANGE_STRUCT ViewRange 
		{
			set 
			{
				this.checkBoxUseNewRange.Checked = (value.flag == 1);
				this.textBoxNewRangeFull.Text = value.fFull.ToString();
				this.textBoxNewRangeBase.Text = value.fBase.ToString();
			}
			get 
			{
				VIEW_RANGE_STRUCT view = new VIEW_RANGE_STRUCT();

				view.flag = this.checkBoxUseNewRange.Checked ? (sbyte)1 : (sbyte)0;
				view.fFull = ConvertTool.ToDouble(this.textBoxNewRangeFull.Text);
				view.fBase = ConvertTool.ToDouble(this.textBoxNewRangeBase.Text);

				return view;
			}
		}

		public GUIDE_LINE_STRUCT GuideLine
		{
			set 
			{
				this.checkBoxDisplayNumber.Checked = (value.bLevelString == 1);
				this.buttonGuideBigColor.BackColor = value.colorBig;
				this.buttonGuideSmallColor.BackColor = value.colorSmall;
				this.numericUpDownGuideBig.Value = value.devideBig;
				this.numericUpDownGuideSmall.Value = value.devideSmall;
				this.numericUpDownGuideSize.Value = value.line_length;

				this.radioButtonGuideDisplay0.Checked = (value.method == 0);
				this.radioButtonGuideDisplay1.Checked = (value.method == 1);
				this.radioButtonGuideDisplay2.Checked = (value.method == 2);
			}
			get 
			{
				GUIDE_LINE_STRUCT view = new GUIDE_LINE_STRUCT();

				view.bLevelString = this.checkBoxDisplayNumber.Checked ? (sbyte)1 : (sbyte)0;
				view.colorBig = this.buttonGuideBigColor.BackColor;
				view.colorSmall = this.buttonGuideSmallColor.BackColor;
				view.devideBig = ConvertTool.ToInt32(this.numericUpDownGuideBig.Value);
				view.devideSmall = ConvertTool.ToInt32(this.numericUpDownGuideSmall.Value);
				view.line_length = ConvertTool.ToInt32(this.numericUpDownGuideSize.Value);

				if(radioButtonGuideDisplay0.Checked)		view.method = 0;
				else if(radioButtonGuideDisplay1.Checked)	view.method = 1;
				else if(radioButtonGuideDisplay2.Checked)	view.method = 2;
				else										view.method = 0;

				return view;
			}
		}
		
		public ObjectArgsAnalogRectangle ObjectArgs 
		{
			get 
			{
				ObjectArgsAnalogRectangle args = new ObjectArgsAnalogRectangle();

				if(radioButtonBarDir0.Checked)				args.nBarDir = 0;
				else if(radioButtonBarDir1.Checked)			args.nBarDir = 1;
				else if(radioButtonBarDir2.Checked)			args.nBarDir = 2;
				else if(radioButtonBarDir3.Checked)			args.nBarDir = 3;
				else										args.nBarDir = 0;

				return args;
			}
			set 
			{
				this.radioButtonBarDir0.Checked = (value.nBarDir == 0);
				this.radioButtonBarDir1.Checked = (value.nBarDir == 1);
				this.radioButtonBarDir2.Checked = (value.nBarDir == 2);
				this.radioButtonBarDir3.Checked = (value.nBarDir == 3);
			}
		}
		*/
	}
}
