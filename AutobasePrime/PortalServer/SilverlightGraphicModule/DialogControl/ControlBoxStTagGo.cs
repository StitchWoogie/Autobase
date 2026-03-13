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
    public class ControlBoxStringTagGo
    {
        public void Go(UserControl owner, string tag)
        {
            Go(owner, tag, 0, "", "?", "?");
        }

        public void Go(UserControl owner, string tag, sbyte bUseUserControl, string modulename, string dialog_title, string dialog_description)
        {
            TagStClass st;
            int[] pos = new int[1];

            st = TagLib.GetStructST(tag, ref pos);

            if (!SharedData.userInfo.IsHaveTagRight(st.tag))
            {
                string msg;
                if (NetTools.Tools.IsLangKorean())
                {
                    msg = String.Format("이 태그를 수동 작동할 권한이 없습니다.\n값을 변경하려면 권한이 있는 사용자 이름으로\nLOGIN 하시기 바랍니다.\n\n태그={0}", st.tag);
                    MessageBox.Show(msg, "작동 권한 없음", MessageBoxButton.OK);
                }
                else if (NetTools.Tools.IsLangChinese())
                {
                    msg = String.Format("没有权限把这个标记以非自动方式启动。\n想要更改值，请以有权限的用户名登录。\n\n标记={0}", st.tag);
                    MessageBox.Show(msg, "没有启动权限", MessageBoxButton.OK);
                }
                else
                {
                    msg = String.Format("Access to the TAG is denied.\nLOGIN another username to control the TAG\n\nTAG name={0}", st.tag);
                    MessageBox.Show(msg, "Access denied", MessageBoxButton.OK);
                }

                return;
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
                    ControlBoxAnalogInputGo.procUserControl(modulename, tag, "Tag : " + st.tag, st.description, "0", "100");
                }
                else
                {
                    MyDialogStringTag dialog = new MyDialogStringTag(tag);

                    dialog.Show(DialogStyle.Modal);
                }
            }
            
        }

    }

    class MyDialogStringTag : Dialog
    {
        TextBox textBoxValue = new TextBox();
        PageControlString childAnalog;
        Grid gridBackground;

        public MyDialogStringTag(string tag)
        {
            sTag = tag;
        }

        protected override FrameworkElement GetContent()
        {
            PageControlString dialog = new PageControlString(sTag);

            dialog.procClose = new PageControlString.DelegateClose(Close);

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
