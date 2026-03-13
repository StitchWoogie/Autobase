using System;
using AutoLibLocal;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using NetTools;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Reflection;

namespace ExportServer
{
	/// <summary>
	/// Summary description for ClassServer.
	/// </summary>
	public class ClassServer
	{
		public int port = 7300;
		public string protocol;
		public string protocol_option;
		public ArrayList arrayTag = new ArrayList();
		public int nDisconnectTimeOutOnNoSignal = 10;	// 통신을 하지않을때 끊는 시간
		public bool bUseTestValue = false;

        bool m_Done = false;
        bool m_bEnded = false;
        TcpListener m_Listener = null;
        Thread threadListener = null;



        //---로깅 추가 20241209 PSU ////
        private string logDir;  
        private readonly object logLock = new object();
        private bool isShuttingDown = false;  // 종료 상태 플래그 추가
        //---로깅 추가 20241209 PSU ////

        //20241216 PSU 상태태그 추가
        public string statusTag;
        public bool bUseStatusTag = false;

		public ClassServer()
		{
			//
			// TODO: Add constructor logic here
			//

            //ㅡㅡㅡㅡ로깅 추가 20241209 PSU ////
            try
            {
                string work_dir = TotalConfig.sDirWorkProject;
                logDir = Path.Combine(TotalConfig.GetProjectDataLogDirectory(work_dir), "ExportServer");

                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to create log directory: " + ex.Message);
            }
            //ㅡㅡㅡㅡㅡ로깅 추가 20241209 PSU ////

		}

        //ㅡㅡㅡㅡㅡ로깅 추가 20241209 PSU ////
        private string GetLogFilePath()
        {
            string protocolName = string.IsNullOrEmpty(protocol) ? "Unknown" : protocol;
            string date = DateTime.Now.ToString("yyyyMMdd");
            return Path.Combine(logDir,
                string.Format("{0}_{1}_{2}.log", date, protocolName, port));
        }

        // 일반 로그용 오버로드
        private void WriteLog(string message, Socket socket)
        {
            WriteLogInternal(message, socket, false);
        }

        // 에러 로그용 오버로드
        private void WriteLogError(string message, Socket socket)
        {
            WriteLogInternal(message, socket, true);
        }


