using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Opc.Da;
using System.Threading;
using OpcClient;

namespace OpcClient.plcScanComm
{
	/// <summary>
	/// Summary description for plcScanCommMainClass.
	/// </summary>
	public class plcScanCommMainClass
	{

		enum eRWDataType { READ_DATA = 100, WRITE_DATA = 200, ACK_DATA = 6,  };
		enum eErrorCodeType { NONE_ERROR = 0, CRC_ERROR, TIMEOUT_ERROR, NONE_DATA, SERVER_NAME_NOT_EXIST, GROUP_NAME_NOT_EXIST, ITEM_NAME_NOT_EXIST, SERVER_CONNECTION_ERROR, WRITE_ITEM_ERROR, WRITE_ITEM_READ_ERROR, WRITE_ITEM_TYPE_ERROR, ITEM_DATA_SIZE_TOO_BIG, UNKNOWN_COMMAND };
		public enum eRWDataTypeCode { NOT_READ = 0, FLOAT_DATA = 1, DOUBLE_DATA = 2, STRING_DATA = 3, DATETIME_DATA = 4, NOT_SUPPORTED = 0x80 };
		

		static AutoLib.PlcScanDeviceSharedMemory sharedDevice;
		static bool						bStart = true;
		static bool						bDllLoad = false;

		static int						nMaxSendBufCount = 9950;// 10000 - 7(tail) -43(여분)
		static int						nHeaderSize = 18;		// write packet의 기본 header 길이
		static int						nPacketNo = 0;			// write packet 번호
		static int						nStartItemPos = 0;		// write packet 시작아이템 번호
		static int						nSendBufPos = nHeaderSize;// write packet 버퍼위치
		static int						nPacketItemCount = 0;	// write packet 하나의 패킷에 들어있는 아이템 수

		static byte[]					sendBuf = new byte[10000];
		static byte[]					recvBuf = new byte[10000];
		static byte[]					startCode = new byte[10];

		static ushort					nTns;
		static eRWDataType				cReadCommand;
		static string					readServerName, readGroupName, readItemName;
		public static string			writeString;
		public static double			dWriteVal;
		static bool						bPlcScanMainThreadPause = false;
		static bool						bPlcScanMainThreadPaused = false;

		static int						nReadBufPos = 0;
		static int						onePacketLength = 12;
		static bool						bReadPacketSize = false;
		static bool						bCheckStart = false;

		public plcScanCommMainClass()
		{
			//
			// TODO: Add constructor logic here
			//			
		}

		static void setInitAndDllLoad()
		{
			bStart = false;					// 한번만 로딩되므로
			startCode[0] = (byte)'O';
			startCode[1] = (byte)'P';
			startCode[2] = (byte)'C';
			startCode[3] = (byte)'_';
			startCode[4] = (byte)'C';
			startCode[5] = (byte)'L';
			startCode[6] = (byte)'I';
			startCode[7] = (byte)'E';
			startCode[8] = (byte)'N';
			startCode[9] = (byte)'T';

			try 
			{				
				sharedDevice = new AutoLib.PlcScanDeviceSharedMemory();
				sharedDevice.Open(opcBasic.opcClientConfig.shareName);
			}
			catch
			{
				if(NetTools.Tools.IsLangKorean())
					MessageBox.Show("PlcScanDeviceSharedMemory.DLL 파일이 없습니다.");
				else
					MessageBox.Show("PlcScanDeviceSharedMemory.DLL File Does Not Exist.");
				bDllLoad = false;
				return;
			}
			bDllLoad = true;			
		}

		static public bool plcScanThreadPause(bool bPause)
		{
			bPlcScanMainThreadPause = bPause;
			TimeOutClass timeout = new TimeOutClass();

			timeout.Reset();
			while(true)
			{
				if(bPlcScanMainThreadPaused == bPause) return true;
				if(timeout.IsTimeOut(2)) return false;	// 2초 동안만 기다린다
			}
		}

		static public void plcScanCommMain()
		{
			if(bStart) setInitAndDllLoad();
			if(bDllLoad == false) return;

			while(Form1.bPlcScanMainThreadEnd == false) 
			{
				Thread.Sleep(1);
				bPlcScanMainThreadPaused = bPlcScanMainThreadPause;
				if(bPlcScanMainThreadPaused) continue;
				readWriteDataFromPlcScan();
			}
		}

		static bool isMatchHeaderStart()
		{
			for(int i = 0; i < 10; i++) 
			{
				if(recvBuf[i] != startCode[i]) return false;				
			}
			return true;
		}

		static bool checkStartHeader(ref int buf_pos)
		{
			if(buf_pos <= 9) return false;
			int count = buf_pos - 9;

			for(int i = 0; i < count; i++) 
			{
				if(isMatchHeaderStart()) 
				{
					bCheckStart = true;
					return true;
				}
				basicTool.copyBufPrev(ref recvBuf, buf_pos);
				buf_pos--;
			}
			return false;
		}

		static void readWriteParameterReset()
		{
			nReadBufPos = 0;
			onePacketLength = 12;
			bReadPacketSize = false;
			bCheckStart = false;
		}

		static void readWriteDataFromPlcScan()
		{			
			int		retn;

			//retn = sharedDevice.Read();
			//if(retn == -1) return;						
			//recvBuf[nReadBufPos++] = (byte)(retn % 256);
			while(true) 
			{
				if(nReadBufPos >= nMaxSendBufCount) readWriteParameterReset();// 최대 읽을 메모리보다 크면, 메모리 위치를 초기화
				retn = sharedDevice.Read();
				if(retn == -1) break;
				recvBuf[nReadBufPos++] = (byte)(retn % 256);
				if(bCheckStart == false) checkStartHeader(ref nReadBufPos);	// Start 코드가 아닐 때
				if(nReadBufPos >= onePacketLength)
				{
					if(bReadPacketSize == false)  
					{
						onePacketLength = recvBuf[10] * 256 + recvBuf[11];
						bReadPacketSize = true;
						continue;
					}
					break;
				}
			}			
			if(bReadPacketSize == false || nReadBufPos < onePacketLength) return;
			if(nReadBufPos > 21) interpretPacketCommand(nReadBufPos);
			readWriteParameterReset();
		}

		

		static void readDataCommandParameter()
		{
			int			count, pos = 15;

			count = recvBuf[pos++];
			if(count <= 0) 
			{
				makeAndSendDataPacket(true, true);
				return;
			}
			
			readServerName = basicTool.byteDataToString(recvBuf, pos, count);			
			pos += count;
			//bCommFromDrive = true;
			count = recvBuf[pos++];
			if(count <= 0) 
			{				
				makeAndSendDataPacket(false, true);
				return;
			}
			readGroupName = basicTool.byteDataToString(recvBuf, pos, count);
			makeAndSendDataPacket(false, false);
		}

