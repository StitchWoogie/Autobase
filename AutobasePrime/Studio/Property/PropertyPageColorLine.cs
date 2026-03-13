using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageColor.
	/// </summary>
	public class PropertyPageColorLine : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panelColor;
		private System.Windows.Forms.Panel panelThick;
		private System.Windows.Forms.VScrollBar vScrollBarThick;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonOption0;
		private System.Windows.Forms.RadioButton radioButtonOption2;
		private System.Windows.Forms.RadioButton radioButtonOption4;
		private System.Windows.Forms.RadioButton radioButtonOption5;
		private System.Windows.Forms.RadioButton radioButtonOption3;
		private System.Windows.Forms.RadioButton radioButtonOption1;
		PropertyPageColorPublic formChild = new PropertyPageColorPublic();

		bool bMultiSelectThick = false;
		bool bMultiSelectOption = false;
		int  nLineThick;
        private Label labelLineThick;
		int  nLineOption;

		public void SetSelectedColor(Color val, bool first_flag)
		{
			formChild.SetSelectedColor(val, first_flag);
		}

		public Color GetSelectedColor()
		{
			return formChild.GetSelectedColor();
		}

		public int GetSelectedThick()
		{
			return nLineThick;
		}

		public int GetSelectedOption()
		{
			return nLineOption;
		}

		public bool IsMultiSelectedColor()
		{
			return formChild.IsMultiSelected();
		}

		public bool IsMultiSelectedThick()
		{
			return bMultiSelectThick;
		}

		public bool IsMultiSelectedOption()
		{
			return bMultiSelectOption;
		}

		public PropertyPageColorLine()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageColorLine));
            this.panelColor = new System.Windows.Forms.Panel();
            this.panelThick = new System.Windows.Forms.Panel();
            this.labelLineThick = new System.Windows.Forms.Label();
            this.vScrollBarThick = new System.Windows.Forms.VScrollBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonOption5 = new System.Windows.Forms.RadioButton();
            this.radioButtonOption3 = new System.Windows.Forms.RadioButton();
            this.radioButtonOption1 = new System.Windows.Forms.RadioButton();
            this.radioButtonOption4 = new System.Windows.Forms.RadioButton();
            this.radioButtonOption2 = new System.Windows.Forms.RadioButton();
            this.radioButtonOption0 = new System.Windows.Forms.RadioButton();
            this.panelThick.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelColor
            // 
            this.panelColor.AccessibleDescription = null;
            this.panelColor.AccessibleName = null;
            resources.ApplyResources(this.panelColor, "panelColor");
            this.panelColor.BackgroundImage = null;
            this.panelColor.Font = null;
            this.panelColor.Name = "panelColor";
            // 
            // panelThick
            // 
            this.panelThick.AccessibleDescription = null;
            this.panelThick.AccessibleName = null;
            resources.ApplyResources(this.panelThick, "panelThick");
            this.panelThick.BackgroundImage = null;
            this.panelThick.Controls.Add(this.labelLineThick);
            this.panelThick.Font = null;
            this.panelThick.Name = "panelThick";
            this.panelThick.Paint += new System.Windows.Forms.PaintEventHandler(this.panelThick_Paint);
            // 
            // labelLineThick
            // 
            this.labelLineThick.AccessibleDescription = null;
            this.labelLineThick.AccessibleName = null;
            resources.ApplyResources(this.labelLineThick, "labelLineThick");
            this.labelLineThick.Font = null;
            this.labelLineThick.Name = "labelLineThick";
            // 
            // vScrollBarThick
            // 
            this.vScrollBarThick.AccessibleDescription = null;
            this.vScrollBarThick.AccessibleName = null;
            resources.ApplyResources(this.vScrollBarThick, "vScrollBarThick");
            this.vScrollBarThick.BackgroundImage = null;
            this.vScrollBarThick.Font = null;
            this.vScrollBarThick.Maximum = 29;
            this.vScrollBarThick.Minimum = 1;
            this.vScrollBarThick.Name = "vScrollBarThick";
            this.vScrollBarThick.Value = 1;
            this.vScrollBarThick.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBarThick_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.radioButtonOption5);
            this.groupBox1.Controls.Add(this.radioButtonOption3);
            this.groupBox1.Controls.Add(this.radioButtonOption1);
            this.groupBox1.Controls.Add(this.radioButtonOption4);
            this.groupBox1.Controls.Add(this.radioButtonOption2);
            this.groupBox1.Controls.Add(this.radioButtonOption0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonOption5
            // 
            this.radioButtonOption5.AccessibleDescription = null;
            this.radioButtonOption5.AccessibleName = null;
            resources.ApplyResources(this.radioButtonOption5, "radioButtonOption5");
            this.radioButtonOption5.BackgroundImage = null;
            this.radioButtonOption5.Font = null;
            this.radioButtonOption5.Name = "radioButtonOption5";
            this.radioButtonOption5.CheckedChanged += new System.EventHandler(this.radioButtonOption5_CheckedChanged);
            // 
            // radioButtonOption3
            // 
            this.radioButtonOption3.AccessibleDescription = null;
            this.radioButtonOption3.AccessibleName = null;
            resources.ApplyResources(this.radioButtonOption3, "radioButtonOption3");
            this.radioButtonOption3.BackgroundImage = null;
            this.radioButtonOption3.Font = null;
            this.radioButtonOption3.Name = "radioButtonOption3";
            this.radioButtonOption3.CheckedChanged += new System.EventHandler(this.radioButtonOption3_CheckedChanged);
            // 
            // radioButtonOption1
            // 
            this.radioButtonOption1.AccessibleDescription = null;
            this.radioButtonOption1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonOption1, "radioButtonOption1");
            this.radioButtonOption1.BackgroundImage = null;
            this.radioButtonOption1.Font = null;
            this.radioButtonOption1.Name = "radioButtonOption1";
            this.radioButtonOption1.CheckedChanged += new System.EventHandler(this.radioButtonOption1_CheckedChanged);
            // 
            // radioButtonOption4
            // 
            this.radioButtonOption4.AccessibleDescription = null;
            this.radioButtonOption4.AccessibleName = null;
            resources.ApplyResources(this.radioButtonOption4, "radioButtonOption4");
            this.radioButtonOption4.BackgroundImage = null;
            this.radioButtonOption4.Font = null;
            this.radioButtonOption4.Name = "radioButtonOption4";
            this.radioButtonOption4.CheckedChanged += new System.EventHandler(this.radioButtonOption4_CheckedChanged);
            // 
            // radioButtonOption2
            // 
            this.radioButtonOption2.AccessibleDescription = null;
            this.radioButtonOption2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonOption2, "radioButtonOption2");
            this.radioButtonOption2.BackgroundImage = null;
            this.radioButtonOption2.Font = null;
            this.radioButtonOption2.Name = "radioButtonOption2";
            this.radioButtonOption2.CheckedChanged += new System.EventHandler(this.radioButtonOption2_CheckedChanged);
            // 
            // radioButtonOption0
            // 
            this.radioButtonOption0.AccessibleDescription = null;
            this.radioButtonOption0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonOption0, "radioButtonOption0");
            this.radioButtonOption0.BackgroundImage = null;
            this.radioButtonOption0.Font = null;
            this.radioButtonOption0.Name = "radioButtonOption0";
            this.radioButtonOption0.CheckedChanged += new System.EventHandler(this.radioButtonOption0_CheckedChanged);
            // 
            // PropertyPageColorLine
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.vScrollBarThick);
            this.Controls.Add(this.panelThick);
            this.Controls.Add(this.panelColor);
            this.Icon = null;
            this.Name = "PropertyPageColorLine";
            this.Load += new System.EventHandler(this.PropertyPageColor_Load);
            this.panelThick.ResumeLayout(false);
            this.panelThick.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void PropertyPageColor_Load(object sender, System.EventArgs e)
		{
			formChild.TopLevel = false;

			panelColor.Controls.Add(formChild);
			formChild.Show();
			this.vScrollBarThick.Value = 21-nLineThick;
		}

		public void SetSelectedThick(int val, bool first_flag)
		{
			if(val > 20)	val = 20;

			if(first_flag) 
			{
				nLineThick = val;
                SetLineThickText();
			}
			else 
			{
				if(nLineThick != val) 
				{
					bMultiSelectThick = true;
                    SetLineThickText();
				}
			}
		}

		public void SetSelectedOption(int val, bool first_flag)
		{
			//formChild.SetSelectedColor(val, first_flag);
			if(first_flag) 
			{
				nLineOption = val;
				this.radioButtonOption0.Checked = (val == 0);
				this.radioButtonOption1.Checked = (val == 1);
				this.radioButtonOption2.Checked = (val == 2);
				this.radioButtonOption3.Checked = (val == 3);
				this.radioButtonOption4.Checked = (val == 4);
				this.radioButtonOption5.Checked = (val == 5);
			}
			else 
			{
				if(nLineOption != val) 
				{
					bMultiSelectOption = true;

					this.radioButtonOption0.Checked = false;
					this.radioButtonOption1.Checked = false;
					this.radioButtonOption2.Checked = false;
					this.radioButtonOption3.Checked = false;
					this.radioButtonOption4.Checked = false;
					this.radioButtonOption5.Checked = false;
				}
			}
		}

		private void vScrollBarThick_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
            int old_thick = nLineThick;

			nLineThick = 21-this.vScrollBarThick.Value;
			if(nLineThick < 1)	nLineThick = 1;
			if(nLineThick > 20) nLineThick = 20;

            if (old_thick != nLineThick)
            {
                bMultiSelectThick = false;  // 크기를 변경하면 멀티선택을 지워준다.
            }

			panelThick.Invalidate();

            SetLineThickText();
		}

        void SetLineThickText()
        {
            this.labelLineThick.Text = nLineThick.ToString();

            if (bMultiSelectThick) this.labelLineThick.ForeColor = Color.DarkGray;
            else
            {
                this.labelLineThick.ForeColor = Color.Black;
            }
        }

		private void radioButtonOption0_CheckedChanged(object sender, System.EventArgs e)
		{
			bMultiSelectOption = false;
			nLineOption = 0;
		}

		private void radioButtonOption1_CheckedChanged(object sender, System.EventArgs e)
		{
			bMultiSelectOption = false;
			nLineOption = 1;
		}

		private void radioButtonOption2_CheckedChanged(object sender, System.EventArgs e)
		{
			bMultiSelectOption = false;
			nLineOption = 2;
		}

		private void radioButtonOption3_CheckedChanged(object sender, System.EventArgs e)
		{
			bMultiSelectOption = false;
			nLineOption = 3;
		}

		private void radioButtonOption4_CheckedChanged(object sender, System.EventArgs e)
		{
			bMultiSelectOption = false;
			nLineOption = 4;
		}

		private void radioButtonOption5_CheckedChanged(object sender, System.EventArgs e)
		{
			bMultiSelectOption = false;
			nLineOption = 5;
		}

		private void panelThick_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			int pos = panelThick.ClientRectangle.Top+panelThick.ClientSize.Height/2 - nLineThick/2;
			Color color;
			if(bMultiSelectThick)	color = Color.DarkGray;
			else					color = Color.Black;
			DrawClass.gcls(e.Graphics, panelThick.ClientRectangle.Left+2, pos, panelThick.ClientRectangle.Right-2, pos+nLineThick-1, color); 
		}

	}
}
