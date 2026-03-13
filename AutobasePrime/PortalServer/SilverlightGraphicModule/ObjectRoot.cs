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
using System.IO;
using NetTools;
using System.Windows.Resources;
using NetTools.OldDefine;
using AutoLibLocal;

namespace SilverlightGraphicModule
{
    
    public enum EnumModType
    {
        known,	//
        mod,	// 옛날 방식 (Ascii, script file)
        modx,	// 현재 방식 (Unicode)
    }

    /// <summary>
    /// Summary description for ObjectRoot.
    /// </summary>
    public class ObjectRoot
    {
        //public string sSelectedTagName;
        //public EnumTagType eSelectedTagType;

        public ObjectLayer groupRoot;
        //readonly int ON = 1;
        //readonly int OFF = 0;

        ObjectBack backBitmap;
        int nBackBitmapDisplayMethod = 0;	// 0 - 왼쪽 상단
        // 1 - tile으로

        public ObjectCommonProperty objCommonProperty = new ObjectCommonProperty();

        POINT pBasePoint = new POINT();
        Color lBackGroundColor = Colors.White;	// 배경 색상 초기색상을 지정하지 않으면 완전투명으로 된다.
        public int cObjectOpticMethod;			// Object를 보여주는 방법 (실제,화면에 맞춤)
        int nModuleWindowStyle;				// module의 window create style
        uint dwModuleWindowStyleFlag = 0xFFFF;	// caption, border 등의 기타 옵션
        public int nModuleSizeX = 640;			// 모듈 가로 크기
        public int nModuleSizeY = 480;			// 모듈 세로 크기
        int nScreenSizeX;				// 화면 가로 크기
        int nScreenSizeY;				// 화면 세로 크기
        public int nOpticRate = 100;			// Default = 100 %

        public int GetOpticRate() { return nOpticRate; }
        public void GetModuleSize(out int x, out int y) { x = nModuleSizeX; y = nModuleSizeY; }

        public void SetModuleWindowStyle(int style) { nModuleWindowStyle = style; }
        public int GetModuleWindowStyle() { return nModuleWindowStyle; }

        public void SetModuleWindowStyleFlag(uint style) { dwModuleWindowStyleFlag = style; }
        public uint GetModuleWindowStyleFlag() { return dwModuleWindowStyleFlag; }

        //int nDefaultLocationMethod;
        //int nDefaultLocationX;
        //int nDefaultLocationY;

        bool bModulePopupDialog = false;
        int nModuleOpacity = 100;

        public sbyte bNotUsedMdiLimit = 0;

        public ScriptClass scriptModuleStart;
        public ScriptClass scriptModuleEnd;
        public ScriptClass scriptModuleAlways;
        public ScriptClass scriptModuleActive;
        public ScriptClass scriptModuleDeactive;

        public bool ModulePopupDialog
        {
            get
            {
                return bModulePopupDialog;
            }
            set
            {
                bModulePopupDialog = value;
            }
        }

        public int ModuleOpacity
        {
            get
            {
                return nModuleOpacity;
            }
            set
            {
                nModuleOpacity = value;
                if (nModuleOpacity > 100) nModuleOpacity = 100;
                if (nModuleOpacity < 1) nModuleOpacity = 100;
            }
        }

        public ObjectRoot()
        {
            //
            // TODO: Add constructor logic here
            //
            groupRoot = new ObjectLayer(this.objCommonProperty, null, null, null, null);
            
            pBasePoint.x = 0;
            pBasePoint.y = 0;
            backBitmap = null;
            lBackGroundColor = Colors.White;
            nBackBitmapDisplayMethod = 0;			// 0 - 왼쪽 상단에 그림을 뿌림
            cObjectOpticMethod = 0;     
            nModuleWindowStyle = 0;
            nModuleSizeX = 1024;
            nModuleSizeY = 700;
            nScreenSizeX = 1024;
            nScreenSizeY = 768;

            //nDefaultLocationMethod = 0;
            //nDefaultLocationX = 0;
            //nDefaultLocationY = 0;

            nOpticRate = 100;

            //groupRoot.UpdateGroupRealSize(nModuleSizeX, nModuleSizeY);

            //EnumWindowStyleFlags flags = 0;
            //flags |= EnumWindowStyleFlags.WS_CAPTION;
            //flags |= EnumWindowStyleFlags.WS_SYSMENU;
            //flags |= EnumWindowStyleFlags.WS_BORDER;
            //this.SetModuleWindowStyleFlag((uint)flags);
        }

