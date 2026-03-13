using System;

namespace ReportBasicLib
{
	/// <summary>
	/// Summary description for ScriptArgumentString.
	/// </summary>
	public class ScriptArgumentString
	{
		int scanBufPos;
		int scanBufHap;
		string scanBuf;

		public ScriptArgumentString()
		{
			//
			// TODO: Add constructor logic here
			//
			scanBufPos = 0;
			scanBufHap = 0;
			scanBuf = "";
		}

		public void Set(string str)
		{
			scanBuf = str;

			scanBufPos = 0;
			scanBufHap = str.Length;
		}	

		public void GetArgument(ref string str)
		{
			int hap = 0;
			int open = 0, close = 0;
			bool string_open = false;
			str = "";

			while(true) 
			{
				if(scanBufPos >= scanBufHap) 
				{
					return;
				}
				if(scanBuf[scanBufPos] == '\n' || scanBuf[scanBufPos] == '\r') 
				{
					scanBufPos++;
					return;
				}
				if(open == close && string_open == false) 
				{
					if(scanBuf[scanBufPos] == ',') 
					{
						scanBufPos++;
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
				else {}

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


/*











*/ 