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
	public class PropertyPageObjectText : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBoxText;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private GroupBox groupBox2;
        private RadioButton radioButtonHorzAlign2;
        private RadioButton radioButtonHorzAlign1;
        private RadioButton radioButtonHorzAlign0;
        private GroupBox groupBox3;
        private RadioButton radioButtonVertAlign2;
        private RadioButton radioButtonVertAlign1;
        private RadioButton radioButtonVertAlign0;

		MultiSelectTextBox multiSelectText = new MultiSelectTextBox();
        MultiSelectRadioButton multiSelectHorzAlign = new MultiSelectRadioButton();
        private CheckBox checkBoxDirectionVertical;
        private CheckBox checkBoxNoWrap;
        MultiSelectRadioButton multiSelectVertAlign = new MultiSelectRadioButton();

        MultiSelectCheckBox multiSelectDirectionVertical = new MultiSelectCheckBox();
        MultiSelectCheckBox multiSelectNoWrap = new MultiSelectCheckBox();

		public PropertyPageObjectText()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			multiSelectText.Add(this.textBoxText);

            multiSelectHorzAlign.Add(this.radioButtonHorzAlign0,
                                    this.radioButtonHorzAlign1,
                                    this.radioButtonHorzAlign2);
            multiSelectVertAlign.Add(this.radioButtonVertAlign0,
                                    this.radioButtonVertAlign1,
                                    this.radioButtonVertAlign2);

            multiSelectDirectionVertical.Add(this.checkBoxDirectionVertical);
            multiSelectNoWrap.Add(this.checkBoxNoWrap);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageObjectText));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxText = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonHorzAlign2 = new System.Windows.Forms.RadioButton();
            this.radioButtonHorzAlign1 = new System.Windows.Forms.RadioButton();
            this.radioButtonHorzAlign0 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonVertAlign2 = new System.Windows.Forms.RadioButton();
            this.radioButtonVertAlign1 = new System.Windows.Forms.RadioButton();
            this.radioButtonVertAlign0 = new System.Windows.Forms.RadioButton();
            this.checkBoxDirectionVertical = new System.Windows.Forms.CheckBox();
            this.checkBoxNoWrap = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxText);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // textBoxText
            // 
            this.textBoxText.AcceptsReturn = true;
            resources.ApplyResources(this.textBoxText, "textBoxText");
            this.textBoxText.Name = "textBoxText";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonHorzAlign2);
            this.groupBox2.Controls.Add(this.radioButtonHorzAlign1);
            this.groupBox2.Controls.Add(this.radioButtonHorzAlign0);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // radioButtonHorzAlign2
            // 
            resources.ApplyResources(this.radioButtonHorzAlign2, "radioButtonHorzAlign2");
            this.radioButtonHorzAlign2.Name = "radioButtonHorzAlign2";
            // 
            // radioButtonHorzAlign1
            // 
            resources.ApplyResources(this.radioButtonHorzAlign1, "radioButtonHorzAlign1");
            this.radioButtonHorzAlign1.Name = "radioButtonHorzAlign1";
            // 
            // radioButtonHorzAlign0
            // 
            resources.ApplyResources(this.radioButtonHorzAlign0, "radioButtonHorzAlign0");
            this.radioButtonHorzAlign0.Name = "radioButtonHorzAlign0";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButtonVertAlign2);
            this.groupBox3.Controls.Add(this.radioButtonVertAlign1);
            this.groupBox3.Controls.Add(this.radioButtonVertAlign0);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // radioButtonVertAlign2
            // 
            resources.ApplyResources(this.radioButtonVertAlign2, "radioButtonVertAlign2");
            this.radioButtonVertAlign2.Name = "radioButtonVertAlign2";
            // 
            // radioButtonVertAlign1
            // 
            resources.ApplyResources(this.radioButtonVertAlign1, "radioButtonVertAlign1");
            this.radioButtonVertAlign1.Name = "radioButtonVertAlign1";
            // 
            // radioButtonVertAlign0
            // 
            resources.ApplyResources(this.radioButtonVertAlign0, "radioButtonVertAlign0");
            this.radioButtonVertAlign0.Name = "radioButtonVertAlign0";
            // 
            // checkBoxDirectionVertical
            // 
            resources.ApplyResources(this.checkBoxDirectionVertical, "checkBoxDirectionVertical");
            this.checkBoxDirectionVertical.Name = "checkBoxDirectionVertical";
            this.checkBoxDirectionVertical.UseVisualStyleBackColor = true;
            // 
            // checkBoxNoWrap
            // 
            resources.ApplyResources(this.checkBoxNoWrap, "checkBoxNoWrap");
            this.checkBoxNoWrap.Name = "checkBoxNoWrap";
            this.checkBoxNoWrap.UseVisualStyleBackColor = true;
            // 
            // PropertyPageObjectText
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.checkBoxNoWrap);
            this.Controls.Add(this.checkBoxDirectionVertical);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "PropertyPageObjectText";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		
		public void SetObjectArgs(ObjectArgsText args) 
		{
			multiSelectText.Set(args.text);
            multiSelectHorzAlign.Set(args.align.x);
            multiSelectVertAlign.Set(args.align.y);

            multiSelectDirectionVertical.Set(args.formatFlagDirectionVertical);
            multiSelectNoWrap.Set(args.formatFlagNoWrap);
		}

		public ObjectArgsText GetObjectArgs(ObjectArgsText org) 
		{
			ObjectArgsText args = (ObjectArgsText)Tools.CopyObject(org);

			multiSelectText.Get(ref args.text);
            multiSelectHorzAlign.Get(ref args.align.x);
            multiSelectVertAlign.Get(ref args.align.y);

            multiSelectDirectionVertical.Get(ref args.formatFlagDirectionVertical);
            multiSelectNoWrap.Get(ref args.formatFlagNoWrap);

			return args;
		}

		/*
		public ObjectArgsText ObjectArgs 
		{
			set 
			{
				this.textBoxText.Text = value.text;
			}
			get 
			{
				ObjectArgsText args = new ObjectArgsText();
				args.text = this.textBoxText.Text;
				return args;
			}
		}
		*/
	}
}
