using System;
using AutoLib;
using System.Windows.Forms;
using System.IO;
using NetTools;
using System.Drawing;
using AutoLibLocal;
using System.Collections;
using System.Runtime.InteropServices;

namespace GraphicModule
{

    /// <summary>
    /// Summary description for GraphicTool.
    /// </summary>
    public class GraphicTool
    {
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

        static bool HashBufferRuntime(byte[] buffer, int index, int size)
        {
            int[] rand_hash = new int[17] { 0x78, 0x31, 0x91, 0x78, 0xD7, 0x63, 0xB7, 0x19, 0x7F, 0xB2, 0xCA, 0x8E, 0x23, 0xA8, 0x6F, 0x78, 0x71 };

            for (int i = 0, pos = index; i < size; i++, pos++)
            {
                buffer[pos] = (byte)(buffer[pos] ^ (i % 256) ^ rand_hash[i % 17]);
            }

            return true;
        }

        //static void GetPropertyFromTextReader(Form form, TextReader reader, ref int module_sizex, ref int module_sizey, ref EnumModuleWindowStyle module_style, ref EnumWindowStyleFlags module_style_flag, ref int location_method, ref int location_x, ref int location_y, ref bool popup_dialog, ref int opacity, ref bool alwaysontop)
        //{
        //    string str = "";
        //    CommaBlockString comma = new CommaBlockString();
        //    string imsi = "";

        //    bool flag_resolution = false;
        //    bool flag_style = false;
        //    bool flag_style_flag = false;
        //    bool flag_default_location = false;

        //    while (true)
        //    {
        //        if (flag_resolution &&
        //        flag_style &&
        //        flag_style_flag &&
        //        flag_default_location) break;		// 필요한 특성은 읽었으므로 시간을 절약하기 위해 return;

        //        str = reader.ReadLine();

        //        if (str == null) break;

        //        if (str.Length == 0) continue;
        //        if (str[0] == '[') continue;

        //        comma.Set(str);
        //        comma.GetString(ref imsi);

        //        if (imsi == "Resolution")
        //        {
        //            flag_resolution = true;
        //            comma.GetInt(ref module_sizex);
        //            comma.GetInt(ref module_sizey);
        //        }
        //        else if (imsi == "DefaultLocation")
        //        {
        //            flag_default_location = true;
        //            comma.GetInt(ref location_method);
        //            comma.GetInt(ref location_x);
        //            comma.GetInt(ref location_y);
        //        }
        //        else if (imsi == "ModuleWindowStyle")
        //        {
        //            flag_style = true;
        //            int style = 0;
        //            comma.GetInt(ref style);
        //            module_style = (EnumModuleWindowStyle)style;
        //            comma.GetBool(ref popup_dialog);
        //            comma.GetInt(ref opacity);
        //            if (opacity < 1 || opacity > 100) opacity = 100;

        //            comma.Skip();   // Buf.GetChar(ref cSmoothingMode);
        //            comma.GetBool(ref alwaysontop);
        //        }
        //        else if (imsi == "ModuleWindowStyleFlag")
        //        {
        //            flag_style_flag = true;
        //            uint flags = 0;
        //            comma.GetDWORD(ref flags);
        //            module_style_flag = (EnumWindowStyleFlags)flags;
        //        }
        //        else if (imsi == "Group")
        //        {
        //            break;
        //        }
        //        else { }
        //    }
        //}


