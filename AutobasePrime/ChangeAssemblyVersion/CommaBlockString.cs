using System;

namespace ChangeAssemblyVersion
{
	/// <summary>
	/// Summary description for CommaBlockString.
	/// </summary>
	public class CommaBlockString
	{
		int  scanBufPos;
		int  scanBufHap;
		string scanBuf;
		char cBlockCode;	// 블럭을 구분하는 단위 default ,

		public void SetBlockCode(char code) { cBlockCode = code; }
		public bool IsEOS() { return scanBufPos >= scanBufHap; }

		public CommaBlockString()
		{
			//
			// TODO: Add constructor logic here
			// 
			scanBufPos = 0;
			scanBufHap = 0;
			scanBuf = null;
			cBlockCode = ',';

			//if(s != null)	Set(s);
		}

		public void Set(string s)
		{
			scanBufPos = 0;
			scanBuf = s;
			scanBufHap = scanBuf.Length;
		}

		public void Skip()
		{
			string s="";
			GetString(ref s);
		}

		public void GetString(ref string s)
		{
			s = "";

			while(true) 
			{
				if(scanBufPos >= scanBufHap) 
				{
					return;
				}
				if(scanBuf[scanBufPos] == cBlockCode || scanBuf[scanBufPos] == '\n' || scanBuf[scanBufPos] == '\r') 
				{
					scanBufPos++;
					return;
				}
				
				if(s.Length == 0 && (scanBuf[scanBufPos] == ' '  || scanBuf[scanBufPos] == '\t' ||
						scanBuf[scanBufPos] == '\n' || scanBuf[scanBufPos] == '\r')) 
				{
				}
				else 
				{
					s += scanBuf[scanBufPos];
				}

				scanBufPos++;
			}
		}

		public void GetStringTotalRemain(ref string s)
		{
			s = "";

			while(true) {
				if(scanBufPos >= scanBufHap) {
					return;
				}

				if(s.Length == 0 && (scanBuf[scanBufPos] == ' ' || scanBuf[scanBufPos] == '\t')) {

				}
				else {
					s += scanBuf[scanBufPos];
				}

				scanBufPos++;
			}
		}

		public void GetInt(ref int val)
		{
			string buf = "";
			GetString(ref buf);
			if(buf.Length == 0)	
			{
				val = 0;
				return;
			}

			try 
			{
				val = int.Parse(buf);
			}
			catch 
			{
				val = 0;
			}
		}

		public void GetInt(ref short val)
		{
			string buf = "";
			GetString(ref buf);
			if(buf.Length == 0)	
			{
				val = 0;
				return;
			}

			try 
			{
				val = short.Parse(buf);
			}
			catch 
			{
				val = 0;
			}
		}

		public void GetChar(ref sbyte val)
		{
			string buf = "";
			GetString(ref buf);
			if(buf.Length == 0)	
			{
				val = 0;
				return;
			}

			try 
			{
				val = sbyte.Parse(buf);
			}
			catch 
			{
				val = 0;
			}
		}

		public void GetChar(ref char val)
		{
			string buf = "";
			GetString(ref buf);
			if(buf.Length == 0)	
			{
				val = ' ';
				return;
			}

			try 
			{
				val = char.Parse(buf);
			}
			catch 
			{
				val = ' ';
			}
		}

		public void GetFloat(ref float val)
		{
			string buf = "";
			GetString(ref buf);
			if(buf.Length == 0)	
			{
				val = 0;
				return;
			}

			try 
			{
				val = float.Parse(buf);
			}
			catch 
			{
				val = 0;
			}
		}

		public void GetDouble(ref double val)
		{
			string buf = "";
			GetString(ref buf);
			if(buf.Length == 0)	
			{
				val = 0;
				return;
			}

			try 
			{
				val = double.Parse(buf);
			}
			catch 
			{
				val = 0;
			}
		}

		public void GetDWORD(ref uint val)
		{
			string buf = "";
			GetString(ref buf);
			if(buf.Length == 0)	
			{
				val = 0;
				return;
			}

			try 
			{
				val = uint.Parse(buf);
			}
			catch 
			{
				val = 0;
			}
		}

		public void GetDWORD(ref ulong val)
		{
			string buf = "";
			GetString(ref buf);
			if(buf.Length == 0)	
			{
				val = 0;
				return;
			}

			try 
			{
				val = ulong.Parse(buf);
			}
			catch 
			{
				val = 0;
			}
		}

		public void GetBYTE(ref byte val)
		{
			string buf = "";
			GetString(ref buf);
			if(buf.Length == 0)	
			{
				val = 0;
				return;
			}

			try 
			{
				val = byte.Parse(buf);
			}
			catch 
			{
				val = 0;
			}
		}

		public void GetLong(ref long val)
		{
			string buf = "";
			GetString(ref buf);
			if(buf.Length == 0)	
			{
				val = 0;
				return;
			}

			try 
			{
				val = long.Parse(buf);
			}
			catch 
			{
				val = 0;
			}
		}

