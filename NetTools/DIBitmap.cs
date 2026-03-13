using System;
using System.IO;
using System.Drawing;
using NetTools;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace NetTools
{
	/// <summary>
	/// Summary description for DIBitmap.
	/// </summary>
	public class DIBitmap
	{
		public DIBitmap()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		struct  BITMAPINFOHEADER
		{
			public uint  biSize;		// 총 40 바이트
			public int   biWidth;
			public int   biHeight;
			public ushort biPlanes;
			public ushort biBitCount;	// 여기까지가 BITMAPCOREHEADER

			public uint  biCompression;
			public uint  biSizeImage;
			public int   biXPelsPerMeter;
			public int   biYPelsPerMeter;
			public uint  biClrUsed;
			public uint  biClrImportant;

			public void Read(BinaryReader reader)
			{
				biSize = reader.ReadUInt32();
				biWidth = reader.ReadInt32();
				biHeight = reader.ReadInt32();
				biPlanes = reader.ReadUInt16();
				biBitCount = reader.ReadUInt16();

				biCompression = reader.ReadUInt32();
				biSizeImage   = reader.ReadUInt32();
				biXPelsPerMeter = reader.ReadInt32();
				biYPelsPerMeter = reader.ReadInt32();
				biClrUsed = reader.ReadUInt32();
				biClrImportant = reader.ReadUInt32();
			}
		}

		struct RGBQUAD 
		{
			public byte    rgbBlue; 
			public byte    rgbGreen; 
			public byte    rgbRed; 
			public byte    rgbReserved; 
		}

		class BITMAPINFO
		{
			public BITMAPINFOHEADER bmiHeader = new BITMAPINFOHEADER();
			public RGBQUAD[] bmiColors = null;

			public void Read(BinaryReader reader)
			{
				bmiHeader.Read(reader);

				uint table_size = GetDibColorTableSize(bmiHeader);
				if(table_size > 0)
				{
					bmiColors = new RGBQUAD[table_size];
					for(int j = 0; j < table_size; j++) 
					{
						bmiColors[j].rgbBlue = reader.ReadByte();
						bmiColors[j].rgbGreen = reader.ReadByte();
						bmiColors[j].rgbRed = reader.ReadByte();
						bmiColors[j].rgbReserved = reader.ReadByte();
					}
				}
			}

			uint GetDibInfoHeaderSize(BITMAPINFOHEADER header)
			{
				return header.biSize;
			}

			uint GetDibColorTableSize(BITMAPINFOHEADER header)
			{
				uint dwNumColors, dwColorTableSize;
				ushort wBitCount;

				if (GetDibInfoHeaderSize(header) == 12) // sizeof (BITMAPCOREHEADER))
				{
					wBitCount = header.biBitCount;

					if (wBitCount != 24)
						dwNumColors = (uint)1 << wBitCount;
					else
						dwNumColors = 0;

					dwColorTableSize = dwNumColors * 3;//sizeof (RGBTRIPLE) ;
				}
				else 
				{
					wBitCount = header.biBitCount;

					if(GetDibInfoHeaderSize(bmiHeader) >= 36)
						dwNumColors = header.biClrUsed;
					else
						dwNumColors = 0;

					if (dwNumColors == 0)
					{
						if (wBitCount != 24)
							dwNumColors = (uint)1 << wBitCount ;
						else
							dwNumColors = 0;
					}

					dwColorTableSize = dwNumColors * 4;//sizeof (RGBQUAD) ;
				}

				return dwNumColors;
			}
		}

		public Bitmap Read(BinaryReader reader)
		{
			// bitmap info
			BITMAPINFO info = new BITMAPINFO();
			info.Read(reader);

			// bitmap data
			int  bytesperline = Tools.BmpWidthToByte(info.bmiHeader.biWidth, info.bmiHeader.biBitCount);
			int  bytesperline24 = Tools.BmpWidthToByte(info.bmiHeader.biWidth, 24);
			byte[] data = new byte[bytesperline24*info.bmiHeader.biHeight];
			byte[] data_org = new byte[bytesperline];
			int pos;

			if(info.bmiHeader.biBitCount == 24) 
			{
				for(int j = 0; j < info.bmiHeader.biHeight; j++) 
				{
					pos = bytesperline*(info.bmiHeader.biHeight-1)-bytesperline*j;

					for(int k = 0; k < bytesperline; k++) 
					{
						data[pos+k] = reader.ReadByte();
					}
				}
			}
			else if(info.bmiHeader.biBitCount == 8) 
			{
				for(int j = 0; j < info.bmiHeader.biHeight; j++) 
				{
					pos = bytesperline24*(info.bmiHeader.biHeight-1)-bytesperline24*j;

					data_org = reader.ReadBytes(bytesperline);

					for(int k = 0; k < info.bmiHeader.biWidth; k++) 
					{
						data[pos+k*3+2] = info.bmiColors[data_org[k]].rgbRed;
						data[pos+k*3+1] = info.bmiColors[data_org[k]].rgbGreen;
						data[pos+k*3+0] = info.bmiColors[data_org[k]].rgbBlue;
					}
				}
			}
			else if(info.bmiHeader.biBitCount == 4) 
			{
				int color_no;
				for(int j = 0; j < info.bmiHeader.biHeight; j++) 
				{
					pos = bytesperline24*(info.bmiHeader.biHeight-1)-bytesperline24*j;

					data_org = reader.ReadBytes(bytesperline);

					for(int k = 0; k < info.bmiHeader.biWidth; k++) 
					{
						if(k%2 == 0) 
							color_no = (data_org[k/2] >> 4) & 0x0F;
						else
							color_no = (data_org[k/2] >> 0) & 0x0F;

						data[pos+k*3+2] = info.bmiColors[color_no].rgbRed;
						data[pos+k*3+1] = info.bmiColors[color_no].rgbGreen;
						data[pos+k*3+0] = info.bmiColors[color_no].rgbBlue;
					}
				}
			}

			else	// 데이터만 읽어준다. 실제 2칼라를 사용한 애니매이션은 없다(편집기 버거로 만들 수 없다.)
			{
				for(int j = 0; j < info.bmiHeader.biHeight; j++) 
				{
					data_org = reader.ReadBytes(bytesperline);
				}
			}

			GCHandle ptrBuffer = GCHandle.Alloc(data, GCHandleType.Pinned);
			IntPtr ptr = ptrBuffer.AddrOfPinnedObject();

			Bitmap bitmap = new Bitmap(info.bmiHeader.biWidth, info.bmiHeader.biHeight, bytesperline24, PixelFormat.Format24bppRgb, ptr);
			bitmap = new Bitmap(bitmap);
			ptrBuffer.Free();
			

			return bitmap;
		}
	}
}
