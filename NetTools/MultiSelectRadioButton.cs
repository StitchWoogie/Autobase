using System;
using System.Collections;
using System.Windows.Forms;

namespace NetTools
{
	/// <summary>
	/// Summary description for ControlMultiSelect.
	/// </summary>
	public class MultiSelectRadioButton
	{
		public MultiSelectRadioButton()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		ArrayList array = new ArrayList();
			
		public void Add(params object[] args)
		{
			for(int i = 0; i < args.Length; i++)
				array.Add(args[i]);
		}

		int nRadioPos = -1;	// -1은 초기상태, -2는 서로 맞지 않는 상태

		public void Set(int pos)
		{
			if(nRadioPos == -2)	return;	// 2개이상 비교해서 서로 맞지 않는다.

			if(pos >= array.Count)	pos = 0;

			if(nRadioPos == -1) 
			{	// first execute
				((RadioButton)array[pos]).Checked = true;
				nRadioPos = pos;
			}
			else 
			{
				if(pos == nRadioPos)	return;	// same

				((RadioButton)array[nRadioPos]).Checked = false;	
				nRadioPos = -2;
			}
		}

		public void Get(ref int pos)
		{
			for(int i = 0; i < array.Count; i++) 
			{
				if(((RadioButton)array[i]).Checked) 
				{
					pos = (int)i;
					return;
				}
			}
		}

		public void Get(ref short pos)
		{
			for(int i = 0; i < array.Count; i++) 
			{
				if(((RadioButton)array[i]).Checked) 
				{
					pos = (short)i;
					return;
				}
			}
		}

		public void Get(ref byte pos)
		{
			for(int i = 0; i < array.Count; i++) 
			{
				if(((RadioButton)array[i]).Checked) 
				{
					pos = (byte)i;
					return;
				}
			}
		}

		public void Get(ref sbyte pos)
		{
			for(int i = 0; i < array.Count; i++) 
			{
				if(((RadioButton)array[i]).Checked) 
				{
					pos = (sbyte)i;
					return;
				}
			}
		}

        public bool IsMultipleSelected()
        {
            return (nRadioPos == -2);	// 2개이상 비교해서 서로 맞지 않는다.
        }
	}
}
