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
	public class MultiSelectNumericUpDown
	{
		public MultiSelectNumericUpDown()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		NumericUpDown array;
			
		public void Add(NumericUpDown obj)
		{
			array = obj;
			
			array.ValueChanged += new System.EventHandler(this.local_ValueChanged);
            array.Enter += new EventHandler(array_Enter);
		}

        void array_Enter(object sender, EventArgs e)
        {
            ((NumericUpDown)sender).Select(0, 100);
        }

		int nCheckPos = -1;	// -1은 초기상태, -2는 서로 맞지 않는 상태
		decimal nValue;

		private void local_ValueChanged(object sender, System.EventArgs e)
		{
			array.BackColor = Color.White;
		}

		public void Set(decimal val)
		{
			if(val < array.Minimum)	
				val = array.Minimum;
			if(val > array.Maximum)	
				val = array.Maximum;

			if(nCheckPos == -2)	return;	// 2개 이상 비교해서 서로 맞지 않는다.

			if(nCheckPos == -1) 
			{	// first execute
				array.Value = val;
				nValue = val;
				nCheckPos = 0;
			}
			else 
			{
				if(val == nValue)	return;	// same

				array.BackColor = MultiSelectTextBox.colorReservation;
				nCheckPos = -2;
			}
		}

        /*
		public void Set(float val)
		{
			Set((decimal)val);
		}*/

		public bool Get(ref int val)
		{
			// 서로 맞지 않는 텍스트를 사용자가 입력한 적이 없다.
			if(array.BackColor == MultiSelectTextBox.colorReservation) 
			{
				return false;
			}
		
			val = Convert.ToInt32(array.Value);
			return true;
		}

		public bool Get(ref byte val)
		{
			// 서로 맞지 않는 텍스트를 사용자가 입력한 적이 없다.
			if(array.BackColor == MultiSelectTextBox.colorReservation) 
			{
				return false;
			}
		
			val = Convert.ToByte(array.Value);
			return true;
		}

		public bool Get(ref sbyte val)
		{
			// 서로 맞지 않는 텍스트를 사용자가 입력한 적이 없다.
			if(array.BackColor == MultiSelectTextBox.colorReservation) 
			{
				return false;
			}
		
			val = Convert.ToSByte(array.Value);
			return true;
		}

		public bool Get(ref ushort val)
		{
			// 서로 맞지 않는 텍스트를 사용자가 입력한 적이 없다.
			if(array.BackColor == MultiSelectTextBox.colorReservation) 
			{
				return false;
			}
		
			val = Convert.ToUInt16(array.Value);
			return true;
		}

		public bool Get(ref short val)
		{
			// 서로 맞지 않는 텍스트를 사용자가 입력한 적이 없다.
			if(array.BackColor == MultiSelectTextBox.colorReservation) 
			{
				return false;
			}
		
			val = Convert.ToInt16(array.Value);
			return true;
		}

        public bool Get(ref float val)
		{
			// 서로 맞지 않는 텍스트를 사용자가 입력한 적이 없다.
			if(array.BackColor == MultiSelectTextBox.colorReservation) 
			{
				return false;
			}
		
			val = Convert.ToSingle(array.Value);
			return true;
		}

        public bool Get(ref double val)
        {
            // 서로 맞지 않는 텍스트를 사용자가 입력한 적이 없다.
            if (array.BackColor == MultiSelectTextBox.colorReservation)
            {
                return false;
            }

            val = Convert.ToDouble(array.Value);
            return true;
        }
	}
}
