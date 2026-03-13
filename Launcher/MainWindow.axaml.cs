using AutobaseApp.WinformMethod;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace LauncherMain
{

    public partial class MainWindow : Window
    {
        Dictionary<string, string> programlist = new Dictionary<string, string>();
        string appDir = AppContext.BaseDirectory;
        int nIndex = 0;
        int nMinimumpage = 0;
        int nMaximumpage = 1;

        int nLang = 1; // 0 = EN, 1 = KR
        public MainWindow()
        {
            InitializeComponent();

            this.button_1.Click += pressed_button1;
            this.Page_left.Click += pressed_left;
            this.Page_right.Click += pressed_right;

            this.button_01.Click += button_KR_click;
            this.button_02.Click += button_EN_click;

            this.Loaded += Page_Loaded;

            LoadPrograms();


        }

        private void Page_Loaded(object? sender, EventArgs e)
        {

            if(IsAdmin() )
            {
                nMaximumpage = 2;
                this.Label_1.Text = "관리자 권한 실행 중";
            }
            RefreshPage();
            ChangeLang();

            this.Label_2.Text = Assembly.GetExecutingAssembly().GetName().Version!.ToString();
        }


        private void LoadPrograms()
        {
            string? parentDir = Path.GetFullPath(Path.Combine(appDir, ".."));


            if (Directory.Exists(parentDir))
            {
                var files = Directory.GetFiles(parentDir, "*.exe");
                foreach (var f in files)
                {
                    if (f.Contains("ExportServer"))
                    {
                        programlist["ExportServer"] = f;
                        this.border_5.IsVisible = true;

                    }
                    else if (f.Contains("LangTool"))
                    {
                        programlist["LangTool"] = f;
                        this.border_6.IsVisible = true;
                    }
                    else if (f.Contains("LocalConfig"))
                    {
                        programlist["LocalConfig"] = f;
                        this.border_7.IsVisible = true;
                    }
                    else if (f.Contains("LocalMain"))
                    {
                        programlist["LocalMain"] = f;
                        this.border_2.IsVisible = true;
                    }
                    else if (f.Contains("OpcClient"))
                    {
                        programlist["OpcClient"] = f;
                        this.border_11.IsVisible = true;
                    }
                    else if (f.Contains("PLC_SCAN"))
                    {
                        programlist["PLC_SCAN"] = f;
                        this.border_3.IsVisible = true;
                    }
                    else if (f.Contains("ProjectManager"))
                    {
                        programlist["ProjectManager"] = f;
                        this.border_0.IsVisible = true;
                    }
                    else if (f.Contains("RunMain"))
                    {
                        programlist["RunMain"] = f;
                        this.border_9.IsVisible = true;
                    }
                    else if (f.Contains("SMS"))
                    {
                        programlist["SMS"] = f;
                        this.border_8.IsVisible = true;
                    }
                    else if (f.Contains("Studio"))
                    {
                        programlist["Studio"] = f;
                        this.border_1.IsVisible = true;
                    }
                    else if (f.Contains("ViewMain"))
                    {
                        programlist["ViewMain"] = f;
                        this.border_10.IsVisible = true;
                    }
                    else if (f.Contains("WatchDog"))
                    {
                        programlist["WatchDog"] = f;
                        this.border_4.IsVisible = true;
                    }
                    else if (f.Contains("RegisterNetDll32"))
                    {
                        programlist["RegisterNetDll32"] = f;
                        this.border_16.IsVisible = true;
                    }
                    else if (f.Contains("RegisterNetDll.exe"))
                    {
                        programlist["RegisterNetDll"] = f;
                        this.border_17.IsVisible = true;
                    }


                }
            }

            string? parentDir2 = Path.GetFullPath(Path.Combine(parentDir, "OPCUAClient"));



            if (Directory.Exists(parentDir2))
            {
                var files2 = Directory.GetFiles(parentDir2, "*.exe");
                foreach (var f in files2)
                {
                    if (f.Contains("OpcUAClient"))
                    {
                        programlist["OpcUAClient"] = f;
                        this.border_12.IsVisible = true;

                    }
                }
            }

            string? parentDir3 = Path.GetFullPath(Path.Combine(parentDir, "OPCUAServer"));



            if (Directory.Exists(parentDir3))
            {
                var files3 = Directory.GetFiles(parentDir3, "*.exe");
                foreach (var f in files3)
                {
                    if (f.Contains("OpcUAServer"))
                    {
                        programlist["OpcUAServer"] = f;
                        this.border_13.IsVisible = true;

                    }
                }
            }

            string? parentDir4 = Path.GetFullPath(Path.Combine(parentDir, "Config"));



            if (Directory.Exists(parentDir4))
            {
                var files4 = Directory.GetFiles(parentDir4, "*.ini");
                foreach (var f in files4)
                {
                    if (f.Contains("Program"))
                    {
                        TextReader reader;

                        try
                        {
                            reader = new StreamReader(f, UTF8Encoding.UTF8);
                            while (true)
                            {
                                string text = reader.ReadLine();
                                if (text == null) break;
                                if (text.Length == 0) continue;

                                if( text.Contains("Lang="))
                                {
                                    if (text.Contains("Korean")) this.nLang = 1;
                                    else this.nLang = 0;
                                }
                            }
                        }
                        catch
                        {
                            return;
                        }

                        if (reader == null)
                        {
                            this.nLang = 1;
                            return;
                        }

                    }
                }
            }

            //언어
            string? parentDir5 = Path.GetFullPath(Path.Combine(parentDir, "RESTAPIClient"));

        }

        private  void pressed_0(object? sender, PointerPressedEventArgs e)
        {

           
            program_run("ProjectManager");
        }
        private  void pressed_1(object? sender, PointerPressedEventArgs e)
        {

            program_run("Studio");
        }

        private async void pressed_2(object? sender, PointerPressedEventArgs e)
        {

            
            program_run("LocalMain");
        }

        private  void pressed_3(object? sender, PointerPressedEventArgs e)
        {

         
            program_run("PLC_SCAN");
        }

        private  void pressed_4(object? sender, PointerPressedEventArgs e)
        {

            program_run("WatchDog");
        }

        private  void pressed_5(object? sender, PointerPressedEventArgs e)
        {

            
            program_run("ExportServer");
        }

        private  void pressed_6(object? sender, PointerPressedEventArgs e)
        {

            program_run("LangTool");
        }

        private  void pressed_7(object? sender, PointerPressedEventArgs e)
        {

           
            program_run("LocalConfig");
        }

        private  void pressed_8(object? sender, PointerPressedEventArgs e)
        {

            
            program_run("SMS");
        }

        private  void pressed_9(object? sender, PointerPressedEventArgs e)
        {

          
            program_run("RunMain");
        }

        private  void pressed_10(object? sender, PointerPressedEventArgs e)
        {

            
            program_run("ViewMain");
        }

        private  void pressed_11(object? sender, PointerPressedEventArgs e)
        {

            
            program_run("OpcClient");
        }

        private  void pressed_12(object? sender, PointerPressedEventArgs e)
        {

           
            program_run("OpcUAClient");
        }

        private  void pressed_13(object? sender, PointerPressedEventArgs e)
        {

           
            program_run("OpcUAServer");
        }

        private  void pressed_14(object? sender, PointerPressedEventArgs e)
        {

           
            program_run("RESTAPIClient");
        }

        private void pressed_16(object? sender, PointerPressedEventArgs e)
        {
                
            program_run("RegisterNetDll32");
        }

        private void pressed_17(object? sender, PointerPressedEventArgs e)
        {

           
            program_run("RegisterNetDll");
        }

        private void pressed_button1(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void pressed_left(object? sender, RoutedEventArgs e)
        {
            this.nIndex--;
            RefreshPage();
        }

        private void pressed_right(object? sender, RoutedEventArgs e)
        {
            this.nIndex++;
            RefreshPage();
        }

        private void RefreshPage()
        {
           

            if ( this.nIndex < this.nMinimumpage )
            {
                this.nIndex = this.nMinimumpage;
                               
            }

            if( this.nIndex > this.nMaximumpage)
            {
                this.nIndex = this.nMaximumpage;
               
            }

            if( this.nIndex == this.nMinimumpage)
            {
                this.Page_left.IsVisible = false;
                this.Page_right.IsVisible = true;
            }
            else if (this.nIndex == this.nMaximumpage )
            {
                this.Page_left.IsVisible = true;
                this.Page_right.IsVisible = false;
            }
            else
            {
                this.Page_left.IsVisible = true;
                this.Page_right.IsVisible = true;
            }

                switch (nIndex)
                {
                    case 0:
                        this.page_0.IsVisible = true;
                        this.page_1.IsVisible = false;
                        this.page_2.IsVisible = false;
                        break;
                    case 1:
                        this.page_0.IsVisible = false;
                        this.page_1.IsVisible = true;
                        this.page_2.IsVisible = false;
                        break;
                    case 2:
                        this.page_0.IsVisible = false;
                        this.page_1.IsVisible = false;
                        this.page_2.IsVisible = true;
                        break;
                    default:
                        this.page_0.IsVisible = true;
                        this.page_1.IsVisible = false;
                        this.page_2.IsVisible = false;
                        break;
                }

        }

        private async void program_run(string _filename)
        {
            try
            {
                var processes = Process.GetProcessesByName(_filename);

                foreach (var p in processes)
                {
                    if (p != null)
                    {
                        await MessageBox.Show("프로그램이 실행 중 입니다.", "알림", MessageBoxButtons.OK, this);
                        return;
                    }
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = programlist[_filename],
                    //UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                if (ex is Win32Exception exx)
                {
                    if (exx.NativeErrorCode == 740)
                    {
                        await MessageBox.Show("관리자 권한이 필요합니다.", "오류", MessageBoxButtons.OK, this);
                        return;
                    }
                }

                Console.WriteLine(ex.ToString());
            }
        }

        public bool IsAdmin()
        {
            if (OperatingSystem.IsWindows())
            {
                using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
                {
                    var principal = new System.Security.Principal.WindowsPrincipal(identity);
                    return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
                }
            }
            else
            {
                // Linux / macOS = root user
                return Environment.UserName == "root";
            }
        }
        private void button_EN_click(object? sender, RoutedEventArgs e)
        {
            nLang = 0;
            ChangeLang();
        }

        private void button_KR_click(object? sender, RoutedEventArgs e)
        {
            nLang = 1;
            ChangeLang();
        }


        private void ChangeLang()
        {
            if (nLang == 1)
            { 
                this.border_0.STitle = "작업선택";
                this.border_1.STitle = "스튜디오";
                this.border_2.STitle = "감시프로그램";
                this.border_3.STitle = "통신프로그램";
                this.border_4.STitle = "와치독";
                this.border_5.STitle = "익스포트 서버";
                this.border_8.STitle = "문자 메시지 관리기";
                this.border_9.STitle = "데이터 서버";
                this.border_6.STitle = "언어 선택";
                this.border_7.STitle = "기본 환경설정";
                this.border_10.STitle = "웹 뷰어";

                this.button_01.Content = "한국어";
                this.button_02.Content = "영어";
                this.button_1.Content = "나가기";

                if (IsAdmin())
                {
                    this.Label_1.Text = "관리자 권한 실행 중";
                }
                
            }
            else
            {
                this.border_0.STitle = "Project Manager";
                this.border_1.STitle = "Studio";
                this.border_2.STitle = "LocalMain";
                this.border_3.STitle = "PLC_SCAN";
                this.border_4.STitle = "WatchDog";
                this.border_5.STitle = "Export Server";
                this.border_8.STitle = "SMS";
                this.border_9.STitle = "RunMain";
                this.border_6.STitle = "LangTool";
                this.border_7.STitle = "LocalConfig";
                this.border_10.STitle = "ViewMain";

                this.button_01.Content = "KR";
                this.button_02.Content = "EN";
                this.button_1.Content = "Exit";

                if (IsAdmin())
                {
                    this.Label_1.Text = "Elevated for administrator privileges";
                }
                
            }
        }



    }



}