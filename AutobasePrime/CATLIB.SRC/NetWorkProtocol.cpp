// TAG size O.K
#include "stdafx.h"

#include <tools.h>
#include <totaldef.h>
#include <crc.hpp>
#include <dataswap.h>

#include "..\catlib.src\NetWorkProtocol.h"

WORD GetTransaction()
{
	static WORD id = 1;

	id++;
	return id;
}

NetWorkProtocolRecv::NetWorkProtocolRecv()
{
	int i;
	
	nNodeType = 0;
	strcpy(sNodeName, "???");

	nPort = 0;			// plc scan port
	nBroadCastPort = 0;	// plc scan 포트에서 제공받고자 하는 Port
	nStation = 0;		// plc scan station
	dwAddress = 0;		// plc scan address
	sExtra1[0] = 0;		// Plc Scan Extra1
	wExtra2 = 0;		// Plc Scan Extra2
	fValue = 0;			// value

//	smBufType = 0;
//	smDeviceType[0] = 0;
//	smTarget = 0;
//	smSize = 1;
	sTag[0] = 0;
	wTagMember = 0;
	nBlockSize = 0;
	Block = NULL;

	sString = new char[1];
	sString[0] = 0;

	memset(&tDateTime, 0, sizeof(SYSTEMTIME));

	wTransaction = 0;

	bServerActive = 1;
	bPlcScanTimeOut = 0;
	bAnotherProgramExit = 0;

	for(i = 0; i < 16; i++) {
		wBroadCastPorts[i] = 0;
	}

	nVersionMajor = 0;
	nVersionMinor = 0;
}

NetWorkProtocolRecv::~NetWorkProtocolRecv()
{
	FreeBuf();
}

void NetWorkProtocolRecv::FreeBuf()
{
	if(Block) {
		delete Block;
		Block = NULL;
	}
	if(sString) {
		delete sString;
		sString = NULL;
	}
}

