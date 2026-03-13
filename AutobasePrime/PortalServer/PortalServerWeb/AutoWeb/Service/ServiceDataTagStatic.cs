using AutoLibLocal;
using PortalServerWeb.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using NetTools;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PortalServerWeb.AutoWeb.Service
{
    public static class ServiceDataTagStatic
    {
        class ClientSocket
        {
            public string ip;
            public Socket socket;
        }

        static ArrayList arrayClient = new ArrayList();

        #region Connect Sockets

        static Socket ConnectSocketEvery(string server, int port)
        {
            Socket s = null;
            IPHostEntry hostEntry = Dns.GetHostByName(server);

            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ep = new IPEndPoint(address, port);
                Socket temp = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try { temp.Connect(ep); }
                catch { }

                if (temp.Connected)
                {
                    s = temp;
                    break;
                }
            }
            return s;
        }

        static ClientSocket ConnectSocketUseOldConnection(string server, int port, string clientIP)
        {
            foreach (ClientSocket cli in arrayClient)
            {
                if (cli.ip == clientIP && cli.socket != null)
                    return cli;
            }

            ClientSocket newCli = new ClientSocket();
            newCli.ip = clientIP;
            arrayClient.Add(newCli);

            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(server), port);
            newCli.socket = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try { newCli.socket.Connect(ep); }
            catch { newCli.socket = null; }

            return newCli;
        }

        #endregion

        #region Send & Receive

        public static bool SendAndGetData(HttpContext ctx, EnumMultiBlockCommand cmd, string data, out string recv)
        {
            bool isHigh = ConfigWeb.LocalVersionIs941OrHigher();
            return isHigh ?
                SendAndGetData941OrAbove(ctx, cmd, data, out recv) :
                SendAndGetData941Less(ctx, cmd, data, out recv);
        }

        public static bool SendAndGetData941OrAbove(HttpContext ctx, EnumMultiBlockCommand cmd, string data, out string recv)
        {
            recv = "";

            string ip = ConfigWeb.GetLocalIP();
            string port = ConfigWeb.GetLocalPort();
            string clientIP = ctx.Request.UserHostAddress;

            Socket socket;
            try { socket = ConnectSocketEvery(ip, Convert.ToInt32(port)); }
            catch { socket = null; }

            if (socket == null) return false;

            MultiBlockProtocolSend send = new MultiBlockProtocolSend();
            MultiBlockProtocolRecv rec = new MultiBlockProtocolRecv();

            send.ReadyBuf(cmd, data);

            bool next;
            int len;
            byte[] buf;

            while (true)
            {
                buf = send.GetBlock(MultiBlockProtocolSend.MakeTns(), out next, out len);
                try { socket.Send(buf, len, SocketFlags.None); }
                catch { socket.Close(); return false; }
                if (!next) break;
            }

            TimeOutClass timeout = new TimeOutClass();
            timeout.Reset();

            while (true)
            {
                if (timeout.IsTimeOut(10))
                {
                    socket.Close();
                    return false;
                }

                int count = socket.Available;
                if (count == 0)
                {
                    System.Threading.Thread.Sleep(1);
                    continue;
                }

                buf = new byte[count];
                try { count = socket.Receive(buf, 0, count, SocketFlags.None); }
                catch { socket.Close(); return false; }

                if (count > 0)
                {
                    if (!rec.SetBlock(buf, count, out next))
                    {
                        // ignore partial block
                    }
                    else if (!next)
                    {
                        recv = rec.Split();
                        break;
                    }
                }
            }

            socket.Close();
            return true;
        }

        public static bool SendAndGetData941Less(HttpContext ctx, EnumMultiBlockCommand cmd, string data, out string recv)
        {
            recv = "";

            string ip = ConfigWeb.GetLocalIP();
            string port = ConfigWeb.GetLocalPort();
            string clientIP = ctx.Request.UserHostAddress;

            ClientSocket cli = ConnectSocketUseOldConnection(ip, Convert.ToInt32(port), clientIP);
            if (cli?.socket == null) return false;

            MultiBlockProtocolSend send = new MultiBlockProtocolSend();
            MultiBlockProtocolRecv rec = new MultiBlockProtocolRecv();

            send.ReadyBuf(cmd, data);

            bool next;
            int len;
            byte[] buf;

            while (true)
            {
                buf = send.GetBlock(MultiBlockProtocolSend.MakeTns(), out next, out len);
                try { cli.socket.Send(buf, len, SocketFlags.None); }
                catch { cli.socket.Close(); arrayClient.Remove(cli); return false; }

                if (!next) break;
            }

            TimeOutClass timeout = new TimeOutClass();
            timeout.Reset();

            while (true)
            {
                if (timeout.IsTimeOut(10))
                {
                    cli.socket.Close();
                    arrayClient.Remove(cli);
                    return false;
                }

                int count = cli.socket.Available;
                if (count == 0) continue;

                buf = new byte[count];

                try { count = cli.socket.Receive(buf, 0, count, SocketFlags.None); }
                catch { cli.socket.Close(); arrayClient.Remove(cli); return false; }

                if (count > 0)
                {
                    if (!rec.SetBlock(buf, count, out next))
                    {
                        // ignore
                    }
                    else if (!next)
                    {
                        recv = rec.Split();
                        break;
                    }
                }
            }

            return true;
        }

        #endregion

        #region DataLocal wrappers (async)

        public static async Task<DataSet> GetDataAi(HttpContext ctx, string tag, int vType, int tType,
            int y, int m, int d, int h, int min, int count, int gab)
        {
            try
            {
                string workDir = ProjectLib.GetWorkDir(ctx.Request);
                TryLogIn(workDir);
                TotalConfig.sDirWorkProject = workDir;

                DataLocal local = new DataLocal();
                return await local.GetDataAi(tag, (EnumDataType)vType, (EnumDataTime)tType,
                                       y, m, d, h, min, count, gab);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("ServiceDataTagStatic.GetDataAi Exception: " + ex.ToString());
                return null;
            }

        }

        public static async Task<DataSet> GetDataDi(HttpContext ctx, string tag, int vType, int tType,
            int y, int m, int d, int h, int min, int count, int gab)
        {
            try
            {
                string workDir = ProjectLib.GetWorkDir(ctx.Request);
                TryLogIn(workDir);
                TotalConfig.sDirWorkProject = workDir;

                DataLocal local = new DataLocal();
                return await local.GetDataDi(tag, (EnumDataType)vType, (EnumDataTime)tType,
                                       y, m, d, h, min, count, gab);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ServiceDataTagStatic.GetDataDi Exception: " + ex.ToString());
                return null;
            }
        }

        public static void TryLogIn(string workDir)
        {
            TotalConfig.sWebServiceDataDirBasic = ConfigWeb.DataFolderBasic();
            TotalConfig.sWebServiceDataDirLog = ConfigWeb.DataFolderLog();

            if (!ConfigWeb.LogOnUse()) return;

            IntPtr tk = IntPtr.Zero;

            string username = ConfigWeb.LogOnUsername();
            string password = ConfigWeb.LogOnPassword();
            string domain = ConfigWeb.LogOnDomain();

            if (LogonUser(username, domain, password, 2, 0, ref tk))
                ImpersonateLoggedOnUser(tk);
        }

        [DllImport("advapi32.dll")] public static extern bool LogonUser(string username, string domain, string password, uint logontype, uint provider, ref IntPtr phToken);
        [DllImport("advapi32.dll")] public static extern bool ImpersonateLoggedOnUser(IntPtr phToken);

        #endregion
    }
}