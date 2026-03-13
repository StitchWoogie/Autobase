using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;
using System.Drawing.Drawing2D;
using ReportBasicLib;

namespace ReportModule
{
	/// <summary>
	/// Summary description for FormDialogConfigCellBorder.
	/// </summary>
	public class FormDialogConfigCellBorder : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonLineType0;
		private System.Windows.Forms.RadioButton radioButtonLineType1;
		private System.Windows.Forms.RadioButton radioButtonLineType2;
		private System.Windows.Forms.RadioButton radioButtonLineType3;
		private System.Windows.Forms.RadioButton radioButtonLineType7;
		private System.Windows.Forms.RadioButton radioButtonLineType6;
		private System.Windows.Forms.RadioButton radioButtonLineType5;
		private System.Windows.Forms.RadioButton radioButtonLineType4;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Panel panelPreview;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panelBorder0;
		private System.Windows.Forms.Panel panelBorder1;
		private System.Windows.Forms.Panel panelBorder2;
		private System.Windows.Forms.Panel panelBorder3;
		private System.Windows.Forms.Panel panelBorder4;
		private System.Windows.Forms.Panel panelBorder5;
		private System.Windows.Forms.Panel panelBorder6;
		private System.Windows.Forms.Panel panelBorder7;
		private System.Windows.Forms.Panel panelColor;
		private System.Windows.Forms.Panel panelThick;

		public BORDER_STRUCT[] border = new BORDER_STRUCT[8];

