using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;

namespace NetTools
{
	/// <summary>
	/// Summary description for ControlMultiSelect.
	/// </summary>
	public class MultiSelectColorButton
	{
		public MultiSelectColorButton()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		Button array;
			
		public void Add(Button obj)
		{
			array = obj;

			array.BackColorChanged += new System.EventHandler(this.local_BackColorChanged);
		}

		bool bSetting = false;	// Set함수중에는 BackColorChanged이벤트를 받으면 안된다.

		private void local_BackColorChanged(object sender, System.EventArgs e)
		{
			if(bSetting)	return;
			nCheckPos = 1;
			array.Text = "";
		}

		int nCheckPos = -1;	// -1은 초기상태, -2는 서로 맞지 않는 상태

		public void Set(Color color)
		{
			if(nCheckPos == -2)	return;	// 2개 이상 비교해서 서로 맞지 않는다.

			bSetting = true;
			if(nCheckPos == -1) 
			{	// first execute
				array.BackColor = color;
				nCheckPos = 1;
			}
			else 
			{
				if(color != array.BackColor) 
				{
					nCheckPos = -2;
					array.BackColor = MultiSelectTextBox.colorReservation;
					array.Text = "?";
				}
			}
			bSetting = false;
		}

		public void Get(ref Color color)
		{
			if(nCheckPos != 1)	return;	
			
			color = array.BackColor;
		}
	}
}
