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
using System.Threading.Tasks;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Linq;
using AutoLib;

namespace GraphicModule
{
    [Serializable]
    public class ObjectArgsWebView
    {
        public string url;
    }

    [Serializable]
    public class ObjectWebView : ObjectExpand
    {
        ObjectArgsWebView objArgs;

        [NonSerialized]
        private WebView2 wndChild; // 20250918 PSU NuGet WebView2 컨트롤로 변경

        [NonSerialized]
        Form formParent; // 20250312 PSU

        [NonSerialized]
        static public List<ObjectExpand> arrayClassList = new List<ObjectExpand>();

        [NonSerialized]
        private bool isInitialized = false; // 초기화 상태 추적

        [NonSerialized]
        private CoreWebView2Environment webViewEnvironment; // WebView2 환경 객체

        public ObjectArgsWebView ObjectArgs
        {
            set
            {
                objArgs = value;
            }
            get
            {
                return objArgs;
            }
        }

        public ObjectWebView(ObjectCommonProperty ocp, Form form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsWebView args)
			: base(ocp, rect, eid, lf, general)
		{
			//
			// TODO: Add constructor logic here
			//
			enumObjectType = EnumObjectType.WebView;
            bSupportObjectOnCE = false;
			objArgs = args;

            formParent = form; //20250312 PSU

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {      
                InitializeWebView2Async();
            }	
		}

        private async void InitializeWebView2Async()
        {
            try
            {
                wndChild = new WebView2();
                wndChild.Name = objGeneral.sClassName;

                formParent.Controls.Add(wndChild);

                int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                GetViewZone(ref x1, ref y1, ref x2, ref y2);
                if (x1 > x2) Tools.Temp(ref x1, ref x2);
                if (y1 > y2) Tools.Temp(ref y1, ref y2);
                wndChild.Left = x1;
                wndChild.Top = y1;
                wndChild.Width = x2 - x1;
                wndChild.Height = y2 - y1;

                // 사용자 데이터 폴더를 지정하여 WebView2 환경 생성
                string userDataFolder = System.IO.Path.Combine(Application.StartupPath, "LocalMain.exe.WebView2");
                webViewEnvironment = await CoreWebView2Environment.CreateAsync(null, userDataFolder, null);
                await wndChild.EnsureCoreWebView2Async(webViewEnvironment);

                string localFolderPath;
                // 로컬 폴더를 가상 호스트로 매핑
                if (ConfigVarTotal.bLocalFlag)
                {
                    localFolderPath = Path.Combine(TotalConfig.sDirWorkProject, "webView");
                    // 폴더가 없으면 생성
                    if (!Directory.Exists(localFolderPath))
                    {
                        Directory.CreateDirectory(localFolderPath);
                    }

                    // Local 에서만 가상호스팅 사용 가능. Web Client에서는 IIS 호스팅 Url 사용.
                    wndChild.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    "app.local",
                    localFolderPath,
                    CoreWebView2HostResourceAccessKind.Allow);
                }
                else
                {              

                }

              

                // webview 설정
                ConfigureWebView();

                wndChild.Show();
                arrayClassList.Add(this);

                var toolTipTask = base.SetToolTipOnChildWindow(wndChild);
                OnVisible(ExpandCalcVisible());

                isInitialized = true;

                // URL 탐색 - Virtual Host 사용
                if (!string.IsNullOrEmpty(objArgs.url))
                {
                    string navigateUrl = ConvertToVirtualHostUrl(objArgs.url);
                    wndChild.CoreWebView2.Navigate(navigateUrl);
                }
            }
            catch (Exception ex)
            {
                if (wndChild != null)
                {
                    // 폼의 컨트롤 목록에서도 제거
                    if (formParent.Controls.Contains(wndChild))
                    {
                        formParent.Controls.Remove(wndChild);
                    }
                    // WebView2 컨트롤 해제 및 관련 리소스 정리
                    wndChild.Dispose();
                    wndChild = null;
                }

                //string message = "An error occurred while initializing the WebView2 control. Please ensure that the WebView2 Runtime is installed.\n\n" +
                // "Would you like to go to the download link below?\n\n" +
                // "WebView2 Runtime: https://developer.microsoft.com/microsoft-edge/webview2\n\n" + ex.Message;

                //DialogResult result = MessageBox.Show(message, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                //if (result == DialogResult.Yes)
                //{
                //    Process.Start(new ProcessStartInfo("https://developer.microsoft.com/microsoft-edge/webview2") { UseShellExecute = true });
                //}

                string message = Tools.IsLangKorean()
                ? $"WebView2 컨트롤 초기화 중 오류가 발생했습니다.\n\n{ex.Message}"
                : $"An error occurred while initializing the WebView2 control.\n\n{ex.Message}";

                MessageDisplay.Show(message);
            }
        }

        // URL 변환 헬퍼 메서드 추가
        private string ConvertToVirtualHostUrl(string url)
        {
            // 이미 http/https URL이면 그대로 반환
            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                return url;
            }

