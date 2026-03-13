using System;
using System.IO;
using System.Windows.Forms;

namespace PicTools
{
	class SPTHEADER
	{
		public byte[] 	id = new byte[3];		// always "SPT"
		public byte 	version;				// 1
		public ushort	width;					// 그림 가로
		public ushort	height;					// 그림 세로
		public ushort	startx;					// 시작위치
		public ushort	starty;					//
		public byte		bitsperpixel;			// 팩셀당 차지하는 비트수
		public byte		nplanes;				// 플랜수
		public int		backcolor;				// 배경칼라
		public byte		interlace;				// 인터레이스 방식
		public ushort	hres;					// 화면 수평 해상도
		public ushort	vres;					// 화면 수직 해상도
		public byte		compression;			// 압축
		public byte[]	extra = new byte[104];  // 여분
	}

	/// <summary>
	/// Summary description for SptTools.
	/// </summary>
	public class SptTools : PictureFileClass
	{
		public SptTools()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		SPTHEADER head = new SPTHEADER();

		

		int nBitsPerPixelOriginal;
		
		byte getrvalue(ushort val) { return (byte)((val >> 10) & 0x1F); }
		byte getgvalue(ushort val) { return (byte)((val >> 5) & 0x1F); }
		byte getbvalue(ushort val) { return (byte)((val >> 0) & 0x1F); }

		protected override bool ReadOpenLocal()
		{
			/*
			public sbyte[] 	id = new sbyte[3];		// always "SPT"
		public byte 	version;				// 1
		public ushort	width;					// 그림 가로
		public ushort	height;					// 그림 세로
		public ushort	startx;					// 시작위치
		public ushort	starty;					//
		public byte		bitsperpixel;			// 팩셀당 차지하는 비트수
		public byte		nplanes;				// 플랜수
		public int		backcolor;				// 배경칼라
		public byte		interlace;				// 인터레이스 방식
		public ushort	hres;					// 화면 수평 해상도
		public ushort	vres;					// 화면 수직 해상도
		public byte		compression;			// 압축
		public byte[]	extra = new byte[104];  // 여분
			*/

			head.id = br.ReadBytes(3);
			head.version = br.ReadByte();
			head.width = br.ReadUInt16();
			head.height = br.ReadUInt16();
			head.startx = br.ReadUInt16();
			head.starty = br.ReadUInt16();
			head.bitsperpixel = br.ReadByte();
			head.nplanes = br.ReadByte();
			head.backcolor = br.ReadInt32();
			head.interlace = br.ReadByte();
			head.hres = br.ReadUInt16();
			head.vres = br.ReadUInt16();
			head.compression = br.ReadByte();
			head.extra = br.ReadBytes(104);

			if(head.id[0] == (byte)'S' && head.id[1] == (byte)'P' && head.id[2] == (byte)'T') 
			{
				if(head.version != 1) 
				{
					MessageBox.Show("상위 버전의 그림파일입니다.(버전 1.0 이하만 지원)",  "버전 높음");
					return false;
				}
			}
			else {
				MessageBox.Show("SPT 그림파일 형식이 아닙니다.",  "파일 형식 틀림");	
				return false;
			}

			nWidth = head.width;
			nHeight = head.height;
			nBitsPerPixel = head.bitsperpixel;
			nBitsPerPixelOriginal = nBitsPerPixel;
			if(nBitsPerPixel == 15 || nBitsPerPixel == 16) 
			{
				nBitsPerPixel = 24;
			}

			switch(head.bitsperpixel) 
			{
				case 1:		nBytesPerLine = (nWidth+7)/8;	break;
				case 4:		nBytesPerLine = (nWidth)/8*4;	break;
				case 8:		nBytesPerLine = (nWidth);		break;
				case 15:
				case 16:	nBytesPerLine = (nWidth)*2;	break;
				case 24:	nBytesPerLine = (nWidth)*3;	break;
			}

			switch(nBitsPerPixelOriginal) 
			{
				case 1:
					dac[0] = 0;
					dac[1] = 0;
					dac[2] = 0;
					dac[3] = 255;
					dac[4] = 255;
					dac[5] = 255;
					break;
				case 4:
					dac = br.ReadBytes(48);
					break;
				case 8:
					dac = br.ReadBytes(768);
					break;
			}
			return true;
		}

