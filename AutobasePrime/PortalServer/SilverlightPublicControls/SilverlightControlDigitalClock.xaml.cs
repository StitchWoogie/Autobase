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

namespace SilverlightPublicControls
{
    public partial class SilverlightControlDigitalClock : UserControl
    {
        System.Windows.Threading.DispatcherTimer timer;

        public SilverlightControlDigitalClock()
        {
            InitializeComponent();

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            this.Document.RenderTransform = scaleTransform;

            DrawDigital();    // 미리 계산
        }

        ScaleTransform scaleTransform = new ScaleTransform();

        void DrawDigital()
        {
            DateTime t = DateTime.Now;

            if (t.Second == old_sec) return;
            old_sec = t.Second;

            DrawSegment(Path_38, Path_39, Path_40, Path_41, Path_42, Path_43, H, t.Hour / 10);
            DrawSegment(Path_31, Path_32, Path_33, Path_34, Path_35, Path_36, Path_37, t.Hour % 10);
            DrawSegment(Path_24, Path_25, Path_26, Path_27, Path_28, Path_29, Path_30, t.Minute / 10);
            DrawSegment(Path_17, Path_18, Path_19, Path_20, Path_21, Path_22, Path_23, t.Minute % 10);
            DrawSegment(Path, Path_4, Path_5, Path_6, Path_7, Path_8, Path_9, t.Second / 10);
            DrawSegment(Path_10, Path_11, Path_12, Path_13, Path_14, Path_15, Path_16, t.Second % 10);

            Color color;
            if (t.Second % 2 == 0)
            {
                color = Color.FromArgb(255, 229, 229, 229);
            }
            else
            {
                color = Colors.Black;
            }

            this.dot.Fill = new SolidColorBrush(color);
            this.dot_0.Fill = new SolidColorBrush(color);
            this.dot_2.Fill = new SolidColorBrush(color);
            this.dot_3.Fill = new SolidColorBrush(color);
        }

        int old_sec = -1;

        void timer_Tick(object sender, EventArgs e)
        {
            DrawDigital();
        }

        void DrawSegment(Path seg0, Path seg1, Path seg2, Path seg3, Path seg4, Path seg5, Path seg6, int val)
        {
            bool f0 =false;
            bool f1 = false;
            bool f2 = false;
            bool f3 = false;
            bool f4 = false;
            bool f5 = false;
            bool f6 = false;

            val = val % 10;

            if (val == 0)
            {
                f0 = true; f1 = false; f2 = true; f3 = true; f4 = true; f5 = true; f6 = true;
            }
            else if (val == 1)
            {
                f0 = false; f1 = false; f2 = false; f3 = true; f4 = false; f5 = true; f6 = false; 
            }
            else if (val == 2)
            {
                f0 = true; f1 = true; f2 = true; f3 = false; f4 = true; f5 = true; f6 = false;
            }
            else if (val == 3)
            {
                f0 = true; f1 = true; f2 = true; f3 = true; f4 = false; f5 = true; f6 = false;
            }
            else if (val == 4)
            {
                f0 = false; f1 = true; f2 = false; f3 = true; f4 = false; f5 = true; f6 = true;
            }
            else if (val == 5)
            {
                f0 = true; f1 = true; f2 = true; f3 = true; f4 = false; f5 = false; f6 = true;
            }
            else if (val == 6)
            {
                f0 = true; f1 = true; f2 = true; f3 = true; f4 = true; f5 = false; f6 = true;
            }
            else if (val == 7)
            {
                f0 = true; f1 = false; f2 = false; f3 = true; f4 = false; f5 = true; f6 = true;
            }
            else if (val == 8)
            {
                f0 = true; f1 = true; f2 = true; f3 = true; f4 = true; f5 = true; f6 = true;
            }
            else if (val == 9)
            {
                f0 = true; f1 = true; f2 = true; f3 = true; f4 = false; f5 = true; f6 = true;
            }

            Color coloron = Color.FromArgb(255, 229, 229, 229);

            seg0.Fill = new SolidColorBrush(f0 ? coloron : Colors.Black);
            seg1.Fill = new SolidColorBrush(f1 ? coloron : Colors.Black);
            seg2.Fill = new SolidColorBrush(f2 ? coloron : Colors.Black);
            seg3.Fill = new SolidColorBrush(f3 ? coloron : Colors.Black);
            seg4.Fill = new SolidColorBrush(f4 ? coloron : Colors.Black);
            seg5.Fill = new SolidColorBrush(f5 ? coloron : Colors.Black);
            seg6.Fill = new SolidColorBrush(f6 ? coloron : Colors.Black);

        }

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double scalex, scaley, scale;

            scalex = this.ActualWidth / 291;
            scaley = this.ActualHeight / 98;

            if (scalex < scaley) scale = scalex;
            else scale = scaley;

            
            scaleTransform.ScaleX = scalex;
            scaleTransform.ScaleY = scaley;
        }

        
    }
}
