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

namespace SilverlightGraphicModule
{
    class DialogCommon : SilverlightDialogControl.Dialog
    {
        Grid gridBackground;
        Canvas gridFrame;       // 이것을 그리드로 하면 Scale Transform이 적용될 때 오른쪽/아래 영역이 비례적으로 안보이는 현상이 나타난다.
        Grid gridIn;
        public string Text = "Dialog";
        Grid gridClose;
        Rectangle rectTitle;

        protected override FrameworkElement GetContent()
        {
            Grid grid = new Grid() { Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)), };     // 배경으로 반투명 하게 그리는 부분 전체 사각형

            grid.SizeChanged += new SizeChangedEventHandler(grid_SizeChanged);
            
            gridBackground = grid;

            Grid gridp = new Grid();

            grid.Children.Add(gridp);

            gridFrame = new Canvas() { Width=0,Height=0, Margin=new Thickness(0,0,0,0)};

            gridp.Children.Add(gridFrame);

            //Rectangle r = new Rectangle() { RadiusX = 4, RadiusY = 4, StrokeThickness = 3, Stroke = new SolidColorBrush(Color.FromArgb(0x7f, 0x9b, 0x9b, 0x9b)), Width=100, Height=100, Margin=new Thickness(0,0,0,0) };
            //gridFrame.Children.Add(r);

