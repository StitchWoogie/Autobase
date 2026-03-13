// english O.K
//------------------------------------------------------------------------------
//	AB PLC-5 Protocol
//	만들어진 lib 파일을 protocol main 과 링크시키면 된다.
//	view main 과는 연계될 필요가 없다.
// commmain.lib 파일을 함께 링크한다.
//------------------------------------------------------------------------------

#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <dos.h>

#include <totaldef.h>
#include <tools.h>
#include <glib.h>

#include "..\plc_scan.h" 
#include "pro_lib.h"
#include "pro_main.h"

void PlcScanDrawMethodTitleAbplc5(HDC hdc, int x, int y)
{
	char *string = "ST, TYPE, ADDR, BUF, SIZE, (AB PLC-5)";

	TextOut(hdc, x, y, string, strlen(string));
}

void PlcScanDrawMethodAbplc5(HDC hdc, int x, int y, SCAN_METHOD_STRUCT *sm)
{
	TEXTMETRIC tm;
	char buf[80];
	int cxChar;

	GetTextMetrics(hdc, &tm);
	cxChar = tm.tmAveCharWidth+tm.tmExternalLeading;
	wsprintf(buf, "%3d", sm->station);
	TextOut(hdc, x, y, buf, strlen(buf));

	wsprintf(buf, "%s", sm->type);
	TextOut(hdc, x+cxChar*4, y, buf, strlen(buf));

	wsprintf(buf, "%3d", sm->address);
	TextOut(hdc, x+cxChar*8, y, buf, strlen(buf));

	wsprintf(buf, "%3d", sm->target);
	TextOut(hdc, x+cxChar*12, y, buf, strlen(buf));

	wsprintf(buf, "%3d", sm->size);		// size word of read
	TextOut(hdc, x+cxChar*16, y, buf, strlen(buf));
}

//------------------------------------------------------------------------------
// ABPLC 는 통신을 보내고 DLE와 ACK를 받고 다시 DLE와 ACK를 보내면 결과 코드가 들어온다.
//------------------------------------------------------------------------------

int WaitDLE_ACK_Read(LOCAL_PORT_STRUCT *pt, WORD time_out)
{
	BYTE buf[10];
	int  curr = 0;
	int  count;

	while(1) {
		if(pt->timeout->IsTimeOut(time_out))	return COMMUNICATION_TIME_OUT;

		count = PlcDeviceReadContinue(&pt->device, (char*)&buf[curr], 1);

		if(count == 0)	return COMMUNICATION_WAITING;

		if(buf[curr] == DLE) {	// DLE가 들어오면 처음 부터 시작한다.
			curr = 1;
			continue;
		}

		curr += count;

		if(curr >= 2) {
			PlcDeviceWrite(&pt->device, DLE);	// 원래 ACK와야 보내주나 이상코드가 들어올 때도 DLE+ACK를 보내면 PLC에 남아있던 명령어들을 모두 보내주게 되므로 다음에 통신할때는 깨끗하게 할 수 있다.
			PlcDeviceWrite(&pt->device, ACK);

			if(buf[0] != DLE && buf[1] != ACK) {	// DLE ACK가 들어올때 까지 계속한다.
				curr = 0;
				continue;
			}

			//PlcDeviceWrite(pt->device, DLE);
			//PlcDeviceWrite(pt->device, ACK);

			DisplaySendCodeNextLine(pt->no);

			return COMMUNICATION_OK;
		}
	}
}

//------------------------------------------------------------------------------
// ABPLC 는 통신을 보내고 DLE와 ACK를 받고 다시 DLE와 ACK를 보내면 결과 코드가 들어온다.
//------------------------------------------------------------------------------

int WaitDLE_ACK_Write(DEVICE_STRUCT *dev, WORD time_out)
{
	BYTE buf[10];
	int  curr = 0;
	int  count;
	TimeOutClass timeout;

	timeout.Reset();

	while(1) {
		count = PlcDeviceReadContinue(dev, (char*)&buf[curr], 1);

		curr += count;

		if(curr >= 2) {
			if(buf[0] != DLE) {
				return COMMUNICATION_CODE_BAD;
			}
			if(buf[1] != ACK) {
				return COMMUNICATION_CODE_BAD;
			}

			PlcDeviceWrite(dev, DLE);
			PlcDeviceWrite(dev, ACK);
			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(time_out))	return COMMUNICATION_TIME_OUT;
	}
}

//--------------------------------------------------------
//	pt->commSendBuf에 code를 추가하고 crc를 중가시키며 count도 증가시킨다.
//--------------------------------------------------------

void AddCodeToSendBuf(LOCAL_PORT_STRUCT *pt, BYTE code, BYTE *crc)
{
	pt->commSendBuf[pt->commCountSend] = code;
	(*crc) = (*crc) + code;
	pt->commCountSend ++;
	if((code) == 0x10) {
		pt->commSendBuf[pt->commCountSend] = 0x10;	// 코드중에 0x10이 나오면 한번 더 보내준다.
		pt->commCountSend++;
	}
}

