using System;
using NetTools;
using AutoLibLocal;
using System.Net.Sockets;
using System.Net;
using NetCommon;
using System.Collections;

namespace NetClient
{
	/// <summary>
	/// Summary description for NetServerStatus.
	/// </summary>
	public class NetClientStatus
	{
		public NetClientStatus()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static void Status()
		{
			ushort recv_command = 0;
			ushort recv_trans = 0;
			string recv_ip = "";

			NetCheckRecvCommand(NetLib.socketNetwork1, ref recv_command, ref recv_trans, ref recv_ip);
			if(NetLib.headerNode.bUseDual == 1) 
			{
				NetCheckRecvCommand(NetLib.socketNetwork2, ref recv_command, ref recv_trans, ref recv_ip);
			}

			//CheckPlcScanEvent();
			FormViewServer.CheckServerStatus();
			ClassNetworkEvent.Check();
			FormViewNodeList.CheckNetworkList();

			LifeSignalSendCheck();	

			
		}

		static TimeOutClass timeoutLifeSend = new TimeOutClass();

		static void LifeSignalSendCheck()
		{
			if(timeoutLifeSend.IsTimeOut(4)) 
			{
				timeoutLifeSend.Reset();
				SendCommandLifeSignalClient();
			}
		}

		static void SendCommandLifeSignalClient()
		{
			SendBlockToBroadcast(EnumNetworkCommand.LIFE_SIGNAL_CLIENT, "");
		}

		static void SendBlockToBroadcast(EnumNetworkCommand command, string buf)
		{
			NetWorkProtocolSend send = new NetWorkProtocolSend();

			ushort trans = NetWorkProtocolSend.GetTransaction();

			send.MakeBlock(command, trans, buf);

			NetLib.SendCodeToBroadcast(send.bufSend, send.nBufCount);
		}

		static void SendBlockToAllServer(EnumNetworkCommand command, string buf)
		{
			NetWorkProtocolSend send = new NetWorkProtocolSend();

			ushort trans = NetWorkProtocolSend.GetTransaction();

			send.MakeBlock(command, trans, buf);

			NetLib.SendCodeToAllServer(command, trans, send.bufSend, send.nBufCount);
		}

		static void SendEventNetworkToViewMain(byte[] data, int size)
		{
            ClassNetworkEvent.smNetworkToViewMain.AddItem(data);
            /*
			using(ComNetServer.ClassServer com = new ComNetServer.ClassServer())
			{
				com.AddToLocalMain(data, size);
			}*/
		}

		public const int MAX_SERVER_LIST = 3;
		public static ArrayList serverList = new ArrayList();