        /*
        // 이 오브젝트를 닫기전에 해야 할 일
        public void Close()
        {
            groupRoot.Close();
        }

        public void AddObject(object p)
        {
            groupRoot.AddObject(p);
        }

        public void Display(Graphics gScreen, Rectangle rcScreen, Rectangle rcPaint, Point scroll_pos)
        {
            if (rcScreen.Width < 1 || rcScreen.Height < 1) return;	// 너무 작은 사이즈는 오류가 난다.

            pBasePoint.x = scroll_pos.X;
            pBasePoint.y = scroll_pos.Y;

            // Memory 비트맵을 사용하면 이 윈도우 위에 투명한 윈도우가 오면 업데이트 시 쓰레기 라인이 보이는 현상이 발생한다. 그래서 form시작에 DoubleBuffer를 선언해서 사용함 2007.6.14

            gScreen.SmoothingMode = SmoothingMode.HighSpeed;
            gScreen.CompositingMode = CompositingMode.SourceOver;

            gScreen.FillRectangle(new SolidBrush(lBackGroundColor), rcScreen);

            if (backBitmap != null)
            {
                backBitmap.Display(gScreen, rcPaint, nBackBitmapDisplayMethod, scroll_pos);
            }

            SetBasePoint(scroll_pos.X, scroll_pos.Y);
            groupRoot.Display(gScreen, rcPaint, 0, 0);
        }

        public void DisplayPrint(Graphics g, Rectangle rcScreen, Rectangle rcPaint, Point scroll_pos)
        {
            if (rcScreen.Width < 1 || rcScreen.Height < 1) return;	// 너무 작은 사이즈는 오류가 난다.

            pBasePoint.x = scroll_pos.X;
            pBasePoint.y = scroll_pos.Y;

            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.CompositingMode = CompositingMode.SourceOver;

            g.FillRectangle(new SolidBrush(lBackGroundColor), rcScreen);

            if (backBitmap != null)
            {
                backBitmap.Display(g, rcPaint, nBackBitmapDisplayMethod, scroll_pos);
            }

            SetBasePoint(scroll_pos.X, scroll_pos.Y);
            groupRoot.Display(g, rcPaint, 0, 0);
        }
        */
        public void EventTimer(Page form)
        {
            groupRoot.EventTimer(form);
            //form.Update();             일단 10.0 부터는 제외했다. 화면 업데이트가 늦는 경우 다시 사용해야 할 수도 있다. 더블 버퍼인 경우 사용해 보지 않았기 때문에 Upgrade하면서 제외해 보았다.
        }
        /*
        public void EventTag(System.Windows.Forms.Form form, COMM_EVENT_STRUCT tagevent)
        {
            groupRoot.EventTag(form, tagevent);
        }*/

        public void WmLeftButtonDown(UserControl form, MouseEventArgs e)
        {
            groupRoot.WmLeftButtonDown(form, e);
        }

        public bool WmMouseMove(UserControl form, MouseEventArgs e)
        {
            //ObjectExpand.ToolTipCheckStart();
            bool retn = groupRoot.WmMouseMove(form, e);
            //ObjectExpand.ToolTipCheckEnd();
            return retn;
        }

        public void WmLeftButtonUp(UserControl form, MouseEventArgs e)
        {
            groupRoot.WmLeftButtonUp(form, e);
        }

        public void OnTagFileReaded()
        {
            groupRoot.OnTagFileReaded();
        }

