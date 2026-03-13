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
using System.Xml.Linq;

namespace SilverlightAutoLibLocal
{
    public class DataSet
    {
        public string DataSetName;
        public List<DataTable> Tables = new List<DataTable>();

        public void Dispose()
        {

        }

        DataTable SeekTable(string name)
        {
            DataTable dt;

            for (int i = 0; i < Tables.Count; i++)
            {
                dt = Tables[i];
                if (name == dt.DataTableName) return dt;
            }

            dt = new DataTable();
            dt.DataTableName = name;
            Tables.Add(dt);

            return dt;
        }

        public void GetDataFromString(string data)
        {
            XDocument xd = XDocument.Parse(data);
            XElement root = xd.Root;

            //this.DataSetName = root.Name.LocalName;

            DataTable dt;

            foreach (XElement el in root.Elements())
            {
                dt = SeekTable(el.Name.LocalName);
                bool first_flag = true;
                    
                foreach (XElement n in el.Elements())
                {
                    DataRow row = new DataRow();
                    row.parentTable = dt;

                    if (first_flag) // 맨처음 라인은 컬럼 정보를 만든다.
                    {
                        first_flag = false;
                        dt.Columns.Add(n.Name.LocalName);
                    }

                    row.Datas.Add(n.Value);

                    dt.Rows.Add(row);
                }

            }
        }

    }
}
