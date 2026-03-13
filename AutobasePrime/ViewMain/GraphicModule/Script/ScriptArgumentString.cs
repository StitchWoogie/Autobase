using System;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for ScriptArgumentString.
	/// </summary>
	/// 
	public class ScriptArgumentString
	{
		string scanBuf;
		int scanBufPos = 0;
		int scanBufHap = 0;

		public ScriptArgumentString()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void Set(string str, int size)
		{
			scanBuf = str.Substring(0, size);
			scanBufPos = 0;
			scanBufHap = size;
		}

		public void Set(string str)
		{
			Set(str, str.Length);
		}

		public void GetArgument(out string str)
		{
			int hap = 0;
			int open = 0, close = 0;
			bool string_open = false;
			str = "";

			while(true) 
			{
				if(scanBufPos >= scanBufHap) 
				{
					str = str.Trim();
					return;
				}

				if(scanBuf[scanBufPos] == '\n' || scanBuf[scanBufPos] == '\r') 
				{
					scanBufPos++;
					str = str.Trim();
					return;
				}
				if(open == close && string_open == false) 
				{
					if(scanBuf[scanBufPos] == ',') 
					{
						scanBufPos++;
						str = str.Trim();
						return;
					}
				}
		
				if(scanBuf[scanBufPos] == '(') 
				{
					open ++;
				}
				else if(scanBuf[scanBufPos] == ')') 
				{
					close ++;
				}
				else if(scanBuf[scanBufPos] == '"') 
				{
					string_open = string_open ? false : true;
				}
				else{}

				if(hap == 0 && (scanBuf[scanBufPos] == ' '  || scanBuf[scanBufPos] == '\t' ||
					scanBuf[scanBufPos] == '\n' || scanBuf[scanBufPos] == '\r')) 
				{
				}
				else 
				{
					str += scanBuf[scanBufPos];
					hap++;
				}

				scanBufPos++;
			}
		}

	}
}

