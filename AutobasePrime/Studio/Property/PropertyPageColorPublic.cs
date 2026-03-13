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
	public class PropertyPageColorPublic : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panelZone;
		
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Panel panelPreview;
		private System.Windows.Forms.NumericUpDown numericUpDownR;
		private System.Windows.Forms.NumericUpDown numericUpDownG;
		private System.Windows.Forms.NumericUpDown numericUpDownB;
		private System.Windows.Forms.NumericUpDown numericUpDownA;

		Color m_color;
		int nSelectColor;
		bool bEditToColorFlag = false;
		private System.Windows.Forms.GroupBox groupBox1;
        private Label label5;
        private TextBox textBoxHex;
		bool bMultiFlag = false;	// 다중 색상이 등록되어 있다.

		public void SetSelectedColor(Color val, bool first_flag)
		{
			if(first_flag) 
			{
				bEditToColorFlag = false;
				m_color = val;
				numericUpDownA.Value = val.A;
				numericUpDownR.Value = val.R;
				numericUpDownG.Value = val.G;
				numericUpDownB.Value = val.B;
				bEditToColorFlag = true;

				SeekSelectColorNumber(val.A, val.R, val.G, val.B);

                UpdateHexFromColor();
			}
			else // 다중 등록일 때 
			{
				if(val == m_color)	return;	// 같은 색이다.
				bMultiFlag = true;
				nSelectColor = -1;
			}
		}

		public Color GetSelectedColor()
		{
			return m_color;
		}

		public bool IsMultiSelected()
		{
			return bMultiFlag;
		}

		public PropertyPageColorPublic()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyPageColorPublic));
            this.panelZone = new System.Windows.Forms.Panel();
            this.numericUpDownR = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDownG = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownB = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownA = new System.Windows.Forms.NumericUpDown();
            this.panelPreview = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxHex = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownA)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelZone
            // 
            resources.ApplyResources(this.panelZone, "panelZone");
            this.panelZone.Name = "panelZone";
            this.panelZone.Tag = "";
            this.panelZone.Paint += new System.Windows.Forms.PaintEventHandler(this.panelZone_Paint);
            this.panelZone.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelZone_MouseDown);
            // 
            // numericUpDownR
            // 
            resources.ApplyResources(this.numericUpDownR, "numericUpDownR");
            this.numericUpDownR.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownR.Name = "numericUpDownR";
            this.numericUpDownR.TextChanged += new System.EventHandler(this.numericUpDownR_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // numericUpDownG
            // 
            resources.ApplyResources(this.numericUpDownG, "numericUpDownG");
            this.numericUpDownG.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownG.Name = "numericUpDownG";
            this.numericUpDownG.TextChanged += new System.EventHandler(this.numericUpDownG_TextChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // numericUpDownB
            // 
            resources.ApplyResources(this.numericUpDownB, "numericUpDownB");
            this.numericUpDownB.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownB.Name = "numericUpDownB";
            this.numericUpDownB.TextChanged += new System.EventHandler(this.numericUpDownB_TextChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // numericUpDownA
            // 
            resources.ApplyResources(this.numericUpDownA, "numericUpDownA");
            this.numericUpDownA.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownA.Name = "numericUpDownA";
            this.numericUpDownA.TextChanged += new System.EventHandler(this.numericUpDownA_TextChanged);
            // 
            // panelPreview
            // 
            resources.ApplyResources(this.panelPreview, "panelPreview");
            this.panelPreview.Name = "panelPreview";
            this.panelPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.panelPreview_Paint);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panelPreview);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // textBoxHex
            // 
            resources.ApplyResources(this.textBoxHex, "textBoxHex");
            this.textBoxHex.Name = "textBoxHex";
            this.textBoxHex.TextChanged += new System.EventHandler(this.textBoxHex_TextChanged);
            // 
            // PropertyPageColorPublic
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.textBoxHex);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericUpDownA);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericUpDownB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericUpDownG);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownR);
            this.Controls.Add(this.panelZone);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PropertyPageColorPublic";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownA)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void panelZone_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics g = e.Graphics;
	
			// TODO: Add your message handler code here
			Rectangle rect = panelZone.ClientRectangle;
			int x1, y1, x2, y2;
			int pos = 0;
			int i, j;
			Color color;

			x1 = 0;
			y1 = 0;
			x2 = 0;
			for(i = 0; i < 16; i++) 
			{
				y1 = 0;
				for(j = 0; j < 16; j++, pos++) 
				{
					x2 = rect.Right*(i+1)/16;
					y2 = rect.Bottom*(j+1)/16;

					color = Color.FromArgb(DEFAULT_RGB.dac[pos*3+0], DEFAULT_RGB.dac[pos*3+1], DEFAULT_RGB.dac[pos*3+2]);

					DrawClass.gcls(g, x1, y1, x2-1, y2-1, color);

					if(pos == nSelectColor) 
					{
						DrawClass.PushRectangle3(g, x1, y1, x2-1, y2-1);
					}

					y1 = y2;
				}
				x1 = x2;
			}
		}

		private void panelZone_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Rectangle rect = panelZone.ClientRectangle;
			int x1, y1, x2, y2;
			int pos = 0;
			int i, j;

			x1 = 0;
			y1 = 0;
			x2 = 0;
			for(i = 0; i < 16; i++) 
			{
				y1 = 0;
				for(j = 0; j < 16; j++, pos++) 
				{
					x2 = rect.Right*(i+1)/16;
					y2 = rect.Bottom*(j+1)/16;

					if(e.X >= x1 && e.X < x2 && e.Y >= y1 && e.Y < y2) 
					{
						
						bMultiFlag = false;
						nSelectColor = pos;
						m_color = Color.FromArgb(DEFAULT_RGB.dac[pos*3+0], DEFAULT_RGB.dac[pos*3+1], DEFAULT_RGB.dac[pos*3+2]);
						bEditToColorFlag = false;
						numericUpDownA.Value = 255;
						numericUpDownR.Value = DEFAULT_RGB.dac[pos*3+0];
						numericUpDownG.Value = DEFAULT_RGB.dac[pos*3+1];
						numericUpDownB.Value = DEFAULT_RGB.dac[pos*3+2];
						bEditToColorFlag = true;

                        UpdateHexFromColor();
						panelZone.Invalidate();
						panelPreview.Invalidate();
						return;
					}

					y1 = y2;
				}
				x1 = x2;
			}
		}

		private void panelPreview_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if(bMultiFlag) 
			{
				
			}
			else
			{
				DrawClass.gcls(e.Graphics, panelPreview.ClientRectangle, m_color);
			}
		}

		void SeekSelectColorNumber(int a, int r, int g, int b)
		{
			m_color = Color.FromArgb(a, r, g, b);

			int i;

			for(i = 0; i < 256; i++) 
			{
				if(r == DEFAULT_RGB.dac[i*3+0] &&
					g == DEFAULT_RGB.dac[i*3+1] &&
					b == DEFAULT_RGB.dac[i*3+2]) 
				{
					nSelectColor = i;
					return;
				}	
			}

			nSelectColor = -1;
		}

		void EditToColor()
		{
			if(!bEditToColorFlag)	return;	// 지금은 하지 않는다.

			bMultiFlag = false;
			
			int a = ConvertTool.ToInt32(numericUpDownA.Value);
			int r = ConvertTool.ToInt32(numericUpDownR.Value);
			int g = ConvertTool.ToInt32(numericUpDownG.Value);
			int b = ConvertTool.ToInt32(numericUpDownB.Value);

			// panelZone의 화면이 떠는것을 방지하기위해 코드 추가
			int old_color = nSelectColor;
			SeekSelectColorNumber(a, r, g, b); 
			if(old_color != nSelectColor) 
			{
				panelZone.Invalidate();
			}
            UpdateHexFromColor();
			panelPreview.Invalidate();
		}

		private void numericUpDownR_TextChanged(object sender, System.EventArgs e)
		{
			EditToColor();
		}

		private void numericUpDownG_TextChanged(object sender, System.EventArgs e)
		{
			EditToColor();
		}

		private void numericUpDownB_TextChanged(object sender, System.EventArgs e)
		{
			EditToColor();
		}

		private void numericUpDownA_TextChanged(object sender, System.EventArgs e)
		{
			EditToColor();
		}

        private void UpdateHexFromColor()
        {
            textBoxHex.Text = string.Format("{0:X2}{1:X2}{2:X2}", m_color.R, m_color.G, m_color.B);
        }

        private void UpdateColorFromHex(string hexColor)
        {
            if (hexColor.Length == 6)
            {
                try
                {
                    int r = Convert.ToInt32(hexColor.Substring(0, 2), 16);
                    int g = Convert.ToInt32(hexColor.Substring(2, 2), 16);
                    int b = Convert.ToInt32(hexColor.Substring(4, 2), 16);

                    bEditToColorFlag = false;
                    numericUpDownR.Value = r;
                    numericUpDownG.Value = g;
                    numericUpDownB.Value = b;
                    bEditToColorFlag = true;

                    EditToColor();
                }
                catch
                {
                    // Invalid hex color, do nothing
                }
            }
        }


        private void textBoxHex_TextChanged(object sender, EventArgs e)
        {
            if (!bEditToColorFlag) return;
            UpdateColorFromHex(textBoxHex.Text);

        }

	}
}
