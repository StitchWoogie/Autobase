using System;
using System.Windows.Forms;
using System.Drawing;

namespace NetTools
{
	/// <summary>
	/// Summary description for OnPaintBitmap.
	/// </summary>
	public class OnPaintBitmap
	{
		public OnPaintBitmap()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static Bitmap bitmapClient = null;
		//static Graphics graphic = null;

		public static Graphics CreateGraphics(Graphics gScreen, int width, int height)
		{
			if(bitmapClient == null || width > bitmapClient.Width || height > bitmapClient.Height) 
			{
				bitmapClient = new Bitmap(width,height,gScreen);
				
				GC.Collect();
			}

			return Graphics.FromImage(bitmapClient);
			
			//return graphic;
		}

		public static void DrawImageUnscaled(Graphics gScreen, int x, int y, int width, int height)
		{
			gScreen.DrawImageUnscaled(bitmapClient, x, y, width, height);
		}
	}
}
