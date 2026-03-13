using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using AutoLibLocal;
using System.IO;
using System.Diagnostics;

namespace GraphicModule
{
    public partial class VLCActiveXControl : UserControl
    {
        private MyAxHost axControl;
        ScriptClass scriptClass = new ScriptClass();

        public bool bLoadFailed;

        public VLCActiveXControl()
        {
            {
                InitializeComponent();
                LoadActiveXControl("VideoLAN.VLCPlugin.2");

            }
        }

        //private void LoadActiveXControl(string progID)
        //{
        //    try
        //    {
        //        // ProgID로부터 CLSID 검색
        //        string clsid = GetCLSIDFromProgID(progID);
        //        if (string.IsNullOrEmpty(clsid))
        //        {
        //            throw new Exception("CLSID not found for ProgID: " + progID);
        //        }

        //        // 커스텀 AxHost 클래스를 사용하여 인스턴스 생성
        //        axControl = new MyAxHost(clsid);

        //        // 컨트롤의 위치 및 크기를 설정
        //        axControl.Location = new System.Drawing.Point(10, 10);
        //        axControl.Size = new System.Drawing.Size(300, 300);
        //        axControl.Dock = DockStyle.Fill;

        //        // 폼에 컨트롤 추가
        //        this.Controls.Add(axControl);

        //    }
        //    catch (Exception ex)
        //    {
        //        //UserControl의 Controls 컬렉션에서도 제거합니다.
        //        if (this.Controls.Contains(this.axControl))
        //        {
        //            this.Controls.Remove(this.axControl);
        //        }
        //        if (this.axControl != null)
        //        {
        //            // AxHost 컨트롤은 Dispose() 메서드를 통해 해제해야 합니다.
        //            this.axControl.Dispose();
        //            this.axControl = null;
        //        }

        //        string message = "ActiveX object Load Fail" + ex.Message;

        //        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //    }
        //}

        //private string GetCLSIDFromProgID(string progID)
        //{
        //    try
        //    {
        //        using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(progID + @"\CLSID"))
        //        {
        //            if (key != null)
        //            {
        //                object clsidValue = key.GetValue("");
        //                if (clsidValue != null)
        //                {
        //                    return clsidValue.ToString();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error retrieving CLSID: " + ex.Message);
        //    }
        //    return null;
        //}

        
        //20240703 PSU MessageBox >> MessageShow, determine 64/32, dll fileExist.
        private void LoadActiveXControl(string progID)
        {
            try
            {
                // ProgID로부터 CLSID 검색
                string clsid = GetCLSIDFromProgID(progID);
                if (string.IsNullOrEmpty(clsid))
                {
                    throw new Exception("CLSID not found for ProgID: " + progID);
                }

                // InprocServer32 경로 확인
                string inprocServer32Path = GetInprocServer32Path(clsid);
                if (string.IsNullOrEmpty(inprocServer32Path) || !File.Exists(inprocServer32Path))
                {
                    throw new FileNotFoundException("InprocServer32 path is invalid or the file does not exist: " + inprocServer32Path);
                }

                // 커스텀 AxHost 클래스를 사용하여 인스턴스 생성
                axControl = new MyAxHost(clsid);

                // 컨트롤의 위치 및 크기를 설정
                axControl.Location = new System.Drawing.Point(10, 10);
                axControl.Size = new System.Drawing.Size(300, 300);
                axControl.Dock = DockStyle.Fill;

                // 폼에 컨트롤 추가
                this.Controls.Add(axControl);

            }
            catch (Exception ex)
            {
                //UserControl의 Controls 컬렉션에서도 제거합니다.
                if (this.Controls.Contains(this.axControl))
                {
                    this.Controls.Remove(this.axControl);
                }
                if (this.axControl != null)
                {
                    // AxHost 컨트롤은 Dispose() 메서드를 통해 해제해야 합니다.
                    this.axControl.Dispose();
                    this.axControl = null;
                }

                string message = "VLC ActiveX object Load Fail. " + ex.Message;

                //MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                MessageDisplay.Show(message);
                bLoadFailed = true;


            }
        }