//------------------------------------------------------------------------------
//	Scan Method 에 있는 방법대로 읽어서 버퍼에 저장한다.
//------------------------------------------------------------------------------

int PlcScanReadAbplc5(LOCAL_PORT_STRUCT *pt, int pos)
{
	SCAN_METHOD_STRUCT *sm = &pt->scanMethod[pos];
	int  i;
	int  	 count;
//	int  	 need;
	BYTE crc;
	static WORD tns = 0;
	int  file_number;
	int  retn;
	BYTE imsi[MAX_RECV_BUF];
	char dle_flag = OFF;

	file_number = atoi(&sm->type[1]);

	if(pt->bReadingFlag == OFF) {
	
		PlcDeviceClear(&pt->device);

		pt->commCountSend = 0;
		crc = 0;

		pt->commSendBuf[pt->commCountSend] = DLE;
		pt->commCountSend ++;

		pt->commSendBuf[pt->commCountSend] = STX;
		pt->commCountSend ++;

		AddCodeToSendBuf(pt, (BYTE)sm->station, &crc);		// destination ( address of the 1771-KA2 )
		AddCodeToSendBuf(pt, 0x00, &crc);					// source node address
		AddCodeToSendBuf(pt, 0x0F, &crc);					// command
		AddCodeToSendBuf(pt, 0x00, &crc);					// status
		tns++;
		AddCodeToSendBuf(pt, tns%256, &crc);				// TNS (transaction) check rand number
		AddCodeToSendBuf(pt, tns/256, &crc);				// TNS (transaction) check rand number
		AddCodeToSendBuf(pt, 0x01, &crc);					// function (read block)
		AddCodeToSendBuf(pt, (WORD)sm->address%256, &crc);	// packed offset
		AddCodeToSendBuf(pt, (WORD)sm->address/256, &crc);	// packed offset
	//	AddCodeToSendBuf(pt, sm->extra2%256, &crc);	// total trans
	//	AddCodeToSendBuf(pt, sm->extra2/256, &crc);	// total trans
		AddCodeToSendBuf(pt, ((WORD)sm->address+sm->size)%256, &crc);	// total trans
		AddCodeToSendBuf(pt, ((WORD)sm->address+sm->size)/256, &crc);	// total trans
		AddCodeToSendBuf(pt, 0x06, &crc);					// address
		AddCodeToSendBuf(pt, file_number%256, &crc);  // file number
		AddCodeToSendBuf(pt, file_number/256, &crc);  // file number
		AddCodeToSendBuf(pt, sm->size*2, &crc);			// read size

		pt->commSendBuf[pt->commCountSend] = DLE;
		pt->commCountSend ++;
		pt->commSendBuf[pt->commCountSend] = ETX;
		pt->commCountSend ++;

		crc = 0x0100-crc;

		pt->commSendBuf[pt->commCountSend] = crc;
		 
		pt->commCountSend++;

		pt->timeout->Reset();

		PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, pt->commCountSend);

		pt->bReadingFlag = ON;

		DisplaySendCodeNextLine(pt->no);

		return COMMUNICATION_WAITING;
	}

	retn = WaitDLE_ACK_Read(pt, pt->MAX_TIME_OUT_READ);
	
	if(retn != COMMUNICATION_OK)	return retn;
	
	// DLE+ACK 신호가 들어왔을 때만 다음으로 진행한다.

	pt->commCountNeed = 11+sm->size*2;
	pt->commCountCurr = 0;

	pt->timeout->Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
//		need  = pt->commCountNeed-pt->commCountCurr;
		//count = PlcDeviceReadContinue(pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], need);
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)imsi, 1);

		if(count > 0) {	// 읽은 바이트가 있다.
			// 아래 부분은 0x10이 두번나올 때 하나를 제거하기 위해서 존재한다.
			if(imsi[0] == DLE) {
				if(dle_flag) {
					dle_flag = OFF;
				}
				else {
					pt->commRecvBuf[pt->commCountCurr] = DLE;
					pt->commCountCurr ++;
					dle_flag = ON;
				}
			}
			else {
				pt->commRecvBuf[pt->commCountCurr] = imsi[0];
				pt->commCountCurr ++;
				dle_flag = OFF;
			}
		}

		// 필요한 바이트 수 만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[6] != (tns%256) ||
				pt->commRecvBuf[7] != (tns/256)) {
				// 모통 이런 경우는 동일한 데이터가 계속해서 밀려있는 경우이므로 DLE+ACK를 보내서 tns를 일치시키도록 한다.
				PlcDeviceWrite(&pt->device, DLE);	// 
				PlcDeviceWrite(&pt->device, ACK);

				return COMMUNICATION_CODE_BAD;
			}

			int address = sm->target;
			for(i = 0; i < sm->size; i++) {
				PokeValue(pt, sm, address+i, pt->commRecvBuf[i*2+8]+pt->commRecvBuf[i*2+9]*256);
			}
			return COMMUNICATION_OK;
		}

		if(pt->timeout->IsTimeOut(pt->MAX_TIME_OUT_READ))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
