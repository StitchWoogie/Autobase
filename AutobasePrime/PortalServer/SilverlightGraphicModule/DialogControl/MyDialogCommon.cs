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
using SilverlightGraphicModule;
using AutoLibLocal;

namespace SilverlightDialogControl
{
    public class MyDialogCommon : Dialog
    {
        Grid gridBackground;
        Canvas gridFrame;       // 이것을 그리드로 하면 Scale Transform이 적용될 때 오른쪽/아래 영역이 비례적으로 안보이는 현상이 나타난다.
        Grid gridIn;
        public string Text = "Dialog";
        Rectangle rectClose;
        Rectangle rectTitle;

        protected override FrameworkElement GetContent()
        {
            Grid grid = new Grid() { Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)), };     // 배경으로 반투명 하게 그리는 부분

            grid.SizeChanged += new SizeChangedEventHandler(grid_SizeChanged);

            gridBackground = grid;

            Grid gridp = new Grid();

            grid.Children.Add(gridp);

            //gridFrame = new Canvas() { Width = 0, Height = 0, Margin = new Thickness(0, 0, 0, 0) };
            gridFrame = new Canvas() { Width = pageThis.Width, Height = pageThis.Height+20, Margin = new Thickness(0, 0, 0, 0) };

            gridp.Children.Add(gridFrame);

            rectTitle = new Rectangle();
            rectTitle.Height = 20;
            rectTitle.Width = 0;
            rectTitle.Fill = new SolidColorBrush(Color.FromArgb(128, 0, 0, 128));
            gridFrame.Children.Add(rectTitle);

            TextBlock text = new TextBlock();
            text.Text = Text;
            text.Margin = new Thickness(4, 4, 0, 0);
            text.Foreground = new SolidColorBrush(Colors.White);
            gridFrame.Children.Add(text);

            Rectangle rect = new Rectangle();
            rectClose = rect;
            rect.Width = 16;
            rect.Height = 16;
            rect.HorizontalAlignment = HorizontalAlignment.Right;
            rect.VerticalAlignment = VerticalAlignment.Top;
            rect.Margin = new Thickness(0, 2, 2, 0);
            rect.Fill = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
            rect.Stroke = new SolidColorBrush(Colors.Black);
            rect.MouseLeftButtonDown += new MouseButtonEventHandler(rect_MouseLeftButtonDown);
            gridFrame.Children.Add(rect);

            gridIn = new Grid() { Margin = new Thickness(0, 20, 0, 0), };

            gridFrame.Children.Add(gridIn);

            gridIn.Children.Add(pageThis);
            //childAnalog = pageThis;

            // ReCalcSize();

            Canvas.SetLeft(rectClose, gridFrame.Width - 18);

            rectTitle.Width = this.gridFrame.Width;

            return grid;
        }

        void ReCalcSize()
        {
            uint flags = 0xFFFF;// pageThis.obj.GetModuleWindowStyleFlag();
            int title_size = 0;
            int border_thick = 0;

            if ((flags & (uint)EnumWindowStyleFlags.WS_CAPTION) == (uint)EnumWindowStyleFlags.WS_CAPTION ||
                (flags & (uint)EnumWindowStyleFlags.WS_SYSMENU) == (uint)EnumWindowStyleFlags.WS_SYSMENU)
            {
                title_size = 20;
            }
            else
            {
                title_size = 0;
                gridIn.Margin = new Thickness(0, 0, 0, 0);
                rectTitle.Fill = null;
            }

            /*
            if ((flags & (uint)EnumWindowStyleFlags.WS_BORDER) == (uint)EnumWindowStyleFlags.WS_BORDER)
            {
                border_thick = 1;
            }*/

            //ChangeTagName(pageUser, sTempUserControlBoxTag);
            this.gridFrame.Width = pageThis.Width + border_thick * 2;
            this.gridFrame.Height = pageThis.Height + title_size + border_thick * 2;
            //throw new NotImplementedException();

            nOrgWidth = this.gridFrame.Width;
            nOrgHeight = this.gridFrame.Height;

            Canvas.SetLeft(rectClose, gridFrame.Width - 18);

            rectTitle.Width = this.gridFrame.Width;

            //ScrollUpdate();
        }

        void rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
            //throw new NotImplementedException();
        }

        double nOrgWidth = 1, nOrgHeight = 1;

        void ScrollUpdate()
        {
            Grid g = gridBackground;

            if (nOrgWidth > g.ActualWidth || nOrgHeight > g.ActualHeight)
            {
                ScaleTransform transform = new ScaleTransform();

                if (nOrgWidth > g.ActualWidth)
                {
                    transform.ScaleX = g.ActualWidth / nOrgWidth;
                }
                else
                    transform.ScaleX = 1;

                if (nOrgHeight > g.ActualHeight)
                {
                    transform.ScaleY = g.ActualHeight / nOrgHeight;
                }
                else
                    transform.ScaleY = 1;

                gridFrame.RenderTransform = transform;
            }
            else
            {
                gridFrame.RenderTransform = null;
            }
        }

        void grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollUpdate();
        }

        public void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        UserControl pageThis;

        public MyDialogCommon(UserControl page)
        {
            //sTag = tag;
            pageThis = page;
        }

        protected override void OnClickOutside()
        {
            // Close();
        }

        protected override void OnLoad()
        {
            //this.textBoxValue.Select(0, textBoxValue.Text.Length);
            //this.textBoxValue.Focus();
        }

        public void ShowDialog(string title)
        {
            this.Text = title;
            Show(DialogStyle.Modal);
        }
    }
}
