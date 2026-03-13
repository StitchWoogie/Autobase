using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace NetTools
{
	/// <summary>
	/// Summary description for ScreenCapture.
	/// </summary>
	public class ScreenCapture
	{
		public ScreenCapture()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		[System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
		private static extern IntPtr  CreateDC(
			string lpszDriver,        // driver name
			string lpszDevice,        // device name
			string lpszOutput,        // not used; should be NULL
			IntPtr lpInitData	// optional printer data
			);

		[System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
		private static extern bool BitBlt(
			IntPtr hdcDest, // handle to destination DC
			int nXDest,  // x-coord of destination upper-left corner
			int nYDest,  // y-coord of destination upper-left corner
			int nWidth,  // width of destination rectangle
			int nHeight, // height of destination rectangle
			IntPtr hdcSrc,  // handle to source DC
			int nXSrc,   // x-coordinate of source upper-left corner
			int nYSrc,   // y-coordinate of source upper-left corner
			System.Int32 dwRop  // raster operation code
			);

        /// <summary>
        /// CaptureZone을 사용해도 되지만 이전 버전에서 계속 사용했으므로 수정하지 않고 사용한다.
        /// </summary>
        /// <returns></returns>
		public static Bitmap Capture()
		{
            int width, height;

            GetScreenSize(out width, out height);   // Scale을 적용해야 실제 스크린사이즈가 나온다. 2016-2-4

            //double sx, sy;
            //getScalingFactor(out sx, out sy);            
            //string msg = String.Format("Width={0},Height={1},ww={2},wh={3},sx={4},sy={5}", Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, width, height, sx, sy);
            //MessageBox.Show(msg);

			IntPtr dc1 = CreateDC("DISPLAY", null, null, (IntPtr)null);
			Graphics g1 = Graphics.FromHdc(dc1);
            Bitmap MyImage = new Bitmap(width, height, g1);
			Graphics g2 = Graphics.FromImage(MyImage);

			dc1 = g1.GetHdc();
			IntPtr dc2 = g2.GetHdc();
            BitBlt(dc2, 0, 0, width, height, dc1, 0, 0, 13369376);
			g1.ReleaseHdc(dc1);
			g2.ReleaseHdc(dc2);

			return MyImage;
		}

        /// <summary>
        /// 2012-3-28 지원, 다중 모니터에서 다른 모니터를 캡춰할 수 없으므로 새로 지원했다.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap CaptureZone(int x, int y, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);

            Graphics g = Graphics.FromImage(bitmap);
            Size size = new Size(width, height);
            g.CopyFromScreen(x, y, 0, 0, size);

            return bitmap;
        }


        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
        }

        /// <summary>
        /// Screen.PrimaryScreen.Bounds 는 윈도우즈 설정에서 100%, 125%, 150% 설정을하면 다르게 나타난다. *Scale을 적용하면 실제 해상도가 나온다.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void GetScreenSize(out int x, out int y)
        {
            double sx, sy;
            getScalingFactor(out sx, out sy);
            Rectangle r = Screen.PrimaryScreen.Bounds;

            x = (int)(r.Width*sx);
            y = (int)(r.Height * sy);
        }

        private static void getScalingFactor(out double sx, out double sy)
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);
            g.ReleaseHdc(desktop);

            sy = (double)PhysicalScreenHeight / (double)LogicalScreenHeight;
            sx = sy;
            
        }
        
	}
}
