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
using System.Collections.Generic;

namespace SilverlightAutoLibLocal
{
    public class DataTable
    {
        public string DataTableName;


        public List<DataRow> Rows = new List<DataRow>();
        public List<string> Columns = new List<string>();
    }
}
