#include "stdafx.h"
#include <io.h>
#include <fcntl.h>
#include <stdio.h>
#include <string.h>

#include <compiler.hpp>

#include <tools.h>
#include <glib.h>
#include <zip.h>
#include <mmptool.h>

#include <f_format.h>

#include "slide.h"

//int  TestExtension(char *filename, char *ext);

class getFileFormat {
#define	MAX_READ_BUF	100
		char filename[MAXPATH];
		BYTE *head;
		char file_ext[MAXEXT];

		WORD	GetWord(int pos);
		DWORD	GetDword(int pos);
	public:
		FILE *in;
		getFileFormat(char *name);
		~getFileFormat();
		int  	PcxFormat();
		int  	GifFormat();
		int  	BmpFormat();
		int  	LbmFormat();
		int  	JpgFormat();
		int  	PicFormat();
		int  	TifFormat();
		int	HgeFormat();
		int	SptFormat();
		int	MmpFormat();
		int	MacFormat();

		int	WmfFormat();

		int  	FliFormat();

		int	SeeSlideFormat();

		int  	IcoFormat();


		int  	BnkFormat();
		int  	VocFormat();
		int  	WavFormat();

		int  	ZipFormat();
		int  	LzhFormat();
		int   ArcFormat();

		int  	HwpFormat(int& format);
		int	GwpFormat();
		int	HanaFormat();

		int  	ExeFormat();

		int   TextOrHex();
		int	Extension(char *ext);
};

//----------------------------------------------------------------------------
//	단순히 확장자만을 가지고 파일의 형식을 검사한다.
//----------------------------------------------------------------------------

int TestExtension(char *filename, char *ext)
{
   char *ptr;

   strupr(ext);

   ptr = strchr(filename, '.');
   if (!ptr) 	return (FALSE);

   if(strcmp(ptr+1, ext) == 0)	return (TRUE);

   return(FALSE);
}

getFileFormat :: getFileFormat(char *name)
{
   in = NULL;
   head = NULL;
	
   strcpy(filename, name);
   strupr(filename);

   in = fopen(filename, "rb");
	if(in != NULL) {
		head = new BYTE[MAX_READ_BUF];
		if(head != NULL) {
			fread(head, 1, MAX_READ_BUF, in);
		}
	}

	FnSplit fnsplit;

	fnsplit.fnsplit(filename);
	
    fnsplit.GetExt(file_ext);
}

getFileFormat :: ~getFileFormat()
{
   if(in != NULL)	fclose(in);
   if(head != NULL)	delete head;
}

WORD getFileFormat :: GetWord(int pos)
{
   return ( (WORD) ( head[pos]+head[pos+1]*256u ) );
}

DWORD getFileFormat :: GetDword(int pos)
{
   return ( ((DWORD)head[pos]) +
	    ((DWORD)head[pos+1] << 8) +
	    ((DWORD)head[pos+2] << 16) +
	    ((DWORD)head[pos+3] << 24) );
}

//----------------------------------------------------------------------------
//	GIF 파일인가 검사
//----------------------------------------------------------------------------

int getFileFormat :: GifFormat()
{
   if(head[0] == 'G' &&
      head[1] == 'I' &&
      head[2] == 'F')		return (TRUE);

   return (FALSE);
}

//----------------------------------------------------------------------------
//	BMP 파일인가 검사
//----------------------------------------------------------------------------

int getFileFormat :: BmpFormat()
{
   WORD id = GetWord(0);                    // bitmap file header

//   lseek(handle, 0, SEEK_SET);
//   read(handle, &id, sizeof (WORD));	// bitmap file head 14 byte

   if(id != 0x4D42)	{	// allways "BM"
      return (FALSE);
   }
   return (TRUE);
}

//----------------------------------------------------------------------------
//       *.LBM 파일인가 검사
//----------------------------------------------------------------------------

int getFileFormat :: LbmFormat()
{
//   LBMHEADER head;

//   lseek(handle, 0, SEEK_SET);
//   read(handle, &head, sizeof (LBMHEADER));

   if(memcmp(head, "FORM", 4) != 0)		return (FALSE);
   if(memcmp(&head[12], "BMHD", 4) != 0)		return (FALSE);

   return (TRUE);
}

//----------------------------------------------------------------------------
//       *.JPG 파일인가 검사
//----------------------------------------------------------------------------