        public int Load(Page pgrid, string filename)
        {
            this.objCommonProperty.sModuleName = filename;
            //this.objCommonProperty.rootCanvas = pgrid.LayoutRoot;
            this.objCommonProperty.rootPage = pgrid;

            if (filename.IndexOf(".mod.txt", StringComparison.CurrentCultureIgnoreCase) != -1)
            {
                TextBlock tb = new TextBlock();

                if(Tools.IsLangKorean())
                    tb.Text = "실버라이트에서는 MOD파일 형식을 읽을 수 없습니다.\n스튜디오에서 MODX 형식으로 변경하여 사용하시기 바랍니다.";
                else
                    tb.Text = "Cannot load the MOD file format in Silverlight.\nYou must convert MOD file to MODX in Studio.";

                tb.Text += String.Format("\nFilename={0}", System.IO.Path.GetFileNameWithoutExtension(filename));

                tb.TextAlignment = TextAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;
                
                this.objCommonProperty.rootPage.LayoutRoot.Children.Add(tb);

                return 1;
            }
            
            WebClient client = new WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            client.OpenReadAsync(new Uri(filename, UriKind.Absolute));

            return 1;
        }

        public delegate void DelegateOnModuleReadComplete();
        public DelegateOnModuleReadComplete procOnModuleReadComplete = null;

        // Hash된 파일에서 가져온다.
        TextReader GetNewReaderFromHashedFile(string buffer_org)
        {
            //string filename = "";

            byte[] buffer = new byte[buffer_org.Length / 2];

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = HexBuf.ToByte(buffer_org, i * 2);
            }

            if (buffer[6] > 1)  // major version
            {
                string msg;
                msg = String.Format("Binary Modx file Major Version too Higher\nFile Version={0}.{1}\nProgram Version=1.0", buffer[6], buffer[7]);
                //MessageBox.Show(msg, filename);
                return null;
            }

            HashBufferRuntime(buffer, 8, buffer.Length - 8);

            ushort file_crc = (ushort)(buffer[buffer.Length - 2] + buffer[buffer.Length - 1] * 256);
            ushort crc = NetTools.GetCRC.SumWORD(buffer, 0, buffer.Length - 2);

            if (file_crc != crc)
            {
                //MessageBox.Show("Binary Modx CRC mismatched.", filename);
                return null;
            }

            uint size = (uint)(buffer[8] + (buffer[9] << 8) + (buffer[10] << 16) + (buffer[11] << 24));

            if (size != buffer.Length)
            {
                //MessageBox.Show("Size mismatched.", filename);
                return null;
            }

            byte[] buffer2 = new byte[buffer.Length - 102];

            Array.Copy(buffer, 100, buffer2, 0, buffer.Length - 102);

            HashBufferRuntime(buffer2, 0, buffer2.Length);

            MemoryStream stream = new MemoryStream(buffer2);

            MemoryStream smod = ObjectAnimation.RestoreFromZipStream(stream, "group.modx");

            if (smod == null) return null;

            TextReader reader = new StreamReader(smod);

            return reader;
        }

        bool HashBufferRuntime(byte[] buffer, int index, int size)
        {
            int[] rand_hash = new int[17] { 0x78, 0x31, 0x91, 0x78, 0xD7, 0x63, 0xB7, 0x19, 0x7F, 0xB2, 0xCA, 0x8E, 0x23, 0xA8, 0x6F, 0x78, 0x71 };

            for (int i = 0, pos = index; i < size; i++, pos++)
            {
                buffer[pos] = (byte)(buffer[pos] ^ (i % 256) ^ rand_hash[i % 17]);
            }

            return true;
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null || e.Cancelled == true) return;

                if (e.Result == null)
                {
                    MessageBox.Show(this.objCommonProperty.sModuleName + "의 e.Result가 null");
                    return;
                }

                StreamResourceInfo resinfo = new StreamResourceInfo(e.Result, null);

