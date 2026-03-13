using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using SMS.SmsUtil;
using SMS.SmsFunc;
using AutoLibLocal;
using NetTools;
using System.Threading;


namespace SMS.SmsComm
{
	/// <summary>
	/// Summary description for SmsServerClientProc.
	/// </summary>
	public class SmsServerClientProc
	{
		//public enum eServerCommStatus { COMM_OK = 0, ERROR, WAITING, };

		public static Socket smsUdpSocket = null;
		//public static Socket writeSocket = null;
		public static ushort nTrans = 0;
		//static bool bSendAfterReadAckProcess = false;
		
		public SmsServerClientProc()
		{
			//
			// TODO: Add constructor logic here
			//			
		}

        //public static void checkAndMakeReadSocket(bool bNew)// 새로 소켓을 생성할 것인가
        //{
        //    bool	bCreate = false;
        //    if(smsUdpSocket == null)
        //    {
        //        smsUdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //        bCreate = true;
        //    }
        //    else if(bNew) 
        //    {
        //        smsUdpSocket.Close();
        //        smsUdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //        bCreate = true;
        //    }
			
        //    if(smsUdpSocket == null) return;
        //    if(smsUdpSocket.LocalEndPoint == null)							// bind 를 할 수 없거나 2번이상하면 catch에 걸린다.
        //    {
        //        try 
        //        {
        //            // Warning이 생겨도 바꾸지 말것 GetHostEntry는 Vista에서 잘안됨
        //            IPHostEntry lipa = Dns.Resolve(Dns.GetHostName());
        //            EndPoint ep = (EndPoint)new IPEndPoint(lipa.AddressList[0], SmsBasic.smsConfig.wPortNo);
        //            smsUdpSocket.Bind(ep);					
        //        }
        //        catch 
        //        {
        //            if(bCreate) socketBindErrorMessageDisplay();
        //        }
        //    }
        //}

        //가상네트워크어댑터가 있거나 어댑터가 2개 이상인 경우, 바인딩 문제 해결. 20241111 PSU
        public static void checkAndMakeReadSocket(bool bNew)// 새로 소켓을 생성할 것인가
        {
            bool bCreate = false;
            if (smsUdpSocket == null)
            {
                smsUdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                bCreate = true;
            }
            else if (bNew)
            {
                smsUdpSocket.Close();
                smsUdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                bCreate = true;
            }

            if (smsUdpSocket == null) return;
            if (smsUdpSocket.LocalEndPoint == null)							// bind 를 할 수 없거나 2번이상하면 catch에 걸린다.
            {
                try
                {
                    // Warning이 생겨도 바꾸지 말것 GetHostEntry는 Vista에서 잘안됨
                    //IPHostEntry lipa = Dns.Resolve(Dns.GetHostName());
                    //EndPoint ep = (EndPoint)new IPEndPoint(lipa.AddressList[0], SmsBasic.smsConfig.wPortNo);

                    IPAddress localAddress;
                    if (SmsBasic.smsConfig.eConnectType == SendSMSData.eConectionType.WEB_SERVICE)
                    {
                        // 서버: config에 설정된 IP 사용
                        byte[] ipBytes = new byte[4];
                        ipBytes[0] = SmsBasic.smsConfig.cIP1;
                        ipBytes[1] = SmsBasic.smsConfig.cIP2;
                        ipBytes[2] = SmsBasic.smsConfig.cIP3;
                        ipBytes[3] = SmsBasic.smsConfig.cIP4;
                        localAddress = new IPAddress(ipBytes);
                        EndPoint ep = (EndPoint)new IPEndPoint(localAddress, SmsBasic.smsConfig.wPortNo);
                        smsUdpSocket.Bind(ep);
                    }
                    else
                    {
                        // 클라이언트: 현재 사용 중인 네트워크의 로컬 IP 얻기
                        try
                        {
                            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                            {
                                // 서버 IP로 연결을 시도하여 사용할 로컬 IP 확인
                                byte[] serverIpBytes = new byte[4];
                                serverIpBytes[0] = SmsBasic.smsConfig.cIP1;
                                serverIpBytes[1] = SmsBasic.smsConfig.cIP2;
                                serverIpBytes[2] = SmsBasic.smsConfig.cIP3;
                                serverIpBytes[3] = SmsBasic.smsConfig.cIP4;
                                IPAddress serverAddress = new IPAddress(serverIpBytes);
                                socket.Connect(serverAddress, SmsBasic.smsConfig.wPortNo);
                                IPEndPoint localEndPoint = socket.LocalEndPoint as IPEndPoint;
                                localAddress = localEndPoint.Address;
                            }
                        }
                        catch
                        {
                            // 연결 실패 시 로컬호스트 사용
                            localAddress = IPAddress.Loopback;
                        }

                        //EndPoint ep = (EndPoint)new IPEndPoint(localAddress, 0);  // 동적 포트 할당
                        EndPoint ep = (EndPoint)new IPEndPoint(localAddress, SmsBasic.smsConfig.wPortNo);  // 동일한 고정 포트 할당
                        smsUdpSocket.Bind(ep);
                    }
                }
                catch
                {
                    if (bCreate) socketBindErrorMessageDisplay();
                }
            }
        }
		
		
		public static void socketClose()
		{
			if(smsUdpSocket == null) return;
			smsUdpSocket.Close();
			smsUdpSocket = null;
		}