		static void writeDataCommandParameter()
		{
			int				count, pos = 15;

			count = recvBuf[pos++];
			if(count <= 0) 
			{
				writeErrorDataPacket(eErrorCodeType.SERVER_NAME_NOT_EXIST, true);
				return;
			}
			readServerName = basicTool.byteDataToString(recvBuf, pos, count);
			pos += count;

			//bCommFromDrive = true;
			count = recvBuf[pos++];
			if(count <= 0) readGroupName = "";		// 그룹이름은 없을 수도 있다.
			else readGroupName = basicTool.byteDataToString(recvBuf, pos, count);
			pos += count;

			count = recvBuf[pos++];
			if(count <= 0) 
			{
				writeErrorDataPacket(eErrorCodeType.ITEM_NAME_NOT_EXIST, true);
				return;
			}
			readItemName = basicTool.byteDataToString(recvBuf, pos, count);
			pos += count;

			eRWDataTypeCode flag = (eRWDataTypeCode)recvBuf[pos++];
			switch(flag) {
				case eRWDataTypeCode.FLOAT_DATA :
					dWriteVal = Convert.ToDouble(basicTool.byteDataToFloat(recvBuf, pos));
					pos += 4;
					break;
				case eRWDataTypeCode.DOUBLE_DATA :
					dWriteVal = basicTool.byteDataToDouble(recvBuf, pos);
					pos += 8;
					break;
				case eRWDataTypeCode.STRING_DATA :
				case eRWDataTypeCode.DATETIME_DATA :					
					count = recvBuf[pos++];
					if(count > 0)
						writeString = basicTool.byteDataToString(recvBuf, pos, count);
					else
						writeString = "";
					pos += count;
					break;				
				default:
					writeErrorDataPacket(eErrorCodeType.UNKNOWN_COMMAND, true);
					return;
			}
			ushort itemPos = (ushort)(recvBuf[pos]*0x100 + recvBuf[pos+1]);
			writeItemAndSendDataPacket(flag, itemPos);
		}

		static void interpretPacketCommand(int buf_pos)
		{			
			ushort crc = basicTool.GetCRC_SumWORD(recvBuf, buf_pos-2);
			if(crc != recvBuf[buf_pos-2] * 256 + recvBuf[buf_pos-1]) 
			{
				writeErrorDataPacket(eErrorCodeType.CRC_ERROR, true);
				return;
			}
			
			cReadCommand = (eRWDataType)recvBuf[12];
			nTns = (ushort)(recvBuf[13] * 256 + recvBuf[14]);
			switch(cReadCommand) 
			{
				case eRWDataType.READ_DATA :
					readDataCommandParameter();
					break;
				case eRWDataType.WRITE_DATA :
					writeDataCommandParameter();
					break;
				default :
					writeErrorDataPacket(eErrorCodeType.UNKNOWN_COMMAND, true);
					break;
			}			
		}

		
		static bool checkServerName(ref int nServer)
		{			
			if(opcBasic.getServerStringFromServerNamePos(ref nServer, readServerName) == false) 
			{
				writeErrorDataPacket(eErrorCodeType.SERVER_NAME_NOT_EXIST, true);
				return false;
			}
			return true;
		}

		static bool checkGroupName(int nServer, ref int nGroup, bool bRead)
		{			
			if(opcBasic.getGroupStringFromGroupNamePos(nServer, ref nGroup, readGroupName) == false) // 임의의 서버이름을 붙여서 그룹이름 위치를 가져올 수 있게 한다
			{
				if(bRead)
					writeErrorDataPacket(eErrorCodeType.GROUP_NAME_NOT_EXIST, true);
				return false;
			}
			return true;
		}

		static bool checkItemName(int nServer, int nGroup, ref int nItem)
		{			
			if(opcBasic.getItemStringFromItemNamePos(nServer, nGroup, ref nItem, readItemName) == false) // 임의의 서버/그룹이름을 붙여서 그룹이름 위치를 가져올 수 있게 한다
				return false;
			return true;
		}

		static void setStringDataToBuf(object val)
		{
			if(nSendBufPos+stringDataToBufCount(val) >= nMaxSendBufCount) writeOnePacketData(false);	// check size overflow

			string		data = (string)val;			
			sendBuf[nSendBufPos++] = (byte)eRWDataTypeCode.STRING_DATA;	// string 데이터
			if(data.Length >= 256) data = data.Substring(0, 255);
			sendBuf[nSendBufPos++] = (byte)(data.Length);
			for(int i = 0; i < data.Length; i++) 
			{
				sendBuf[nSendBufPos++] = (byte)data[i];
			}
			nPacketItemCount++;
		}

		static int stringDataToBufCount(object val)
		{
			string		data = (string)val;

			if(data.Length >= 256) return 255+1;	// length + type
			return data.Length+1;			
		}
		
		static void setDateTimeToBuf(object val)
		{
			if(nSendBufPos+dateTimeToBufCount(val) >= nMaxSendBufCount) writeOnePacketData(false);	// check size overflow

			DateTime	ti = (DateTime)val;
			string		data;

			sendBuf[nSendBufPos++] = (byte)eRWDataTypeCode.DATETIME_DATA;							// Date Time 데이터
			data = String.Format("{0,4:d04}-{1,2:d02}-{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", ti.Year, ti.Month, ti.Day, ti.Hour, ti.Minute, ti.Second);
			if(data.Length >= 256) data = data.Substring(0, 255);
			sendBuf[nSendBufPos++] = (byte)(data.Length);
			for(int i = 0; i < data.Length; i++) 
			{
				sendBuf[nSendBufPos++] = (byte)data[i];
			}
			nPacketItemCount++;
		}

