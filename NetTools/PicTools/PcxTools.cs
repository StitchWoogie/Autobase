using System;
using System.IO;
using System.Windows.Forms;

namespace PicTools
{
	// total 128 byte
	public class PCXHEADER
	{
		public sbyte maker;					// allways 10
		public sbyte version;					//
		public sbyte code;
		public sbyte bitperpixel;			//	1 - mono, 16color, 8 - 256 color
		public ushort  x1;						// 그림의 시작점 x
		public ushort  y1;                 // 그림의 시작점 y
		public ushort  x2;                 //	그림의 끝점   x
		public ushort  y2;                 //	그림의 끝점   y
		public ushort  hres;               //	그림그릴때의 수평해상도
		public ushort  vres;               //	그림그릴때의 수직해상도
		public byte[] rgb = new byte[48];					//	RGB [16][3]
		public sbyte vmode;					//
		public sbyte nplanes;					//	4 - 16 color, 1 - mono, 256
		public ushort  byteperline;        //	bytes per line
		public ushort  palinfo;
		public ushort  shres;
		public ushort  svres;
		public byte[] extra = new byte[54];
	} 
	/// <summary>
	/// Summary description for PcxTools.
	/// </summary>
	public class PcxTools : PictureFileClass
	{
		public PCXHEADER head = new PCXHEADER();

		public PcxTools()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		protected override bool ReadOpenLocal()
		{
			try 
			{
				head.maker = br.ReadSByte();				// always 10
				head.version = br.ReadSByte();				//
				head.code = br.ReadSByte();
				head.bitperpixel = br.ReadSByte();			// 1 - mono, 16color, 8 - 256 color
				head.x1 = br.ReadUInt16();					// 그림의 시작점 x
				head.y1 = br.ReadUInt16();					// 그림의 시작점 y
				head.x2 = br.ReadUInt16();					// 그림의 끝점   x
				head.y2 = br.ReadUInt16();					// 그림의 끝점   y
				head.hres = br.ReadUInt16();				// 그림그릴때의 수평해상도
				head.vres = br.ReadUInt16();				// 그림그릴때의 수직해상도
				head.rgb = br.ReadBytes(48);				// RGB [16][3]
				head.vmode = br.ReadSByte();				//
				head.nplanes = br.ReadSByte();				// 4 - 16 color, 1 - mono, 256
				head.byteperline = br.ReadUInt16();			// bytes per line
				head.palinfo = br.ReadUInt16();
				head.shres = br.ReadUInt16();
				head.svres = br.ReadUInt16();
				head.extra = br.ReadBytes(54);
			}
			catch (Exception exception)
			{
				string msg = String.Format("This file is not PCX format.\nFileName={0}\nError Message={1}", this.sFileName, exception.Message);
				MessageBox.Show(msg, "Read Error");
				return false;
			}

			if(head.maker != 10) 
			{
				if(NetTools.Tools.IsLangKorean())
                    MessageBox.Show("PCX 파일이 아닙니다.", this.sFileName);
				else
                    MessageBox.Show("This file is not PCX format.", this.sFileName);

				return false;
			}

			nWidth = head.x2-head.x1+1;
			nHeight = head.y2-head.y1+1;

			//---------------------------------------------------------------------------
			//	주어진 그림에서 사용된 색상수를 얻는다
			//---------------------------------------------------------------------------
			if(head.nplanes == 3 && head.bitperpixel == 8)
				nBitsPerPixel = 24;
			else if( (head.nplanes == 1) && head.bitperpixel == 8)
				nBitsPerPixel = 8;
			else if( (head.nplanes == 1 || head.nplanes == 2) && head.bitperpixel == 1)
				nBitsPerPixel = 1;
			else if( head.nplanes == 4 && head.bitperpixel == 1)
				nBitsPerPixel = 4;
			else 
			{
				string message;

				if(NetTools.Tools.IsLangKorean()) 
				{
					message = String.Format("지원되지 않는 색상을 가진 그림입니다.\nhead.nplanes={0}\nhead.bitperpixel={1}",
						head.nplanes, head.bitperpixel);
					MessageBox.Show(message, "PCX 색상 이상");
				}
				else {
					message = String.Format("This color format is not supported.\nhead.nplanes={0}\nhead.bitperpixel={1}",
						head.nplanes, head.bitperpixel);
					MessageBox.Show(message, "PCX format error");
				}

				return false;
			}

			//----------------------------------------------------------------------------
			// RGB 정보를 읽어온다.
			// RGB 정보는 256일때는 화일의 맨뒤에 768바이트가 붙는다.
			//----------------------------------------------------------------------------

			int ch, i;

			switch( nBitsPerPixel ) 
			{
				case 8:	br.BaseStream.Seek(-769, SeekOrigin.End);
                    ch = br.BaseStream.ReadByte();
					if(ch == 12) 
					{
						dac = br.ReadBytes(768);
					}
					else 
					{
						br.BaseStream.Seek(16, SeekOrigin.Begin);
                        //dac = br.ReadBytes(48);  dac는 항상 768이 필요하므로 다른버퍼에 읽은 뒤 복사해서 사용한다. (2017-8-11)
                        byte[] imsi_dac = br.ReadBytes(48);
                        Array.Copy(imsi_dac, dac, 48);
						dac[765] = dac[766] = dac[767] = 255;
					}
					break;
				case 4:
					for(i = 0; i < 48; i++)	dac[i] = head.rgb[i];
					//head.rgb.CopyTo(dac, 0);
					break;
				case 1:
					for(i = 0; i < 3; i++) dac[i] = 0;
					for(i = 3; i < 6; i++) dac[i] = 255;
					break;
			}

            br.BaseStream.Seek(128, SeekOrigin.Begin);

			return true;
		}