		static void socketBindErrorMessageDisplay()
		{
			string		message = "";
			if(Tools.IsLangKorean()) 
			{
				message = string.Format("UDP/IP 통신용 소켓을 (Port No = {0}) Bind 할 수 없습니다.", SmsBasic.smsConfig.wPortNo);
				MessageBox.Show(message, "소켓 열기 오류");
			}
			else 
			{
				message = string.Format("Cannot Bind UDP/IP Socket. (Port No = {0})", SmsBasic.smsConfig.wPortNo);				
				MessageBox.Show(message, "Socket Binding Error");
			}
		}

		
		static int SendCodeACK(IPEndPoint iep)
		{
			NetWorkProtocolSend send = new NetWorkProtocolSend();

			iep.Port = SmsBasic.smsConfig.wPortNo;							// 읽은포트가 다를 수 있으므로 포트설정을 다시한다.
			nTrans++;
			send.MakeBlock(EnumNetworkCommand.ACK, nTrans, "");
			try
			{
				return NetworkTools.writeUdpData(iep, send.bufSend, send.nBufCount);
			}
			catch
			{
				return 0;
			}
		}

		static void blockByteToBuf(NetWorkProtocolRecv split, ref string buf)
		{
			if(split == null || split.Block == null) return;			
			for(int i = 0; i < split.Block.Length/2; i++) 
			{
				buf += (char)(split.Block[i*2] * 0x100 + split.Block[i*2+1]);
			}			
		}

		static SendSMSData.eCommStatus SmsServerCheckRecvCommand(out EnumNetworkCommand command, ref string buf, ref EndPoint ep)
		{
			checkAndMakeReadSocket(false);
			command = EnumNetworkCommand.SMS_CALL_ERROR;			
			byte[] readData = NetworkTools.readUdpData(smsUdpSocket, ref ep);
			if(readData == null) return SendSMSData.eCommStatus.COMM_WAITING;			

			NetWorkProtocolRecv split = new NetWorkProtocolRecv();
			if(split.Split(readData, readData.Length))
			{
				command = (EnumNetworkCommand)split.wCommand;
				IPEndPoint	iep = (IPEndPoint)ep;
				if(split.wCommand == (ushort)EnumNetworkCommand.SMS_DATA) 
				{
					if(SmsBasic.smsConfig.bUseServer == false || SmsBasic.smsConfig.eConnectType == SendSMSData.eConectionType.UDP_IP) return SendSMSData.eCommStatus.COMM_WAITING;// 서버가 아니면 응답하지 않는다.
					if(SendCodeACK(iep) == 0) return SendSMSData.eCommStatus.COMM_ERROR;
					blockByteToBuf(split, ref buf);
					return SendSMSData.eCommStatus.COMM_OK;
				}
				if(split.wCommand == (ushort)EnumNetworkCommand.SMS_CALL_ERROR) 
				{
					if(SmsBasic.smsConfig.eConnectType != SendSMSData.eConectionType.UDP_IP) return SendSMSData.eCommStatus.COMM_WAITING;// 클라이언트가 아니면 응답하지 않는다.
					//if(SendCodeACK(iep) == 0) return SendSMSData.eCommStatus.COMM_ERROR;
					blockByteToBuf(split, ref buf);
					return SendSMSData.eCommStatus.COMM_OK;
				}
			}
			return SendSMSData.eCommStatus.COMM_ERROR;
		}