int NetWorkProtocolRecv :: Split(void *splitBuf, int nRecvCount)
{
	char *recvBuf = (char*)splitBuf;
	
	if(nRecvCount < 10)					return 0;
	if(recvBuf[0] != STX)				return 0;
	if(recvBuf[nRecvCount-1] != ETX)	return 0;

	WORD crc = GetCRC_SumWORD(&recvBuf[1], nRecvCount-6);
	if(crc != HexBufToWORD(&recvBuf[nRecvCount-5]))	return 0;
	wCommand = HexBufToWORD(&recvBuf[1]);

	wTransaction = HexBufToWORD(&recvBuf[5]);
	
	FreeBuf();
	int data_size = HexBufToWORD(&recvBuf[9]);

	if(data_size) {
		StackChar buf(data_size+1);
		strncpy(buf.data, &recvBuf[13], data_size);
		buf.data[data_size] = 0;

		CommaBlockString comma;
		comma.Set(buf.data);
		while(1) {
			comma.GetString(buf.data, data_size+1);
			if(strlen(buf.data) == 0)	break;

			if(strnicmp(buf.data, "NodeType=", 9) == 0) {
				nNodeType = atoi(&buf.data[9]);
			}
			else if(strnicmp(buf.data, "NodeName=", 9) == 0) {
				strcpy(sNodeName, &buf.data[9]);
			}
			else if(strnicmp(buf.data, "Port=", 5) == 0) {
				nPort = atoi(&buf.data[5]);
			}
			else if(strnicmp(buf.data, "BroadCastPort=", 14) == 0) {
				nBroadCastPort = atoi(&buf.data[14]);
			}
			else if(strnicmp(buf.data, "BroadCastPorts=", 15) == 0) {
				for(int i = 0; i < 16; i++) {
					wBroadCastPorts[i] = HexBufToWORD(&buf.data[15+i*4]);
				}
			}

			else if(strnicmp(buf.data, "Station=", 8) == 0) {
				nStation = atoi(&buf.data[8]);
			}
			else if(strnicmp(buf.data, "Address=", 8) == 0) {
				dwAddress = atoi(&buf.data[8]);
			}
			else if(strnicmp(buf.data, "Extra1=", 7) == 0) {
				strcpy(sExtra1, &buf.data[7]);
			}
			else if(strnicmp(buf.data, "Extra2=", 7) == 0) {
				wExtra2 = atoi(&buf.data[7]);
			}
			else if(strnicmp(buf.data, "Value=", 6) == 0) {
				fValue = atof(&buf.data[6]);
				i64Value = _atoi64(&buf.data[6]);
			}
			/*
			else if(strnicmp(buf.data, "smBufType=", 10) == 0) {
				smBufType = atoi(&buf.data[10]);
			}
			else if(strnicmp(buf.data, "smDeviceType=", 13) == 0) {
				strcpy(smDeviceType, &buf.data[13]);
			}
			else if(strnicmp(buf.data, "smTarget=", 9) == 0) {
				smTarget = atoi(&buf.data[9]);
			}
			else if(strnicmp(buf.data, "smSize=", 7) == 0) {
				smSize = atoi(&buf.data[7]);
			}
			*/
			else if(strnicmp(buf.data, "Tag=", 4) == 0) {
				strcpy(sTag, &buf.data[4]);
			}
			else if(strnicmp(buf.data, "TagMember=", 10) == 0) {
				wTagMember = atoi(&buf.data[10]);
			}
			else if(strnicmp(buf.data, "Year=", 5) == 0) {
				tDateTime.wYear = atoi(&buf.data[5]);
			}
			else if(strnicmp(buf.data, "Mon=", 4) == 0) {
				tDateTime.wMonth = atoi(&buf.data[4]);
			}
			else if(strnicmp(buf.data, "Day=", 4) == 0) {
				tDateTime.wDay = atoi(&buf.data[4]);
			}
			else if(strnicmp(buf.data, "Hour=", 5) == 0) {
				tDateTime.wHour = atoi(&buf.data[5]);
			}
			else if(strnicmp(buf.data, "Min=", 4) == 0) {
				tDateTime.wMinute = atoi(&buf.data[4]);
			}
			else if(strnicmp(buf.data, "Sec=", 4) == 0) {
				tDateTime.wSecond = atoi(&buf.data[4]);
			}
			else if(strnicmp(buf.data, "BlockSize=", 10) == 0) {
				nBlockSize = atoi(&buf.data[10]);
			}
			else if(strnicmp(buf.data, "Block=", 6) == 0) {
				if(Block) {
					delete Block;
					Block = NULL;
				}
				if(nBlockSize > 0 && nBlockSize*2 == (short)strlen(&buf.data[6])) {
					Block = new BYTE[nBlockSize];
					for(int i = 0; i < nBlockSize; i++) {
						Block[i] = HexBufToBYTE(&buf.data[6+i*2]);
					}
				}
				else if(nBlockSize > 0 && nBlockSize*2 < (short)strlen(&buf.data[6])) {
					Block = new BYTE[nBlockSize];
					for(int i = 0; i < nBlockSize; i++) {
						Block[i] = HexBufToBYTE(&buf.data[6+i*2]);
					}
				}
				else if(nBlockSize == 0) {
					Block = new BYTE[1];
					Block[0] = 0;
				}
				else {
					CString msg;
					msg.Format("nBlockSize and Block count is mismatched");
					Block = new BYTE[strlen(msg)+1];
					strcpy((char*)Block, msg);
					nBlockSize = (short)strlen(msg);
				}
			}
			else if(strnicmp(buf.data, "String=", 7) == 0) {
				int count = (int)strlen(&buf.data[7]);
				if(sString) {
					delete sString;
					sString = NULL;
				}
				if(count) {
					sString = new char[count+1];
					strcpy(sString, &buf.data[7]);
				}
			}
			else if(strnicmp(buf.data, "ServerActive=", 13) == 0) {
				bServerActive = atoi(&buf.data[13]);
			}
			else if(strnicmp(buf.data, "PSTO=", 5) == 0) {
				bPlcScanTimeOut = atoi(&buf.data[5]);
			}
			else if(strnicmp(buf.data, "APE=", 4) == 0) {
				bAnotherProgramExit = atoi(&buf.data[4]);
			}
			else if(strnicmp(buf.data, "AW=", 3) == 0) {
				bAutoWatch = atoi(&buf.data[3]);
			}
			else if(strnicmp(buf.data, "Version=", 8) == 0) {
				CommaBlockString comma_v;
				comma_v.SetBlockCode('.');
				comma_v.Set(&buf.data[8]);
				comma_v.GetInt(nVersionMajor);
				comma_v.GetInt(nVersionMinor);
			}
			else;
		}
	}
		
	return 1;
}

NetWorkProtocolSend::NetWorkProtocolSend()
{
	bufSend = NULL;
}

