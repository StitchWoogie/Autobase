using System;
using System.Text;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for NetworkProtocol.
	/// </summary>
	/// 

	public class MultiBlockProtocolRecv
	{
		public MultiBlockProtocolRecv()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public EnumMultiBlockCommand nCommand;
		public ushort nTns;
		int nBlockRemain;
		byte[] ready;
		int nBufPos = 0;

		public bool SetBlock(byte[] buf, int count, out bool next)
		{
			next = false;
			if(buf.Length < 10) 
			{
				nBlockRemain = 0;
				nBufPos = 0;
				return false;
			}

			ushort crc_make = MultiBlockProtocolSend.CrcSum(buf, buf.Length-2);
			ushort crc = (ushort)(buf[buf.Length-2]+buf[buf.Length-1]*256);

			if(crc != crc_make) 
			{
				nBlockRemain = 0;
				nBufPos = 0;
				return false;
			}

			int block_size;

			nCommand = (EnumMultiBlockCommand)(buf[0]+buf[1]*256);
			if(nCommand == EnumMultiBlockCommand.NEXT_BLOCK) 
			{
				nBlockRemain = buf[2]+buf[3]*256;
				nTns = (ushort)(buf[4]+buf[5]*256);
				block_size = buf[6]+buf[7]*256;
			}
			else 
			{
				nBlockRemain = buf[2]+buf[3]*256;
				nTns = (ushort)(buf[4]+buf[5]*256);
				block_size = buf[6]+buf[7]*256;

				if(nBlockRemain == 0) 
					ready = new byte[block_size];
				else
					ready = new byte[block_size*(nBlockRemain+1)];

				nBufPos = 0;
			}

			for(int i = 0; i < block_size; i++, nBufPos++) 
			{
				ready[nBufPos] = buf[8+i];
			}
			next = (nBlockRemain > 0);
			return true;
		}

		public string Split()
		{
			Encoding ASCII = Encoding.UTF8;

			string buf = ASCII.GetString(ready, 0, nBufPos);

			return buf;
		}
	}
}