int getFileFormat :: JpgFormat()
{
   if(!Extension("JPG"))	return FALSE;

   if(head[0] != 0xFF || head[1] != 0xD8)	return (FALSE);

   //if(memcmp(&head[6], "JFIF", 4) != 0)		return (FALSE);

   return (TRUE);
}

//----------------------------------------------------------------------------
//       *.PIC 파일인가 검사
//----------------------------------------------------------------------------

int getFileFormat :: PicFormat()
{
//   PICHEADER head;
   WORD id = GetWord(0);

//   lseek(handle, 0, SEEK_SET);
//   read(handle, &head, sizeof (PICHEADER));

   if(id != 0x4841)     return (FALSE);  	// "AH"

   return (TRUE);
}

//----------------------------------------------------------------------------
//       *.TIF 파일인가 검사
//----------------------------------------------------------------------------

int getFileFormat :: TifFormat()
{
   WORD id = GetWord(0);

//   lseek(handle, 0, SEEK_SET);
//   read(handle, &id, 2);

   if(id == 0x4949 || id == 0x4d4d)     return (TRUE); 	 // "II", "MM"

   return (FALSE);
}

//----------------------------------------------------------------------------
//	*.HGE
//----------------------------------------------------------------------------

int getFileFormat :: HgeFormat()
{
//   char id[30];
//   lseek(handle, 0, SEEK_SET);
//   read(handle, &id, 20);

   if(memcmp(head, "HGE image file", 14) == 0)	return (TRUE);
   return (FALSE);
}

//----------------------------------------------------------------------------
//	*.SPT
//----------------------------------------------------------------------------

int getFileFormat :: SptFormat()
{
//   char id[30];
//   lseek(handle, 0, SEEK_SET);
//   read(handle, &id, 3);

   if(memcmp(head, "SPT", 3) == 0)	return (TRUE);
   return (FALSE);
}

//----------------------------------------------------------------------------
//	*.MMP
//----------------------------------------------------------------------------

int getFileFormat :: MmpFormat()
{
//   char id[6];
//   lseek(handle, 0, SEEK_SET);
//   read(handle, &id, 6);

   if(memcmp(head, MMP_HDR_STRING_12, 6) == 0)	return (TRUE); // "YUV12C"
   if(memcmp(head, MMP_HDR_STRING_16, 6) == 0)	return (TRUE); // "YUV16C"

   return (FALSE);
}

//----------------------------------------------------------------------------
//	*.MAC
//----------------------------------------------------------------------------

int getFileFormat :: MacFormat()
{
//   char id[8];
//   lseek(handle, 0x41, SEEK_SET);
//   read(handle, &id, 8);

   if(memcmp(head, "PNTGMPNT", 8) == 0)	return (TRUE);
   return (FALSE);
}


//----------------------------------------------------------------------------
//	*.WMF
//----------------------------------------------------------------------------

int getFileFormat :: WmfFormat()
{
   WORD *p = (WORD*) head;
   WORD checksum = 0;
   int i;

   if(head[0] == 1 &&
      head[1] == 0 &&
      head[2] == 0x09 &&
      head[3] == 0x00 ) {
      return TRUE;
   }

   if(head[0] != 0xd7 ||		// check id 0x9AC6CDD7
      head[1] != 0xcd ||
      head[2] != 0xc6 ||
      head[3] != 0x9a ) {
      return FALSE;
   }
   for(i = 0; i < 10; i++) 	checksum ^= p[i];
   if(checksum == p[10])	return TRUE;

   return FALSE;
}

//----------------------------------------------------------------------------
//       *.FLI 파일인가 검사
//----------------------------------------------------------------------------

int getFileFormat :: FliFormat()
{
   WORD id = GetWord(4);
//   lseek(handle, 4, SEEK_SET);
//   read(handle, &id, 2);

   if(id == 0xAF11)     return (TRUE);

   return (FALSE);
}

//----------------------------------------------------------------------------
//       *.ICO 파일인가 검사
//----------------------------------------------------------------------------

int getFileFormat :: IcoFormat()
{
   WORD type = GetWord(2);

//   lseek(handle, 2, SEEK_SET);
//   read(handle, &type,  2);

   if(type != 1)		return (FALSE);
   if(Extension("ICO"))		return(TRUE);

   return (FALSE);
}



//----------------------------------------------------------------------------
//	Bank 파일인가 검사
//----------------------------------------------------------------------------

int getFileFormat :: BnkFormat()
{
//   char id[7];

//   lseek(handle, 2, SEEK_SET);
//   read(handle, &id, 6);	// bank file header

   if(memcmp(&head[2], "ADLIB-", 6) == 0)
      return (TRUE);
   return (FALSE);
}