        //monitorIndex 추가 20241010 PSU
        //static void GetPropertyFromTextReader(Form form, TextReader reader, ref int module_sizex, ref int module_sizey, ref EnumModuleWindowStyle module_style, ref EnumWindowStyleFlags module_style_flag, ref int location_method, ref int location_x, ref int location_y, ref bool popup_dialog, ref int opacity, ref bool alwaysontop)
        static void GetPropertyFromTextReader(Form form, TextReader reader, ref int module_sizex, ref int module_sizey, ref EnumModuleWindowStyle module_style, ref EnumWindowStyleFlags module_style_flag, ref int location_method, ref int location_x, ref int location_y, ref bool popup_dialog, ref int opacity, ref bool alwaysontop, ref int monitorIndex)
        {
            string str = "";
            CommaBlockString comma = new CommaBlockString();
            string imsi = "";

            bool flag_resolution = false;
            bool flag_style = false;
            bool flag_style_flag = false;
            bool flag_default_location = false;
            bool flag_default_monitorIndex = false; //monitorIndex 추가 20241010

            while (true)
            {
                if (flag_resolution &&
                flag_style &&
                flag_style_flag &&
                flag_default_location && flag_default_monitorIndex) break;	// 필요한 특성은 읽었으므로 시간을 절약하기 위해 return;

                str = reader.ReadLine();

                if (str == null) break;

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
                else if (imsi == "DefaultMonitorIndex")   //monitorIndex 추가 20241010
                {
                    flag_default_monitorIndex = true;
                    comma.GetInt(ref monitorIndex);
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

                    comma.Skip();   // Buf.GetChar(ref cSmoothingMode);
                    comma.GetBool(ref alwaysontop);
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
        }


        // 20241010 monitorIndex 추가
        static bool GetPropertyByHashedFile(Form form, byte[] buffer_org, string filename, ref int module_sizex, ref int module_sizey, ref EnumModuleWindowStyle module_style, ref EnumWindowStyleFlags module_style_flag, ref int location_method, ref int location_x, ref int location_y, ref bool popup_dialog, ref int opacity, ref bool alwaysontop, ref int monitorIndex)
        {
            byte[] buffer = new byte[buffer_org.Length / 2];

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = HexBuf.ToByte(buffer_org, i * 2);
            }

            if (buffer[6] > 1)  // major version
            {
                string msg;
                msg = String.Format("Binary Modx file Major Version too Higher\nFile Version={0}.{1}\nProgram Version=1.0", buffer[6], buffer[7]);
                MessageBox.Show(msg, filename);
                return false;
            }

            HashBufferRuntime(buffer, 8, buffer.Length - 8);

            ushort file_crc = (ushort)(buffer[buffer.Length - 2] + buffer[buffer.Length - 1] * 256);
            ushort crc = NetTools.GetCRC.SumWORD(buffer, 0, buffer.Length - 2);

            if (file_crc != crc)
            {
                MessageBox.Show("Binary Modx CRC mismatched.", filename);
                return false;
            }

            uint size = (uint)(buffer[8] + (buffer[9] << 8) + (buffer[10] << 16) + (buffer[11] << 24));

            if (size != buffer.Length)
            {
                MessageBox.Show("Size mismatched.", filename);
                return false;
            }

            byte[] buffer2 = new byte[buffer.Length - 102];

            Array.Copy(buffer, 100, buffer2, 0, buffer.Length - 102);

            HashBufferRuntime(buffer2, 0, buffer2.Length);

            MemoryStream stream = new MemoryStream(buffer2);

            MemoryStream smod = ObjectAnimation.RestoreFromZipStream(stream, "group.modx");

            if (smod == null) return false;

            TextReader reader = new StreamReader(smod);

            GetPropertyFromTextReader(form, reader, ref module_sizex, ref module_sizey, ref module_style, ref module_style_flag, ref location_method, ref location_x, ref location_y, ref popup_dialog, ref opacity, ref alwaysontop, ref monitorIndex);

            smod.Close();
            stream.Close();

            return true;
        }



        //monitorIndex 추가 20241010
        static int GetModuleProperty(string filename, ref int module_sizex, ref int module_sizey, ref EnumModuleWindowStyle module_style, ref EnumWindowStyleFlags module_style_flag, ref int location_method, ref int location_x, ref int location_y, ref bool popup_dialog, ref int opacity, ref bool alwaysontop, ref int monitorIndex)
        {
            string path;

            module_sizex = 100;
            module_sizey = 100;
            module_style = EnumModuleWindowStyle.MDI;
            module_style_flag = EnumWindowStyleFlags.WS_FULL;
            location_method = 0;
            location_x = 0;
            location_y = 0;
            opacity = 100;
            popup_dialog = false;
            alwaysontop = false;
            monitorIndex = 0;


            path = filename;

            if (!File.Exists(path)) return 0;

            byte[] buffer = File.ReadAllBytes(path);

            //7E214023
            //if (buffer.Length > 102 && buffer[0] == '~' && buffer[1] == '!' && buffer[2] == '@' && buffer[3] == '#')
            if (buffer.Length > 102 && buffer[0] == '7' && buffer[1] == 'E' && buffer[2] == '2' && buffer[3] == '1' &&
                buffer.Length > 102 && buffer[4] == '4' && buffer[5] == '0' && buffer[6] == '2' && buffer[7] == '3')
            {
                if (!GetPropertyByHashedFile(TotalConfig.formMain, buffer, filename, ref module_sizex, ref module_sizey, ref module_style, ref module_style_flag, ref location_method, ref location_x, ref location_y, ref popup_dialog, ref opacity, ref alwaysontop, ref monitorIndex))

                    return 1;

                return 0;
            }
            else
            {
                MemoryStream stream = new MemoryStream(buffer);
                TextReader reader = new StreamReader(stream);

                if (reader == null) return 0;

                GetPropertyFromTextReader(TotalConfig.formMain, reader, ref module_sizex, ref module_sizey, ref module_style, ref module_style_flag, ref location_method, ref location_x, ref location_y, ref popup_dialog, ref opacity, ref alwaysontop, ref monitorIndex);

                reader.Close();
            }

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

        // MakeGraphicPopupWindow 메서드 수정
        private static FormGraphicFrame MakeGraphicPopupWindow(string filename, int sizex, int sizey,
            EnumWindowStyleFlags style, int location_method, int posx, int posy, bool popup_dialog, int opacity, int monitorIndex)
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


            // 모니터 정보 가져오기
            Screen[] screens = Screen.AllScreens;
            if (monitorIndex < 0 || monitorIndex >= screens.Length)
            {
                monitorIndex = 0; // 유효하지 않은 인덱스인 경우 기본 모니터 사용
            }
            Screen targetScreen = screens[monitorIndex];

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

            // 위치 계산
            switch (location_method)
            {
                case 0:
                    startx = targetScreen.Bounds.Left + targetScreen.Bounds.Width / 2 - sizex / 2 + posx;
                    starty = targetScreen.Bounds.Top + targetScreen.Bounds.Height / 2 - sizey / 2 + posy;
                    break;
                case 1:
                    startx = targetScreen.Bounds.Left + posx;
                    starty = targetScreen.Bounds.Top + posy;
                    break;
                case 2:
                    startx = targetScreen.Bounds.Right - sizex - posx;
                    starty = targetScreen.Bounds.Top + posy;
                    break;
                case 3:
                    startx = targetScreen.Bounds.Left + posx;
                    starty = targetScreen.Bounds.Bottom - sizey - posy;
                    break;
                case 4:
                    startx = targetScreen.Bounds.Right - sizex - posx;
                    starty = targetScreen.Bounds.Bottom - sizey - posy;
                    break;
                default:
                    startx = targetScreen.Bounds.Left + targetScreen.Bounds.Width / 2 - sizex / 2;
                    starty = targetScreen.Bounds.Top + targetScreen.Bounds.Height / 2 - sizey / 2;
                    break;
            }

            // 화면 범위 체크
            //startx = Math.Max(targetScreen.Bounds.Left, Math.Min(startx, targetScreen.Bounds.Right - sizex));
            //starty = Math.Max(targetScreen.Bounds.Top, Math.Min(starty, targetScreen.Bounds.Bottom - sizey));

            ex.StartPosition = FormStartPosition.Manual;
            ex.Left = startx;
            ex.Top = starty;
            ex.Width = sizex;
            ex.Height = sizey;

            ex.ShowInTaskbar = false;
            ex.Opacity = opacity / 100.0;

            return ex;
        }

        //20241010 monitorIndex 추가
        static void MakeMdiChildGraphicWindow(string filename, int pos_method, int posx, int posy,
    int module_sizex, int module_sizey, EnumModuleWindowStyle module_style, EnumWindowStyleFlags module_style_flag,
    int module_location_method, int module_posx, int module_posy, bool popup_dialog, int opacity, bool alwaysontop, int module_moniotrIndex)
        {
            if (module_style == EnumModuleWindowStyle.POPUP)
            {
                if (pos_method == -1)
                {
                    FormGraphicFrame dialog = MakeGraphicPopupWindow(filename, module_sizex, module_sizey, module_style_flag,
                        module_location_method, module_posx, module_posy, popup_dialog, opacity, module_moniotrIndex);

                    if (popup_dialog) dialog.ShowDialog();
                    else
                    {
                        dialog.Owner = TotalConfig.formMain;
                        if (alwaysontop) dialog.TopMost = true;
                        dialog.Show();
                    }
                }
                else
                {
                    FormGraphicFrame dialog = MakeGraphicPopupWindow(filename, module_sizex, module_sizey, module_style_flag,
                        pos_method, posx, posy, popup_dialog, opacity, module_moniotrIndex);

                    if (popup_dialog) dialog.ShowDialog();
                    else
                    {
                        dialog.Owner = TotalConfig.formMain;
                        if (alwaysontop) dialog.TopMost = true;
                        dialog.Show();
                    }
                }
                return;
            }

            FormGraphicFrame ex = new GraphicModule.FormGraphicFrame(filename);

            // mdi가 하나도 없을 때는 최대화 시킨다.
            if (TotalConfig.formMain.MdiChildren.Length == 0)
            {
                ex.WindowState = FormWindowState.Maximized;
                //ex.Dock = DockStyle.Fill;
            }

            // MdiParent가 WindowState보다 앞에 있으면 적용이 되지 않는다.
            if (TotalConfig.formMain.IsMdiContainer)
            {
                ex.MdiParent = TotalConfig.formMain;
            }

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

        // 이전화면 메뉴동작.
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

        const int WM_MDINEXT = 0x224;

        static void ActivateMdiChild(Form main, Form childToActivate)
        {
            if (main.ActiveMdiChild != childToActivate)
            {
                MdiClient mdiClient = GetMDIClient(main);

                if (mdiClient == null) return;

                int count = main.MdiChildren.Length;
                Control form = null;  // next or previous MDIChild form

                int pos = mdiClient.Controls.IndexOf(childToActivate);
                if (pos < 0)
                {
                    MessageDisplay.Show("MDIChild form not found in MdiClient controls.");
                    return; // MDIChild form not found
                }

                if (mdiClient.Controls.Count < 2)
                    return; // MDI 자식이 1개뿐이면 WM_MDINEXT 불필요

                if (pos == 0)
                    form = mdiClient.Controls[1];  // get next and activate previous
                else
                    form = mdiClient.Controls[pos - 1];  // get previous and activate next


                // flag indicating whether to activate previous or next MDIChild
                IntPtr direction = new IntPtr(pos == 0 ? 1 : 0);

                // bada bing, bada boom
                SendMessage(mdiClient.Handle, WM_MDINEXT, form.Handle, direction);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg,
                                    IntPtr wParam, IntPtr lParam);


        static MdiClient GetMDIClient(Form main)
        {
            foreach (Control c in main.Controls)
            {
                if (c is MdiClient)
                    return (MdiClient)c;
            }

            return null;
            //throw new InvalidOperationException("No MDIClient !!!");
        }


        //20241010 monitorIndex 요소 추가 전체 수정.
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
                    // 버그를 방지하기 위해 옵션을 사용한다.
                    if (ConfigViewMain.bRemoveFlashingWhenActivatingMdiModule)
                    {
                        if (form.IsMdiChild)
                        {
                            ActivateMdiChild(TotalConfig.formMain, form);
                        }
                        else // PopUP
                        {
                            form.Focus();
                        }
                    }
                    else
                    {
                        form.Focus();
                    }

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
            bool alwaysontop = false;

            //GetModuleProperty(filename, ref module_sizex, ref module_sizey, ref module_style, ref module_style_flag,
            //    ref module_location_method, ref module_posx, ref module_posy, ref popup_dialog, ref opacity, ref alwaysontop);

            int monitorIndex = 0;
            GetModuleProperty(filename, ref module_sizex, ref module_sizey, ref module_style, ref module_style_flag,
                ref module_location_method, ref module_posx, ref module_posy, ref popup_dialog, ref opacity, ref alwaysontop, ref monitorIndex);

            //// 모니터 정보 가져오기
            //Screen[] screens = Screen.AllScreens;
            //if (monitorIndex < 0 || monitorIndex >= screens.Length)
            //{
            //    monitorIndex = 0; // 유효하지 않은 인덱스인 경우 기본 모니터 사용
            //}
            //Screen targetScreen = screens[monitorIndex];

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
                    module_location_method, module_posx, module_posy, popup_dialog, opacity, alwaysontop, monitorIndex);

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
                    module_location_method, module_posx, module_posy, popup_dialog, opacity, alwaysontop, monitorIndex);
            }
        }

