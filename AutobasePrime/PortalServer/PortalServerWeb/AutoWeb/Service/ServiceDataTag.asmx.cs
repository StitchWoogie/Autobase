using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using AutoLibLocal;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using System.Runtime.InteropServices;
using NetTools;
using PortalServerWeb.Library;

namespace PortalServerWeb.AutoWeb.Service
{
    /// <summary>
    /// 이클래스는 버전이 계속 업그레이드 되어도 CheckServiceAlive 만은 남겨두도록 한다. 오토베이스 웹서버가 실행되는가를 체크하는 가장 간단한
    /// 루틴이다.
    /// 다른 함수는 보안을 강화할 때 삭제하거나 수정해도 CheckServiceAlive() 만은 남겨 두도록한다. 2017-5-24
    /// 
    /// asmx 는 async 지원이 안됨. .GetAwaiter().GetResult() 로 동기화하고 안되면 wcf로 모두 변경해야 할 듯. 20250926 PUS
    /// WCF 도 .NET 5+ 에서는 지원이 안됨.
    /// 
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ServiceDataTag : System.Web.Services.WebService
    {
        // 웹서비스가 실행되고 있는지를 알아보기 위해서 가장 간단하고 빠르게 체크되는 함수를
        // 하나 만들었다. 값을 반전시켜 보내준다.

        [WebMethod]
        public int CheckServiceAlive(int val)
        {
            return (~val);
        }



        // DNS 가 적용될 수 있도록 한다
        static Socket connectSocketEvery(string server, int port)
        {
            Socket s = null;
            IPHostEntry hostEntry = null;

            // Get host related information.

            //server = "192.168.1.10";

            hostEntry = Dns.GetHostByName(server);  // 일단 Warnning이 떠도 이것을 사용할 것 
            //hostEntry = Dns.GetHostEntry(server);  // 192.168.1.22 이런것도 잘안됨 v6도 함께 올라옴 이상함 Vista에서 이상

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host IP Address is not compatible with the address family
            // (typical in the IPv6 case).
            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket =
                    new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    tempSocket.Connect(ipe);
                }
                catch
                {
                    //int a = 1;
                }

                if (tempSocket.Connected)
                {
                    s = tempSocket;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return s;
        }

        public static bool SendAndGetData941OrAbove(WebService webservice, EnumMultiBlockCommand command, string data, out string recv_data)
        {
            recv_data = "";

            string ip = ConfigWeb.GetLocalIP();// TotalConfig.LoadRegAutoBaseConfig("Project", TotalConfig.sDirWorkProject, "WebMainLocalIP", "127.0.0.1");
            string port = ConfigWeb.GetLocalPort();// TotalConfig.LoadRegAutoBaseConfig("Project", TotalConfig.sDirWorkProject, "WebMainLocalPort", "7200");

            string client_ip = webservice.Context.Request.UserHostAddress;

            Socket socket;

            try
            {
                socket = connectSocketEvery(ip, Convert.ToInt32(port));
            }
            catch
            {
                socket = null;
            }

            if (socket == null) return false;

            int count;

            MultiBlockProtocolSend send = new MultiBlockProtocolSend();
            MultiBlockProtocolRecv recv = new MultiBlockProtocolRecv();

            bool next;
            int len;
            byte[] byte_data;
            TimeOutClass timeout = new TimeOutClass();

            send.ReadyBuf(command, data);
            while (true)
            {
                byte_data = send.GetBlock(MultiBlockProtocolSend.MakeTns(), out next, out len);
                try
                {
                    socket.Send(byte_data, len, SocketFlags.None);
                }
                catch
                {
                    socket.Close();
                    return false;
                }

                if (next == false) break;
            }

            timeout.Reset();
            while (true)
            {
                if (timeout.IsTimeOut(10))
                {
                    socket.Close();
                    return false;
                }
                count = socket.Available;
                if (count == 0)
                {
                    System.Threading.Thread.Sleep(1);   // 2009.8.19  while 루프일 때 기다리는 것이 중요하다.
                    continue;
                }
                byte_data = new byte[count];

                try
                {
                    count = socket.Receive(byte_data, 0, count, SocketFlags.None);
                }
                catch
                {
                    socket.Close();
                    return false;
                }

                if (count > 0)
                {
                    if (!recv.SetBlock(byte_data, count, out next))
                    {
                        //tcpc.Close();
                        //return false;
                    }
                    else
                    {
                        if (next == false)
                        {
                            recv_data = recv.Split();
                            break;
                        }
                    }
                }
            }

            socket.Close();
            return true;
        }


        // 
        class ClientSocket
        {
            public string ip;
            public Socket socket;
        }

        static ArrayList arrayClient = null;
        // 이전의 접속을 사용하는 스타일
        static ClientSocket connectSocketUseOldConnection(string server, int port, string client_ip)
        {
            if (arrayClient == null)
            {
                arrayClient = new ArrayList();
            }

            ClientSocket client;

            for (int i = 0; i < arrayClient.Count; i++)
            {
                client = (ClientSocket)arrayClient[i];
                if (client_ip == client.ip)
                {
                    if (client.socket != null) return client;
                    else
                    {
                        goto alloc;
                    }
                }
            }

            client = new ClientSocket();
            client.ip = client_ip;
            arrayClient.Add(client);

        alloc:
            IPAddress ipad = IPAddress.Parse(server);
            IPEndPoint ipe = new IPEndPoint(ipad, Convert.ToInt32(port));
            client.socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                client.socket.Connect(ipe);
            }
            catch
            {
                client.socket = null;
            }

            return client;
        }

