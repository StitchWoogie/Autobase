using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Text;
using AutoLibLocal;
using NetTools;

namespace RunMain
{
    public class SocketThreadClass
    {
        public Thread thread;
        public Socket socket;
        public bool bUdp = false;
    }

    public class CommunicationStatus
    {
        public string ip;
        public int nFrameSend = 0;
        public int nFrameRecv = 0;
        public TimeOutClass timeout = new TimeOutClass();
    }

    /// <summary>
    /// Summary description for ShareServerMain.
    /// </summary>
    public class ShareServerMain
    {
        public static int nPortTcp = 7200;
        //public static int nPortUdp = 7201;    // 2017-6-26 필요없어서 제거했다.
        static TcpListener m_Listener = null;

        public static bool bShowRunTagOffStatus = true;

        static ShareServerMain()
        {
            //
            // TODO: Add constructor logic here
            //
            LoadConfig();
        }

        static void LoadConfig()
        {
            nPortTcp = TotalConfig.LoadRegAutoBaseConfig("DataServer", "Config", "nPortTcp", 7200);
            bShowRunTagOffStatus = TotalConfig.LoadRegAutoBaseConfig("DataServer", "Config", "bShowRunTagOffStatus", true); 
        }

        public static void SaveConfg()
        {
            TotalConfig.SaveRegAutoBaseConfig("DataServer", "Config", "nPortTcp", nPortTcp);
            TotalConfig.SaveRegAutoBaseConfig("DataServer", "Config", "bShowRunTagOffStatus", bShowRunTagOffStatus); 
        }

        /*
        static bool bUsingArray = false;
        public static void SetWait()
        {
            TimeOutClass timeout = new TimeOutClass();
            timeout.Reset();
            while (bUsingArray)
            {
                Thread.Sleep(1);
                if (timeout.IsTimeOut(5)) break;
            }
            bUsingArray = true;
        }

        public static void ResetWait()
        {
            bUsingArray = false;
        }*/

        public static object objLockArrayCommStatus = new object();

        static Thread threadListener = null;

        public static void Init()
        {
            nThreadCount = 0;

            m_Done = false;
            m_bEnded = false;

            IPAddress ipAddress = IPAddress.Parse("0.0.0.0");

            m_Listener = new TcpListener(ipAddress, nPortTcp);

            try
            {
                m_Listener.Start();
            }
            catch (Exception exception)
            {
                m_Listener = null;
                MessageDisplay.Show("Listener.Start() Port={0} Error:{1}", nPortTcp,  exception.Message);
            }

            threadListener = new Thread(new ThreadStart(Listener));
            threadListener.Start();
        }

        static bool m_Done = false;
        static bool m_bEnded = false;

        public static void UnInit()
        {
            m_Done = true;
            
            if (m_Listener != null)
            {
                m_Listener.Stop();
                m_Listener = null; 
            }

            // m_Done = true;   //이것을 맨 위로 옮겼다. 2023-3-13   m_Listener.Pending() 에서 exception 이 걸린다.

            while (!m_bEnded)
            {
                Thread.Sleep(1);	// wait thread ended
            }

            //threadListener.Abort();
            //threadListener.Join();

            lock (objLockArrayCommStatus)
            {
                arrayCommStatus.Clear();
            }
        }

        //public static ArrayList arraySocketThread = new ArrayList();
        public static int nThreadCount = 0;
        public static ArrayList arrayCommStatus = new ArrayList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="command"></param>
        /// <param name="tns"></param>
        /// <param name="data"></param>
        /// <returns>send에서 오류가 나면 false</returns>
        static bool SendData(Socket socket, EnumMultiBlockCommand command, ushort tns, string data)
        {
            MultiBlockProtocolSend send = new MultiBlockProtocolSend();

            send.ReadyBuf(command, data);
            byte[] block;
            bool next;
            int len;
            while (true)
            {
                block = send.GetBlock(tns, out next, out len);
                try
                {
                    socket.Send(block, 0, len, SocketFlags.None);
                }
                catch
                {
                    return false;
                }
                if (next == false) break;
            }

            return true;
        }

