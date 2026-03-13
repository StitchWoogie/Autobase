using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DialogConfigUser;
using AutoLib;
using System.Windows.Forms;
using AutoLibLocal;
using System.Threading.Tasks;

namespace ViewMain
{
    class LogInOut
    {
        public static async Task<bool> AutoBaseLogIn()
        {
            FormLogIn login = new FormLogIn();
            login.StartPosition = FormStartPosition.CenterParent;
            
            if (login.ShowDialog(TotalConfig.formMain) == DialogResult.OK)
            {
                if(ConfigVarTotal.bLocalFlag)   // viewmain 에서 local로 접속시. 2019-10-10 추가
                    await LogInByUsername(login.textBoxUsername.Text);

                SharedViewMain.EventGoUserChanged();	// 사용자가 바뀌었다.
                return true;
            }

            return false;
        }

        const string sSupervisorName = "admin";

        public static async Task LogInByUsername(string username)
        {
            //if (String.Compare(ConfigRunMain.sUserName, "_PUBLIC_", true) != 0)
            //{	// 다른 사용자가 Login되어 있는 상태
            //    PlayScriptWhenLogOutBefore();
            //}

            //SaveUserConfig(ConfigRunMain.sUserName);
            //ConfigRunMain.sUserName = username;
            SharedData.userInfo = new UserInfoStruct(); // 없는 사용자가 로그인 하는 경우를 대비해서 초기화한다.
            SharedData.userInfo.sUsername = username;
            //LoadUserConfig(ConfigRunMain.sUserName);

            if (ConfigVarTotal.bLocalFlag && String.Compare(username, sSupervisorName, true) == 0)
            {

            }
            else
            {
                string path = MakeFilePath.Users(username);
                string err_msg;

                if (!SharedData.userInfo.LoadUser(out err_msg, path, username))
                {
                    MessageBox.Show(err_msg, username);
                    return;
                }
            }

            SharedViewMain.EventGoUserChanged();
            //SendEventToChild.SendEventChangeUserToChild();
            //SendEventToChild.SendEventChangeColorToChild();
            //SendEventToChild.SendEventChangeFontToChild();

            //SendEventToChild.SendPaintToChild();

            //if (Tools.IsLangKorean())
            //{
            //    SmLog.Message("{0} 사용자 login", ConfigRunMain.sUserName);
            //}
            //else if (Tools.IsLangJapanese())
            //{
            //    SmLog.Message("{0} ユーザー login", ConfigRunMain.sUserName);
            //}
            //else if (Tools.IsLangChinese())
            //{
            //    SmLog.Message("{0} 用户 登录", ConfigRunMain.sUserName);
            //}
            //else
            //{
            //    SmLog.Message("{0} User login", ConfigRunMain.sUserName);
            //}

            //AlarmDisplay.AlarmDataSaveGeneral(ConfigRunMain.sUserName, "", "LOG IN", EnumAlarmType.GENERAL, EnumAlarmSubType.General_LOGIN);

            //PlayScriptWhenLogIn();

            await Task.CompletedTask;
        }

        public static async Task AutoBaseLogOut(bool message_flag)
        {
            if (String.Compare(SharedData.userInfo.sUsername, "_PUBLIC_", true) == 0) return;	// already log out

            //PlayScriptWhenLogOutBefore();

            //SaveUserConfig(ConfigRunMain.sUserName);

            //if (Tools.IsLangKorean())
            //{
            //    SmLog.Message("{0} 사용자 logout", ConfigRunMain.sUserName);
            //}
            //else if (Tools.IsLangJapanese())
            //{
            //    SmLog.Message("{0} ユーザー logout", ConfigRunMain.sUserName);
            //}
            //else if (Tools.IsLangChinese())
            //{
            //    SmLog.Message("{0} 用户 注销", ConfigRunMain.sUserName);
            //}
            //else
            //{
            //    SmLog.Message("{0} User logout", ConfigRunMain.sUserName);
            //}

            //AlarmDisplay.AlarmDataSaveGeneral(ConfigRunMain.sUserName, "", "LOG OUT", EnumAlarmType.GENERAL, EnumAlarmSubType.General_LOGOUT);

            SharedData.userInfo = new UserInfoStruct(); // 새로 할당하면 default가 된다.
            //ConfigRunMain.sUserName = "_PUBLIC_";
            SharedData.userInfo.sUsername = "_PUBLIC_";

            //LoadUserConfig(ConfigRunMain.sUserName);

            //ReadProtectLevel(ConfigRunMain.sUserName);

            SharedViewMain.EventGoUserChanged();
            SharedViewMain.EventGoColorChanged();
            //SendEventToChild.SendEventChangeUserToChild();
            //SendEventToChild.SendEventChangeColorToChild();
            //SendEventToChild.SendEventChangeFontToChild();
            //SendEventToChild.SendPaintToChild();

            //PlayScriptWhenLogOutAfter();

            await Task.CompletedTask;
        }
    }
}
