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
using AutoLibLocal;
using AutoLib;

namespace SilverlightGraphicModule
{
    public partial class ControlBasicGraph : UserControl
    {
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        List<object> blockMember = new List<object>();
        ControlMultiGraph formChild;
        ObjectArgsMultiGraph objArgs = new ObjectArgsMultiGraph();

        public ControlBasicGraph()
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

            objArgs.wShowUnit = 60;
            objArgs.nDataTime = 1000;
            OptionMultiGraph option = new OptionMultiGraph();

            option.nDataTime = objArgs.nDataTime;

            formChild = new ControlMultiGraph(objArgs.wShowUnit, blockMember, 3, 10, 5, DateTime.Now, 2, new BrushSolid(Colors.LightGray), new BrushSolid(Colors.White), Colors.LightGray, EnumDisplayFlag.DESX | EnumDisplayFlag.DESY, option, true);

            this.gridGraph.Children.Add(formChild);

            formChild.Width = Double.NaN;
            formChild.Height = Double.NaN;

            timer.Interval = new TimeSpan(1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            if (NetTools.Tools.IsLangKorean())
            {
                textBlockTrendView.Text = "트랜드보기";
                buttonTrendHour1.Content = "1시간";
                buttonTrendHour8.Content = "8시간";
                buttonTrendHour24.Content = "1일";
                buttonTrendHour48.Content = "2일";
                buttonTrendHour72.Content = "3일";
                buttonTrendDay7.Content = "7일";
                buttonTrendDay15.Content = "15일";
                buttonTrendDay30.Content = "30일";
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            formChild.Timer();
        }

        void GoTrend(int hours)
        {
            string tag =  System.Windows.Browser.HtmlPage.Document.QueryString["Tag"];

            string page = String.Format("BasicTrend.basic&Tag={0}&Hours={1}", tag, hours);

            string url = MakeFilePath.SilverlightGraphicModule(page);
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(url, UriKind.Absolute));
        }

        private void buttonTrendHour1_Click(object sender, RoutedEventArgs e)
        {
            GoTrend(1);   
        }

        private void buttonTrendHour8_Click(object sender, RoutedEventArgs e)
        {
            GoTrend(8);   
        }

        private void buttonTrendHour24_Click(object sender, RoutedEventArgs e)
        {
            GoTrend(24);   
        }

        private void buttonTrendHour48_Click(object sender, RoutedEventArgs e)
        {
            GoTrend(2*24);   
        }

        private void buttonTrendHour72_Click(object sender, RoutedEventArgs e)
        {
            GoTrend(3 * 24);   
        }

        private void buttonTrendDay7_Click(object sender, RoutedEventArgs e)
        {
            GoTrend(7 * 24);   
        }

        private void buttonTrendDay15_Click(object sender, RoutedEventArgs e)
        {
            GoTrend(15 * 24);   
        }

        private void buttonTrendDay30_Click(object sender, RoutedEventArgs e)
        {
            GoTrend(30 * 24);   
        }

    }
}
