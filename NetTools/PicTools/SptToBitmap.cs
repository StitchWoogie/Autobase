using System;
using System.Drawing;
using NetTools;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;

namespace PicTools
{
	/// <summary>
	/// Summary description for SptToBitmap.
	/// </summary>
	public class SptToBitmap
	{
		public SptToBitmap()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public Bitmap Load(string filename)
		{
			SptTools spt = new SptTools();
			//int i; 
			int widthbyte;
			byte[] buf;
			int width, height;
			byte[] dac = new byte[768];
			Bitmap bitmap = null;

			if(!spt.ReadOpen(filename)) 
			{
				return null;
			}
			spt.GetDac(ref dac);
			spt.GetSize(out width, out height);

			widthbyte = Tools.BmpWidthToByte(width, spt.GetColor());

			buf = new byte[widthbyte];
			int  bytesperline24 = Tools.BmpWidthToByte(width, 24);
			byte[] data = new byte[bytesperline24*height];

			int x, y;
			int color;
			int pos;

			if(spt.GetColor() == 24) 
			{
				for(y = 0; y < height; y++) 
				{
					if(!spt.GetOneLine(ref buf, width))	break;
					pos = bytesperline24*y;

					for(x = 0; x < bytesperline24; x++)
					{
						data[pos+x] = buf[x];
					}
				}
			}
			else if(spt.GetColor() == 8) 
			{
				for(y = 0; y < height; y++) 
				{
					if(!spt.GetOneLine(ref buf, width))	break;
					pos = bytesperline24*y;

					for(x = 0; x < width; x++)
					{
						data[pos+x*3+2] = dac[buf[x]*3+0];
						data[pos+x*3+1] = dac[buf[x]*3+1];
						data[pos+x*3+0] = dac[buf[x]*3+2];
					}
				}
			}
			else if(spt.GetColor() == 4) 
			{
				//int color_no;
				for(y = 0; y < height; y++) 
				{
					if(!spt.GetOneLine(ref buf, width))	break;
					pos = bytesperline24*y;

					for(x = 0; x < width; x++)
					{
						if(x%2 == 0) 
							color = (buf[x/2] >> 4) & 0x0F;
						else
							color = (buf[x/2] >> 0) & 0x0F;

						data[pos+x*3+2] = dac[color*3+0];
						data[pos+x*3+1] = dac[color*3+1];
						data[pos+x*3+0] = dac[color*3+2];
					}
				}
			}
			else if(spt.GetColor() == 1) 
			{
				byte val;

				for(y = 0; y < height; y++) 
				{
					if(!spt.GetOneLine(ref buf, width))	break;
					pos = bytesperline24*y;

					for(x = 0; x < width; x++)
					{
						if((buf[x/8] & PictureFileClass.BIT_MASK[x%8]) == PictureFileClass.BIT_MASK[x%8]) 
							val = 255;
						else
							val = 0;

						data[pos+x*3+0] = val;
						data[pos+x*3+1] = val;
						data[pos+x*3+2] = val;
					}
				}
			}

			else	
			{
			}

			spt.ReadClose();

			GCHandle ptrBuffer = GCHandle.Alloc(data, GCHandleType.Pinned);
			IntPtr ptr = ptrBuffer.AddrOfPinnedObject();		
			bitmap = new Bitmap(width, height, bytesperline24, PixelFormat.Format24bppRgb, ptr);
			//ptrBuffer.Free();

			//IntPtr intptr = Marshal.AllocCoTaskMem(bytesperline24*height);
			//Marshal.Copy(data, 0, intptr, data.Length);
			//bitmap = new Bitmap(width, height, bytesperline24, PixelFormat.Format24bppRgb, intptr);

			// 이 부분이 없으니까 Dispose되어도 메모리에 남아있는다.
			bitmap = new Bitmap(bitmap);
			ptrBuffer.Free();

			return bitmap;
		}


