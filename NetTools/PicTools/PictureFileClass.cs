using System;
using System.Windows.Forms;
using System.IO;

namespace PicTools
{
	/// <summary>
	/// Summary description for PictureFileClass.
	/// </summary>
	public class PictureFileClass
	{
		protected byte[] dac = new byte[768];
		protected int nWidth;
		protected int nHeight;
		protected int nBitsPerPixel;
		protected FileStream fs = null;
		protected BinaryReader br;
		protected int  nBytesPerLine;
		protected string sFileName;

		public static readonly byte[] BIT_MASK = { 128, 64, 32, 16, 8, 4, 2, 1 };

		public PictureFileClass()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public int GetColor()
		{
			return nBitsPerPixel;
		}

		public void GetSize(out int width, out int height)
		{
			width = nWidth;
			height = nHeight;
		}

		public void GetDac(ref byte[] d)
		{
			dac.CopyTo(d, 0);
		}

		protected void ReadOpenError(string filename)
		{
			string message;

			if(NetTools.Tools.IsLangKorean()) 
			{
				message = String.Format("{0} 파일을\n읽을수가 없습니다.", filename);
				MessageBox.Show(message, "파일 읽기(열기) 이상");
			}
			else 
			{
				message = String.Format("Cannot read the file {0}.", filename);
				MessageBox.Show(message, "File read error");
			}
		}

		public bool ReadOpen(string filename)
		{
			sFileName = filename;

			if(!File.Exists(filename))	return false;

			fs = File.OpenRead(filename);

			if(fs == null)	
			{
				ReadOpenError(filename);
				return false;
			}

			br = new BinaryReader(fs);

			if(!ReadOpenLocal()) 
			{
				fs.Close();
				return false;
			}

			return true;
		}

        public bool ReadOpen(MemoryStream stream)
        {
            fs = null;

            br = new BinaryReader(stream);

            if (!ReadOpenLocal())
            {
                return false;
            }

            return true;
        }

		protected virtual bool ReadOpenLocal()
		{
			return false;
		}

        public void ReadClose()
        {
            if(fs != null)
                fs.Close();
        }
	}
}
