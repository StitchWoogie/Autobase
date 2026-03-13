using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace SilverlightGraphicModule
{
    [Flags]
    public enum EnumWindowStyleFlags : uint
    {
        WS_OVERLAPPED = 0x00000000,
        WS_POPUP = 0x80000000,
        WS_CHILD = 0x40000000,
        WS_MINIMIZE = 0x20000000,
        WS_VISIBLE = 0x10000000,
        WS_DISABLED = 0x08000000,
        WS_CLIPSIBLINGS = 0x04000000,
        WS_CLIPCHILDREN = 0x02000000,
        WS_MAXIMIZE = 0x01000000,
        WS_CAPTION = 0x00C00000,     /* WS_BORDER | WS_DLGFRAME  */
        WS_BORDER = 0x00800000,
        WS_DLGFRAME = 0x00400000,
        WS_VSCROLL = 0x00200000,
        WS_HSCROLL = 0x00100000,
        WS_SYSMENU = 0x00080000,
        WS_THICKFRAME = 0x00040000,
        WS_GROUP = 0x00020000,
        WS_TABSTOP = 0x00010000,

        WS_MINIMIZEBOX = 0x00020000,
        WS_MAXIMIZEBOX = 0x00010000,


        //WS_TILED            WS_OVERLAPPED
        //WS_ICONIC           WS_MINIMIZE
        WS_SIZEBOX = 0x00040000,
        //WS_TILEDWINDOW      WS_OVERLAPPEDWINDOW
        WS_FULL = 0xFFFFFFFF,

        CBS_SIMPLE = 0x00000001,
        CBS_DROPDOWN = 0x00000002,
        CBS_DROPDOWNLIST = 0x00000003,
        CBS_OWNERDRAWFIXED = 0x00000010,
        CBS_OWNERDRAWVARIABLE = 0x00000020,
        CBS_AUTOHSCROLL = 0x00000040,
        CBS_OEMCONVERT = 0x00000080,
        CBS_SORT = 0x00000100,
        CBS_HASSTRINGS = 0x00000200,
        CBS_NOINTEGRALHEIGHT = 0x00000400,
        CBS_DISABLENOSCROLL = 0x00000800,
        CBS_UPPERCASE = 0x00002000,
        CBS_LOWERCASE = 0x00004000,

        LBS_NOTIFY = 0x00000001,
        LBS_SORT = 0x00000002,
        LBS_NOREDRAW = 0x00000004,
        LBS_MULTIPLESEL = 0x00000008,
        LBS_OWNERDRAWFIXED = 0x00000010,
        LBS_OWNERDRAWVARIABLE = 0x00000020,
        LBS_HASSTRINGS = 0x00000040,
        LBS_USETABSTOPS = 0x00000080,
        LBS_NOINTEGRALHEIGHT = 0x00000100,
        LBS_MULTICOLUMN = 0x00000200,
        LBS_WANTKEYBOARDINPUT = 0x00000400,
        LBS_EXTENDEDSEL = 0x00000800,
        LBS_DISABLENOSCROLL = 0x00001000,
        LBS_NODATA = 0x00002000,
        LBS_NOSEL = 0x00004000,
        LBS_STANDARD = (LBS_NOTIFY | LBS_SORT | WS_VSCROLL | WS_BORDER),

        ES_LEFT = 0x00000000,
        ES_CENTER = 0x00000001,
        ES_RIGHT = 0x00000002,
        ES_MULTILINE = 0x00000004,
        ES_UPPERCASE = 0x00000008,
        ES_LOWERCASE = 0x00000010,
        ES_PASSWORD = 0x00000020,
        ES_AUTOVSCROLL = 0x00000040,
        ES_AUTOHSCROLL = 0x00000080,
        ES_NOHIDESEL = 0x00000100,
        ES_OEMCONVERT = 0x00000400,
        ES_READONLY = 0x00000800,
        ES_WANTRETURN = 0x00001000,
        ES_NUMBER = 0x00002000,
    }

    
    /// <summary>
    /// Summary description for GraphicTool.
    /// </summary>
    public class GraphicTool
    {
        /*
        public static bool bLoadOnLibrary = false;
        public static string sLoadOnLibraryDir;

        public GraphicTool()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public enum EnumModuleWindowStyle
        {
            MDI,
            POPUP,
        }

        static int GetModuleProperty(string filename, ref int module_sizex, ref int module_sizey, ref EnumModuleWindowStyle module_style, ref EnumWindowStyleFlags module_style_flag, ref int location_method, ref int location_x, ref int location_y, ref bool popup_dialog, ref int opacity)
        {
            FileStream fs;
            string str = "";
            string path;
            CommaBlockString comma = new CommaBlockString();
            bool flag_resolution = false;
            bool flag_style = false;
            bool flag_style_flag = false;
            bool flag_default_location = false;
            string imsi = "";

            module_sizex = 100;
            module_sizey = 100;
            module_style = EnumModuleWindowStyle.MDI;
            module_style_flag = EnumWindowStyleFlags.WS_FULL;
            location_method = 0;
            location_x = 0;
            location_y = 0;
            opacity = 100;
            popup_dialog = false;

            //MakeFilePath mfp = new MakeFilePath(TotalConfig.sDirWorkProject);

            //path = mfp.Graphic(filename);
            path = filename;

            if (!File.Exists(path)) return 0;

            fs = File.OpenRead(path);

            if (fs == null) return 0;

            while (true)
            {
                if (flag_resolution &&
                flag_style &&
                flag_style_flag &&
                flag_default_location) break;	// 필요한 특성은 읽었으므로 시간을 절약하기 위해 return;

                if (!Tools.TextGetOneLine(fs, ref str)) break;

                if (str.Length == 0) continue;
                if (str[0] == '[') continue;

                comma.Set(str);
                comma.GetString(ref imsi);

                if (imsi == "Resolution")
                {
                    flag_resolution = true;
                    comma.GetInt(ref module_sizex);
                    comma.GetInt(ref module_sizey);
                }
                else if (imsi == "DefaultLocation")
                {
                    flag_default_location = true;
                    comma.GetInt(ref location_method);
                    comma.GetInt(ref location_x);
                    comma.GetInt(ref location_y);
                }
                else if (imsi == "ModuleWindowStyle")
                {
                    flag_style = true;
                    int style = 0;
                    comma.GetInt(ref style);
                    module_style = (EnumModuleWindowStyle)style;
                    comma.GetBool(ref popup_dialog);
                    comma.GetInt(ref opacity);
                    if (opacity < 1 || opacity > 100) opacity = 100;
                }
                else if (imsi == "ModuleWindowStyleFlag")
                {
                    flag_style_flag = true;
                    uint flags = 0;
                    comma.GetDWORD(ref flags);
                    module_style_flag = (EnumWindowStyleFlags)flags;
                }
                else if (imsi == "Group")
                {
                    break;
                }
                else { }
            }

            fs.Close();

            // 이전 버전의 POPUP윈도우 일때는 크기를 조금 바꾸어 주어야 한다.
            string ext = Path.GetExtension(filename);
            if (String.Compare(ext, ".mod", true) == 0)
            {
                if (module_style == EnumModuleWindowStyle.POPUP)
                {
                    // caption이 없을때만 크기를 조절한다.
                    if ((module_style_flag & EnumWindowStyleFlags.WS_CAPTION) == 0)
                    {
                        module_sizex += 8;
                        module_sizey += 34;
                    }
                }
            }

            return 1;
        }

        // 다중모니터일 때는 전체 화면의 크기를 구한다.
        public static void GetMultiScreenSize(out int x1, out int y1, out int x2, out int y2)
        {
            Rectangle r = Screen.PrimaryScreen.Bounds;

            x1 = 0;//GetSystemMetrics(SM_XVIRTUALSCREEN);
            y1 = 0;//GetSystemMetrics(SM_YVIRTUALSCREEN);
            x2 = r.Width;//GetSystemMetrics(SM_CXVIRTUALSCREEN);
            y2 = r.Height;//GetSystemMetrics(SM_CYVIRTUALSCREEN);

            // 다중모니터일 때는 전체 화면의 크기를 구한다.
            foreach (Screen screen in Screen.AllScreens)
            {
                int w = screen.Bounds.Left + screen.Bounds.Width;
                if (w > x2) x2 = w;
                int h = screen.Bounds.Top + screen.Bounds.Height;
                if (h > y2) y2 = h;

                if (screen.Bounds.Left < x1) x1 = screen.Bounds.Left;
                if (screen.Bounds.Top < y1) y1 = screen.Bounds.Top;
            }
        }

        static void MakeGraphicPopupWindow(string filename, int sizex, int sizey, EnumWindowStyleFlags style, int location_method, int posx, int posy, bool popup_dialog, int opacity)
        {
            FormGraphicFrame ex = new GraphicModule.FormGraphicFrame(filename);

            ex.MinimizeBox = false;
            ex.MaximizeBox = false;

            ex.WindowState = FormWindowState.Normal;	// windowstatus가 maximized되어 있을 때는 크기조절이 안된다.

            if ((style & EnumWindowStyleFlags.WS_SYSMENU) == EnumWindowStyleFlags.WS_SYSMENU)
            {
                ex.FormBorderStyle = FormBorderStyle.FixedDialog;
                ex.ControlBox = true;
            }
            else
                ex.ControlBox = false;

            if ((style & EnumWindowStyleFlags.WS_CAPTION) == EnumWindowStyleFlags.WS_CAPTION ||
                (style & EnumWindowStyleFlags.WS_SYSMENU) == EnumWindowStyleFlags.WS_SYSMENU)
            {
                ex.FormBorderStyle = FormBorderStyle.FixedDialog;
            }
            else
            {
                ex.FormBorderStyle = FormBorderStyle.None;
            }

            //if((style & EnumWindowStyleFlags.WS_BORDER) == EnumWindowStyleFlags.WS_BORDER)
            //	ex.FormBorderStyle = FormBorderStyle.FixedDialog;
            //else
            //	ex.FormBorderStyle = FormBorderStyle.None;

            // 아래 세줄이 빠져도 정확한 윈도우 계산이 안된다.
            ex.StartPosition = FormStartPosition.Manual;
            ex.Left = 100;	// 작은 숫자를 주면 정확한 윈도우의 크기가 안나온다.(원인:모름)
            ex.Top = 100;

            Rectangle r = ex.ClientRectangle;

            sizex += (ex.Width - r.Width);
            sizey += (ex.Height - r.Height);

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

            if (location_method == 0)
            {
                startx = window_x / 2 - sizex / 2 + posx;
                starty = window_y / 2 - sizey / 2 + posy;
            }
            else if (location_method == 1)
            {
                startx = posx;
                starty = posy;
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
            }

            if (startx < virtual_x1) startx = virtual_x1;
            if (starty < virtual_y1) starty = virtual_y1;
            if (startx >= virtual_x2) startx = virtual_x2 - 50;
            if (starty >= virtual_y2) starty = virtual_y2 - 50;

            ex.StartPosition = FormStartPosition.Manual;

            ex.Left = startx;
            ex.Top = starty;

            ex.Width = sizex;
            ex.Height = sizey;

            ex.ShowInTaskbar = false;
            ex.Opacity = opacity / 100.0;

            if (popup_dialog) ex.ShowDialog();
            else
            {
                ex.Owner = TotalConfig.formMain;
                ex.Show();
            }
        }

        static void MakeMdiChildGraphicWindow(string filename, int pos_method, int posx, int posy,
            int module_sizex, int module_sizey, EnumModuleWindowStyle module_style, EnumWindowStyleFlags module_style_flag,
            int module_location_method, int module_posx, int module_posy, bool popup_dialog, int opacity)
        {
            if (module_style == EnumModuleWindowStyle.POPUP)
            {
                if (pos_method == -1)
                {
                    MakeGraphicPopupWindow(filename, module_sizex, module_sizey, module_style_flag,
                        module_location_method, module_posx, module_posy, popup_dialog, opacity);
                }
                else
                {
                    MakeGraphicPopupWindow(filename, module_sizex, module_sizey, module_style_flag,
                        pos_method, posx, posy, popup_dialog, opacity);
                }
                return;
            }

            FormGraphicFrame ex = new GraphicModule.FormGraphicFrame(filename);

            // mdi가 하나도 없을 때는 최대화 시킨다.
            if (TotalConfig.formMain.MdiChildren.Length == 0)
                ex.WindowState = FormWindowState.Maximized;

            ex.MdiParent = TotalConfig.formMain;

            ex.Show();
        }

        static ArrayList historyModule = new ArrayList();	// 그래픽 모듈을 연 정보
        static bool bPop = false;

        class HistoryModuleItem
        {
            public string filename;
            public int pos_method;
            public int posx;
            public int posy;
        }

        public static void Pop()
        {
            if (historyModule.Count <= 1) return;

            int pos = historyModule.Count - 1;

            historyModule.RemoveAt(pos);

            pos = historyModule.Count - 1;
            HistoryModuleItem item = (HistoryModuleItem)historyModule[pos];
            bPop = true;
            RestoreGraphicWindow(item.filename, item.pos_method, item.posx, item.posy);
        }

        static public void RestoreGraphicWindow(string filename, int pos_method, int posx, int posy)
        {
            if (!bPop)
            {
                HistoryModuleItem item = new HistoryModuleItem();

                item.filename = filename;
                item.pos_method = pos_method;
                item.posx = posx;
                item.posy = posy;
                historyModule.Add(item);
                if (historyModule.Count > 100)
                {
                    historyModule.RemoveAt(0);
                }
            }
            bPop = false;

            int count = FormGraphicFrame.arrayFormGraphFrame.Count;
            int i;
            GraphicModule.FormGraphicFrame form;

            for (i = 0; i < count; i++)
            {
                form = (GraphicModule.FormGraphicFrame)FormGraphicFrame.arrayFormGraphFrame[i];
                if (String.Compare(form.formChild.sFileName, filename, true) == 0)
                {
                    //sharedData.formMain.ActiveMdiChild = form;
                    //form.set
                    //form.Activate();
                    //sharedData.formMain.ActiveMdiChild = form;
                    form.Focus();

                    if (form.WindowState == FormWindowState.Minimized)
                        form.WindowState = FormWindowState.Normal;

                    return;
                }
            }

            int module_sizex = 0;
            int module_sizey = 0;
            EnumModuleWindowStyle module_style = 0;
            EnumWindowStyleFlags module_style_flag = 0;
            int module_location_method = 0;
            int module_posx = 0;
            int module_posy = 0;
            int opacity = 0;
            bool popup_dialog = false;

            GetModuleProperty(filename, ref module_sizex, ref module_sizey, ref module_style, ref module_style_flag,
                ref module_location_method, ref module_posx, ref module_posy, ref popup_dialog, ref opacity);

            // mdi창을 만들때만 창제한을 검사한다.
            if (module_style == EnumModuleWindowStyle.MDI)
            {
                // 같은 이름의 그래픽 모듈 파일을 찾지 못했다.
                // MDI개수가 제한 갯수를 넘으면 처음 MDI를 닫는다.
                int mdi_count = 0;
                GraphicModule.FormGraphicFrame formMdi = null;

                for (i = 0; i < count; i++)
                {
                    form = (GraphicModule.FormGraphicFrame)FormGraphicFrame.arrayFormGraphFrame[i];
                    if (form.MdiParent != null)
                    {
                        if (form.formChild.objectGraphic.bNotUsedMdiLimit == 0 && form.bCloseCommand == false) // 창제한을 사용하는 윈도우이고 지금 닫는중이 아닌 윈도우만 
                        {
                            if (formMdi == null)
                                formMdi = form;
                        }

                        mdi_count++;
                    }
                }

                bool bClose = false;

                // 창 제한 개수에 걸린다.
                if (mdi_count >= ConfigViewMain.nMdiCountOnGraphic)
                {
                    bClose = true;
                }

                MakeMdiChildGraphicWindow(filename, pos_method, posx, posy, module_sizex, module_sizey, module_style, module_style_flag,
                    module_location_method, module_posx, module_posy, popup_dialog, opacity);

                // 창 제한 개수에 걸린다.
                // 위에서 새 MDI를 만들고 난 후 닫아야 한다. 그렇지 않으면 부하가 걸리는 그래픽의 경우 MDI 최대, 최소 닫기가 이중으로 생기기도 한다.
                if (bClose)
                {
                    // 첫번째 MDI 모듈을 닫는다.
                    if (formMdi != null)
                    {
                        formMdi.bCloseCommand = true;
                    }
                }
            }
            else
            {
                MakeMdiChildGraphicWindow(filename, pos_method, posx, posy, module_sizex, module_sizey, module_style, module_style_flag,
                    module_location_method, module_posx, module_posy, popup_dialog, opacity);
            }
        }*/

        static List<Page> arrayFormGraphFrame = new List<Page>();

        public static void RegisterPage(Page page)
        {
            arrayFormGraphFrame.Add(page);
        }

        public static void UnRegisterPage(Page page)
        {
            arrayFormGraphFrame.Remove(page);
        }

        static public object ExecuteClassNameOnlyObject(string classname, string command, params object[] args)
        {
            
            int count = arrayFormGraphFrame.Count;
            int i;
            Page form;

            object retn = 0;

            for (i = 0; i < count; i++)
            {
                form = arrayFormGraphFrame[i];

                retn = form.obj.ExecuteClassNameOnlyObject(form, classname, command, args);
            }

            //retn = ToolBarModule.ExecuteClassNameOnlyObject(classname, command, args);

            return retn;
        }

        public static void OnTagFileLoaded()
        {
            int count = arrayFormGraphFrame.Count;
            int i;
            Page form;

            object retn = 0;

            for (i = 0; i < count; i++)
            {
                form = arrayFormGraphFrame[i];

                form.obj.OnTagFileReaded();
            }
        }

    }
}