		//---------------------------------------------------------------------------
		//	PCX 화일에서 한라인의 그림을 읽어온다..
		//	maxx 는 버퍼에서 제한하는 최대값이다.
		//	bufcolor 는 채울버퍼의 칼라를 큉함한다.
		//---------------------------------------------------------------------------

		public bool GetOneLine(ref byte[] buf, int maxx)
		{
			bool retn = false;

			switch ( nBitsPerPixel ) 
			{
				case 1:
					retn = GetOneLine2To2(ref buf, maxx);	break;
				case 4:
					retn = GetOneLine16To16(ref buf, maxx);	break;
				case 8:
					retn = GetOneLine256To256(ref buf, maxx);	break;
				case 24:
					retn = GetOneLine1600To1600(ref buf, maxx);	break;
			}
			return ( retn );
		}

		//----------------------------------------------------------------------------
		//	그림이 1600인 화일을 1600버퍼에 읽어온다.
		//----------------------------------------------------------------------------

		bool GetOneLine1600To1600(ref byte[] buf, int maxx)
		{
			int xx=0, xb, rc, ch;
			byte data;
			int plane = 2;

			xb = head.byteperline;

			while(true)
			{
				try 
				{
					ch = br.ReadByte();
				}
				catch
				{
					return false;
				}
				if( (ch & 0xC0) == 0xC0 ) 
				{  	// 1100 0000
					rc = ch & 0x3F;                // 같은 데이타가 rc 만큼 반복한다.
                    try
                    {
                        data = br.ReadByte();
                    }
                    catch
                    {
                        return false;
                    }
				}
				else 
				{
					rc = 1;
					data = (byte)ch;
				}

				while ( rc-- > 0) 
				{
					if(xx < maxx)        		// 좼퍼의 범챦를 벗어나지 않았을때
						buf[xx*3+plane] = data;
					xx++;
					if(xx >= xb) 
					{                    // 한줄을 모두 읽었을때
						if(plane <= 0)	return true;
						xx = 0;
						plane--;
					}
				}
			}
		}

