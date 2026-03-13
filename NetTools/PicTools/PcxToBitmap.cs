using System;
using System.Drawing;
using System.Drawing.Imaging;
using NetTools;
using System.Runtime.InteropServices;
using System.IO;

namespace PicTools
{
	/// <summary>
	/// Summary description for PcxToBitmap.
	/// </summary>
	public class PcxToBitmap
	{
		public PcxToBitmap()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public Bitmap Load(string filename)
		{
			PcxTools pcx = new PcxTools();
			int widthbyte;
			byte[] buf;
			int width, height;
			byte[] dac = new byte[768];
			
			Bitmap bitmap = null;

			if(!pcx.ReadOpen(filename)) 
			{
				return null;
			}
			pcx.GetDac(ref dac);
			pcx.GetSize(out width, out height);

			widthbyte = Tools.BmpWidthToByte(width, pcx.GetColor());

			buf = new byte[widthbyte];
            int  bytesperline24 = Tools.BmpWidthToByte(width, 24);
			byte[] data = new byte[bytesperline24*height];

			int x, y;
			int color;
			int pos;

			if(pcx.GetColor() == 24) 
			{
				for(y = 0; y < height; y++) 
				{
					if(!pcx.GetOneLine(ref buf, width))	break;
					pos = bytesperline24*y;

					for(x = 0; x < bytesperline24; x++)
					{
						data[pos+x] = buf[x];
					}
				}
			}
			else if(pcx.GetColor() == 8) 
			{
				for(y = 0; y < height; y++) 
				{
					if(!pcx.GetOneLine(ref buf, width))	break;
					pos = bytesperline24*y;

					for(x = 0; x < width; x++)
					{
						data[pos+x*3+2] = dac[buf[x]*3+0];
						data[pos+x*3+1] = dac[buf[x]*3+1];
						data[pos+x*3+0] = dac[buf[x]*3+2];
					}
				}
			}
			else if(pcx.GetColor() == 4) 
			{
				//int color_no;
				for(y = 0; y < height; y++) 
				{
					if(!pcx.GetOneLine(ref buf, width))	break;
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
			else if(pcx.GetColor() == 1) 
			{
				byte val;

				for(y = 0; y < height; y++) 
				{
					if(!pcx.GetOneLine(ref buf, width))	break;
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

            pcx.ReadClose();

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
            PcxTools pcx = new PcxTools();
            int widthbyte;
            byte[] buf;
            int width, height;
            byte[] dac = new byte[768];

            Bitmap bitmap = null;

            if (!pcx.ReadOpen(stream))
            {
                return null;
            }

            pcx.GetDac(ref dac);
            pcx.GetSize(out width, out height);

            widthbyte = Tools.BmpWidthToByte(width, pcx.GetColor());

            buf = new byte[widthbyte];
            int bytesperline24 = Tools.BmpWidthToByte(width, 24);
            byte[] data = new byte[bytesperline24 * height];

            int x, y;
            int color;
            int pos;

            if (pcx.GetColor() == 24)
            {
                for (y = 0; y < height; y++)
                {
                    if (!pcx.GetOneLine(ref buf, width)) break;
                    pos = bytesperline24 * y;

                    for (x = 0; x < bytesperline24; x++)
                    {
                        data[pos + x] = buf[x];
                    }
                }
            }
            else if (pcx.GetColor() == 8)
            {
                for (y = 0; y < height; y++)
                {
                    if (!pcx.GetOneLine(ref buf, width)) break;
                    pos = bytesperline24 * y;

                    for (x = 0; x < width; x++)
                    {
                        data[pos + x * 3 + 2] = dac[buf[x] * 3 + 0];
                        data[pos + x * 3 + 1] = dac[buf[x] * 3 + 1];
                        data[pos + x * 3 + 0] = dac[buf[x] * 3 + 2];
                    }
                }
            }
            else if (pcx.GetColor() == 4)
            {
                //int color_no;
                for (y = 0; y < height; y++)
                {
                    if (!pcx.GetOneLine(ref buf, width)) break;
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
            else if (pcx.GetColor() == 1)
            {
                byte val;

                for (y = 0; y < height; y++)
                {
                    if (!pcx.GetOneLine(ref buf, width)) break;
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

            pcx.ReadClose();

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

        /*
        public Bitmap LoadFromStream(MemoryStream stream)
        {
            PcxTools pcx = new PcxTools();
            int widthbyte;
            byte[] buf;
            int width, height;
            byte[] dac = new byte[768];

            Bitmap bitmap = null;

            if (!pcx.ReadOpen(stream))
            {
                return null;
            }
            pcx.GetDac(ref dac);
            pcx.GetSize(out width, out height);

            widthbyte = Tools.BmpWidthToByte(width, pcx.GetColor());

            buf = new byte[widthbyte];
            int bytesperline24 = Tools.BmpWidthToByte(width, 24);
            byte[] data = new byte[bytesperline24 * height];

            int x, y;
            int color;
            int pos;

            if (pcx.GetColor() == 24)
            {
                for (y = 0; y < height; y++)
                {
                    if (!pcx.GetOneLine(ref buf, width)) break;
                    pos = bytesperline24 * y;

                    for (x = 0; x < bytesperline24; x++)
                    {
                        data[pos + x] = buf[x];
                    }
                }
            }
            else if (pcx.GetColor() == 8)
            {
                for (y = 0; y < height; y++)
                {
                    if (!pcx.GetOneLine(ref buf, width)) break;
                    pos = bytesperline24 * y;

                    for (x = 0; x < width; x++)
                    {
                        data[pos + x * 3 + 2] = dac[buf[x] * 3 + 0];
                        data[pos + x * 3 + 1] = dac[buf[x] * 3 + 1];
                        data[pos + x * 3 + 0] = dac[buf[x] * 3 + 2];
                    }
                }
            }
            else if (pcx.GetColor() == 4)
            {
                //int color_no;
                for (y = 0; y < height; y++)
                {
                    if (!pcx.GetOneLine(ref buf, width)) break;
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
            else if (pcx.GetColor() == 1)
            {
                byte val;

                for (y = 0; y < height; y++)
                {
                    if (!pcx.GetOneLine(ref buf, width)) break;
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

            pcx.ReadClose();

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
        */
		


	}
}
