using System;
using DialogConfigUser;
using AutoLib;
using System.Windows.Forms;
using System.Drawing;
using NetTools;
using GraphicModule;
using ReportModule;
using AutoLibLocal;
using System.IO;

namespace ViewMainPublic
{
	/// <summary>
	/// Summary description for PublicMenu.
	/// </summary>
	public class PublicMenu
	{
		public PublicMenu()
		{
			//
			// TODO: Add constructor logic here 
			//
		}

        //20241010 PSU 화면위치수정.
        //form.StartPosition = FormStartPosition.CenterParent;
        //form.ShowDialog(TotalConfig.formMain);
		public static void menuItemFileScreenPrint_Click(Form form)
		{
			form.Update();

			Bitmap bitmap = ScreenCapture.Capture();

			DialogClipBoardPrint.FormClipBoardPrint dialog = new DialogClipBoardPrint.FormClipBoardPrint();

			dialog.prepareBitmap = bitmap;

            dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog(form);
		}

		public static void menuItemFileScreenSave_Click(Form form)
		{
			form.Update();
			Bitmap bitmap = ScreenCapture.Capture();

			string filename;

			SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "PNG files (*.png)|*.png|Jpeg files (*.jpg)|*.jpg|Windows Bitmap Files (*.bmp)|*.bmp";

			if(Tools.IsLangKorean()) 
			{
				dialog.Title = "화면 저장";
			}
			else if(Tools.IsLangChinese()) 
			{
				dialog.Title = "保存全屏显示";
			}
			else
				dialog.Title = "Screen Save";

            if(dialog.ShowDialog(form) != DialogResult.OK)	return;

			filename = dialog.FileName;

			bitmap.Save(filename);
		}

		public static void menuItemFileSelectScreenSave_Click(Form form)
		{
			form.Update();
			Bitmap bitmap = ScreenCapture.Capture();

			DialogClipBoardSave.FormClipBoardSave dialog = new DialogClipBoardSave.FormClipBoardSave();

			dialog.prepareBitmap = bitmap;

            dialog.StartPosition = FormStartPosition.CenterParent;
			dialog.ShowDialog(form);
		}

		public static void menuItemViewGraphic_Click()
		{
			string filename;
			
			if(SharedData.userInfo.sStartPage.Length == 0) 
			{
				filename = MakeFilePath.Graphic("StartUp.modx");
				if(!File.Exists(filename)) 
				{
                    filename = MakeFilePath.Graphic("StartUp.mod");
                    // 2021-2-18 원래 mod에서 끝났는데 mod가 존재하지 않으면 startup.modx로 다시 변경했다. startup.mod 파일이 없다는 메시지가 나와서 사용자가 혼동이 있다.
                    if (!File.Exists(filename))
                    {
                        filename = MakeFilePath.Graphic("StartUp.modx");
                    }
				}
			}
			else 
			{
				filename = MakeFilePath.Graphic(SharedData.userInfo.sStartPage);
			}
			
			GraphicTool.RestoreGraphicWindow(filename, -1, 0, 0);
		}