		public FormDialogConfigCellBorder()
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
		protected override void Dispose(bool disposing)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDialogConfigCellBorder));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonLineType7 = new System.Windows.Forms.RadioButton();
            this.radioButtonLineType6 = new System.Windows.Forms.RadioButton();
            this.radioButtonLineType5 = new System.Windows.Forms.RadioButton();
            this.radioButtonLineType4 = new System.Windows.Forms.RadioButton();
            this.radioButtonLineType3 = new System.Windows.Forms.RadioButton();
            this.radioButtonLineType2 = new System.Windows.Forms.RadioButton();
            this.radioButtonLineType1 = new System.Windows.Forms.RadioButton();
            this.radioButtonLineType0 = new System.Windows.Forms.RadioButton();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.panelPreview = new System.Windows.Forms.Panel();
            this.panelBorder0 = new System.Windows.Forms.Panel();
            this.panelBorder1 = new System.Windows.Forms.Panel();
            this.panelBorder2 = new System.Windows.Forms.Panel();
            this.panelBorder3 = new System.Windows.Forms.Panel();
            this.panelBorder4 = new System.Windows.Forms.Panel();
            this.panelBorder5 = new System.Windows.Forms.Panel();
            this.panelBorder6 = new System.Windows.Forms.Panel();
            this.panelBorder7 = new System.Windows.Forms.Panel();
            this.panelColor = new System.Windows.Forms.Panel();
            this.panelThick = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.radioButtonLineType7);
            this.groupBox1.Controls.Add(this.radioButtonLineType6);
            this.groupBox1.Controls.Add(this.radioButtonLineType5);
            this.groupBox1.Controls.Add(this.radioButtonLineType4);
            this.groupBox1.Controls.Add(this.radioButtonLineType3);
            this.groupBox1.Controls.Add(this.radioButtonLineType2);
            this.groupBox1.Controls.Add(this.radioButtonLineType1);
            this.groupBox1.Controls.Add(this.radioButtonLineType0);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonLineType7
            // 
            this.radioButtonLineType7.AccessibleDescription = null;
            this.radioButtonLineType7.AccessibleName = null;
            resources.ApplyResources(this.radioButtonLineType7, "radioButtonLineType7");
            this.radioButtonLineType7.BackgroundImage = null;
            this.radioButtonLineType7.Font = null;
            this.radioButtonLineType7.Name = "radioButtonLineType7";
            this.radioButtonLineType7.CheckedChanged += new System.EventHandler(this.radioButtonLineType7_CheckedChanged);
            // 
            // radioButtonLineType6
            // 
            this.radioButtonLineType6.AccessibleDescription = null;
            this.radioButtonLineType6.AccessibleName = null;
            resources.ApplyResources(this.radioButtonLineType6, "radioButtonLineType6");
            this.radioButtonLineType6.BackgroundImage = null;
            this.radioButtonLineType6.Font = null;
            this.radioButtonLineType6.Name = "radioButtonLineType6";
            this.radioButtonLineType6.CheckedChanged += new System.EventHandler(this.radioButtonLineType6_CheckedChanged);
            // 
            // radioButtonLineType5
            // 
            this.radioButtonLineType5.AccessibleDescription = null;
            this.radioButtonLineType5.AccessibleName = null;
            resources.ApplyResources(this.radioButtonLineType5, "radioButtonLineType5");
            this.radioButtonLineType5.BackgroundImage = null;
            this.radioButtonLineType5.Font = null;
            this.radioButtonLineType5.Name = "radioButtonLineType5";
            this.radioButtonLineType5.CheckedChanged += new System.EventHandler(this.radioButtonLineType5_CheckedChanged);
            // 
            // radioButtonLineType4
            // 
            this.radioButtonLineType4.AccessibleDescription = null;
            this.radioButtonLineType4.AccessibleName = null;
            resources.ApplyResources(this.radioButtonLineType4, "radioButtonLineType4");
            this.radioButtonLineType4.BackgroundImage = null;
            this.radioButtonLineType4.Font = null;
            this.radioButtonLineType4.Name = "radioButtonLineType4";
            this.radioButtonLineType4.CheckedChanged += new System.EventHandler(this.radioButtonLineType4_CheckedChanged);
            // 
            // radioButtonLineType3
            // 
            this.radioButtonLineType3.AccessibleDescription = null;
            this.radioButtonLineType3.AccessibleName = null;
            resources.ApplyResources(this.radioButtonLineType3, "radioButtonLineType3");
            this.radioButtonLineType3.BackgroundImage = null;
            this.radioButtonLineType3.Font = null;
            this.radioButtonLineType3.Name = "radioButtonLineType3";
            this.radioButtonLineType3.CheckedChanged += new System.EventHandler(this.radioButtonLineType3_CheckedChanged);
            // 
            // radioButtonLineType2
            // 
            this.radioButtonLineType2.AccessibleDescription = null;
            this.radioButtonLineType2.AccessibleName = null;
            resources.ApplyResources(this.radioButtonLineType2, "radioButtonLineType2");
            this.radioButtonLineType2.BackgroundImage = null;
            this.radioButtonLineType2.Font = null;
            this.radioButtonLineType2.Name = "radioButtonLineType2";
            this.radioButtonLineType2.CheckedChanged += new System.EventHandler(this.radioButtonLineType2_CheckedChanged);
            // 
            // radioButtonLineType1
            // 
            this.radioButtonLineType1.AccessibleDescription = null;
            this.radioButtonLineType1.AccessibleName = null;
            resources.ApplyResources(this.radioButtonLineType1, "radioButtonLineType1");
            this.radioButtonLineType1.BackgroundImage = null;
            this.radioButtonLineType1.Font = null;
            this.radioButtonLineType1.Name = "radioButtonLineType1";
            this.radioButtonLineType1.CheckedChanged += new System.EventHandler(this.radioButtonLineType1_CheckedChanged);
            // 
            // radioButtonLineType0
            // 
            this.radioButtonLineType0.AccessibleDescription = null;
            this.radioButtonLineType0.AccessibleName = null;
            resources.ApplyResources(this.radioButtonLineType0, "radioButtonLineType0");
            this.radioButtonLineType0.BackgroundImage = null;
            this.radioButtonLineType0.Font = null;
            this.radioButtonLineType0.Name = "radioButtonLineType0";
            this.radioButtonLineType0.CheckedChanged += new System.EventHandler(this.radioButtonLineType0_CheckedChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.AccessibleDescription = null;
            this.buttonOK.AccessibleName = null;
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.BackgroundImage = null;
            this.buttonOK.Font = null;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.AccessibleDescription = null;
            this.buttonCancel.AccessibleName = null;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.BackgroundImage = null;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = null;
            this.buttonCancel.Name = "buttonCancel";
            // 
            // panelPreview
            // 
            this.panelPreview.AccessibleDescription = null;
            this.panelPreview.AccessibleName = null;
            resources.ApplyResources(this.panelPreview, "panelPreview");
            this.panelPreview.BackgroundImage = null;
            this.panelPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelPreview.Font = null;
            this.panelPreview.Name = "panelPreview";
            this.panelPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.panelPreview_Paint);
            this.panelPreview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelPreview_MouseDown);
            // 
            // panelBorder0
            // 
            this.panelBorder0.AccessibleDescription = null;
            this.panelBorder0.AccessibleName = null;
            resources.ApplyResources(this.panelBorder0, "panelBorder0");
            this.panelBorder0.BackgroundImage = null;
            this.panelBorder0.Font = null;
            this.panelBorder0.Name = "panelBorder0";
            this.panelBorder0.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBorder0_Paint);
            this.panelBorder0.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelBorder0_MouseDown);
            // 
            // panelBorder1
            // 
            this.panelBorder1.AccessibleDescription = null;
            this.panelBorder1.AccessibleName = null;
            resources.ApplyResources(this.panelBorder1, "panelBorder1");
            this.panelBorder1.BackgroundImage = null;
            this.panelBorder1.Font = null;
            this.panelBorder1.Name = "panelBorder1";
            this.panelBorder1.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBorder1_Paint);
            this.panelBorder1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelBorder1_MouseDown);
            // 
            // panelBorder2
            // 
            this.panelBorder2.AccessibleDescription = null;
            this.panelBorder2.AccessibleName = null;
            resources.ApplyResources(this.panelBorder2, "panelBorder2");
            this.panelBorder2.BackgroundImage = null;
            this.panelBorder2.Font = null;
            this.panelBorder2.Name = "panelBorder2";
            this.panelBorder2.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBorder2_Paint);
            this.panelBorder2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelBorder2_MouseDown);
            // 
            // panelBorder3
            // 
            this.panelBorder3.AccessibleDescription = null;
            this.panelBorder3.AccessibleName = null;
            resources.ApplyResources(this.panelBorder3, "panelBorder3");
            this.panelBorder3.BackgroundImage = null;
            this.panelBorder3.Font = null;
            this.panelBorder3.Name = "panelBorder3";
            this.panelBorder3.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBorder3_Paint);
            this.panelBorder3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelBorder3_MouseDown);
            // 
            // panelBorder4
            // 
            this.panelBorder4.AccessibleDescription = null;
            this.panelBorder4.AccessibleName = null;
            resources.ApplyResources(this.panelBorder4, "panelBorder4");
            this.panelBorder4.BackgroundImage = null;
            this.panelBorder4.Font = null;
            this.panelBorder4.Name = "panelBorder4";
            this.panelBorder4.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBorder4_Paint);
            this.panelBorder4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelBorder4_MouseDown);
            // 
            // panelBorder5
            // 
            this.panelBorder5.AccessibleDescription = null;
            this.panelBorder5.AccessibleName = null;
            resources.ApplyResources(this.panelBorder5, "panelBorder5");
            this.panelBorder5.BackgroundImage = null;
            this.panelBorder5.Font = null;
            this.panelBorder5.Name = "panelBorder5";
            this.panelBorder5.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBorder5_Paint);
            this.panelBorder5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelBorder5_MouseDown);
            // 
            // panelBorder6
            // 
            this.panelBorder6.AccessibleDescription = null;
            this.panelBorder6.AccessibleName = null;
            resources.ApplyResources(this.panelBorder6, "panelBorder6");
            this.panelBorder6.BackgroundImage = null;
            this.panelBorder6.Font = null;
            this.panelBorder6.Name = "panelBorder6";
            this.panelBorder6.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBorder6_Paint);
            this.panelBorder6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelBorder6_MouseDown);
            // 
            // panelBorder7
            // 
            this.panelBorder7.AccessibleDescription = null;
            this.panelBorder7.AccessibleName = null;
            resources.ApplyResources(this.panelBorder7, "panelBorder7");
            this.panelBorder7.BackgroundImage = null;
            this.panelBorder7.Font = null;
            this.panelBorder7.Name = "panelBorder7";
            this.panelBorder7.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBorder7_Paint);
            this.panelBorder7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelBorder7_MouseDown);
            // 
            // panelColor
            // 
            this.panelColor.AccessibleDescription = null;
            this.panelColor.AccessibleName = null;
            resources.ApplyResources(this.panelColor, "panelColor");
            this.panelColor.BackgroundImage = null;
            this.panelColor.Font = null;
            this.panelColor.Name = "panelColor";
            this.panelColor.Paint += new System.Windows.Forms.PaintEventHandler(this.panelColor_Paint);
            this.panelColor.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelColor_MouseDown);
            // 
            // panelThick
            // 
            this.panelThick.AccessibleDescription = null;
            this.panelThick.AccessibleName = null;
            resources.ApplyResources(this.panelThick, "panelThick");
            this.panelThick.BackgroundImage = null;
            this.panelThick.Font = null;
            this.panelThick.Name = "panelThick";
            this.panelThick.Paint += new System.Windows.Forms.PaintEventHandler(this.panelThick_Paint);
            this.panelThick.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelThick_MouseDown);
            // 
            // FormDialogConfigCellBorder
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.panelBorder0);
            this.Controls.Add(this.panelPreview);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panelBorder1);
            this.Controls.Add(this.panelBorder2);
            this.Controls.Add(this.panelBorder3);
            this.Controls.Add(this.panelBorder4);
            this.Controls.Add(this.panelBorder5);
            this.Controls.Add(this.panelBorder6);
            this.Controls.Add(this.panelBorder7);
            this.Controls.Add(this.panelColor);
            this.Controls.Add(this.panelThick);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDialogConfigCellBorder";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.FormDialogConfigCellBorder_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		static Color tempColor = Color.Black;
		static sbyte	    tempThick = 1;
		static sbyte    tempType = 1;

		Color m_color;
		sbyte m_type;
		sbyte m_thick;

		private void FormDialogConfigCellBorder_Load(object sender, System.EventArgs e)
		{
			m_color = tempColor;
			m_thick = tempThick;
			m_type  = tempType;

			this.radioButtonLineType0.Checked = (m_type == 0);
			this.radioButtonLineType1.Checked = (m_type == 1);
			this.radioButtonLineType2.Checked = (m_type == 2);
			this.radioButtonLineType3.Checked = (m_type == 3);
			this.radioButtonLineType4.Checked = (m_type == 4);
			this.radioButtonLineType5.Checked = (m_type == 5);
			this.radioButtonLineType6.Checked = (m_type == 6);
			this.radioButtonLineType7.Checked = (m_type == 7);
		}

		private void panelBorder0_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			DrawButton(e.Graphics, e.ClipRectangle, border[0].type);
			DrawRectangle(e.Graphics, e.ClipRectangle);
			DrawLineOne(e.Graphics, e.ClipRectangle, 0);			
		}

		private void panelBorder1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			DrawButton(e.Graphics, e.ClipRectangle, border[1].type);
			DrawRectangle(e.Graphics, e.ClipRectangle);
			DrawLineOne(e.Graphics, e.ClipRectangle, 1);			
		}

		private void panelBorder2_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			DrawButton(e.Graphics, e.ClipRectangle, border[2].type);
			DrawRectangle(e.Graphics, e.ClipRectangle);
			DrawLineOne(e.Graphics, e.ClipRectangle, 2);			
		}

		private void panelBorder3_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			DrawButton(e.Graphics, e.ClipRectangle, border[3].type);
			DrawRectangle(e.Graphics, e.ClipRectangle);
			DrawLineOne(e.Graphics, e.ClipRectangle, 3);			
		}

		private void panelBorder4_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			DrawButton(e.Graphics, e.ClipRectangle, border[4].type);
			DrawRectangle(e.Graphics, e.ClipRectangle);
			DrawLineOne(e.Graphics, e.ClipRectangle, 4);			
		}

		private void panelBorder5_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			DrawButton(e.Graphics, e.ClipRectangle, border[5].type);
			DrawRectangle(e.Graphics, e.ClipRectangle);
			DrawLineOne(e.Graphics, e.ClipRectangle, 5);			
		}

		private void panelBorder6_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			DrawButton(e.Graphics, e.ClipRectangle, border[6].type);
			DrawRectangle(e.Graphics, e.ClipRectangle);
			DrawLineOne(e.Graphics, e.ClipRectangle, 6);			
		}

		private void panelBorder7_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			DrawButton(e.Graphics, e.ClipRectangle, border[7].type);
			DrawRectangle(e.Graphics, e.ClipRectangle);
			DrawLineOne(e.Graphics, e.ClipRectangle, 7);			
		}

		void DrawButton(Graphics g, Rectangle rect, sbyte flag)
		{
			if(flag != 0)
				DrawClass.PushBox2(g, rect.Left, rect.Top, rect.Right-1, rect.Bottom-1, Color.White);
			else
				DrawClass.PopBox2(g, rect.Left, rect.Top, rect.Right-1, rect.Bottom-1, Color.LightGray);
		}

		void DrawRectangle(Graphics g, Rectangle rect)
		{
			Pen pen = new Pen(Color.DarkGray, 1);
			pen.DashStyle = DashStyle.Dot;
	
			g.DrawLine(pen, rect.Left+3, rect.Top+3, rect.Right-4, rect.Top+3);
			g.DrawLine(pen, rect.Right-4, rect.Top+3, rect.Right-4, rect.Bottom-4);
			g.DrawLine(pen, rect.Right-4, rect.Bottom-4, rect.Left+3, rect.Bottom-4);
			g.DrawLine(pen, rect.Left+3, rect.Bottom-4, rect.Left+3, rect.Top+3);
			g.DrawLine(pen, rect.Left+3, rect.Bottom/2, rect.Right-4, rect.Bottom/2);
			g.DrawLine(pen, rect.Right/2, rect.Top+3, rect.Right/2, rect.Bottom-4);
		}

		void DrawLineOne(Graphics g, Rectangle rect, int pos)
		{
			DrawLineExtend(g, rect, pos, DashStyle.Solid, 1, Color.Black, 0);
		}

		void DrawLineExtend(Graphics g, Rectangle rect, int pos, DashStyle type, int thick, Color color, int border_type)
		{
			Pen pen = new Pen(color, thick);
			pen.DashStyle = type;

			int x1, y1, x2, y2;

			if(pos == 0) 
			{
				x1 = rect.Left+3;
				x2 = rect.Right-4;
				y1 = rect.Top+3;
				y2 = rect.Top+3;
			}
			else if(pos == 1) 
			{
				x1 = rect.Left+3;
				x2 = rect.Right-4;
				y1 = rect.Bottom/2;
				y2 = rect.Bottom/2;
			}
			else if(pos == 2) 
			{
				x1 = rect.Left+3;
				x2 = rect.Right-4;
				y1 = rect.Bottom-4;
				y2 = rect.Bottom-4;
			}
			else if(pos == 3) 
			{
				x1 = rect.Left+3;
				x2 = rect.Left+3;
				y1 = rect.Top+3;
				y2 = rect.Bottom-4;
			}
			else if(pos == 4) 
			{
				x1 = rect.Right/2;
				x2 = rect.Right/2;
				y1 = rect.Top+3;
				y2 = rect.Bottom-4;
			}
			else if(pos == 5) 
			{
				x1 = rect.Right-4;
				x2 = rect.Right-4;
				y1 = rect.Top+3;
				y2 = rect.Bottom-4;
			}
			else if(pos == 6) 
			{
				x1 = rect.Left+3;
				x2 = rect.Right-4;
				y1 = rect.Top+3;
				y2 = rect.Bottom-4;
			}
			else 
			{
				x1 = rect.Left+3;
				x2 = rect.Right-4;
				y1 = rect.Bottom-4;
				y2 = rect.Top+3;
			}

			if(pos == 0 || pos == 1 || pos == 2) 
			{
				if(border_type == 6) 
				{
					g.DrawLine(pen, x1, y1-1, x2, y2-1);
					g.DrawLine(pen, x1, y1+1, x2, y2+1);
				}
				else 
				{
					g.DrawLine(pen, x1, y1, x2, y2);
				}
			}
			else if(pos == 3 || pos == 4 || pos == 5) 
			{
				if(border_type == 6) 
				{
					g.DrawLine(pen, x1-1, y1, x2-1, y2);
					g.DrawLine(pen, x1+1, y1, x2+1, y2);
				}
				else 
				{
					g.DrawLine(pen, x1, y1, x2, y2);
				}
			}
			else 
			{
				g.DrawLine(pen, x1, y1, x2, y2);
			}
		}

		private void panelPreview_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			DrawClass.gcls(e.Graphics, e.ClipRectangle, Color.White);
			for(int i = 0; i < 8; i++) 
			{
				DrawLinePreview(e.Graphics, e.ClipRectangle, i, border[i]);
			}
		}

		void DrawLinePreview(Graphics g, Rectangle rect, int pos, BORDER_STRUCT border)
		{
			DashStyle type;
			int thick = border.thick;

			if(border.type == 0)		return;
			else if(border.type == 2)	type = DashStyle.Dot;
			else if(border.type == 3)	type = DashStyle.Dash;
			else if(border.type == 4)	type = DashStyle.DashDot;
			else if(border.type == 5)	type = DashStyle.DashDotDot;
			else if(border.type == 6)	
			{
				thick = 1;
				type = DashStyle.Solid;	// 이중선
			}
			else						type = DashStyle.Solid;

			DrawLineExtend(g, rect, pos, type, thick, border.color, border.type);
		}

		private void panelBorder0_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			ChangeBorder(0);
			this.panelBorder0.Invalidate();
			this.panelPreview.Invalidate();
		}

		private void panelBorder1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			ChangeBorder(1);
			this.panelBorder1.Invalidate();
			this.panelPreview.Invalidate();
		}

		private void panelBorder2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			ChangeBorder(2);
			this.panelBorder2.Invalidate();
			this.panelPreview.Invalidate();
		}

		private void panelBorder3_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			ChangeBorder(3);
			this.panelBorder3.Invalidate();
			this.panelPreview.Invalidate();
		}

		private void panelBorder4_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			ChangeBorder(4);
			this.panelBorder4.Invalidate();
			this.panelPreview.Invalidate();
		}

		private void panelBorder5_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			ChangeBorder(5);
			this.panelBorder5.Invalidate();
			this.panelPreview.Invalidate();
		}

		private void panelBorder6_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			ChangeBorder(6);
			this.panelBorder6.Invalidate();
			this.panelPreview.Invalidate();
		}

		private void panelBorder7_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			ChangeBorder(7);
			this.panelBorder7.Invalidate();
			this.panelPreview.Invalidate();
		}

		void ChangeBorder(int pos)
		{
			if(border[pos].type == 0) 
			{
				border[pos].type = m_type;
				border[pos].thick = m_thick;
				border[pos].color = m_color;
			}
			else 
			{
				border[pos].type = 0;
			}
		}

		void GetRadioType()
		{
			if(this.radioButtonLineType0.Checked)		m_type = 0;
			else if(this.radioButtonLineType1.Checked)	m_type = 1;
			else if(this.radioButtonLineType2.Checked)	m_type = 2;
			else if(this.radioButtonLineType3.Checked)	m_type = 3;
			else if(this.radioButtonLineType4.Checked)	m_type = 4;
			else if(this.radioButtonLineType5.Checked)	m_type = 5;
			else if(this.radioButtonLineType6.Checked)	m_type = 6;
			else if(this.radioButtonLineType7.Checked)	m_type = 7;
			else										m_type = 1;
		}

		private void radioButtonLineType0_CheckedChanged(object sender, System.EventArgs e)
		{
			GetRadioType();
		}

		private void radioButtonLineType1_CheckedChanged(object sender, System.EventArgs e)
		{
			GetRadioType();
		}

		private void radioButtonLineType2_CheckedChanged(object sender, System.EventArgs e)
		{
			GetRadioType();
		}

		private void radioButtonLineType3_CheckedChanged(object sender, System.EventArgs e)
		{
			GetRadioType();
		}

		private void radioButtonLineType4_CheckedChanged(object sender, System.EventArgs e)
		{
			GetRadioType();
		}

		private void radioButtonLineType5_CheckedChanged(object sender, System.EventArgs e)
		{
			GetRadioType();
		}

		private void radioButtonLineType6_CheckedChanged(object sender, System.EventArgs e)
		{
			GetRadioType();
		}

		private void radioButtonLineType7_CheckedChanged(object sender, System.EventArgs e)
		{
			GetRadioType();
		}

		private void panelPreview_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Rectangle r;

			r = this.panelPreview.ClientRectangle;

			int[] gab = new int[6];

			gab[0] = Math.Abs(e.Y-r.Top);
			gab[1] = Math.Abs(e.Y-(r.Top+(r.Bottom-r.Top)/2));
			gab[2] = Math.Abs(e.Y-r.Bottom);
			gab[3] = Math.Abs(e.X-r.Left);
			gab[4] = Math.Abs(e.X-(r.Left+(r.Right-r.Left)/2));
			gab[5] = Math.Abs(e.X-r.Right);

			int min = gab[0];
			int min_pos = 0;
			int i;

			// 제일 가까운 Point를 찾는다.
			for(i = 1; i < 6; i++) 
			{
				if(gab[i] < min) 
				{
					min = gab[i];
					min_pos = i;
				}
			}

			if(min_pos == 0) 
			{
				ChangeBorder(0);
				this.panelBorder0.Invalidate();
				this.panelPreview.Invalidate();
			}
			else if(min_pos == 1) 
			{
				ChangeBorder(1);
				this.panelBorder1.Invalidate();
				this.panelPreview.Invalidate();
			}
			else if(min_pos == 2)	
			{
				ChangeBorder(2);
				this.panelBorder2.Invalidate();
				this.panelPreview.Invalidate();
			}
			else if(min_pos == 3) 
			{
				ChangeBorder(3);
				this.panelBorder3.Invalidate();
				this.panelPreview.Invalidate();
			}
			else if(min_pos == 4) 
			{
				ChangeBorder(4);
				this.panelBorder4.Invalidate();
				this.panelPreview.Invalidate();
			}
			else 
			{
				ChangeBorder(5);
				this.panelBorder5.Invalidate();
				this.panelPreview.Invalidate();
			}
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			tempColor = m_color;
			tempThick = m_thick;
			tempType = m_type;

			DialogResult = DialogResult.OK;
			Close();
		}

		private void panelColor_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			DrawClass.PopBox(e.Graphics, e.ClipRectangle, m_color);
		}

		private void panelThick_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			DrawClass.PopBox(e.Graphics, e.ClipRectangle, Color.LightGray);
			Rectangle r = e.ClipRectangle;
			int y1 = r.Bottom/2-m_thick/2;
			int y2 = y1+m_thick-1;
			DrawClass.gcls(e.Graphics, r.Left+3, y1, r.Right-4, y2, Color.Black);
		}

		private void panelColor_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			ColorDialog dialog = new ColorDialog();

			if(dialog.ShowDialog(this) == DialogResult.OK) 
			{
				m_color = dialog.Color;
				this.panelColor.Invalidate();
			}
		}

		private void panelThick_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_thick++;
			if(m_thick >= 6)	m_thick = 1;
			this.panelThick.Invalidate();
		}
	}
}