        static bool ExecuteCommand(SocketThreadClass thread, Socket socket, EnumMultiBlockCommand command, ushort tns, string data, CommunicationStatus status)
        {
            if (command == EnumMultiBlockCommand.TagValueList)
            {
                // 문자열에 , 가 포함되면 넘어오지 않아서 CommaTextMaker를 사용한다. 2016-12-21
                string result = "";
                string tag = "";
                CommaBlockString comma = new CommaBlockString();
                CommaTextMaker ctm = new CommaTextMaker();
                comma.Set(data);

                while (true)
                {
                    comma.GetString(ref tag);
                    if (tag.Length == 0) break;

                    string val = "";

                    if (!SharedTag.GetCurr(tag, ref val))
                        val = "0";

                    ctm.Write("{0},", val);
                }

                result = ctm.GetResult();

                status.nFrameSend++;

                return SendData(socket, EnumMultiBlockCommand.TagValueList_Reply, tns, result);

                // 문자열에 , 가 포함되면 넘어오지 않는다. 2016-12-21
                //
                //string result = "";
                //string tag = "";
                //CommaBlockString comma = new CommaBlockString();
                //comma.Set(data);
                //
                //while (true)
                //{
                //    comma.GetString(ref tag);
                //    if (tag.Length == 0) break;
                //
                //    string val = "";
                //
                //    if (!SharedTag.GetCurr(tag, ref val))
                //        val = "0";
                //
                //    result += val + ",";
                //}
                //
                //status.nFrameSend++;
                //
                //return SendData(socket, EnumMultiBlockCommand.TagValueList_Reply, tns, result);
            }
            else if (command == EnumMultiBlockCommand.SetTagValue)
            {
                string result = "";
                string tag = "";
                string val = "";
                string user = "";
                string ip = "";
                string computer = "";

                CommaTextReader comma = new CommaTextReader();
                comma.Set(data);

                comma.GetString(ref tag);
                comma.GetString(ref val);
                comma.GetString(ref user);
                comma.GetString(ref ip);
                comma.GetString(ref computer);

                //DataLocal local = new DataLocal();
                //local.WriteCurrST(tag, val, user, ip, computer);
                SharedTag.SetCurr(tag, val, user, ip, computer);

                status.nFrameSend++;

                return SendData(socket, EnumMultiBlockCommand.ACK, tns, result);
            }
            else
            {

            }

            return true;
        }

        static CommunicationStatus GetStatusClass(EndPoint endpoint)
        {
            string ip;
            if (endpoint == null)
            {
                ip = "Unknown";
            }
            else
            {
                IPEndPoint ipendpoint = (IPEndPoint)(endpoint);
                ip = ipendpoint.Address.ToString();
            }

            CommunicationStatus status;
            for (int i = 0; i < arrayCommStatus.Count; i++)
            {
                status = (CommunicationStatus)arrayCommStatus[i];

                if (status.ip == ip)
                {
                    return status;
                }
            }

            status = new CommunicationStatus();
            status.ip = ip;

            //ShareServerMain.SetWait();
            lock (ShareServerMain.objLockArrayCommStatus)
            {
                arrayCommStatus.Add(status);
            }
            //ShareServerMain.ResetWait();

            return status;
        }