        public static bool SendAndGetData941Less(WebService webservice, EnumMultiBlockCommand command, string data, out string recv_data)
        {
            recv_data = "";

            string ip = ConfigWeb.GetLocalIP();// TotalConfig.LoadRegAutoBaseConfig("Project", TotalConfig.sDirWorkProject, "WebMainLocalIP", "127.0.0.1");
            string port = ConfigWeb.GetLocalPort();// TotalConfig.LoadRegAutoBaseConfig("Project", TotalConfig.sDirWorkProject, "WebMainLocalPort", "7200");

            string client_ip = webservice.Context.Request.UserHostAddress;

            ClientSocket client = connectSocketUseOldConnection(ip, Convert.ToInt32(port), client_ip);

            if (client == null) return false;
            if (client.socket == null) return false;

            int count;

            MultiBlockProtocolSend send = new MultiBlockProtocolSend();
            MultiBlockProtocolRecv recv = new MultiBlockProtocolRecv();

            bool next;
            int len;
            byte[] byte_data;
            TimeOutClass timeout = new TimeOutClass();

            send.ReadyBuf(command, data);
            while (true)
            {
                byte_data = send.GetBlock(MultiBlockProtocolSend.MakeTns(), out next, out len);
                try
                {
                    client.socket.Send(byte_data, len, SocketFlags.None);
                }
                catch
                {
                    client.socket.Close();
                    arrayClient.Remove(client);
                    return false;
                }

                if (next == false) break;
            }

            timeout.Reset();
            while (true)
            {
                if (timeout.IsTimeOut(10))
                {
                    client.socket.Close();
                    arrayClient.Remove(client);
                    return false;
                }
                count = client.socket.Available;
                if (count == 0) continue;
                byte_data = new byte[count];

                try
                {
                    count = client.socket.Receive(byte_data, 0, count, SocketFlags.None);
                }
                catch
                {
                    client.socket.Close();
                    arrayClient.Remove(client);
                    return false;
                }

                if (count > 0)
                {
                    if (!recv.SetBlock(byte_data, count, out next))
                    {
                        //tcpc.Close();
                        //return false;
                    }
                    else
                    {
                        if (next == false)
                        {
                            recv_data = recv.Split();
                            break;
                        }
                    }
                }
            }


            return true;
        }

        public static bool SendAndGetData(WebService webservice, EnumMultiBlockCommand command, string data, out string recv_data)
        {
            //recv_data = "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20";
            //return true;

            bool bLocalIs9_4_1orAbove = ConfigWeb.LocalVersionIs941OrHigher();// TotalConfig.LoadRegAutoBaseConfig("Project", TotalConfig.sDirWorkProject, "LocalVersionIs941OrHigher", true);

            if (bLocalIs9_4_1orAbove)
            {
                return SendAndGetData941OrAbove(webservice, command, data, out recv_data);
            }
            else
            {
                return SendAndGetData941Less(webservice, command, data, out recv_data);
            }
        }

