using System;
using System.Collections;
using System.Windows.Forms;

namespace NetTools
{
	/// <summary>
	/// Summary description for ControlMultiSelect.
	/// </summary>
	public class MultiSelectCheckBox
	{
		public MultiSelectCheckBox()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		CheckBox array;
			
		public void Add(CheckBox obj)
		{
			array = obj;
		}

		int nCheckPos = -1;	// -1은 초기상태, -2는 서로 맞지 않는 상태

		public void Set(int check)
		{
			if(nCheckPos == -2)	return;	// 2개이상 비교해서 서로 맞지 않는다.

			if(nCheckPos == -1) 
			{	// first execute
				array.Checked = check == 0 ? false : true;
				nCheckPos = check;
			}
			else 
			{
				if(check == nCheckPos)	return;	// same

				array.Checked = true;
				array.CheckState = CheckState.Indeterminate;
				nCheckPos = -2;
			}
		}

		public void Set(bool check)
		{
			Set(check ? 1 : 0);
		}

        public void SetFlag(uint flags, int bit_pos)
        {
            bool check = (flags & Tools.DWORD_MASK[bit_pos]) > 0;
            Set(check ? 1 : 0);
        }

        public void GetFlag(ref uint flags, uint bit_pos)
        {
            bool check = (flags & Tools.DWORD_MASK[bit_pos]) > 0;

            Get(ref check);

            if (check)
                flags |= Tools.DWORD_MASK[bit_pos];
            else
                flags &= (0xFFFF - Tools.DWORD_MASK[bit_pos]);
        }

		public void Get(ref short pos)
		{
			if(array.CheckState == CheckState.Indeterminate)	return;	
			
			pos = array.Checked ? (short)1 : (short)0;
		}

		public void Get(ref int pos)
		{
			if(array.CheckState == CheckState.Indeterminate)	return;	
			
			pos = array.Checked ? (int)1 : (int)0;
		}

		public void Get(ref byte pos)
		{
			if(array.CheckState == CheckState.Indeterminate)	return;	
			
			pos = array.Checked ? (byte)1 : (byte)0;
		}

		public void Get(ref sbyte pos)
		{
			if(array.CheckState == CheckState.Indeterminate)	return;	
			
			pos = array.Checked ? (sbyte)1 : (sbyte)0;
		}

		public void Get(ref bool pos)
		{
			if(array.CheckState == CheckState.Indeterminate)	return;	
			
			pos = array.Checked ? true : false;
		}
	}
}
