using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace ScriptLibEdit.Editor
{
    public partial class UserControlScriptEditor : UserControl
    {
        public HScrollBar hScrollBar1 = new HScrollBar();
        public VScrollBar vScrollBar1 = new VScrollBar();
        //public TextArea panelEditor;

        // panelEditor: IScriptEditorPanel 인터페이스 + Control 기반 컨트롤
        // 소비자 코드(FormNewScriptEditor 등)에서 직접 접근하므로 IScriptEditorPanel 타입으로 노출.
        public IScriptEditorPanel panelEditor;

        // 내부 Control 참조 (panelEditor가 Control이므로)
        private Control panelEditorControl;

        private bool useMonaco = false;

        public UserControlScriptEditor()
        {
            // panelEditor = new TextArea(this);   // 2013-10-2 InitializeComponent()다음에 있으니 윈도우즈 7에서 바탕 텍스트를 중간으로 바꾸면 sizechanged가 초기하전에 먼저 발생한다.

            ConfigScriptEditor.LoadConfig();
            useMonaco = ConfigScriptEditor.bUseMonacoEditor;

            if (useMonaco)
            {
                var monaco = new MonacoEditorBridge(this);
                panelEditor = monaco;
                panelEditorControl = monaco;
            }
            else
            {
                var textArea = new TextArea(this);
                panelEditor = textArea;
                panelEditorControl = textArea;
            }

            InitializeComponent();

            Controls.Add(panelEditorControl);

            if (!useMonaco)
            {
                // 레거시 TextArea일 때만 스크롤바 사용
                Controls.Add(vScrollBar1);
                Controls.Add(hScrollBar1);

                vScrollBar1.Scroll += new ScrollEventHandler(vScrollBar1_Scroll);
                hScrollBar1.Scroll += new ScrollEventHandler(hScrollBar1_Scroll);
            }

            if (!useMonaco)
            {
                //20250331 PSU 특정 설정(명확하지 않음) 에서 스크롤바 잔상 증상 해결.
                // Monaco(WebView2) 모드에서는 이 스타일이 자식 컨트롤 렌더링을 막으므로 적용하지 않는다.
                SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw |
                 ControlStyles.UserPaint, true);
                this.DoubleBuffered = true;
                UpdateStyles();
            }
        }

        void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UserControlScriptEditor_Load(object sender, EventArgs e)
        {
            OnSize();

            if (!useMonaco)
            {
                this.timer1.Enabled = true;
            }
        }
        
        bool bTimerFlag;

        private void timer1_Tick(object sender, EventArgs e)
        {
            bTimerFlag = !bTimerFlag;

            if (bTimerFlag) return; // 둘중에 한번만

            panelEditor.OnTimer();
        }

        public void OnSize()
        {
            if (useMonaco)
            {
                // Monaco는 Dock.Fill로 전체를 채운다
                panelEditorControl.Left = 0;
                panelEditorControl.Top = 0;
                panelEditorControl.Width = this.ClientRectangle.Width;
                panelEditorControl.Height = this.ClientRectangle.Height;
                panelEditor.OnSize();
            }
            else
            {
                // 레거시 TextArea + 스크롤바 레이아웃
                this.vScrollBar1.Left = this.ClientRectangle.Right - this.vScrollBar1.Width;
                this.vScrollBar1.Top = 0;
                this.vScrollBar1.Height = this.ClientRectangle.Height - this.hScrollBar1.Height;

                this.hScrollBar1.Left = 0;
                this.hScrollBar1.Top = this.ClientRectangle.Bottom - hScrollBar1.Height;
                this.hScrollBar1.Width = this.ClientRectangle.Right - vScrollBar1.Width;

                panelEditorControl.Left = 0;
                panelEditorControl.Top = 0;
                panelEditorControl.Width = this.ClientRectangle.Right - vScrollBar1.Width;
                panelEditorControl.Height = this.ClientRectangle.Bottom - hScrollBar1.Height;

                // 스크롤바 강제 새로고침
                this.vScrollBar1.Refresh();
                this.hScrollBar1.Refresh();

                panelEditor.OnSize();

                this.vScrollBar1.Minimum = 0;
                this.vScrollBar1.Maximum = this.vScrollBar1.LargeChange - 1 + panelEditor.arrayString.Count;
            }
        }

        private void UserControlScriptEditor_SizeChanged(object sender, EventArgs e)
        {
            OnSize();
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (panelEditor is TextArea textArea)
            {
                textArea.nPageY = e.NewValue;
                textArea.Invalidate();
            }
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (panelEditor is TextArea textArea)
            {
                textArea.nPageX = e.NewValue;
                textArea.Invalidate();
            }
        }

        public void LoadFromFile(string filename)
        {
            panelEditor.LoadFromFile(filename);
            OnSize();    // 라인이 바뀌었으므로 스크롤 다시계산
        }

        public void SetSourceString(string text)
        {
            panelEditor.LoadFromString(text);
            OnSize();    // 라인이 바뀌었으므로 스크롤 다시계산
        }

        public string GetSourceString()
        {
            if (panelEditor == null) return "";
            return panelEditor.GetSourceString();
        }

        public delegate void CallBackOnEvent(EnumEditorEventType type, params object[] param);
        public CallBackOnEvent procCallBackOnEvent = null;

        public void SendEvent(EnumEditorEventType type, params object[] param)
        {
            if (procCallBackOnEvent == null) return;

            procCallBackOnEvent(type, param);
        }

        public void Copy()
        {
            panelEditor.Copy();
        }

        public void Paste()
        {
            panelEditor.Paste();
            OnSize();    // 라인이 바뀌었으므로 스크롤 다시계산
        }

        public void SetFont(Font font)
        {
            this.Font = font;
            panelEditor.SetFont(font);
        }

        // 외부에서 환경설정이 바뀌었을 때 혹시 바꿀것이 있으면 바꾸어준다.
        public void OnConfigurationChanged()
        {
            this.panelEditor.OnConfigurationChanged();
        }

        // 선택된 텍스트 가져오기
        public string GetSelectedText()
        {
            if (panelEditor == null) return "";
            return panelEditor.GetSelectedText();
        }

        // 커서 위치 가져오기 (전체 텍스트에서의 인덱스)
        public int GetCursorPosition()
        {
            if (panelEditor == null) return 0;
            return panelEditor.GetCursorPosition();
        }

        // 텍스트 선택 및 커서 이동
        public void SetSelection(int start, int length)
        {
            if (panelEditor == null) return;
            panelEditor.SetSelection(start, length);
        }

        // 커서 위치로 스크롤
        public void ScrollToCursor()
        {
            if (panelEditor == null) return;
            panelEditor.ScrollToCursor();
        }

        // 지정된 위치로 이동하고 스크롤
        public void GotoPosition(int position)
        {
            if (panelEditor == null) return;
            panelEditor.GotoPosition(position);
        }

        /// <summary>
        /// 디버그 모드 시작 (정적 메서드).
        /// TextArea.DebugStart() / MonacoEditorBridge.DebugStart() 를 대신하여
        /// 소비자 코드에서 통합적으로 호출할 수 있도록 한다.
        /// </summary>
        public static void DebugStart()
        {
            if (ConfigScriptEditor.bUseMonacoEditor)
                MonacoEditorBridge.DebugStart();
            else
                TextArea.DebugStart();
        }

        /// <summary>
        /// 태그 자동완성 데이터를 설정한다 (Monaco 전용).
        /// JSON 배열: [{"name":"TagName","type":"AI","description":"desc"}, ...]
        /// </summary>
        public void SetCompletionTags(string json)
        {
            if (panelEditor is MonacoEditorBridge monaco)
            {
                monaco.SetCompletionTags(json);
            }
        }

        /// <summary>
        /// 메서드 자동완성 데이터를 설정한다 (Monaco 전용).
        /// JSON 배열: [{"name":"MethodName","returnType":"int","signature":"MethodName(int a)"}, ...]
        /// </summary>
        public void SetCompletionMethods(string json)
        {
            if (panelEditor is MonacoEditorBridge monaco)
            {
                monaco.SetCompletionMethods(json);
            }
        }

        /// <summary>
        /// 텍스트를 현재 커서 위치에 직접 삽입한다 (Monaco 전용).
        /// </summary>
        public void InsertText(string text)
        {
            if (panelEditor is MonacoEditorBridge monaco)
            {
                monaco.InsertText(text);
            }
            else
            {
                // TextArea 폴백: 클립보드 방식
                Clipboard.SetDataObject(text);
                panelEditor.Paste();
            }
            OnSize();
        }


    }
}