		static int dateTimeToBufCount(object val)
		{
			DateTime	ti = (DateTime)val;
			string		data;

			data = String.Format("{0,4:d04}-{1,2:d02}-{2,2:d02} {3,2:d02}:{4,2:d02}:{5,2:d02}", ti.Year, ti.Month, ti.Day, ti.Hour, ti.Minute, ti.Second);
			if(data.Length >= 256) return 255+1;
			return data.Length+1;
		}

		
		static void setDecimalOneItemDataToBuf(object val)
		{
			if(nSendBufPos+5 >= nMaxSendBufCount) writeOnePacketData(false);	// check size overflow

			byte[]		imsi = new byte[8];
			int			i;
			float		fData = 0.0F;
			
			//if(val.GetType() == Opc.Type.DOUBLE) 
			//{
			//	sendBuf[buf_pos++] = eRWDataTypeCode.DOUBLE_DATA;			// double 데이터
			//	basicTool.doubleDataToByte(ref imsi, (double)val);
			//	for(i = 7; i >= 0; i--) sendBuf[buf_pos++] = imsi[i];	// data
			//	return 9;
			//}
			

			try
			{
				if(val.GetType() == Opc.Type.BOOLEAN) 
				{
					if(String.Compare(val.ToString(), "TRUE", true) == 0) fData = 1.0F;
					else fData = 0.0F;
				}
				else
					fData = Convert.ToSingle(val);
			}
			catch{}
			sendBuf[nSendBufPos++] = (byte)eRWDataTypeCode.FLOAT_DATA;		// float 데이터
			basicTool.floatDataToByte(ref imsi, fData);
			for(i = 3; i >= 0; i--) sendBuf[nSendBufPos++] = imsi[i];	// data			
			nPacketItemCount++;
		}

		
		static void setObjectDataToBuf(object val)
		{
			if(opcBasic.isValueObjectIsSingleType(val)) 
			{
				if(val.GetType() == Opc.Type.STRING) 
				{
					setStringDataToBuf(val);
				}
				else if(val.GetType() == Opc.Type.DATETIME) 
				{					
					setDateTimeToBuf(val);
				}
				else {
					setDecimalOneItemDataToBuf(val);
				}				
				return;
			}

			int			i;
			if(val.GetType() == Opc.Type.ARRAY_INT) 
			{
				int[] iVal = (int[])val;
				for(i = 0; i < iVal.Length; i++) 
				{					
					 setDecimalOneItemDataToBuf(iVal[i]);
				}			
			}
			else if(val.GetType() == Opc.Type.ARRAY_DOUBLE) 
			{
				double[] dVal = (double[])val;
				for(i = 0; i < dVal.Length; i++) 
				{
					setDecimalOneItemDataToBuf(dVal[i]);
				}				
			}
			else if(val.GetType() == Opc.Type.ARRAY_BINARY) 
			{
				byte[] byteVal = (byte[])val;
				for(i = 0; i < byteVal.Length; i++) 
				{
					setDecimalOneItemDataToBuf(byteVal[i]);
				}
			}
			else if(val.GetType() == Opc.Type.ARRAY_SBINARY) 
			{
				sbyte[] sbyteVal = (sbyte[])val;
				for(i = 0; i < sbyteVal.Length; i++) 
				{
					setDecimalOneItemDataToBuf(sbyteVal[i]);
				}
			}
			else if(val.GetType() == Opc.Type.ARRAY_FLOAT) 
			{
				float[] fVal = (float[])val;
				for(i = 0; i < fVal.Length; i++) 
				{
					setDecimalOneItemDataToBuf(fVal[i]);
				}
			}
			else if(val.GetType() == Opc.Type.ARRAY_STRING) 
			{
				string[] sVal = (string[])val;
				for(i = 0; i < sVal.Length; i++) 
				{					
					setStringDataToBuf(sVal[i]);
				}
			}
			else if(val.GetType() == Opc.Type.ARRAY_SHORT) 
			{
				short[] shortVal = (short[])val;
				for(i = 0; i < shortVal.Length; i++) 
				{
					setDecimalOneItemDataToBuf(shortVal[i]);
				}
			}
			else if(val.GetType() == Opc.Type.ARRAY_USHORT) 
			{
				ushort[] ushortVal = (ushort[])val;
				for(i = 0; i < ushortVal.Length; i++) 
				{
					setDecimalOneItemDataToBuf(ushortVal[i]);
				}
			}
			else if(val.GetType() == Opc.Type.ARRAY_UINT) 
			{
				uint[] uintVal = (uint[])val;
				for(i = 0; i < uintVal.Length; i++) 
				{
					setDecimalOneItemDataToBuf(uintVal[i]);
				}
			}
			else if(val.GetType() == Opc.Type.ARRAY_LONG) 
			{
				long[] longVal = (long[])val;
				for(i = 0; i < longVal.Length; i++) 
				{
					setDecimalOneItemDataToBuf(longVal[i]);
				}
			}
			else if(val.GetType() == Opc.Type.ARRAY_ULONG) 
			{
				ulong[] ulongVal = (ulong[])val;
				for(i = 0; i < ulongVal.Length; i++) 
				{
					setDecimalOneItemDataToBuf(ulongVal[i]);
				}
			}
			else if(val.GetType() == Opc.Type.ARRAY_DECIMAL) 
			{
				decimal[] decimalVal = (decimal[])val;
				for(i = 0; i < decimalVal.Length; i++) 
				{
					setDecimalOneItemDataToBuf(decimalVal[i]);
				}
			}
			else if(val.GetType() == Opc.Type.ARRAY_BOOLEAN) 
			{
				bool[] boolVal = (bool[])val;
				for(i = 0; i < boolVal.Length; i++) 
				{
					setDecimalOneItemDataToBuf(boolVal[i]);
				}
			}
			else if(val.GetType() == Opc.Type.ARRAY_DATETIME) 
			{
				DateTime[] DateTimeVal = (DateTime[])val;
				for(i = 0; i < DateTimeVal.Length; i++) 
				{					
					setDateTimeToBuf(DateTimeVal[i]);
				}
			}
			else if(val.GetType() == Opc.Type.ARRAY_ANY_TYPE) 
			{
				object[] objectVal = (object[])val;
				for(i = 0; i < objectVal.Length; i++) 
				{
					setObjectDataToBuf(objectVal[i]);
				}				
			}
			else 
			{
				if(nSendBufPos+1 >= nMaxSendBufCount) writeOnePacketData(false);
				sendBuf[nSendBufPos++] = (byte)eRWDataTypeCode.NOT_SUPPORTED;		// 0x80				
			}
		}

		

