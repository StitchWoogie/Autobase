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

namespace SilverlightApplication1
{
    public partial class Page : UserControl
    {
        System.Windows.Threading.DispatcherTimer timer;
        System.Collections.ObjectModel.ObservableCollection<Data> source;

        public Page()
        {
            InitializeComponent();

            source = new System.Collections.ObjectModel.ObservableCollection<Data>();

            int itemsCount = 1;

            for (int i = 0; i < itemsCount; i++)
            {
                source.Add(new Data(){ FirstName="First", LastName="Last", Age=i,Available=(i%2==0), textbox=new TextBox()});
            }
            //dg.ItemsSource = source;

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
            TextBox t = new TextBox();
            t.Text = "Hello00";
            LayoutRoot.Children.Add(t);

            Grid.SetColumn(t, 0);
            Grid.SetRow(t, 0);

            TextBox t2 = new TextBox();
            t2.Text = "Hello11";
            LayoutRoot.Children.Add(t2);

            Grid.SetColumn(t2, 1);
            Grid.SetRow(t2, 1);
        }

        int count = 0;

        void timer_Tick(object sender, EventArgs e)
        {
            count++;
            source[0].Age++;

            //source[0].textbox.Text = source[0].Age.ToString();
            //dg.Visibility = Visibility.Collapsed;
            //dg.ItemsSource = source;
            //dg.Visibility = Visibility.Visible;
            //Data item = new Data() { FirstName = "First", LastName = "Last", Age = count, Available = (count % 2 == 0), textbox = new TextBox() };

            
        }
    }
}
