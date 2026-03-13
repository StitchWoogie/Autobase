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
    public partial class SilverlightControlClock : UserControl
    {
        public SilverlightControlClock()
        {
            InitializeComponent();
        }

        public void Canvas_Loaded(object sender, EventArgs e)
        {

            // The current date and time.
            System.DateTime date = DateTime.Now;

            // Find the appropriate angle (in degrees) for the hour hand
            // based on the current time.
            double hourangle = (((float)date.Hour) / 12) * 360 + date.Minute / 2;

            // The transform is already rotated 116.5 degrees to make the hour hand be
            // in the 12 o'clock position. You must build this already existing angle
            // into the hourangle.
            hourangle += 180;

            // The same as for the hour angle.
            double minangle = (((float)date.Minute) / 60) * 360;
            minangle += 180;

            // The same for the hour angle.
            double secangle = (((float)date.Second) / 60) * 360;
            secangle += 180;

            // Set the beginning of the animation (From property) to the angle 
            // corresponging to the current time.
            hourAnimation.From = hourangle;

            // Set the end of the animation (To property)to the angle 
            // corresponding to the current time PLUS 360 degrees. Thus, the
            // animation will end after the clock hand moves around the clock 
            // once. Note: The RepeatBehavior property of the animation is set
            // to "Forever" so the animation will begin again as soon as it completes.
            hourAnimation.To = hourangle + 360;

            // Same as with the hour animation.
            minuteAnimation.From = minangle;
            minuteAnimation.To = minangle + 360;

            // Same as with the hour animation.
            secondAnimation.From = secangle;
            secondAnimation.To = secangle + 360;

            // Start the storyboard.
            clockStoryboard.Begin();

        }
    }
}