            rectTitle = new Rectangle();    // 타이틀 부분
            rectTitle.Height = 22;
            rectTitle.Width = 0;
            //rectTitle.RadiusX = 4;
            //rectTitle.RadiusY = 4;
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0.5, 0);
            brush.EndPoint = new Point(0.5, 1);
            brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0, 0, 0), Offset = 0.321 });
            brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x66, 0x66, 0x66), Offset = 0.987 });
            rectTitle.Fill = brush;// new SolidColorBrush(Color.FromArgb(128, 0, 0, 128));
            gridFrame.Children.Add(rectTitle);

            TextBlock text = new TextBlock();
            text.Text = Text;
            text.Margin = new Thickness(4, 4, 0, 0);
            text.Foreground = new SolidColorBrush(Colors.White);
            gridFrame.Children.Add(text);

            /*
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

            gridFrame.Children.Add(rect);*/
            gridClose = MakeCloseGrid();
            gridFrame.Children.Add(gridClose);

            gridIn = new Grid() { Margin = new Thickness(0, 22, 0, 0), };

            gridFrame.Children.Add(gridIn);

            
            gridIn.Children.Add(pageThis);




            return grid;
        }

        Storyboard storyClose = new Storyboard();

        Grid MakeCloseGrid()
        {
            Grid grid = new Grid();

            grid.Width = 18;
            grid.Height = 18;
            //rect.HorizontalAlignment = HorizontalAlignment.Right;
            //rect.VerticalAlignment = VerticalAlignment.Top;
            //rect.Margin = new Thickness(0, 2, 2, 0);
            //rect.Fill = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
            //rect.Stroke = new SolidColorBrush(Colors.Black);
            grid.Margin = new Thickness(0, 2, 2, 0);
            grid.MouseLeftButtonDown += new MouseButtonEventHandler(rect_MouseLeftButtonDown);
            grid.MouseEnter += new MouseEventHandler(grid_MouseEnter);
            grid.MouseLeave += new MouseEventHandler(grid_MouseLeave);

            Rectangle rect = new Rectangle() { Stroke = new SolidColorBrush(Color.FromArgb(0x4C, 0xFF, 0xA3, 0xA3)), StrokeThickness = 1, RadiusX = 4, RadiusY = 4 };

            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0.5, 0);
            brush.EndPoint = new Point(0.5, 1);
            brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x1E, 0x0D, 0x0D), Offset = 0.321 });
            brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xFF, 0x70, 0x57, 0x57), Offset = 0.987 });
            rect.Fill = brush;

            grid.Children.Add(rect);

            Path path1 = new Path();
            path1.Height = 12.333;
            path1.Margin = new Thickness(3, 2.932, 2, 3.068);
            path1.RenderTransformOrigin = new Point(0.5, 0.5);
            path1.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            path1.Stretch = Stretch.Fill;
            path1.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x99, 0x4b, 0x4b));
            path1.StrokeThickness = 2;
            path1.Data = new LineGeometry() { StartPoint = new Point(529, 48), EndPoint = new Point(543, 61) };

            grid.Children.Add(path1);

            Path path2 = new Path();
            path2.Height = 12.333;
            path2.Margin = new Thickness(3, 2.932, 2, 3.068);
            path2.RenderTransformOrigin = new Point(0.5, 0.5);
            path2.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));
            path2.Stretch = Stretch.Fill;
            path2.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x99, 0x4b, 0x4b));
            path2.StrokeThickness = 2;
            path2.Data = new LineGeometry() { StartPoint = new Point(529, 48), EndPoint = new Point(543, 61) };
            path2.RenderTransform = new RotateTransform() { Angle = 90 }; 

            grid.Children.Add(path2);

            Storyboard story = storyClose;

            ColorAnimationUsingKeyFrames ani = new ColorAnimationUsingKeyFrames();
            SplineColorKeyFrame c = new SplineColorKeyFrame();
            c.Value = Color.FromArgb(0xFF, 0x7C, 0x04, 0x04);
            c.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5));
            ani.KeyFrames.Add(c);

            Storyboard.SetTarget(ani, rect);
            Storyboard.SetTargetProperty(ani, new PropertyPath("(Shape.Fill).(GradientBrush.GradientStops)[0].(GradientStop.Color)"));

            story.Children.Add(ani);

            ani = new ColorAnimationUsingKeyFrames();
            c = new SplineColorKeyFrame();
            c.Value = Color.FromArgb(0xFF, 0xCA, 0x00, 0x00);
            c.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5));
            ani.KeyFrames.Add(c);

            Storyboard.SetTarget(ani, rect);
            Storyboard.SetTargetProperty(ani, new PropertyPath("(Shape.Fill).(GradientBrush.GradientStops)[1].(GradientStop.Color)"));

            story.Children.Add(ani);


            ani = new ColorAnimationUsingKeyFrames();
            c = new SplineColorKeyFrame();
            c.Value = Color.FromArgb(0xB2, 0xff, 0x00, 0x00);
            c.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5));
            ani.KeyFrames.Add(c);

            Storyboard.SetTarget(ani, rect);
            Storyboard.SetTargetProperty(ani, new PropertyPath("(Shape.Stroke).(SolidColorBrush.Color)"));

            story.Children.Add(ani);


            ani = new ColorAnimationUsingKeyFrames();
            c = new SplineColorKeyFrame();
            c.Value = Color.FromArgb(0xFF, 0x49, 0x02, 0x02);
            c.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5));
            ani.KeyFrames.Add(c);

            Storyboard.SetTarget(ani, path1);
            Storyboard.SetTargetProperty(ani, new PropertyPath("(Shape.Stroke).(SolidColorBrush.Color)"));

            story.Children.Add(ani);

            ani = new ColorAnimationUsingKeyFrames();
            c = new SplineColorKeyFrame();
            c.Value = Color.FromArgb(0xFF, 0x49, 0x02, 0x02);
            c.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5));
            ani.KeyFrames.Add(c);

            Storyboard.SetTarget(ani, path2);
            Storyboard.SetTargetProperty(ani, new PropertyPath("(Shape.Stroke).(SolidColorBrush.Color)"));

            story.Children.Add(ani);

            //story.Begin();
            
            return grid;
        }

        void grid_MouseLeave(object sender, MouseEventArgs e)
        {
            storyClose.Stop();
            //storyClose.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            storyClose.AutoReverse = true;
            storyClose.Begin();
            storyClose.Seek(TimeSpan.FromSeconds(0.5)); // Seek가 Begin보다 먼저 있으면 잘 안됨
        }

        void grid_MouseEnter(object sender, MouseEventArgs e)
        {
            storyClose.Stop();
            storyClose.AutoReverse = false;
            
            storyClose.Begin();
            storyClose.Seek(TimeSpan.FromSeconds(0));   
        }

        void rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        double nOrgWidth=1, nOrgHeight=1;

        void pageThis_eventHandlerModuleLoadComplete()
        {
            int title_size = 20;
            int border_thick = 0;

            /*
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
            }*/

            this.gridFrame.Width = pageThis.ActualWidth + border_thick * 2;
            this.gridFrame.Height = pageThis.ActualHeight + title_size + border_thick * 2;

            nOrgWidth = this.gridFrame.Width;
            nOrgHeight = this.gridFrame.Height;

            Canvas.SetLeft(gridClose, gridFrame.Width - 22);

            rectTitle.Width = this.gridFrame.Width;

            //ScrollUpdate();
        }

        void pageThis_eventHandlerModuleClose(object sender, EventArgs e)
        {
            Close();
        }

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
            pageThis_eventHandlerModuleLoadComplete();
            ScrollUpdate();
        }

        

        UserControl pageThis;

        public DialogCommon(UserControl page)
        {
            pageThis = page;
        }

        protected override void OnClickOutside()
        {

        }

        protected override void OnLoad()
        {

        }

        /*
		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			Close();
		}*/

        /// <summary>
        /// 사용자가 정의한 제어 박스를 표시한다.
        /// </summary>
        /// <param name="filename"></param>
        public static void DisplayDialog(UserControl page, string dialog_title)
        {
            DialogCommon dialog = new DialogCommon(page);

            dialog.Text = dialog_title;

            dialog.Show(SilverlightDialogControl.DialogStyle.Modal);
        }

        public void RegisterCancelButton(Button button)
        {
            button.Click += new RoutedEventHandler(button_Click);
        }

        public delegate bool DelegateOnOK(UserControl control);
        DelegateOnOK procOnOK = null;

        public void RegisterOkButton(Button button, DelegateOnOK proc)
        {
            button.Click += new RoutedEventHandler(buttonOK_Click);
            procOnOK = proc;
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (procOnOK != null)
            {
                if (!procOnOK(pageThis)) return;
            }
            Close();
        }
    }


}