		public bool GetOneLine(ref byte[] buf, int maxx)
		{
			int i;
			byte[] imsi;

			switch ( nBitsPerPixelOriginal ) 
			{
				case 1:
				case 8:
				case 24:  
					imsi = br.ReadBytes(nBytesPerLine);
					imsi.CopyTo(buf, 0);
					break;
				case 15:
				case 16:
					ushort word_buf;
					int r, g, b;
					for(i = 0; i < nWidth; i++) 
					{
						word_buf = br.ReadUInt16();
						r = (getrvalue(word_buf) << 3);
						g = (getgvalue(word_buf) << 3);
						b = (getbvalue(word_buf) << 3);

						buf[i*3+0] = (byte)b;
						buf[i*3+1] = (byte)g;
						buf[i*3+2] = (byte)r;
					}
					break;
				case 4:	
					// packed buffer memory
					int byteperline = (nWidth+7)/8;
					byte data;
					int xx, plane;

					for(i = 0; i < buf.Length; i++)	buf[i] = 0;

					for(plane = 0; plane < 4; plane++) 
					{
						for(xx = 0; xx < byteperline; xx++) 
						{
							data = br.ReadByte();
							for(i = 0; i < 8; i+=2) 
							{		// packed memory buffer
								buf[xx*4+i/2] |= (data & PictureFileClass.BIT_MASK[i]) > 0   ? PictureFileClass.BIT_MASK[plane]   : (byte)0;
								buf[xx*4+i/2] |= (data & PictureFileClass.BIT_MASK[i+1]) > 0 ? PictureFileClass.BIT_MASK[plane+4] : (byte)0;
							}
						}
					}
					break;
			}
			return true;
		}
	}
}

