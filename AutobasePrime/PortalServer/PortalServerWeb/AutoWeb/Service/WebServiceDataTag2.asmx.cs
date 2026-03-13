using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using NetTools;
using PortalServerWeb.Library;
using AutoLibLocal;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace PortalServerWeb.AutoWeb.Service
{
    /// <summary>
    /// Summary description for WebServiceDataTag2
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceDataTag2 : System.Web.Services.WebService
    {
        [WebMethod]
        public int CheckServiceAlive(int val)
        {
            return (~val);
        }

        /*    
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
                catch (Exception exception)
                {
                    int a = 1;
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

            string ip = WebConfig.GetLocalIP();// TotalConfig.LoadRegAutoBaseConfig("Project", TotalConfig.sDirWorkProject, "WebMainLocalIP", "127.0.0.1");
            string port = WebConfig.GetLocalPort();// TotalConfig.LoadRegAutoBaseConfig("Project", TotalConfig.sDirWorkProject, "WebMainLocalPort", "7200");

            string client_ip = webservice.Context.Request.UserHostAddress;

            Socket socket;

            try
            {
                socket = connectSocketEvery(ip, Convert.ToInt32(port));
            }
            catch(Exception exception)
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
                if (count == 0) continue;
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

            string ip = WebConfig.GetLocalIP();// TotalConfig.LoadRegAutoBaseConfig("Project", TotalConfig.sDirWorkProject, "WebMainLocalIP", "127.0.0.1");
            string port = WebConfig.GetLocalPort();// TotalConfig.LoadRegAutoBaseConfig("Project", TotalConfig.sDirWorkProject, "WebMainLocalPort", "7200");

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
            bool bLocalIs9_4_1orAbove = WebConfig.LocalVersionIs941OrHigher();// TotalConfig.LoadRegAutoBaseConfig("Project", TotalConfig.sDirWorkProject, "LocalVersionIs941OrHigher", true);

            if (bLocalIs9_4_1orAbove)
            {
                return SendAndGetData941OrAbove(webservice, command, data, out recv_data);
            }
            else
            {
                return SendAndGetData941Less(webservice, command, data, out recv_data);
            }
        }*/

        /*
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

           CommaBlockString comma = new CommaBlockString();
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
       */

        static XDocument GetTagValueListFromTcpXDocument(WebService webservice, System.Collections.Generic.List<string> array)
        {
            string buf = "";
            for (int i = 0; i < array.Count; i++)
            {
                buf += (string)array[i];
                buf += ",";
            }

            string recv_data;

            if (!ServiceDataTag.SendAndGetData(webservice, EnumMultiBlockCommand.TagValueList, buf, out recv_data))
                return null;

            XDocument ds = new XDocument();

            XElement root = new XElement("Root");
            ds.Add(root);

            XElement container;

            CommaBlockString comma = new CommaBlockString();
            comma.Set(recv_data);

            string tag = "";
            string val = "";
            for (int i = 0; i < array.Count; i++)
            {
                tag = (string)array[i];
                comma.GetString(ref val);

                container = new XElement("Values");
                container.Add(new XElement("Tag", tag));
                container.Add(new XElement("Curr", val));

                root.Add(container);
            }

            return ds;
        }

        /*
        [WebMethod(EnableSession = true)]
        public DataSet GetTagValueList(ArrayList array)
        {
            ServiceLib.SetCommonVars(this);

            return GetTagValueListFromTcp(this, array);
        }*/

        [WebMethod(EnableSession = true)]
        public string GetTagValueLists(System.Collections.Generic.List<string> array)
        {
            ServiceLib.SetCommonVars(this);

            XDocument doc = GetTagValueListFromTcpXDocument(this, array);

            if (doc == null) return null;

            else return doc.ToString();
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

            //string buf = String.Format("{0},{1},", tag, val);

            string recv_data;

            ServiceDataTag.SendAndGetData(this, EnumMultiBlockCommand.SetTagValue, buf, out recv_data);
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

            //string buf = String.Format("{0},{1},", tag, val);

            string recv_data;

            ServiceDataTag.SendAndGetData(this, EnumMultiBlockCommand.SetTagValue, buf, out recv_data);
        }

        [WebMethod]
        public string GetDataAi(string tag, int value_type, int data_time, int year, int mon, int day, int hour, int min, int data_count, int data_gab)
        {
            ServiceDataTag service = new ServiceDataTag();

            DataSet ds = service.GetDataAi(tag, value_type, data_time, year, mon, day, hour, min, data_count, data_gab);

            return ds.GetXml();

        }

        [WebMethod]
        public string GetDataDi(string tag, int value_type, int data_time, int year, int mon, int day, int hour, int min, int data_count, int data_gab)
        {

            ServiceDataTag service = new ServiceDataTag();

            DataSet ds = service.GetDataDi(tag, value_type, data_time, year, mon, day, hour, min, data_count, data_gab);

            return ds.GetXml();
        }

        [WebMethod]
        public string GetLogLists()
        {
            ServiceDataTag service = new ServiceDataTag();

            DataSet ds = service.GetLogLists();

            return ds.GetXml();
        }

        [WebMethod]
        public string GetLogFile(string name)
        {
            ServiceDataTag service = new ServiceDataTag();

            DataSet ds = service.GetLogFile(name);

            return ds.GetXml();
        }

        [WebMethod]
        public string GetAlarmLists()
        {
            ServiceDataTag service = new ServiceDataTag();

            DataSet ds = service.GetAlarmLists();

            return ds.GetXml();
        }

        [WebMethod]
        public string GetAlarmFile(string name)
        {
            ServiceDataTag service = new ServiceDataTag();

            DataSet ds = service.GetAlarmFile(name);

            return ds.GetXml();
        }

        [WebMethod]
        public string GetAlarmFileByScript2(DateTime tFrom, DateTime tTo, string option)
        {
            ServiceDataTag service = new ServiceDataTag();

            DataSet ds = service.GetAlarmFileByScript2(tFrom, tTo, option);

            return ds.GetXml();
        }

        public static Version ServerVersion()
        {
            // 10.2.7.6 - 경보 이벤트도 지원
            // 10.2.9.11 -  GetServerInformations() 지원. TimeZone 항목 추가

            // 10.3.1.6 - SecurityLevel3 지원 - WebService3.asmx 에 있음. 일단 제어에 관련되 치명적인 것 부터 지원한다.
            //            이때부터 서버가 SecurityLevel을 요청하면 클라이언트는 그에 맞는 Level 을 사용하여 요청해야 한다. 그렇지 않으면 통신이 되지 않는다.
            // 10.3.1.9 - ExcelReport와 Report 를 사용하기전에 GlobalSetVar를 사용하면 세션이 끊어져서 데이터가 안보이는 경우가 있는데
            //            세션을 사용하지 않고 변수를 모아서 가져가는 형식으로 변경하였다.  
            // 10.3.2.1 - 10.3.1.9 에 지원한 기능이 잘 안되어서 다시 수정했다.

            // 10.3.2.5 -   1) 이후부터 새로 추가되는 함수는 모두 CommonMethod를 통하여 지원된다.
            //              2) MilliDataTrend 지원

            // 10.3.2.6 -   1) MilliDataGetGroupLists 지원
            //              2) MilliDataGetFileLists 지원
            //              3) MilliDataGetOneFile 지원

            // 10.3.6.3 -   1) 웹클라이언트에서 DataGetAiYear, DataGetDiYear 가 적용되지 않는 문제점 수정

            // 10.3.7.5 -   1) 동시접속자 수 제한 및 체험판 모드(guid, heartbeat) 추가.

            return new Version(10, 3, 7, 5);
        }

        // 이 함수는 10.2 부터 지원되었다.
        [WebMethod]
        public string GetServerVersion()
        {
            Version version = ServerVersion();

            return version.ToString();
        }

        // 이 함수는 10.2.9.11 부터 지원되었다.
        [WebMethod]
        public string GetServerInformations()
        {
            StringBuilder sb = new StringBuilder();

            TimeZoneInfo tzi = TimeZoneInfo.Local;
            sb.Append(String.Format("TimeZoneSeconds={0},", tzi.BaseUtcOffset.TotalSeconds)); // 초로 환산해서 보낸다. 서울 3600*9 = 32400
            sb.Append(String.Format("TimeZoneID={0},", tzi.Id));                              // TimeZone 고유 ID
            sb.Append(String.Format("SecurityLevel={0},", ConfigWeb.SecurityLevel()));        // 10.3.1.6 추가. SecurityLevel
            return sb.ToString();
        }

        // 이 함수는 10.2.1 부터 지원되었다. 모든 출력을 할 때는 아래 함수를 사용하도록 한다.
        // 아무곳에서 출력할 수 없도록 Hash해서 사용한다.
        [WebMethod(Description = "WriteCurr Session", EnableSession = true)]
        public void WriteCurr(string username, string computername, string tag, string val, byte[] hash)
        {
            string err_msg;
            if (!ConfigWeb.CheckBelowSecurityLevel3(out err_msg))
            {
                return;
            }

            byte[] hash_s = WebServiceAndroid.StringToBytes("WriteCurr" + username + computername + tag + val);

            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] result = sha.ComputeHash(hash_s);

            bool match = result.SequenceEqual(hash);

            if (!match) return;

            string work_dir = ProjectLib.GetWorkDir(Context.Request);
            TotalConfig.sDirWorkProject = work_dir;
            string clientip = Context.Request.UserHostAddress;

            //string buf = String.Format("{0},{1},{2},{3},{4}", tag, val, username, clientip, computername); 
            // ,가 들어간 문자열은 안되어서 2016-12-21 수정 CommaTextMaker 로 변경했다.
            // RunMain은 이미 CommaTextReader 를 사용한다. 호환성은 문제 없을 듯...
            CommaTextMaker ctm = new CommaTextMaker();
            ctm.Write("{0},{1},{2},{3},{4}", tag, val, username, clientip, computername);
            string buf = ctm.GetResult();

            string recv_data;

            ServiceDataTag.SendAndGetData(this, EnumMultiBlockCommand.SetTagValue, buf, out recv_data);
        }
    }
}
