using System;
using System.Windows.Forms;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for KeyScript.
	/// </summary>
	public class KeyScript
	{
		public KeyScript()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static bool KeyScriptCodeToString(int wParam, out string key_string)
		{
			key_string = "";
			string buf = "";

			if(wParam >= 'A' && wParam <= 'Z') 
			{
				buf = String.Format("{0}", (char)wParam);
			}
			else if(wParam >= '0' && wParam <= '9') 
			{
				buf = String.Format("{0}", (char)wParam);
			}
			else if(wParam >= (int)Keys.F1 && wParam <= (int)Keys.F24) 
			{
				buf = String.Format("F{0}", wParam-(int)Keys.F1+1);
			}
			else if(wParam >= (int)Keys.NumPad0 && wParam <= (int)Keys.NumPad9) 
			{
				buf = String.Format("NUM{0}", wParam-(int)Keys.NumPad0);
			}
			else 
			{
				if(wParam == (int)Keys.Escape)			buf = "ESC";
				else if(wParam == (int)Keys.Tab)		buf = "TAB";	
				else if(wParam == (int)Keys.CapsLock)	buf = "CapsLock";
				else if(wParam == (int)Keys.Scroll)		buf = "Scroll";
				else if(wParam == (int)Keys.NumLock)	buf = "NumLock";
				else if(wParam == (int)Keys.Pause)		buf = "PAUSE";
				else if(wParam == (int)Keys.Insert)		buf = "Insert";
				else if(wParam == (int)Keys.Delete)		buf = "Delete";
				else if(wParam == (int)Keys.Home)		buf = "HOME";
				else if(wParam == (int)Keys.End)		buf = "END";
				else if(wParam == (int)Keys.PageUp)		buf = "PageUp";
				else if(wParam == (int)Keys.PageDown)	buf = "PageDown";
				else if(wParam == (int)Keys.Left)		buf = "LEFT";
				else if(wParam == (int)Keys.Right)		buf = "RIGHT";
				else if(wParam == (int)Keys.Up)			buf = "UP";
				else if(wParam == (int)Keys.Down)		buf = "DOWN";
				else if(wParam == (int)Keys.Space)		buf = "SPACE";
				else if(wParam == (int)Keys.Enter)		buf = "ENTER";
				else if(wParam == (int)Keys.Back)		buf = "BACK";
				else if(wParam == (int)Keys.Decimal)	buf = "NUMDEL";
				else if(wParam == (int)Keys.Multiply)	buf = "NUMMUL";
				else if(wParam == (int)Keys.Add)		buf = "NUMADD";
				else if(wParam == (int)Keys.Subtract)	buf = "NUMSUB";
				else if(wParam == (int)Keys.Divide)		buf = "NUMDIV";

				else 
				{
				}
			}

			if(buf.Length != 0) 
			{
				if ((Control.ModifierKeys & Keys.Control) == Keys.Control) 
				{
					key_string = String.Format("Ctl_{0}", buf);
				}
				else if((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
				{
					key_string = String.Format("Sft_{0}", buf);
				}
				else 
				{
					key_string = buf;
				}

				return true;
			}

			return false;
		}
	}
}
