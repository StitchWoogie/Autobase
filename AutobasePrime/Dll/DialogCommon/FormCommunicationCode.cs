using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NetTools;

namespace DialogCommon
{
	/// <summary>
	/// Summary description for FormCommunicationCode.
	/// </summary>
	public class FormCommunicationCode : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		int cxChar;
		int cyChar;
		int nPosX;
		int nPosY;
	
		public sbyte cDisplayMethod; // 0 = hex, 1 = ascii, 2 = decimal
		bool bFirstCommunicationFlag;
		public bool bPause;

		public FormCommunicationCode()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			cDisplayMethod = 1;
			bFirstCommunicationFlag = false;
			bPause = false;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCommunicationCode));
            this.SuspendLayout();
            // 
            // FormCommunicationCode
            // 
            resources.ApplyResources(this, "$this");
            this.Name = "FormCommunicationCode";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormCommunicationCode_Paint);
            this.SizeChanged += new System.EventHandler(this.FormCommunicationCode_SizeChanged);
            this.Load += new System.EventHandler(this.FormCommunicationCode_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void FormCommunicationCode_Load(object sender, System.EventArgs e)
		{
			cxChar = this.Font.Height/2;
			cyChar = this.Font.Height;
		}

		private void FormCommunicationCode_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			//DrawClass.gcls(e.Graphics, this.ClientRectangle, BACK_COLOR);
			if(bitmap != null)
				e.Graphics.DrawImage(bitmap, 0, 0);
		}

		Color TEXT_COLOR = Color.FromArgb(255, 255, 255);
		Color BACK_COLOR = Color.FromArgb(0, 0, 0x80);

		public void DisplayCode(string buf, int size) 
		{
			int i;

			for(i = 0; i < size; i++) 
			{
				DisplayOneChar(buf[i]);
			}
		}

		public void DisplayCode(byte[] buf, int size) 
		{
			int i;

			for(i = 0; i < size; i++) 
			{
				DisplayOneChar(buf[i]);
			}
		}

		Bitmap bitmap = null;

		private void FormCommunicationCode_SizeChanged(object sender, System.EventArgs e)
		{
			int width = this.ClientRectangle.Width;
			int height = this.ClientRectangle.Height;

			if(width == 0)	width = 1;
			if(height == 0)	height = 1;

			Graphics g = CreateGraphics();
			bitmap = new Bitmap(width, height, g);

			g = Graphics.FromImage(bitmap);
			DrawClass.gcls(g, this.ClientRectangle, BACK_COLOR);

			this.Invalidate();

			nPosX = 0;
			nPosY = 0;		
		}

		void NextLine(Graphics g)
		{
			if(bPause == true)	return;
	
			nPosX = 0;

			Rectangle rClient = this.ClientRectangle;

			if(nPosY+cyChar*2 >= rClient.Bottom) 
			{
				Bitmap bt = new Bitmap(bitmap);
				Graphics gt = Graphics.FromImage(bt);
				gt.DrawImage(bitmap, 0, -cyChar);
				DrawClass.gcls(gt, 0, nPosY, rClient.Right-1, rClient.Bottom-1, BACK_COLOR);
				bitmap = bt;
				Invalidate();
			}
			else 
			{
				nPosY += cyChar;
			}
		}

		public void DisplayOneChar(int ch)
		{
			if(bPause == true)	return;
	
			string buf;
			SizeF size;
			Color lBackColor;

			if(cDisplayMethod == 0) 
			{
				if(bFirstCommunicationFlag) 
				{
					bFirstCommunicationFlag = false;
					lBackColor = Color.FromArgb(0x80, 0, 0);
				}
				else
					lBackColor = BACK_COLOR;

				buf = String.Format("{0:X02} ", ch);
			}
			else if(cDisplayMethod == 2) 
			{
				if(bFirstCommunicationFlag) 
				{
					bFirstCommunicationFlag = false;
					lBackColor = Color.FromArgb(0x80, 0, 0);
				}
				else
					lBackColor = BACK_COLOR;

				buf = String.Format("{0:000} ", ch);
			}
			else 
			{
				lBackColor = Color.FromArgb(0x80, 0, 0);
				switch(ch) 
				{
					case 0:		lBackColor = Color.FromArgb(0x80, 0x80, 0x80);
						buf = "<NULL>";
						break;							
					case (int)EnumAsciiCode.SOH:	lBackColor = Color.FromArgb(0x80, 0x40, 0x80);
						buf = "<SOH>";
						break;							
					case (int)EnumAsciiCode.STX:   lBackColor = Color.FromArgb(0x80, 0, 0);
						buf = "<STX>";
						break;
					case (int)EnumAsciiCode.ETX:   lBackColor = Color.FromArgb(0, 0x80, 0);
						buf = "<ETX>";
						break;
					case (int)EnumAsciiCode.EOT:	lBackColor = Color.FromArgb(0x80, 0x80, 0);
						buf = "<EOT>";
						break;
					case (int)EnumAsciiCode.ENQ:	lBackColor = Color.FromArgb(0x80, 0, 0x80);
						buf = "<ENQ>";
						break;
					case (int)EnumAsciiCode.ACK:	lBackColor = Color.FromArgb(0, 0x80, 0x80);
						buf =  "<ACK>";
						break;
					case (int)EnumAsciiCode.LF:	buf =  "<LF>";	break;
					case (int)EnumAsciiCode.CR:	buf =  "<CR>";	break;
					case (int)EnumAsciiCode.DLE:	buf =  "<DLE>";	break;
					case (int)EnumAsciiCode.NAK:	buf =  "<NAK>";	break;
					case 0x09:	buf =  "<HT>";	break;
					default:
						lBackColor = BACK_COLOR;
						buf = String.Format("{0}", (char)ch);
						break;
				}
			}

			Graphics g = Graphics.FromImage(bitmap);

			size = g.MeasureString(buf, this.Font);

			if(size.Width+nPosX > this.ClientRectangle.Right) 
			{
				NextLine(g);
			}

			g = Graphics.FromImage(bitmap);

            SafeException.SafeDrawString(g, buf, this.Font, Brushes.White, nPosX, nPosY);

			Invalidate();

			nPosX += (int)size.Width;
		}

		public void DisplayNextLine()
		{
			Graphics g = this.CreateGraphics();

			NextLine(g);
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			//base.OnPaintBackground (pevent);
		}

	}
}


