using System;
using System.Text;
using NetTools.OldDefine;
using NetTools;

namespace AutoLibLocal
{
	/// <summary>
	/// Summary description for NetworkProtocol.
	/// </summary>
	/// 

	public class NetWorkProtocolRecv
	{
		public ushort wCommand;
		public ushort wTransaction;
		public int  nNodeType;			// NodeType
		public string sNodeName;		// NodeName

		public int  nPort;					// plc scan port
		public int  nBroadCastPort;		// plc scan 포트에서 제공받고자 하는 Port
		public ushort[] wBroadCastPorts = new ushort[16];	// plc scan 포트에서 제공받고자 하는 Port
		public int  nStation;			// plc scan station
		public uint dwAddress;		// plc scan address
		public string sExtra1;		// Plc Scan Extra1
		public ushort wExtra2;			// Plc Scan Extra2
		public double fValue;		// value
		public string sTag;			// tag name
		public ushort wTagMember;		// TagMember No
		public short nBlockSize;		// count
		public byte[] Block;			// block count = nCount를 사용한다.
		public string sString;			// "String" 값.
		public SYSTEMTIME tDateTime = new SYSTEMTIME();	// SYSTEM TIME
		public sbyte bServerActive;		// 해당서버가 활성화가 되어있는가를 검사
		public sbyte bPlcScanTimeOut;
		public sbyte bAnotherProgramExit;
		public sbyte bAutoWatch;		// 자동 감시 상태

		public NetWorkProtocolRecv()
		{
			//
			// TODO: Add constructor logic here
			//

			int i;
	
			nNodeType = 0;
			sNodeName = "???";

			nPort = 0;			// plc scan port
			nBroadCastPort = 0;	// plc scan 포트에서 제공받고자 하는 Port
			nStation = 0;		// plc scan station
			dwAddress = 0;		// plc scan address
			sExtra1 = "";		// Plc Scan Extra1
			wExtra2 = 0;		// Plc Scan Extra2
			fValue = 0;			// value

			//	smBufType = 0;
			//	smDeviceType[0] = 0;
			//	smTarget = 0;
			//	smSize = 1;
			sTag = "";
			wTagMember = 0;
			nBlockSize = 0;
			Block = null;

			sString = "";

			//memset(&tDateTime, 0, sizeof(SYSTEMTIME));

			wTransaction = 0;

			bServerActive = 1;
			bPlcScanTimeOut = 0;
			bAnotherProgramExit = 0;

			for(i = 0; i < 16; i++) 
			{
				wBroadCastPorts[i] = 0;
			}
		}

