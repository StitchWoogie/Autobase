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
    public class DataRow
    {
        public List<string> Datas = new List<string>();
        public DataTable parentTable;

        public object this[string columnName] {
            get {
                for (int i = 0; i < parentTable.Columns.Count; i++)
                {
                    if (String.Compare(parentTable.Columns[i], columnName, StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        return Datas[i];
                    }
                }
                return "";
            }
            set {
                for (int i = 0; i < parentTable.Columns.Count; i++)
                {
                    if (String.Compare(parentTable.Columns[i], columnName, StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        Datas[i] = (string)value;
                        return;
                    }
                }
            }
        }
    }
}
