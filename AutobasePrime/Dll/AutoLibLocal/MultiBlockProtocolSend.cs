using System;
using System.Text;

namespace AutoLibLocal
{
	public enum EnumMultiBlockCommand
	{
		ACK = 0x0006,
		NAK = 0x0015,

		NEXT_BLOCK   = 0x0101,
		TagValueList = 0x0102,
		SetTagValue  = 0x0103,

		TagValueList_Reply = 0x8102,
	}

	/// <summary>
	/// Summary description for NetworkProtocol.
	/// </summary>
	/// 
	public class MultiBlockProtocolSend
	{
		public MultiBlockProtocolSend()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static ushort nNewTnsNo = 0;

		public static ushort MakeTns()
		{
			nNewTnsNo++;
			return nNewTnsNo;
		}

		public static ushort CrcSum(byte[] buf, int len)
		{
			ushort crc = 0;

			for(int i = 0; i < len; i++) 
			{
				crc += buf[i];
			}

			return crc;
		}

		readonly int MAX_BUF = 4000;
		byte[] ready_buffer;
		int ready_count;
		EnumMultiBlockCommand ready_command;
		int block_total;
		int block_curr;

		public void ReadyBuf(EnumMultiBlockCommand command, string block)
		{
			if(block.Length == 0) 
			{
				ready_buffer = null;
				ready_command = command;
				ready_count = 0;
				block_total = 1;
				block_curr = 0;
			}
			else 
			{
				Encoding ASCII = Encoding.UTF8;

				ready_buffer = new byte[block.Length*3+1];

				ready_command = command;
				ready_count = ASCII.GetBytes(block.ToCharArray(), 0, block.Length, ready_buffer, 0);
				block_total = (ready_count+(MAX_BUF-1))/MAX_BUF;
				block_curr = 0;
			}
		}

		public byte[] GetBlock(ushort tns, out bool next, out int len)
		{
			byte[] make = new byte[10+MAX_BUF];

			len = 0;
			
			ushort val;
			if(block_curr == 0)
				val = (ushort)ready_command;
			else
				val = (ushort)EnumMultiBlockCommand.NEXT_BLOCK;
			make[len++] = (byte)(val%256);
			make[len++] = (byte)(val/256);

			val = (ushort)(block_total-(block_curr+1));
			make[len++] = (byte)(val%256);
			make[len++] = (byte)(val/256);

			val = tns;
			make[len++] = (byte)(val%256);
			make[len++] = (byte)(val/256);

			//val = (ushort)();
			//make[len++] = (byte)(val%256);
			//make[len++] = (byte)(val/256);
			len+=2;

			int pos = block_curr*MAX_BUF;
			for(int i = 0; i < MAX_BUF && pos < ready_count; i++, len++, pos++) 
			{
				make[len] = ready_buffer[pos];
			}

			val = (ushort)(len-8);
			make[6] = (byte)(val%256);
			make[7] = (byte)(val/256);

			val = CrcSum(make, len);
			make[len++] = (byte)(val%256);
			make[len++] = (byte)(val/256);

			block_curr++;

			if(block_curr == block_total)	next = false;
			else							next = true;
            
			return make;
		}

		public byte[] MakeACK(ushort tns, out int len)
		{
			ReadyBuf(EnumMultiBlockCommand.ACK, "");	
			bool next;
			return GetBlock(tns, out next, out len);
		}
	}
}