                TextReader reader;
                string msg;
                string one_line = "";
                string imsi = "";
                CommaBlockString commaBuf = new CommaBlockString();
                bool tile_flag = false;
                int version_major = 0, version_minor = 0, version_build = 0, version_revision = 0;
                EnumModType load_type = EnumModType.modx;

                string filename = this.objCommonProperty.sModuleName;

                if (resinfo.Stream == null) return;

                reader = new StreamReader(resinfo.Stream);

                if (reader == null)
                {
                    msg = String.Format("{0}\nCan't open file.", filename);
                    MessageBox.Show(msg, "ObjectRoot.Load (StreamReader = NULL)", MessageBoxButton.OK);
                    return;
                }

                cObjectOpticMethod = 0;
                nModuleSizeX = 1024;
                nModuleSizeY = 700;

                while (true)
                {
                    one_line = reader.ReadLine();
                    if (one_line == null) break;
                    if (one_line.Length == 0) continue;
                    if (one_line[0] == '[') continue;

                    // 암호화된 파일
                    if (String.Compare(one_line, 0, "7E214023", 0, 8) == 0)
                    {
                        reader.Close();
                        reader = GetNewReaderFromHashedFile(one_line);
                        continue;
                    }

                    commaBuf.Set(one_line);
                    commaBuf.GetString(ref imsi);

                    if (imsi == "Resolution")
                    {
                        commaBuf.GetInt(ref nModuleSizeX);
                        commaBuf.GetInt(ref nModuleSizeY);
                    }
                    else if (imsi == "DefaultLocation")
                    {
                        //commaBuf.GetInt(ref nDefaultLocationMethod);
                        //commaBuf.GetInt(ref nDefaultLocationX);
                        //commaBuf.GetInt(ref nDefaultLocationY);
                    }
                    else if (imsi == "Version")
                    {
                        commaBuf.GetInt(ref version_major);
                        commaBuf.GetInt(ref version_minor);
                        commaBuf.GetInt(ref version_build);
                        commaBuf.GetInt(ref version_revision);
                    }
                    else if (imsi == "ModuleOptic")
                    {
                        commaBuf.GetInt(ref cObjectOpticMethod);
                    }
                    else if (imsi == "ModuleWindowStyle")
                    {
                        commaBuf.GetInt(ref nModuleWindowStyle);
                        commaBuf.GetBool(ref bModulePopupDialog);
                        int opacity = 0;
                        commaBuf.GetInt(ref opacity);
                        ModuleOpacity = opacity;	// 값이 초과할 때 초기화 필요.
                    }
                    else if (imsi == "ModuleWindowStyleFlag")
                    {
                        commaBuf.GetDWORD(ref dwModuleWindowStyleFlag);
                        commaBuf.GetChar(ref bNotUsedMdiLimit);
                    }
                    else if (imsi == "BackGroundColor")
                    {
                        int cr = 0;
                        int cg = 0;
                        int cb = 0;
                        int ca = 0;

                        commaBuf.GetInt(ref cr);
                        commaBuf.GetInt(ref cg);
                        commaBuf.GetInt(ref cb);
                        commaBuf.GetInt(ref ca);

                        Color color = Color.FromArgb((byte)ca, (byte)cr, (byte)cg, (byte)cb);
                        SetBackGroundColor(color);
                    }
                    else if (imsi == "BackGround")
                    {
                        commaBuf.GetString(ref imsi);
                        commaBuf.GetInt(ref nBackBitmapDisplayMethod);

                        int x = 0, y = 0;   // 9.3.4부터 지원
                        commaBuf.GetInt(ref x);
                        commaBuf.GetInt(ref y);

                        if (imsi.Length > 0)
                        {
                            tile_flag = nBackBitmapDisplayMethod == 1 ? true : false;
                            backBitmap = new ObjectBack(objCommonProperty, this.objCommonProperty.rootPage.LayoutRoot, imsi, x, y, tile_flag);
                        }
                    }
                    else if (imsi == "ObjectGroup")
                    {	// group의 시작이다.
                        groupRoot.Load(objCommonProperty, this.objCommonProperty.rootPage.LayoutRoot, null, reader, filename, 0, load_type);
                    }
                    else if (imsi == "ScriptModuleActive")
                    {
                        this.scriptModuleActive = LoadObjectFromModX.LoadOneScript(reader, imsi);
                    }
                    else if (imsi == "ScriptModuleAlways")
                    {
                        this.scriptModuleAlways = LoadObjectFromModX.LoadOneScript(reader, imsi);
                    }
                    else if (imsi == "ScriptModuleDeactive")
                    {
                        this.scriptModuleDeactive = LoadObjectFromModX.LoadOneScript(reader, imsi);
                    }
                    else if (imsi == "ScriptModuleEnd")
                    {
                        this.scriptModuleEnd = LoadObjectFromModX.LoadOneScript(reader, imsi);
                    }
                    else if (imsi == "ScriptModuleStart")
                    {
                        this.scriptModuleStart = LoadObjectFromModX.LoadOneScript(reader, imsi);
                    }
                    else
                    {
                        // 알 수 없는 명령어지만 처리하지 않는다.
                    }
                }