//----------------------------------------------------------------------------
//	VOICE 파일인가 검사
//----------------------------------------------------------------------------

int getFileFormat :: VocFormat()
{
//   char id[20];

//   lseek(handle, 0, SEEK_SET);
//   read(handle, &id, 19);	// voice file header

   if(memcmp(head, "Creative Voice File", 19) == 0)
      return (TRUE);
   return (FALSE);
}

//----------------------------------------------------------------------------
//	WAVE 파일인가를 검사한다.
//----------------------------------------------------------------------------

int getFileFormat :: WavFormat()
{
//   char id[5];

//   lseek(handle, 8, SEEK_SET);
//   read(handle, &id, 4);	// wave file header

   if(memcmp(&head[8], "WAVE", 4) == 0)
      return (TRUE);
   return (FALSE);
}

//----------------------------------------------------------------------------
//	ZIP 파일인가를 검사
//----------------------------------------------------------------------------

int getFileFormat :: ZipFormat()
{
   DWORD code = GetDword(0);

//   lseek(handle, 0, SEEK_SET);
//   read(handle, &code, sizeof(long));

   if(code == LOCAL_FILE_HEADER_SIGNATURE)	return (TRUE);

   return (FALSE);
}

//----------------------------------------------------------------------------
//	ARC 파일인가를 검사
//----------------------------------------------------------------------------

int getFileFormat :: ArcFormat()
{
//   BYTE sign;

//   lseek(handle, 0, SEEK_SET);
//   read(handle, &sign, 1);

   if(head[0] != 0x1A )	return (FALSE);
   if(!Extension("ARC"))	return (FALSE);
   return (TRUE);
}

//----------------------------------------------------------------------------
//	HWP 파일인가를 검사
//----------------------------------------------------------------------------

int getFileFormat :: HwpFormat(int& format)
{
//   char id[24];

//   lseek(handle, 0, SEEK_SET);
//   read(handle, &id, 23);

   if(memcmp(head, "HWP Document File V1.20", 23) == 0) {
      format = HWP_FORMAT;
   }
   else if(memcmp(head, "HWP Document File V2.00", 23) == 0) {
      format = HWP20_FORMAT;
   }
   else if(memcmp(head, "HWP Document File V2.10", 23) == 0) {
      format = HWP21_FORMAT;
   }
   else	return (FALSE);

   return (TRUE);
}

int getFileFormat :: HanaFormat()
{
//   BYTE imsi[2];

   if(!Extension("HWP"))	return (FALSE);
//   lseek(handle, 0, SEEK_SET);
//   read(handle, &imsi, 2);
   if(head[0] != 0x1A)	return FALSE;	// offset 0
   if(head[1] != 0x1A)	return FALSE;   // offset 1
   return TRUE;
}

//----------------------------------------------------------------------------
//	GWP 파일인가를 검사
//----------------------------------------------------------------------------

int getFileFormat :: GwpFormat()
{
//   char id[24];

//   lseek(handle, 0, SEEK_SET);
//   read(handle, &id, 23);

   if(memcmp(head, "Word Processor 21", 17) == 0)	return (TRUE);

   return (FALSE);
}

//----------------------------------------------------------------------------
//      실행파일인가 검사
//----------------------------------------------------------------------------

int getFileFormat :: ExeFormat()
{
   WORD id = GetWord(0);

//   lseek(handle, 0, SEEK_SET);
//   read(handle, &id, 2);

   if(!Extension("EXE"))	return (FALSE);
   if(id != 0x5A4D)	return (FALSE);

   return (TRUE);
}

int getFileFormat :: SeeSlideFormat()
{
//   char gbuf[60];

//   lseek(handle, 0, SEEK_SET);
//   read(handle, &gbuf[0], strlen(SEE_SLIDE_STRING_ID));

   if(memcmp(head, SEE_SLIDE_STRING_ID, strlen(SEE_SLIDE_STRING_ID)) == 0)
      return TRUE;
   return FALSE;
}

//----------------------------------------------------------------------------
//	모든 포맷을 검사한후에 마지막으로 텍스트인지 HEX 인지를 검사한다.
//----------------------------------------------------------------------------

