using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace OpcClient
{
	/// <summary>
	/// Summary description for basicTool.
	/// </summary>
	public class basicTool
	{
		public basicTool()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static uint GetMinSecMilliHap(int min, int second, int milli)
		{
			return (uint)min * 60000 + (uint)second * 1000 + (uint)milli;
		}

		public static void copyBufPrev(ref byte[] data, int pos)
		{
			for(int i = 0; i < pos-1; i++) data[i] = data[i+1];			
		}

		public static ushort GetCRC_SumWORD(byte[] buf, int length)
		{
			int i;
			ushort crc = 0;
	                
			for(i = 0; i < length; i++)	crc += (ushort)buf[i];
			return crc;
		}

		public static string byteDataToString(byte[] data, int buf_pos, int count)
		{
			string		buf="";
			System.Text.Decoder d = System.Text.Encoding.Default.GetDecoder();
			char[] chars = new char[count];
			int retn = d.GetChars(data, buf_pos, count, chars, 0);

			try 
			{
				for(int i = 0; i < count; i++) 
				{
					if(chars[i] == 0) break;
					buf += Char.ToString(chars[i]);//(char)data[buf_pos+i];
					
				}
			}
			catch
			{
			}
			return buf;
		}


		public static void floatDataToByte(ref byte[] data, float val)
		{
			byte[] imsi = new Byte[4];

			MemoryStream stream = new MemoryStream(data);
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(val);
			writer.Close();
		}

		public static void doubleDataToByte(ref byte[] data, double val)
		{
			byte[] imsi = new Byte[8];

			MemoryStream stream = new MemoryStream(data);
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(val);
			writer.Close();
		}

		public static double byteDataToFloat(byte[] data, int pos)
		{
			byte[] imsi = new byte[4];
			for(int i = 0; i < 4; i++) imsi[i] = data[pos+i];

			MemoryStream stream = new MemoryStream(imsi);
			BinaryReader reader = new BinaryReader(stream);
			float val = reader.ReadSingle();
			reader.Close();
			return val;
		}

		public static double byteDataToDouble(byte[] data, int pos)
		{
			byte[] imsi = new byte[8];
			for(int i = 0; i < 8; i++) imsi[i] = data[pos+i];

			MemoryStream stream = new MemoryStream(imsi);
			BinaryReader reader = new BinaryReader(stream);
			double val = reader.ReadDouble();
			reader.Close();
			return val;
		}

		



	}
}
