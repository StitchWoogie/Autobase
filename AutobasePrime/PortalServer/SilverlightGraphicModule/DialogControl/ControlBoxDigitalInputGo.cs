using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using AutoLibLocal;
using NetTools;
using AutoLib;
using System.Windows.Controls.Primitives;

namespace SilverlightDialogControl
{
    public class ControlBoxDigitalInputGo
    {
        public void Go(UserControl owner, string tag)
        {
            Go(owner, tag, 0, "", "?", "?");
        }

        public void Go(UserControl owner, string tag, sbyte bUseUserControl, string modulename, string dialog_title, string dialog_description)
        {
            TagDiClass di;
            int[] pos = new int[1];

            di = TagLib.GetStructDI(tag, ref pos);

            if ((di.wProtectFlags & EnumProtectFlag.CONTROL) > 0)
            {
                if (Tools.IsLangKorean())
                    MessageBox.Show("이 태그는 제어 금지가 설정되어 있습니다.", di.tag, MessageBoxButton.OK);
                else if (Tools.IsLangChinese())
                    MessageBox.Show("这个标记已设置为禁止控制。", di.tag, MessageBoxButton.OK);
                else
                    MessageBox.Show("This tag protect to control.", di.tag, MessageBoxButton.OK);

                return;
            }

            di = TagLib.GetDirectTag(di);

            if (!SharedData.userInfo.IsHaveTagRight(di.tag))
            {
                string msg;
                if (NetTools.Tools.IsLangKorean())
                {
                    msg = String.Format("이 태그를 수동 작동할 권한이 없습니다.\n값을 변경하려면 권한이 있는 사용자 이름으로\nLOGIN 하시기 바랍니다.\n\n태그={0}", di.tag);
                    MessageBox.Show(msg, "작동 권한 없음", MessageBoxButton.OK);
                }
                else if (NetTools.Tools.IsLangChinese())
                {
                    msg = String.Format("没有权限把这个标记以非自动方式启动。\n想要更改值，请以有权限的用户名登录。\n\n标记={0}", di.tag);
                    MessageBox.Show(msg, "没有启动权限", MessageBoxButton.OK);
                }
                else
                {
                    msg = String.Format("Access to the TAG is denied.\nLOGIN another username to control the TAG\n\nTAG name={0}", di.tag);
                    MessageBox.Show(msg, "Access denied", MessageBoxButton.OK);
                }

                return;
            }

            if (di.cTagLinkType == 0 || di.cTagLinkType == 1)
            {
                // 이함수를 호출해 주어야 최초에 실행될때 태그가 없을 경우 오류를 찾을 수 있다.
                TagLib.GetStructDI(di.sSubOutDigital1, ref di.nSubOutDigital1);

                if (di.bUseAsOutput == 0 && di.nSubOutDigital1[0] == TagLib.TAG_NOT_FOUND)
                {
                    if (NetTools.Tools.IsLangKorean())
                        MessageBox.Show("설정된 디지털 출력태그(OUT1 또는 OUT2)가 없습니다.");
                    else
                        MessageBox.Show("OUT1 or OUT2 Tag is not defined.");

                    return;
                }
            }

            if (bUseUserControl == 1 && ControlBoxAnalogInputGo.procUserControl != null)
            {
                //string filename = MakeFilePath.Graphic(modulename);
                ControlBoxAnalogInputGo.procUserControl(modulename, tag, dialog_title, dialog_description, "0", "100");
            }
            else
            {
                if (ConfigViewMain.bUserControlBoxUseAnalogModule && ControlBoxAnalogInputGo.procUserControl != null)
                {
                    //string filename = MakeFilePath.Graphic(ConfigViewMain.sUserControlBoxAnalogModule);
                    ControlBoxAnalogInputGo.procUserControl(modulename, tag, "Tag : " + di.tag, di.description, "0", "100");
                }
                else
                {
                    MyDialogDigitalInput dialog = new MyDialogDigitalInput(tag);

                    dialog.Show(DialogStyle.Modal);
                }
            }
            
        }

    }

    class MyDialogDigitalInput : Dialog
    {
        TextBox textBoxValue = new TextBox();
        PageControlDigital childAnalog;
        Grid gridBackground;

        public MyDialogDigitalInput(string tag)
        {
            sTag = tag;
        }

        protected override FrameworkElement GetContent()
        {
            PageControlDigital dialog = new PageControlDigital(sTag);

            dialog.procClose = new PageControlDigital.DelegateClose(Close);

            Grid grid = new Grid() { Background=new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)),};

            grid.Children.Add(dialog);

            grid.SizeChanged += new SizeChangedEventHandler(grid_SizeChanged);

            childAnalog = dialog;
            gridBackground = grid;

            return grid; 
        }

        void grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Grid g = gridBackground;

            if (childAnalog.Width > g.ActualWidth || childAnalog.Height > g.ActualHeight)
            {
                ScaleTransform transform = new ScaleTransform();

                if (childAnalog.Width > g.ActualWidth)
                    transform.ScaleX = g.ActualWidth / childAnalog.Width;
                else
                    transform.ScaleX = 1;

                if (childAnalog.Height > g.ActualHeight)
                    transform.ScaleY = g.ActualHeight / childAnalog.Height;
                else
                    transform.ScaleY = 1;

                childAnalog.LayoutRoot.RenderTransform = transform;
            }
            else
            {
                childAnalog.LayoutRoot.RenderTransform = null;
            }
        }

        /*
        void button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }*/

        string sTag;

        
    }


}