		public static void SmsServerClientDataRecvCheck()
		{
			//if(bSendAfterReadAckProcess) return;			// 통신데이터를 서버에 보내고 ACK를 받는 중이다.
			if(SmsBasic.smsConfig.eConnectType == SendSMSData.eConectionType.WEB_SERVICE && SmsBasic.smsConfig.bUseServer == false) return;
			if(SmsBasic.smsConfig.eConnectType == SendSMSData.eConectionType.SERIAL && SmsBasic.smsConfig.bUseServer == false) return;

			SendSMSData.eCommStatus		retn;
			string						buf = "";
			EnumNetworkCommand			command;
		
			EndPoint	ep = (EndPoint)new IPEndPoint(IPAddress.Any, SmsBasic.smsConfig.wPortNo);
			retn = SmsServerCheckRecvCommand(out command, ref buf, ref ep);
			if(retn != SendSMSData.eCommStatus.COMM_OK) return;
			
			IPEndPoint		iep = (IPEndPoint)ep;
			switch(command) 
			{
				case EnumNetworkCommand.SMS_DATA : readSmsClientDataToBlock(buf, iep.Address.ToString()); break;
				case EnumNetworkCommand.SMS_CALL_ERROR : 
					SaveCommSendData.SaveErrorCallBackFromServer(buf, iep.Address.ToString()); break;				
			}
		}

		static void setClientDataItem(alarmDataMain	data, string buf, string ipString)
		{
			CommaBlockString	comma = new CommaBlockString();
			comma.Set(buf);
			string			imsi = "";

			comma.GetString(ref imsi);
			data.userName = imsi;
			if(comma.IsEOS()) return;

			comma.GetString(ref imsi);
			data.phoneNo = imsi;
			if(comma.IsEOS()) return;

			comma.GetString(ref imsi);
			data.sendPhoneNo = imsi;
			if(comma.IsEOS()) return;

			comma.GetInt(ref data.nSmsType);
			if(comma.IsEOS()) return;

            //텔레그램 20241111 PSU
            comma.GetString(ref imsi);
            data.ChatID = imsi;
            if (comma.IsEOS()) return;

			comma.GetStringTotalRemain(ref imsi);
			data.data = imsi;
		}

		static void readSmsClientDataToBlock(string buf, string ipString)
		{
			alarmDataMain		data = new alarmDataMain();
			
			data.bManual = false;
			data.nUserNo = 0;										// 사용자 위치 0 ~ 255
			data.bClient = true;
			data.dIpAddress = ipString;
			data.userName = "Client";
			data.phoneNo = "";
			data.sendPhoneNo = "";
			setClientDataItem(data, buf, ipString);

			SmsBasic.arrSmsQueue.Add(data);
		}

		static SendSMSData.eCommStatus WaitACK(IPEndPoint iep)
		{
			checkAndMakeReadSocket(false);
			EndPoint	ep = (EndPoint)new IPEndPoint(IPAddress.Any, SmsBasic.smsConfig.wPortNo);
			byte[] readData = NetworkTools.readUdpData(smsUdpSocket, ref ep);
			if(readData == null) return SendSMSData.eCommStatus.COMM_WAITING;
			
			NetWorkProtocolRecv split = new NetWorkProtocolRecv();
			if(split.Split(readData, readData.Length)) 
			{
				IPEndPoint	imsi = (IPEndPoint)ep;
				//if(split.wCommand == (ushort)EnumNetworkCommand.SMS_DATA) 클라이언트 루프이므로 읽지 않는다.
				if(split.wCommand == (ushort)EnumNetworkCommand.ACK) 
				{
					if(iep.Address.ToString() != imsi.Address.ToString()) return SendSMSData.eCommStatus.COMM_WAITING;// 포트는 다를 수도 있음
					return SendSMSData.eCommStatus.COMM_OK;
				}
				if(split.wCommand == (ushort)EnumNetworkCommand.SMS_CALL_ERROR) // 이전에 송신한 메시지 에러코드를 받는다
				{
					if(SmsBasic.smsConfig.eConnectType != SendSMSData.eConectionType.UDP_IP) return SendSMSData.eCommStatus.COMM_WAITING;
					string		buf = "";
					blockByteToBuf(split, ref buf);
					SaveCommSendData.SaveErrorCallBackFromServer(buf, imsi.Address.ToString());
					return SendSMSData.eCommStatus.COMM_WAITING;
				}
			}
			return SendSMSData.eCommStatus.COMM_WAITING;
		}


