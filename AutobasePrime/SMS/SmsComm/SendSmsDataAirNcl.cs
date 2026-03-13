using System;
using SMS.SmsFunc;
using NetTools;
using System.Net;
using System.Net.Sockets;
using AutoLibLocal;
using SMS.Display;
using System.Threading;
using DialogCommon;

namespace SMS.SmsComm
{
	/// <summary>
	/// Summary description for SendSmsDataAirNcl.
	/// </summary>
	public class SendSmsDataAirNcl
	{
		static public ushort			tagNo = 0;

		public SendSmsDataAirNcl()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static public ushort GetCheckSum(byte[] cpBuf, int nLen)
		{
			int				i;
			ushort			 nCS=0;
		
			if(nLen >= 1280) return 0;

			for(i=0;i<nLen;i++)
			{
				nCS += cpBuf[i];
			}
			return nCS;
		}

		static public ushort GetCheckSumString(string cpBuf, int start, int end)
		{
			int				i;
			ushort			nCS=0;
		
			for(i = start;i < end;i++)
			{
				nCS += (byte)cpBuf[i];
			}
			return nCS;
		}

		static short EscapeProcessing(ref byte[] cpBuf, int nLen)
		{
			byte[]	cTemp = new byte[1280];
			short	nCnt=0, i;

			if(nLen >= 1280)	return 0;

			cpBuf.CopyTo(cTemp, 0);
			for(i=0;i<nLen;i++)
			{
				if((cTemp[i] == (byte)EnumAsciiCode.SOH) || (cTemp[i] == (byte)EnumAsciiCode.EOT) || (cTemp[i] == (byte)EnumAsciiCode.DLE) || (cTemp[i] == (byte)EnumAsciiCode.XON) ||
					(cTemp[i] == (byte)EnumAsciiCode.XOFF))
				{
					cpBuf[nCnt++] = (byte)EnumAsciiCode.DLE;
					cpBuf[nCnt++] = (byte)(cTemp[i] + 0x20);
				}
				else
					cpBuf[nCnt++] = cTemp[i];
			}
			return nCnt;
		}

		static public void EscapeUnprocessing(ref string cpBuf)
		{
			string	cTemp = cpBuf;
			int		nLen = cpBuf.Length, i;
			
			cpBuf = "";
			for(i = 0;i < nLen;i++)
			{
				if(cTemp[i] == (char)EnumAsciiCode.DLE)
				{
					i++;
					cpBuf += (char)(cTemp[i] - 0x20);
				}
				else
					cpBuf += cTemp[i];
			}
		}

		static public int EscapeUnprocessing(ref byte[] cpBuf, int nLen)
		{
			byte[]		cTemp = new byte[1280];
			short		nCnt = 0, i;

			if(nLen >= 1280) nLen = 1280;
			cpBuf.CopyTo(cTemp, 0);
			for(i = 0;i < nLen;i++)
			{
				if(cTemp[i] == (byte)EnumAsciiCode.DLE)
				{
					i++;
					cpBuf[nCnt++] = (byte)(cTemp[i] - 0x20);
				}
				else
					cpBuf[nCnt++] = cTemp[i];
			}
			return nCnt;
		}

		/*
		* 모든 데이터에 SOH 데이터 EOT를 붙인다.
		*/
		static short SOHEOTProcessing(ref byte[] cpBuf, int nLen)
		{
			byte[]		cTemp = new byte[1280];
			short		nCnt=0, i;

			if(nLen >= 1280)	return 0;

			cpBuf.CopyTo(cTemp, 0);
			cpBuf[nCnt++] = (byte)EnumAsciiCode.SOH;
			for(i=0;i<nLen;i++)
			{
				cpBuf[nCnt++] = cTemp[i];
			}
			cpBuf[nCnt++] = (byte)EnumAsciiCode.EOT;
			return nCnt;
		}



		//
		// cpBuf는 Escape처리, Checksum, SOH, EOT등을 처리하지 않은 데이터이다.
		// NCLWrite는 위의 처리를 한 후 Port에 Write.
		//
		static int NCLWrite(ref byte[] cpBuf, int nLen)
		{
			ushort			unChecksum = 0;
			
			unChecksum = GetCheckSum(cpBuf, nLen);
			cpBuf[nLen++] = (byte)(unChecksum / 256);
			cpBuf[nLen++] = (byte)(unChecksum % 256);
			nLen = EscapeProcessing(ref cpBuf, nLen);
			nLen = SOHEOTProcessing(ref cpBuf, nLen);
			return nLen;
		}

		static int stringDataToByteCopy(ref byte[] commSendBuf, string data, int buf_pos, int size)
		{
			if(size <= 0) return 0;

			int			len;
			byte[]		imsi;

			imsi = Tools.StringToBytes(data);
			len = imsi.Length;
			for(int i = 0; i < size; i++) 
			{
				if(i < len) commSendBuf[i+buf_pos] = imsi[i];
				else		commSendBuf[i+buf_pos] = (byte)0;
			}

			return size;
		}

		static int stringDataToByteCopy(ref byte[] commSendBuf, string data, int buf_pos)
		{
			byte[]		imsi;

			imsi = Tools.StringToBytes(data);

			int limit = imsi.Length;
			if(limit > SmsBasic.smsConfig.nMaxSendChar)
				limit = SmsBasic.smsConfig.nMaxSendChar;

			for(int i = 0; i < limit; i++)
			{
				commSendBuf[i+buf_pos] = imsi[i];
			}

			return limit;
		}

		
		static public int makeAirNclSendWriteCode(ref byte[] commSendBuf, string telNo, string sendTelNo, string message)
		{
			int				buf_pos = 0;
			
			commSendBuf[buf_pos++] = (byte)'A';	
			commSendBuf[buf_pos++] = (byte)0x00;
			commSendBuf[buf_pos++] = (byte)0x04;
			commSendBuf[buf_pos++] = (byte)(0x200 / 256);					// Tag
			commSendBuf[buf_pos++] = (byte)(0x200 % 256);
			commSendBuf[buf_pos++] = (byte)'1';
			commSendBuf[buf_pos++] = (byte)0x40;							//cRequestOptions;

			commSendBuf[buf_pos++] = (byte)0x00;
			commSendBuf[buf_pos++] = (byte)0x02;
			commSendBuf[buf_pos++] = (byte)'Q';
			commSendBuf[buf_pos++] = (byte)'M';
			commSendBuf[buf_pos++] = (byte)0x2A;							// Code1
			commSendBuf[buf_pos++] = (byte)0x09;							// Code2
			tagNo += 16;
			commSendBuf[buf_pos++] = (byte)(tagNo/256);
			commSendBuf[buf_pos++] = (byte)(tagNo%256);			
			commSendBuf[buf_pos++] = (byte)0x01;							// 전송할 전화번호 개수 1 ~ 15, 여기서는 항상 1개
			
			buf_pos += stringDataToByteCopy(ref commSendBuf, sendTelNo, buf_pos, 16);
			buf_pos += stringDataToByteCopy(ref commSendBuf, telNo, buf_pos, 16);
			message = message.Trim();
			buf_pos += stringDataToByteCopy(ref commSendBuf, message, buf_pos);
			commSendBuf[buf_pos++] = (byte)0x06;							// End Code

			commSendBuf[1] = (byte)((buf_pos-5) / 256);						// Data size
			commSendBuf[2] = (byte)((buf_pos-5) % 256);
			buf_pos = NCLWrite(ref commSendBuf, buf_pos);
			return buf_pos;
		}


	}
}
