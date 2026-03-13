using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using AutoLib;
using System.Windows.Browser;

namespace SilverlightGraphicModule
{
    public partial class Page : UserControl
    {
        public ObjectRoot obj;
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        
        double fOpticX = 1.0;
        double fOpticY = 1.0;

        public event EventHandler eventHandlerModuleLoadComplete;
        public event EventHandler eventHandlerModuleClose;

        public Page()
        {
            InitializeComponent();

            timer.Interval = new TimeSpan(1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            this.LayoutRoot.MouseLeftButtonDown += new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonDown);
            this.LayoutRoot.MouseLeftButtonUp += new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonUp);
            this.LayoutRoot.MouseMove += new MouseEventHandler(LayoutRoot_MouseMove);
            
            //this.gridBackground.MouseLeftButtonDown += new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonDown);
            //this.gridBackground.MouseLeftButtonUp += new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonUp);
            //this.gridBackground.MouseMove += new MouseEventHandler(LayoutRoot_MouseMove);

            this.SizeChanged += new SizeChangedEventHandler(Page_SizeChanged);

            this.Width = Double.NaN;        // 자동 사이즈로 만든다.
            this.Height = Double.NaN;       // 자동 사이즈로 만든다.

            GraphicTool.RegisterPage(this);
        }
            
        public void Load(string mod_file, int optic_method)
        {
            this.LayoutRoot.Background = null;  // 초기 로딩 시 화면 깜박임을 방지하기 위해서

            //mod_file = System.Windows.Browser.HttpUtility.UrlEncode(mod_file);
            mod_file = System.Uri.EscapeDataString(mod_file);

            string filename = MakeFilePath.Graphic(mod_file);

            filename = MakeFilePath.MakePublishTextPath(filename);

            obj = new ObjectRoot();

            obj.procOnModuleReadComplete = new ObjectRoot.DelegateOnModuleReadComplete(OnModuleReadComplete);

            obj.Load(this, filename);

            nSaveOpticMethod = optic_method;
        }

        void PlayScriptWhenModStartEnd(string when)
        {
            ScriptClass control;

            if (String.Compare(when, "ModStart") == 0)
                control = this.obj.scriptModuleStart;
            else if (String.Compare(when, "ModEnd") == 0)
                control = this.obj.scriptModuleEnd;
            else
                control = null;

            if (control == null) return;

            control.Run(this);

            if (control.IsError())
            {
                string message;
                message = control.GetError();
                MessageBox.Show(message, when + " Script", MessageBoxButton.OK);
            }
        }

        // 같은 페이지에 덮어 쓸 때 사용한다. 이렇게 하면 익스플로어 네비게이션이 되지 않는다.
        public void ReLoad(string mod_file)
        {
            this.LayoutRoot.Children.Clear();
            this.LayoutRoot.Background = null;  // 초기 로딩 시 화면 깜박임을 방지하기 위해서

            //mod_file = System.Windows.Browser.HttpUtility.UrlEncode(mod_file);
            mod_file = System.Uri.EscapeDataString(mod_file);

            string filename = MakeFilePath.Graphic(mod_file);

            obj = new ObjectRoot();

            obj.procOnModuleReadComplete = new ObjectRoot.DelegateOnModuleReadComplete(OnModuleReadComplete);

            obj.Load(this, filename);
        }

        int nSaveOpticMethod;

        bool bModuleLoaded = false;

        // 모듈을 다 읽고 나면 확대/축소 모드를 다시 정리한다.
        void OnModuleReadComplete()
        {
            bModuleLoaded = true;

            // 모듈을 다 읽고 난 후 읽는 중간에 설정된 모듈의 확대/ 축소 모드를 다시 정리한다.
            if (nSaveOpticMethod != -1)         // 파일의 정보를 사용한다.
            {
                obj.SetObjectOpticMethod(nSaveOpticMethod);
            }

            ScrollUpdate();
            PlayScriptWhenModStartEnd("ModStart");

            if (eventHandlerModuleLoadComplete != null)
                eventHandlerModuleLoadComplete(this, EventArgs.Empty);
        }

