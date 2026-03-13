using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using NetTools.OldDefine;
using System.Drawing;
using NetTools;
using AutoLibLocal;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;


namespace GraphicModule
{
    public partial class WebView2ActiveXControl : UserControl
    {
        private MyAxHost axControl;

        public WebView2ActiveXControl()
        {
            {
                InitializeComponent();
                LoadActiveXControl("Autobase.WebView2Ctrl.1");
            }
        }

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

                string message = "An error occurred while initializing the WebView2ActiveX control. Please ensure that the WebView2 components are installed.\n\n" +
                 "Would you like to go to the download links below?\n\n" +
                 "1. ActiveX Control: https://autobase.biz/Korean/Download/WebView2ActiveX.html" +
                 "2. WebView2 Runtime: https://developer.microsoft.com/microsoft-edge/webview2\n\n" + ex.Message;

                DialogResult result = MessageBox.Show(message, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo("https://autobase.biz/Korean/Download/WebView2ActiveX.html") { UseShellExecute = true });
                    Process.Start(new ProcessStartInfo("https://developer.microsoft.com/microsoft-edge/webview2") { UseShellExecute = true });
                }
            }
        }

        private string GetCLSIDFromProgID(string progID)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving CLSID: " + ex.Message);
            }
            return null;
        }

        private class MyAxHost : AxHost
        {
            public MyAxHost(string clsid)
                : base(clsid)
            {
            }
        }

        public void Navigate(string url)
        {
            if (axControl != null)
            {
                InvokeNavigateMethod(axControl, url);
            }
        }

        private void InvokeNavigateMethod(AxHost axControl, string url)
        {
            try
            {
                // ActiveX 컨트롤의 Navigate 메서드 호출
                object activeXInstance = axControl.GetOcx();
                Type comType = activeXInstance.GetType();
                comType.InvokeMember("Navigate", System.Reflection.BindingFlags.InvokeMethod, null, activeXInstance, new object[] { url });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error invoking Navigate method: " + ex.Message);
            }
        }
    }
}
