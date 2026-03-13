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
using NetTools;

namespace SilverlightGraphicModule
{
    public partial class DialogGraph : UserControl
    {
        public DialogGraph()
        {
            InitializeComponent();

            LinearGradientBrush brush = new LinearGradientBrush();
            brush.EndPoint = new Point(0.5, 1);
            brush.StartPoint = new Point(0.5, 0);

            brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xB2, 0xCE, 0xCD, 0xCD), Offset = 1 });
            brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(0xD8, 0xFF, 0xFF, 0xFF), Offset = 0 });

            this.LayoutRoot.Background = brush; //new LinearGradientBrush new SolidColorBrush(Color.FromArgb(220, 220, 220, 220));

            if (NetTools.Tools.IsLangKorean())
            {
                checkBoxUseDisplayPointTime.Content = "선택한 시점 시간 표시";
                buttonOK.Content = "확인";
                buttonCancel.Content = "취소";
                radioButtonStartType0.Content = "자동";
                radioButtonStartType1.Content = "날짜 선택";

                textBlockYear.Text = "년";
                textBlockMonth.Text = "월";
                textBlockDay.Text = "일";
                textBlockHour.Text = "시";
                textBlockMinute.Text = "분";
            }
        }

        void EnableDisable()
        {
            bool flag = (this.radioButtonStartType1.IsChecked == true);
            textBoxYear.IsEnabled = flag;
            textBoxMonth.IsEnabled = flag;
            textBoxDay.IsEnabled = flag;
            textBoxHour.IsEnabled = flag;
            textBoxMinute.IsEnabled = flag;
            buttonToLeft.IsEnabled = flag;
            buttonToRight.IsEnabled = flag;
        }

        private void radioButtonStartType0_Checked(object sender, RoutedEventArgs e)
        {
            EnableDisable();
        }

        private void radioButtonStartType1_Checked(object sender, RoutedEventArgs e)
        {
            EnableDisable();
        }

        public int nTimeType;
        public int nShowUnit;
        public int nDataCycle = 1;

        void ShiftTime(int shift)
        {
            int data_cycle = shift * nDataCycle;
            DateTime t;

            t = new DateTime(ConvertTool.ToInt32(textBoxYear.Text),
                    ConvertTool.ToInt32(textBoxMonth.Text),
                    ConvertTool.ToInt32(textBoxDay.Text),
                    ConvertTool.ToInt32(textBoxHour.Text),
                    ConvertTool.ToInt32(textBoxMinute.Text),
                    0);

            if (nTimeType == 0)
            {	// milli data
                t = t.AddMilliseconds(data_cycle);
            }
            else if (nTimeType == 1)
            {	// sec data
                t = t.AddSeconds(data_cycle);
            }
            else if (nTimeType == 2)
            {	// min data
                t = t.AddMinutes(data_cycle);
            }
            else if (nTimeType == 3)
            {	// hour data
                t = t.AddHours(data_cycle);
            }
            else if (nTimeType == 4)
            {	// day data
                t = t.AddDays(data_cycle);
            }
            else if (nTimeType == 5)
            {	// month data
                t = t.AddMonths(data_cycle);
            }
            else
            {	// year data
                t = t.AddYears(data_cycle);
            }

            textBoxYear.Text = t.Year.ToString();
            textBoxMonth.Text = t.Month.ToString();
            textBoxDay.Text = t.Day.ToString();
            textBoxHour.Text = t.Hour.ToString();
            textBoxMinute.Text = t.Minute.ToString();
        }

        private void buttonToLeft_Click(object sender, RoutedEventArgs e)
        {
            ShiftTime(-nShowUnit);
        }

        private void buttonToRight_Click(object sender, RoutedEventArgs e)
        {
            ShiftTime(+nShowUnit);
        }

        
    }
}
