using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace NetTools
{
    public class FormSizeRestore
    {
        string sName;
        string sAppName;

        public FormSizeRestore(string name)
        {
            sName = name;
            sAppName = String.Format("FormSize\\{0}", Application.ProductName);
        }

        public void Save(Form form)
        {
            RegistryTool.SaveConfig(sAppName, sName, "X", form.Left);
            RegistryTool.SaveConfig(sAppName, sName, "Y", form.Top);
            RegistryTool.SaveConfig(sAppName, sName, "Width", form.Width);
            RegistryTool.SaveConfig(sAppName, sName, "Height", form.Height);
        }

        public void Load(Form form)
        {
            int posx = RegistryTool.LoadConfig(sAppName, sName, "X", 50);
            int posy = RegistryTool.LoadConfig(sAppName, sName, "Y", 50);
            int sizex = RegistryTool.LoadConfig(sAppName, sName, "Width", 800);
            int sizey = RegistryTool.LoadConfig(sAppName, sName, "Height", 600);

            // 아래 세줄이 빠져도 정확한 윈도우 계산이 안된다.
            form.StartPosition = FormStartPosition.Manual;
            form.Left = 100;	// 작은 숫자를 주면 정확한 윈도우의 크기가 안나온다.(원인:모름)
            form.Top = 100;

            Rectangle r = form.ClientRectangle;

            //sizex += (form.Width - r.Width);
            //sizey += (form.Height - r.Height);

            r = Screen.PrimaryScreen.Bounds;

            int window_x = r.Width;//GetSystemMetrics(SM_CXSCREEN);
            int window_y = r.Height;//GetSystemMetrics(SM_CYSCREEN);
            int virtual_x1 = 0;//GetSystemMetrics(SM_XVIRTUALSCREEN);
            int virtual_y1 = 0;//GetSystemMetrics(SM_YVIRTUALSCREEN);
            int virtual_x2 = r.Width;//GetSystemMetrics(SM_CXVIRTUALSCREEN);
            int virtual_y2 = r.Height;//GetSystemMetrics(SM_CYVIRTUALSCREEN);

            // 다중모니터일 때는 전체 화면의 크기를 구한다.
            foreach (Screen screen in Screen.AllScreens)
            {
                int w = screen.Bounds.Left + screen.Bounds.Width;
                if (w > virtual_x2) virtual_x2 = w;
                int h = screen.Bounds.Top + screen.Bounds.Height;
                if (h > virtual_y2) virtual_y2 = h;

                if (screen.Bounds.Left < virtual_x1) virtual_x1 = screen.Bounds.Left;
                if (screen.Bounds.Top < virtual_y1) virtual_y1 = screen.Bounds.Top;
            }

            int startx, starty;

            /*
            if (location_method == 0)
            {
                startx = window_x / 2 - sizex / 2 + posx;
                starty = window_y / 2 - sizey / 2 + posy;
            }
            else if (location_method == 1)
            {*/
            startx = posx;
            starty = posy;
            /*
            }
            else if (location_method == 2)
            {
                startx = window_x - sizex - posx;
                starty = posy;
            }
            else if (location_method == 3)
            {
                startx = posx;
                starty = window_y - sizey - posy;
            }
            else if (location_method == 4)
            {
                startx = window_x - sizex - posx;
                starty = window_y - sizey - posy;
            }
            else
            {
                startx = window_x - sizex / 2;
                starty = window_y - sizey / 2;
            }*/

            if (startx < virtual_x1) startx = virtual_x1;
            if (starty < virtual_y1) starty = virtual_y1;
            if (startx >= virtual_x2 - 100) startx = virtual_x2 - 100;
            if (starty >= virtual_y2 - 100) starty = virtual_y2 - 100;

            form.StartPosition = FormStartPosition.Manual;

            form.Left = startx;
            form.Top = starty;

            form.Width = sizex;
            form.Height = sizey;


            //form.Location = new System.Drawing.Point(x, y);
            //form.Size = new System.Drawing.Size(w, h);
        }
    }
}
