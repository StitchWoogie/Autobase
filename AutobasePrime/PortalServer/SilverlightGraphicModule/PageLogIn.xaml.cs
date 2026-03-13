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
using AutoLibLocal;
using SilverlightNetTools;

namespace SilverlightGraphicModule
{
    public partial class PageLogIn : UserControl
    {
        string sInitPage;

        public PageLogIn(string init_page)
        {
            InitializeComponent();

            sInitPage = init_page;

            this.Width = Double.NaN;        // 자동 사이즈로 만든다.
            this.Height = Double.NaN;       // 자동 사이즈로 만든다.

            if (NetTools.Tools.IsLangKorean())
            {
                textBlockUsername.Text = "사용자 ID";
                textBlockPassword.Text = "비밀 번호";
                checkBoxRememberUsername.Content = "사용자 ID 기억";
                buttonOK.Content = "확인";
            }
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            vars.SetString("Username", "bUsernameRemember", this.checkBoxRememberUsername.IsChecked.ToString());
            vars.SetString("Username", "sUsernameRemember", this.textBoxUsername.Text);
            vars.Save();

            if (this.textBoxUsername.Text.Length == 0)
            {
                MessageBox.Show("Input Username");
                return;
            }

            SilverlightAutoLibLocal.ServiceReferenceUserProtect.ServiceUserProtectSoapClient service = new SilverlightAutoLibLocal.ServiceReferenceUserProtect.ServiceUserProtectSoapClient(new System.ServiceModel.BasicHttpBinding(), DataGate.GetEndPoint("ServiceUserProtect.asmx"));
            service.CheckUserNameWithVersionCompleted += new EventHandler<SilverlightAutoLibLocal.ServiceReferenceUserProtect.CheckUserNameWithVersionCompletedEventArgs>(service_CheckUserNameWithVersionCompleted);

            string sPassCode = UserInfoStruct.ZipPassword(textBoxUsername.Text, textBoxPassword.Password).ToString();

            service.CheckUserNameWithVersionAsync(this.textBoxUsername.Text, sPassCode);
        }

        void service_CheckUserNameWithVersionCompleted(object sender, SilverlightAutoLibLocal.ServiceReferenceUserProtect.CheckUserNameWithVersionCompletedEventArgs e)
        {
            if (e.Result == true)
            {
                //sUserName = this.textBoxUsername.Text;
                Page page = new Page();
                page.Load(sInitPage, -1);

                this.LayoutRoot.Children.Clear();
                this.LayoutRoot.Children.Add(page);

                SetVarsLogInOK(this.textBoxUsername.Text);

                string path = MakeFilePath.Users(this.textBoxUsername.Text);
                path = MakeFilePath.MakePublishTextPath(path);
                SharedData.userInfo.LoadFile(this.textBoxUsername.Text, path);
            }
            else
            {
                MessageBox.Show(e.err_msg, "로그인 오류", MessageBoxButton.OK);
            }
        }

        public static ClassIsolatedVar vars = new ClassIsolatedVar("Username Information");

        public static void SetVarsLogInOK(string username)
        {
            vars.SetBool("Username", "bOkLogIn", true);
            vars.SetString("Username", "sOkUsername", username);

            vars.Save();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string val = "";

            bool flag = vars.GetBool("Username", "bUsernameRemember", false);

            if (flag)
            {
                this.checkBoxRememberUsername.IsChecked = true;

                val = vars.GetString("Username", "sUsernameRemember", "");
                
                if (val != null)
                    this.textBoxUsername.Text = val;

                this.textBoxPassword.Focus();
            }
        }

    }
}