//	한 비트를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteBitAbplc5(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD flag, char *device)
{
	int  i;
	int  	 count;
//	int  	 need;
	BYTE crc;
	static WORD tns = 0;
	int  file_number;
	int  retn;
	BYTE imsi[MAX_RECV_BUF];
	TimeOutClass timeout;

	file_number = atoi(&device[1]);

	PlcDeviceClear(&pt->device);

	pt->commCountSend = 0;
	crc = 0;

	pt->commSendBuf[pt->commCountSend] = DLE;
	pt->commCountSend ++;

	pt->commSendBuf[pt->commCountSend] = STX;
	pt->commCountSend ++;

	AddCodeToSendBuf(pt, station, &crc);				// destination ( address of the 1771-KA2 )
	AddCodeToSendBuf(pt, 0x00, &crc);				// source node address
	AddCodeToSendBuf(pt, 0x0F, &crc);				// command
	AddCodeToSendBuf(pt, 0x00, &crc);				// status
	tns++;
	AddCodeToSendBuf(pt, tns%256, &crc);			// TNS (transaction) check rand number
	AddCodeToSendBuf(pt, tns/256, &crc);			// TNS (transaction) check rand number
	AddCodeToSendBuf(pt, 0x26, &crc);				// function (write block)

	// plc 5 system logical binary address
	if(address/16 > 255) {			// 주소가 255보다 클 때
		AddCodeToSendBuf(pt, 0x06, &crc);					// address
		AddCodeToSendBuf(pt, file_number, &crc);  // file number
		AddCodeToSendBuf(pt, 0xFF, &crc);
		AddCodeToSendBuf(pt, (address/16)%256, &crc);  // file number
		AddCodeToSendBuf(pt, (address/16)/256, &crc);
	}
	else {
		AddCodeToSendBuf(pt, 0x07, &crc);				// address
		AddCodeToSendBuf(pt, 0, &crc);  				// file number
		AddCodeToSendBuf(pt, file_number, &crc);  	// file number
		AddCodeToSendBuf(pt, address/16, &crc);  	// file number
		//AddCodeToSendBuf(pt, 0x06, &crc);					// address
		//AddCodeToSendBuf(pt, file_number, &crc);  // file number
		//AddCodeToSendBuf(pt, 0xFF, &crc);
		//AddCodeToSendBuf(pt, (address/16)%256, &crc);  // file number
		//AddCodeToSendBuf(pt, (address/16)/256, &crc);
	}

	WORD and_mask, or_mask;

	if(flag == OFF) {
		and_mask = 0xFFFF-WORD_MASK[address%16];
		or_mask  = 0;
	}
	else {
		and_mask = 0xFFFF;
		or_mask  = WORD_MASK[address%16];
	}

	AddCodeToSendBuf(pt, and_mask%256, &crc);
	AddCodeToSendBuf(pt, and_mask/256, &crc);
	AddCodeToSendBuf(pt, or_mask %256, &crc);
	AddCodeToSendBuf(pt, or_mask /256, &crc);

	pt->commSendBuf[pt->commCountSend] = DLE;
	pt->commCountSend ++;
	pt->commSendBuf[pt->commCountSend] = ETX;
	pt->commCountSend ++;

	crc = 0x0100-crc;

	pt->commSendBuf[pt->commCountSend] = crc;
	pt->commCountSend ++;

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, pt->commCountSend);

	retn = WaitDLE_ACK_Write(&pt->device, pt->MAX_TIME_OUT_WRITE);
	if(retn != COMMUNICATION_OK)
		return retn;

	pt->commCountNeed = 9;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
//		need  = pt->commCountNeed-pt->commCountCurr;
		//count = PlcDeviceReadContinue(pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], need);
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&imsi, 1);

		if(count > 0) {	// 읽은 바이트가 있다.
			// 아래 부분은 0x10이 두번나올 때 하나를 제거하기 위해서 존재한다.
			for(i = 0; i < count; i++) {
				if(imsi[i] == DLE && pt->commCountCurr > 0 &&
					imsi[i-1] == DLE) {

				}
				else {
					pt->commRecvBuf[pt->commCountCurr] = imsi[i];
					pt->commCountCurr ++;
				}
			}
		}

		// 필요한 바이트 수 만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[6] != (tns%256) ||
				pt->commRecvBuf[7] != (tns/256)) {
				PlcDeviceWrite(&pt->device, DLE);
				PlcDeviceWrite(&pt->device, ACK);
				return COMMUNICATION_CODE_BAD;
			}
			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}

