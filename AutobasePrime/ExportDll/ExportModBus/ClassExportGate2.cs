using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using System.Net.Sockets;
using ExportLib;

namespace ExportModBus
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	/// 
	public class ClassExportGate2 : ExportLib.ClassExportLib2
	{
		byte		cStation = 0;
		byte[]		commRecvBuf = new byte[5000];
		int			commCountCurr = 0, commCountNeed = 8;
		bool		bReadSizeflag = false, bFloatData = false, bReadValueFloat = false;
		bool		bUseHeader = false;

        int         nCommand;
		int			nStartAddr;
        int         nRegistersSize;

        //int         nReadTagSize = 5000;
		int			MAX_TAG_LIST = 10000; 
		
		public override string ProtocolGetName()
		{
			return "MODBUS";
		}

		void getInitOptionData(string option)
		{
			if(option.Length <= 0) return;

			CommaBlockString comma = new CommaBlockString();
			byte			i = 0;

			comma.Set(option);				
			comma.GetBYTE(ref cStation);
			if(comma.IsEOS()) return;

			comma.GetBYTE(ref i);
			bFloatData = (i == 1) ? true : false;
			if(comma.IsEOS()) return;

			comma.GetBYTE(ref i);
			bReadValueFloat = (i == 1) ? true : false;			

			comma.GetBYTE(ref i);
			bUseHeader = (i == 1) ? true : false;			
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
			int			i, j, lenth = commCountCurr;

			if(this.bUseHeader) 
			{
				while(true) {
					if(commCountCurr < 7)	return;

					if( commRecvBuf[2] == 0 &&
						commRecvBuf[3] == 0 &&
						commRecvBuf[6] == cStation) 
					{
						return;		
					} 
					
					commCountCurr--;
					for(j = 0; j < commCountCurr;j++) 
					{
						commRecvBuf[j] = commRecvBuf[j+1];
					}
				}
			}
			else {
				for(i = 0; i < lenth; i++) 
				{
					if(commRecvBuf[0] == cStation) return;		// buf가 계속당겨지기 때문에 buf[0]
				
					commCountCurr--;
					for(j = 0; j < commCountCurr;j++) 
					{
						commRecvBuf[j] = commRecvBuf[j+1];
					}
				}
			}
		}

		bool IsOnePacketCodeInModbus()
		{
			if(commCountCurr >= 2) 
			{
				switch(commRecvBuf[1]) 
				{
					case 0x10 : break;
					default : 	commCountNeed = 8; break;
				}
			}

			if(commCountCurr < commCountNeed) return false;

			int		nSize;
			ushort	crc = 0;

			switch(commRecvBuf[1]) 
			{
				case 5 :
					break;
				case 6 :
					break;
				case 0x10 :
					if(bReadSizeflag == false) 
					{
						nSize = commRecvBuf[6];
						commCountNeed = nSize + 9;
						bReadSizeflag = true;
						if(commCountCurr < commCountNeed) return false;
						//pt->timeout->Reset();
					}
					break;
			}	
			bReadSizeflag = false;
			crc = ClassMakeCrcData.GetCRC_16_15_2_1(commRecvBuf, commCountNeed-2);
			if(crc == commRecvBuf[commCountNeed-2] * 256 + commRecvBuf[commCountNeed-1]) return true;
			commCountCurr = 0;			
			return false;
		}

		bool IsOnePacketCodeInModbusTcp()
		{
			if(commCountCurr < 8)	return false; 
			
			commCountNeed = commRecvBuf[4]*256+commRecvBuf[5]+6;

			if(commCountCurr < commCountNeed) return false;

			return true;
		}

		bool IsOnePacketCode()
		{
			if(this.bUseHeader)	return this.IsOnePacketCodeInModbusTcp();
			else				return this.IsOnePacketCodeInModbus();
		}

		bool readPacketStartAddrAndReadSize()
		{
			if(this.bUseHeader) 
			{
				switch(commRecvBuf[7]) 
				{
					case 3 :
					case 4 : 
					case 5 :	// Single Coil Write
					case 6 :
					case 0x10 : break;
					default :
						MakeAndSendErrorCodeData(1);	// Illegal function
						return false;
				}

                nCommand = commRecvBuf[7];
				nStartAddr = commRecvBuf[8] * 256 + commRecvBuf[9];

				if(commRecvBuf[7] == 6) 
                    nRegistersSize = 1;
				else					
                    nRegistersSize = commRecvBuf[10] * 256 + commRecvBuf[11];
			}
			else 
			{
				switch(commRecvBuf[1]) 
				{
					case 3 :
					case 4 : 
					case 5 :
					case 6 :
					case 0x10 : break;
					default :
						MakeAndSendErrorCodeData(1);	// Illegal function
						return false;
				}

                nCommand = commRecvBuf[1];
				nStartAddr = commRecvBuf[2] * 256 + commRecvBuf[3];
				if(commRecvBuf[1] == 6)
                    nRegistersSize = 1;
				else
                    nRegistersSize = commRecvBuf[4] * 256 + commRecvBuf[5];
			}
	
			return true;
		}

		bool isWriteDataPacket()
		{
			if(this.bUseHeader) 
			{
				switch(commRecvBuf[7]) 
				{
					case 5 :
					case 6 :
					case 0x10 : return true;
					default : 	return false;
				}	
			}
			else 
			{
				switch(commRecvBuf[1]) 
				{
					case 5 :
					case 6 :
					case 0x10 : return true;
					default : 	return false;
				}	
			}
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

		void MakeAndSendWantedData()
		{
			int			i, pos, buf_pos = 0, nSize;
			ushort		crc, val;
			float		fVal;
			byte[]		imsi = new byte[50];
			byte[]		commSendBuf = new byte[5000];

            if (nStartAddr + nRegistersSize > MAX_TAG_LIST) 
			{
				MakeAndSendErrorCodeData(2);				
				return;
			}
            if (nRegistersSize <= 0) 
			{
				MakeAndSendErrorCodeData(3);				
				return;
			}

			if(this.bUseHeader) 
			{
				commSendBuf[buf_pos++] = commRecvBuf[0];
				commSendBuf[buf_pos++] = commRecvBuf[1];
				commSendBuf[buf_pos++] = commRecvBuf[2];
				commSendBuf[buf_pos++] = commRecvBuf[3];
				commSendBuf[buf_pos++] = commRecvBuf[4];
				commSendBuf[buf_pos++] = commRecvBuf[5];
				commSendBuf[buf_pos++] = cStation;
				commSendBuf[buf_pos++] = commRecvBuf[7];
                commSendBuf[buf_pos++] = (byte)(nRegistersSize * 2);
                nSize = nRegistersSize;
				if(bFloatData) nSize /= 2;
				if(nSize >= 256) nSize = 255;
			
				for(i = 0; i < nSize; i++) 
				{
                    pos = (i + nStartAddr) % MAX_TAG_LIST;
					if(bFloatData) 
					{
						fVal = (float)GetTagValueDouble(pos);
						floatDataToByte(ref imsi, fVal);
						commSendBuf[buf_pos++] = imsi[3];				// float
						commSendBuf[buf_pos++] = imsi[2];
						commSendBuf[buf_pos++] = imsi[1];
						commSendBuf[buf_pos++] = imsi[0];			
					}
					else 
					{
						val = (ushort)GetTagValueDouble(pos);
						commSendBuf[buf_pos++] = (byte)(val/256);
						commSendBuf[buf_pos++] = (byte)(val);
					}
				}

				commSendBuf[4] = (byte)((buf_pos-6)/256);
				commSendBuf[5] = (byte)((buf_pos-6)%256);

				SendBytes(commSendBuf, buf_pos);
				commCountCurr = 0;
			}
			else 
			{
				commSendBuf[buf_pos++] = cStation;
				commSendBuf[buf_pos++] = commRecvBuf[1];
                commSendBuf[buf_pos++] = (byte)(nRegistersSize * 2);
                nSize = nRegistersSize;
				if(bFloatData) nSize /= 2;
				if(nSize >= 256) nSize = 255;
			
				for(i = 0; i < nSize; i++) 
				{
                    pos = (i + nStartAddr) % MAX_TAG_LIST;
					if(bFloatData) 
					{
						fVal = (float)GetTagValueDouble(pos);
						floatDataToByte(ref imsi, fVal);
						commSendBuf[buf_pos++] = imsi[3];				// float
						commSendBuf[buf_pos++] = imsi[2];
						commSendBuf[buf_pos++] = imsi[1];
						commSendBuf[buf_pos++] = imsi[0];
					}
					else 
					{
						val = (ushort)GetTagValueDouble(pos);
						commSendBuf[buf_pos++] = (byte)(val/256);
						commSendBuf[buf_pos++] = (byte)(val);
					}
				}
				crc = ClassMakeCrcData.GetCRC_16_15_2_1(commSendBuf, buf_pos);
				commSendBuf[buf_pos++] = (byte)(crc/256);		// CRC
				commSendBuf[buf_pos++] = (byte)(crc);

				SendBytes(commSendBuf, buf_pos);
				commCountCurr = 0;
			}
			
		}

		void MakeAndSendErrorCodeData(byte code)
		{
			int			buf_pos = 0;
			ushort		crc;
			byte[]		commSendBuf = new byte[50];

			if(this.bUseHeader) 
			{
				commSendBuf[buf_pos++] = commRecvBuf[0];
				commSendBuf[buf_pos++] = commRecvBuf[1];
				commSendBuf[buf_pos++] = commRecvBuf[2];
				commSendBuf[buf_pos++] = commRecvBuf[3];
				commSendBuf[buf_pos++] = 0;
				commSendBuf[buf_pos++] = 3;
				commSendBuf[buf_pos++] = cStation;
				commSendBuf[buf_pos++] = (byte)(commRecvBuf[7] | 0x80);
				commSendBuf[buf_pos++] = code;
				SendBytes(commSendBuf, buf_pos);
				commCountCurr = 0;
			}
			else 
			{
				commSendBuf[buf_pos++] = cStation;
				commSendBuf[buf_pos++] = (byte)(commRecvBuf[1] | 0x80);
				commSendBuf[buf_pos++] = code;

				crc = ClassMakeCrcData.GetCRC_16_15_2_1(commSendBuf, buf_pos);			
				commSendBuf[buf_pos++] = (byte)(crc/256);		// CRC
				commSendBuf[buf_pos++] = (byte)(crc);
				SendBytes(commSendBuf, buf_pos);
				commCountCurr = 0;
			}
		}

        void MakeWriteAckDataAndSend()
		{
			int			buf_pos = 0;
			ushort		crc;
			byte[]		commSendBuf = new byte[50];

			if(this.bUseHeader) 
			{
				commSendBuf[buf_pos++] = commRecvBuf[0];
				commSendBuf[buf_pos++] = commRecvBuf[1];
				commSendBuf[buf_pos++] = commRecvBuf[2];
				commSendBuf[buf_pos++] = commRecvBuf[3];
				commSendBuf[buf_pos++] = 0;
				commSendBuf[buf_pos++] = 6;
				commSendBuf[buf_pos++] = cStation;
				commSendBuf[buf_pos++] = commRecvBuf[7];
				commSendBuf[buf_pos++] = (byte)(nStartAddr/256);
				commSendBuf[buf_pos++] = (byte)(nStartAddr);
                if (nCommand == 0x10)
                {
                    commSendBuf[buf_pos++] = commRecvBuf[nRegistersSize / 256];
                    commSendBuf[buf_pos++] = commRecvBuf[nRegistersSize % 256];
                }
                else
                {
                    commSendBuf[buf_pos++] = commRecvBuf[10];
                    commSendBuf[buf_pos++] = commRecvBuf[11];
                }

				SendBytes(commSendBuf, buf_pos);
				commCountCurr = 0;
			}
			else 
			{
				commSendBuf[buf_pos++] = cStation;
				commSendBuf[buf_pos++] = commRecvBuf[1]; 
				commSendBuf[buf_pos++] = (byte)(nStartAddr/256);
				commSendBuf[buf_pos++] = (byte)(nStartAddr);
                if (nCommand == 0x10)
                {
                    commSendBuf[buf_pos++] = commRecvBuf[nRegistersSize / 256];
                    commSendBuf[buf_pos++] = commRecvBuf[nRegistersSize % 256];
                }
                else
                {
                    commSendBuf[buf_pos++] = commRecvBuf[4];
                    commSendBuf[buf_pos++] = commRecvBuf[5];
                }
				crc = ClassMakeCrcData.GetCRC_16_15_2_1(commSendBuf, buf_pos);
				commSendBuf[buf_pos++] = (byte)(crc/256);		// CRC
				commSendBuf[buf_pos++] = (byte)(crc);

				SendBytes(commSendBuf, buf_pos);
				commCountCurr = 0;
			}
		}		

		void saveToReceivedDataAndSendAck()
		{
			int			i, nSize;
			ushort		lo_word, hi_word;
			uint		val;
			float		fVal = 0;
			int start = this.bUseHeader ? 6 : 0;

			if(commRecvBuf[start+1] == 5) 											// Write Single Coil
			{
				if(commRecvBuf[start+4] == 0 && commRecvBuf[start+5] == 0) 
					SetTagValue(nStartAddr, 0);
				else	// 0xFF00
					SetTagValue(nStartAddr, 1);
			}
			else if(commRecvBuf[start+1] == 6)											// Function 6 = 항상 워드
			{
				SetTagValue(nStartAddr, commRecvBuf[start+4]*256u+commRecvBuf[start+5]); //PokeWORD(pt, localVars->nStartAddr, pt->commRecvBuf[4]*256u+(BYTE)pt->commRecvBuf[5]);				
			}
            else if(commRecvBuf[start+1] == 16)// 0x10 Write Multiple registers
			{
                // Request
                // Function code 1 Byte 0x10
                // Starting Address 2 Bytes 0x0000 to 0xFFFF
                // Quantity of Registers 2 Bytes 0x0001 to 0x0078
                // Byte Count 1 Byte 2 x N*
                // Registers Value N* x 2 Bytes value

                // Response
                // Function code 1 Byte 0x10
                // Starting Address 2 Bytes 0x0000 to 0xFFFF
                // Quantity of Registers 2 Bytes 1 to 123 (0x7B)

				nSize = nRegistersSize;

				if(bReadValueFloat) 
                    nSize /= 2;
		
				for(i = 0; i < nSize; i++) 
				{
					if(bReadValueFloat) 
					{
						lo_word = (ushort)(commRecvBuf[start+9+i*4]*256+commRecvBuf[start+10+i*4]);
						hi_word = (ushort)(commRecvBuf[start+7+i*4]*256+commRecvBuf[start+8+i*4]);
						val = (uint)(hi_word * 0x10000 + lo_word);

						dwordDataToFloat(ref fVal, val);		//memcpy(&f_value, &val, 4);
						SetTagValue(nStartAddr+i, fVal);			//PokeFLOAT(pt, localVars->nStartAddr+i, f_value);
					}
					else 
					{
						SetTagValue(nStartAddr+i, commRecvBuf[start+7+i*2]*256u+commRecvBuf[start+8+i*2]); //PokeWORD(pt, localVars->nStartAddr+i, pt->commRecvBuf[7+i*2]*256u+(BYTE)pt->commRecvBuf[8+i*2]);
					}
				}
			}
			MakeWriteAckDataAndSend();			
			commCountNeed = 8;			
		}

		public override void ProtocolRecvBytes(byte[] buf, int size)
		{			
			for(int i = 0; i < size; i++) 
			{
				if(commCountCurr+i >= 5000) 
				{
					commCountCurr = 0;
					return;
				}

				commRecvBuf[commCountCurr+i] = buf[i];
			}
			commCountCurr += size;
			checkStartStation();			

			if(IsOnePacketCode()) 
			{
				readPacketStartAddrAndReadSize();
				if(isWriteDataPacket()) 
				{
					saveToReceivedDataAndSendAck();
					return;
				}
				MakeAndSendWantedData();
			}
		}

		public override int ProtocolOption(ref string option)
		{
			FormProtocolOption form = new FormProtocolOption();

			getInitOptionData(option);			// 옵션 데이터를 다시 읽는다, 실제 통신에는 영향이 없다			
			form.numericUpDownStation.Value = cStation;
			form.checkBoxReadFloat.Checked = bFloatData;
			form.checkBoxWriteFloat.Checked = bReadValueFloat;
			form.checkBoxUseHeader.Checked = bUseHeader;

			if(form.ShowDialog() == DialogResult.OK) 
			{
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
				return 1;
			}
			return 0;
		}
	}
}
