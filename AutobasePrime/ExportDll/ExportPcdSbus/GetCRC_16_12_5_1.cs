using System;

namespace NetTools
{
	/// <summary>
	/// Summary description for GetCRC_16_12_5_1.
	/// </summary>
	public class GetCRC_16_12_5_1
	{
		public GetCRC_16_12_5_1()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static ushort[] CrcTable = new ushort[256];	/* Look-up table */
		static byte zero;
		static bool bInit;		/* TRUE=CrcTable has been initialized */

		/* Compute CRC using standard method (for look-up table only) */

		static ushort _CrcCalc(ushort crc, ref byte bp)
		{
			int i;

			// crc ^= (ushort)((byte)(bp++) << 8);	의미가 없음
			
			for (i = 0; i < 8; ++i) 
			{
				if ((crc & 0x8000) > 0)
					crc = Win32Function.LOWORD((uint)((crc << 1) ^ 0x1021));
				else
					crc <<= 1;
			}

			return(crc);
		}


		/* Initialize the look-up table */

		static void CrcInit()
		{
			int       i;

			bInit = true;
			zero = 0;
			for (i = 0; i < 256; ++i)
				CrcTable[i] = _CrcCalc(Win32Function.LOWORD((uint)(i << 8)), ref zero);
		}


		/* Compute CRC using the look-up table */

		public static ushort Calc(byte[] bp, int start, int len)
		{
			ushort crc = 0;
			int pos = start;

			if (!bInit)
				CrcInit();
			while(len > 0) 
			{
				len--;
				crc = Win32Function.LOWORD((uint)(CrcTable[(crc >> 8) ^ (bp[pos++])] ^ (crc << 8)));
			}
			return(crc);
		}

	}
}
