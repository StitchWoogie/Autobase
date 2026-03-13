using System;

namespace DialogCommon
{
	/// <summary>
	/// Summary description for CodeViewPush.
	/// </summary>
	public class CodeViewPush
	{
		public CodeViewPush()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		const int MAX_RING = 50000;
		int	nTarget = 0;
		int nCurrent = 0;

		int[] ringCode = new int[MAX_RING];
		byte[] ringType = new byte[MAX_RING];

		void AddCode(byte type, int code)
		{
			ringType[nTarget] = type;
			ringCode[nTarget] = code;
			nTarget ++;
			nTarget %= MAX_RING;
		}

		public void DisplayAllRemainCode()
		{
			int code;
			byte type;
			while(nTarget != nCurrent) 
			{
				type = ringType[nCurrent];
				code = ringCode[nCurrent];
				nCurrent ++;
				nCurrent %= MAX_RING;

				if(type == 0)
					FormCodeView.DisplaySendCode(code);
				else if(type == 1)
					FormCodeView.DisplaySendNextLine();
				else if(type == 2)
					FormCodeView.DisplayRecvCode(code);
				else if(type == 3)
					FormCodeView.DisplayRecvNextLine();
				else {}
			}
		}

		public void DisplaySendCode(byte[] codes, int length)
		{
			for(int i = 0; i < length; i++)
				AddCode(0, codes[i]);
		}

		public void DisplaySendCode(string codes, int length)
		{
			for(int i = 0; i < length; i++)
				AddCode(0, codes[i]);
		}

		public void DisplaySendNextLine()
		{
			AddCode(1, 0);
		}

		public void DisplayRecvCode(byte[] codes, int length)
		{
			for(int i = 0; i < length; i++)
				AddCode(2, codes[i]);
		}

		public void DisplayRecvCode(string codes, int length)
		{
			for(int i = 0; i < length; i++)
				AddCode(2, codes[i]);
		}

		public void DisplayRecvNextLine()
		{
			AddCode(3, 0);
		}
	}
}
