using System;
using NetTools;
using AutoLibLocal;
using System.Net.Sockets;
using System.Net;
using NetCommon;

namespace NetServer
{
	public class STATUS_STRUCT
	{
		public string sAnotherName;
		public string sAnotherIP;
	}
	/// <summary>
	/// Summary description for NetServerStatus.
	/// </summary>
	public class NetServerStatus
	{
		public NetServerStatus()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		// 통신 두절이 생기면 절환해 준다.
		static TimeOutClass timeoutPlcScan = new TimeOutClass();

		static void CheckPlcScanTimeOut()
		{
			if(IsAutoWatch() == false)	return;	// 자동 절환 상태가 아니다.
			if(configStruct.bChangeConditionPlcScanTimeOut == 0)	return;	// 기능을 사용 안한다.
	
			//static TimeOutClass timeout;

			if(SystemStatusMemory.GetDI(SSMDI.DuplexConnect) == 0)	return;

			if(!IsChangeAbleTime())	return;

			if(SystemStatusMemory.GetDI(SSMDI.DuplexActiveI) == 1 &&
				SystemStatusMemory.GetDI(SSMDI.DuplexActiveYou) == 0 &&
				SystemStatusMemory.GetDI(SSMDI.ErrorStatusPlcScanTimeOut) == 1) 
			{			// 통신 에러일 때 
				if(configStruct.nNodeType == 1 || configStruct.nNodeType == 2) 
				{	// Primary or secondary Server 
					if(SystemStatusMemory.GetDI(SSMDI.AnotherProgramExitingYou) == 0 &&
						SystemStatusMemory.GetDI(SSMDI.ErrorStatusPlcScanTimeOutYou) == 0) 
					{

						if(timeoutPlcScan.IsTimeOut(3)) 
						{
							SendCommandChangeMain(statusStruct.sAnotherName);
							timeoutPlcScan.Reset();
						}
					}
				}
			}
		}

		static bool IsChangeAbleTime()
		{
			if(configStruct.bChangeConditionProtectChangeTime == 0)	return true;

			DateTime t = DateTime.Now;

			if(t.Second >= 15 && t.Second <= 45)	return true;
			return false;
		}

		static TimeOutClass timeoutAnotherProgramExit = new TimeOutClass();

		//------------------------------------------------------------------------------------------
		// 다른 프로그램 감시프로그램, 통신프로그램, RUNMAIN 프로그램 등이 실행중이 아니면 절체한다.
		//------------------------------------------------------------------------------------------

		static void CheckAnotherProgramExit()
		{
            bool bIsExit = false;
            bool bCheckExit = false;

            bool createdNew = false;

            System.Threading.Mutex m;

            if (configStruct.bChangeConditionProgramViewMain == 1)
            {
                bCheckExit = true;

                m = new System.Threading.Mutex(true, "AutoBaseLocalMainMutex", out createdNew);
                if (createdNew)
                {
                    bIsExit = true;
                }

                m.Close();
            }
            if (configStruct.bChangeConditionProgramPlcScan == 1)
            {
                bCheckExit = true;

                if (Win32Function.FindWindow("PlcScanMainFrame", null) == IntPtr.Zero)
                {
                    bIsExit = true;
                }
            }
            if (configStruct.bChangeConditionProgramRunMain == 1)
            {
                bCheckExit = true;

                m = new System.Threading.Mutex(true, "AutoBaseRunMainMutex", out createdNew);
                if (createdNew)
                {
                    bIsExit = true;
                }

                m.Close();
            }

            bAnotherProgrammExit = bIsExit;
            SystemStatusMemory.SetDI(SSMDI.AnotherProgramExitingI, bIsExit ? (sbyte)1 : (sbyte)0);

            if (IsAutoWatch() == false) return;		    // 자동 절환 상태가 아니다.
            if (bCheckExit == false) return;		    // 프로그램 종료조건으로 절체하지 않는다.
            if (bIsExit == false) return;		// 모든 프로그램이 살아 있다.

            if (SystemStatusMemory.GetDI(SSMDI.DuplexConnect) == 0) return;

            if (!IsChangeAbleTime()) return;

            if (SystemStatusMemory.GetDI(SSMDI.DuplexActiveI) == 1 &&
                SystemStatusMemory.GetDI(SSMDI.DuplexActiveYou) == 0)
            {			// 
                if (configStruct.nNodeType == 1 || configStruct.nNodeType == 2)
                {	// Primary or secondary Server 
                    if (SystemStatusMemory.GetDI(SSMDI.AnotherProgramExitingYou) == 0 &&
                        SystemStatusMemory.GetDI(SSMDI.ErrorStatusPlcScanTimeOutYou) == 0)
                    {
                        if (timeoutAnotherProgramExit.IsTimeOut(3))
                        {
                            SendCommandChangeMain(statusStruct.sAnotherName);
                            timeoutAnotherProgramExit.Reset();
                        }
                    }
                }
            }
		}

