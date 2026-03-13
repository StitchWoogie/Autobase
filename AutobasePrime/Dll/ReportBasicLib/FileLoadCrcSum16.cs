using System;
using System.IO;
using System.Drawing;

namespace ReportBasicLib
{
	/// <summary>
	/// Summary description for FileLoadCrcSum16.
	/// </summary>
	public class FileLoadCrcSum16
	{
		public ushort crc = 0;
		BinaryReader reader;

		public FileLoadCrcSum16(BinaryReader file)
		{
			//
			// TODO: Add constructor logic here
			//
			reader = file;
		}

		public byte[] ReadBytes(int size) 
		{
			byte[] data = reader.ReadBytes(size);
			for(int i = 0; i < data.Length; i++) 
			{
				crc += data[i];
			}
			return data;
		}

		/*
		public float ReadSingle()
		{
			float data = reader.ReadSingle();
			return data;
		}
		*/

		public sbyte ReadSByte()
		{
			sbyte data = reader.ReadSByte();
			crc += (byte)data;
			return data;
		}

		public byte ReadByte()
		{
			byte data = reader.ReadByte();
			crc += (byte)data;
			return data;
		}

		public ushort ReadUInt16()
		{
			ushort data = reader.ReadUInt16();
			crc += (byte)((data >> 0) & 0xFF);
			crc += (byte)((data >> 8) & 0xFF);
			return data;
		}

		public int ReadInt32()
		{
			int data = reader.ReadInt32();
			crc += (byte)((data >> 0) & 0xFF);
			crc += (byte)((data >> 8) & 0xFF);
			crc += (byte)((data >> 16) & 0xFF);
			crc += (byte)((data >> 24) & 0xFF);
			return data;
		}

		public uint ReadUInt32()
		{
			uint data = reader.ReadUInt32();
			crc += (byte)((data >> 0) & 0xFF);
			crc += (byte)((data >> 8) & 0xFF);
			crc += (byte)((data >> 16) & 0xFF);
			crc += (byte)((data >> 24) & 0xFF);
			return data;
		}

		public Color ReadColor()
		{
			uint data = reader.ReadUInt32();
			crc += (byte)((data >> 0) & 0xFF);
			crc += (byte)((data >> 8) & 0xFF);
			crc += (byte)((data >> 16) & 0xFF);
			crc += (byte)((data >> 24) & 0xFF);
            data |= 0xFF000000;
			return Color.FromArgb((int)data);
		}

		public float ReadSingle()
		{
			return reader.ReadSingle();
		}
		
	}
}