		public static SendSMSData.eCommStatus SendSMSDataToServer(string data, EnumNetworkCommand command)
		{
			if(data.Length <= 0) return SendSMSData.eCommStatus.COMM_ERROR;
			IPEndPoint	iep = NetworkTools.getUdpPortIpEndPoint(SmsBasic.smsConfig.cIP1, SmsBasic.smsConfig.cIP2, SmsBasic.smsConfig.cIP3, SmsBasic.smsConfig.cIP4, SmsBasic.smsConfig.wPortNo);
			return SendSMSDataToServerClient(data, iep, command);
		}


		public static SendSMSData.eCommStatus SendSMSDataToClient(string data, string ipString, EnumNetworkCommand command)
		{
			if(data.Length <= 0 || ipString.Length <= 0) return SendSMSData.eCommStatus.COMM_ERROR;
			CommaBlockString		comma = new CommaBlockString();
			byte		ip1 = 192, ip2 = 168, ip3 = 1, ip4 = 1;

			try 
			{
				comma.SetBlockCode('.');
				comma.Set(ipString);
				comma.GetBYTE(ref ip1);
				comma.GetBYTE(ref ip2);
				comma.GetBYTE(ref ip3);
				comma.GetBYTE(ref ip4);
			}
			catch 
			{
				return SendSMSData.eCommStatus.COMM_ERROR;
			}
			IPEndPoint	iep = NetworkTools.getUdpPortIpEndPoint(ip1, ip2, ip3, ip4, SmsBasic.smsConfig.wPortNo);
			return SendSMSDataToServerClient(data, iep, command);
		}

		public static SendSMSData.eCommStatus SendSMSDataToServerClient(string data, IPEndPoint	iep, EnumNetworkCommand command)
		{
			if(data.Length <= 0) return SendSMSData.eCommStatus.COMM_ERROR;
			NetWorkProtocolSend send = new NetWorkProtocolSend();
			string			buf;
			
			buf = string.Format("BlockSize={0},Block=", data.Length*2);
			for(int i = 0; i < data.Length; i++) 
			{
				buf += string.Format("{0,4:X04}", (ushort)data[i]);
			}
			buf += ",";
			nTrans++;
			send.MakeBlock(command, nTrans, buf);

			//bSendAfterReadAckProcess = true;				// 통신데이터를 서버에 보내고 ACK를 받는 중이다.
			try
			{
				int nRetn = NetworkTools.writeUdpData(iep, send.bufSend, send.nBufCount);
				if(nRetn == 0) return SendSMSData.eCommStatus.COMM_ERROR;

                string help_msg = String.Format("Send To {0} Cmd={1}", iep.ToString(), command.ToString());
                SendSMSData.codeViewPush.DisplaySendCode(help_msg, help_msg.Length);
                SendSMSData.codeViewPush.DisplaySendNextLine();
			}
			catch
			{
				//bSendAfterReadAckProcess = false;				// 통신데이터를 서버에 보내고 ACK를 받는 중이 아니다.
				return SendSMSData.eCommStatus.COMM_ERROR;
			}			
			
			TimeOutClass			timeout = new TimeOutClass();			
			SendSMSData.eCommStatus	retn;
			timeout.Reset();
			
			while(true) 
			{
				Thread.Sleep(1);
				if(timeout.IsTimeOut(SmsBasic.smsConfig.cAckTimeOut)) 
				{
					//bSendAfterReadAckProcess = false;			// 통신데이터를 서버에 보내고 ACK를 받는 중이 아니다.
					return SendSMSData.eCommStatus.COMM_ERROR;
				}
				if(SendSMSData.bSmsCommThreadPause) 
				{
					//bSendAfterReadAckProcess = false;			// 통신데이터를 서버에 보내고 ACK를 받는 중이 아니다.
					return SendSMSData.eCommStatus.COMM_ERROR;	// thread 정지명령이 들어오면 return
				}
				retn = WaitACK(iep);
				if(retn == SendSMSData.eCommStatus.COMM_WAITING) continue;

                string help_msg = retn.ToString();
                SendSMSData.codeViewPush.DisplayRecvCode(help_msg, help_msg.Length);
                SendSMSData.codeViewPush.DisplayRecvNextLine();
				
				return retn;
			}
		}

		







	}
}
