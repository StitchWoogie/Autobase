using System;
using System.Drawing;
using System.Drawing.Imaging;
using NetTools;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using PicTools;
using System.Drawing.Drawing2D;
using ICSharpCode.SharpZipLib.Zip;
using System.Threading.Tasks;
//using ICSharpCode.SharpZipLib.Checksums;

namespace AutoLibLocal
{
	class ANIMATION_STRUCT2
	{
		public byte[]	id = new byte[9];			// "ANIMATION"
		public short	version;					// 2
		public byte[]	description = new byte[78]; // description
		public short	rpm;						// rpm
		public short	frame;
		public short	clock;						// one frame clock count
		public short	width;						//
		public short	height;						//
		public short	bitsperpixel;				// bits per pixel
		public System.Int32  oneframesize;			// one frame size
	}

	/// <summary>
	/// Summary description for AnimationClass.
	/// </summary>
	/// 
	[Serializable]
	public class AnimationClass
	{
		Bitmap[] bitmapThis;
		int nMaxFrame = 0;
		int nRpm = 0;
		int nWidth = 100;
		int nHeight = 50;
        int nBitsPerPixel = 32;
        public int nVersion = 0;

		[NonSerialized] 
		string sErrorString;

        public bool bError = false;
        string sFilename;

		public AnimationClass()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        public AnimationClass(int width, int height, int frame, int rpm, int bitsperpixel)
        {
            //
            // TODO: Add constructor logic here
            //

            bitmapThis = new Bitmap[frame];

            nWidth = width;
            nHeight = height;
            nBitsPerPixel = bitsperpixel;
            nRpm = rpm;
            nMaxFrame = frame;

            for (int i = 0; i < frame; i++)
            {
                bitmapThis[i] = NewBitmap();
            }
        }

        int ConvertToBitsPerPixel(Bitmap bitmap)
        {
            if (bitmap.PixelFormat == PixelFormat.Format24bppRgb)
                return 24;

            return 32;
        }

        int LoadFileAnimation(string filename, BinaryReader br)
        {
            ANIMATION_STRUCT2 ani = new ANIMATION_STRUCT2();

            ani.id = br.ReadBytes(9);
            ani.version = br.ReadInt16();
            ani.description = br.ReadBytes(78);
            ani.rpm = br.ReadInt16();
            ani.frame = br.ReadInt16();
            ani.clock = br.ReadInt16();
            ani.width = br.ReadInt16();
            ani.height = br.ReadInt16();
            ani.bitsperpixel = br.ReadInt16();
            ani.oneframesize = br.ReadInt32();

            nVersion = ani.version;

            if (ani.version != 2 && ani.version != 3)
            {
                MessageBox.Show(filename, "Version mismatched\nNeed Version.2 or 3");
                bError = true;
                return -2;
            }

            bitmapThis = new Bitmap[ani.frame];

            if (ani.version == 2)
            {
                for (int i = 0; i < ani.frame; i++)
                {
                    //fs.Seek(105 + i * ani.oneframesize, SeekOrigin.Begin); 
                    DIBitmap dib = new DIBitmap();

                    bitmapThis[i] = dib.Read(br);
                }
            }
            else
            {
                for (int i = 0; i < ani.frame; i++)
                {
                    int size = br.ReadInt32();
                    byte[] buffer = br.ReadBytes(size);
                    ushort crc = br.ReadUInt16();

                    if (crc != NetTools.GetCRC.SumWORD(buffer, 0, buffer.Length))
                    {
                        bitmapThis[i] = NewBitmap();
                    }
                    else
                    {
                        EncodeBuffer(ref buffer);
                        MemoryStream ms = new MemoryStream(buffer);
                        try
                        {
                            bitmapThis[i] = (Bitmap)Bitmap.FromStream(ms);
                        }
                        catch
                        {
                            bitmapThis[i] = NewBitmap();
                        }
                    }
                }
            }

            if (ani.rpm < 1 || ani.rpm > 600)
            {
                nRpm = (int)(60.0 / ((ani.clock / 18.0) * ani.frame));
            }
            else
            {
                nRpm = ani.rpm;
            }

            nMaxFrame = ani.frame;
            nWidth = ani.width;
            nHeight = ani.height;
            nBitsPerPixel = ani.bitsperpixel;

            return 1;            
        }