            // --------- ① 로컬 모드일 경우 : Virtual Host 사용 ---------
            if (ConfigVarTotal.bLocalFlag)
            {
                // file:// 제거
                string cleanUrl = url.Replace("file:///", "").Replace("file://", "");

                // 백슬래시를 슬래시로 변경
                cleanUrl = cleanUrl.Replace("\\", "/");

                // webView 폴더 이후 경로 추출
                if (cleanUrl.Contains("webView/"))
                {
                    string relativePath = cleanUrl.Substring(cleanUrl.IndexOf("webView/") + 8);
                    return "https://app.local/" + relativePath;
                }

                // 상대 경로인 경우
                return "https://app.local/" + cleanUrl;
            }

            // --------- ② 서버 모드(!bLocalFlag)일 경우 : IIS URL로 변환 ---------
            // url = ConfigVarTotal.MakeRootUrl()  (예: http://192.168.0.50:8080)
            string root = ConfigVarTotal.MakeRootUrl();

            // root 끝에 / 없게 통일
            if (root.EndsWith("/"))
                root = root.TrimEnd('/');

            // 전달된 url이 상대경로라면 서버 URL 형태로 조합
            // 예: "index.html" → "http://server:port/AutoWeb/Project/webView/index.html"
            string finalUrl = $"{root}/AutoWeb/Project/webView";

            // clean up
            string rel = url
                .Replace("file:///", "")
                .Replace("file://", "")
                .Replace("\\", "/");

            // 상대 파일 경로 붙이기
            if (!string.IsNullOrEmpty(rel))
                finalUrl += "/" + rel.TrimStart('/');