        void ScrollUpdate()
        {
            if (!bModuleLoaded) return; // 페이지 로딩이 Async이므로 로딩된 다음에 스크롤이나 확대/축소를 정리한다. 

            if (obj.cObjectOpticMethod == 1)
            {
                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

                fOpticX = this.ActualWidth / obj.nModuleSizeX;
                fOpticY = this.ActualHeight / obj.nModuleSizeY;

                if (fOpticX < fOpticY) fOpticY = fOpticX;
                else
                {
                    fOpticX = fOpticY;
                }

                ScaleTransform transform = new ScaleTransform();
                transform.ScaleX = fOpticX;
                transform.ScaleY = fOpticY;
                this.LayoutRoot.RenderTransform = transform;

                this.scrollViewer.Width = obj.nModuleSizeX * fOpticX;
                this.scrollViewer.Height = obj.nModuleSizeY * fOpticY;

                if (fOpticX > 1) this.LayoutRoot.Width = obj.nModuleSizeX * fOpticX;
                else this.LayoutRoot.Width = obj.nModuleSizeX / fOpticX;

                if (fOpticY > 1) this.LayoutRoot.Height = obj.nModuleSizeY * fOpticY;
                else this.LayoutRoot.Height = obj.nModuleSizeY / fOpticY;

                // 10.1.2 까지는 이것을 사용했다. Canvas 가 축소되었을 때는 축소된 만큼 마우스 영역이 좁아져서 오른쪽/아래 위치에 마우스 응답이 없어서 scrollViewer를 맞는 크기로 하고 속에 있는 Canvas는 그 크키를 따라갔다.
                //this.LayoutRoot.Width = obj.nModuleSizeX * fOpticX;
                //this.LayoutRoot.Height = obj.nModuleSizeY * fOpticY;
            }
            else if (obj.cObjectOpticMethod == 2)
            {
                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

                fOpticX = this.ActualWidth / obj.nModuleSizeX;
                fOpticY = this.ActualHeight / obj.nModuleSizeY;

                ScaleTransform transform = new ScaleTransform();
                transform.ScaleX = fOpticX;
                transform.ScaleY = fOpticY;

                this.LayoutRoot.RenderTransform = transform;

                //this.LayoutRoot.Width = obj.nModuleSizeX * fOpticX;
                //this.LayoutRoot.Height = obj.nModuleSizeY * fOpticY;

                this.scrollViewer.Width = obj.nModuleSizeX * fOpticX;
                this.scrollViewer.Height = obj.nModuleSizeY * fOpticY;

                if (fOpticX > 1) this.LayoutRoot.Width = obj.nModuleSizeX * fOpticX;
                else this.LayoutRoot.Width = obj.nModuleSizeX / fOpticX;

                if (fOpticY > 1) this.LayoutRoot.Height = obj.nModuleSizeY * fOpticY;
                else this.LayoutRoot.Height = obj.nModuleSizeY / fOpticY;
            }
            else
            {
                ScaleTransform transform = new ScaleTransform();
                transform.ScaleX = fOpticX;
                transform.ScaleY = fOpticY;
                this.LayoutRoot.RenderTransform = transform;

                this.LayoutRoot.Width = obj.nModuleSizeX * fOpticX;
                this.LayoutRoot.Height = obj.nModuleSizeY * fOpticY;

                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
        }

        void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //throw new NotImplementedException();
            //this.LayoutRoot.Width = this.Width;
            //this.LayoutRoot.Height = this.Height;

            ScrollUpdate();
        }

        void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //throw new NotImplementedException();
            obj.WmLeftButtonDown(this, e);
        }

        void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // throw new NotImplementedException();
            obj.WmLeftButtonUp(this, e);
        }

        void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
        {
            // throw new NotImplementedException();
            obj.WmMouseMove(this, e);
        }

        bool timer_flag;
        void timer_Tick(object sender, EventArgs e)
        {
            timer_flag = !timer_flag;

            if (!timer_flag) return;

            timer.Stop();
            obj.EventTimer(this);
            ScriptFunctionElse.PopupMessageTimer();
            //ThreadDataChange.TimerMain(); App에서 한다.
            //ModuleScriptAlwaysTimer();    모듈실행시 스크립트는 10.2 까지도 지원되지 않아서 지원하려고 했지만 어떤 명령이 있을지 모르기 때문에 일단 보류했다.
            timer.Start();
        }

        // 일단 지원 보류
        void ModuleScriptAlwaysTimer()
        {
            ScriptClass script = obj.scriptModuleAlways;

            if (script == null) return;

            if (script.IsError()) return;

            script.Run(this);

            if (script.IsError())
            {
                string message;
                string title;
                message = script.GetError();
                if (NetTools.Tools.IsLangKorean())
                {
                    title = String.Format("Module Script Always에서 오류");
                }
                else
                {
                    title = String.Format("Module Script Always Error");
                }
                MessageBox.Show(title, message, MessageBoxButton.OK);
                return;
            }
        }

        public void Close()
        {
            if (eventHandlerModuleClose != null)
                eventHandlerModuleClose(this, EventArgs.Empty);
        }

        private void ContentPresenter_MouseEnter(object sender, MouseEventArgs e)
        {
            //((Storyboard)FindName("MouseEnterState")).Begin();
        }

        private void ContentPresenter_MouseLeave(object sender, MouseEventArgs e)
        {
            //((Storyboard)FindName("MouseLeaveState")).Begin();
        }
    }
}