		static void oneGroupDataMakeBuf(opcGroupReadWriteClass opcGroup)
		{
			opcItemReadWriteClass	opcItem;
			int						i;//, count, old_pos;
			
			for(i = 0; i < opcGroup.arrItem.Count; i++) 
			{
				opcItem = (opcItemReadWriteClass)opcGroup.arrItem[i];
				oneItemDataMakeBuf(opcItem);
				//if(count == -1 || buf_pos >= nMaxSendBufCount) 
				//{
				//	writeOnePacketData(old_pos, itemCount, nPacket++, ref nCurrItemPos, false, eErrorCodeType.NONE_ERROR);
				//	buf_pos = nHeaderSize;
				//	nCurrItemPos += itemCount;
				//	itemCount = 0;
				//	count = oneItemDataMakeBuf(opcItem, ref buf_pos);
				//	if(count == -1) writeOnePacketData(old_pos, itemCount, nPacket++, ref nCurrItemPos, false, eErrorCodeType.NONE_ERROR);
				//	else itemCount += count;
				//}
				//itemCount += count;
			}
			//return itemCount;

			/*if(opcGroup.arrItem.Count <= 0) return 0;

			opcItemReadWriteClass opcItem;
			int				itemCount = 0;
			for(int i = 0; i < opcGroup.arrItem.Count; i++) 
			{
				opcItem = (opcItemReadWriteClass)opcGroup.arrItem[i];
				if(opcItem.bFlag == false) // 데이터를 읽지 못했다.
				{					
					sendBuf[buf_pos++] = (byte)eRWDataTypeCode.NOT_READ;
					itemCount++;
					continue;
				}
				if(opcBasic.isValueObjectSupportedType(opcItem.readData) == false) 
				{											// 지원하지 않는 아이템
					sendBuf[buf_pos++] = (byte)eRWDataTypeCode.NOT_SUPPORTED;		// 0x80
					itemCount++;
					continue;
				}
				setObjectDataToBuf(opcItem.readData, ref buf_pos, ref itemCount);
			}
			return itemCount;			// 아이템의 개수, array는 여러개가 증가된다*/
			
		}

		static void oneItemDataMakeBuf(opcItemReadWriteClass opcItem)
		{
			if(opcItem.bFlag == false && opcItem.reuslt != Opc.ResultID.E_WRITEONLY) // 데이터를 읽지 못했다.
			{
				if(nSendBufPos+1 >= nMaxSendBufCount) writeOnePacketData(false);
				sendBuf[nSendBufPos++] = (byte)eRWDataTypeCode.NOT_READ;
				nPacketItemCount++;
			}
			else if(opcBasic.isValueObjectSupportedType(opcItem.readData) == false) 
			{											// 지원하지 않는 아이템
				if(nSendBufPos+1 >= nMaxSendBufCount) writeOnePacketData(false);
				sendBuf[nSendBufPos++] = (byte)eRWDataTypeCode.NOT_SUPPORTED;		// 0x80					
				nPacketItemCount++;
			}
			else
				setObjectDataToBuf(opcItem.readData);			
		}

		static void makeAndSendDataPacket(bool bServerAll, bool bGroupAll)
		{
			int			nServer = 0, nGroup = 0;

			if(bServerAll == false && checkServerName(ref nServer) == false) return;
			if(bGroupAll == false && checkGroupName(nServer, ref nGroup, true) == false) return;
			
			int						i, j;
			opcServerReadWriteClass opcServer;
			opcGroupReadWriteClass	opcGroup;

			nPacketNo = 0;
			nStartItemPos = 0;
			nPacketItemCount = 0;
			if(bServerAll) 
			{
				for(i = 0; i < opcBasic.arrOpcServer.Count; i++) 
				{
					opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[i];
					for(j = 0; j < opcServer.arrGroup.Count; j++) 
					{
						opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[j];
						oneGroupDataMakeBuf(opcGroup);
					}
				}
			}
			else if(bGroupAll) 
			{
				opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
				for(j = 0; j < opcServer.arrGroup.Count; j++) 
				{
					opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[j];
					oneGroupDataMakeBuf(opcGroup);
				}
			}
			else 
			{
				opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
				opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nGroup];
				oneGroupDataMakeBuf(opcGroup);
			}
			writeOnePacketData(true);			// 마지막 패킷전송, 데이터가 없더라도 한번 더 전송			
		}