//------------------------------------------------------------------------------
//	한 워드를 통신을 통해 쓴다.
//------------------------------------------------------------------------------

int PlcScanWriteWordAbplc5(LOCAL_PORT_STRUCT *pt, int station, WORD address, WORD value, char *device, WORD /*file_size*/)
{
	int  i;
	int  	 count;
//	int  	 need;
	TimeOutClass timeout;
	BYTE crc;
	static WORD tns = 0;
	int  file_number;
	int  retn;
	BYTE imsi[MAX_RECV_BUF];

	file_number = atoi(&device[1]);

	PlcDeviceClear(&pt->device);

	pt->commCountSend = 0;
	crc = 0;

	pt->commSendBuf[pt->commCountSend] = DLE;
	pt->commCountSend ++;

	pt->commSendBuf[pt->commCountSend] = STX;
	pt->commCountSend ++;

	AddCodeToSendBuf(pt, station, &crc);				// destination ( address of the 1771-KA2 )
	AddCodeToSendBuf(pt, 0x00, &crc);				// source node address
	AddCodeToSendBuf(pt, 0x0F, &crc);				// command
	AddCodeToSendBuf(pt, 0x00, &crc);				// status
	tns++;
	AddCodeToSendBuf(pt, tns%256, &crc);			// TNS (transaction) check rand number
	AddCodeToSendBuf(pt, tns/256, &crc);			// TNS (transaction) check rand number
	AddCodeToSendBuf(pt, 0x00, &crc);				// function (write block)
	AddCodeToSendBuf(pt, address%256, &crc);	// packed offset
	AddCodeToSendBuf(pt, address/256, &crc);	// packed offset
//	AddCodeToSendBuf(pt, file_size%256, &crc);	// total trans
//	AddCodeToSendBuf(pt, file_size/256, &crc);	// total trans
	AddCodeToSendBuf(pt, (address+1)%256, &crc);	// total trans
	AddCodeToSendBuf(pt, (address+1)/256, &crc);	// total trans
	AddCodeToSendBuf(pt, 0x06, &crc);					// address
	AddCodeToSendBuf(pt, file_number%256, &crc);  // file number
	AddCodeToSendBuf(pt, file_number/256, &crc);  // file number
	AddCodeToSendBuf(pt, value%256, &crc);
	AddCodeToSendBuf(pt, value/256, &crc);

	pt->commSendBuf[pt->commCountSend] = DLE;
	pt->commCountSend ++;
	pt->commSendBuf[pt->commCountSend] = ETX;
	pt->commCountSend ++;

	crc = 0x0100-crc;

	pt->commSendBuf[pt->commCountSend] = crc;
	pt->commCountSend ++;

	PlcDeviceWriteContinue(&pt->device, (char*)pt->commSendBuf, pt->commCountSend);

	retn = WaitDLE_ACK_Write(&pt->device, pt->MAX_TIME_OUT_WRITE);
	if(retn != COMMUNICATION_OK)
		return retn;

	pt->commCountNeed = 9;
	pt->commCountCurr = 0;

	timeout.Reset();

	if(pt->commCountNeed > MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;

	while(1) {
//		need  = pt->commCountNeed-pt->commCountCurr;
		//count = PlcDeviceReadContinue(pt->device, (char*)&pt->commRecvBuf[pt->commCountCurr], need);
		if(pt->commCountCurr >= MAX_RECV_BUF)	return COMMUNICATION_ERR_SIZE_TOO_BIG;
		count = PlcDeviceReadContinue(&pt->device, (char*)&imsi, 1);

		if(count > 0) {	// 읽은 바이트가 있다.
			// 아래 부분은 0x10이 두번나올 때 하나를 제거하기 위해서 존재한다.
			for(i = 0; i < count; i++) {
				if(imsi[i] == DLE && pt->commCountCurr > 0 &&
					imsi[i-1] == DLE) {

				}
				else {
					pt->commRecvBuf[pt->commCountCurr] = imsi[i];
					pt->commCountCurr ++;
				}
			}
		}

		// 필요한 바이트 수 만큼 데이터를 모두 받았다.
		if(pt->commCountCurr >= pt->commCountNeed) {
			if(pt->commRecvBuf[6] != (tns%256) ||
				pt->commRecvBuf[7] != (tns/256)) {
				PlcDeviceWrite(&pt->device, DLE);
				PlcDeviceWrite(&pt->device, ACK);
				return COMMUNICATION_CODE_BAD;
			}
			return COMMUNICATION_OK;
		}

		if(timeout.IsTimeOut(pt->MAX_TIME_OUT_WRITE))	return COMMUNICATION_TIME_OUT;
	}
}