		static bool NetCheckRecvCommandOne(Socket socket, ref ushort recv_command, ref ushort recv_trans, ref string recv_ip, byte[] recv_data, int index, int retn_size, string ip)
		{
			NetWorkProtocolRecv split = new NetWorkProtocolRecv();

			if(split.Split(recv_data, index, retn_size)) 
			{
				if(split.wCommand == (ushort)EnumNetworkCommand.LIFE_SIGNAL_SERVER) 
				{
					if(split.nNodeType >= 0 && split.nNodeType < 3) 
					{
						SERVER_LIST server = (SERVER_LIST)NetClientStatus.serverList[split.nNodeType];
						server.timeout.Reset();

						server.bDualActive = split.bServerActive == 1;
						if(split.nNodeType == 1) 
						{	// primary server
							SystemStatusMemory.SetDI(SSMDI.DuplexActivePrimary, split.bServerActive);
						}
						else if(split.nNodeType == 2) 
						{	// secondary server
							SystemStatusMemory.SetDI(SSMDI.DuplexActiveSecondary, split.bServerActive);
						}
						else {}

						if(server.flag == 0) 
						{
							server.flag = 1;
							FormViewServer.InvalidateServerView();
						}
						if(String.Compare(server.ip, ip) != 0 || String.Compare(server.name, split.sNodeName) != 0) 
						{
							server.ip = ip;
							server.name = split.sNodeName;
							FormViewServer.InvalidateServerView();
						}
					}
					FormViewNodeList.NetworkListLifeSignalUpdate(ip, split.nNodeType, 1);
				}
				else if(split.wCommand == (ushort)EnumNetworkCommand.LIFE_SIGNAL_CLIENT) 
				{
					FormViewNodeList.NetworkListLifeSignalUpdate(ip, 3, 1);	// 3 = client type
				}
				else if(split.wCommand == (ushort)EnumNetworkCommand.CLIENT_GOODBY) 
				{
					FormViewNodeList.NetworkListLifeSignalUpdate(ip, 3, 0);	// 3 = client type
				}
				else if(split.wCommand == (ushort)EnumNetworkCommand.SERVER_UNREGISTER) 
				{
					if(split.nNodeType >= 0 && split.nNodeType < MAX_SERVER_LIST) 
					{
						SERVER_LIST server = (SERVER_LIST)NetClientStatus.serverList[split.nNodeType];
						server.flag = 0;
						server.ip = "???";
						server.name = "???";
						FormViewServer.InvalidateServerView();
					}
				}
				else if(split.wCommand == (ushort)EnumNetworkCommand.TIME_SYNC) 
				{
					NetLib.SendCodeACK(socket, ip, split.wTransaction);
					TimeUtil.SetLocalTime(split.tDateTime.wYear, split.tDateTime.wMonth, split.tDateTime.wDay, split.tDateTime.wHour, split.tDateTime.wMinute, split.tDateTime.wSecond);
				}	

					/*
				else if(split.wCommand == COMMAND_PLCSCAN_VALUE_WORD) 
				{
					SendCodeACK(socket, ip, split.wTransaction);
					PokeValue(socket, &split, ip);
				}
				else if(split.wCommand == COMMAND_PLCSCAN_VALUE_DWORD) 
				{
					SendCodeACK(socket, ip, split.wTransaction);
					PokeValue(socket, &split, ip);
				}
				else if(split.wCommand == COMMAND_PLCSCAN_VALUE_FLOAT) 
				{
					SendCodeACK(socket, ip, split.wTransaction);
					PokeValue(socket, &split, ip);
				}
				else if(split.wCommand == COMMAND_PLCSCAN_VALUE_SYSTEM) 
				{
					SendCodeACK(socket, ip, split.wTransaction);
					PokeValue(socket, &split, ip);
				}
				else if(split.wCommand == COMMAND_PLCSCAN_VALUE_WORD_BLOCK) 
				{
					SendCodeACK(socket, ip, split.wTransaction);
					PokeValueBlockWORD(socket, &split, ip);
				}
				else if(split.wCommand == COMMAND_PLCSCAN_VALUE_DWORD_BLOCK) 
				{
					SendCodeACK(socket, ip, split.wTransaction);
					PokeValueBlockDWORD(socket, &split, ip);
				}
				else if(split.wCommand == COMMAND_PLCSCAN_VALUE_FLOAT_BLOCK) 
				{
					SendCodeACK(socket, ip, split.wTransaction);
					PokeValueBlockFLOAT(socket, &split, ip);
				}
				*/
				
				else if(split.wCommand == (ushort)EnumNetworkCommand.TAG_PROTECT_FLAG_CHANGE) 
				{
					NetLib.SendCodeACK(socket, ip, split.wTransaction);
					SendEventNetworkToViewMain(recv_data, retn_size);
				}
				else if(split.wCommand == (ushort)EnumNetworkCommand.TAG_VALUE_CHANGE) 
				{
					NetLib.SendCodeACK(socket, ip, split.wTransaction);
					SendEventNetworkToViewMain(recv_data, retn_size);
				}
				else if(split.wCommand == (ushort)EnumNetworkCommand.ALARM_LIST_CONFIRM_ONE) 
				{
					NetLib.SendCodeACK(socket, ip, split.wTransaction);
					SendEventNetworkToViewMain(recv_data, retn_size);
				}
				else if(split.wCommand == (ushort)EnumNetworkCommand.ALARM_LIST_DELETE_ONE) 
				{
					NetLib.SendCodeACK(socket, ip, split.wTransaction);
					SendEventNetworkToViewMain(recv_data, retn_size);
				}
				else if(split.wCommand == (ushort)EnumNetworkCommand.TAG_MEMBER_CHANGED) 
				{
					NetLib.SendCodeACK(socket, ip, split.wTransaction);
					SendEventNetworkToViewMain(recv_data, retn_size);
				}
				else {}
			}

			if(recv_command == split.wCommand && recv_trans == split.wTransaction && String.Compare(recv_ip, ip, true) == 0)
				return true;

			return false;
		}

		public static bool NetCheckRecvCommand(Socket socket, ref ushort recv_command, ref ushort recv_trans, ref string recv_ip)
		{
			if(socket == null)	return false;
	
			int remain;

			remain = socket.Available;
			
			if(remain == 0)	return false;
			if(remain <  1)	return false;
			if(remain == 1)	return false;

			string ip = "";
	
			byte[] recv = new byte[remain];
			int retn_size;

			IPEndPoint sender = new IPEndPoint(IPAddress.Any, ConfigNetCommon.nServerPort);
			EndPoint senderRemote = (EndPoint)sender;

			try 
			{
				retn_size = socket.ReceiveFrom(recv, 0, remain, SocketFlags.None, ref senderRemote);
			}
			catch 
			{
				retn_size = 0;
			}

			if(retn_size == 0) 
			{
				return false;
			}

			ip = ((IPEndPoint)senderRemote).Address.ToString();

			NetLib.DisplayRecvCode(ip, recv, retn_size);

			int start_pos = 0;
			bool retn = false;

			for(int i = 0; i < retn_size; i++) 
			{
				if(recv[i] == (byte)EnumAsciiCode.ETX) 
				{
					if(NetCheckRecvCommandOne(socket, ref recv_command, ref recv_trans, ref recv_ip, recv, start_pos, i-start_pos+1, ip)) 
					{
						retn = true;
					}
					start_pos = i+1;
				}
			}

			return retn;
		}

		public static void ClientSignalGoodBy()
		{
			NetWorkProtocolSend send = new NetWorkProtocolSend();
			ushort trans = NetWorkProtocolSend.GetTransaction();

			send.MakeBlock(EnumNetworkCommand.CLIENT_GOODBY, trans, "");
			NetLib.SendCodeToBroadcast(send.bufSend, send.nBufCount);
		}
	}
}
