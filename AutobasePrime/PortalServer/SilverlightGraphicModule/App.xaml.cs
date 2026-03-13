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

namespace SilverlightGraphicModule
{
    public partial class App : Application
    {
        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        public static string sHostSourceUrl;
        PublicInit publicInit = new PublicInit();

        string sPageName;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            sHostSourceUrl = this.Host.Source.AbsoluteUri;

            //string pagename;

            // <param name="initParams" value="filename=iocard.modx" /> 와 같이 initParams가 Page querystring에 우선한다.
            try
            {
                string startPage = e.InitParams["filename"];
                sPageName = startPage;
            }
            catch
            {
                try
                {
                    sPageName = System.Windows.Browser.HtmlPage.Document.QueryString["filename"];
                }
                catch
                {
                    sPageName = "startup.modx";
                }
            }

            Init();
        }

        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

        void Init()
        {
            SystemValue.Init();     // SystemValueGet, Set을 위한 콜백 함수를 등록한다.

            SilverlightDialogControl.ControlBoxAnalogInputGo.procUserControl = new SilverlightDialogControl.ControlBoxAnalogInputGo.DelegateUserControl(SilverlightDialogControl.MyDialogUserControl.DisplayUserControlBox);

            AutoLibLocal.ConfigVarTotal.bLocalFlag = false;
            AutoLibLocal.ConfigVarTotal.Init(App.sHostSourceUrl);

            AutoLibLocal.TagFile.eventHandlerOnTagFileReaded += new EventHandler(TagFile_eventHandlerOnTagFileReaded);// = new AutoLibLocal.TagFile.DeleOnTagFileReaded(OnTagFileReaded);
            publicInit.Init();
        }

        void TagFile_eventHandlerOnTagFileReaded(object sender, EventArgs e)
        {
            GraphicTool.OnTagFileLoaded();

            // 태그가 로딩된 다음 그래픽 파일을 열도록 한다.
            UserControl page;

            if (String.Compare(sPageName, "BasicGraph.basic", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                page = new ControlBasicGraph();
            }
            else if (String.Compare(sPageName, "BasicTrend.basic", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                page = new ControlBasicTrend();
            }
            else
            {
                page = new PageInitialWait(sPageName);
            }

            this.RootVisual = page;

            timer.Interval = new TimeSpan(1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            ThreadDataChange.TimerMain();
        }

        void UnInit()
        {

        }

        /*
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            sHostSourceUrl = this.Host.Source.AbsoluteUri;

            string pagename;

            // <param name="initParams" value="filename=iocard.modx" /> 와 같이 initParams가 Page querystring에 우선한다.
            try
            {
                string startPage = e.InitParams["filename"];
                pagename = startPage;
            }
            catch
            {
                try
                {
                    pagename = System.Windows.Browser.HtmlPage.Document.QueryString["filename"];
                }
                catch
                {
                    pagename = "startup.modx";
                }
            }
            
            Init();

            UserControl page;

            if (String.Compare(pagename, "BasicGraph.basic", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                page = new ControlBasicGraph();
            }
            else if (String.Compare(pagename, "BasicTrend.basic", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                page = new ControlBasicTrend();
            }
            else
            {
                page = new PageInitialWait(pagename);
            }

            this.RootVisual = page;
        }

        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

        void Init()
        {
            SystemValue.Init();     // SystemValueGet, Set을 위한 콜백 함수를 등록한다.

            SilverlightDialogControl.ControlBoxAnalogInputGo.procUserControl = new SilverlightDialogControl.ControlBoxAnalogInputGo.DelegateUserControl(SilverlightDialogControl.MyDialogUserControl.DisplayUserControlBox);

            AutoLibLocal.ConfigVarTotal.bLocalFlag = false;
            AutoLibLocal.ConfigVarTotal.Init(App.sHostSourceUrl);

            AutoLibLocal.TagFile.eventHandlerOnTagFileReaded += new EventHandler(TagFile_eventHandlerOnTagFileReaded);// = new AutoLibLocal.TagFile.DeleOnTagFileReaded(OnTagFileReaded);
            publicInit.Init();

            timer.Interval = new TimeSpan(1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void TagFile_eventHandlerOnTagFileReaded(object sender, EventArgs e)
        {
            GraphicTool.OnTagFileLoaded();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            ThreadDataChange.TimerMain();
        }

        void UnInit()
        {

        }*/

        private void Application_Exit(object sender, EventArgs e)
        {

        }
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;

                try
                {
                    string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                    errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                    System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight 2 Application " + errorMsg + "\");");
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
