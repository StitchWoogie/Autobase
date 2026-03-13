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

namespace NetTools
{
    public class UriSplit
    {
        Uri uri;

        public void Split(string s)
        {
            uri = new Uri(s);
        }

        public string GetDirectoryName()
        {
            string dir = String.Format("{0}://{1}:{2}{3}", uri.Scheme, uri.Host, uri.Port, System.IO.Path.GetDirectoryName(uri.LocalPath).Replace('\\', '/'));

            return dir;
        }

        public string GetFileName()
        {
            return System.IO.Path.GetFileName(uri.LocalPath);
        }

        public string GetExtension()
        {
            return System.IO.Path.GetExtension(uri.LocalPath);
        }

    }
}