		public void GetColor(ref System.Drawing.Color val)
		{
			uint color=0;
			GetDWORD(ref color);
			int bc = (int)((color >> 16) & 0xFF);
			int gc = (int)((color >> 8) & 0xFF);
			int rc = (int)((color >> 0) & 0xFF);
			val = System.Drawing.Color.FromArgb(rc, gc, bc);
		}

		public void GetWORD(ref ushort val)
		{
			string buf = "";
			GetString(ref buf);
			if(buf.Length == 0)	
			{
				val = 0;
				return;
			}

			try 
			{
				val = ushort.Parse(buf);
			}
			catch 
			{
				val = 0;
			}
		}

		public void GetBool(ref bool val)
		{
			string buf = "";
			GetString(ref buf);
			if(String.Compare(buf, "true", true) == 0)
				val = true;
			else
				val = false;
		}

		public void GetHexDWORD(ref uint val)
		{
			string buf = "";
			GetString(ref buf);
			buf = buf.ToUpper();

			int pos;
			uint u;
			uint imsi;
			uint gob = 1;

			val = 0;

			for(u = 0, pos = buf.Length-1; u < buf.Length; u++, pos--) 
			{
				imsi = 0;
				if(buf[pos] >= 'A' && buf[pos] <= 'F') 
				{
					imsi = (uint)(buf[pos]-'A'+10);
				}
				else if(buf[pos] >= '0' && buf[pos] <= '9') 
				{
					imsi = (uint)(buf[pos]-'0');
				}
				imsi = imsi*gob;

				gob *= 0x10;

				val += imsi;
			}
		}

		public void GetHexWORD(ref ushort val)
		{
			uint retn = 0;

			GetHexDWORD(ref retn);

			val = (ushort)retn;
		}

	}
}


/*
 * 
 * 

//----------------------------------------------------------------------------
//	DI address 는 앞세자리는 10진수, 뒤의 한자리는 16진수로 구성된다.
//----------------------------------------------------------------------------

void CommaBlockString :: GetAddressDI(WORD &val)
{
	char buf[10];
	GetString(buf, 9);

	if(buf[0] == 0)	{
		val = 0;
		return;
	}
	val = 0;

	if(buf[3] >= '0' && buf[3] <= '9') {
		val |= ((((unsigned)(buf[3]-'0'))) & 0x000F);
	}
	else if(buf[3] >= 'A' && buf[3] <= 'F') {
		val |= ((((unsigned)(buf[3]-'A')+0x0A)) & 0x000F);
	}
	else;

	buf[3] = 0;
	val += (atoi(buf)*16);
}

void CommaBlockString :: GetCOLORREF(COLORREF &val)
{
	char buf[20];
	GetString(buf, 19);

	val = atol(buf);
}

void CommaBlockString :: GetDate(struct date *d)
{
	char buf[20];
	char imsi[20];

	GetString(buf, 19);

	strncpy(imsi, &buf[0], 4);
	imsi[4] = 0;
	d->da_year = atoi(imsi);

	strncpy(imsi, &buf[5], 2);
	imsi[2] = 0;
	d->da_mon = atoi(imsi);

	strncpy(imsi, &buf[8], 2);
	imsi[2] = 0;
	d->da_day = atoi(imsi);
}

void CommaBlockString :: GetTime(struct time *t)
{
	char buf[20];
	char imsi[20];

	GetString(buf, 19);

	strncpy(imsi, &buf[0], 2);
	imsi[2] = 0;
	t->ti_hour = atoi(imsi);

	strncpy(imsi, &buf[3], 2);
	imsi[2] = 0;
	t->ti_min = atoi(imsi);

	strncpy(imsi, &buf[6], 2);
	imsi[2] = 0;
	t->ti_sec = atoi(imsi);
}

void CommaBlockString :: GetDateTime(SYSTEMTIME *t)
{
	char buf[80];
	char imsi[20];

	ZeroMemory(t, sizeof(SYSTEMTIME));

	GetString(buf, sizeof(buf));

	strncpy(imsi, &buf[0], 4);
	imsi[4] = 0;
	t->wYear = atoi(imsi);

	strncpy(imsi, &buf[5], 2);
	imsi[2] = 0;
	t->wMonth = atoi(imsi);

	strncpy(imsi, &buf[8], 2);
	imsi[2] = 0;
	t->wDay = atoi(imsi);

	strncpy(imsi, &buf[11], 2);
	imsi[2] = 0;
	t->wHour = atoi(imsi);

	strncpy(imsi, &buf[14], 2);
	imsi[2] = 0;
	t->wMinute = atoi(imsi);

	strncpy(imsi, &buf[17], 2);
	imsi[2] = 0;
	t->wSecond = atoi(imsi);
}


*/