int getFileFormat :: TextOrHex()
{
   int ch;
   int count = 0;
   fseek(in, 0, SEEK_SET);

   while (1) {
      if((ch = fgetc(in)) == EOF)	break;
      count++;
      if(count > 1000)	break;
      if(ch == 0x0D)	continue;   	// carriage return
      if(ch == 0x0A)	continue;       // line feed
      if(ch == 0x09)	continue;       // horizontal tab
      if(ch >= 32)	continue;
      if(ch == 0x1A)	continue;	// CTRL_Z

      return NONE_FORMAT;
   }

   return TXT_FORMAT;
}

int getFileFormat :: Extension(char *ext)
{
   //char *ptr;

   strupr(ext);

   //ptr = strchr(filename, '.');
   //if (!ptr) 	return (FALSE);

   //if(strcmp(ptr+1, ext) == 0)	return (TRUE);

   //return(FALSE);
   if(file_ext[0] != '.')	return FALSE;
   if(strcmp(&file_ext[1], ext) == 0)	return TRUE;

   return FALSE;
}

//----------------------------------------------------------------------------
//	파일의 형식을 알려준다.
//----------------------------------------------------------------------------

int  GetFileFormat(char *FileName)
{
   getFileFormat test(FileName);

	if(test.in == NULL)	return NONE_FORMAT;
	
   int  format;

   if(test.Extension("ROL"))			format = ROL_FORMAT;
   else if(test.Extension("IMS"))	format = IMS_FORMAT;
   else if(test.Extension("SND"))	format = SND_FORMAT;

   else if(test.Extension("CPP"))	format = CPP_FORMAT;
   else if(test.Extension("C"))		format = C_FORMAT;
   else if(test.Extension("BAS"))	format = BAS_FORMAT;
   else if(test.Extension("PAS"))	format = PAS_FORMAT;
   else if(test.Extension("ASM"))	format = ASM_FORMAT;
   else if(test.Extension("INC"))	format = INC_FORMAT;
   else if(test.Extension("COB"))	format = COB_FORMAT;
   else if(test.Extension("FOR"))	format = FOR_FORMAT;
   else if(test.Extension("H"))		format = H_FORMAT;
   else if(test.Extension("PCX"))	format = PCX_FORMAT;
   else if(test.Extension("TGA"))	format = TGA_FORMAT;
   else if(test.Extension("LZH"))	format = LZH_FORMAT;
   else if(test.Extension("ARJ"))	format = ARJ_FORMAT;

   else if(test.Extension("DOC"))	format = DOC_FORMAT;
   else if(test.Extension("TXT"))	format = TXT_FORMAT;

   else if(test.Extension("DBF"))	format = DBF_FORMAT;

   else if(test.Extension("BAT"))	format = BAT_FORMAT;
   else if(test.Extension("COM"))	format = COM_FORMAT;

   else if(test.ZipFormat()) 		format = ZIP_FORMAT;
   else if(test.ArcFormat())		format = ARC_FORMAT;

   else if(test.Extension("PNG")) 	format = PNG_FORMAT;
   else if(test.GifFormat()) 		format = GIF_FORMAT;
   else if(test.BmpFormat()) 		format = BMP_FORMAT;
   else if(test.LbmFormat()) 		format = LBM_FORMAT;
   else if(test.JpgFormat())		format = JPG_FORMAT;
   else if(test.PicFormat())		format = PIC_FORMAT;
   else if(test.TifFormat())		format = TIF_FORMAT;
   else if(test.HgeFormat())		format = HGE_FORMAT;
   else if(test.SptFormat())		format = SPT_FORMAT;
   else if(test.MmpFormat())		format = MMP_FORMAT;
   else if(test.MacFormat())		format = MAC_FORMAT;

   else if(test.WmfFormat())		format = WMF_FORMAT;

   else if(test.FliFormat())		format = FLI_FORMAT;

   else if(test.SeeSlideFormat())	format = SEE_SLIDE_FORMAT;

   else if(test.IcoFormat())		format = ICO_FORMAT;

   else if(test.BnkFormat())		format = BNK_FORMAT;
   else if(test.HwpFormat(format)) ;
   else if(test.HanaFormat())		format = HANA_FORMAT;
   else if(test.GwpFormat())		format = GWP_FORMAT;
   else if(test.ExeFormat()) 		format = EXE_FORMAT;
   else if(test.VocFormat()) 		format = VOC_FORMAT;
   else if(test.WavFormat()) 		format = WAV_FORMAT;

   else if(test.Extension("VOC"))	format = VOC_NO_FORMAT;

   else 				format = test.TextOrHex();

   return (format);
}