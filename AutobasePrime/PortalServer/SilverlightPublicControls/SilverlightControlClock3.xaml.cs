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
    public partial class SilverlightControlClock3 : UserControl
    {
        System.Windows.Threading.DispatcherTimer timer;

        public SilverlightControlClock3()
        {
            InitializeComponent();

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            this.NeedleHour.RenderTransform = rotateHour;
            this.NeedleMinute.RenderTransform = rotateMinute;

            rotateHour.CenterX = 1.93;
            rotateHour.CenterY = 28.0;

            rotateMinute.CenterX = 1.93;
            rotateMinute.CenterY = 1.5;

            this.ClockRoot.RenderTransform = scaleTransform;

            this.LayoutRoot.Background = null;

            CalcAngle();    // 화면이 그려지기 전에 미리 각도를 계산한다.
        }

        ScaleTransform scaleTransform = new ScaleTransform();

        RotateTransform rotateHour = new RotateTransform();
        RotateTransform rotateMinute = new RotateTransform();

        void CalcAngle()
        {
            DateTime t = DateTime.Now;

            double angle = (t.Minute * 60 + t.Second) * 360.0 / 3600.0;
            angle += 180;

            rotateMinute.Angle = angle;

            angle = ((t.Hour % 12) * 60 + t.Minute) * 360.0 / (12 * 60);
            rotateHour.Angle = angle;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            CalcAngle();
        }

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //this.ClockRoot.Width = 130;
            //this.ClockRoot.Height = 130;

            double scalex, scaley, scale;

            scalex = this.ActualWidth / 130;
            scaley = this.ActualHeight / 130;

            if (scalex < scaley) scale = scalex;
            else scale = scaley;

            //scaleTransform.CenterX = 65;
            //scaleTransform.CenterY = 65;
            scaleTransform.ScaleX = scalex;
            scaleTransform.ScaleY = scaley;
        }

    }
}
