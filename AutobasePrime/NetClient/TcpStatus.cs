using System;
using System.Net.Sockets;
using System.Net;
using NetCommon;

namespace NetClient
{
	/// <summary>
	/// Summary description for TcpStatus.
	/// </summary>
	public class TcpStatus
	{
		public TcpStatus()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static Socket socketTcp = null; 

		static Socket ConnectSocket(string ip)
		{
			Socket socket  = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			IPEndPoint iep = new IPEndPoint(NetLib.ChangeAddress(ip), configStruct.nTcpPort);

			try 
			{
				socket.Connect(iep);
			}
			catch 
			{
				socket = null;
			}

			return socket;
		}

		public static void Status()
		{
			if(socketTcp == null) 
			{
				SERVER_LIST server;
				for(int i = 0; i < NetClientStatus.serverList.Count; i++) 
				{
					server = (SERVER_LIST)NetClientStatus.serverList[i];

					if(server.flag == 0)	continue;	// 연결이 되지 않았다.
					if(i == 0) 
					{	// stand alone server
						socketTcp = ConnectSocket(server.ip);
					}
					else 
					{
						if(server.bDualActive == true)
						{
							socketTcp = ConnectSocket(server.ip);
						}
					}
				}
			}

			if(socketTcp == null)	return;	// 연결할 만한 소켓이 없거나 있더라도 오류 발생

			byte[] buffer = new byte[1];
			buffer[0] = 1;
			
			try 
			{
				socketTcp.Send(buffer);
			}
			catch // 상대편이 접속을 끊은 경우가 대부분.
			{
				socketTcp.Close();
				socketTcp = null;
			}
		}
	}
}