            return finalUrl;
        }

        public override void Close()
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				arrayClassList.Remove(this);

                // WebView2 리소스 정리
                if (wndChild != null)
                {
                    wndChild.Dispose();
                    wndChild = null;
                }
            }
		}

		public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) 
			{
				if(x1 > x2)	Tools.Temp(ref x1, ref x2);
				if(y1 > y2)	Tools.Temp(ref y1, ref y2);

				Font font = MakeFont();

				int cyChar = (int)font.GetHeight()+1;
				int cxChar = (int)(font.GetHeight()/2);

				RECT r = new RECT();

				DrawClass.PopBox2(g, x1, y1, x2, y2, Color.LightGray);

				StringFormat format = new StringFormat();
				format.Alignment = StringAlignment.Center;
				format.LineAlignment = StringAlignment.Center;
				//format.FormatFlags |= StringFormatFlags.NoWrap;
				Brush brush = Brushes.Black;

                r.left = x1;
                r.top = y1;
                r.right = x2;
                r.bottom = y2;
           
                DrawClass.DrawText(g, this.objArgs.url, font, brush, r, format);

                //string str = objGeneral.sClassName;                
                //DrawClass.DrawText(g, str, font, brush, r, format);
			}

            else if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {

                UpdateControlState(wndChild, formParent);//20250312 PSU
            }
		}

 
		public override void OnMove(int x1, int y1, int x2, int y2)
		{
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				if(wndChild != null) 
				{
					//Font font = MakeFont();
					//wndChild.Font = font;  //readonly

					wndChild.Left = x1;
					wndChild.Top = y1;
					wndChild.Width = x2-x1;
					wndChild.Height = y2-y1;
				}
			}
		}


        private void ConfigureWebView()
        {
            if (wndChild?.CoreWebView2 != null)
            {
                // 미디어 자동 재생 허용
                wndChild.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;
                wndChild.CoreWebView2.Settings.AreHostObjectsAllowed = true;
                wndChild.CoreWebView2.Settings.IsWebMessageEnabled = true;
                wndChild.CoreWebView2.Settings.AreDevToolsEnabled = true; // 필요시 true로 설정

                wndChild.CoreWebView2.Settings.IsStatusBarEnabled = false;

                // 권한 요청 처리 (마이크, 카메라 등)
                wndChild.CoreWebView2.PermissionRequested += (sender, e) =>
                {
                    // 위치 정보 권한은 브라우저 허용
                    if (e.PermissionKind == CoreWebView2PermissionKind.Geolocation)
                    {
                        // Default 상태로 두면 브라우저가 기본 팝업을 표시함 >> 유튜브 embed 창에선 표시가 안되서 허용으로 변경.
                        e.State = CoreWebView2PermissionState.Allow;
                    }
                    // 다른 미디어 권한들도  허용
                    else if (e.PermissionKind == CoreWebView2PermissionKind.Microphone ||
                             e.PermissionKind == CoreWebView2PermissionKind.Camera)
                    {
                        e.State = CoreWebView2PermissionState.Allow;
                    }
                    // 자동재생, 알림, 파일 접근은 자동 허용
                    else if (e.PermissionKind == CoreWebView2PermissionKind.Autoplay ||
                             e.PermissionKind == CoreWebView2PermissionKind.Notifications ||
                             e.PermissionKind == CoreWebView2PermissionKind.FileReadWrite)
                    {
                        e.State = CoreWebView2PermissionState.Allow;
                    }
                };
            }
        }

        public override async Task<object> ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            //await Task.Yield(); // 경고 해결용

            if (command == "WebViewNavigate")
            {
                if (wndChild != null && isInitialized)
                {
                    string url = (string)args[1];

                    // WebView2가 초기화되었는지 확인
                    if (wndChild.CoreWebView2 != null)
                    {
                        string navigateUrl = ConvertToVirtualHostUrl((string)args[1]);
                        wndChild.CoreWebView2.Navigate(navigateUrl);
                    }
                    else
                    {
                        // 초기화가 완료될 때까지 대기 후 탐색
                        if (webViewEnvironment != null)
                        {
                            // 기존에 생성된 환경 사용
                            await wndChild.EnsureCoreWebView2Async(webViewEnvironment);
                        }
                        else
                        {
                            // 환경이 없으면 새로 생성
                            string userDataFolder = System.IO.Path.Combine(Application.StartupPath, "LocalMain.exe.WebView2");
                            webViewEnvironment = await CoreWebView2Environment.CreateAsync(null, userDataFolder, null);
                            await wndChild.EnsureCoreWebView2Async(webViewEnvironment);
                        }

                        string navigateUrl = ConvertToVirtualHostUrl((string)args[1]);
                        wndChild.CoreWebView2.Navigate(navigateUrl);
                    }
                }
                return 1;
            }
            else if (command == "WebViewRunScript")
            {
                return await ExecuteScriptAsync((string)args[1]);
            }
            else if (command == "WebViewRefresh")
            {
                RefreshPage();
                return 1;
            }
            else if (command == "WebViewGoBack")
            {
                GoBack();
                return 1;
            }
            else if (command == "WebViewGoForward")
            {
                GoForward();
                return 1;
            }
            else if (command == "WebViewClearData")
            {
                await ClearAllUserDataAsync();
                return 1;
            }
            return 0;
        }


        // 페이지 새로고침
        public void RefreshPage()
        {
            if (wndChild?.CoreWebView2 != null)
            {
                wndChild.CoreWebView2.Reload();
            }
        }

        // 뒤로 가기
        public void GoBack()
        {
            if (wndChild?.CoreWebView2 != null && wndChild.CoreWebView2.CanGoBack)
            {
                wndChild.CoreWebView2.GoBack();
            }
        }

        // 앞으로 가기
        public void GoForward()
        {
            if (wndChild?.CoreWebView2 != null && wndChild.CoreWebView2.CanGoForward)
            {
                wndChild.CoreWebView2.GoForward();
            }
        }

        // 사용자 데이터 초기화 (쿠키, 권한 등 모두 삭제)
        public async Task ClearAllUserDataAsync()
        {
            if (wndChild?.CoreWebView2 != null)
            {
                try
                {
                    // 모든 브라우저 데이터 삭제
                    await wndChild.CoreWebView2.Profile.ClearBrowsingDataAsync();
                    if(Tools.IsLangKorean())
                    MessageBox.Show("사용자 데이터가 초기화되었습니다.\n페이지를 새로고침하면 권한 팝업이 다시 표시됩니다.", "데이터 초기화");
                    else MessageBox.Show("User data has been cleared.\nRefresh the page to see permission prompts again.", "Data Cleared");
                }
                catch (Exception ex)
                {
                    if (Tools.IsLangKorean())
                        MessageBox.Show($"데이터 초기화 오류: {ex.Message}", "오류");
                    else MessageBox.Show($"Data clear error: {ex.Message}", "Error");
                }
            }
        }

        public async Task<string> ExecuteScriptAsync(string script)
        {
            if (wndChild != null && isInitialized && wndChild.CoreWebView2 != null)
            {
                return await wndChild.CoreWebView2.ExecuteScriptAsync(script);
            }
            return string.Empty;
        }


        // 사용자 데이터 초기화 (쿠키, 캐시 등 삭제)
        public async Task ClearUserDataAsync()
        {
            if (wndChild?.CoreWebView2 != null)
            {
                // 브라우저 데이터 삭제
                await wndChild.CoreWebView2.Profile.ClearBrowsingDataAsync();
            }
        }


        // 추가: 페이지 로드 완료 이벤트 처리 (필요시 사용)
        private void SetupWebViewEvents()
        {
            if (wndChild?.CoreWebView2 != null)
            {
                wndChild.CoreWebView2.NavigationCompleted += (sender, e) =>
                {
                    // 페이지 로드 완료 시 처리할 로직
                };

                wndChild.CoreWebView2.DocumentTitleChanged += (sender, e) =>
                {
                    // 문서 제목 변경 시 처리할 로직
                };
            }
        }

        public override void ObjectSave(CommaTextWriter writer)
		{
			ObjectSaveFont(writer);

			writer.Write("\tStringOption,");
			writer.Write("{0},", objArgs.url);

			writer.WriteLine();
		}

        public override void OnVisible(bool flag)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                this.wndChild.Visible = flag;
            }

        }
    }
}
