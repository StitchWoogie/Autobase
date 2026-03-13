using System;

namespace NetTools
{
	/// <summary>
	/// Summary description for HexBuf.
	/// </summary>
	public class HexBuf
	{
		public HexBuf()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static byte ToByte(byte[] b, int pos)
		{
			byte val;
			byte retn = 0;

			for(int i = 0; i < 2; i++) 
			{
				retn <<= 4;
				if(b[pos+i] >= (byte)'0' && b[pos+i] <= (byte)'9')	
					val = (byte)(b[pos+i]-'0');
				else if(b[pos+i] >= (byte)'A' && b[pos+i] <= (byte)'F')	
					val = (byte)(b[pos+i]-'A'+10);
				else 
					val = 0;
				retn |= val;
			}

			return retn;
		}

		public static byte ToByte(string b, int pos)
		{
			byte val;
			byte retn = 0;

			for(int i = 0; i < 2; i++) 
			{
				retn <<= 4;
				if(b[pos+i] >= (byte)'0' && b[pos+i] <= (byte)'9')	
					val = (byte)(b[pos+i]-'0');
				else if(b[pos+i] >= (byte)'A' && b[pos+i] <= (byte)'F')	
					val = (byte)(b[pos+i]-'A'+10);
				else 
					val = 0;
				retn |= val;
			}

			return retn;
		}

		public static ushort ToUshort(byte[] b, int pos)
		{
			ushort val;
			ushort retn = 0;

			for(int i = 0; i < 4; i++) 
			{
				retn <<= 4;
				if(b[pos+i] >= (byte)'0' && b[pos+i] <= (byte)'9')	
					val = (byte)(b[pos+i]-'0');
				else if(b[pos+i] >= (byte)'A' && b[pos+i] <= (byte)'F')	
					val = (byte)(b[pos+i]-'A'+10);
				else 
					val = 0;
				retn |= val;
			}

			return retn;
		}

		public static ushort ToWORD(string b, int pos)
		{
			ushort val;
			ushort retn = 0;

			for(int i = 0; i < 4; i++) 
			{
				retn <<= 4;
				if(b[pos+i] >= '0' && b[pos+i] <= '9')	
					val = (ushort)(b[pos+i]-'0');
				else if(b[pos+i] >= 'A' && b[pos+i] <= 'F')	
					val = (ushort)(b[pos+i]-'A'+10);
				else 
					val = 0;
				retn |= val;
			}

			return retn;
		}

		public static uint ToUint(byte[] b, int pos)
		{
			uint val;
			uint retn = 0;

			for(int i = 0; i < 8; i++) 
			{
				retn <<= 4;
				if(b[pos+i] >= (byte)'0' && b[pos+i] <= (byte)'9')	
					val = (byte)(b[pos+i]-'0');
				else if(b[pos+i] >= (byte)'A' && b[pos+i] <= (byte)'F')	
					val = (byte)(b[pos+i]-'A'+10);
				else 
					val = 0;
				retn |= val;
			}

			return retn;
		}

		public static uint ToUint(string s)
		{
			uint val;
			uint retn = 0;

			for(int i = 0; i < s.Length && i < 8; i++) 
			{
				retn <<= 4;
				if(s[i] >= '0' && s[i] <= '9')	
					val = (byte)(s[i]-'0');
				else if(s[i] >= (byte)'A' && s[i] <= (byte)'F')	
					val = (byte)(s[i]-'A'+10);
				else 
					val = 0;
				retn |= val;
			}

			return retn;
		}

		public static uint ToDWORD(string s)
		{
			return ToUint(s);
		}
	}
}