        public async Task<int> LoadFileAnimationAsync(string filename, BinaryReader br)
        {
            return await Task.Run(() =>
            {
                ANIMATION_STRUCT2 ani = new ANIMATION_STRUCT2();
                ani.id = br.ReadBytes(9);
                ani.version = br.ReadInt16();
                ani.description = br.ReadBytes(78);
                ani.rpm = br.ReadInt16();
                ani.frame = br.ReadInt16();
                ani.clock = br.ReadInt16();
                ani.width = br.ReadInt16();
                ani.height = br.ReadInt16();
                ani.bitsperpixel = br.ReadInt16();
                ani.oneframesize = br.ReadInt32();
                nVersion = ani.version;

                if (ani.version != 2 && ani.version != 3)
                { 
                    MessageDisplay.Show(filename, "Version mismatched\nNeed Version.2 or 3");
                    
                    bError = true;
                    return -2;
                }

                bitmapThis = new Bitmap[ani.frame];

                if (ani.version == 2)
                {
                    for (int i = 0; i < ani.frame; i++)
                    {
                        //fs.Seek(105 + i * ani.oneframesize, SeekOrigin.Begin); 
                        DIBitmap dib = new DIBitmap();
                        bitmapThis[i] = dib.Read(br);
                    }
                }
                else
                {
                    for (int i = 0; i < ani.frame; i++)
                    {
                        int size = br.ReadInt32();
                        byte[] buffer = br.ReadBytes(size);
                        ushort crc = br.ReadUInt16();

                        if (crc != NetTools.GetCRC.SumWORD(buffer, 0, buffer.Length))
                        {
                            bitmapThis[i] = NewBitmap();
                        }
                        else
                        {
                            EncodeBuffer(ref buffer);
                            MemoryStream ms = new MemoryStream(buffer);
                            try
                            {
                                bitmapThis[i] = (Bitmap)Bitmap.FromStream(ms);
                            }
                            catch
                            {
                                bitmapThis[i] = NewBitmap();
                            }
                        }
                    }
                }

                if (ani.rpm < 1 || ani.rpm > 600)
                {
                    nRpm = (int)(60.0 / ((ani.clock / 18.0) * ani.frame));
                }
                else
                {
                    nRpm = ani.rpm;
                }

                nMaxFrame = ani.frame;
                nWidth = ani.width;
                nHeight = ani.height;
                nBitsPerPixel = ani.bitsperpixel;
                return 1;
            });
        }

        public async Task<int> LoadImage(string filename)
		{
			string err;
            bError = false;

			if(!File.Exists(filename)) 
			{
				err = String.Format("File not found ({0})", filename);
				SetErrorString(err);
                bError = true;
				return 0;
			}

            sFilename = filename;

			string ext = Path.GetExtension(filename);

            if (String.Compare(ext, ".ANI", true) == 0)
            {
                FileStream fs = null;

                try
                {
                    fs = File.OpenRead(filename);

                    if (fs == null)
                    {
                        MessageDisplay.Show(filename, "ANI Open Error");
                        bError = true;
                        return -2;
                    }

                    BinaryReader br = new BinaryReader(fs);

                    int retn = await LoadFileAnimationAsync(filename, br);
                    return retn;
                }
                catch (Exception ex)
                {
                    MessageDisplay.Show($"ANI 파일 로드 중 오류: {ex.Message}", "Error");
                    bError = true;
                    return -2;
                }
                finally
                {
                    fs?.Close();
                }
            }
            else
            {
                bitmapThis = new Bitmap[1];

                PictureToBitmap load = new PictureToBitmap();

                bitmapThis[0] = load.Load(filename);
                nMaxFrame = 1;

                if (bitmapThis[0] == null)
                {
                    bitmapThis[0] = new Bitmap(200, 200);
                    Graphics g = Graphics.FromImage(bitmapThis[0]);
                    DrawClass.gcls(g, 0, 0, 200, 200, Color.White);
                    Rectangle r = new Rectangle(0, 0, 200, 200);
                    SafeException.SafeDrawString(g, "Error: " + filename, new Font("Gulim", 9), Brushes.Black, r);
                }
                nWidth = bitmapThis[0].Width;
                nHeight = bitmapThis[0].Height;
                nBitsPerPixel = ConvertToBitsPerPixel(bitmapThis[0]);

                return 1;
            }
		}

