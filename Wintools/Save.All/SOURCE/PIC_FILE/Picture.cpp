#include "stdafx.h"
//#include <iostream.h>
#include <picture.h>

pictureFileClass :: pictureFileClass()
{
	in = NULL;
	out = NULL;
	dac = new BYTE[768];
	image_order = IMAGE_ORDER_DOWN;
	nWidth = 0;
	nHeight = 0;
	nBitsPerPixel = 8;

//	if(dac != NULL) 	memcpy(dac, DEFAULT_RGB, 768);

	if(dac == NULL) {
		MessageBox(NULL, _T("기본 메모리가 부족합니다.\n널려있는 작업들을 종료한 후\n사용하세요."), _T("기본 메모리 부족"), MB_OK);
	}
}

pictureFileClass :: ~pictureFileClass()
{
	if(dac != NULL)	delete dac;
	if(out != NULL)	fclose(out);
	if(in != NULL)  	fclose(in);
}

void pictureFileClass :: ReadOpenError(HWND hwnd, const TCHAR *filename)
{
	TCHAR message[256];
	_stprintf(message, _T("%s 파일을\n읽을수가 없습니다."), filename);
	MessageBox(hwnd, message, _T("파일 읽기(열기) 이상"), MB_OK);
}

void pictureFileClass :: WriteOpenError(HWND hwnd, const TCHAR *filename)
{
	TCHAR message[256];
	_stprintf(message, _T("%s 파일을\n쓸수가 없습니다."), filename);
	MessageBox(hwnd, message, _T("파일 쓰기(열기) 이상"), MB_OK);
}

void pictureFileClass :: ReadSizeError(HWND hwnd, const TCHAR *filename)
{
	TCHAR message[256];
	_stprintf(message, _T("%s 파일을\n읽을 수가 없습니다.\n파일이 깨져 있거나 형식이 다릅니다."), filename);
	MessageBox(hwnd, message, _T("파일 크기 틀림"), MB_OK);
}

void pictureFileClass :: GetDac(BYTE *buf)
{
   if(dac != NULL)	memcpy(buf, dac, 768);
}

void pictureFileClass :: GetSize(int& sizex, int& sizey)
{
   sizex = nWidth;
   sizey = nHeight;
}

int  pictureFileClass :: GetColor()
{
   return nBitsPerPixel;
}

char pictureFileClass :: ImageOrder()
{
   return image_order;
}

void pictureFileClass :: GetInformation(int &sizex, int &sizey, int &bitsperpixel)
{
	sizex = nWidth;
   sizey = nHeight;
   bitsperpixel = nBitsPerPixel;
}

void pictureFileClass :: SetDac(BYTE *buf)
{
   if(dac != NULL)	memcpy(dac, buf, 768);
}

void pictureFileClass :: SetSize(int sizex, int sizey)
{
   nWidth = sizex;
   nHeight = sizey;
}

void pictureFileClass :: SetColor(int color)
{
   nBitsPerPixel = color;
}

void pictureFileClass :: ReadClose(void)
{
   if(in != NULL) {
      fclose(in);
      in = NULL;
   }
}

void pictureFileClass :: WriteClose(void)
{
   WriteClosePrepare();	// 파일을 닫기전에 마지막 할일을 정리한다.
	
	if(out != NULL) {
      fclose(out);
      out = NULL;
   }
}

//--------------------------------------------------
// 가상 함수.
//	파일을 닫기 전에 해야할일이 있으면 여기에서 한다.
// 예: 마지막에 RGB를 쓴다든지.
//--------------------------------------------------  

void pictureFileClass :: WriteClosePrepare()
{
	// 파일을 닫기전에 할일이 없다.	
}