        // 실제 로그 작성을 처리하는 내부 메소드
        private void WriteLogInternal(string message, Socket socket, bool isError)
        {
            try
            {
                lock (logLock)
                {
                    string logMessage = string.Format("[{0}] Protocol:{1} Port:{2} ",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        string.IsNullOrEmpty(protocol) ? "Unknown" : protocol,
                        port);

                    if (socket != null && socket.Connected)
                    {
                        try
                        {
                            IPEndPoint remoteEndPoint = (IPEndPoint)socket.RemoteEndPoint;
                            logMessage += string.Format("Client({0}:{1}) ",
                                remoteEndPoint.Address,
                                remoteEndPoint.Port);
                        }
                        catch (Exception)
                        {
                            logMessage += "Client(Unknown) ";
                        }
                    }

                    logMessage += string.Format("[Active Connections:{0}] ", arraySocketThread.Count);

                    if (isError)
                    {
                        logMessage += "[ERROR] ";
                    }

                    logMessage += message;

                    string logPath = GetLogFilePath();

                    string directory = Path.GetDirectoryName(logPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    using (StreamWriter writer = File.AppendText(logPath))
                    {
                        writer.WriteLine(logMessage);
                    }

                    if (isError)
                    {
                        System.Diagnostics.Debug.WriteLine(logMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to write log: " + ex.Message);

                try
                {
                    System.Diagnostics.EventLog.WriteEntry("ExportServer",
                        "Failed to write log: " + ex.Message,
                        System.Diagnostics.EventLogEntryType.Error);
                }
                catch { }
            }
        }
        //ㅡㅡㅡㅡㅡ로깅 추가 20241209 PSU ////


//        public void Init()
//        {
//            m_Done = false;
//            m_bEnded = false;

//            IPAddress ipAddress = IPAddress.Parse("0.0.0.0");
//            m_Listener = new TcpListener(ipAddress, port);

//            try
//            {
//                m_Listener.Start();
//            }
//            catch (Exception exception)
//            {
//                // 포트가 사용 중일때 오류가 나서 추가했다. 특히 PLC_SCAN에서 테스트하다가 중복되는 경우가 있다. 2019-12-3 추가.
//                MessageBox.Show(exception.Message, "m_Listener.Start() error Port="+port.ToString());
//                m_Listener.Stop();
//                m_Listener = null;
//            }

//            if (m_Listener != null)
//            {
//                threadListener = new Thread(new ThreadStart(Listener));
//                threadListener.Start();
//            }
//        }



//        public void UnInit()
//        {
//            m_Done = true;

//            if (threadListener != null)
//            {
//                TcpClient s = new TcpClient();
//                try
//                {
//                    s.Connect("127.0.0.1", port);
//                }
//                catch
//                {

//                }

//                TimeOutClass timeout = new TimeOutClass();

//                while (!m_bEnded)
//                {
//                    Thread.Sleep(1);	// wait thread ended
//                    if (timeout.IsTimeOut(5)) break;
//                }

//                threadListener.Join();
//            }

//            threadListener = null;
//        }

//        public ArrayList arraySocketThread = new ArrayList();
//        SocketThreadClass tempSocketThread;
//        public object syncLock = new object();

//        public void Listener() 
//        {
            
//            while(!m_Done) 
//            {
//                Thread.Sleep(1);

//                Socket socket = m_Listener.AcceptSocket();

//                // if(m_Done)	break;	// 프로그램을 종료하기 위해서 끝냈다.

//                tempSocketThread = new SocketThreadClass();
//                tempSocketThread.socket = socket;
//                //tempSocketThread.bClose = false;
//                tempSocketThread.thread = new Thread(new ThreadStart(ThreadSocket));
//                tempSocketThread.thread.Start();
//            }
//            m_Listener.Stop();

//            /*
//            SocketThreadClass st;
//            for(int i = 0; i < arraySocketThread.Count; i++) 
//            {
//                st = (SocketThreadClass)arraySocketThread[i];
//                st.bClose = true;
//            }
//            */

//            while(true) 
//            {
//                Thread.Sleep(1);
//                if(arraySocketThread.Count == 0)	break;
//            }

//            m_bEnded = true;
		
//            return;
//        }

//        void ThreadSocket()
//        {
//            SocketThreadClass socketThread = tempSocketThread;

//            lock (syncLock)
//            {
//                arraySocketThread.Add(socketThread);
//            }
			

//            Socket socket = socketThread.socket;

//#if USE_EXCEPTION_REPORT
//            try 
//            {
//#endif
//                int count;
				
//                Thread thread = socketThread.thread;

//                Type type = Type.GetType("System.Int32");
//                socketThread.obj = DriverList.LoadDriver(this.protocol); 

//                if(socketThread.obj == null)	goto load_fail;

//                if(socketThread.obj != null) 
//                {
//                    socketThread.obj.SetFunctionSend(new ExportLib.ClassExportLib2.DelegateSendBytes(socketThread.Send));
//                    socketThread.obj.SetFunctionGetTagValue(new ExportLib.ClassExportLib2.DelegateGetTagValue(GetTagValue));
//                    socketThread.obj.SetFunctionSetTagValue(new ExportLib.ClassExportLib2.DelegateSetTagValue(SetTagValue));
//                    socketThread.obj.SetFunctionGetTagValueByName(new ExportLib.ClassExportLib2.DelegateGetTagValueByName(GetTagValueByName));
//                    socketThread.obj.SetFunctionSetTagValueByName(new ExportLib.ClassExportLib2.DelegateSetTagValueByName(SetTagValueByName));
//                    socketThread.obj.ProtocolInit(this.protocol_option);
//                }

//                TimeOutClass timeout_nosignal = new TimeOutClass();

//                byte[] recv_data = new byte[10000];

//                while(!m_Done) 
//                {
//                    Thread.Sleep(1);
//                    if(timeout_nosignal.IsTimeOut(this.nDisconnectTimeOutOnNoSignal))	break;
//                    if(!socket.Connected)	break;
//                    count = socket.Available;
//                    if(count == 0)	continue;

//                    timeout_nosignal.Reset();

//                    if(count > 10000)	count = 10000;

//                    try
//                    {
//                        count = socket.Receive(recv_data);
//                    }
//                    catch
//                    {
//                        count = 0;
//                    }

//                    socketThread.AddRecvCode(recv_data, count);
//                    socketThread.obj.ProtocolRecvBytes(recv_data, count);
//                }

//                socketThread.obj.ProtocolUnInit();
//#if USE_EXCEPTION_REPORT				
//            }
//            catch (Exception exception)
//            {
//                ExceptionReport.FormExceptionReport.Go(exception);
//            }
//#endif

//            load_fail:

//            lock (syncLock)
//            {
//                arraySocketThread.Remove(socketThread);
//            }
//            socket.Close();
//        }


        /*
		bool bUsingArray = false;
		public void SetWait()
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

		public void ResetWait()
		{
			bUsingArray = false;
		}*/

        //ㅡㅡㅡㅡㅡ로깅 추가 20241209 PSU ////
        public void Init()
        {
            try
            {
                m_Done = false;
                m_bEnded = false;

                IPAddress ipAddress = IPAddress.Parse("0.0.0.0");
                m_Listener = new TcpListener(ipAddress, port);

                try
                {
                    m_Listener.Start();
                    WriteLog("Server Start", null);
                }
                catch (Exception ex)
                {
                    string errorMsg = string.Format("Port {0} Failed to start: {1}", port, ex.Message);
                    WriteLogError(errorMsg, null);

                    //20241216 PSU
                    try
                    {
                        if(bUseStatusTag) AutoLibLocal.SharedTag.SetCurr(statusTag, -1);
                    }
                    catch { }

                    if (m_Listener != null)
                    {
                        m_Listener.Stop();
                        m_Listener = null;
                    }
                    return;
                }

                if (m_Listener != null)
                {
                    threadListener = new Thread(new ThreadStart(Listener));
                    threadListener.Start();
                }
            }
            catch (Exception ex)
            {
                WriteLogError("Error initializing server: " + ex.Message, null);
                //20241216 PSU
                try
                {
                    if(bUseStatusTag) AutoLibLocal.SharedTag.SetCurr(statusTag, -1);
                }
                catch { }
            }
        }

        public void UnInit()
        {
            isShuttingDown = true;  // 종료 시작 표시
            WriteLog("Server Shutdown Request", null);

            //20241216 PSU
            // 연결된 모든 소켓의 통신 프레임 정보 로깅
            if (arraySocketThread.Count > 0)
            {
                foreach (SocketThreadClass socketThread in arraySocketThread)
                {
                    try
                    {
                        Socket socket = socketThread.socket;
                        WriteLog(string.Format("Disconnect during Server Shutdown - Communication frames (Sent:{0}, Received:{1})",
                            socketThread.nSendFrame,
                            socketThread.nRecvFrame),
                            socket);
                    }
                    catch { }

                }
            }

            try
            {
                if (bUseStatusTag) AutoLibLocal.SharedTag.SetCurr(statusTag, 0);
            }
            catch { }

            m_Done = true;

            if (threadListener != null)
            {
                TcpClient s = new TcpClient();
                try
                {
                    s.Connect("127.0.0.1", port);
                }
                catch
                {

                }

                TimeOutClass timeout = new TimeOutClass();

                while (!m_bEnded)
                {
                    Thread.Sleep(1);	// wait thread ended
                    if (timeout.IsTimeOut(5)) break;
                }

                threadListener.Join();
            }

            threadListener = null;
            isShuttingDown = false;  // 종료 완료 표시

            //20241216 PSU
            WriteLog("Server Shutdown Done", null);
        }

        public ArrayList arraySocketThread = new ArrayList();
        SocketThreadClass tempSocketThread;

        public void Listener()
        {

            while (!m_Done)
            {
                Thread.Sleep(1);

                Socket socket = m_Listener.AcceptSocket();

                // if(m_Done)	break;	// 프로그램을 종료하기 위해서 끝냈다.

                tempSocketThread = new SocketThreadClass();
                tempSocketThread.socket = socket;
                //tempSocketThread.bClose = false;
                tempSocketThread.thread = new Thread(new ThreadStart(ThreadSocket));
                tempSocketThread.thread.Start();
            }
            m_Listener.Stop();

            /*
            SocketThreadClass st;
            for(int i = 0; i < arraySocketThread.Count; i++) 
            {
                st = (SocketThreadClass)arraySocketThread[i];
                st.bClose = true;
            }
            */

            while (true)
            {
                Thread.Sleep(1);
                if (arraySocketThread.Count == 0) break;
            }

            m_bEnded = true;

            return;
        }


        public object syncLock = new object();


        void ThreadSocket()
        {
            SocketThreadClass socketThread = tempSocketThread;
            Socket socket = socketThread.socket;
            string clientInfo = "";

            try
            {
                lock (syncLock)
                {
                    arraySocketThread.Add(socketThread);
                }

                IPEndPoint remoteEndPoint = (IPEndPoint)socket.RemoteEndPoint;
                clientInfo = remoteEndPoint.Address.ToString() + ":" + remoteEndPoint.Port.ToString();

                // 종료 중 더미 연결은 로깅하지 않음
                if (!isShuttingDown)
                {
                    WriteLog("New client connected", socket);
                }

                // 프로토콜 로드 및 초기화
                socketThread.obj = DriverList.LoadDriver(this.protocol);

                if (socketThread.obj != null)
                {
                    socketThread.obj.SetFunctionSend(new ExportLib.ClassExportLib2.DelegateSendBytes(socketThread.Send));
                    socketThread.obj.SetFunctionGetTagValue(new ExportLib.ClassExportLib2.DelegateGetTagValue(GetTagValue));
                    socketThread.obj.SetFunctionSetTagValue(new ExportLib.ClassExportLib2.DelegateSetTagValue(SetTagValue));
                    socketThread.obj.SetFunctionGetTagValueByName(new ExportLib.ClassExportLib2.DelegateGetTagValueByName(GetTagValueByName));
                    socketThread.obj.SetFunctionSetTagValueByName(new ExportLib.ClassExportLib2.DelegateSetTagValueByName(SetTagValueByName));
                    socketThread.obj.ProtocolInit(this.protocol_option);

                    if (!isShuttingDown)
                    {
                        WriteLog("Protocol initialization successful", socket);
                        //20241216
                        try
                        {
                            if (bUseStatusTag) AutoLibLocal.SharedTag.SetCurr(statusTag, arraySocketThread.Count);
                        }
                        catch { }
                    }
                }
                else
                {
                    if (!isShuttingDown)
                    {
                        WriteLog("Protocol initialization failed", socket);
                        //20241216
                        try
                        {
                            if (bUseStatusTag) AutoLibLocal.SharedTag.SetCurr(statusTag, -1);
                        }
                        catch { }
                    }
                    return;
                }

                TimeOutClass timeout_nosignal = new TimeOutClass();
                byte[] recv_data = new byte[10000];
                int count;

                while (!m_Done)
                {
                    Thread.Sleep(1);
                    if (timeout_nosignal.IsTimeOut(this.nDisconnectTimeOutOnNoSignal))
                    { 
                        //WriteLog("Connection terminated due to no response (Timeout: " + nDisconnectTimeOutOnNoSignal.ToString() + "seconds)", socket);
                        //20241216 PSU
                        WriteLog(string.Format("Connection terminated - no response (Timeout: {0}seconds), Communication frames - Sent:{1}, Recieved:{2}",
                               nDisconnectTimeOutOnNoSignal,
                               socketThread.nSendFrame,
                               socketThread.nRecvFrame), socket);

                        break;
                    }
                    //if (!socket.Connected) break;
                    //20241216 PSU
                    if (!socket.Connected)
                    {
                        WriteLog(string.Format("Connection terminated - socket disconnected, Communication frames - Sent:{0}, Recieved:{1}",
                            socketThread.nSendFrame,
                            socketThread.nRecvFrame), socket);
                       
                        break;
                    }


                    count = socket.Available;
                    if (count == 0) continue;

                    timeout_nosignal.Reset();

                    if (count > 10000) count = 10000;

                    try
                    {
                        count = socket.Receive(recv_data);
                    }
                    catch
                    {
                        count = 0;
                    }

                    socketThread.AddRecvCode(recv_data, count);
                    socketThread.obj.ProtocolRecvBytes(recv_data, count);
                }

                //20241216
                //if (!isShuttingDown)  // 종료 중이 아닐 때만 로그
                //{
                //    WriteLog("Client connection closed (Communication frames - Sent:" +
                //        socketThread.nSendFrame.ToString() + ", Received:" +
                //        socketThread.nRecvFrame.ToString() + ")", socket);
                //}

            }
            catch (Exception ex)
            {
                if (!isShuttingDown)  // 종료 중이 아닐 때만 로그
                {
                    WriteLog("Communication error: " + ex.Message, socket);
                    //20241216 PSU
                    try
                    {
                        if (bUseStatusTag) AutoLibLocal.SharedTag.SetCurr(statusTag, -1);
                    }
                    catch { }
                }
            }
            finally
            {
                if (socketThread.obj != null)
                {
                    socketThread.obj.ProtocolUnInit();
                }

                lock (syncLock)
                {
                    arraySocketThread.Remove(socketThread);
                }
                socket.Close();

                //20241216 PSU
                if (!isShuttingDown)  // 서버 종료 중이 아닐 때만 로그 및 상태 업데이트
                {
                    // 연결 종료 후 남은 연결 수 로깅
                    WriteLog("Socket Close", socket);
                    // 상태 태그 업데이트
                    try
                    {
                        if (bUseStatusTag) AutoLibLocal.SharedTag.SetCurr(statusTag, arraySocketThread.Count);
                    }
                    catch { }
                }
            }
        }

        //ㅡㅡㅡㅡㅡ로깅 추가 20241209 PSU ////




		Random rand = new Random();

		object GetTagValue(int address)
		{
			if(this.bUseTestValue) 
			{
				return rand.Next();
			}

			ExportTag tag;
			for(int i = 0; i < this.arrayTag.Count; i++) 
			{
				tag = (ExportTag)this.arrayTag[i];

				if(tag.address == address)
				{
					string val="";
					
					if(AutoLibLocal.SharedTag.GetCurr(tag.tag, ref val)) 
					{
						return val;						
					}
					return 0;
				}
			}

			return 0;
		}

		bool SetTagValue(int address, object val)
		{
			ExportTag tag;
			for(int i = 0; i < this.arrayTag.Count; i++) 
			{
				tag = (ExportTag)this.arrayTag[i];

				if(tag.address == address) 
				{
					if(val.GetType() == typeof(string))
						return AutoLibLocal.SharedTag.SetCurr(tag.tag, (string)val); 
					else if(val.GetType() == typeof(float))
						return AutoLibLocal.SharedTag.SetCurr(tag.tag, (float)val); 
					else if(val.GetType() == typeof(double))
						return AutoLibLocal.SharedTag.SetCurr(tag.tag, (double)val); 
					else if(val.GetType() == typeof(int))
						return AutoLibLocal.SharedTag.SetCurr(tag.tag, (int)val); 
					else if(val.GetType() == typeof(uint))
						return AutoLibLocal.SharedTag.SetCurr(tag.tag, (uint)val); 
					else if(val.GetType() == typeof(long))
						return AutoLibLocal.SharedTag.SetCurr(tag.tag, (long)val); 
					else if(val.GetType() == typeof(ulong))
						return AutoLibLocal.SharedTag.SetCurr(tag.tag, (ulong)val); 
					else
						return false;
				}
			}

			return false;
		}


		object GetTagValueByName(string tag)
		{
			if(this.bUseTestValue) 
			{
				return rand.Next();
			}

			string val="";
			
			if(AutoLibLocal.SharedTag.GetCurr(tag, ref val)) 
			{
				return val;						
			}
			return 0;
		}

		bool SetTagValueByName(string tag, object val)
		{
			if(val.GetType() == typeof(string))
				return AutoLibLocal.SharedTag.SetCurr(tag, (string)val); 
			else if(val.GetType() == typeof(float))
				return AutoLibLocal.SharedTag.SetCurr(tag, (float)val); 
			else if(val.GetType() == typeof(double))
				return AutoLibLocal.SharedTag.SetCurr(tag, (double)val); 
			else if(val.GetType() == typeof(int))
				return AutoLibLocal.SharedTag.SetCurr(tag, (int)val); 
			else if(val.GetType() == typeof(uint))
				return AutoLibLocal.SharedTag.SetCurr(tag, (uint)val); 
			else if(val.GetType() == typeof(long))
				return AutoLibLocal.SharedTag.SetCurr(tag, (long)val); 
			else if(val.GetType() == typeof(ulong))
				return AutoLibLocal.SharedTag.SetCurr(tag, (ulong)val); 
			else
				return false;
		}
	}

	public class ExportTag
	{
		public string tag;
		public int	  address;
	}

	public class SocketThreadClass
	{
		public Thread thread;
		public Socket socket;
		public DateTime tStart = DateTime.Now;
		public ulong nSendFrame = 0;
		public ulong nRecvFrame = 0;
		
		public ExportLib.ClassExportLib2 obj;

		public void Send(byte[] buf, int size)
		{
            try
            {
                socket.Send(buf, size, SocketFlags.None);
            }
            catch
            {

            }

            AddSendCode(buf, size);
		}

		const int MAX_RING = 10000;
		ushort[] ringBuf = new ushort[MAX_RING];
		int nTargetPos = 0;
		int nCurrentPos = 0;
        
		void AddCode(ushort code)
		{
			int next = nTargetPos+1;
			next %= MAX_RING;

			ringBuf[next] = code;
			nTargetPos = next;
		}

		public bool GetCode(ref ushort code)
		{
			if(nCurrentPos == nTargetPos)	return false;
			nCurrentPos += 1;
			nCurrentPos %= MAX_RING;
			code = ringBuf[nCurrentPos];
			return true;
		}

		public bool PeekCode(ref ushort code)
		{
			if(nCurrentPos == nTargetPos)	return false;
			int next = nCurrentPos+1;
			next %= MAX_RING;
			code = ringBuf[next];
			return true;
		}

		void AddSendCode(byte[] buf, int size)
		{
			nSendFrame++;
			for(int i = 0; i < size; i++) 
			{
				AddCode(buf[i]);
			}
		}

		public void AddRecvCode(byte[] buf, int size)
		{
			nRecvFrame++;
			for(int i = 0; i < size; i++) 
			{
				AddCode((ushort)(0x100|buf[i]));
			}
		}

		public bool IsExist() 
		{
			return (nCurrentPos != nTargetPos);
		}
	}
    
	public class ServerList
	{
		public static void Load()
		{
			string path = TotalConfig.sDirWorkProject+"\\ExportServer\\ExpportServer.lst";

			if(!File.Exists(path))	return;

			TextReader reader = new StreamReader(path);

			if(reader == null)	return;

			CommaTextReader comma = new CommaTextReader();
			string one_line;
			string buf = "";

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;
				comma.Set(one_line);
				comma.GetString(ref buf);

				if(buf == "Connection") 
				{
					ClassServer server = new ClassServer();
					ReadOneConnection(buf, reader, server);
					ExternData.serverList.Add(server);
				}
			}
				
			reader.Close();
		}

		static void ReadOneConnection(string command, TextReader reader, ClassServer server)
		{
			CommaTextReader comma = new CommaTextReader();
			string one_line;
			string buf = "";

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	return;
				comma.Set(one_line);
				comma.GetString(ref buf);

				if(buf == command) 
				{
					return;
				}
				else if(buf == "Port") 
				{
					comma.GetInt(ref server.port);
				}
				else if(buf == "Protocol") 
				{
					comma.GetString(ref server.protocol);
				}
				else if(buf == "ProtocolOption") 
				{
					comma.GetString(ref server.protocol_option);
				}
				else if(buf == "DisconnectTimeOutOnNoSignal")
				{
					comma.GetInt(ref server.nDisconnectTimeOutOnNoSignal);
					if(server.nDisconnectTimeOutOnNoSignal < 5)		server.nDisconnectTimeOutOnNoSignal = 10;
					if(server.nDisconnectTimeOutOnNoSignal > 3600)	server.nDisconnectTimeOutOnNoSignal = 10;
				}
                //20241216 PSU
                else if (buf == "StatusTag")
                {
                    server.bUseStatusTag = comma.GetBool();
                    server.statusTag = comma.GetString();
                }
				else if(buf == "Tag") 
				{
					ExportTag tag = new ExportTag();
					comma.GetString(ref tag.tag);
					comma.GetInt(ref tag.address);
					server.arrayTag.Add(tag);
				}
				else 
				{

				}
			}
		}

		public static void Save()
		{
			string path = TotalConfig.sDirWorkProject+"\\ExportServer";

			if(!Directory.Exists(path)) 
			{
				Directory.CreateDirectory(path);
			}

			path = TotalConfig.sDirWorkProject+"\\ExportServer\\ExpportServer.lst";

			CommaTextWriter writer = new CommaTextWriter(path);
			ClassServer server;

			if(writer == null) 
			{
				MessageBox.Show(path, "파일을 저장할 수 없습니다.");
				return;
			}

			for(int i = 0; i < ExternData.serverList.Count; i++) 
			{
				server = (ClassServer)ExternData.serverList[i];
				writer.WriteLine("Connection,Begin,");
				writer.WriteLine("\tPort,{0},",server.port);
				writer.WriteLine("\tProtocol,{0},", server.protocol);
				writer.WriteLine("\tProtocolOption,{0},", server.protocol_option);
				writer.WriteLine("\tDisconnectTimeOutOnNoSignal,{0},", server.nDisconnectTimeOutOnNoSignal);
                //20241216 PSU
                writer.WriteLine("StatusTag,{0},{1},", server.bUseStatusTag, server.statusTag);
				ExportTag tag;
				for(int j = 0; j < server.arrayTag.Count; j++) 
				{
					tag = (ExportTag)server.arrayTag[j];
					writer.WriteLine("\tTag,{0},{1},", tag.tag, tag.address);
				}
				writer.WriteLine("Connection,End,");
				
			}
			writer.Close();
		}

		public static void InitAll()
		{
			ClassServer server;

			for(int i = 0; i < ExternData.serverList.Count; i++) 
			{
				server = (ClassServer)ExternData.serverList[i];
				server.Init();
			}
		}

		public static void UnInitAll()
		{
			ClassServer server;

			for(int i = 0; i < ExternData.serverList.Count; i++) 
			{
				server = (ClassServer)ExternData.serverList[i];
				server.UnInit();
			}
		}
	}
}