        static DataSet GetTagValueListFromTcp(WebService webservice, ArrayList array)
        {
            string buf = "";
            for (int i = 0; i < array.Count; i++)
            {
                buf += (string)array[i];
                buf += ",";
            }

            string recv_data;

            if (!SendAndGetData(webservice, EnumMultiBlockCommand.TagValueList, buf, out recv_data))
                return null;

            DataSet ds = new DataSet("TAG");
            DataTable dt = new DataTable("Table1");
            DataColumn dc;
            dc = new DataColumn("TAG");
            dc.MaxLength = 256;
            dt.Columns.Add(dc);
            dc = new DataColumn("Curr");
            dc.MaxLength = 256;
            dt.Columns.Add(dc);
            ds.Tables.Add(dt);

            //CommaBlockString comma = new CommaBlockString();
            CommaTextReader comma = new CommaTextReader();  // 2016-12-21 CommaBlockString에서 수정
            comma.Set(recv_data);

            string tag = "";
            DataRow row;
            string val = "";
            for (int i = 0; i < array.Count; i++)
            {
                tag = (string)array[i];
                comma.GetString(ref val);
                row = dt.NewRow();
                row[0] = tag;
                row[1] = val;
                dt.Rows.Add(row);
            }

            return ds;
        }

        [WebMethod(EnableSession = true)]
        public DataSet GetTagValueList(ArrayList array)
        {
            ServiceLib.SetCommonVars(this);

            return GetTagValueListFromTcp(this, array);
        }

        [WebMethod(Description = "WriteCurrAI Session", EnableSession = true)]
        public void WriteCurrAI(string tag, double val)
        {
            string err_msg;
            if (!ConfigWeb.CheckBelowSecurityLevel3(out err_msg))
            {
                return;
            }

            string work_dir = ProjectLib.GetWorkDir(Context.Request);
            TotalConfig.sDirWorkProject = work_dir;

            string clientip = Context.Request.UserHostAddress;

            // , 가 들어간 문자열은 안된다.  이전버전에서 사용하므로 10.2.1 이전까지만 사용하므로 그냥둔다.
            string buf = String.Format("{0},{1},{2},{3},{4}", tag, val, "WebService", clientip, "WebServer");

            string recv_data;

            SendAndGetData(this, EnumMultiBlockCommand.SetTagValue, buf, out recv_data);
        }

        [WebMethod(Description = "WriteCurrST Session", EnableSession = true)]
        public void WriteCurrST(string tag, string val)
        {
            string err_msg;
            if (!ConfigWeb.CheckBelowSecurityLevel3(out err_msg))
            {
                return;
            }

            string work_dir = ProjectLib.GetWorkDir(Context.Request);
            TotalConfig.sDirWorkProject = work_dir;

            string clientip = Context.Request.UserHostAddress;

            // , 가 들어간 문자열은 안된다.  이전버전에서 사용하므로 10.2.1 이전까지만 사용하므로 그냥둔다.
            string buf = String.Format("{0},{1},{2},{3},{4}", tag, val, "WebService", clientip, "WebServer");

            string recv_data;

            SendAndGetData(this, EnumMultiBlockCommand.SetTagValue, buf, out recv_data);
        }



    

        [DllImport("advapi32.dll")]
        public static extern bool LogonUser(string username, string domain, string password, uint logontype, uint provider, ref IntPtr phToken);

        [DllImport("advapi32.dll")]
        public static extern bool ImpersonateLoggedOnUser(IntPtr phToken);

        static byte[] security_code = new byte[10] { 1, 4, 8, 99, 123, 23, 11, 34, 55, 8 };