		public static void menuItemViewAllTagView_Click(Form form)
		{
			BasicScreen.kdymain.ViewGroupTagListMainNew.ringForm.CreateMdi(TotalConfig.formMain, new BasicScreen.kdymain.ViewGroupTagListMainNew(), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		public static void menuItemViewAnalogInput_Click(Form form)
		{
			BasicScreen.kdymain.ViewAnalogInputMain.ringForm.CreateMdi(TotalConfig.formMain, new BasicScreen.kdymain.ViewAnalogInputMain(null), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		public static void menuItemViewAnalogOutput_Click(Form form)
		{
			BasicScreen.kdymain.ViewAnalogOutputMain.ringForm.CreateMdi(TotalConfig.formMain, new BasicScreen.kdymain.ViewAnalogOutputMain(null), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		public static void menuItemViewDigitalInput_Click(Form form)
		{
			BasicScreen.kdymain.ViewDigitalInputMain.ringForm.CreateMdi(TotalConfig.formMain, new BasicScreen.kdymain.ViewDigitalInputMain(null), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		public static void menuItemViewDigitalOutput_Click(Form form)
		{
			BasicScreen.kdymain.ViewDigitalOutputMain.ringForm.CreateMdi(TotalConfig.formMain, new BasicScreen.kdymain.ViewDigitalOutputMain(null), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		public static void menuItemViewStringTag_Click(Form form)
		{
			BasicScreen.kdymain.ViewStringTagMain.ringForm.CreateMdi(TotalConfig.formMain, new BasicScreen.kdymain.ViewStringTagMain(null), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		public static void menuItemViewRegisterdGroup_Click(Form form)
		{
			BasicScreen.kdymain.ViewRegisteredGroupMain.ringForm.CreateMdi(TotalConfig.formMain, new BasicScreen.kdymain.ViewRegisteredGroupMain(), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		public static void menuItemViewReport_Click()
		{
            if (!TotalConfig.GetOemTypeRightAndUnavailableMsg(EnumOemTypeRight.Report)) return;

			FormDialogOpenReport dialog = new FormDialogOpenReport();
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(TotalConfig.formMain) == DialogResult.OK) //20241010 PSU TotalConfig.formMain
			{
				FormDialogOpenReport.OpenReportAtViewMain(dialog.FileName);
			}
		}

		public static void menuItemViewAlarmFile_Click(Form form)
		{
			BasicScreen.kdymain.ViewListAlarmMain.ringForm.CreateMdi(TotalConfig.formMain, new BasicScreen.kdymain.ViewListAlarmMain(-1), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		public static void menuItemViewLogFile_Click(Form form)
		{
			BasicScreen.kdymain.ViewEventLogMain.ringForm.CreateMdi(TotalConfig.formMain, new BasicScreen.kdymain.ViewEventLogMain(-1), ConfigViewMain.nMdiCountOnBasicScreen);
		}

		public static void menuItemViewNavigator_Click(Form form)
		{
			if(SharedData.formNavigator == null) 
			{
				FormNavigator nav = new FormNavigator(SharedData.userInfo.sStartPage);

				nav.Owner = form;
				nav.Show();
			}
			else 
			{
				SharedData.formNavigator.Close();
			}
		}

		public static void menuItemConfigUser_Click()
		{
			FormConfigUser form = new FormConfigUser();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(TotalConfig.formMain);
		}

		public static void menuItemUserInformation_Click()
		{
			string msg;

            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                if (String.Compare(SharedData.userInfo.sUsername, "_PUBLIC_", true) == 0 ||
                    String.Compare(SharedData.userInfo.sUsername, "PUBLIC", true) == 0)
                {
                    msg = "현재 로그아웃된 상태입니다.";
                    MessageBox.Show(msg, "로그인 사용자 확인");
                }
                else
                {
                    msg = "현재 사용자는 [" + SharedData.userInfo.sUsername + "] 입니다.";
                    MessageBox.Show(msg, "로그인 사용자 확인");
                }
                return;
            }
			
			if(Tools.IsLangKorean()) 
			{ 
				msg = "현재 사용자는 ["+SharedData.userInfo.sUsername+"] 입니다.";
				MessageBox.Show(msg, "사용자 이름");
			}
			else if(Tools.IsLangJapanese()) 
			{
				msg = "現在のユーザー名は ["+SharedData.userInfo.sUsername+"] です.";
				MessageBox.Show(msg, "ユーザー名");
			}
			else if(Tools.IsLangChinese()) 
			{
                msg = "现在的用户是 [" + SharedData.userInfo.sUsername + "]";
				MessageBox.Show(msg, "用户名");
			}
			else 
			{
				msg = "Current username is ["+SharedData.userInfo.sUsername+"].";
				MessageBox.Show(msg, "username");
			}
		}

		public static void menuItemConfigFont_Click()
		{
			FontDialog dialog = new FontDialog();

			dialog.Font = ConfigViewMain.fontMain;

            if (dialog.ShowDialog(TotalConfig.formMain) == DialogResult.OK) 
			{
				ConfigViewMain.fontMain = dialog.Font;
				SharedViewMain.EventGoMainFontChanged();
				ConfigViewMain.Save();
			}
		}

		public static void menuItemConfigTotalColor_Click(Form form)
		{
			ConfigColorTotal dialog = new ConfigColorTotal();
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(TotalConfig.formMain) == DialogResult.OK) 
			{
				Form[] childForm = form.MdiChildren; 
				//Make sure to ask for saving the doc before exiting the app 
				for(int i=0; i < childForm.Length; i++) 
					childForm[i].Refresh();

				SharedViewMain.EventGoColorChanged();
			}
		}

		public static void menuItemConfigAlarmColor_Click(Form form)
		{
			ConfigColorAlarm dialog = new ConfigColorAlarm();
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(TotalConfig.formMain) == DialogResult.OK) 
			{
				Form[] childForm = form.MdiChildren;
				//Make sure to ask for saving the doc before exiting the app
				for(int i=0; i < childForm.Length; i++) 
					childForm[i].Refresh();

				SharedViewMain.EventGoColorChanged();
			}
		}

        // 메뉴와 타이틀이 없는 상태에서 Mdi Child를 최대화하면 밑부분 영역이 20 Pixel정도 적게나와서 일시적으로 Child의 속성을 FormBorStyle을 None으로 바꿔준다.
        // 메뉴/타이틀 표시/비표시 명령이나 메인에서 MdiActivate 시에 이 함수를 호출하도록 한다.
        public static void SetFormBorderStyleMdiChild(Form parent, Form child)
        {
            if (child == null) return;
            FormBorderStyle style;

            if (!ConfigViewMain.bShowMainMenu && !ConfigViewMain.bShowMainTitle)
            {
                style = FormBorderStyle.None;
            }
            else
            {
                style = FormBorderStyle.Sizable;
            }

            if (style != child.FormBorderStyle)
            {
                child.FormBorderStyle = style;

                // 메뉴/타이틀이 모두 없는 경우에는 창조절 메뉴가 어차피 없으므로 최대화 해 준다.
                if (style == FormBorderStyle.None)
                {
                    // WindowState를 번갈아 바꾸지 않으면 Style이 잘 적용이 안된다.
                    child.WindowState = FormWindowState.Normal;
                    child.WindowState = FormWindowState.Maximized;      
                }
                else
                {
                    for (int i = 0; i < parent.MdiChildren.Length; i++)
                    {
                        if (parent.MdiChildren[i].FormBorderStyle != style)
                        {
                            parent.MdiChildren[i].FormBorderStyle = style;
                        }
                    }
                    
                    if (child.WindowState == FormWindowState.Maximized)
                    {
                        child.WindowState = FormWindowState.Normal;
                        child.WindowState = FormWindowState.Maximized;
                    }
                    else if (child.WindowState == FormWindowState.Normal)
                    {
                        child.WindowState = FormWindowState.Maximized;
                        child.WindowState = FormWindowState.Normal;
                    }
                }
            }
        }

        public static void EnableDisableMainTitle(Form form)
        {
            //SetFormBorderStyleMdiChildren(form);

            if (ConfigViewMain.bShowMainTitle)
            {
                form.FormBorderStyle = FormBorderStyle.Sizable;
                //form.ControlBox = true;
            }
            else
            {
                bool bMax = (form.WindowState == FormWindowState.Maximized);

                if (bMax) form.WindowState = FormWindowState.Normal;
                form.FormBorderStyle = FormBorderStyle.None;
                if (bMax) form.WindowState = FormWindowState.Maximized;
                
                //form.ControlBox = false;
                //form.Text = string.Empty;
            }

            SetFormBorderStyleMdiChild(form, form.ActiveMdiChild);
        }

		public static void menuItemViewWindowTitle_Click(Form form)
		{
			ConfigViewMain.bShowMainTitle = !ConfigViewMain.bShowMainTitle;

            ConfigViewMain.Save();

            EnableDisableMainTitle(form);
		}

        public static void EnableDisableMainMenu(Form form, MenuStrip menu)
        {
            //SetFormBorderStyleMdiChildren(form);

            if (ConfigViewMain.bShowMainMenu)
            {
                //form.Menu = menu;
                menu.Visible = true;
                //menu.Show();
            }
            else
            {
                //menu.Hide();
                menu.Visible = false;
                //form.Menu = null;
            }

            //form.WindowState = FormWindowState.Normal;
            //form.WindowState = FormWindowState.Maximized;

            SetFormBorderStyleMdiChild(form, form.ActiveMdiChild);
        }

		public static void menuItemViewMainMenu_Click(Form form, MenuStrip menu)
		{
			ConfigViewMain.bShowMainMenu = !ConfigViewMain.bShowMainMenu;

            ConfigViewMain.Save();

            EnableDisableMainMenu(form, menu);
		}

		
	}
}
