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

namespace SilverlightGraphicModule
{
    public partial class PageInitialWait : UserControl
    {
        string sInitPage;
        string sUserName;

        public PageInitialWait(string init_page)
        {
            InitializeComponent();

            sInitPage = init_page;

            this.Width = Double.NaN;        // 자동 사이즈로 만든다.
            this.Height = Double.NaN;       // 자동 사이즈로 만든다.

            if(NetTools.Tools.IsLangKorean()) {
                textBlock.Text = "로딩 중...";
            }

            this.Storyboard1.RepeatBehavior = RepeatBehavior.Forever;
            this.Storyboard1.Begin();

            bool flag = PageLogIn.vars.GetBool("Username", "bOkLogIn", false);

            if (flag)
            {
                sUserName = PageLogIn.vars.GetString("Username", "sOkUsername", "");

                LogIn();

                return;
            }

            CheckDefaultUser();
        }

        void LogIn()
        {
            string path = MakeFilePath.Users(sUserName);
            path = MakeFilePath.MakePublishTextPath(path);
            SharedData.userInfo.LoadFile(sUserName, path);

            Page page = new Page();
            page.Load(sInitPage, -1);

            this.LayoutRoot.Children.Clear();
            this.LayoutRoot.Children.Add(page);
        }

        void CheckDefaultUser()
        {
            SilverlightAutoLibLocal.ServiceReferenceUserProtect.ServiceUserProtectSoapClient service = new SilverlightAutoLibLocal.ServiceReferenceUserProtect.ServiceUserProtectSoapClient(new System.ServiceModel.BasicHttpBinding(), DataGate.GetEndPoint("ServiceUserProtect.asmx"));
            service.DefaultUserCheckWithVersionCompleted += new EventHandler<SilverlightAutoLibLocal.ServiceReferenceUserProtect.DefaultUserCheckWithVersionCompletedEventArgs>(service_DefaultUserCheckWithVersionCompleted);

            service.DefaultUserCheckWithVersionAsync();
        }

        void service_DefaultUserCheckWithVersionCompleted(object sender, SilverlightAutoLibLocal.ServiceReferenceUserProtect.DefaultUserCheckWithVersionCompletedEventArgs e)
        {
            if (e.Result == true)
            {
                sUserName = e.username;

                LogIn();

                PageLogIn.SetVarsLogInOK(sUserName);

                return;
            }
            else
            {
                PageLogIn page = new PageLogIn(sInitPage);

                this.LayoutRoot.Children.Clear();
                this.LayoutRoot.Children.Add(page);
            }
        }


    }
}
