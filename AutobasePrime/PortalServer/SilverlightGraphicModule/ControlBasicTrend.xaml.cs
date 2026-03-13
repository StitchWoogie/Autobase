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

namespace SilverlightGraphicModule
{
    public partial class ControlBasicTrend : UserControl
    {
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        List<object> blockMember = new List<object>();
        ControlMultiTrend formChild;
        ObjectArgsMultiTrend objArgs = new ObjectArgsMultiTrend();
        DateTime tNow = DateTime.Now;

        public ControlBasicTrend()
        {
            InitializeComponent();

            this.Width = Double.NaN;        // 자동 사이즈로 만든다.
            this.Height = Double.NaN;       // 자동 사이즈로 만든다.

            PUBLIC_GRAPH_MEMBER member = new PUBLIC_GRAPH_MEMBER();
            List<object> list = new List<object>();

            member.tag = System.Windows.Browser.HtmlPage.Document.QueryString["Tag"];
            member.color = Colors.Green;
            member.nLineThick = 2;
            member.nLevelTo = 100;
            member.nAxisPosition = 1;

            blockMember.Add(member);

            int totalhours;

            try
            {
                totalhours = NetTools.ConvertTool.ToInt32(System.Windows.Browser.HtmlPage.Document.QueryString["Hours"]);
            }
            catch
            {
                totalhours = 1;
            }

            objArgs.wShowUnit = totalhours * 60;

            int days = (totalhours / 24);
            int hours = (totalhours % 24);
            int gab = days < 2 ? 1 : days;

            this.textBoxDay.Text = days.ToString();
            this.textBoxHour.Text = hours.ToString();
            this.textBoxGab.Text = gab.ToString();

            formChild = new ControlMultiTrend(objArgs.wShowUnit, blockMember, 3, objArgs.wShowUnit/10, 5, tNow, 0, new BrushSolid(Colors.LightGray), new BrushSolid(Colors.White), Colors.LightGray, EnumDisplayFlag.DESX | EnumDisplayFlag.DESY, true, gab);

            this.gridGraph.Children.Add(formChild);

            formChild.Width = Double.NaN;
            formChild.Height = Double.NaN;


            if (NetTools.Tools.IsLangKorean())
            {
                textBlockDataSize.Text = "자료범위";

                textBlockDay.Text = "일";
                textBlockHour.Text = "시간";

                textBlockInterval.Text = "읽기간격";
                textBlockMinute.Text = "분";

                buttonReLoad.Content = "다시읽기";

                TrendHour1.Content = "1시간";
                TrendHour8.Content = "8시간";
                TrendHour24.Content = "1일";
                TrendHour48.Content = "2일";
                TrendHour72.Content = "3일";
                buttonTrendDay7.Content = "7일";
                buttonTrendDay15.Content = "15일";
                buttonTrendDay30.Content = "30일";

                buttonPrevData.Content = "이전자료";
                buttonNextData.Content = "다음자료";
            }
        }

        int GetMinutes()
        {
            int minutes = NetTools.ConvertTool.ToInt32(this.textBoxDay.Text) * 24 * 60 + NetTools.ConvertTool.ToInt32(this.textBoxHour.Text) * 60;

            return minutes;
        }

        void ReLoadTrend()
        {
            int minutes = GetMinutes();
            int gab = NetTools.ConvertTool.ToInt32(this.textBoxGab.Text);
            
            formChild.SetShowUnit(minutes);
            formChild.nDataGab = gab;
            DateTime t = tNow.AddMinutes(-minutes);
            formChild.SetStartTime(t);
            formChild.ReadAllPoint();
        }

        private void TrendHour1_Click(object sender, RoutedEventArgs e)
        {
            this.textBoxDay.Text = "0";
            this.textBoxHour.Text = "1";
            this.textBoxGab.Text = "1";
            tNow = DateTime.Now;
            ReLoadTrend();
        }

        private void TrendHour8_Click(object sender, RoutedEventArgs e)
        {
            this.textBoxDay.Text = "0";
            this.textBoxHour.Text = "8";
            this.textBoxGab.Text = "1";
            tNow = DateTime.Now;
            ReLoadTrend();
        }

        private void TrendHour24_Click(object sender, RoutedEventArgs e)
        {
            this.textBoxDay.Text = "1";
            this.textBoxHour.Text = "0";
            this.textBoxGab.Text = "1";
            tNow = DateTime.Now;
            ReLoadTrend();
        }

        private void TrendHour48_Click(object sender, RoutedEventArgs e)
        {
            this.textBoxDay.Text = "2";
            this.textBoxHour.Text = "0";
            this.textBoxGab.Text = "2";
            tNow = DateTime.Now;
            ReLoadTrend();
        }

        private void TrendHour72_Click(object sender, RoutedEventArgs e)
        {
            this.textBoxDay.Text = "3";
            this.textBoxHour.Text = "0";
            this.textBoxGab.Text = "3";
            tNow = DateTime.Now;
            ReLoadTrend();
        }

        private void buttonTrendDay7_Click(object sender, RoutedEventArgs e)
        {
            this.textBoxDay.Text = "7";
            this.textBoxHour.Text = "0";
            this.textBoxGab.Text = "7";
            tNow = DateTime.Now;
            ReLoadTrend();
        }

        private void buttonTrendDay15_Click(object sender, RoutedEventArgs e)
        {
            this.textBoxDay.Text = "15";
            this.textBoxHour.Text = "0";
            this.textBoxGab.Text = "15";
            tNow = DateTime.Now;
            ReLoadTrend();
        }

        private void TrendDay30_Click(object sender, RoutedEventArgs e)
        {
            this.textBoxDay.Text = "30";
            this.textBoxHour.Text = "0";
            this.textBoxGab.Text = "30";
            tNow = DateTime.Now;
            ReLoadTrend();
        }

        private void buttonReLoad_Click(object sender, RoutedEventArgs e)
        {
            ReLoadTrend();
        }

        private void buttonPrevData_Click(object sender, RoutedEventArgs e)
        {
            tNow = tNow.AddMinutes(-GetMinutes());
            ReLoadTrend();
        }

        private void buttonNextData_Click(object sender, RoutedEventArgs e)
        {
            tNow = tNow.AddMinutes(GetMinutes());
            ReLoadTrend();
        }
        
    }
}