		//----------------------------------------------------------------------------
		//	그림이 256인 화일을 256좼퍼찌 읽어온다.
		//----------------------------------------------------------------------------
		bool GetOneLine256To256(ref byte[] buf, int maxx)
		{
			int xx=0, xb, rc, ch;
			byte data;

			xb = head.byteperline;

			while(true) 
			{
				try 
				{
					ch = br.ReadByte();
				}
				catch
				{
					return false;
				}
				if( (ch & 0xC0) == 0xC0 ) 
				{  	// 1100 0000
					rc = ch & 0x3F;                // 같은 온이타가 rc 만큼 반복한다.
                    try
                    {
                        data = br.ReadByte();
                    }
                    catch
                    {
                        return false;
                    }
				}
				else 
				{
					rc = 1;
					data = (byte)ch;
				}

				while ( rc-- > 0) 
				{
					if(xx < maxx)        		// 좼퍼의 범챦를 벗어나지 않았을때
						buf[xx] = data;

					xx++;

					if(xx >= xb)                     // 한줄을 모두 읽었을때
						return (true);
				}

				if(xx >= xb)                     // 한줄을 모두 읽었을때
					return (true);
			}
		}

		//----------------------------------------------------------------------------
		//	그림이 16인 화일을 16버퍼에 읽어온다.
		//----------------------------------------------------------------------------

		bool GetOneLine16To16(ref byte[] buf, int maxx)
		{
			int xx=0, xb, rc, ch;
			byte data;
			int plane = 4;

			int limitbyte = (maxx+7)/8;

			xb = head.byteperline;

			for(int i = 0; i < buf.Length; i++)	buf[i] = 0;

			while(true) 
			{
				try 
				{
					ch = br.ReadByte();
				}
				catch 
				{
					return false;
				}
				if( (ch & 0xC0) == 0xC0 ) 
				{  	// 1100 0000
					rc = ch & 0x3F;                // 같은 데이타가 rc 만큼 반복한다.
                    try
                    {
                        data = br.ReadByte();
                    }
                    catch
                    {
                        return false;
                    }
				}
				else 
				{
					rc = 1;
					data = (byte)ch;
				}

				while ( rc-- > 0) 
				{
					if(xx < limitbyte) 
					{       		// 버퍼의 범위를 벗어나지 않았을때
						for(int i = 0; i < 8; i+=2) 
						{	// packed memory buffer
							buf[xx*4+i/2] |= (data & BIT_MASK[i]) == BIT_MASK[i]  ? BIT_MASK[(plane-1)] : (byte)0;
							buf[xx*4+i/2] |= (data & BIT_MASK[i+1]) == BIT_MASK[i+1] ? BIT_MASK[(plane-1)+4] : (byte)0;
						}
					}
					xx++;

					if(xx >= xb) 
					{
						if(plane <= 1)	return (true);
						xx = 0;
						plane--;
					}
				}
			}
		}

		bool GetOneLine2To2(ref byte[] buf, int maxx)
		{
			int xx=0, xb, rc, ch;
			byte data;

			int limitbyte = (maxx+7) / 8;

			xb = head.byteperline;

			while(true) 
			{
				try 
				{
					ch = br.ReadByte();
				}
				catch
				{
					return false;
				}
				if( (ch & 0xC0) == 0xC0) 
				{
					rc = ch & 0x3F;
                    try
                    {
                        data = br.ReadByte();
                    }
                    catch
                    {
                        return false;
                    }
				}
				else 
				{
					rc = 1;
					data = (byte)ch;
				}

				while ( rc-- > 0) 
				{
					if(xx < limitbyte) 
					{
						buf[xx] = data;
					}
					xx++;
					if(xx >= xb) 
					{
						return (true);
					}
				}
				if(xx >= xb) 
				{
					return (true);
				}
			}
		}
	}
}

