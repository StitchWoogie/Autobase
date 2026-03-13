using System;
using AutoLib;
using NetTools;
using System.Drawing;

namespace BasicScreen.kdymain
{
	/// <summary>
	/// Summary description for ButtonCheck2.
	/// </summary>
	public class ButtonCheck2 //: System.Windows.Forms.Form
	{
		bool	bCaptureFlag = false, bMouseInFlag = false;
		int		nSaveX1, nSaveY1, nSaveX2, nSaveY2, nSaveID;
		string	saveBuf;
		Font	saveFont;


		public ButtonCheck2()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void ButtonCheckDraw2(Graphics g, int x1, int y1, int x2, int y2, Font f, string buf, StringAlignment alignment)
		{
			
			StringFormat	format = new StringFormat();
			format.Alignment = alignment;//StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			format.FormatFlags = StringFormatFlags.NoWrap;
			
			if(bMouseInFlag == false) 
			{
				DrawClass.PopRectangle2(g, x1, y1, x2, y2);
				DrawClass.GrayDrawText(g, x1, y1, x2-x1, y2-y1, buf, Color.Gray, f, format);
			}
			else 
			{
				DrawClass.PushRectangle2(g, x1, y1, x2, y2);
				DrawClass.GrayDrawText(g, x1+2, y1+2, x2-x1, y2-y1, buf, Color.Gray, f, format);
			}
		}

		public bool ButtonCheckDown2(System.Windows.Forms.Form form, System.Windows.Forms.MouseEventArgs e, int x1, int y1, int x2, int y2, Font f, string buf, int id)
		{			
			if(e.X >= x1 && e.Y >= y1 && e.X <= x2 && e.Y <= y2) 
			{
				bCaptureFlag = true;
				bMouseInFlag = true;		// 마우스가 버턴속에 있다.
				nSaveX1 = x1;
				nSaveY1 = y1;
				nSaveX2 = x2;
				nSaveY2 = y2;
				nSaveID = id;
				saveFont = f;
				saveBuf = buf;
				Rectangle  rect = new Rectangle(x1, y1, x2-x1+1, y2-y1+1);
				form.Invalidate(rect);				
				return true;
			}
			return false;
		}

		public bool ButtonCheckMove2(System.Windows.Forms.Form form, System.Windows.Forms.MouseEventArgs e)
		{
			if(bCaptureFlag == false)	return false;	// 마우스가 눌러져 있지 않다.
			bool	_in;
			
			if(e.X >= nSaveX1 && e.Y >= nSaveY1 && e.X <= nSaveX2 && e.Y <= nSaveY2) _in = true;
			else																	 _in = false;

			if(bMouseInFlag == _in)	return true;			// 마우스 움직임에 변화를 줄필요가 없다.

			Rectangle  rect = new Rectangle(nSaveX1, nSaveY1, nSaveX2-nSaveX1+1, nSaveY2-nSaveY1+1);
			bMouseInFlag = _in;
			form.Invalidate(rect);
			return true;
		}

		//------------------------------------------------------------------------------
		//	return - (-1) 체크된 버턴이 없다.
		// return - else (체크된 ID)
		//------------------------------------------------------------------------------

		public int ButtonCheckUp2(System.Windows.Forms.Form form)
		{
			if(bCaptureFlag == false)	return -1;
			bCaptureFlag = false;			
			if(bMouseInFlag == false)	return -1;

			Rectangle  rect = new Rectangle(nSaveX1, nSaveY1, nSaveX2-nSaveX1+1, nSaveY2-nSaveY1+1);
			form.Invalidate(rect);
			bCaptureFlag = false;
			bMouseInFlag = false;
			return nSaveID;
		}

	}
}
