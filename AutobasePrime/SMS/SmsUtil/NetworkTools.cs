using System;
using System.IO;
using System.Net;
using System.Net.Sockets;


namespace SMS.SmsUtil
{
	/// <summary>
	/// Summary description for NetworkTools.
	/// </summary>
	public class NetworkTools
	{
		public NetworkTools()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static uint getReadCount(Socket s)
		{
			if(s == null) return 0;
			byte[] inValue = BitConverter.GetBytes(0);
			byte[] outValue = BitConverter.GetBytes(0);
			int retn = s.IOControl(0x4004667F, inValue, outValue);
			return BitConverter.ToUInt32(outValue, 0);
		}

		public static byte[] readUdpData(Socket s, ref EndPoint ep)
		{
			if(s == null || ep == null) return null;
			
			try 
			{
				if(s.Available <= 0) return null;
				byte[] buf = new byte[s.Available];
				int len = s.ReceiveFrom(buf, buf.Length, SocketFlags.None, ref ep);
				if(len != buf.Length) return null;
				return buf;
			}
			catch 
			{
				return null;
			}
		}

		public static int writeUdpData(byte ip1, byte ip2, byte ip3, byte ip4, ushort wPort, byte[] buf, int count)
		{
			Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			IPEndPoint iep = getUdpPortIpEndPoint(ip1, ip2, ip3, ip4, wPort);
			if(s == null || iep == null) return 0;
			
			int retn = s.SendTo(buf, 0, count, SocketFlags.None, iep);
			s.Close();
			return retn;
		}

		public static int writeUdpData(IPEndPoint iep, byte[] buf, int count)
		{
			Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			if(s == null || iep == null) return 0;
			
			int retn = s.SendTo(buf, 0, count, SocketFlags.None, iep);
			s.Close();
			return retn;
		}

		public static int writeUdpData(Socket s, IPEndPoint lep, byte[] buf, int count)
		{
			if(s == null || lep == null) return 0;
			return s.SendTo(buf, 0, count, SocketFlags.None, lep);
		}

		public static IPAddress getIpAddress(byte ip1, byte ip2, byte ip3, byte ip4)
		{
			uint		val = (uint)ip4 * 0x1000000 + (uint)ip3 * 0x10000 + (uint)ip2 * 0x100 + (uint)ip1;
			return new IPAddress(val);
		}

		public static IPEndPoint getUdpPortIpEndPoint(byte ip1, byte ip2, byte ip3, byte ip4, UInt16 wPort)
		{
			uint		addr = (uint)ip4 * 0x1000000 + (uint)ip3 * 0x10000 + (uint)ip2 * 0x100 + (uint)ip1;
			return new IPEndPoint(addr, wPort);
		}
		

	}
}
