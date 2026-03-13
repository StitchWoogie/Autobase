using NetTools;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Studio
{
	/// <summary>
	/// Summary description for PropertyPageColor.
	/// </summary>
	public class PropertyPageButtonThick : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
        private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panelThick;
        private System.Windows.Forms.VScrollBar vScrollBarThick;
		//PropertyPageColorPublic formChild = new PropertyPageColorPublic();

		bool bMultiSelectThick = false;
		bool bMultiSelectOption = false;
		bool bMultiSelectDesignType = false;
		bool bMultiSelectRadius = false;
		bool bMultiSelectLineColor = false;
		int  nLineThick;
        private Label labelLineThick;
		int  nLineOption;
		int  nDesignType;
		int  nRadius;
		Color nLineColor = Color.Black;

		private Label labelDesignType;
		private ComboBox comboBoxDesignType;
		private Label labelRadius;
		private NumericUpDown numericUpDownRadius;
		private Label labelLineColorText;
		private Panel panelLineColor;
		private Label labelThickText;

        //public void SetSelectedColor(Color val, bool first_flag)
        //{
        //    formChild.SetSelectedColor(val, first_flag);
        //}

        //public Color GetSelectedColor()
        //{
        //    return formChild.GetSelectedColor();
        //}

		public int GetSelectedThick()
		{
			return nLineThick;
		}

		public int GetSelectedOption()
		{
			return nLineOption;
		}

        //public bool IsMultiSelectedColor()
        //{
        //    return formChild.IsMultiSelected();
        //}

		public bool IsMultiSelectedThick()
		{
			return bMultiSelectThick;
		}

		public bool IsMultiSelectedOption()
		{
			return bMultiSelectOption;
		}

		public int GetSelectedDesignType()
		{
			return nDesignType;
		}

		public bool IsMultiSelectedDesignType()
		{
			return bMultiSelectDesignType;
		}

		public int GetSelectedRadius()
		{
			return nRadius;
		}

		public bool IsMultiSelectedRadius()
		{
			return bMultiSelectRadius;
		}

		public Color GetSelectedLineColor()
		{
			return nLineColor;
		}

		public bool IsMultiSelectedLineColor()
		{
			return bMultiSelectLineColor;
		}

        public PropertyPageButtonThick()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// border thickness 0 허용 (기존 최소값은 1)
			this.vScrollBarThick.Maximum = 30;  // max reachable = 30 - LargeChange(10) + 1 = 21, thick = 21-21 = 0

			this.comboBoxDesignType.SelectedIndex = 0;

			SetLanguageText();
		}

		void SetLanguageText()
		{
			if (Tools.IsLangKorean())
			{
				this.Text = "버튼 모양";
				this.labelThickText.Text = "테두리:";
				this.labelLineColorText.Text = "라인색:";
				this.labelDesignType.Text = "디자인:";
				this.labelRadius.Text = "둥글기(%):";
			}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageButtonThick));
            this.panelThick = new System.Windows.Forms.Panel();
            this.labelLineThick = new System.Windows.Forms.Label();
            this.vScrollBarThick = new System.Windows.Forms.VScrollBar();
            this.labelThickText = new System.Windows.Forms.Label();
            this.labelLineColorText = new System.Windows.Forms.Label();
            this.panelLineColor = new System.Windows.Forms.Panel();
            this.labelDesignType = new System.Windows.Forms.Label();
            this.comboBoxDesignType = new System.Windows.Forms.ComboBox();
            this.labelRadius = new System.Windows.Forms.Label();
            this.numericUpDownRadius = new System.Windows.Forms.NumericUpDown();
            this.panelThick.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRadius)).BeginInit();
            this.SuspendLayout();
            // 
            // panelThick
            // 
            this.panelThick.Controls.Add(this.labelLineThick);
            resources.ApplyResources(this.panelThick, "panelThick");
            this.panelThick.Name = "panelThick";
            this.panelThick.Paint += new System.Windows.Forms.PaintEventHandler(this.panelThick_Paint);
            // 
            // labelLineThick
            // 
            resources.ApplyResources(this.labelLineThick, "labelLineThick");
            this.labelLineThick.Name = "labelLineThick";
            // 
            // vScrollBarThick
            // 
            resources.ApplyResources(this.vScrollBarThick, "vScrollBarThick");
            this.vScrollBarThick.Maximum = 29;
            this.vScrollBarThick.Minimum = 1;
            this.vScrollBarThick.Name = "vScrollBarThick";
            this.vScrollBarThick.Value = 1;
            this.vScrollBarThick.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBarThick_Scroll);
            // 
            // labelThickText
            // 
            resources.ApplyResources(this.labelThickText, "labelThickText");
            this.labelThickText.Name = "labelThickText";
            // 
            // labelLineColorText
            // 
            resources.ApplyResources(this.labelLineColorText, "labelLineColorText");
            this.labelLineColorText.Name = "labelLineColorText";
            // 
            // panelLineColor
            // 
            this.panelLineColor.BackColor = System.Drawing.Color.Black;
            this.panelLineColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLineColor.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.panelLineColor, "panelLineColor");
            this.panelLineColor.Name = "panelLineColor";
            this.panelLineColor.Click += new System.EventHandler(this.panelLineColor_Click);
            // 
            // labelDesignType
            // 
            resources.ApplyResources(this.labelDesignType, "labelDesignType");
            this.labelDesignType.Name = "labelDesignType";
            // 
            // comboBoxDesignType
            // 
            this.comboBoxDesignType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDesignType.Items.AddRange(new object[] {
            resources.GetString("comboBoxDesignType.Items"),
            resources.GetString("comboBoxDesignType.Items1"),
            resources.GetString("comboBoxDesignType.Items2")});
            resources.ApplyResources(this.comboBoxDesignType, "comboBoxDesignType");
            this.comboBoxDesignType.Name = "comboBoxDesignType";
            this.comboBoxDesignType.SelectedIndexChanged += new System.EventHandler(this.comboBoxDesignType_SelectedIndexChanged);
            // 
            // labelRadius
            // 
            resources.ApplyResources(this.labelRadius, "labelRadius");
            this.labelRadius.Name = "labelRadius";
            // 
            // numericUpDownRadius
            // 
            resources.ApplyResources(this.numericUpDownRadius, "numericUpDownRadius");
            this.numericUpDownRadius.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownRadius.Name = "numericUpDownRadius";
            this.numericUpDownRadius.ValueChanged += new System.EventHandler(this.numericUpDownRadius_ValueChanged);
            // 
            // PropertyPageButtonThick
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.numericUpDownRadius);
            this.Controls.Add(this.labelRadius);
            this.Controls.Add(this.comboBoxDesignType);
            this.Controls.Add(this.labelDesignType);
            this.Controls.Add(this.panelLineColor);
            this.Controls.Add(this.labelLineColorText);
            this.Controls.Add(this.labelThickText);
            this.Controls.Add(this.vScrollBarThick);
            this.Controls.Add(this.panelThick);
            this.Name = "PropertyPageButtonThick";
            this.Load += new System.EventHandler(this.PropertyPageColor_Load);
            this.panelThick.ResumeLayout(false);
            this.panelThick.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRadius)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void PropertyPageColor_Load(object sender, System.EventArgs e)
		{
            //formChild.TopLevel = false;

            //panelColor.Controls.Add(formChild);
            //formChild.Show();
			this.vScrollBarThick.Value = 21-nLineThick;
		}

		public void SetSelectedThick(int val, bool first_flag)
		{
			if(val > 20)	val = 20;
			if(val < 0)		val = 0;

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

            //if(first_flag)
            //{
            //    nLineOption = val;
            //    this.radioButtonOption0.Checked = (val == 0);
            //    this.radioButtonOption1.Checked = (val == 1);
            //    this.radioButtonOption2.Checked = (val == 2);
            //    this.radioButtonOption3.Checked = (val == 3);
            //    this.radioButtonOption4.Checked = (val == 4);
            //    this.radioButtonOption5.Checked = (val == 5);
            //}
            //else
            //{
            //    if(nLineOption != val)
            //    {
            //        bMultiSelectOption = true;

            //        this.radioButtonOption0.Checked = false;
            //        this.radioButtonOption1.Checked = false;
            //        this.radioButtonOption2.Checked = false;
            //        this.radioButtonOption3.Checked = false;
            //        this.radioButtonOption4.Checked = false;
            //        this.radioButtonOption5.Checked = false;
            //    }
            //}
		}

		public void SetSelectedDesignType(int val, bool first_flag)
		{
			if (val < 0) val = 0;
			if (val > 2) val = 2;

			if (first_flag)
			{
				nDesignType = val;
				if (comboBoxDesignType != null)
					comboBoxDesignType.SelectedIndex = val;
			}
			else
			{
				if (nDesignType != val)
				{
					bMultiSelectDesignType = true;
					if (comboBoxDesignType != null)
						comboBoxDesignType.SelectedIndex = -1;
				}
			}
		}

		public void SetSelectedRadius(int val, bool first_flag)
		{
			if (val < 0) val = 0;
			if (val > 100) val = 100;

			if (first_flag)
			{
				nRadius = val;
				if (numericUpDownRadius != null)
					numericUpDownRadius.Value = val;
			}
			else
			{
				if (nRadius != val)
				{
					bMultiSelectRadius = true;
				}
			}
		}

		public void SetSelectedLineColor(Color val, bool first_flag)
		{
			if (first_flag)
			{
				nLineColor = val;
				if (panelLineColor != null)
					panelLineColor.BackColor = val;
			}
			else
			{
				if (nLineColor != val)
				{
					bMultiSelectLineColor = true;
					if (panelLineColor != null)
						panelLineColor.BackColor = Color.DarkGray;
				}
			}
		}

		private void comboBoxDesignType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBoxDesignType.SelectedIndex >= 0)
			{
				nDesignType = comboBoxDesignType.SelectedIndex;
				bMultiSelectDesignType = false;
			}
		}

		private void numericUpDownRadius_ValueChanged(object sender, EventArgs e)
		{
			int old_radius = nRadius;
			nRadius = (int)numericUpDownRadius.Value;

			if (old_radius != nRadius)
			{
				bMultiSelectRadius = false;
			}
		}

		private void panelLineColor_Click(object sender, EventArgs e)
		{
            FormColorDialog dialog = new FormColorDialog();
            dialog.SetSelectedColor(nLineColor, true);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                nLineColor = dialog.GetSelectedColor();
                bMultiSelectLineColor = false;
                panelLineColor.BackColor = nLineColor;
                panelThick.Invalidate();
            }
        }

		private void vScrollBarThick_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
            int old_thick = nLineThick;

			nLineThick = 21-this.vScrollBarThick.Value;
			if(nLineThick < 0)	nLineThick = 0;
			if(nLineThick > 20) nLineThick = 20;

            if (old_thick != nLineThick)
            {
                bMultiSelectThick = false;
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
