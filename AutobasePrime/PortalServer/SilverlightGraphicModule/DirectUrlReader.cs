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
using System.IO;

namespace SilverlightGraphicModule
{
    public class DirectUrlReader
    {
        Stream resultStream = null;

        public Stream GetStream(string url)
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);

            resultStream = null;

            Uri u = new Uri(url); 
            client.OpenReadAsync(u);

            while (true)
            {
                //System.Threading.Thread.Sleep(1);
                System.Threading.Thread.Sleep(1);
                
                if (resultStream != null) return resultStream;
            }
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            System.Windows.Resources.StreamResourceInfo info = new System.Windows.Resources.StreamResourceInfo(e.Result, null);

            resultStream = info.Stream;
        }
    }
}