        // form_parint_action = 이 스크립트가 실행된 form
        static public object ExecuteClassNameOnlyObject(string classname, string command, out object retn_value, params object[] args)
        {
            int count = FormGraphicFrame.arrayFormGraphFrame.Count;
            int i;
            GraphicModule.FormGraphicFrame form;
            bool retn = false;
            retn_value = 0;

            for (i = 0; i < count; i++)
            {
                form = (GraphicModule.FormGraphicFrame)FormGraphicFrame.arrayFormGraphFrame[i];

                retn = form.formChild.objectGraphic.ExecuteClassNameOnlyObject(form.formChild, classname, command, out retn_value, args);
                if (retn) return true;
            }

            retn = ToolBarModule.ExecuteClassNameOnlyObject(classname, command, out retn_value, args);

            return retn;
        }

        static void ChangeTagName(FormGraphicFrame ex, string tag)
        {
            ArrayList array = new ArrayList();

            ex.formChild.objectGraphic.groupRoot.GetMultiSelectTagList(array);

            MULTI_SELECT_TAG_STRUCT list;

            for (int i = 0; i < array.Count; i++)
            {
                list = (MULTI_SELECT_TAG_STRUCT)array[i];

                if (list.tagSource == "__ControlBox_Tag")
                {
                    list.tagTarget = tag;
                }
            }

            ex.formChild.objectGraphic.groupRoot.SetMultiSelectTagList(array);
        }