        public Bitmap Load(MemoryStream stream)
        {
            SptTools spt = new SptTools();
            //int i; 
            int widthbyte;
            byte[] buf;
            int width, height;
            byte[] dac = new byte[768];
            Bitmap bitmap = null;

            if (!spt.ReadOpen(stream))
            {
                return null;
            }
            spt.GetDac(ref dac);
            spt.GetSize(out width, out height);

            widthbyte = Tools.BmpWidthToByte(width, spt.GetColor());

            buf = new byte[widthbyte];
            int bytesperline24 = Tools.BmpWidthToByte(width, 24);
            byte[] data = new byte[bytesperline24 * height];

            int x, y;
            int color;
            int pos;

            if (spt.GetColor() == 24)
            {
                for (y = 0; y < height; y++)
                {
                    if (!spt.GetOneLine(ref buf, width)) break;
                    pos = bytesperline24 * y;

                    for (x = 0; x < bytesperline24; x++)
                    {
                        data[pos + x] = buf[x];
                    }
                }
            }
            else if (spt.GetColor() == 8)
            {
                for (y = 0; y < height; y++)
                {
                    if (!spt.GetOneLine(ref buf, width)) break;
                    pos = bytesperline24 * y;

                    for (x = 0; x < width; x++)
                    {
                        data[pos + x * 3 + 2] = dac[buf[x] * 3 + 0];
                        data[pos + x * 3 + 1] = dac[buf[x] * 3 + 1];
                        data[pos + x * 3 + 0] = dac[buf[x] * 3 + 2];
                    }
                }
            }
            else if (spt.GetColor() == 4)
            {
                //int color_no;
                for (y = 0; y < height; y++)
                {
                    if (!spt.GetOneLine(ref buf, width)) break;
                    pos = bytesperline24 * y;

                    for (x = 0; x < width; x++)
                    {
                        if (x % 2 == 0)
                            color = (buf[x / 2] >> 4) & 0x0F;
                        else
                            color = (buf[x / 2] >> 0) & 0x0F;

                        data[pos + x * 3 + 2] = dac[color * 3 + 0];
                        data[pos + x * 3 + 1] = dac[color * 3 + 1];
                        data[pos + x * 3 + 0] = dac[color * 3 + 2];
                    }
                }
            }
            else if (spt.GetColor() == 1)
            {
                byte val;

                for (y = 0; y < height; y++)
                {
                    if (!spt.GetOneLine(ref buf, width)) break;
                    pos = bytesperline24 * y;

                    for (x = 0; x < width; x++)
                    {
                        if ((buf[x / 8] & PictureFileClass.BIT_MASK[x % 8]) == PictureFileClass.BIT_MASK[x % 8])
                            val = 255;
                        else
                            val = 0;

                        data[pos + x * 3 + 0] = val;
                        data[pos + x * 3 + 1] = val;
                        data[pos + x * 3 + 2] = val;
                    }
                }
            }

            else
            {
            }

            spt.ReadClose();

            GCHandle ptrBuffer = GCHandle.Alloc(data, GCHandleType.Pinned);
            IntPtr ptr = ptrBuffer.AddrOfPinnedObject();
            bitmap = new Bitmap(width, height, bytesperline24, PixelFormat.Format24bppRgb, ptr);
            //ptrBuffer.Free();

            //IntPtr intptr = Marshal.AllocCoTaskMem(bytesperline24*height);
            //Marshal.Copy(data, 0, intptr, data.Length);
            //bitmap = new Bitmap(width, height, bytesperline24, PixelFormat.Format24bppRgb, intptr);

            // 이 부분이 없으니까 Dispose되어도 메모리에 남아있는다.
            bitmap = new Bitmap(bitmap);
            ptrBuffer.Free();

            return bitmap;
        }
	}
}

/*
HPICTURE PictureLoadSpt(HWND hwnd, char *filename)
{
	sptClass spt;
	int i; 
	WORD widthbyte;
	BYTE *buf;
	int width, height;
	StackBYTE dac(768);
	HPICTURE hPic;
	PICTURE_STRUCT *pic;

	if(dac.data == NULL)	return 0;

	if(!spt.ReadOpen(filename)) {
		return NULL;
	}
	spt.GetDac(dac.data);
	spt.GetSize(width, height);

	widthbyte = (WORD)BmpWidthToByte(width, spt.GetColor());

	buf = new BYTE[widthbyte];
	if(buf == NULL) {
		MessageBox(hwnd, "기본 메모리 부족으로 그림을 읽어올 수 없습니다.", "메모리 부족", MB_OK);
		return NULL;
	}

	hPic = PictureLoadNew(hwnd, width, height, spt.GetColor(), dac.data);

	if(hPic == NULL) {
		delete buf;
		return NULL;
	}

	pic = (PICTURE_STRUCT *) PictureLock(hPic);
	WaitCursorStart(0, height);
	for(i = 0; i < height; i++) {
		WaitCursorStatus(i);
		if(!spt.GetOneLine(buf, width))	break;
		PicturePutOneLine(pic, i, buf);
	}
	WaitCursorEnd();
	PictureUnlock(hPic, pic);

	delete buf;

	spt.ReadClose();

	return hPic;
}
*/