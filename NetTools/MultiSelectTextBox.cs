using System;
using System.Collections;
using System.Windows.Forms;
using NetTools;
using System.Drawing;

namespace NetTools
{
	/// <summary>
	/// Summary description for ControlMultiSelect.
	/// </summary>
	public class MultiSelectTextBox
	{
		bool bHex = false;
		public MultiSelectTextBox()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public MultiSelectTextBox(bool hex)
		{
			//
			// TODO: Add constructor logic here
			//
			bHex = hex;
		}

		TextBox array;
			
		public void Add(TextBox obj)
		{
			array = obj;

			array.TextChanged += new System.EventHandler(this.local_TextChanged);
		}

		private void local_TextChanged(object sender, System.EventArgs e)
		{
			array.BackColor = Color.White;
		}

		int nCheckPos = -1;	// -1은 초기상태, -2는 서로 맞지 않는 상태

		/*
		string ToHexBuf(uint val)
		{
			uint remain;
			string imsi = "";
			while(true) 
			{
				remain = val%16;
				val = val/16;

				if(remain > 9) 
				{
					imsi += (char)('A'+(remain-10));
				}
				else 
				{
					imsi += (char)('0'+remain);
				}

				if(val == 0)	break;
			}

			string result = "";

			for(int i = 0, pos=imsi.Length-1; i < imsi.Length; i++, pos--) 
			{
				result += imsi[pos];
			}

			return result;
		}
		*/

		public void Set(short val)
		{
			if(bHex)
				Set(val.ToString("X"));
			else
				Set(val.ToString());
		}

		public void Set(ushort val)
		{
			if(bHex)
				Set(val.ToString("X"));
			else
				Set(val.ToString());
		}

		public void Set(uint val)
		{
			if(bHex)
				Set(val.ToString("X"));
			else
				Set(val.ToString());
		}

        public void Set(int val)
        {
            if (bHex)
                Set(val.ToString("X"));
            else
                Set(val.ToString());
        }

		public int nDecimalPointLow = 2;	// 소수점 이하 자릿수, -1 = 자동 변환 

		public void Set(float val)
		{
			if(bHex) 
				Set(val.ToString("X"));
			else 
			{
                if(nDecimalPointLow == -1)
                    Set(val.ToString());
                else 
				    Set(val.ToString("F"+nDecimalPointLow.ToString()));
			}
		}

		public void Set(double val)
		{
			if(bHex) 
				Set(val.ToString("X"));
			else 
			{
                if (nDecimalPointLow == -1)
                    Set(val.ToString());
                else
				    Set(val.ToString("F"+nDecimalPointLow.ToString()));
			}
		}

		public static Color colorReservation = Color.FromArgb(230, 230, 255);

		public void Set(string text)
		{
			if(nCheckPos == -2)	return;	// 2개이상 비교해서 서로 맞지 않는다.

			if(nCheckPos == -1) 
			{	// first execute
				array.Text = text;
				nCheckPos = 0;
			}
			else 
			{
				if(text == array.Text)	return;	// same

				array.Text = "";

				array.BackColor = colorReservation;
				nCheckPos = -2;
			}
		}

		public bool Get(ref string text)
		{
			// 서로 맞지 않는 텍스트를 사용자가 입력한 적이 없다.
			if(nCheckPos == -2 && array.Text.Length == 0) 
			//if(array.BackColor == colorReservation)
			{
				return false;
			}
		
			text = array.Text;
			return true;
		}

		uint HexBufToUint(string text)
		{
			CommaBlockString comma = new CommaBlockString();
			uint hex = 0;
			comma.Set(text);
			comma.GetHexDWORD(ref hex);
			return hex;
		}

		public void Get(ref short val)
		{
			string text = val.ToString();
			
			if(Get(ref text)) 
			{
				if(bHex) 
				{
					val = (short)HexBufToUint(text);
				}
				else
					val = (short)ConvertTool.ToInt32(text);
			}
		}

		public void Get(ref ushort val)
		{
			string text = val.ToString();
			
			if(Get(ref text)) 
			{
				if(bHex) 
				{
					val = (ushort)HexBufToUint(text);
				}
				else
					val = (ushort)ConvertTool.ToInt32(text);
			}
		}

		public void Get(ref float val)
		{
			string text = val.ToString();
			
			if(Get(ref text)) 
			{
				if(bHex) 
				{
					val = (float)HexBufToUint(text);
				}
				else
					val = ConvertTool.ToSingle(text);
			}
		}

		public void Get(ref double val)
		{
			string text = val.ToString();
			
			if(Get(ref text)) 
			{
				if(bHex) 
				{
					val = (double)HexBufToUint(text);
				}
				else
					val = ConvertTool.ToDouble(text);
			}
		}

		public void Get(ref uint val)
		{
			string text = val.ToString();
			
			if(Get(ref text)) 
			{
				if(bHex) 
				{
					val = (uint)HexBufToUint(text);
				}
				else
					val = (uint)ConvertTool.ToInt32(text);
			}
		}

        public void Get(ref int val)
        {
            string text = val.ToString();

            if (Get(ref text))
            {
                if (bHex)
                {
                    val = (int)HexBufToUint(text);
                }
                else
                    val = (int)ConvertTool.ToInt32(text);
            }
        }

		public void Get(ref sbyte val)
		{
			string text = val.ToString();
			
			if(Get(ref text)) 
			{
				if(bHex) 
				{
					val = (sbyte)HexBufToUint(text);
				}
				else
					val = (sbyte)ConvertTool.ToInt32(text);
			}
		}
	}
}