        // 매번 로그인을 해야 되는것 같다.
        public static void TryLogIn(string work_dir)
        {
            TotalConfig.sWebServiceDataDirBasic = ConfigWeb.DataFolderBasic();
            TotalConfig.sWebServiceDataDirLog = ConfigWeb.DataFolderLog();

            bool use = ConfigWeb.LogOnUse();

            if (!use) return;

            IntPtr tk = IntPtr.Zero;
            bool retn;

            string username = ConfigWeb.LogOnUsername();

            string password;

            password = ConfigWeb.LogOnPassword();

            string domain = ConfigWeb.LogOnDomain();// TotalConfig.LoadRegAutoBaseConfig("Project", work_dir, "LogOnDomain", "");

            if (LogonUser(username, domain, password, 2, 0, ref tk))
            {
                retn = ImpersonateLoggedOnUser(tk);
            }
        }

        [WebMethod]
        public DataSet GetDataAi(string tag, int value_type, int data_time, int year, int mon, int day, int hour, int min, int data_count, int data_gab)
        {
            string work_dir = ProjectLib.GetWorkDir(Context.Request);
            TryLogIn(work_dir);

            DataLocal local = new DataLocal();
            TotalConfig.sDirWorkProject = work_dir;
            return local.GetDataAi(tag, (EnumDataType)value_type, (EnumDataTime)data_time, year, mon, day, hour, min, data_count, data_gab).GetAwaiter().GetResult();
        }

        [WebMethod]
        public DataSet GetDataDi(string tag, int value_type, int data_time, int year, int mon, int day, int hour, int min, int data_count, int data_gab)
        {
            string work_dir = ProjectLib.GetWorkDir(Context.Request);
            TryLogIn(work_dir);

            DataLocal local = new DataLocal();
            TotalConfig.sDirWorkProject = work_dir;
            return local.GetDataDi(tag, (EnumDataType)value_type, (EnumDataTime)data_time, year, mon, day, hour, min, data_count, data_gab).GetAwaiter().GetResult();
        }

        [WebMethod]
        public DataSet GetLogLists()
        {
            string work_dir = ProjectLib.GetWorkDir(Context.Request);
            TryLogIn(work_dir);

            DataLocal local = new DataLocal();
            TotalConfig.sDirWorkProject = work_dir;
            return local.GetLogLists().GetAwaiter().GetResult();
        }

        [WebMethod]
        public DataSet GetLogFile(string name)
        {
            string work_dir = ProjectLib.GetWorkDir(Context.Request);
            TryLogIn(work_dir);

            DataLocal local = new DataLocal();
            TotalConfig.sDirWorkProject = work_dir;
            return local.GetLogFile(name).GetAwaiter().GetResult();
        }

        [WebMethod]
        public DataSet GetAlarmLists()
        {
            string work_dir = ProjectLib.GetWorkDir(Context.Request);
            TryLogIn(work_dir);

            DataLocal local = new DataLocal();
            TotalConfig.sDirWorkProject = work_dir;
            return local.GetAlarmLists();
        }

        [WebMethod]
        public DataSet GetAlarmFile(string name)
        {
            string work_dir = ProjectLib.GetWorkDir(Context.Request);
            TryLogIn(work_dir);

            DataLocal local = new DataLocal();
            TotalConfig.sDirWorkProject = work_dir;
            return local.GetAlarmFileAsync(name).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 9.5.1 까지 사용하는 서비스
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        [WebMethod]
        public DataSet GetAlarmFileByScript(int year, int month, int day, int option)
        {
            string work_dir = ProjectLib.GetWorkDir(Context.Request);
            TryLogIn(work_dir);

            DataLocal local = new DataLocal();
            TotalConfig.sDirWorkProject = work_dir;
            DateTime tFrom = new DateTime(year, month, day, 0, 0, 0);
            DateTime tTo = new DateTime(year, month, day, 23, 59, 59);
            return local.GetAlarmFileByScript(tFrom, tTo, "", false).GetAwaiter().GetResult() ;
        }

        /// <summary>
        /// 9.5.2 부터는 이것을 사용
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        [WebMethod]
        public DataSet GetAlarmFileByScript2(DateTime tFrom, DateTime tTo, string option)
        {
            string work_dir = ProjectLib.GetWorkDir(Context.Request);
            TryLogIn(work_dir);

            DataLocal local = new DataLocal();
            TotalConfig.sDirWorkProject = work_dir;
            return local.GetAlarmFileByScript(tFrom, tTo, option, false).GetAwaiter().GetResult();
        }
    }
}
