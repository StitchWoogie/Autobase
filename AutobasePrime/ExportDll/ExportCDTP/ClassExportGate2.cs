using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using System.Net.Sockets;
using ExportLib;
using System.Text;

namespace ExportCDTP
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	/// 
	public class ClassExportGate2 : ExportLib.ClassExportLib2
	{
		byte[]		commRecvBuf = new byte[5000];
		byte[]		commSendBuf = new byte[5000];
		int			commCountCurr = 0;
		bool		bDleFlag = false;

		int			nRecvTns, nRecvStation, nRecvFunction;	// 
		
		/// <summary>
		/// Export 프로토콜의 이름을 반환한다
		/// </summary>
		/// <returns>프로토콜명</returns>
		public override string ProtocolGetName()
		{
			return "CDTP (Complex Data Transfer Protocol)";
		}

		void getInitOptionData(string option)
		{
			if(option.Length <= 0) return;

			CommaBlockString comma = new CommaBlockString();
			//byte			i = 0;

			comma.Set(option);				
			/*
			comma.GetBYTE(ref cStation);
			if(comma.IsEOS()) return;

			comma.GetBYTE(ref i);
			bFloatData = (i == 1) ? true : false;
			if(comma.IsEOS()) return;

			comma.GetBYTE(ref i);
			bReadValueFloat = (i == 1) ? true : false;			
			*/
		}
		
		public override void ProtocolInit(string option)
		{			
			getInitOptionData(option);			
		}

		public override void ProtocolUnInit()
		{
			
		}

		bool IsOnePacketCode()
		{
			if(commCountCurr < 8)	return false;

			if(commRecvBuf[0] != 0x10 || commRecvBuf[1] != 0x02) 
			{
				commCountCurr = 0;
				return false;
			}

			// 아직코드가 덜 들어온 것으로 판단.
			if(commRecvBuf[commCountCurr-2] != 0x10 || commRecvBuf[commCountCurr-1] != 0x03) 
			{
				return false;
			}

			int		nSize;

			nSize = commRecvBuf[2]+commRecvBuf[3]*256;

			if(nSize != commCountCurr-8)	// 사이즈가 틀리다.
			{
				commCountCurr = 0;
				return false;
			}

			ushort	crc = 0;

			crc = NetTools.GetCRC.SumWORD(commRecvBuf, 2, commCountCurr-6);

			if(crc != commRecvBuf[commCountCurr-4] + commRecvBuf[commCountCurr-3]*256) // crc 오류
			{
				commCountCurr = 0;
				return false;	
			}

			nRecvTns = commRecvBuf[4]+commRecvBuf[5]*256;
			nRecvStation = commRecvBuf[6];
			nRecvFunction = commRecvBuf[7];

			return true;
		}

		void AddCode(ref ushort crc, ref int count, byte ch, ref bool bOverFlow)
		{
			if(count+2 >= commSendBuf.Length) 
			{
				bOverFlow = true;
				count++;
				return;
			}

			commSendBuf[count++] = ch;

			crc += ch;

			if(ch == 0x10) 
			{
				commSendBuf[count++] = 0x10;
			}
		}

		void SendBuf(byte[] buf, int size, int function)
		{
			int count = 0;
			ushort crc = 0;
			bool bOverFlow = false;

			commSendBuf[count++] = 0x10;
			commSendBuf[count++] = 0x02;

			AddCode(ref crc, ref count, (byte)((size+4)%256), ref bOverFlow);
			AddCode(ref crc, ref count, (byte)((size+4)/256), ref bOverFlow);
			AddCode(ref crc, ref count, (byte)(nRecvTns%256), ref bOverFlow);
			AddCode(ref crc, ref count, (byte)(nRecvTns/256), ref bOverFlow);
			AddCode(ref crc, ref count, (byte)(nRecvStation), ref bOverFlow);
			AddCode(ref crc, ref count, (byte)(function),     ref bOverFlow);

			for(int i = 0; i < size; i++)
			{
				AddCode(ref crc, ref count, buf[i], ref bOverFlow);
			}

			byte low = (byte)(crc%256);
			byte high = (byte)(crc/256);

			AddCode(ref crc, ref count, (byte)(low), ref bOverFlow);
			AddCode(ref crc, ref count, (byte)(high), ref bOverFlow);

			if(bOverFlow) 
			{
				string msg = String.Format("Requied Size is too big. (Required byte={0} > ready buf={1})", count, commSendBuf.Length);
				SendNAK(0, msg);
				return;
			}

			commSendBuf[count++] = 0x10;
			commSendBuf[count++] = 0x03;

			SendBytes(commSendBuf, count);
		}

		void SendACK()
		{
			SendBuf(null, 0, 0x06);
			commCountCurr = 0;
		}

		void SendNAK(ushort code, string msg)
		{
			MemoryStream stream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(stream);

			writer.Write((ushort)(code));
			writer.Write((ushort)(msg.Length*2));
			for(int j = 0; j < msg.Length; j++) 
			{
				writer.Write((ushort)msg[j]);
			}

			byte[] buf = stream.ToArray();
			
			SendBuf(buf, buf.Length, 0x15);

			writer.Close();

			commCountCurr = 0;
		}

		void ExecuteFunction1()
		{
			MemoryStream stream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(stream);

			int address = commRecvBuf[8]+commRecvBuf[9]*256;
			int size = commRecvBuf[10]+commRecvBuf[11]*256;
			object obj;

			for(int i = 0; i < size; i++) 
			{
				obj = GetTagValue(address+i);
				
				if(obj.GetType() == typeof(bool)) 
				{
					writer.Write((byte)1);
					writer.Write((ushort)1);		
					writer.Write((bool)obj);
				}
				else if(obj.GetType() == typeof(char)) 
				{
					writer.Write((byte)2);
					writer.Write((ushort)2);		
					writer.Write((char)obj);
				}
				else if(obj.GetType() == typeof(sbyte)) 
				{
					writer.Write((byte)3);
					writer.Write((ushort)1);
					writer.Write((sbyte)obj);
				}
				else if(obj.GetType() == typeof(byte)) 
				{
					writer.Write((byte)4);
					writer.Write((ushort)1);
					writer.Write((byte)obj);
				}
				else if(obj.GetType() == typeof(short)) 
				{
					writer.Write((byte)5);
					writer.Write((ushort)2);
					writer.Write((short)obj);
				}
				else if(obj.GetType() == typeof(ushort)) 
				{
					writer.Write((byte)6);
					writer.Write((ushort)2);
					writer.Write((ushort)obj);
				}
				else if(obj.GetType() == typeof(int)) 
				{
					writer.Write((byte)7);
					writer.Write((ushort)4);
					writer.Write((int)obj);
				}
				else if(obj.GetType() == typeof(System.UInt32)) 
				{
					writer.Write((byte)8);
					writer.Write((ushort)4);
					writer.Write((System.UInt32)obj);
				}
				else if(obj.GetType() == typeof(System.Int64)) 
				{
					writer.Write((byte)9);
					writer.Write((ushort)8);
					writer.Write((System.Int64)obj);
				}
				else if(obj.GetType() == typeof(System.UInt64)) 
				{
					writer.Write((byte)10);
					writer.Write((ushort)8);
					writer.Write((System.UInt64)obj);
				}
				else if(obj.GetType() == typeof(float)) 
				{
					writer.Write((byte)11);
					writer.Write((ushort)4);
					writer.Write((float)obj);
				}
				else if(obj.GetType() == typeof(double)) 
				{
					writer.Write((byte)12);
					writer.Write((ushort)8);
					writer.Write((int)obj);
				}
				else if(obj.GetType() == typeof(string)) 
				{
					writer.Write((byte)13);
					string str = (string)obj;
					writer.Write((ushort)(str.Length*2));
					for(int j = 0; j < str.Length; j++) 
					{
						writer.Write((ushort)str[j]);
					}
				}
				else 
				{
					writer.Write((byte)7);
					writer.Write((ushort)4);
					writer.Write((int)0);
				}
			}

			byte[] buf = stream.ToArray();

			SendBuf(buf, buf.Length, nRecvFunction);

			writer.Close();

			commCountCurr = 0;
		}

		double BytesToDouble(byte[] val, int pos)
		{
			MemoryStream stream = new MemoryStream(val, pos, 8);
			BinaryReader reader = new BinaryReader(stream);
			double fVal;

			try 
			{
				fVal = reader.ReadDouble();
			}
			catch 
			{
				fVal = 0;
			}
			reader.Close();			

			return fVal;
		}

        string BytesToString(byte[] val, int pos, int byte_size)
        {
            StringBuilder s = new StringBuilder();

            for (int i = 0; i < byte_size; i+=2)
            {
                s.Append((char)(val[pos + i] + val[pos + i + 1] * 256));
            }

            return s.ToString();
        }

		void ExecuteFunction2()
		{
			int address = commRecvBuf[8]+commRecvBuf[9]*256;
			int size = commRecvBuf[10]+commRecvBuf[11]*256;
			int type = commRecvBuf[12];
			int type_size = commRecvBuf[13]+commRecvBuf[14]*256;

			int curr = 12;
			for(int i = 0; i < size; i++)
			{
				type = commRecvBuf[curr];
				type_size = commRecvBuf[curr+1]+commRecvBuf[curr+2]*256;

				if(type == 12)	// double
				{
					double val = BytesToDouble(commRecvBuf, curr+3);
					SetTagValue(address+i, val);
					SendACK();
				}
                else if (type == 13)	// string
                {
                    string val = BytesToString(commRecvBuf, curr + 3, type_size);
                    SetTagValue(address + i, val);
                    SendACK();
                }
				else 
				{

				}

				curr+=type_size+3;
			}
			
			commCountCurr = 0;
		}

		public override void ProtocolRecvBytes(byte[] buf, int size)
		{			
			for(int i = 0; i < size; i++) 
			{
				if(bDleFlag) 
				{
					bDleFlag = false;

					if(buf[i] == 0x10) 
					{
						commRecvBuf[commCountCurr++] = 0x10;
					}
					else if(buf[i] == 0x02) // STX
					{
						commCountCurr = 0;
						commRecvBuf[commCountCurr++] = 0x10;
						commRecvBuf[commCountCurr++] = 0x02;
						
					}
					else if(buf[i] == 0x03) 
					{
						commRecvBuf[commCountCurr++] = 0x10;
						commRecvBuf[commCountCurr++] = 0x03;

						if(commRecvBuf[0] == 0x10 && commRecvBuf[1] == 0x02) // 시작과 끝이 맞다.
						{
							// 하나의 패킷이 완성되었으므로 남은 버퍼는 무시하도록한다.
							break;	
						}
					}
					else 
					{
						commRecvBuf[commCountCurr++] = 0x10;
						commRecvBuf[commCountCurr++] = buf[i];
					}
				}
				else 
				{
					if(buf[i] == 0x10) 
					{
						bDleFlag = true;
					}
					else 
					{
						if(commCountCurr < commRecvBuf.Length-3)	// buf overflow
						{
							commRecvBuf[commCountCurr++] = buf[i];							
						}
					}
				}
			}

			if(IsOnePacketCode()) 
			{
				switch(commRecvBuf[7]) 
				{
					case 1:	// Read By Address
						ExecuteFunction1();
						break;
					case 2:	// Write By Address
						ExecuteFunction2();
						break;
				}
			}
		}

		public override int ProtocolOption(ref string option)
		{
			FormProtocolOption form = new FormProtocolOption();

			getInitOptionData(option);			// 옵션 데이터를 다시 읽는다, 실제 통신에는 영향이 없다.

			//form.numericUpDownStation.Value = cStation;
			//form.checkBoxReadFloat.Checked = bFloatData;
			//form.checkBoxWriteFloat.Checked = bReadValueFloat;

			if(form.ShowDialog() == DialogResult.OK) 
			{
				/*
				int		i, j;
				i = (form.checkBoxReadFloat.Checked) ? 1 : 0;
				j = (form.checkBoxWriteFloat.Checked) ? 1 : 0;
				try 
				{
					option = form.numericUpDownStation.Value.ToString() + "," + i.ToString() + "," + j + ",";
					option += String.Format("{0},", form.checkBoxUseHeader.Checked ? 1:0);
				}
				catch
				{
				}
				*/
				return 1;
			}

			return 0;
		}
	}
}
