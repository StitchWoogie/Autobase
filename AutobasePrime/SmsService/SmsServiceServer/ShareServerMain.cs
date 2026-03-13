using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Text;
using AutoLibLocal;
using NetTools;

namespace SmsServiceServer
{
	public class SocketThreadClass 
	{
		public Thread thread;
		public Socket socket;
	}

	public struct COMM_STATUS
	{
		public int nContinueFail;	// ¿¬¼Ó ½ÇÆÐ È½¼ö
		public int nCountTotal;		// ÃÑ Åë½Å È½¼ö
		public int nCountFail;		// ÃÑ ½ÇÆÐ È½¼ö
	}

	/// <summary>
	/// Summary description for ShareServerMain.
	/// </summary>
	public class ShareServerMain
	{
		public static int nPortTcp = 7110; 
		public static string sSiteNamePrimary; 
		public static string sSiteNameSecondary; 
		public static string sUserName;
		public static byte[] aPassCode = new byte[1];
		//public static int nPortUdp = 7201;
		static TcpListener m_Listener = null;
		public static COMM_STATUS[] commStatus = new COMM_STATUS[2];

		static ShareServerMain()
		{
			//
			// TODO: Add constructor logic here
			//
			LoadConfig();
		}

		public static void LoadConfig()
		{
			sSiteNamePrimary = TotalConfig.LoadRegAutoBaseConfig("SMSService", "Config", "SiteNamePrimary", "www.sms.autobase.biz");
			sSiteNameSecondary = TotalConfig.LoadRegAutoBaseConfig("SMSService", "Config", "SiteNameSecondary", "www.sms2.autobase.biz");
			sUserName = TotalConfig.LoadRegAutoBaseConfig("SMSService", "Config", "Username", "");
			aPassCode = TotalConfig.LoadRegAutoBaseConfig("SMSService", "Config", "PassCode", aPassCode);
			nPortTcp = TotalConfig.LoadRegAutoBaseConfig("SMSService", "Config", "LocalServerPort", 7110);	
		}

		static bool bUsingArray = false;
		public static void SetWait()
		{
			TimeOutClass timeout = new TimeOutClass();
			timeout.Reset();
			while(bUsingArray) 
			{
				Thread.Sleep(1);
				if(timeout.IsTimeOut(5))	break;
			}
			bUsingArray = true;
		}

		public static void ResetWait()
		{
			bUsingArray = false;
		}

		static Thread threadListener = null;

		public static void Init()
		{
			m_Done = false;
			m_bEnded = false;

			IPAddress ipAddress = IPAddress.Parse("0.0.0.0");

			m_Listener = new TcpListener(ipAddress, nPortTcp);

			threadListener = new Thread(new ThreadStart(Listener));
			threadListener.Start();
		}

		static bool m_Done = false;
		static bool m_bEnded = false;

		public static void UnInit()
		{
			m_Done = true;
			
			/*
			TcpClient s = new TcpClient();
			try 
			{
				s.Connect("127.0.0.1", nPortTcp);
			}
			catch 
			{

			}
			*/

			while(!m_bEnded) 
			{
				Thread.Sleep(1);	// wait thread ended
			}
			

			threadListener.Abort();
			threadListener.Join();
		}

		public static ArrayList arraySocketThread = new ArrayList();
		static SocketThreadClass tempSocketThread;

		static bool SendCommand(Socket socket, byte[] buf, int size, string site, ref COMM_STATUS status)
		{
			status.nCountTotal++;

			if(size < 2)	return true;

			int total_length = buf[0]+buf[1]*256;
			if(total_length != size)	return true;

			string sms_recv;
			string sms_send;
			string sms_msg;
			int pos = 2;
			int length;
			byte[] imsi;
			int i;
			
			if(buf[pos++] != 0)	return true;
			length = buf[pos++];
			imsi = new byte[length];
			for(i = 0; i < length; i++) 
			{
				imsi[i] = buf[pos++];
			}
			sms_recv = Tools.BytesToString(imsi);

			if(buf[pos++] != 1)	return true;
			length = buf[pos++];
			imsi = new byte[length];
			for(i = 0; i < length; i++) 
			{
				imsi[i] = buf[pos++];
			}
			sms_send = Tools.BytesToString(imsi);

			if(buf[pos++] != 2)	return true;
			length = buf[pos++];
			imsi = new byte[length];
			for(i = 0; i < length; i++) 
			{
				imsi[i] = buf[pos++];
			}
			sms_msg = Tools.BytesToString(imsi);

			int sms_type = 0;
			if(pos < total_length) 
			{
				if(buf[pos++] == 3) 
				{
					length = buf[pos++];
					sms_type = buf[pos++];
				}
			}

			biz.autobase.sms.www.ServiceSend2 send = new SmsServiceServer.biz.autobase.sms.www.ServiceSend2(site);
			int retn;
			
			try 
			{
				retn = send.SendByAutoBase(sUserName, aPassCode, sms_recv, sms_send, sms_msg, sms_type);
			}
			catch 
			{
				status.nContinueFail++;
				status.nCountFail++;
				return false;
			}

			byte[] send_buf = new byte[1];
			send_buf[0] = (byte)retn;
			socket.Send(send_buf);

			status.nContinueFail = 0;
			return true;
		}

		static void ExecuteCommand(Socket socket, byte[] buf, int size)
		{
			if(commStatus[1].nContinueFail < commStatus[0].nContinueFail)
			{
				if(SendCommand(socket, buf, size, sSiteNameSecondary, ref commStatus[1]))
					return;
				if(SendCommand(socket, buf, size, sSiteNamePrimary, ref commStatus[0]))	
					return;
			}
			else 
			{
				if(SendCommand(socket, buf, size, sSiteNamePrimary, ref commStatus[0]))	
					return;
				if(SendCommand(socket, buf, size, sSiteNameSecondary, ref commStatus[1]))
					return;
			}
		}
        
		static void ThreadSocket()
		{
			int count;
			SocketThreadClass socketThread = tempSocketThread;

			ShareServerMain.SetWait();
			arraySocketThread.Add(socketThread);
			ShareServerMain.ResetWait();
			Socket socket = socketThread.socket;
			Thread thread = socketThread.thread;
			
			byte[] recv_data;
			TimeOutClass timeout = new TimeOutClass();
			TimeOutClass timeout_nosignal = new TimeOutClass();
						
            while(!m_Done) 
			{
				Thread.Sleep(1);
				if(timeout_nosignal.IsTimeOut(60))	break;
				if(!socket.Connected)	break;
				count = socket.Available;
				if(count == 0)	continue;

				timeout_nosignal.Reset();

				recv_data = new byte[count];

				count = socket.Receive(recv_data);

				ExecuteCommand(socket, recv_data, count);
			}

			//thread.Abort();
			//thread.Join();
			ShareServerMain.SetWait();
			arraySocketThread.Remove(socketThread);
			ShareServerMain.ResetWait();
			socket.Close();
		}

		public static void Listener() 
		{
			m_Listener.Start();
			while(!m_Done) 
			{
				Thread.Sleep(1);

				if(m_Listener.Pending()) 
				{
					Socket socket = m_Listener.AcceptSocket();

					tempSocketThread = new SocketThreadClass();
					tempSocketThread.socket = socket;
					tempSocketThread.thread = new Thread(new ThreadStart(ThreadSocket));
					tempSocketThread.thread.Start();
				}
			}
			m_Listener.Stop();

			while(true) 
			{
				Thread.Sleep(1);
				if(arraySocketThread.Count == 0)	break;
			}

			m_bEnded = true;
		
			return;
		}

	}
}