        /// <summary>
        /// 사용자가 정의한 제어 박스를 표시한다.
        /// </summary>
        /// <param name="filename"></param>
        public static void DisplayUserControlBox(string filename, string tag, string dialog_title, string dialog_description, string dialog_minvalue, string dialog_maxvalue)
        {
            if (!File.Exists(filename))
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("사용자 정의 출력상자용 모듈 파일을 찾을 수 없습니다.", filename);
                else
                    MessageBox.Show("The module name of User Defined Control Box is not founded.", filename);

                return;
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
            bool alwaysontop = false;
            TagPublicClass tp;
            int[] tag_pos = new int[1];
            int monitorIndex = 0;

            tp = TagLib.GetStructPublic(tag, ref tag_pos);

            GetModuleProperty(filename, ref module_sizex, ref module_sizey, ref module_style, ref module_style_flag,
                ref module_location_method, ref module_posx, ref module_posy, ref popup_dialog, ref opacity, ref alwaysontop, ref monitorIndex);

            //popup_dialog = true; 9에서 Modal과 UnModal을 사용할 수 있어서 일단 뺏다 2009.6.4

            sTempUserControlBoxTag = tag;

            if (dialog_title == "?")
            {
                sTempUserControlBoxTitle = "Tag : " + tag;
            }
            else
            {
                sTempUserControlBoxTitle = dialog_title;
            }

            if (dialog_description == "?")
            {
                sTempUserControlBoxDescription = tp.description;
            }
            else
            {
                sTempUserControlBoxDescription = dialog_description;
            }

            sTempUserControlBoxMaxValue = dialog_maxvalue;
            sTempUserControlBoxMinValue = dialog_minvalue;
            //if (pos_method == -1)
            //{
            FormGraphicFrame dialog = MakeGraphicPopupWindow(filename, module_sizex, module_sizey, module_style_flag,
                module_location_method, module_posx, module_posy, popup_dialog, opacity, monitorIndex);

            ChangeTagName(dialog, tag);

            dialog.formChild.bUseUserDefinedTitle = true;
            dialog.Text = sTempUserControlBoxTitle;

            if (popup_dialog) dialog.ShowDialog();
            else
            {
                dialog.Owner = TotalConfig.formMain;
                if (alwaysontop) dialog.TopMost = true;
                dialog.Show();
            }

            /*
            }
            else
            {
                MakeGraphicPopupWindow(filename, module_sizex, module_sizey, module_style_flag,
                    pos_method, posx, posy, popup_dialog, opacity);
            }*/
        }