		public static void Status()
		{
			ushort recv_command = 0;
			ushort recv_trans = 0;
			string recv_ip = "";

			CheckServerStatus();
			CheckPlcScanTimeOut();
			CheckAnotherProgramExit();
			NetCheckRecvCommand(NetLib.socketNetwork1, ref recv_command, ref recv_trans, ref recv_ip);
			if(NetLib.headerNode.bUseDual == 1) 
			{
				NetCheckRecvCommand(NetLib.socketNetwork2, ref recv_command, ref recv_trans, ref recv_ip);
			}
			FormViewNodeList.CheckNetworkList();
		}

		static TimeOutClass timeoutServer = new TimeOutClass();
		static bool bErrorFlag;
		static TimeOutClass timeoutLifeSend = new TimeOutClass();
		static TimeOutClass timeoutLifeRecv = new TimeOutClass();
		public static STATUS_STRUCT statusStruct = new STATUS_STRUCT();

		static void LifeSignalSendCheck()
		{
			if(timeoutLifeSend.IsTimeOut(3)) 
			{
				timeoutLifeSend.Reset();
				SendCommandLifeSignalServer();
			}
		}

		static bool bAnotherProgrammExit = false;	// 프로그램 중 하나라도 종료되어 있으면 ON이 된다.

		public static bool IsAutoWatch()
		{
			if(SystemStatusMemory.GetDI(SSMDI.DuplexAutoWatchI) == 1)	return true;
			else														return false;
		}

		static void SendCommandLifeSignalServer()
		{
			string buf;

			int active = SystemStatusMemory.GetDI(SSMDI.DuplexActiveI);

			buf = String.Format("NodeType={0},NodeName={1},ServerActive={2},PSTO={3},APE={4},AW={5},", configStruct.nNodeType, configStruct.sNodeName, active, SystemStatusMemory.GetDI(SSMDI.ErrorStatusPlcScanTimeOut), bAnotherProgrammExit ? 1 : 0, IsAutoWatch() ? 1 : 0);
	
			SendBlockToBroadcast(EnumNetworkCommand.LIFE_SIGNAL_SERVER, buf);
		}

		static void SendBlockToBroadcast(EnumNetworkCommand command, string buf)
		{
			NetWorkProtocolSend send = new NetWorkProtocolSend();

			ushort trans = NetWorkProtocolSend.GetTransaction();

			send.MakeBlock(command, trans, buf);

			NetLib.SendCodeToBroadcast(send.bufSend, send.nBufCount);
		}

		static void ControlItemEnable(sbyte flag)
		{
			if(configStruct.bLatchLinePrinter == 1) 
			{
				SystemStatusMemory.SetDI(SSMDI.DuplexControlItemLinePrinter, flag);
			}
			else 
			{
				SystemStatusMemory.SetDI(SSMDI.DuplexControlItemLinePrinter, 1);
			}

			if(configStruct.bLatchReportPrinter == 1) 
			{
				SystemStatusMemory.SetDI(SSMDI.DuplexControlItemReportPrinter, flag);
			}
			else 
			{
				SystemStatusMemory.SetDI(SSMDI.DuplexControlItemReportPrinter, 1);
			}
		}

		static void SendBlockToAllServer(EnumNetworkCommand command, string buf)
		{
			NetWorkProtocolSend send = new NetWorkProtocolSend();

			ushort trans = NetWorkProtocolSend.GetTransaction();

			send.MakeBlock(command, trans, buf);

			NetLib.SendCodeToAllServer(command, trans, send.bufSend, send.nBufCount);
		}

		static void SendCommandChangeMain(string name)
		{
			string buf;

			buf = String.Format("NodeName={0},", name);
	
			SendBlockToAllServer(EnumNetworkCommand.CHANGE_MAIN, buf);
		}

		