NetWorkProtocolSend::~NetWorkProtocolSend()
{
	if(bufSend != NULL)	delete bufSend;
}

void NetWorkProtocolSend::MakeBlock(WORD command, WORD trans, const char *buf, WORD size)
{
	if(bufSend != NULL)	delete bufSend;
	bufSend = new char[size+100];

	if(bufSend == NULL)	return;

	int count = 0;
	WORD crc;

	bufSend[count] = STX;
	count++;
	sprintf(&bufSend[count], "%04X", command);
	count+=4;
	sprintf(&bufSend[count], "%04X", trans);
	count+=4;
	sprintf(&bufSend[count], "%04X", size);
	count+=4;
	if(size > 0) {
		memcpy(&bufSend[count], buf, size);
		count+=size;
		
		/*
		if((size%2) != 0) {	// 전체 블럭의 갯수를 짝수로 맞추어 주지 않으면 NT에서는 인식을 하지 못한다.
			bufSend[count] = ',';
			count++;
		}
		*/
	}

	crc = GetCRC_SumWORD(&bufSend[1], count-1);
	sprintf(&bufSend[count], "%04X", crc);
	count+=4;
	bufSend[count] = ETX;
	count++;

	nBufCount = count;
}

int SendEventAndShareMemory(TCHAR *name, void *buf, int count)
{
	char name_buf[160];

	SHARE_PLCSCAN_NETWORK *sharePtr = NULL;
	SharedMemory share;

	sprintf(name_buf, "Share%s", name);

	share.InitByExist(name_buf, sizeof(SHARE_PLCSCAN_NETWORK));

	if(share.ptr == NULL)	return 0;

	sharePtr = (SHARE_PLCSCAN_NETWORK*)share.ptr;

	TimeOutClass timeout;
	while(1) {
		if(timeout.IsTimeOut(3)) {	// 다른 프로세서가 잡고 있다.
			return 0;
		}
		if(sharePtr->bEvent == OFF)	break;
	}

	sharePtr->size = count;
	memcpy(sharePtr->buf, buf, count);

	// string 문자열로 바꾼다.
	if(count < 5000)	sharePtr->buf[count] = 0;

	sharePtr->bEvent = ON;
	
	// 항목의 출력을 받았는가를 검사한다.
	timeout.Reset();
	while(1) {
		if(timeout.IsTimeOut(3))	{
			//MessageDisplay("PlcDeviceWriteContinueNetClient() - SetEvent Is TimeOuted");
			break;
		}
		if(sharePtr->bEvent == OFF)	break;
	}
	
	if(sharePtr) {
		share.Uninit();
		sharePtr = NULL;
	}

	return 1;
}

int SendEventProgramToNetwork(char *buf, int count)
{
	return SendEventAndShareMemory("EtcProgramToNetManager", buf, count);
}

int SendEventNetworkToViewMain(char *buf, int count)
{
	return SendEventAndShareMemory("NetWorkToViewMain", buf, count);
}

//-------------------------------------------------------------------------------------
//	Plc Scan Programm에서 발생하는 Event를 처리한다.
//-------------------------------------------------------------------------------------

RecvEventAndShareMemory :: RecvEventAndShareMemory(TCHAR *name)
{
	TCHAR name_buf[160];

	shareClass = new SharedMemory;
	if(shareClass) {
		sprintf(name_buf, "Share%s", name);
		shareClass->Init(name_buf, sizeof(SHARE_PLCSCAN_NETWORK));
	}
}

RecvEventAndShareMemory :: ~RecvEventAndShareMemory()
{
	if(shareClass) {
		shareClass->Uninit();
		delete shareClass;
		shareClass = NULL;
	}
}

void DisplaySendCode(char *buf, int size);

int RecvEventAndShareMemory::IsEvent()
{
	if(shareClass == NULL)		return 0;
	if(shareClass->ptr == NULL)	return 0;

	SHARE_PLCSCAN_NETWORK *share = (SHARE_PLCSCAN_NETWORK*) shareClass->ptr;

	if(share->bEvent) {
		return 1;
	}

	return 0;
}

void RecvEventAndShareMemory::ResetEvent()
{
	if(shareClass == NULL)		return;
	if(shareClass->ptr == NULL)	return;

	SHARE_PLCSCAN_NETWORK *share = (SHARE_PLCSCAN_NETWORK*) shareClass->ptr;

	share->bEvent = OFF;
}

void *RecvEventAndShareMemory::GetPointer()
{
	return shareClass->ptr;
}