/*

int sptClass :: ReadOpen(char *filename)
{
   SPTHEADER head;

   if(dac == NULL)	return 0;

   if(filename != NULL) {	// if NULL already open
      in = fopen(filename, "rb");
      if(in == NULL)	return 0;
   }

   struct oldHead {
		char 	dolhead[20];
		WORD 	cutx1,
				cuty1,
				cutx2,
				cuty2;
		WORD  length;
		char etc[20];
   } oldHead;
   //fpos_t pos;

   fread(&head, 1, sizeof (SPTHEADER), in);
   if(head.id[0] == 'S' && head.id[1] == 'P' && head.id[2] == 'T') {
		if(head.version != 1) {
			MessageBox(NULL, "상위 버전의 그림파일입니다.(버전 1.0 이하만 지원)",  "버전 높음", MB_OK);
			return 0;
		}
   }
   else {	// old version
      fseek(in, -51, SEEK_END);
      fread(&oldHead, 1, sizeof(oldHead), in);
      head.version = 0;            		// old version
      head.width = oldHead.cutx2-oldHead.cutx1+1;
      head.height = oldHead.cuty2-oldHead.cuty1+1;
      head.startx = 0;
      head.starty = 0;
      head.bitsperpixel = COLOR_256;
      head.nplanes = 1;
      head.interlace = 0;
      head.hres = 640;
      head.vres = 480;
      head.compression = 0;

      fseek(in, 4+sizeof(oldHead), SEEK_SET);
   }

   nWidth = head.width;
   nHeight = head.height;
   nBitsPerPixel = head.bitsperpixel;
	nBitsPerPixelOriginal = nBitsPerPixel;
	if(nBitsPerPixel == COLOR_32768 || nBitsPerPixel == COLOR_65536) {
		nBitsPerPixel = COLOR_1600;
	}

   switch(head.bitsperpixel) {
      case COLOR_2:		nBytesPerLine = (nWidth+7)/8;	break;
      case COLOR_16:    nBytesPerLine = (nWidth)/8*4;	break;
      case COLOR_256:	nBytesPerLine = (nWidth);		break;
      case COLOR_32768:
      case COLOR_65536:	nBytesPerLine = (nWidth)*2;	break;
      case COLOR_1600:	nBytesPerLine = (nWidth)*3;	break;
   }

   if(head.version == 0) {
      memcpy(dac, DEFAULT_RGB, 768);
   }
   else {
      switch(nBitsPerPixelOriginal) {
			case COLOR_2:
				dac[0] = 0;
				dac[1] = 0;
				dac[2] = 0;
				dac[3] = 255;
				dac[4] = 255;
				dac[5] = 255;
				break;
			case COLOR_16:
				fread(dac, 48, 1, in);
				break;
			case COLOR_256:
				fread(dac, 768, 1, in);
				break;
      }
   }
   return 1;
}

int sptClass :: GetOneLine(BYTE *buf, int maxx)
{
	int i;

	switch ( nBitsPerPixelOriginal ) {
      case COLOR_2:
      case COLOR_256:
      case COLOR_1600:  fread(buf, 1, nBytesPerLine, in);
			break;
		case COLOR_32768:
      case COLOR_65536:
			WORD word_buf;
			BYTE r, g, b;
			for(i = 0; i < nWidth; i++) {
				fread(&word_buf, 1, 2, in);
				r = (getrvalue(word_buf) << 3);
				g = (getgvalue(word_buf) << 3);
				b = (getbvalue(word_buf) << 3);

				buf[i*3+0] = b;
				buf[i*3+1] = g;
				buf[i*3+2] = r;
			}
			break;
      case COLOR_16:	
			// packed buffer memory
			int byteperline = (nWidth+7)/8;
			char data;
			int xx, plane;

			memset(buf, 0, (maxx+1)/2);

			for(plane = 0; plane < 4; plane++) {
				for(xx = 0; xx < byteperline; xx++) {
					data = fgetc(in);
					for(i = 0; i < 8; i+=2) {		// packed memory buffer
						buf[xx*4+i/2] |= data & BIT_MASK[i]   ? BIT_MASK[plane]   : 0;
						buf[xx*4+i/2] |= data & BIT_MASK[i+1] ? BIT_MASK[plane+4] : 0;
					}
				}
			}
			break;
   }
   return 1;
}


//---------------------------------------------------------------------------
//	SPT 화일에서 RGB 정보를 쓴다.
//---------------------------------------------------------------------------

void sptClass :: PutRGB(BYTE *dac)
{
   switch(nBitsPerPixel) {
      case COLOR_2:
      case COLOR_32768:
      case COLOR_65536:
      case COLOR_1600: 	break;
      case COLOR_16:    fwrite(dac, 1, 48, out);		break;
      case COLOR_256:	fwrite(dac, 1, 768, out);    	break;
   }
}

void sptClass :: MakeHeader(SPTHEADER *head)
{
   strncpy(head->id, "SPT", 3);
   head->version = 1;
   head->width  = nWidth;
   head->height = nHeight;
   head->startx = 0;
   head->starty = 0;
   head->backcolor = 0;
   head->interlace = 0;
   head->hres = 640;
   head->vres = 480;
   head->compression = 0;
   memset(head->extra, 0, 104);

	switch(nBitsPerPixel) {
		case COLOR_1600:
			head->bitsperpixel = 24;
			head->nplanes = 1;
			break;
		case COLOR_32768:
			head->bitsperpixel = 15;
			head->nplanes = 1;
			break;
		case COLOR_256:
			head->bitsperpixel = 8;
			head->nplanes = 1;
			break;
		case COLOR_16:
			head->bitsperpixel = 4;
			head->nplanes = 4;
			break;
		case COLOR_2:
			head->bitsperpixel = 1;
			head->nplanes = 1;
			break;
	}
}

int sptClass :: WriteOpen(char *filename)
{
	SPTHEADER head;
	
	out = fopen(filename, "wb");

	if(out == NULL) {
		return 0;
	}

	MakeHeader(&head);

	fwrite(&head, sizeof(SPTHEADER), 1, out);
	
	PutRGB(dac);

	return 1;
}

int sptClass :: PutOneLine16(BYTE *buf)
{
	int oneplanebyte = (nWidth+7)/8;
	StackBYTE stack(oneplanebyte*4);
	int color;
	int i;
	int plane;

	if(stack.data == NULL)	return 0;

	memset(stack.data, 0, oneplanebyte*4);
	for(i = 0; i < nWidth; i++) {
		if((i%2) == 0)		color = (buf[i/2] >> 4)	& 0x0F;			
		else					color = (buf[i/2] >> 0)	& 0x0F;			
		
		for(plane = 0; plane < 4; plane++) {
			stack.data[oneplanebyte*plane+i/8] |= BIT_MASK[plane+4] & color ? BIT_MASK[i%8] : 0;
		}
	}

	fwrite(stack.data, 1, oneplanebyte*4, out);

	return 1;
}

//----------------------------------------------------------------------------
//
//----------------------------------------------------------------------------

int  sptClass :: PutOneLine(BYTE *buf)
{
	if(nBitsPerPixel == COLOR_16) {
		return PutOneLine16(buf);
	}
   int byteperline;

   switch (nBitsPerPixel) {
      case COLOR_2:	
			byteperline = (nWidth+7)/8;
			break;
      case COLOR_16:	
			byteperline = (nWidth+7)/8*4;
			break;
      case COLOR_256:	
			byteperline = (nWidth);
			break;
      case COLOR_32768:
      case COLOR_65536:	
			byteperline = (nWidth)*2;
			break;
      case COLOR_1600:	
			byteperline = (nWidth)*3;
			break;
   }

   fwrite(buf, 1, byteperline, out);

   return 1;
}
*/