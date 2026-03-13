using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using System.Net.Sockets;
using ExportLib;

namespace ExportGlofaCnet
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	/// 
	public class ClassExportGate2 : ExportLib.ClassExportLib2
	{
		bool bUseStation = false;
		int nStation = 0;
		byte[]		commRecvBuf = new byte[MAX_BUF];
		byte[]		commSendBuf = new byte[MAX_BUF];
		int			commCountCurr = 0;
		const int MAX_BUF = 5000;

		public override string ProtocolGetName()
		{
			return "Glofa Cnet";
		}

		public override EnumCodeMode ProtocolGetCodeMode() 
		{
			return EnumCodeMode.ASCII;	// Ascii Mode
		}

		public override void ProtocolInit(string option)
		{
			CommaBlockString comma = new CommaBlockString();

			int imsi = 0;

			comma.Set(option);
			comma.GetInt(ref imsi);
			this.bUseStation = (imsi == 1);
			comma.GetInt(ref imsi);
			this.nStation = imsi;
		}


		enum ASCII
		{
			ETX = 3,
			EOT = 4,
			ENQ = 5,
			ACK = 6,
		}

		bool GetAddress(byte[] buf, int start, int size, ref int address, ref int rack, ref int slot)
		{
			if(size < 4)	return false;

			int pos = start;

			if(buf[pos] != (byte)'%')	return false;

			address = 0;
			int i;

			if(buf[pos+1] == 'M') 
			{
				for(i = 0, pos = start+3; i < size-3; i++, pos++) 
				{
					address *= 10;
					if(buf[pos] < '0' || buf[pos] > '9')	return false;
					address += buf[pos]-'0';
				}
			}
			else if(buf[pos+1] == 'I' || buf[pos+1] == 'Q') 
			{
				rack = 0;
				for(i = 0, pos = start+3; i < size-3; i++, pos++) 
				{
					if(buf[pos] == '.') 
					{
						i++;
						pos++;
						break;
					}
					rack *= 10;
					if(buf[pos] < '0' || buf[pos] > '9')	return false;
					rack += buf[pos]-'0';
				}
				slot = 0;
				for(; i < size-3; i++, pos++) 
				{
					
					if(buf[pos] == '.') 
					{
						pos++;
						break;
					}
					slot *= 10;
					if(buf[pos] < '0' || buf[pos] > '9')	return false;
					slot += buf[pos]-'0';
				}
				address = 0;
				for(; i < size-3; i++, pos++) 
				{
					address *= 10;
					if(buf[pos] < '0' || buf[pos] > '9')	return false;
					address += buf[pos]-'0';
				}
			}
			else 
			{
				return false;
			}

			return true;
		}

		void ExcuteReadCommand()
		{
			if(commRecvBuf[4] == 'S' && commRecvBuf[5] == 'B') 
			{
				byte address_size = HexBufToByte(commRecvBuf, 6);
				byte read_size = HexBufToByte(commRecvBuf, 8+address_size);
				int send_count;
				int  address = 0;
				byte crc;
				int rack = 0;
				int slot = 0;

				if(!GetAddress(commRecvBuf, 8, address_size, ref address, ref rack, ref slot))	return;

				commSendBuf[0] = (byte)ASCII.ACK;
				commSendBuf[1] = commRecvBuf[1];
				commSendBuf[2] = commRecvBuf[2];
				commSendBuf[3] = commRecvBuf[3];
				commSendBuf[4] = commRecvBuf[4];
				commSendBuf[5] = commRecvBuf[5];
				commSendBuf[6] = commRecvBuf[6];
				commSendBuf[7] = commRecvBuf[7];
				commSendBuf[8] = commRecvBuf[8];
				commSendBuf[9] = commRecvBuf[9];

				send_count = 10;

				string imsi;
				int data_size;
					
				for(int i = 0; i < read_size; i++) 
				{
					if(commRecvBuf[10] == 'X') 
					{
						imsi = String.Format("{0:X02}", (byte)GetTagValueDouble(address+i));
						data_size = 2;
					}
					else if(commRecvBuf[10] == 'W') 
					{
						imsi = String.Format("{0:X04}", (int)GetTagValueDouble(address+i));
						data_size = 4;
					}
					else if(commRecvBuf[10] == 'D') 
					{
						imsi = String.Format("{0:X08}", (int)GetTagValueDouble(address+i));
						data_size = 8;
					}
					else 
					{
						imsi = String.Format("{0:X04}", (int)GetTagValueDouble(address+i));
						data_size = 8;
					}

					if(commRecvBuf[9] != 'M') 
						imsi = "00000000";

					for(int j = 0; j < data_size; j++) 
					{
						commSendBuf[send_count++] = (byte)imsi[j];
					}
				}

				commSendBuf[send_count++] = (byte)ASCII.ETX;
				crc = GetCrc(commSendBuf, 0, send_count);
				imsi = String.Format("{0:X02}", crc);
				commSendBuf[send_count++] = (byte)(imsi[0]);
				commSendBuf[send_count++] = (byte)(imsi[1]);

				SendBytes(commSendBuf, send_count);
			}
		}

		void ExcuteWriteCommand()
		{
			if(commRecvBuf[4] == 'S' && commRecvBuf[5] == 'S') 
			{
				byte block_count = HexBufToByte(commRecvBuf, 6);
				
				byte address_size;
				int  address = 0;
				int rack = 0;
				int slot = 0;
				int pos = 8;
				int data_size;
				double val;

				pos = 8;

				for(int i = 0; i < block_count; i++) 
				{
					address_size = HexBufToByte(commRecvBuf, pos);
					if(!GetAddress(commRecvBuf, pos+2, address_size, ref address, ref rack, ref slot))	return;

					if(commRecvBuf[pos+4] == 'X')
						data_size = 2;
					else if(commRecvBuf[pos+4] == 'B')
						data_size = 2;
					else if(commRecvBuf[pos+4] == 'W')
						data_size = 4;
					else if(commRecvBuf[pos+4] == 'D')
						data_size = 8;
					else if(commRecvBuf[pos+4] == 'F')
						data_size = 8;
					else
						return;

					if(commRecvBuf[pos+3] == 'M') // M 영역만 Write
					{
						if(commRecvBuf[pos+4] == 'X') 
						{
							val = HexBufToByte(commRecvBuf, pos+2+address_size);
						}
						else if(commRecvBuf[pos+4] == 'B') 
						{
							val = HexBufToByte(commRecvBuf, pos+2+address_size);
						}
						else if(commRecvBuf[pos+4] == 'W') 
						{
							val = HexBufToUshort(commRecvBuf, pos+2+address_size);
						}
						else if(commRecvBuf[pos+4] == 'D') 
						{
							val = HexBufToUint(commRecvBuf, pos+2+address_size);
						}
						else
							val = 0;

						SetTagValue(address, val);
					}

					pos += 2+address_size+data_size;
				}

				int send_count;
				byte crc;

				commSendBuf[0] = (byte)ASCII.ACK;
				commSendBuf[1] = commRecvBuf[1];
				commSendBuf[2] = commRecvBuf[2];
				commSendBuf[3] = commRecvBuf[3];
				commSendBuf[4] = commRecvBuf[4];
				commSendBuf[5] = commRecvBuf[5];
					
				send_count = 6;
					
				commSendBuf[send_count++] = (byte)ASCII.ETX;
				crc = GetCrc(commSendBuf, 0, send_count);
				string imsi = String.Format("{0:X02}", crc);
				commSendBuf[send_count++] = (byte)(imsi[0]);
				commSendBuf[send_count++] = (byte)(imsi[1]);

				SendBytes(commSendBuf, send_count);
			}
			else if(commRecvBuf[4] == 'S' && commRecvBuf[5] == 'B') 
			{
				byte block_count = 1;
				
				byte address_size;
				int  address = 0;
				int rack = 0;
				int slot = 0;
				int pos = 8;
				int data_size;
				double val;

				pos = 6;

				for(int i = 0; i < block_count; i++) 
				{
					address_size = HexBufToByte(commRecvBuf, pos);
					if(!GetAddress(commRecvBuf, pos+2, address_size, ref address, ref rack, ref slot))	return;

					if(commRecvBuf[pos+4] == 'X')
						data_size = 2;
					else if(commRecvBuf[pos+4] == 'B')
						data_size = 2;
					else if(commRecvBuf[pos+4] == 'W')
						data_size = 4;
					else if(commRecvBuf[pos+4] == 'D')
						data_size = 8;
					else if(commRecvBuf[pos+4] == 'F')
						data_size = 8;
					else
						return;

					if(commRecvBuf[pos+3] == 'M') // M 영역만 Write
					{
						if(commRecvBuf[pos+4] == 'X') 
						{
							val = HexBufToByte(commRecvBuf, pos+2+address_size);
						}
						else if(commRecvBuf[pos+4] == 'B') 
						{
							val = HexBufToByte(commRecvBuf, pos+2+address_size);
						}
						else if(commRecvBuf[pos+4] == 'W') 
						{
							val = HexBufToUshort(commRecvBuf, pos+2+address_size);
						}
						else if(commRecvBuf[pos+4] == 'D') 
						{
							val = HexBufToUint(commRecvBuf, pos+2+address_size);
						}
						else
							val = 0;

						SetTagValue(address, val);
					}

					pos += 2+address_size+data_size;
				}

				int send_count;
				byte crc;

				commSendBuf[0] = (byte)ASCII.ACK;
				commSendBuf[1] = commRecvBuf[1];
				commSendBuf[2] = commRecvBuf[2];
				commSendBuf[3] = commRecvBuf[3];
				commSendBuf[4] = commRecvBuf[4];
				commSendBuf[5] = commRecvBuf[5];
					
				send_count = 6;
					
				commSendBuf[send_count++] = (byte)ASCII.ETX;
				crc = GetCrc(commSendBuf, 0, send_count);
				string imsi = String.Format("{0:X02}", crc);
				commSendBuf[send_count++] = (byte)(imsi[0]);
				commSendBuf[send_count++] = (byte)(imsi[1]);

				SendBytes(commSendBuf, send_count);
			}
		}

		public override void ProtocolRecvBytes(byte[] buf, int size)
		{			
			for(int i = 0; i < size; i++) 
			{
				if(buf[i] == (byte)ASCII.ENQ) 
				{
					commCountCurr = 0;
				}
				if(commCountCurr == 0 && buf[i] != (byte)ASCII.ENQ)	continue;

				commRecvBuf[commCountCurr] = buf[i];
				commCountCurr++;

				if(commCountCurr >= MAX_BUF) 
				{
					commCountCurr = 0;
					return;
				}
			}

			if(commCountCurr < 4)	return;

			if(commRecvBuf[commCountCurr-3] != (byte)ASCII.EOT)		return;

			int buf_hap = commCountCurr;
			commCountCurr = 0;

			if(bUseStation) 
			{
				byte station = HexBufToByte(commRecvBuf, 1);
				if(station != nStation)	return;
			}
			
			byte crc = GetCrc(commRecvBuf, 0, buf_hap-2);
			byte crc_check = HexBufToByte(commRecvBuf, buf_hap-2);

			if(crc != crc_check)	return;

			if(commRecvBuf[3] == (byte)'r') // read command
			{
				ExcuteReadCommand();
			}

			if(commRecvBuf[3] == (byte)'w') // write command
			{
				ExcuteWriteCommand();
			}
		}

		byte HexBufToByte(byte[] b, int pos)
		{
			byte val;
			byte retn = 0;

			for(int i = 0; i < 2; i++) 
			{
				retn <<= 4;
				if(b[pos+i] >= (byte)'0' && b[pos+i] <= (byte)'9')	
					val = (byte)(b[pos+i]-'0');
				else if(b[pos+i] >= (byte)'A' && b[pos+i] <= (byte)'F')	
					val = (byte)(b[pos+i]-'A'+10);
				else 
					val = 0;
				retn |= val;
			}

			return retn;
		}

		ushort HexBufToUshort(byte[] b, int pos)
		{
			ushort val;
			ushort retn = 0;

			for(int i = 0; i < 4; i++) 
			{
				retn <<= 4;
				if(b[pos+i] >= (byte)'0' && b[pos+i] <= (byte)'9')	
					val = (byte)(b[pos+i]-'0');
				else if(b[pos+i] >= (byte)'A' && b[pos+i] <= (byte)'F')	
					val = (byte)(b[pos+i]-'A'+10);
				else 
					val = 0;
				retn |= val;
			}

			return retn;
		}

		uint HexBufToUint(byte[] b, int pos)
		{
			uint val;
			uint retn = 0;

			for(int i = 0; i < 4; i++) 
			{
				retn <<= 4;
				if(b[pos+i] >= (byte)'0' && b[pos+i] <= (byte)'9')	
					val = (byte)(b[pos+i]-'0');
				else if(b[pos+i] >= (byte)'A' && b[pos+i] <= (byte)'F')	
					val = (byte)(b[pos+i]-'A'+10);
				else 
					val = 0;
				retn |= val;
			}

			return retn;
		}

		byte GetCrc(byte[] buf, int start, int size)
		{
			int i;
			byte crc = 0;
			for(i = 0; i < size; i++, start++) 
			{
				crc += buf[start];
			}

			return crc;
		}

		public override int ProtocolOption(ref string option)
		{
			FormProtocolOption form = new FormProtocolOption();

			form.SetParameters(option);

			if(form.ShowDialog() == DialogResult.OK) 
			{
				form.GetParameters(ref option);
				return 1;
			}

			return 0;
		}

	}
}