        public int LoadImageFromStream(MemoryStream stream, string filename)
        {
            if (stream == null)
            {
                string err = String.Format("Stream is null ({0})", filename);
                SetErrorString(err);
                bError = true;
                return 0;
            }

            bError = false;

            sFilename = filename;
            string ext = Path.GetExtension(filename);

            if (String.Compare(ext, ".ANI", true) == 0)
            {
                //ANIMATION_STRUCT2 ani = new ANIMATION_STRUCT2();

                BinaryReader br = new BinaryReader(stream);

                int retn = LoadFileAnimation(filename, br);
                
                return retn;
            }
            else
            {
                bitmapThis = new Bitmap[1];

                PictureToBitmap load = new PictureToBitmap();

                bitmapThis[0] = load.LoadFromStream(filename, stream);
                nMaxFrame = 1;

                if (bitmapThis[0] == null)
                {
                    bitmapThis[0] = new Bitmap(200, 200);
                    Graphics g = Graphics.FromImage(bitmapThis[0]);
                    DrawClass.gcls(g, 0, 0, 200, 200, Color.White);
                    Rectangle r = new Rectangle(0, 0, 200, 200);
                    SafeException.SafeDrawString(g, "Error: " + filename, new Font("Gulim", 9), Brushes.Black, r);
                }
                nWidth = bitmapThis[0].Width;
                nHeight = bitmapThis[0].Height;
                nBitsPerPixel = ConvertToBitsPerPixel(bitmapThis[0]);

                return 1;
            }
        }

		public int  Width()
		{
			return nWidth;
		}

		public int  Height()
		{
			return nHeight;
		}

		public int  GetMaxFrame()
		{
			return nMaxFrame;
		}

		public int  GetRPM()
		{
			return nRpm;
		}

        public Bitmap GetBitmap(int frame)
        {
            if (bitmapThis == null)
            {
                return null;
            }

            if (frame >= nMaxFrame)
                frame = 0;
            if (frame < 0)
                frame = 0;

            return bitmapThis[frame];
        }

        public void SetBitmap(int frame, Bitmap bitmap)
        {
            if (bitmapThis == null)
            {
                return;
            }

            if (frame >= nMaxFrame) return;
            if (frame < 0) return;

            bitmapThis[frame] = bitmap;
        }

		public void Putimage(Graphics g, int x, int y, int width, int height, int frame)
		{
			if(bitmapThis == null) 
			{
                SafeException.SafeDrawString(g, this.sErrorString, new Font("Gulim", 9), Brushes.Black, x, y);
				return;
			}
			
			if(frame >= nMaxFrame)
				frame = 0;
			if(frame < 0)
				frame = 0;

			//if(width == nWidth && height == nHeight)
			//{
			//	g.DrawImageUnscaled(bitmapThis[frame], x, y);   // 비트맵 메모리에 그리는 경우 사이즈가 틀리게 표시된다.
			//}
			//else 
			//{
            if(bitmapThis[frame] != null)
				g.DrawImage(bitmapThis[frame], x, y, width, height);
			//}
		}

		public void PutimageOverlay(Graphics g, int x, int y, int width, int height, int frame)
		{
			if(bitmapThis == null) 
			{
                SafeException.SafeDrawString(g, this.sErrorString, new Font("Gulim", 9), Brushes.Black, x, y);
				return;
			}

			if(frame >= nMaxFrame)
				frame = 0;
			if(frame < 0)
				frame = 0;

			ImageAttributes imageAttr = new ImageAttributes();
			Color overlay_color = bitmapThis[0].GetPixel(0, 0);

			imageAttr.SetColorKey(overlay_color, overlay_color);
				
			Rectangle r = new Rectangle(x, y, width, height);
			
			//g.DrawImage(bitmapThis[frame], r, 0, 0, nWidth, nHeight, GraphicsUnit.Pixel, imageAttr);
            // nWidth nHeight 를 bitmapThis[frame].Width, bitmapThis[frame].Height 로 변경했다.
            g.DrawImage(bitmapThis[frame], r, 0, 0, bitmapThis[frame].Width, bitmapThis[frame].Height, GraphicsUnit.Pixel, imageAttr);
		}

		void SetErrorString(string err)
		{
			sErrorString = err;
		}