		static public int writeItemValueToObject(eRWDataTypeCode flag, object type, int itemPos, ref int pos, out object val)
		{
			val = (int)0;
			if(itemPos < pos) return -1;				// 출력위치를 벗어났다.				
			try
			{
				if(opcBasic.isValueObjectIsSingleType(type)) 					
				{
					if(itemPos != pos++) return 0;		// 출력할 위치가 아니다					
				}
				val = type;
				if(type.GetType() == Opc.Type.STRING) 
				{
					if(flag != eRWDataTypeCode.STRING_DATA) return -1;
					val = writeString;					
				}
				else if(type.GetType() == Opc.Type.DATETIME) 
				{
					if(flag != eRWDataTypeCode.DATETIME_DATA) return -1;
					val = Convert.ToDateTime(writeString);					
				}
				else if(type.GetType() == Opc.Type.INT) 
				{
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					val = Convert.ToInt32(dWriteVal);
				}
				else if(type.GetType() == Opc.Type.SBYTE) 
				{
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					val = Convert.ToSByte(dWriteVal);
				}
				else if(type.GetType() == Opc.Type.FLOAT) 
				{
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					val = Convert.ToSingle(dWriteVal);
				}
				else if(type.GetType() == Opc.Type.DOUBLE) 
				{
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					val = dWriteVal;
				}
				else if(type.GetType() == Opc.Type.BYTE) 
				{
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					val = Convert.ToByte(dWriteVal);
				}
				else if(type.GetType() == Opc.Type.SHORT) 
				{
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					val = Convert.ToInt16(dWriteVal);
				}
				else if(type.GetType() == Opc.Type.USHORT) 
				{
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					val = Convert.ToUInt16(dWriteVal);
				}
				else if(type.GetType() == Opc.Type.UINT) 
				{
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					val = Convert.ToUInt32(dWriteVal);
				}
				else if(type.GetType() == Opc.Type.LONG) 
				{
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					val = Convert.ToInt64(dWriteVal);
				}
				else if(type.GetType() == Opc.Type.ULONG) 
				{
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					val = Convert.ToUInt64(dWriteVal);
				}
				else if(type.GetType() == Opc.Type.DECIMAL) 
				{
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					val = Convert.ToDecimal(dWriteVal);
				}
				else if(type.GetType() == Opc.Type.BOOLEAN) 
				{
                    if (flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
                    //val = Convert.ToBoolean(dWriteVal);   // delete 2007-12-12, 엠넥스텍에서 안되는 부분해결을 위해
                    if (Convert.ToInt32(dWriteVal) == 0) val = Convert.ToBoolean("False");// add 2007-12-12, 엠넥스텍에서 안되는 부분해결을 위해
                    else val = Convert.ToBoolean("True");// add 2007-12-12, 엠넥스텍에서 안되는 부분해결을 위해
				}
				else if(type.GetType() == Opc.Type.DURATION) 
				{
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					val = Convert.ToInt64(dWriteVal);
				}
				else if(val.GetType() == Opc.Type.ANY_TYPE) 
				{
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					val = dWriteVal;
				}
					// Array Type
					
				else if(type.GetType() == Opc.Type.ARRAY_STRING) 
				{
					string[] stringVal = (string[])val;
					if(stringVal.Length <= 0) return -1;		// array error
					if(itemPos-pos >= stringVal.Length)			// 출력위치가 아니다
					{
						pos += stringVal.Length;
						return 0;
					}
					if(flag != eRWDataTypeCode.STRING_DATA) return -1;
					stringVal[itemPos-pos] = writeString;
					val = stringVal;					
				}
				else if(type.GetType() == Opc.Type.ARRAY_DATETIME) 
				{					
					DateTime[] dateTimeVal = (DateTime[])val;
					if(dateTimeVal.Length <= 0) return -1;		// array error
					if(itemPos-pos >= dateTimeVal.Length)		// 출력위치가 아니다
					{
						pos += dateTimeVal.Length;
						return 0;
					}
					if(flag != eRWDataTypeCode.DATETIME_DATA) return -1;
					dateTimeVal[itemPos-pos] = Convert.ToDateTime(writeString);
					val = dateTimeVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_INT) 
				{
					int[] iVal = (int[])val;
					if(iVal.Length <= 0) return -1;				// array error
					if(itemPos-pos >= iVal.Length)				// 출력위치가 아니다
					{
						pos += iVal.Length;
						return 0;
					}
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					iVal[itemPos-pos] = Convert.ToInt32(dWriteVal);
					val = iVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_SBINARY) 
				{
					sbyte[] sbyteVal = (sbyte[])val;
					if(sbyteVal.Length <= 0) return -1;			// array error
					if(itemPos-pos >= sbyteVal.Length)			// 출력위치가 아니다
					{
						pos += sbyteVal.Length;
						return 0;
					}
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					sbyteVal[itemPos-pos] = Convert.ToSByte(dWriteVal);
					val = sbyteVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_FLOAT) 
				{
					float[] floatVal = (float[])val;
					if(floatVal.Length <= 0) return -1;			// array error
					if(itemPos-pos >= floatVal.Length)			// 출력위치가 아니다
					{
						pos += floatVal.Length;
						return 0;
					}
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					floatVal[itemPos-pos] = Convert.ToSingle(dWriteVal);
					val = floatVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_DOUBLE) 
				{
					double[] doubleVal = (double[])val;
					if(doubleVal.Length <= 0) return -1;		// array error
					if(itemPos-pos >= doubleVal.Length)			// 출력위치가 아니다
					{
						pos += doubleVal.Length;
						return 0;
					}
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					doubleVal[itemPos-pos] = dWriteVal;
					val = doubleVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_BINARY) 
				{
					byte[] byteVal = (byte[])val;
					if(byteVal.Length <= 0) return -1;			// array error
					if(itemPos-pos >= byteVal.Length)			// 출력위치가 아니다
					{
						pos += byteVal.Length;
						return 0;
					}
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					byteVal[itemPos-pos] = Convert.ToByte(dWriteVal);
					val = byteVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_SHORT) 
				{
					short[] shortVal = (short[])val;
					if(shortVal.Length <= 0) return -1;			// array error
					if(itemPos-pos >= shortVal.Length)			// 출력위치가 아니다
					{
						pos += shortVal.Length;
						return 0;
					}
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					shortVal[itemPos-pos] = Convert.ToInt16(dWriteVal);
					val = shortVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_USHORT) 
				{
					ushort[] ushortVal = (ushort[])val;
					if(ushortVal.Length <= 0) return -1;		// array error
					if(itemPos-pos >= ushortVal.Length)			// 출력위치가 아니다
					{
						pos += ushortVal.Length;
						return 0;
					}
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					ushortVal[itemPos-pos] = Convert.ToUInt16(dWriteVal);
					val = ushortVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_UINT) 
				{
					uint[] uintVal = (uint[])val;
					if(uintVal.Length <= 0) return -1;			// array error
					if(itemPos-pos >= uintVal.Length)			// 출력위치가 아니다
					{
						pos += uintVal.Length;
						return 0;
					}
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					uintVal[itemPos-pos] = Convert.ToUInt32(dWriteVal);
					val = uintVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_LONG) 
				{
					long[] longVal = (long[])val;
					if(longVal.Length <= 0) return -1;			// array error
					if(itemPos-pos >= longVal.Length)			// 출력위치가 아니다
					{
						pos += longVal.Length;
						return 0;
					}
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					longVal[itemPos-pos] = Convert.ToInt64(dWriteVal);
					val = longVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_ULONG) 
				{
					ulong[] ulongVal = (ulong[])val;
					if(ulongVal.Length <= 0) return -1;			// array error
					if(itemPos-pos >= ulongVal.Length)			// 출력위치가 아니다
					{
						pos += ulongVal.Length;
						return 0;
					}
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					ulongVal[itemPos-pos] = Convert.ToUInt64(dWriteVal);
					val = ulongVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_DECIMAL) 
				{
					decimal[] decimalVal = (decimal[])val;
					if(decimalVal.Length <= 0) return -1;			// array error
					if(itemPos-pos >= decimalVal.Length)			// 출력위치가 아니다
					{
						pos += decimalVal.Length;
						return 0;
					}
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					decimalVal[itemPos-pos] = Convert.ToDecimal(dWriteVal);
					val = decimalVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_BOOLEAN) 
				{
					bool[] boolVal = (bool[])val;
					if(boolVal.Length <= 0) return -1;				// array error
					if(itemPos-pos >= boolVal.Length)				// 출력위치가 아니다
					{
						pos += boolVal.Length;
						return 0;
					}
					if(flag != eRWDataTypeCode.DOUBLE_DATA && flag != eRWDataTypeCode.FLOAT_DATA) return -1;
					boolVal[itemPos-pos] = Convert.ToBoolean(dWriteVal);
					val = boolVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_ANY_TYPE) 
				{
					object[] objectVal = (object[])val;
					object		newVal;
					int			retn;
					for(int i = 0; i < objectVal.Length; i++) 					
					{
						retn = writeItemValueToObject(flag, objectVal[i], itemPos, ref pos, out newVal);
						if(retn == 0) continue;
						if(retn == 1) objectVal[i] = newVal;
						return retn;
					}
					if(itemPos >= pos) return 0;				// 출력위치가 아니다
				}
				else return -1;
			}
			catch
			{
				return -1;
			}
			return 1;
		}


			/*
		static private bool writeItemValueToObject(eRWDataTypeCode flag, object type, int itemPos, ref int writePos, out object val)
		{
			//if(itemPos != writePos &&

			val = (int)0;
			try
			{				
				if(flag == eRWDataTypeCode.STRING_DATA) 
				{
					if(type.GetType() == Opc.Type.STRING) 
					{
						val = writeString;
						return true;
					}
					else if(type.GetType() == Opc.Type.ARRAY_STRING) 
					{
						string[] stringVal = (string[])val;
						if(stringVal.Length <= itemPos) return false;
						stringVal[itemPos] = writeString;
						val = stringVal;
						return true;
					}
					else return false;
				}
				if(flag == eRWDataTypeCode.DATETIME_DATA) 
				{
					if(type.GetType() == Opc.Type.DATETIME) 
					{
						val = Convert.ToDateTime(writeString);
						return true;
					}
					else if(type.GetType() == Opc.Type.ARRAY_DATETIME) 
					{
						DateTime[] dateTimeVal = (DateTime[])val;
						if(dateTimeVal.Length <= itemPos) return false;
						dateTimeVal[itemPos] = Convert.ToDateTime(writeString);
						val = dateTimeVal;
						return true;
					}
					else return false;
				}
				val = type;				
				if(type.GetType() == Opc.Type.INT) val = Convert.ToInt32(dWriteVal);
				else if(type.GetType() == Opc.Type.SBYTE) val = Convert.ToSByte(dWriteVal);					
				else if(type.GetType() == Opc.Type.FLOAT) val = Convert.ToSingle(dWriteVal);
				else if(type.GetType() == Opc.Type.DOUBLE) val = dWriteVal;
				else if(type.GetType() == Opc.Type.BYTE) val = Convert.ToByte(dWriteVal);
				else if(type.GetType() == Opc.Type.SHORT) val = Convert.ToInt16(dWriteVal);
				else if(type.GetType() == Opc.Type.USHORT) val = Convert.ToUInt16(dWriteVal);
				else if(type.GetType() == Opc.Type.UINT) val = Convert.ToUInt32(dWriteVal);
				else if(type.GetType() == Opc.Type.LONG) val = Convert.ToInt64(dWriteVal);
				else if(type.GetType() == Opc.Type.ULONG) val = Convert.ToUInt64(dWriteVal);
				else if(type.GetType() == Opc.Type.DECIMAL) val = Convert.ToDecimal(dWriteVal);
				else if(type.GetType() == Opc.Type.BOOLEAN) val = Convert.ToBoolean(dWriteVal);
				else if(type.GetType() == Opc.Type.DURATION) val = Convert.ToInt64(dWriteVal);
				else if(val.GetType() == Opc.Type.ANY_TYPE) val = dWriteVal;
					// Array Type
					
				else if(type.GetType() == Opc.Type.ARRAY_INT) 
				{
					int[] iVal = (int[])val;
					if(iVal.Length <= itemPos) return false;
					iVal[itemPos] = Convert.ToInt32(dWriteVal);
					val = iVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_SBINARY) 
				{
					sbyte[] sbyteVal = (sbyte[])val;
					if(sbyteVal.Length <= itemPos) return false;
					sbyteVal[itemPos] = Convert.ToSByte(dWriteVal);
					val = sbyteVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_FLOAT) 
				{
					float[] floatVal = (float[])val;
					if(floatVal.Length <= itemPos) return false;
					floatVal[itemPos] = Convert.ToSingle(dWriteVal);
					val = floatVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_DOUBLE) 
				{
					double[] doubleVal = (double[])val;
					if(doubleVal.Length <= itemPos) return false;
					doubleVal[itemPos] = dWriteVal;
					val = doubleVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_BINARY) 
				{
					byte[] byteVal = (byte[])val;
					if(byteVal.Length <= itemPos) return false;
					byteVal[itemPos] = Convert.ToByte(dWriteVal);
					val = byteVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_SHORT) 
				{
					short[] shortVal = (short[])val;
					if(shortVal.Length <= itemPos) return false;
					shortVal[itemPos] = Convert.ToInt16(dWriteVal);
					val = shortVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_USHORT) 
				{
					ushort[] ushortVal = (ushort[])val;
					if(ushortVal.Length <= itemPos) return false;
					ushortVal[itemPos] = Convert.ToUInt16(dWriteVal);
					val = ushortVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_UINT) 
				{
					uint[] uintVal = (uint[])val;
					if(uintVal.Length <= itemPos) return false;
					uintVal[itemPos] = Convert.ToUInt32(dWriteVal);
					val = uintVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_LONG) 
				{
					long[] longVal = (long[])val;
					if(longVal.Length <= itemPos) return false;
					longVal[itemPos] = Convert.ToInt64(dWriteVal);
					val = longVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_ULONG) 
				{
					ulong[] ulongVal = (ulong[])val;
					if(ulongVal.Length <= itemPos) return false;
					ulongVal[itemPos] = Convert.ToUInt64(dWriteVal);
					val = ulongVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_DECIMAL) 
				{
					decimal[] decimalVal = (decimal[])val;
					if(decimalVal.Length <= itemPos) return false;
					decimalVal[itemPos] = Convert.ToDecimal(dWriteVal);
					val = decimalVal;
				}
				else if(type.GetType() == Opc.Type.ARRAY_BOOLEAN) 
				{
					bool[] boolVal = (bool[])val;
					if(boolVal.Length <= itemPos) return false;
					boolVal[itemPos] = Convert.ToBoolean(dWriteVal);
					val = boolVal;
				}
				//else if(type.GetType() == Opc.Type.ARRAY_ANY_TYPE) 
				//{
				//	object[] objectVal = (object[])val;
				//	for(int i = 0; i < objectVal.Length; i++) 
				//	{
				//		writeItemValueToObject(flag, objectVal[i], itemPos, ref writePos, out val);
				//	}
				//}
				else return false;
			}
			catch
			{
				return false;
			}
			return true;
		}*/

		/*static void checkObjectValueAndWrite(opcServerReadWriteClass opcServer, string itemName, object readData, eRWDataTypeCode flag, int itemPos)
		{
			object		val;
			if(writeItemValueToObject(flag, readData, out val) == false) 
			{
				writeErrorDataPacket(eErrorCodeType.WRITE_ITEM_TYPE_ERROR, true);
				return;
			}

			Opc.IdentifiedResult[] results = opcReadWriteGroupLoopClass.writeOneItem(opcServer, itemName, val);
			
			if (results == null)
			{
				writeErrorDataPacket(eErrorCodeType.WRITE_ITEM_ERROR, true);
				return;
			}
			writeErrorDataPacket(eErrorCodeType.NONE_ERROR, true);
		}*/

		static void writeReadAfterWrite(opcServerReadWriteClass opcServer, opcItemReadWriteClass opcItem, string itemName, eRWDataTypeCode flag, int itemPos)
		{
            //ItemValueResult[] readResults = opcReadWriteGroupLoopClass.readOneItemData(opcServer, itemName, false);     // 처음에는 cache 모드로 읽고
            ItemValueResult[] readResults = opcReadWriteGroupLoopClass.readOneItemData(opcServer, itemName, opcBasic.opcClientConfig.bUseDevicePeriodicReadMode);
            // 이전에는 false 이었으나 중앙제어에서 아직 아이템이 읽지 않아서 활성화가 안되었을 때 출력이 안나간다고 해서 수정함.(9에서는 잘되었다고 함) 2015-2-23 
            
			if (readResults == null) //(readResults[0].ResultID != Opc.ResultID.E_WRITEONLY && readResults[0].Value == null)) deleted 2004-07-23
			{		
				writeErrorDataPacket(eErrorCodeType.WRITE_ITEM_ERROR, true);
				return;
			}

			object		val;
			int			writePos = 0;

			if(readResults[0].ResultID != Opc.ResultID.S_OK)// == Opc.ResultID.E_WRITEONLY)  modify 2004-07-23
			{
				ItemValue item = new ItemValue();
				item.ItemName = itemName;
				opcBasic.GetDefaultValues(opcServer.m_server, new ItemValue[] { item });
				if(item.Value == null) readResults[0].Value = 0;			// Null 일 때는 기본으로 Int32로 설정
				else				   readResults[0].Value = item.Value;				
			}
			
			if(opcBasic.isValueObjectIsSingleType(readResults[0].Value)) itemPos = 0;		// 싱글 타입일 경우는 항상 0, 출력할 번지가 없다
			if(writeItemValueToObject(flag, readResults[0].Value, itemPos, ref writePos, out val) != 1) 
			{
				writeErrorDataPacket(eErrorCodeType.WRITE_ITEM_TYPE_ERROR, true);
				return;
			}

			Opc.IdentifiedResult[] writeResults = opcReadWriteGroupLoopClass.writeOneItem(opcServer, itemName, val);
            //opcItem.nCurrWriteCount = 1;        //2008-10-27 add 엠넥스텍에서 요구한 여러번 출력을 위해
            //opcItem.writeVal = val;             //2008-10-27 add 엠넥스텍에서 요구한 여러번 출력을 위해
            //opcItem.itemPos = itemPos;          //2008-10-28 add 엠넥스텍에서 요구한 여러번 출력을 위해            
            //DateTime dt = DateTime.Now;         //2008-10-28 add 엠넥스텍에서 요구한 여러번 출력을 위해            
            //opcItem.writeStartTime = dt.Minute * 60 + dt.Second; //2008-10-28 add 엠넥스텍에서 요구한 여러번 출력을 위해, 1초 단위로 저장

            if (opcBasic.opcClientConfig.bUseDeviceReadMode)        // 2008-10-31 add
            {
                readResults = opcReadWriteGroupLoopClass.readOneItemData(opcServer, itemName, true);     // device 모드로 읽고
                if (readResults != null)
                {
                    opcItem.bNewRead = true;
                    opcItem.reuslt = readResults[0].ResultID;
                    opcItem.timeStamp = readResults[0].Timestamp;
                    opcItem.quality = readResults[0].Quality;
                    if (readResults[0].Value == null)
                    {
                        opcItem.bFlag = false;
                    }
                    else
                    {
                        opcItem.bFlag = true;
                        if (opcItem.readData != (object)readResults[0].Value) // 값이 바뀌었으면
                        {
                            opcItem.readData = (object)readResults[0].Value;                            
                        }                        
                    }
                }
            }
			
			if (writeResults == null)
			{
				writeErrorDataPacket(eErrorCodeType.WRITE_ITEM_ERROR, true);
				return;
			}
			writeErrorDataPacket(eErrorCodeType.NONE_ERROR, true);
            
			if(readResults[0].ResultID != Opc.ResultID.S_OK)// == Opc.ResultID.E_WRITEONLY)  modify 2004-07-23
			{
				if(opcItem == null) return;
				opcItem.bNewRead = true;
				opcItem.readData = val;
			}


			//checkObjectValueAndWrite(opcServer, itemName, results[0].Value, flag, int itemPos);
		}

        /*static void checkWritedDataReWrite(opcServerReadWriteClass opcServer, opcItemReadWriteClass opcItem) //2008-10-28 add 엠넥스텍에서 요구한 여러번 출력을 위해
        {
            if (opcBasic.opcClientConfig.bUseReWriteCheck == false || opcItem.nCurrWriteCount == 0) return;

            DateTime dt = DateTime.Now;         //2008-10-28 add 엠넥스텍에서 요구한 여러번 출력을 위해
            int currTime = dt.Minute * 600 + dt.Second * 10 + dt.Millisecond / 100;
            if(currTime < opcItem.writeStartTime) currTime += 3600;// 1시간을 더해준다.
            currTime = currTime-opcItem.writeStartTime;
            if (currTime < opcBasic.opcClientConfig.nReWriteCheckTime) return;// 지정한 시간이 지나지 않았다

            if (opcItem.writeVal == opcItem.readData || opcItem.nCurrWriteCount > opcBasic.opcClientConfig.nReWriteCheckCount)
            {
                opcItem.nCurrWriteCount = 0;
                return; 
            }
            opcItem.nCurrWriteCount++;

            ItemValueResult[] readResults = opcReadWriteGroupLoopClass.readOneItemData(opcServer, opcItem.itemName);
            if (readResults == null)
            {
                return;
            }
            Opc.IdentifiedResult[] writeResults = opcReadWriteGroupLoopClass.writeOneItem(opcServer, opcItem.itemName, opcItem.writeVal);
            if (writeResults == null)
            {
                return;
            }
            if (readResults[0].ResultID != Opc.ResultID.S_OK)
            {
                if (opcItem == null) return;
                //opcItem.bNewRead = true;
                //opcItem.readData = val;
            }
        }*/
        
		static bool checkGroupItemName(int nServer, ref int nGroup, ref int nItem)
		{
			if(checkGroupName(nServer, ref nGroup, false) == false) return false;
			if(checkItemName(nServer, nGroup, ref nItem) == false) return false;
			return true;
		}
		

		static void writeItemAndSendDataPacket(eRWDataTypeCode flag, ushort itemPos)
		{
			int			nServer = 0, nGroup = 0, nItem = 0;

			if(checkServerName(ref nServer) == false) return;
			opcServerReadWriteClass opcServer = (opcServerReadWriteClass)opcBasic.arrOpcServer[nServer];
			if(opcServer.m_server == null || opcServer.m_server.IsConnected == false) 
			{
				writeErrorDataPacket(eErrorCodeType.SERVER_CONNECTION_ERROR, true);
				return;
			}

			if(checkGroupItemName(nServer, ref nGroup, ref nItem) == false) 
			{								// 그룹이름이 없을 때... 읽어서 다시 출력
				writeReadAfterWrite(opcServer, null, readItemName, flag, itemPos);
				return;
			}			

			opcGroupReadWriteClass opcGroup = (opcGroupReadWriteClass)opcServer.arrGroup[nGroup];
			opcItemReadWriteClass opcItem = (opcItemReadWriteClass)opcGroup.arrItem[nItem];
			writeReadAfterWrite(opcServer, opcItem, opcItem.itemName, flag, itemPos);		// 항상 읽은 후에 출력
			//if(opcItem.bFlag == false)
			//{
			//	writeReadAfterWrite(opcServer, opcItem.itemName, flag);
			//	return;
			//}
			//if(opcBasic.isValueObjectIsArrayType(opcItem.readData)) // array 타입은 항상 읽은 후애
			//	writeReadAfterWrite(opcServer, opcItem.itemName, flag);
			//else
			//heckObjectValueAndWrite(opcServer, opcItem.itemName, opcItem.readData, flag);
		}

		
		static void writeErrorDataPacket(eErrorCodeType errCode, bool bEnd)
		{
			int			i, buf_pos = 0;

			for(i = 0; i < 10; i++) sendBuf[buf_pos++] = startCode[i];
			buf_pos += 2;
			sendBuf[buf_pos++] = (byte)cReadCommand;		// read Data Response
			sendBuf[buf_pos++] = (byte)(nTns/256);			// Trans
			sendBuf[buf_pos++] = (byte)(nTns%256);
			sendBuf[buf_pos++] = (byte)(errCode);			//eErrorCodeType.NONE_ERROR);
			sendBuf[buf_pos++] = (byte)(nPacketItemCount/256);// Send Data Count
			sendBuf[buf_pos++] = (byte)(nPacketItemCount%256);// 19개의 header 데이터			
			sendBuf[buf_pos++] = (byte)(nPacketNo/256);		// 패킷순서번호
			sendBuf[buf_pos++] = (byte)(nPacketNo%256);		//
			sendBuf[buf_pos++] = (byte)(nStartItemPos/256);	// 시작 아이템 순서번호 0 ~ 
			sendBuf[buf_pos++] = (byte)(nStartItemPos%256);
			sendBuf[buf_pos++] = (bEnd) ? (byte)1 : (byte)0;// packet의 종료이냐?
			sendBuf[10] = (byte)((buf_pos+2)/256);	// total size
			sendBuf[11] = (byte)((buf_pos+2)%256);
			ushort crc = basicTool.GetCRC_SumWORD(sendBuf, buf_pos);
			sendBuf[buf_pos++] = (byte)(crc/256);			// Sum WORD crc
			sendBuf[buf_pos++] = (byte)(crc%256);			

			for(i = 0; i < buf_pos; i++) 
			{
				sharedDevice.Write(sendBuf[i]);
			}
			nPacketNo++;
			nStartItemPos++;
			nPacketItemCount = 0;
			nSendBufPos = nHeaderSize;
		}


		static void writeOnePacketData(bool bEnd)
		{
			if(nSendBufPos > nMaxSendBufCount) 
			{
				writeErrorDataPacket(eErrorCodeType.ITEM_DATA_SIZE_TOO_BIG, bEnd);
				return;//	// 버퍼 최대크기보다 크면...
			}

			int			i, buf_pos = 0;
			for(i = 0; i < 10; i++) sendBuf[buf_pos++] = startCode[i];
			sendBuf[buf_pos++] = (byte)((nSendBufPos+7)/256);	// total size
			sendBuf[buf_pos++] = (byte)((nSendBufPos+7)%256);
			sendBuf[buf_pos++] = (byte)cReadCommand;		// read Data Response
			sendBuf[buf_pos++] = (byte)(nTns/256);			// Trans
			sendBuf[buf_pos++] = (byte)(nTns%256);
			sendBuf[buf_pos++] = (byte)eErrorCodeType.NONE_ERROR;
			sendBuf[buf_pos++] = (byte)(nPacketItemCount/256);// Send Data Count
			sendBuf[buf_pos++] = (byte)(nPacketItemCount%256);// 19개의 header 데이터			
			//..... 중간에 데이터가 들어감
			sendBuf[nSendBufPos++] = (byte)(nPacketNo/256);		// 패킷순서번호
			sendBuf[nSendBufPos++] = (byte)(nPacketNo%256);		//
			sendBuf[nSendBufPos++] = (byte)(nStartItemPos/256);	// 시작 아이템 순서번호 0 ~ 
			sendBuf[nSendBufPos++] = (byte)(nStartItemPos%256);
			sendBuf[nSendBufPos++] = (bEnd) ? (byte)1 : (byte)0;// packet의 종료이냐?
			ushort crc = basicTool.GetCRC_SumWORD(sendBuf, nSendBufPos);
			sendBuf[nSendBufPos++] = (byte)(crc/256);			// Sum WORD crc
			sendBuf[nSendBufPos++] = (byte)(crc%256);

			for(i = 0; i < nSendBufPos; i++) 
			{
				sharedDevice.Write(sendBuf[i]);
			}
			nPacketNo++;
			nStartItemPos += nPacketItemCount;
			nPacketItemCount = 0;
			nSendBufPos = nHeaderSize;
			if(bEnd == false) readAckData();		// 마지막 패킷이 아니면 ACK를 확인
		}

		static bool readAckData()
		{			
			int				retn;			
			TimeOutClass	timeout = new TimeOutClass();
			
			timeout.Reset();
			while(true) 
			{
				Thread.Sleep(1);
				if(timeout.IsTimeOut(2)) return false;				
				retn = sharedDevice.Read();
				if(retn == -1) continue;
				if((eRWDataType)(retn % 256) == eRWDataType.ACK_DATA) return true;				
			}			
		}





	}
}