                reader.Close();

                SetBasePoint(0, 0);		// base point set to 0,0

                if (nModuleSizeX < 10) nModuleSizeX = 10;
                if (nModuleSizeY < 10) nModuleSizeY = 10;

                SetObjectOpticMethod(cObjectOpticMethod);

                SetModuleSize(nModuleSizeX, nModuleSizeY);
                // module크기를 바꾸고 한번 불러준다.
                // groupRoot.UpdateGroupRealSize(nModuleSizeX, nModuleSizeY);

                SetScreenSize(nScreenSizeX, nScreenSizeY);
                SetZoneAtPercent100();

                if (procOnModuleReadComplete != null) procOnModuleReadComplete();

                return;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message+"\n"+exception.StackTrace);
            }
        }

        public void SetBackGroundColor(Color color)
        {
            lBackGroundColor = color;
            this.objCommonProperty.rootPage.LayoutRoot.Background = new SolidColorBrush(color); // null 로 두면 마우스 응답을 하지 않는다.
            this.objCommonProperty.rootPage.gridBackground.Background = new SolidColorBrush(color);
        }

        public Color GetBackGroundColor()
        {
            return lBackGroundColor;
        }
        
        public void SetBasePoint(int x, int y)
        {
            /*
            pBasePoint.x = x;
            pBasePoint.y = y;

            if (backBitmap != null) backBitmap.SetBasePoint(x, y);

            groupRoot.SetBasePoint(x, y);*/
        }
        
        public void SetObjectOpticMethod(int method)
        {
            cObjectOpticMethod = method;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT)
            {
                method = 0;
            }

            if (backBitmap != null)
            {
                backBitmap.SetObjectOpticMethod(method);
            }

            groupRoot.SetObjectOpticMethod(method);
        }
        
        public void SetScreenSize(int x, int y)
        {
            nScreenSizeX = x;
            nScreenSizeY = y;

            if (backBitmap != null)
            {
                backBitmap.SetScreenSize(x, y);
            }

            groupRoot.SetScreenSize(x, y);
        }

        public void SetModuleSize(int x, int y)
        {
            nModuleSizeX = x;
            nModuleSizeY = y;

            if (backBitmap != null)
            {
                backBitmap.SetModuleSize(x, y);
            }

            groupRoot.SetModuleSize(x, y);
        }
        
        public void SetZoneAtPercent100()
        {
            /*
            RECT r = new RECT();

            r.left = 0;
            r.top = 0;
            r.right = nModuleSizeX - 1;
            r.bottom = nModuleSizeY - 1;
            groupRoot.SetZoneAtPercent100(r);*/
        }
        
        public object ExecuteClassNameOnlyObject(Page form, string classname, string command, params object[] args)
        {
            return groupRoot.ExecuteClassNameOnlyObject(form, classname, command, args);
        }
    }
}