        public void Dispose()
        {
            if (bitmapThis != null)
            {
                for (int i = 0; i < bitmapThis.Length; i++)
                {
                    bitmapThis[i].Dispose();
                }
            }
        }

        public Bitmap NewBitmap()
        {
            PixelFormat format;

            if(nBitsPerPixel == 24)
                format = PixelFormat.Format24bppRgb;
            else if (nBitsPerPixel == 8)
                format = PixelFormat.Format8bppIndexed;
            else if (nBitsPerPixel == 4)
                format = PixelFormat.Format4bppIndexed;
            else if (nBitsPerPixel == 1)
                format = PixelFormat.Format1bppIndexed;
            else
                format = PixelFormat.Format32bppArgb;

            Bitmap bitmap = new Bitmap(nWidth, nHeight, format); 
            return bitmap;
        }

        public void Insert(int pos, Bitmap bitmap)
        {
            if (pos > nMaxFrame)
            {
                pos = nMaxFrame;
            }

            Bitmap[] temp = new Bitmap[nMaxFrame + 1];

            int i = 0;

            for (i = 0; i < pos; i++)
            {
                temp[i] = bitmapThis[i];
            }
            temp[i] = bitmap;

            for (; i < nMaxFrame; i++)
            {
                temp[i+1] = bitmapThis[i];
            }

            bitmapThis = temp;

            nMaxFrame++;
        }

        public void Delete(int pos)
        {
            if (pos >= nMaxFrame)   return;   // index over
            if (nMaxFrame <= 0)     return;

            Bitmap[] temp = new Bitmap[nMaxFrame-1];

            int i = 0;

            for (i = 0; i < pos; i++)
            {
                temp[i] = bitmapThis[i];
            }

            for (; i < nMaxFrame-1; i++)
            {
                temp[i] = bitmapThis[i+1];
            }

            bitmapThis = temp;

            nMaxFrame--;

        }

        // Version 3 New Format
        public bool Save(string filename)
        {
            Stream stream = File.Create(filename);
            BinaryWriter writer = new BinaryWriter(stream);

            ANIMATION_STRUCT2 ani = new ANIMATION_STRUCT2();

            ani.version = 3;
            ani.rpm = (short)nRpm;
            ani.frame = (short)nMaxFrame;
            ani.clock = 0;
            ani.width = (short)nWidth;
            ani.height = (short)nHeight;
            ani.bitsperpixel = (short)nBitsPerPixel;
            ani.oneframesize = 0;

            writer.Write(ani.id, 0, 9);
            writer.Write(ani.version);
            writer.Write(ani.description, 0, 78);
            writer.Write(ani.rpm);
            writer.Write(ani.frame);
            writer.Write(ani.clock);
            writer.Write(ani.width);
            writer.Write(ani.height);
            writer.Write(ani.bitsperpixel);
            writer.Write(ani.oneframesize);

            for (int i = 0; i < nMaxFrame; i++)
            {
                MemoryStream ms = new MemoryStream();

                bitmapThis[i].Save(ms, ImageFormat.Png);

                byte[] buffer = ms.ToArray();
                EncodeBuffer(ref buffer);

                writer.Write(buffer.Length);
                writer.Write(buffer);
                writer.Write(NetTools.GetCRC.SumWORD(buffer, 0, buffer.Length));    // CRC도 저장한다.
            }

            writer.Close();
            stream.Close();
            
            return true;
        }

        void EncodeBuffer(ref byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(buffer[i] ^ (i % 256));
            }
        }

        public int GetColor()
        {
            return nBitsPerPixel;
        }

        public void ChangeProperty(int width, int height, int rpm, int color)
        {
            nWidth = width;
            nHeight = height;
            if (rpm < 1) rpm = 1; //10.3.6.22버전까지 gif 60프레임 이상이면 오류발생하여 추가 2024-12-11 PSU
                                  //수정 후 gif의 첫프레임 duration을 읽어와 rpm을 계산하고, 만일 600프레임(장)이 넘어가면 1로 설정.
            nRpm = rpm;
            nBitsPerPixel = color;
        }

        public static void CalcFlipHorz(ref int nRotateFlip)
        {
            nRotateFlip += 4;
            nRotateFlip %= 8;
        }