        static bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 & part2)
            {//connection is closed
                return false;
            }
            return true;
        }

        static void ThreadSocket(object param)
        {
            nThreadCount++;     // arraySocketThread 를 사용하다 배열이 중간에 index가 틀려서 오류나는 경우가 있어서 (CPU 2개 이상에서 특히 발생) Count로만 사용한다.

            int count;
            SocketThreadClass socketThread = (SocketThreadClass)param;

            Socket socket = socketThread.socket;

            MultiBlockProtocolRecv recv = new MultiBlockProtocolRecv();
            byte[] recv_data;
            TimeOutClass timeout = new TimeOutClass();
            TimeOutClass timeout_nosignal = new TimeOutClass();

            CommunicationStatus status = GetStatusClass(socket.RemoteEndPoint);

            while (!m_Done)
            {
                Thread.Sleep(1);

                if (!SocketConnected(socket))
                {
                    break;
                }

                if (!socketThread.bUdp)
                {
                    if (timeout_nosignal.IsTimeOut(5)) 
                        break;
                    if (!socket.Connected) 
                        break;
                }

                count = socket.Available;
                if (count == 0) continue;

                //timeout_nosignal.Reset();

                recv_data = new byte[count];

                try
                {
                    count = socket.Receive(recv_data);  // 도중에 접속이 끊어져서 여기서 오류가 나는 경우가 있다. 2009.8.21
                }
                catch 
                {
                    //exception.Message
                    break;
                }

                status.nFrameRecv++;
                status.timeout.Reset();

                bool next;
                string text_data;

                timeout.Reset();

                while (!m_Done)
                {
                    if (timeout.IsTimeOut(10)) break;
                    if (!recv.SetBlock(recv_data, count, out next)) break;
                    if (next == false)
                    {
                        text_data = recv.Split();
                        if (ExecuteCommand(socketThread, socket, recv.nCommand, recv.nTns, text_data, status))
                            //break;
                            goto end_comm;  // 일단 통신을 종료한다.
                        else
                            goto error;
                    }
                }
            }

        error:
        end_comm:
            //thread.Abort();
            //thread.Join();
            socket.Close();

            nThreadCount--;     // arraySocketThread 를 사용하다 배열이 중간에 index가 틀려서 오류나는 경우가 있어서 (CPU 2개 이상에서 특히 발생) Count로만 사용한다.
            
        }

        public static void Listener()
        {
            //m_Listener.Start(); Init에서 한다.
            while (!m_Done)
            {
                Thread.Sleep(1);

                if (m_Listener == null) continue;

                if (m_Listener.Pending())
                {
                    Socket socket = m_Listener.AcceptSocket();

                    SocketThreadClass socketthread = new SocketThreadClass();
                    socketthread.socket = socket;
                    socketthread.thread = new Thread(new ParameterizedThreadStart(ThreadSocket));

                    socketthread.thread.Start(socketthread);
                }
            }
            //m_Listener.Stop(); UnInit에서 한다.

            TimeOutClass timeout = new TimeOutClass();
            while (true)
            {
                Thread.Sleep(1);
                if (timeout.IsTimeOut(10)) break;

                //if (arraySocketThread.Count == 0) break;
                if (nThreadCount <= 0) break;
            }

            m_bEnded = true;

            return;
        }

        /*
        public static void UdpInit()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            SocketThreadClass socketthread = new SocketThreadClass();
            socketthread.socket = socket;
            socketthread.bUdp = true;
            socketthread.thread = new Thread(new ParameterizedThreadStart(ThreadSocket));
            socketthread.thread.Start(socketthread);

            return;
        }*/


    }
}

