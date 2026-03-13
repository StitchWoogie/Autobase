using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using NetTools;
using System.Threading.Tasks;

namespace PicTools
{
	/// <summary>
	/// Summary description for PictureToBitmap.
	/// </summary>
	public class PictureToBitmap
	{
		public PictureToBitmap()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        public async Task<Bitmap> LoadAsync(string filename)
        {
            return await Task.Run(() => Load(filename));
        }


        public Bitmap Load(string filename)
		{
			if(!File.Exists(filename))
			{
				if(Tools.IsLangKorean())
					MessageBox.Show(filename, "파일이 없습니다.");
				else 
					MessageBox.Show(filename, "File not exist.");

				return null;
			}
			string ext = Path.GetExtension(filename);

			if(String.Compare(ext, ".PCX", true) == 0) 
			{
				PcxToBitmap tob = new PcxToBitmap();
				return tob.Load(filename);
			}
			else if(String.Compare(ext, ".SPT", true) == 0) 
			{
				SptToBitmap tob = new SptToBitmap();
				return tob.Load(filename);
			}
			else 
			{
				Bitmap bitmap;

                try
                {
                    bitmap = new Bitmap(filename);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message+"\nfilename="+filename, "Bitmap Error");
                    return null;
                }

				Bitmap bitmap2 = new Bitmap(bitmap);
				bitmap.Dispose();
				return bitmap2;
				// return new Bitmap(filename);	// 그림 편집기에서 그림을 수정하면 저장을 할 수 없다.
			}
		}

        public async Task<Bitmap> LoadFromStreamAsync(string filename, MemoryStream stream)
        {
            return await Task.Run(() => LoadFromStream(filename, stream));
        }

        public Bitmap LoadFromStream(string filename, MemoryStream stream)
        {
            if (stream == null) return null;

            string ext = Path.GetExtension(filename);

            if (String.Compare(ext, ".PCX", true) == 0)
            {
                PcxToBitmap tob = new PcxToBitmap();
                return tob.Load(stream);
            }
            else if (String.Compare(ext, ".SPT", true) == 0)
            {
                SptToBitmap tob = new SptToBitmap();
                return tob.Load(stream);
            }
            else
            {
                Bitmap bitmap;

                try
                {
                    bitmap = new Bitmap(stream);
                }
                catch
                {
                    bitmap = null;
                }
                
                return bitmap;
            }

        }
	}
}