        public static string sTempUserControlBoxTitle = "";
        public static string sTempUserControlBoxDescription = "";
        public static string sTempUserControlBoxMaxValue = "";
        public static string sTempUserControlBoxMinValue = "";
        public static string sTempUserControlBoxTag = "";



        //monitorIndex를 사용하는 오버로딩 함수 추가.
        public static void RestoreGraphicWindow(string filename, int pos_method, int posx, int posy, int monitorIndex)
        {
            // 기존 히스토리 관리 코드는 그대로 유지
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

            // 기존 열린 창 확인 코드
            int count = FormGraphicFrame.arrayFormGraphFrame.Count;

            for (int i = 0; i < count; i++)
            {
                GraphicModule.FormGraphicFrame form = (GraphicModule.FormGraphicFrame)FormGraphicFrame.arrayFormGraphFrame[i];
                if (String.Compare(form.formChild.sFileName, filename, true) == 0)
                {
                    // 기존 창 활성화 코드
                    if (ConfigViewMain.bRemoveFlashingWhenActivatingMdiModule)
                    {
                        if (form.IsMdiChild)
                        {
                            ActivateMdiChild(TotalConfig.formMain, form);
                        }
                        else // PopUP
                        {
                            form.Focus();
                        }
                    }
                    else
                    {
                        form.Focus();
                    }

                    if (form.WindowState == FormWindowState.Minimized)
                        form.WindowState = FormWindowState.Normal;

                    return;
                }
            }

            // 모듈 속성 가져오기
            int module_sizex = 0, module_sizey = 0;
            EnumModuleWindowStyle module_style = 0;
            EnumWindowStyleFlags module_style_flag = 0;
            int module_location_method = 0, module_posx = 0, module_posy = 0;
            int opacity = 0;
            bool popup_dialog = false, alwaysontop = false;
            int module_moniotrIndex = 0;

            GetModuleProperty(filename, ref module_sizex, ref module_sizey, ref module_style, ref module_style_flag,
                ref module_location_method, ref module_posx, ref module_posy, ref popup_dialog, ref opacity, ref alwaysontop, ref module_moniotrIndex);

            // mdi창을 만들때만 창제한을 검사한다.
            if (module_style == EnumModuleWindowStyle.MDI)
            {
                // MDI 창 생성 로직
                // 같은 이름의 그래픽 모듈 파일을 찾지 못했다.
                // MDI개수가 제한 갯수를 넘으면 처음 MDI를 닫는다.
                int mdi_count = 0;
                GraphicModule.FormGraphicFrame formMdi = null;

                for (int i = 0; i < count; i++)
                {

                    GraphicModule.FormGraphicFrame form = (GraphicModule.FormGraphicFrame)FormGraphicFrame.arrayFormGraphFrame[i];
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
                    module_location_method, module_posx, module_posy, popup_dialog, opacity, alwaysontop, 0);

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
                // 모니터 정보 가져오기
                Screen[] screens = Screen.AllScreens;
                if (monitorIndex < 0 || monitorIndex >= screens.Length)
                {
                    monitorIndex = 0; // 유효하지 않은 인덱스인 경우 기본 모니터 사용
                }

                MakeMdiChildGraphicWindow(filename, pos_method, posx, posy, module_sizex, module_sizey, module_style, module_style_flag,
                    module_location_method, module_posx, module_posy, popup_dialog, opacity, alwaysontop, monitorIndex);
            }

        }


    }
}
