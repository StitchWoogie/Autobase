using System;
using System.Text;
using NetTools;

namespace AutoLibLocal
{
	public enum EnumNetworkCommand : ushort
	{
		
		
		/*
		ACK = 0x0006,
		NAK = 0x0015,

		NEXT_BLOCK = 0x0101,
		
		SetTagValue = 0x0103,

		TagValueList_Reply = 0x8102,
		*/
		LIFE_SIGNAL_SERVER	= 0x0001,
		CHANGE_MAIN			= 0x0002,
		LIFE_SIGNAL_CLIENT = 0x0003,
		ACK					= 0x0004,
		DUPLEX_HAND			= 0x0005,
		DUPLEX_AUTO			= 0x0006,
		SERVER_REGISTER		= 0x0007,	// NetServer->NetServer/NetClient
		SERVER_UNREGISTER	= 0x0008,	// NetServer->NetServer/NetClient
		TIME_SYNC			= 0x0009,	// NetServer->All Network

		CLIENT_GOODBY		= 0x0010,	// NetClient->All Network

		PC_DUAL_LIFE_SIGNAL			= 0x0050,
		PC_DUAL_DISCONNECT_SIGNAL	= 0x0051,
		PC_DUAL_I_AM_ACTIVATED		= 0x0052,		// 이 신호를 받으면 비활성화 되어야 한다.

		SMS_DATA			= 0x0100,
		SMS_CALL_ERROR		= 0x0101,

		PLCSCAN_WRITE_BIT				= 0x8100,	// PLC_SCAN->NetClient->NetServer->PLC_SCAN
		PLCSCAN_WRITE_WORD				= 0x8101,	// PLC_SCAN->NetClient->NetServer->PLC_SCAN	
		PLCSCAN_PORT_LIFE_SIGNAL		= 0x0102,	// PLC_SCAN->NetClient->NetServer
		PLCSCAN_PORT_DISCONNECT_SIGNAL	= 0x0103,	// PLC_SCAN->NetClient->NetServer
		PLCSCAN_VALUE_WORD				= 0x8104,	// NetServer->NetClient	NEED ACK
		PLCSCAN_VALUE_DWORD				= 0x8105,	// NetServer->NetClient	NEED ACK
		PLCSCAN_VALUE_FLOAT				= 0x8106,	// NetServer->NetClient	NEED ACK
		//#define	COMMAND_PLCSCAN_READ_ONE				0x0107	// Plc_scan->Exe comm
		PLCSCAN_VALUE_WORD_BLOCK		= 0x8108,	// NetServer->NetClient	NEED ACK
		PLCSCAN_VALUE_DWORD_BLOCK		= 0x8109,	// NetServer->NetClient	NEED ACK
		PLCSCAN_VALUE_FLOAT_BLOCK		= 0x810A,	// NetServer->NetClient	NEED ACK
		PLCSCAN_VALUE_SYSTEM			= 0x810B,	// NetServer->NetClient	NEED ACK
		PLCSCAN_PORT_LIFE_SIGNAL_MULTI	= 0x010C,	// 
		PLCSCAN_VALUE_STRING			= 0x810D,	// NetServer->NetClient	NEED ACK
		PLCSCAN_NEED_ALL_DATA			= 0x010E,	// Client에서 시작 시에 한번 발생

		TAG_PROTECT_FLAG_CHANGE			= 0x8200,
		ALARM_LIST_CONFIRM_ONE			= 0x8201,	//
		ALARM_LIST_DELETE_ONE			= 0x8202,	//
		TAG_VALUE_CHANGE				= 0x8204,
		TAG_MEMBER_CHANGED				= 0x8205,

	}

	/// <summary>
	/// Summary description for NetworkProtocol.
	/// </summary>
	/// 
	public class NetWorkProtocolSend
	{
		public NetWorkProtocolSend()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static ushort nNewTnsNo = 1;
		public static ushort GetTransaction()
		{
			nNewTnsNo++;
			return nNewTnsNo;
		}

		public byte[] bufSend;
		public int  nBufCount;

		public void MakeBlock(EnumNetworkCommand command, ushort trans, string buf)
		{
			int size = buf.Length;

			ushort crc;
			string make = "";

			make += (char)EnumAsciiCode.STX;
			make += String.Format("{0:X04}", (ushort)command);
			make += String.Format("{0:X04}", trans);
			make += String.Format("{0:X04}", size);
			make += buf;

			crc = GetCRC.SumWORD(make, 1, make.Length-1);

			make += String.Format("{0:X04}", crc);
			make += (char)EnumAsciiCode.ETX;

			Encoding ASCII = Encoding.UTF8;

			bufSend = new byte[make.Length*3+1];
			nBufCount = ASCII.GetBytes(make.ToCharArray(), 0, make.Length, bufSend, 0);
		}

		/*
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
		EnumNetworkCommand ready_command;
		int block_total;
		int block_curr;

		public void ReadyBuf(EnumNetworkCommand command, string block)
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
				val = (ushort)EnumNetworkCommand.NEXT_BLOCK;
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
			ReadyBuf(EnumNetworkCommand.ACK, "");	
			bool next;
			return GetBlock(tns, out next, out len);
		}
		*/

	}
}