        private string GetCLSIDFromProgID(string progID)
        {
            try
            {
                // 운영체제의 비트를 확인하여 레지스트리 경로 결정
                bool is64Bit = Is64BitOperatingSystem();

                // 32비트 시스템이거나 32비트 애플리케이션에서 실행 중인 경우
                if (!is64Bit)
                {
                    using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(progID + @"\CLSID"))
                    {
                        if (key != null)
                        {
                            object clsidValue = key.GetValue("");
                            if (clsidValue != null)
                            {
                                return clsidValue.ToString();
                            }
                        }
                    }
                }
                else
                {
                    // 64비트 시스템에서 32비트 애플리케이션이 실행 중인 경우
                    using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"WOW6432Node\" + progID + @"\CLSID"))
                    {
                        if (key != null)
                        {
                            object clsidValue = key.GetValue("");
                            if (clsidValue != null)
                            {
                                return clsidValue.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error retrieving CLSID: " + ex.Message);.
                MessageDisplay.Show("VLC ActiveX object, Error retrieving CLSID: " + ex.Message);
                bLoadFailed = true;
            }
            return null;
        }

        private string GetInprocServer32Path(string clsid)
        {
            try
            {
                // 운영체제의 비트를 확인하여 레지스트리 경로 결정
                bool is64Bit = Is64BitOperatingSystem();

                // 32비트 시스템이거나 32비트 애플리케이션에서 실행 중인 경우
                if (!is64Bit)
                {
                    using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"CLSID\" + clsid + @"\InprocServer32"))
                    {
                        if (key != null)
                        {
                            object pathValue = key.GetValue("");
                            if (pathValue != null)
                            {
                                return pathValue.ToString();
                            }
                        }
                    }
                }
                else
                {
                    // 64비트 시스템에서 32비트 애플리케이션이 실행 중인 경우
                    using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"WOW6432Node\CLSID\" + clsid + @"\InprocServer32"))
                    {
                        if (key != null)
                        {
                            object pathValue = key.GetValue("");
                            if (pathValue != null)
                            {
                                return pathValue.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error retrieving InprocServer32 path: " + ex.Message);
                MessageDisplay.Show("VLC Media Player object, Error retrieving InprocServer32 path: " + ex.Message);
                bLoadFailed = true;
            }
            return null;
        }

        private bool Is64BitOperatingSystem()
        {
            // 32비트 프로세스에서 64비트 운영체제를 확인하기 위해 P/Invoke 사용
            if (IntPtr.Size == 8) // 64비트 프로세스
            {
                return true;
            }
            else // 32비트 프로세스
            {
                // 64비트 운영체제인지 확인
                bool is64Bit = false;
                IsWow64Process(Process.GetCurrentProcess().Handle, out is64Bit);
                return is64Bit;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] out bool isWow64);


        private class MyAxHost : AxHost
        {
            public MyAxHost(string clsid)
                : base(clsid)
            {
            }
        }
        public void ToolbarOff()
        {
            InvokeToolbarOffMethod(axControl);
        }

        public void PlaylistAdd(string MRL, string options)
        {
            InvokePlaylistAddMethod(axControl, MRL, options);
        }

        public void PlaylistPlay()
        {
            InvokePlaylistPlayMethod(axControl);
        }

        public void PlaylistStop()
        {
            InvokePlaylistStopMethod(axControl);
        }
        public void PlaylistClear()
        {
            InvokePlaylistClearMethod(axControl);
        }
        public void AudioMute(int Mute)
        {
            InvokeAudioMuteMethod(axControl, Mute);
        }
        public void AudioVolume(int volume)
        {
            InvokeAudioVolumeMethod(axControl, volume);
        }
        public void AutoLoop(int loop)
        {
            InvokeAutoLoopMethod(axControl, loop);
        }

        private void InvokeToolbarOffMethod(AxHost axControl)
        {
            try
            {
                object activeXInstance = axControl.GetOcx();
                Type comType = activeXInstance.GetType();
                comType.InvokeMember("Toolbar", System.Reflection.BindingFlags.SetProperty, null, activeXInstance, new object[] { false });
            }
            catch (Exception ex)
            {
                if (ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show("VLC Media Player object, Error invoking ToolbarOff method: " + ex.Message);
                }
            }
        }

        private void InvokePlaylistAddMethod(AxHost axControl, string MRL, string options)
        {
            try
            {
                object activeXInstance = axControl.GetOcx();
                Type comType = activeXInstance.GetType();
                // "playlist" 속성을 가져옵니다.
                object playlist = comType.InvokeMember("playlist", System.Reflection.BindingFlags.GetProperty, null, activeXInstance, null);
                Type playlistType = playlist.GetType();
                playlistType.InvokeMember("add", System.Reflection.BindingFlags.InvokeMethod, null, playlist, new object[] { MRL, null, options });
            }
            catch (Exception ex)
            {
                if (ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show("VLC Media Player object, Error invoking PlaylistAdd method: " + ex.Message);
                }
            }

        }
        private void InvokePlaylistPlayMethod(AxHost axControl)
        {
            try
            {
                object activeXInstance = axControl.GetOcx();
                Type comType = activeXInstance.GetType();
                object playlist = comType.InvokeMember("playlist", System.Reflection.BindingFlags.GetProperty, null, activeXInstance, null);
                Type playlistType = playlist.GetType();
                playlistType.InvokeMember("play", System.Reflection.BindingFlags.InvokeMethod, null, playlist, new object[] { });
            }
            catch (Exception ex)
            {
                if (ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show("VLC Media Player object, Error invoking PlaylistPlay method: " + ex.Message);
                }
            }
        }
        private void InvokePlaylistStopMethod(AxHost axControl)
        {
            try
            {
                object activeXInstance = axControl.GetOcx();
                Type comType = activeXInstance.GetType();
                object playlist = comType.InvokeMember("playlist", System.Reflection.BindingFlags.GetProperty, null, activeXInstance, null);
                Type playlistType = playlist.GetType();
                playlistType.InvokeMember("stop", System.Reflection.BindingFlags.InvokeMethod, null, playlist, new object[] { });
            }
            catch (Exception ex)
            {
                if (ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show("VLC Media Player object, Error invoking PlaylistStop method: " + ex.Message);
                }
            }
        }
        private void InvokePlaylistClearMethod(AxHost axControl)
        {
            try
            {
                object activeXInstance = axControl.GetOcx();
                Type comType = activeXInstance.GetType();
                object playlist = comType.InvokeMember("playlist", System.Reflection.BindingFlags.GetProperty, null, activeXInstance, null);
                Type playlistType = playlist.GetType();
                playlistType.InvokeMember("clear", System.Reflection.BindingFlags.InvokeMethod, null, playlist, new object[] { });
            }
            catch (Exception ex)
            {
                if (ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show("VLC Media Player object, Error invoking PlaylistClear method: " + ex.Message);
                }
            }
        }
        private void InvokeAudioMuteMethod(AxHost axControl, int Mute)
        {
            try
            {
                bool bMute = false;
                if (Mute != 0) bMute = true;

                object activeXInstance = axControl.GetOcx();
                Type comType = activeXInstance.GetType();
                object audio = comType.InvokeMember("audio", System.Reflection.BindingFlags.GetProperty, null, activeXInstance, null);
                Type audioType = audio.GetType();
                audioType.InvokeMember("mute", System.Reflection.BindingFlags.SetProperty, null, audio, new object[] { bMute });
            }
            catch (Exception ex)
            {
                if (ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show("VLC Media Player object, Error invoking AudioMute method: " + ex.Message);
                }
            }
        }
 
        private void InvokeAudioVolumeMethod(AxHost axControl, int volume)
        {
            try
            {
                if (volume > 100) volume = 100;
                else if (volume < 0) volume = 0;

                object activeXInstance = axControl.GetOcx();
                Type comType = activeXInstance.GetType();
                object audio = comType.InvokeMember("audio", System.Reflection.BindingFlags.GetProperty, null, activeXInstance, null);
                Type audioType = audio.GetType();
                audioType.InvokeMember("volume", System.Reflection.BindingFlags.SetProperty, null, audio, new object[] { volume });
            }
            catch (Exception ex)
            {
                if (ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show("VLC Media Player object, Error invoking AudioVolume method: " + ex.Message);
                }
            }
        }
        private void InvokeAutoLoopMethod(AxHost axControl, int loop)
        {
            try
            {
                bool bAutoloop = false;
                if (loop != 0) bAutoloop = true;

                object activeXInstance = axControl.GetOcx();
                Type comType = activeXInstance.GetType();
                comType.InvokeMember("AutoLoop", System.Reflection.BindingFlags.SetProperty, null, activeXInstance, new object[] { bAutoloop });
            }
            catch (Exception ex)
            {
                if (ConfigViewMain.bScriptErrorMessageShow)
                {
                    MessageDisplay.Show("VLC Media Player object, Error invoking AutoLoop method: " + ex.Message);
                }
            }
        }
    }
}

//axVLCPlugin21.playlist.add("file:///C:/AutoBase/AVANTE.mp4", "rtsp", "0");
//axVLCPlugin21.playlist.add("https://www.autobase.biz/images/main/slider2.png", "rtsp", "0");

//        private void PlayRTSP()
//        {
//            axVLCPlugin21.playlist.stop();
//            axVLCPlugin21.playlist.clear();
//            string rtspUrl = "rtsp://210.99.70.120:1935/live/cctv003.stream";
//            string[] rtspOptions = { ":network-caching=1000", ":rtsp-tcp" };
//            //rtsp-tcp 옵션이 있으면 로딩이 더 빠르다.
//            axVLCPlugin21.playlist.add(rtspUrl, null, rtspOptions);
//            axVLCPlugin21.playlist.play();
//        }

//    }
//}