		static void CheckServerStatus()
		{
			LifeSignalSendCheck();

			// 두 서버의 상태가 모두 활성화/비활성화 일때는 자동 감시 모드가 아니라도 조절해 주어야 한다.
			if(SystemStatusMemory.GetDI(SSMDI.DuplexConnect) == 1) 
			{	// 이중화가 연결 되었다.

				if(SystemStatusMemory.GetDI(SSMDI.DuplexActiveI) == 0 &&
					SystemStatusMemory.GetDI(SSMDI.DuplexActiveYou) == 0) 
				{
					if(configStruct.nNodeType == 1) 
					{	// Primary Server
						if(bErrorFlag == false) 
						{
							timeoutServer.Reset();
							bErrorFlag = true;
						}
						else 
						{
							if(timeoutServer.IsTimeOut(3)) 
							{
								SystemStatusMemory.SetDI(SSMDI.DuplexActiveI, 1);
								ControlItemEnable(1);
								SendCommandChangeMain(configStruct.sNodeName);
								FormViewServer.InvalidateServerStatusView();
								bErrorFlag = false;
							}
						}
					}
				}
				else if(SystemStatusMemory.GetDI(SSMDI.DuplexActiveI) == 1 &&
					SystemStatusMemory.GetDI(SSMDI.DuplexActiveYou) == 1) 
				{
					if(configStruct.nNodeType == 1) 
					{	// Primary Server
						if(bErrorFlag == false) 
						{
							timeoutServer.Reset();
							bErrorFlag = true;
						}
						else 
						{
							if(timeoutServer.IsTimeOut(3)) 
							{
								SystemStatusMemory.SetDI(SSMDI.DuplexActiveI, 1);
								ControlItemEnable(1);
								SendCommandChangeMain(configStruct.sNodeName);
								FormViewServer.InvalidateServerStatusView();
								bErrorFlag = false;
							}
						}
					}
				}
				else 
				{
					bErrorFlag = false;
				}
			}

			if(timeoutLifeRecv.IsTimeOut(10)) 
			{	// 상대편의 접속이 끊어졌는가를 감시.
				timeoutLifeRecv.Reset();
				if(SystemStatusMemory.GetDI(SSMDI.DuplexConnect) != 0) 
				{
					SystemStatusMemory.SetDI(SSMDI.DuplexConnect, 0);
					FormViewServer.InvalidateServerStatusView();
				}
				if(SystemStatusMemory.GetDI(SSMDI.DuplexActiveI) != 1) 
				{
					SystemStatusMemory.SetDI(SSMDI.DuplexActiveI, 1);
					ControlItemEnable(1);
					SystemStatusMemory.SetDI(SSMDI.DuplexActiveYou, 0);
					FormViewServer.InvalidateServerStatusView();
				}
			}
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

		static bool NetCheckRecvCommandOne(Socket socket, ref ushort recv_command, ref ushort recv_trans, ref string recv_ip, byte[] recv_data, int index, int retn_size, string ip)
		{
			NetWorkProtocolRecv split = new NetWorkProtocolRecv();

			if(split.Split(recv_data, index, retn_size)) 
			{
				if(split.wCommand == (ushort)EnumNetworkCommand.LIFE_SIGNAL_SERVER) 
				{
					if(split.sNodeName == configStruct.sNodeName) 
					{	// ECHO code

					}
					else 
					{
						timeoutLifeRecv.Reset();

						SystemStatusMemory.SetDI(SSMDI.DuplexConnect, 1);
						SystemStatusMemory.SetDI(SSMDI.DuplexActiveYou, split.bServerActive);
						SystemStatusMemory.SetDI(SSMDI.ErrorStatusPlcScanTimeOutYou, split.bPlcScanTimeOut);
						SystemStatusMemory.SetDI(SSMDI.AnotherProgramExitingYou, split.bAnotherProgramExit);
						SystemStatusMemory.SetDI(SSMDI.DuplexAutoWatchYou, split.bAutoWatch);
						statusStruct.sAnotherName = split.sNodeName;
						statusStruct.sAnotherIP = ip;
						FormViewServer.InvalidateServerStatusView();
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

				else if(split.wCommand == (ushort)EnumNetworkCommand.CHANGE_MAIN) 
				{
					if(String.Compare(split.sNodeName, configStruct.sNodeName, true) == 0) 
					{
						SystemStatusMemory.SetDI(SSMDI.DuplexActiveI, 1);
						ControlItemEnable(1);
						SystemStatusMemory.SetDI(SSMDI.DuplexActiveYou, 0);
					}
					else 
					{
						SystemStatusMemory.SetDI(SSMDI.DuplexActiveI, 0);
						ControlItemEnable(0);
						SystemStatusMemory.SetDI(SSMDI.DuplexActiveYou, 1);
					}

					FormViewServer.InvalidateServerStatusView();
				}
				else if(split.wCommand == (ushort)EnumNetworkCommand.DUPLEX_HAND) 
				{	// 이중화 시스템이 수동이 되었다.
					SystemStatusMemory.SetDI(SSMDI.DuplexAutoWatchI, 0);	// 자동 감시를 하지 않는다.
					FormViewServer.InvalidateServerStatusView();
				}
				else if(split.wCommand == (ushort)EnumNetworkCommand.DUPLEX_AUTO) 
				{	// 이중화 시스템이 자동이 되었다.
					SystemStatusMemory.SetDI(SSMDI.DuplexAutoWatchI, 1);	// 자동 감시를 한다.
					FormViewServer.InvalidateServerStatusView();
				}
				else if(split.wCommand == (ushort)EnumNetworkCommand.SERVER_REGISTER) 
				{
					if(String.Compare(split.sNodeName, configStruct.sNodeName) != 0) 
					{
						statusStruct.sAnotherName = split.sNodeName;
						statusStruct.sAnotherIP = ip;
						FormViewServer.InvalidateServerStatusView();
					}
				}
				else if(split.wCommand == (ushort)EnumNetworkCommand.SERVER_UNREGISTER) 
				{
					if(String.Compare(split.sNodeName, configStruct.sNodeName) != 0) 
					{
						statusStruct.sAnotherName = "????";
						statusStruct.sAnotherIP = "?.?.?.?";
						SystemStatusMemory.SetDI(SSMDI.DuplexConnect, 0);
						FormViewServer.InvalidateServerStatusView();
					}			
				}	
				else if(split.wCommand == (ushort)EnumNetworkCommand.TIME_SYNC) 
				{
					NetLib.SendCodeACK(socket, ip, split.wTransaction);
					TimeUtil.SetLocalTime(split.tDateTime.wYear, split.tDateTime.wMonth, split.tDateTime.wDay, split.tDateTime.wHour, split.tDateTime.wMinute, split.tDateTime.wSecond);
				}	
				else if(split.wCommand == (ushort)EnumNetworkCommand.PLCSCAN_WRITE_BIT) 
				{
					NetLib.SendCodeACK(socket, ip, split.wTransaction);
					//WriteCommandToPlcScan(recv_data, retn_size);
				}	
				else if(split.wCommand == (ushort)EnumNetworkCommand.PLCSCAN_WRITE_WORD) 
				{
					NetLib.SendCodeACK(socket, ip, split.wTransaction);
					//WriteCommandToPlcScan(recv_data, retn_size);
				}
				else if(split.wCommand == (ushort)EnumNetworkCommand.PLCSCAN_PORT_LIFE_SIGNAL) 
				{
					//CheckPlcScanPortLifeSignal(&split);
				}
				else if(split.wCommand == (ushort)EnumNetworkCommand.PLCSCAN_NEED_ALL_DATA) 
				{
					//CheckPlcScanNeedAllData(&split);
				}
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
			LifeSignalSendCheck();	// 여기에서 라이프 시그널을 보내는 이유는 클라이언트에 데이터를 보내고
			// ACK를 기다릴 때 이 함수를 불러주기 때문이다.
			// 클라이언트가 응답이 길어지면 상대서버에서 이 서버의 상태를 인식못하기 때문에 이 부분인 필요하다.

			if(socket == null)	return false;
	
			int remain;

			remain = socket.Available;
			
			/*
			byte[] outValue = BitConverter.GetBytes(0);
			// Check how many bytes have been received.
			const int FIONREAD   = 0x4004667F;
			socket.IOControl(FIONREAD, null, outValue);
    
			uint bytesAvailable = BitConverter.ToUInt32(outValue, 0);
			*/

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

		public static void ServerActive(sbyte i_you)
		{
			SendBlockToAllServer(EnumNetworkCommand.DUPLEX_HAND, "");

			if(i_you == 0) 
			{
				if(SystemStatusMemory.GetDI(SSMDI.DuplexConnect) == 1) 
				{
					SendCommandChangeMain(configStruct.sNodeName);
				}
			}
			else 
			{
				if(SystemStatusMemory.GetDI(SSMDI.DuplexConnect) == 1) 
				{
					SendCommandChangeMain(statusStruct.sAnotherName);
				}	
			}
		}

		public static void SetAutoWatch(bool flag)
		{
			if(flag)	SendBlockToAllServer(EnumNetworkCommand.DUPLEX_AUTO, "");
			else		SendBlockToAllServer(EnumNetworkCommand.DUPLEX_HAND, "");
		}
	}
}