/*
namespace RunMain
{
	public class SocketThreadClass 
	{
		public Thread thread;
		public Socket socket;
		public int nFrameSend = 0;
		public int nFrameRecv = 0;
		//public bool   bClose;
        public bool bUdp = false;
	}

    public class CommunicationStatus
    {
        public string ip;
        public int nFrameSend = 0;
        public int nFrameRecv = 0;
        public TimeOutClass timeout = new TimeOutClass();
    }

	/// <summary>
	/// Summary description for ShareServerMain.
	/// </summary>
	public class ShareServerMain
	{
		public static int nPortTcp = 7200;
		public static int nPortUdp = 7201;
		static TcpListener m_Listener = null;

		public ShareServerMain()
		{
			//
			// TODO: Add constructor logic here
			//
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

            UdpInit();
		}

		static bool m_Done = false;
		static bool m_bEnded = false;

		public static void UnInit()
		{
			m_Done = true;
			
			while(!m_bEnded) 
			{
				Thread.Sleep(1);	// wait thread ended
			}

			//threadListener.Abort();
			//threadListener.Join();

            
		}

		public static ArrayList arraySocketThread = new ArrayList();
		static SocketThreadClass tempSocketThread;
        public static ArrayList arrayCommStatus = new ArrayList();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="socket"></param>
		/// <param name="command"></param>
		/// <param name="tns"></param>
		/// <param name="data"></param>
		/// <returns>send에서 오류가 나면 false</returns>
		static bool SendData(Socket socket, EnumMultiBlockCommand command, ushort tns, string data)
		{
			MultiBlockProtocolSend send = new MultiBlockProtocolSend();

			send.ReadyBuf(command, data);
			byte[] block;
			bool next;
			int len;
			while(true) 
			{
				block = send.GetBlock(tns, out next, out len);
				try 
				{
					socket.Send(block, 0, len, SocketFlags.None);
				}
				catch 
				{
					return false;
				}
				if(next == false)	break;
			}

			return true;
		}

		static bool ExecuteCommand(SocketThreadClass thread, Socket socket, EnumMultiBlockCommand command, ushort tns, string data)
		{
			if(command == EnumMultiBlockCommand.TagValueList) 
			{
				string result = "";
				string tag = "";
				CommaBlockString comma = new CommaBlockString();
				comma.Set(data);

				while(true) 
				{
					comma.GetString(ref tag);
					if(tag.Length == 0)	break;
					
					string val="";

					if(!SharedTag.GetCurr(tag, ref val))
						val = "0";

					result += val+",";
				}

				thread.nFrameSend++;

				return SendData(socket, EnumMultiBlockCommand.TagValueList_Reply, tns, result);
			}
			else if(command == EnumMultiBlockCommand.SetTagValue) 
			{
				string result = "";
				string tag = "";
				string val = "";
				CommaBlockString comma = new CommaBlockString();
				comma.Set(data);

				comma.GetString(ref tag);
				comma.GetString(ref val);

				DataLocal local = new DataLocal();
				local.WriteCurrST(tag, val);

				thread.nFrameSend++;

				return SendData(socket, EnumMultiBlockCommand.ACK, tns, result);
			}
			else 
			{
				
			}

			return true;
		}

        static CommunicationStatus GetStatusClass(string ip)
        {
            CommunicationStatus status;
            for (int i = 0; i < arrayCommStatus.Count; i++)
            {
                status = (CommunicationStatus)arrayCommStatus[i];

                if (status == ip)
                {
                    return status;
                }
            }

            status = new CommunicationStatus();

            arrayCommStatus.Add(status);

            return status;
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
			
			MultiBlockProtocolRecv recv = new MultiBlockProtocolRecv();
			byte[] recv_data;
			TimeOutClass timeout = new TimeOutClass();
			TimeOutClass timeout_nosignal = new TimeOutClass();

            CommunicationStatus status = GetStatusClass(socket.RemoteEndPoint.ToString());
						
			while(!m_Done) 
			{
				Thread.Sleep(1);

                if (!socketThread.bUdp)
                {
                    if (timeout_nosignal.IsTimeOut(60)) break;
                    if (!socket.Connected) break;
                }
				
				count = socket.Available;
				if(count == 0)	continue;

				timeout_nosignal.Reset();

				recv_data = new byte[count];

				count = socket.Receive(recv_data);

				socketThread.nFrameRecv++;

				bool next;
				string text_data;

				timeout.Reset();

				while(!m_Done)
				{
					if(timeout.IsTimeOut(10))	break;
					if(!recv.SetBlock(recv_data, count, out next))	break;
					if(next == false) 
					{
						text_data = recv.Split();
						if(ExecuteCommand(socketThread, socket, recv.nCommand, recv.nTns, text_data))
							//break;
                            goto end_comm;  // 일단 통신을 종료한다.
						else
							goto error;
					}
				}
			}

			error:
            end_comm:
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
                if (m_Listener.Pending())
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

        public static void UdpInit()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            tempSocketThread = new SocketThreadClass();
            tempSocketThread.socket = socket;
            tempSocketThread.bUdp = true;
            tempSocketThread.thread = new Thread(new ThreadStart(ThreadSocket));
            tempSocketThread.thread.Start();

            return;
        }
		

	}
}*/