		bool Split(string recvBuf, int nRecvCount)
		{
			if(nRecvCount < 10)                                     return false;
			if(recvBuf[0] != (char)EnumAsciiCode.STX)				return false;
			if(recvBuf[nRecvCount-1] != (char)EnumAsciiCode.ETX)	return false;

			ushort crc = GetCRC.SumWORD(recvBuf, 1, nRecvCount-6);
			if(crc != HexBuf.ToWORD(recvBuf, nRecvCount-5))	return false;
			wCommand = HexBuf.ToWORD(recvBuf, 1);

			wTransaction = HexBuf.ToWORD(recvBuf, 5);
	
			//FreeBuf();
			int data_size = HexBuf.ToWORD(recvBuf, 9);

			if(data_size > 0) 
			{
				string buf;
				buf = recvBuf.Substring(13, data_size);

				CommaBlockString comma = new CommaBlockString();
				comma.Set(buf);
				while(true) 
				{
					comma.GetString(ref buf);
					if(buf.Length == 0)	break;

					if(String.Compare(buf, 0, "NodeType=", 0, 9, true) == 0) 
					{
						nNodeType = ConvertTool.ToInt32(buf.Substring(9));
					}
					else if(String.Compare(buf, 0, "NodeName=", 0, 9, true) == 0) 
					{
						sNodeName = buf.Substring(9);
					}
					else if(String.Compare(buf, 0, "Port=", 0, 5, true) == 0) 
					{
						nPort = ConvertTool.ToInt32(buf.Substring(5));
					}
					else if(String.Compare(buf, 0, "BroadCastPort=", 0, 14, true) == 0) 
					{
						nBroadCastPort = ConvertTool.ToInt32(buf.Substring(14));
					}
					else if(String.Compare(buf, 0, "BroadCastPorts=", 0, 15, true) == 0) 
					{
						for(int i = 0; i < 16; i++) 
						{
							wBroadCastPorts[i] = HexBuf.ToWORD(buf, 15+i*4);
						}
					}
					else if(String.Compare(buf, 0, "Station=", 0, 8, true) == 0) 
					{
						nStation = ConvertTool.ToInt32(buf.Substring(8));
					}
					else if(String.Compare(buf, 0, "Address=", 0, 8, true) == 0) 
					{
						dwAddress = (uint)ConvertTool.ToInt32(buf.Substring(8));
					}
					else if(String.Compare(buf, 0, "Extra1=", 0, 7, true) == 0) 
					{
						sExtra1 = buf.Substring(7);
					}
					else if(String.Compare(buf, 0, "Extra2=", 0, 7, true) == 0) 
					{
						wExtra2 = (ushort)ConvertTool.ToInt32(buf.Substring(7));
					}
					else if(String.Compare(buf, 0, "Value=", 0, 6, true) == 0) 
					{
						fValue = ConvertTool.ToDouble(buf.Substring(6));
					}

					else if(String.Compare(buf, 0, "Tag=", 0, 4, true) == 0) 
					{
						sTag = buf.Substring(4);
					}
					else if(String.Compare(buf, 0, "TagMember=", 0, 10, true) == 0) 
					{
						wTagMember = (ushort)ConvertTool.ToInt32(buf.Substring(10));
					}
					else if(String.Compare(buf, 0, "Year=", 0, 5, true) == 0) 
					{
						tDateTime.wYear = (ushort)ConvertTool.ToInt32(buf.Substring(5));
					}
					else if(String.Compare(buf, 0, "Mon=", 0, 4, true) == 0) 
					{
						tDateTime.wMonth = (ushort)ConvertTool.ToInt32(buf.Substring(4));
					}
					else if(String.Compare(buf, 0, "Day=", 0, 4, true) == 0) 
					{
						tDateTime.wDay = (ushort)ConvertTool.ToInt32(buf.Substring(4));
					}
					else if(String.Compare(buf, 0, "Hour=", 0, 5, true) == 0) 
					{
						tDateTime.wHour = (ushort)ConvertTool.ToInt32(buf.Substring(5));
					}
					else if(String.Compare(buf, 0, "Min=", 0, 4, true) == 0) 
					{
						tDateTime.wMinute = (ushort)ConvertTool.ToInt32(buf.Substring(4));
					}
					else if(String.Compare(buf, 0, "Sec=", 0, 4, true) == 0) 
					{
						tDateTime.wSecond = (ushort)ConvertTool.ToInt32(buf.Substring(4));
					}
					else if(String.Compare(buf, 0, "BlockSize=", 0, 10, true) == 0) 
					{
						nBlockSize = (short)ConvertTool.ToInt32(buf.Substring(10));
					}
					else if(String.Compare(buf, 0, "Block=", 0, 6, true) == 0) 
					{
						if(Block != null) 
						{
							Block = null;
						}
						if(nBlockSize > 0 && nBlockSize*2 == (short)buf.Substring(6).Length) 
						{
							Block = new byte[nBlockSize];
							for(int i = 0; i < nBlockSize; i++) 
							{
								Block[i] = HexBuf.ToByte(buf, 6+i*2);
							}
						}
						else if(nBlockSize > 0 && nBlockSize*2 < (short)buf.Substring(6).Length) 
						{
							Block = new byte[nBlockSize];
							for(int i = 0; i < nBlockSize; i++) 
							{
								Block[i] = HexBuf.ToByte(buf, 6+i*2);
							}
						}
						else if(nBlockSize == 0) 
						{
							Block = new byte[1];
							Block[0] = 0;
						}
						else 
						{
							//string msg;
							//msg = String.Format("nBlockSize and Block count is mismatched");
							//Block = new byte[msg.Length+1];
							//strcpy((char*)Block, msg);
							//nBlockSize = (short)strlen(msg);
						}
					}
					else if(String.Compare(buf, 0, "String=", 0, 7, true) == 0) 
					{
						sString = buf.Substring(7);
					}
					else if(String.Compare(buf, 0, "ServerActive=", 0, 13, true) == 0) 
					{
						bServerActive = (sbyte)ConvertTool.ToInt32(buf.Substring(13));
					}
					else if(String.Compare(buf, 0, "PSTO=", 0, 5, true) == 0) 
					{
						bPlcScanTimeOut = (sbyte)ConvertTool.ToInt32(buf.Substring(5));
					}
					else if(String.Compare(buf, 0, "APE=", 0, 4, true) == 0) 
					{
						bAnotherProgramExit = (sbyte)ConvertTool.ToInt32(buf.Substring(4));
					}
					else if(String.Compare(buf, 0, "AW=", 0, 3, true) == 0) 
					{
						bAutoWatch = (sbyte)ConvertTool.ToInt32(buf.Substring(3));
					}
					else {}
				}
			}
		
			return true;
		}

		public bool Split(byte[] recvBuf, int index, int nRecvCount)
		{
			Encoding ASCII = Encoding.UTF8;
			string buf = ASCII.GetString(recvBuf, index, nRecvCount);

			return Split(buf, buf.Length);
		}

		public bool Split(byte[] recvBuf, int nRecvCount)
		{
			return Split(recvBuf, 0, nRecvCount); 
		}
		
		/*
		public NetworkProtocolRecv()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		
		public EnumNetworkCommand nCommand;
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

			ushort crc_make = NetworkProtocolSend.CrcSum(buf, buf.Length-2);
			ushort crc = (ushort)(buf[buf.Length-2]+buf[buf.Length-1]*256);

			if(crc != crc_make) 
			{
				nBlockRemain = 0;
				nBufPos = 0;
				return false;
			}

			int block_size;

			nCommand = (EnumNetworkCommand)(buf[0]+buf[1]*256);
			if(nCommand == EnumNetworkCommand.NEXT_BLOCK) 
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
		*/
	}
}

