using System;
using System.Collections;
using System.Windows.Forms;

namespace NetTools
{
	/// <summary>
	/// Summary description for ControlMultiSelect.
	/// </summary>
	public class MultiSelectComboBox
	{
		public MultiSelectComboBox()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		ComboBoxStyle comboStyle = ComboBoxStyle.DropDownList;	// default

		public MultiSelectComboBox(ComboBoxStyle style)
		{
			//
			// TODO: Add constructor logic here
			//
			comboStyle = style;
		}

		ComboBox array;
			
		public void Add(ComboBox obj)
		{
			array = obj;
		}

		int nCheckPos = -1;	// -1은 초기상태, -2는 서로 맞지 않는 상태

		public void Set(int pos)
		{
			if(nCheckPos == -2)	return;	// 2개이상 비교해서 서로 맞지 않는다.

			if(pos >= array.Items.Count)	return;	// range over

			if(nCheckPos == -1) 
			{	// first execute
				array.SelectedIndex = pos;
				nCheckPos = pos;
			}
			else 
			{
				if(pos == nCheckPos)	return;	// same

				array.SelectedIndex = -1;
				nCheckPos = -2;
			}
		}

		public void Get(ref int pos)
		{
			if(array.SelectedIndex == -1)	return;
			
			pos = array.SelectedIndex;
		}

		public void Get(ref byte pos)
		{
			if(array.SelectedIndex == -1)	return;
			
			pos = (byte)array.SelectedIndex;
		}

        public void Get(ref short pos)
        {
            if (array.SelectedIndex == -1) return;

            pos = (short)array.SelectedIndex;
        }

		// 문자열 인자는 DropDown일때 사용한다.
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

				array.BackColor = MultiSelectTextBox.colorReservation;
				nCheckPos = -2;
			}
		}

		// 문자열 인자는 DropDown일때 사용한다.
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
	}
}
