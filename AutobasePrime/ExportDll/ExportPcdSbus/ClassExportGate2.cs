using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using System.Net.Sockets;
using ExportLib;

namespace ExportPcdSbus
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	/// 
	public class ClassExportGate2 : ExportLib.ClassExportLib2
	{
		byte		cStation = 0;
		byte[]		commRecvBuf = new byte[5000];
		int			commCountCurr = 0;
		int			MAX_SEND_TAG_COUNT = 10000; 

		public override string ProtocolGetName()
		{
			return "PCD S-BUS";
		}
		
		void getInitOptionData(string option)
		{
			if(option.Length <= 0) return;

			CommaBlockString comma = new CommaBlockString();

			comma.Set(option);				
			comma.GetBYTE(ref cStation);
		}
		
		public override void ProtocolInit(string option)
		{			
			getInitOptionData(option);			
		}

		public override void ProtocolUnInit()
		{
			
		}

		void checkStartStation()
		{
			int lenth = commCountCurr;

			while(true) 
			{
				if(commCountCurr < 9)	return;

				if( commRecvBuf[0] == 0xB5 &&
					commRecvBuf[1] == 0x00 &&
					commRecvBuf[2] == cStation) 
				{
					return;		
				} 
					
				commCountCurr--;
				for(int i = 0; i < commCountCurr;i++) 
				{
					commRecvBuf[i] = commRecvBuf[i+1];
				}
			}
		}

		bool IsOnePacketCode(out int command, out int size, out int address)
		{
			command = 0;
			size = 0;
			address = 0;

			if(commCountCurr < 9)	return false;
			
			// 0 = 0xB5
			// 1 = 0x00
			// 2 = Station
			// 3 = Type
			// 4 = size
			// 5 = address hi
			// 6 = address lo
			// 7 = crc hi
			// 8 = crc lo
			// 2~8까지는 dle적용 B5는 C5+00, C5=C5+01

			if(commCountCurr >= 9) 
			{
				switch(commRecvBuf[3]) 
				{
					case 0x03:		// I
					case 0x05:		// O
					case 0x02:		// F
						command = commRecvBuf[3];
						size = (commRecvBuf[4]+1)/16;
						address = commRecvBuf[5]%256+commRecvBuf[6];
						commCountCurr = 0;
						return true;
					case 0x06:		// R
					case 0x07:		// T
					case 0x00:		// C
						command = commRecvBuf[3];
						size = (commRecvBuf[4]+1);
						address = commRecvBuf[5]%256+commRecvBuf[6];
						commCountCurr = 0;
						return true;

					default : 	break;
				}
			}

			commCountCurr = 0;

			return false;
		}

		void floatDataToByte(ref byte[] data, float val)
		{
			MemoryStream stream = new MemoryStream(data);
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(val);
			writer.Close();
		}

		void dwordDataToFloat(ref float fVal, uint val)
		{
			byte[] imsi = new Byte[4];
			
			MemoryStream stream = new MemoryStream(imsi);
			imsi[0] = (byte)(val & 0xFF);
			imsi[1] = (byte)((val >> 8) & 0xFF);
			imsi[2] = (byte)((val >> 16) & 0xFF);
			imsi[3] = (byte)((val >> 24) & 0xFF);
			BinaryReader reader = new BinaryReader(stream);
			try 
			{
				fVal = reader.ReadSingle();
			}
			catch 
			{
				fVal = 0;
			}
			reader.Close();			
		}

		static void AddCode(byte[] commSendBuf, byte code, ref int buf_pos)
		{
			if(code == 0xB5) 
			{
				commSendBuf[buf_pos++] = 0xC5;
				commSendBuf[buf_pos++] = 0x00;
			}
			else if(code == 0xC5) 
			{
				commSendBuf[buf_pos++] = 0xC5;
				commSendBuf[buf_pos++] = 0x01;
			}
			else 
			{
				commSendBuf[buf_pos++] = code;	
			}
		}

		void MakeAndSendWantedData(int req_command, int req_address, int req_size)
		{
			int			i, pos, buf_pos = 0;
			ushort		crc, val;
			uint		ival;
			//float		fVal;
			byte[]		imsi = new byte[50];
			byte[]		commSendBuf = new byte[5000];

			if(req_size <= 0) 
			{
				return;
			}

			commSendBuf[buf_pos++] = 0xB5;
			commSendBuf[buf_pos++] = 0x00;
		
			for(i = 0; i < req_size; i++) 
			{
				pos = (i+req_address) % MAX_SEND_TAG_COUNT;

				/*
				if(bFloatData) 
				{
					fVal = (float)GetTagValue(pos);
					floatDataToByte(ref imsi, fVal);
					commSendBuf[buf_pos++] = imsi[3];				// float
					commSendBuf[buf_pos++] = imsi[2];
					commSendBuf[buf_pos++] = imsi[1];
					commSendBuf[buf_pos++] = imsi[0];			
				}
				else 
				{
				*/

				if(req_command == 0x03 || req_command == 0x02 || req_command == 0x05) 
				{
					val = (ushort)GetTagValueDouble(pos);
					
					AddCode(commSendBuf, (byte)(val/256), ref buf_pos);
					AddCode(commSendBuf, (byte)(val%256), ref buf_pos);
				}
				else 
				{
					ival = (uint)GetTagValueDouble(pos);
					AddCode(commSendBuf, (byte)((ival >> 24) & 0xFF), ref buf_pos);
					AddCode(commSendBuf, (byte)((ival >> 18) & 0xFF), ref buf_pos);
					AddCode(commSendBuf, (byte)((ival >>  8) & 0xFF), ref buf_pos);
					AddCode(commSendBuf, (byte)((ival >>  0) & 0xFF), ref buf_pos);
				}
			}
			crc = NetTools.GetCRC_16_12_5_1.Calc(commSendBuf, 0, buf_pos);

			AddCode(commSendBuf, (byte)(crc/256), ref buf_pos);
			AddCode(commSendBuf, (byte)(crc%256), ref buf_pos);

			SendBytes(commSendBuf, buf_pos);
			commCountCurr = 0;
		}

		bool dle_flag = false;

		public override void ProtocolRecvBytes(byte[] buf, int size)
		{			
			for(int i = 0; i < size; i++) 
			{
				if(commCountCurr >= 5000) 
				{
					commCountCurr = 0;
					return;
				}

				// dle code를 걸러낸다.
				if(dle_flag) 
				{
					dle_flag = false;
					if(buf[i] == 0x00) 
					{
						commRecvBuf[commCountCurr++] = 0xB5;
					}
					else if(buf[i] == 0x01) 
					{
						commRecvBuf[commCountCurr++] = 0xC5;
					}
				}
				else 
				{
					if(buf[i] == 0xC5) 
					{
						dle_flag = true;
					}
					else 
					{
						commRecvBuf[commCountCurr++] = buf[i];
					}
				}
			}

			checkStartStation();

			int req_command;
			int req_size;
			int req_address;

			if(IsOnePacketCode(out req_command, out req_size, out req_address)) 
			{
				/*
				if(isWriteDataPacket()) 
				{
					saveToReceivedDataAndSendAck();
					return;
				}
				*/
				MakeAndSendWantedData(req_command, req_address, req_size);
			}
		}

		public override int ProtocolOption(ref string option)
		{
			FormProtocolOption form = new FormProtocolOption();

			getInitOptionData(option);			// 옵션 데이터를 다시 읽는다, 실제 통신에는 영향이 없다			
			form.numericUpDownStation.Value = cStation;

			if(form.ShowDialog() == DialogResult.OK) 
			{
				option = form.numericUpDownStation.Value.ToString();
				return 1;
			}
			return 0;
		}
	}
}