        public static void CalcFlipVert(ref int nRotateFlip)
        {
            if (nRotateFlip == 0) nRotateFlip = 6;
            else if (nRotateFlip == 1) nRotateFlip = 7;
            else if (nRotateFlip == 2) nRotateFlip = 4;
            else if (nRotateFlip == 3) nRotateFlip = 5;
            else if (nRotateFlip == 4) nRotateFlip = 2;
            else if (nRotateFlip == 5) nRotateFlip = 3;
            else if (nRotateFlip == 6) nRotateFlip = 0;
            else if (nRotateFlip == 7) nRotateFlip = 1;
        }

        public static void CalcRotateRight(ref int nRotateFlip)
        {
            if (nRotateFlip < 4)
            {
                nRotateFlip++;
                nRotateFlip %= 4;
            }
            else
            {
                nRotateFlip+=3;
                nRotateFlip %= 4;
                nRotateFlip += 4;
            }
        }

        public static void CalcRotateLeft(ref int nRotateFlip)
        {
            if (nRotateFlip < 4)
            {
                nRotateFlip+=3;
                nRotateFlip %= 4;
            }
            else
            {
                nRotateFlip += 1;
                nRotateFlip %= 4;
                nRotateFlip += 4;
            }
        }

        public void RotateFlip(int no)
        {
            if (no == 0) return;
            if (no < 0 || no > 7) return;   // 0~7까지 8가지가 존재

            for (int i = 0; i < nMaxFrame; i++)
            {
                bitmapThis[i].RotateFlip((RotateFlipType)no);
            }
        }

        public void FlipHorz()
        {
            for (int i = 0; i < nMaxFrame; i++)
            {
                bitmapThis[i].RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
        }

        public void FlipVert()
        {
            for (int i = 0; i < nMaxFrame; i++)
            {
                bitmapThis[i].RotateFlip(RotateFlipType.RotateNoneFlipY);
            }
        }

        public void RotateRight()
        {
            for (int i = 0; i < nMaxFrame; i++)
            {
                bitmapThis[i].RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
        }

        public void RotateLeft()
        {
            for (int i = 0; i < nMaxFrame; i++)
            {
                bitmapThis[i].RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
        }

        public void MakeWebPublishFile(string mod_dir, string file, int nRotateFlip, bool overlay)
        {
            string target_dir = String.Format("{0}\\WebPublish", mod_dir);

            if (!Directory.Exists(target_dir))
            {
                Directory.CreateDirectory(target_dir);
            }

            string source_file = String.Format("{0}\\{1}", mod_dir, file);

            string target_file;

            bool ani_file = (String.Compare(Path.GetExtension(file), ".ANI", true) == 0);

            if (ani_file)   // 애니메이션 파일일 경우만 해당
            {
                target_file = String.Format("{0}\\WebPublish\\{1}.txt", mod_dir, file);
                TextWriter writer = new StreamWriter(target_file);
                writer.WriteLine("Frame,{0}", nMaxFrame);
                writer.WriteLine("Rpm,{0}", nRpm);
                writer.WriteLine("Size,{0},{1}", nWidth, nHeight);
                for (int i = 0; i < nMaxFrame; i++)
                {
                    target_file = String.Format("{0}_{1:00}.png", file, i);
                    writer.WriteLine("Member,{0}", target_file);
                }
                writer.Close();
            }

            for (int i = 0; i < nMaxFrame; i++)
            {
                Bitmap bitmap = bitmapThis[i];

                if (!overlay)
                {
                    target_file = String.Format("{0}\\WebPublish\\{1}_{2:00}_R{3}.png", mod_dir, file, i, nRotateFlip);
                    bitmap.Save(target_file, System.Drawing.Imaging.ImageFormat.Png);
                }
                else
                {
                    bitmap = new Bitmap(bitmap);    // 그림에 오버레이를 넣기 때문에 새로 만든다.
                    Color c = bitmap.GetPixel(0, 0);
                    bitmap.MakeTransparent(c);
                    target_file = String.Format("{0}\\WebPublish\\{1}_{2:00}_R{3}_ovr.png", mod_dir, file, i, nRotateFlip);
                    bitmap.Save(target_file, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        // 이전 버전인 경우는 새버전으로 저장한다.
        public void SaveAnimationFileToVersion3()
        {
            string ext = Path.GetExtension(sFilename);

            if (nVersion == 2 && String.Compare(ext, ".ani", true) == 0)
            {
                Save(sFilename);
            }
        }
    }
}