/*


//----------------------------------------------------------------------------
//	 PCX 헤더를 채운다.
//----------------------------------------------------------------------------

int pcxClass :: FillPcxHeader(PCXHEADER *head)
{
   memset(head, 0, sizeof(PCXHEADER));
	
	head->maker = 10;			// allways 10
   head->version = 5;
   head->code = 1;  			// 압축
   head->x1 = 0;				// 그림의 시작점 x

   head->y1 =	0;          // 그림의 시작점 y
   head->x2 =	nWidth-1;	//	그림의 끝점   x
   head->y2 =	nHeight-1;  //	그림의 끝점   y
   head->hres = 640;			//	그림그릴때의 수평해상도
   head->vres = 480;			//	그림그릴때의 수직해상도
   head->vmode = 0;        //

	switch(nBitsPerPixel) {
		case COLOR_1600:
			head->nplanes = 3;
			head->byteperline = nWidth;
			head->bitperpixel = 8;
			break;
		case COLOR_256:
			head->nplanes = 1;				// 1 - mono, 256
			head->byteperline = nWidth;	//	bytes per line
			head->bitperpixel = 8;			//	1 - mono, 16color, 8 - 256 color
			break;
		case COLOR_16:
			head->nplanes = 4;            // 1 - mono, 256
			head->byteperline = (nWidth+7)/8; 	//	bytes per line
			head->bitperpixel = 1;        //	1 - mono, 16color, 8 - 256 color
			memcpy(head->rgb, dac, 48);
			break;
		case COLOR_2:
			head->nplanes = 1;            // 1 - mono, 256
			head->byteperline = (nWidth+7)/8; 	//	bytes per line
			head->bitperpixel = 1;        //	1 - mono, 16color, 8 - 256 color
			break;
		default:
			MessageBox(NULL, "지원되지 않는 색상", "pcxClass :: FillPcxHeader()", MB_OK);
			return 0;
	}

	return 1;
}

//---------------------------------------------------------------------------
//	PCX 화일에서 RGB 정보를 쓴다.
//	여기서는 256 칼라일때만 유용하므로 나머지는 필요없다.
// 파일의 맨 뒷부분에 (12)+RGB 가 위치한다.
//---------------------------------------------------------------------------

void pcxClass :: PutRGB()
{
   if(nBitsPerPixel != COLOR_256)	return;
	
   fputc(12, out);
   fwrite(dac, 1, 768, out);
}

int pcxClass :: WriteOpen(HWND hwnd, char *filename)
{
   if(dac == NULL)	return 0;

	if(!FillPcxHeader(&head))	return 0;

   out = fopen(filename, "wb");
   if(out == NULL)	{
		WriteOpenError(hwnd, filename);
		return 0;
   }

   fwrite(&head, 1, sizeof(PCXHEADER), out);

   return 1;
}

//-----------------------------------------------------------
//	파일을 닫기전에 256칼라의 색상판을 파일의 맨뒤에 저장한다. 
//-----------------------------------------------------------

void pcxClass :: WriteClosePrepare()
{
	PutRGB();
}

//----------------------------------------------------------------------------
//	PCX 화일에 한 라인을 압축한다.
//
//	압축 방법은 다음과 같다.
//
//	같은 바이트가 반복되면 반복수, 데이타
//	다른 바이트 반복시는 0xC0 보다 작은 데이타는 그냥써주고,
//	0xC0 보다 크거나 같은 데이타는 0xC1, 데이타를 써준다.
//----------------------------------------------------------------------------

int  pcxClass :: PutOneLine(BYTE *buf)
{
   if(nBitsPerPixel == COLOR_16) {
		return PutOneLine16(buf);
	}
	else {
		return PutOneLineElse(buf);
	}
}

int  pcxClass :: PutOneLineElse(BYTE *buf)
{
	BYTE *p;
   register int i;
   register int count = 0;
   BYTE save;
   register int plane;
   int plus = 1; 	
	// 256칼라 이전까지는 1씩 버퍼위치를 증가시키면
	// 되지만 그 이상의 색상에서는 증가율을 조정한다.

   if(head.nplanes == 3 && head.bitperpixel == 8) {	// 1600 color
      plus = 3;         // 버퍼를 저장할때 R,G,B plane 순으로 저장한다.
   }

   for(plane = head.nplanes-1; plane >= 0; plane--) {
      
		if(head.nplanes == 3 && head.bitperpixel == 8)  // 1600M color
			p = &buf[plane];          // 0, 1, 2 순서
      else
			p = &buf[plane*head.byteperline];	// B,G,R,I plane save

      count = 1;
      save = *p;
      p+=plus;
      for(i = 1; i < head.byteperline; i++, p+=plus) {
			if(*p == save) {
				if(count >= 0x3F) {
					fputc(0xFF, out);
					fputc(save, out);
					count = 0;
				}
				count ++;
			}
			else {      		// 앞바이트와 뒤바이트가 같지 않을때
				if(count > 1) {     // 앞에 헤아린 수가 두개 이상일때
					fputc(0xC0 | (BYTE) count, out);
					fputc(save, out);
				}
				else {              // 앞에 헤아린 수가 하나일때
					if(save >= 0xC0) { 	// 데이타가 0xC0 이상일때
						fputc(0xC1, out);
						fputc(save, out);
					}
					else {                   // 데이타가 0xC0 보다 작을때
						fputc(save, out);
					}
				}
				count = 1;
				save = *p;
			}      // 앞바이트와 뒤바이트가 같지 않을때
		}         // byteperline for loop

		//**********************************
		// 미처 다쓰지 못한 데이타를 쓴다.
		// 여기서 이런이유는 모든데이타의 압축은
		// 한줄단위로 (혹은 한 플랜) 압축되기 때문이다.
		//**********************************

		if(count > 1)
			fputc(0xC0 | (BYTE) count, out);
		else if(save >= 0xC0)
			fputc(0xC1, out);
		else;

		fputc(save, out);
	}    	// plane for loop
	return 1;
}


int  pcxClass :: PutOneLine16(BYTE *buf)
{
	BYTE *p;
   register int i;
   register int count = 0;
   BYTE save;
   register int plane;
   int plus = 1; 	
	int oneplanebyte = (nWidth+7)/8;
	StackBYTE stack(oneplanebyte*4);
	int color;

	if(stack.data == NULL)	return 0;

	memset(stack.data, 0, oneplanebyte*4);
	for(i = 0; i < nWidth; i++) {
		if((i%2) == 0)		color = (buf[i/2] >> 4)	& 0x0F;			
		else					color = (buf[i/2] >> 0)	& 0x0F;			
		
		for(plane = 3; plane >= 0; plane--) {
			stack.data[oneplanebyte*plane+i/8] |= BIT_MASK[plane+4] & color ? BIT_MASK[i%8] : 0;
		}
	}

	for(plane = head.nplanes-1; plane >= 0; plane--) {
      
		if(head.nplanes == 3 && head.bitperpixel == 8)  // 1600M color
			p = &stack.data[plane];          // 0, 1, 2 순서
      else
			p = &stack.data[plane*head.byteperline];	// B,G,R,I plane save

      count = 1;
      save = *p;
      p+=plus;
      for(i = 1; i < head.byteperline; i++, p+=plus) {
			if(*p == save) {
				if(count >= 0x3F) {
					fputc(0xFF, out);
					fputc(save, out);
					count = 0;
				}
				count ++;
			}
			else {      		// 앞바이트와 뒤바이트가 같지 않을때
				if(count > 1) {     // 앞에 헤아린 수가 두개 이상일때
					fputc(0xC0 | (BYTE) count, out);
					fputc(save, out);
				}
				else {              // 앞에 헤아린 수가 하나일때
					if(save >= 0xC0) { 	// 데이타가 0xC0 이상일때
						fputc(0xC1, out);
						fputc(save, out);
					}
					else {                   // 데이타가 0xC0 보다 작을때
						fputc(save, out);
					}
				}
				count = 1;
				save = *p;
			}      // 앞바이트와 뒤바이트가 같지 않을때
		}         // byteperline for loop

		//**********************************
		// 미처 다쓰지 못한 데이타를 쓴다.
		// 여기서 이런이유는 모든데이타의 압축은
		// 한줄단위로 (혹은 한 플랜) 압축되기 때문이다.
		//**********************************

		if(count > 1)
			fputc(0xC0 | (BYTE) count, out);
		else if(save >= 0xC0)
			fputc(0xC1, out);
		else;

		fputc(save, out);
	}    	// plane for loop
	return 1;
}

*/

