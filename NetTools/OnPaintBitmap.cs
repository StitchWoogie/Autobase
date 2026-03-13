using System;
using System.Windows.Forms;
using System.Drawing;

namespace NetTools
{
    /// <summary>
    /// 이 클래스 보다는 form의 load에 
    /// this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
    /// this.UpdateStyles(); 
    /// 를 사용하는게 좋을것 같다. 
    /// </summary>
	public class OnPaintBitmap
	{
		public OnPaintBitmap()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        /*
		static Bitmap bitmapClient = null;

		public static Graphics CreateGraphics(Graphics gScreen, int width, int height)
		{
			if(bitmapClient == null || width > bitmapClient.Width || height > bitmapClient.Height) 
			{
				bitmapClient = new Bitmap(width,height,gScreen);
				
				GC.Collect();
			}

			return Graphics.FromImage(bitmapClient);
		}

		public static void DrawImageUnscaled(Graphics gScreen, int x, int y, int width, int height)
		{
			gScreen.DrawImageUnscaled(bitmapClient, x, y, width, height);
		}*/


        Bitmap bitmapClient = null;

        public Graphics CreateGraphics(Graphics gScreen, int width, int height)
        {
            if (bitmapClient == null || width > bitmapClient.Width || height > bitmapClient.Height)
            {
                bitmapClient = new Bitmap(width, height, gScreen);

                GC.Collect();
            }

            return Graphics.FromImage(bitmapClient);
        }

        public void DrawImageUnscaled(Graphics gScreen, int x, int y, int width, int height)
        {
            gScreen.DrawImageUnscaled(bitmapClient, x, y, width, height);
        }
	}
}
