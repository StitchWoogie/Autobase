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
    public partial class SilverlightControlClock2 : UserControl
    {
        System.Windows.Threading.DispatcherTimer timer;

        public SilverlightControlClock2()
        {
            InitializeComponent();

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        RotateTransform rotateHour = new RotateTransform();
        RotateTransform rotateMinute = new RotateTransform();

        void timer_Tick(object sender, EventArgs e)
        {
            DateTime t = DateTime.Now;

            double angle = (t.Minute * 60 + t.Second) * 360 / 3600;

            rotateMinute.Angle = angle;

            
        }
    }
}
