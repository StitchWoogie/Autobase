using System;
using System.Text;

namespace NetTools
{
	/// <summary>
	/// Summary description for CommaBlockString.
	/// </summary>
	public class CommaBlockString
	{
		protected int  scanBufPos;
		protected int  scanBufHap;
		protected string scanBuf;
		protected char cBlockCode;	// 블럭을 구분하는 단위 default ,

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
		}

		//20250723 PSU 추가
        public void Clear()
        {
            scanBuf = null;
            scanBufPos = 0;
            scanBufHap = 0;
            _builder = null;
        }


        // ---- Builder methods (Add* / Get) : inverse of Get* parsing methods ----
        private StringBuilder _builder;

        public void AddString(string val)
        {
            if (_builder == null) _builder = new StringBuilder();
            if (_builder.Length > 0) _builder.Append(cBlockCode);
            if (val != null && CommaTextWriter.IsExistBlockCode(val))
                _builder.Append(CommaTextWriter.MakeString(val));
            else
                _builder.Append(val ?? "");
        }

        public void AddInt(int val)
        {
            if (_builder == null) _builder = new StringBuilder();
            if (_builder.Length > 0) _builder.Append(cBlockCode);
            _builder.Append(val);
        }

        public void AddDouble(double val)
        {
            if (_builder == null) _builder = new StringBuilder();
            if (_builder.Length > 0) _builder.Append(cBlockCode);
            _builder.Append(val.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
        public string Get()
        {
            return _builder != null ? _builder.ToString() : "";
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

        public string GetString()
        {
            string s = "";
            GetString(ref s);
            return s;
        }

		public virtual void GetString(ref string s)
		{
            // s+=는 속도가 늦어서 StringBuilder 로 바꾸었다. 2010.9.3
            StringBuilder sb = new StringBuilder();

            while (true)
            {
                if (scanBufPos >= scanBufHap)
                {
                    s = sb.ToString();
                    return;
                }

                if (scanBuf[scanBufPos] == cBlockCode || scanBuf[scanBufPos] == '\n' || scanBuf[scanBufPos] == '\r')
                {
                    scanBufPos++;
                    s = sb.ToString();
                    return;
                }

                if (sb.Length == 0 && (scanBuf[scanBufPos] == ' ' || scanBuf[scanBufPos] == '\t' ||
                        scanBuf[scanBufPos] == '\n' || scanBuf[scanBufPos] == '\r'))
                {

                }
                else
                {
                    sb.Append(scanBuf[scanBufPos]);
                }

                scanBufPos++;
            }
            /*
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
			}*/
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

			val = ConvertTool.ToInt32(buf);
		}

		public void GetInt(ref short val)
		{
			string buf = "";
			GetString(ref buf);

			val = ConvertTool.ToInt16(buf);
		}

        public int GetInt()
        {
            string buf = "";
            GetString(ref buf);

            return ConvertTool.ToInt32(buf);
        }

		public void GetChar(ref sbyte val)
		{
			string buf = "";
			GetString(ref buf);

			val = ConvertTool.ToSByte(buf);
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

			val = ConvertTool.ToSingle(CultureTool.ciKR, buf);  // 기본형식으로 읽는다.
		}

        public float GetFloat()
        {
            string buf = "";
            GetString(ref buf);

            return ConvertTool.ToSingle(CultureTool.ciKR, buf); // 기본형식으로 읽는다.
        }

		public void GetDouble(ref double val)
		{
			string buf = "";
			GetString(ref buf);

            val = ConvertTool.ToDouble(CultureTool.ciKR, buf);  // 기본형식으로 읽는다.
		}

        public double GetDouble()
        {
            string buf = "";
            GetString(ref buf);

            return ConvertTool.ToDouble(CultureTool.ciKR, buf); // 기본형식으로 읽는다.
        }

		public void GetDWORD(ref uint val)
		{
			string buf = "";
			GetString(ref buf);

			val = ConvertTool.ToUInt32(buf);
		}

		public void GetDWORD(ref ulong val)
		{
			string buf = "";
			GetString(ref buf);

			val = ConvertTool.ToUInt64(buf);
		}

		public void GetBYTE(ref byte val)
		{
			string buf = "";
			GetString(ref buf);

			val = ConvertTool.ToByte(buf);
		}

		public void GetLong(ref long val)
		{
			string buf = "";
			GetString(ref buf);

			val = ConvertTool.ToInt64(buf);
		}

        public long GetLong()
        {
            string buf = "";
            GetString(ref buf);

            return ConvertTool.ToInt64(buf);
        }

		// C++의 RGB(r,g,b) 를 이용해서 만든 int 이다.(0xBBGGRR 구조이다.)
		// C#의  ToArgb(r,g,b)로 만든 int를 읽으면 안된다. (0xAARRGGBB 이므로)
		public void GetColor(ref System.Drawing.Color val)
		{
			int color=0;
			GetInt(ref color);
			int bc = (int)((color >> 16) & 0xFF);
			int gc = (int)((color >> 8) & 0xFF);
			int rc = (int)((color >> 0) & 0xFF);
			val = System.Drawing.Color.FromArgb(rc, gc, bc);
		}

		public void GetColorFromARGB(ref System.Drawing.Color val)
		{
			int a=0,r=0,g=0,b=0;
			GetInt(ref a);
			GetInt(ref r);
			GetInt(ref g);
			GetInt(ref b);
			if(a < 0)	a = 0;
			if(r < 0)	r = 0;
			if(g < 0)	g = 0;
			if(b < 0)	b = 0;
			if(a > 255)	a = 255;
			if(r > 255)	r = 255;
			if(g > 255)	g = 255;
			if(b > 255)	b = 255;
			val = System.Drawing.Color.FromArgb(a, r, g, b);
		}

		public void GetWORD(ref ushort val)
		{
			string buf = "";
			GetString(ref buf);

			val = ConvertTool.ToUInt16(buf);
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

        public bool GetBool()
        {
            string buf = "";
            GetString(ref buf);
            if (String.Compare(buf, "true", true) == 0)
                return true;
            else
                return false;
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
				else 
				{
					continue;
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

		public void GetHexBYTE(ref byte val)
		{
			uint retn = 0;

			GetHexDWORD(ref retn);

			val = (byte)retn;
		}

        public byte[] GetHexArray()
        {
            string buf = "";
            GetString(ref buf);

            int size = buf.Length/2;
            byte high, low;

            byte[] array = new byte[size];

            for (int i = 0, pos=0; i < size; i++, pos+=2)
            {
                if (buf[pos] >= 'A' && buf[pos] <= 'F')
                {
                    high = (byte)(buf[pos] - 'A' + 10);
                }
                else if (buf[pos] >= 'a' && buf[pos] <= 'f')
                {
                    high = (byte)(buf[pos] - 'a' + 10);
                }
                else if (buf[pos] >= '0' && buf[pos] <= '9')
                {
                    high = (byte)(buf[pos] - '0');
                }
                else
                {
                    high = 0;
                }

                if (buf[pos+1] >= 'A' && buf[pos+1] <= 'F')
                {
                    low = (byte)(buf[pos + 1] - 'A' + 10);
                }
                else if (buf[pos + 1] >= 'a' && buf[pos + 1] <= 'f')
                {
                    low = (byte)(buf[pos + 1] - 'a' + 10);
                }
                else if (buf[pos + 1] >= '0' && buf[pos + 1] <= '9')
                {
                    low = (byte)(buf[pos + 1] - '0');
                }
                else
                {
                    low = 0;
                }

                array[i] = (byte)((high << 4) | low);
            }

            return array;
        }

		public DateTime GetDateTime()
		{
			string buf = "";
			int year=1, mon=1, day=1, hour=0, min=0, sec=0, milli=0;

			GetString(ref buf);

			CommaBlockString comma = new CommaBlockString();

			comma.Set(buf);
			comma.SetBlockCode('-');
			comma.GetInt(ref year);
			comma.GetInt(ref mon);
			comma.SetBlockCode(' ');
			comma.GetInt(ref day);
			comma.SetBlockCode(':');
			string shour = "";
			comma.GetString(ref shour);

			if(String.Compare(shour, 0, "오전", 0, 2) == 0) 
			{
				hour = ConvertTool.ToInt32(shour.Substring(2));
				hour %= 12;
			}
			else if(String.Compare(shour, 0, "오후", 0, 2) == 0) 
			{
				hour = ConvertTool.ToInt32(shour.Substring(2));	
				hour %= 12;
				hour += 12;
			}
			else 
			{
				hour = ConvertTool.ToInt32(shour);
			}
            			
			comma.GetInt(ref min);

			comma.SetBlockCode('.');
			comma.GetInt(ref sec);
			comma.GetInt(ref milli);

			if(year < 1)	year = 1;
			if(mon < 1)	mon = 1;
			if(day < 1)	day = 1;

            DateTime t;
            try
            {
                t = new DateTime(year, mon, day, hour, min, sec, milli);
            }
            catch
            {
                t = new DateTime();
            }

            return t;
		}


        // Time만 있는 문자열을 읽어온다.
        public DateTime GetTime(DateTime org)
        {
            string buf = "";
            int year = org.Year, mon = org.Month, day = org.Day, hour = 0, min = 0, sec = 0, milli = 0;

            GetString(ref buf);

            CommaBlockString comma = new CommaBlockString();

            comma.Set(buf);
            
            string shour = "";
            comma.GetString(ref shour);

            if (String.Compare(shour, 0, "오전", 0, 2) == 0)
            {
                hour = ConvertTool.ToInt32(shour.Substring(2));
                hour %= 12;
            }
            else if (String.Compare(shour, 0, "오후", 0, 2) == 0)
            {
                hour = ConvertTool.ToInt32(shour.Substring(2));
                hour %= 12;
                hour += 12;
            }
            else
            {
                hour = ConvertTool.ToInt32(shour);
            }

            comma.GetInt(ref min);

            comma.SetBlockCode('.');
            comma.GetInt(ref sec);
            comma.GetInt(ref milli);

            if (year < 1) year = 1;
            if (mon < 1) mon = 1;
            if (day < 1) day = 1;

            DateTime t;
            try
            {
                t = new DateTime(year, mon, day, hour, min, sec, milli);
            }
            catch
            {
                t = new DateTime();
            }

            return t;
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