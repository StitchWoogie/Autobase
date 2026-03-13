using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NetTools
{
    // Exception 이 발생하는 항목을 차단하는 부분
    public class SafeException
    {
        // 2015-6-25 함수 추가
        // Windows 7에서 2015-6-25 약 두달전 부터 생긴현상 
        // 폰트가 70정도 이상되면 Exception이 발생한다. 한글을 사용할 때 나타나며
        // 영문 Arial 같은 영문폰트에서 영문을 사용하면 나타나지 않는다. 영문폰트에서 한글을 사용해도 같은현상이 발생한다.

        public static void SafeDrawString(Graphics g, string s, Font font, Brush brush, float x, float y)
        {
            try
            {
                g.DrawString(s, font, brush, x, y);
            }
            catch (Exception ex) // GDI+ 일반 오류 발생
            {
                try
                {
                    // 폰트를 작게 시도
                    Font font_new = new Font(font.Name, 10, font.Style);
                    g.DrawString(s, font_new, brush, x, y);
                }
                catch
                {
                    // 두번째는 포기한다.
                }
            }
        }

        public static void SafeDrawString(Graphics g, string s, Font font, Brush brush, float x, float y, StringFormat format)
        {
            try
            {
                g.DrawString(s, font, brush, x, y, format);
            }
            catch // GDI+ 일반 오류 발생
            {
                try
                {
                    // 폰트를 작게 시도
                    Font font_new = new Font(font.Name, 10, font.Style);
                    g.DrawString(s, font_new, brush, x, y, format);
                }
                catch
                {
                    // 두번째는 포기한다.
                }
            }
        }

        public static void SafeDrawString(Graphics g, string s, Font font, Brush brush, RectangleF r, StringFormat format)
        {
            try
            {
                g.DrawString(s, font, brush, r, format);
            }
            catch // GDI+ 일반 오류 발생
            {
                try
                {
                    // 폰트를 작게 시도
                    Font font_new = new Font(font.Name, 10, font.Style);
                    g.DrawString(s, font_new, brush, r, format);
                }
                catch
                {
                    // 두번째는 포기한다.
                }
            }
        }

        public static void SafeDrawString(Graphics g, string s, Font font, Brush brush, RectangleF r)
        {
            try
            {
                g.DrawString(s, font, brush, r);
            }
            catch // GDI+ 일반 오류 발생
            {
                try
                {
                    // 폰트를 작게 시도
                    Font font_new = new Font(font.Name, 10, font.Style);
                    g.DrawString(s, font_new, brush, r);
                }
                catch
                {
                    // 두번째는 포기한다.
                }
            }
        }

        public static void SafeDrawString(Graphics g, string s, Font font, Brush brush, PointF p)
        {
            try
            {
                g.DrawString(s, font, brush, p);
            }
            catch // GDI+ 일반 오류 발생
            {
                try
                {
                    // 폰트를 작게 시도
                    Font font_new = new Font(font.Name, 10, font.Style);
                    g.DrawString(s, font_new, brush, p);
                }
                catch
                {
                    // 두번째는 포기한다.
                }
            }
        }

    }
}
