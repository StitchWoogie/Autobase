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
using System.Xml.Linq;
using AutoLib;
using System.Windows.Data;

namespace SilverlightGraphicModule
{
    public partial class WndDatabase : UserControl
    {
        //int nUpdateTimeCurrent;
        int nOldSec;
        //private System.Windows.Forms.ContextMenu contextMenu1;
        //private System.Windows.Forms.MenuItem menuItemOption;
        //private System.Windows.Forms.MenuItem menuItem1;
        //private System.Windows.Forms.MenuItem menuItem2;
        System.Windows.Threading.DispatcherTimer timer1 = new System.Windows.Threading.DispatcherTimer();

        public WndDatabase()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            sClassName = "DefaultClassHistoryWindow";

            //nRecordCount = 0;
            m_bUseNo = true;
            sTableName = "";

            sSqlTextWhere = "";
            sSqlTextOrderBy = "";
            SetSqlText("", "");

            //objectParent = null;

            //nUpdateTimeCurrent = 0;

            DateTime t = DateTime.Now;
            nOldSec = t.Second;

            m_list.SelectionChanged += new SelectionChangedEventHandler(m_list_SelectionChanged);
        }

        public delegate void DelegateSelChange();
        public DelegateSelChange procSelChange = null;

        /*
        private void m_list_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (m_list.SelectedItems.Count == 0) return;

            if (procSelChange != null)
                procSelChange();
        }*/

        void m_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_list.SelectedItems.Count == 0) return;

            if (procSelChange != null)
                procSelChange();
        }

        List<object> blockField = new List<object>();
        public bool m_bUseNo;
        XDocument rstHistory = new XDocument();
        //int nRecordCount;
        public bool bAutoUpdate;
        public bool bUseGrid;
        public bool bUseFullCursor;
        public int nUpdateTime;

        void DataSetToListView()
        {
            FIELD_VIEW_STRUCT view;
            //HorizontalAlignment align;

            /*
            int old_index = -1;
            if (m_list.SelectedItems.Count > 0)
            {
                old_index = m_list.SelectedItems[0].Index;
            }*/

            //m_list.Visible = false;
            m_list.Columns.Clear();
            //m_list.Items.Clear();

            for (int i = 0; i < blockField.Count; i++)
            {
                view = (FIELD_VIEW_STRUCT)blockField[i];
                if (view.active == 0) continue;

                //if (view.cAlign == 0) align = HorizontalAlignment.Left;
                //else if (view.cAlign == 1) align = HorizontalAlignment.Center;
                //else align = HorizontalAlignment.Right;

                DataGridColumn column = new DataGridTextColumn();
                column.Width = new DataGridLength(view.width);
                m_list.Columns.Add(column);
                //m_list.Columns.Add(new datagridcolunview.DispName, view.width, align);

                /*
                if (rstHistory.Tables.Count > 0)
                    view.nFieldPosOnDs = rstHistory.Tables[0].Columns.IndexOf(view.FieldName);
                else
                    view.nFieldPosOnDs = -1;*/
            }

            //m_list.ItemsSource = rstHistory;
            /*
            if (rstHistory.Tables.Count > 0)
            {
                DataRow row;
                ListViewItem item;
                string data;
                int count = 0;

                for (int i = 0; i < rstHistory.Tables[0].Rows.Count; i++)
                {
                    row = rstHistory.Tables[0].Rows[i];
                    item = new ListViewItem();

                    count = 0;
                    for (int j = 0; j < blockField.Count; j++)
                    {
                        view = (FIELD_VIEW_STRUCT)blockField[j];

                        if (view.active == 0)
                        {
                            continue;
                        }
                        else
                        {
                            if (view.FieldName == "_local_number_")
                            {
                                data = (i + 1).ToString();
                            }
                            else
                            {
                                if (view.nFieldPosOnDs == -1)
                                {
                                    data = "";
                                }
                                else
                                {
                                    data = row[view.nFieldPosOnDs].ToString();
                                }
                            }
                        }

                        if (count == 0) item.Text = data;
                        else item.SubItems.Add(data);

                        count++;
                    }

                    //item.ForeColor = Color.Red;
                    //item.BackColor = Color.Blue;

                    m_list.Items.Add(item);
                }
            }

            if (old_index != -1 && old_index < m_list.Items.Count)
            {
                m_list.Items[old_index].Selected = true;
            }

            m_list.Visible = true;
             */

            /*
            FIELD_VIEW_STRUCT view;
            HorizontalAlignment align;

            int old_index = -1;
            if (m_list.SelectedItems.Count > 0)
            {
                old_index = m_list.SelectedItems[0].Index;
            }

            m_list.Visible = false;
            m_list.Columns.Clear();
            m_list.Items.Clear();

            for (int i = 0; i < blockField.Count; i++)
            {
                view = (FIELD_VIEW_STRUCT)blockField[i];
                if (view.active == 0) continue;

                if (view.cAlign == 0) align = HorizontalAlignment.Left;
                else if (view.cAlign == 1) align = HorizontalAlignment.Center;
                else align = HorizontalAlignment.Right;

                m_list.Columns.Add(view.DispName, view.width, align);
                if (rstHistory.Tables.Count > 0)
                    view.nFieldPosOnDs = rstHistory.Tables[0].Columns.IndexOf(view.FieldName);
                else
                    view.nFieldPosOnDs = -1;
            }

            if (rstHistory.Tables.Count > 0)
            {
                DataRow row;
                ListViewItem item;
                string data;
                int count = 0;

                for (int i = 0; i < rstHistory.Tables[0].Rows.Count; i++)
                {
                    row = rstHistory.Tables[0].Rows[i];
                    item = new ListViewItem();

                    count = 0;
                    for (int j = 0; j < blockField.Count; j++)
                    {
                        view = (FIELD_VIEW_STRUCT)blockField[j];

                        if (view.active == 0)
                        {
                            continue;
                        }
                        else
                        {
                            if (view.FieldName == "_local_number_")
                            {
                                data = (i + 1).ToString();
                            }
                            else
                            {
                                if (view.nFieldPosOnDs == -1)
                                {
                                    data = "";
                                }
                                else
                                {
                                    data = row[view.nFieldPosOnDs].ToString();
                                }
                            }
                        }

                        if (count == 0) item.Text = data;
                        else item.SubItems.Add(data);

                        count++;
                    }

                    //item.ForeColor = Color.Red;
                    //item.BackColor = Color.Blue;

                    m_list.Items.Add(item);
                }
            }

            if (old_index != -1 && old_index < m_list.Items.Count)
            {
                m_list.Items[old_index].Selected = true;
            }

            m_list.Visible = true;*/
        }

        void OpenDataBase()
        {
            blockField.Clear();
            FIELD_VIEW_STRUCT view;

            if (m_bUseNo)
            {
                view = new FIELD_VIEW_STRUCT();
                view.FieldName = "_local_number_";
                view.DispName = "NO";
                view.active = 1;
                view.width = 100;
                blockField.Add(view);
            }

            Open();

            /*
            if (rstHistory.Tables.Count == 0)
            {
                nRecordCount = 0;
                for (int i = 0; i < 3; i++)
                {
                    view = new FIELD_VIEW_STRUCT();
                    view.FieldName = String.Format("Field{0}", i + 1);
                    view.DispName = view.FieldName;
                    view.active = 1;
                    view.width = 100;
                    blockField.Add(view);
                }
            }
            else
            {

                nRecordCount = rstHistory.Tables[0].Rows.Count;

                //ClassAdoTableDef dbTable(m_db);

                int count = rstHistory.Tables[0].Columns.Count;
                DataColumn col;

                for (int i = 0; i < count; i++)
                {
                    view = new FIELD_VIEW_STRUCT();
                    col = rstHistory.Tables[0].Columns[i];
                    view.FieldName = col.ColumnName;
                    view.DispName = view.FieldName;
                    view.active = 1;
                    view.width = 100;
                    blockField.Add(view);
                }
            }

            LoadClassConfig();

            DataSetToListView();*/
        }

        public string sFileName;
        public string sTableName;
        public string sDsnName;
        public int bMdbOrString = 0;
        string sSqlTextWhere;
        string sSqlTextOrderBy;

        void Open()
        {
            // rstHistory = new XDocument();

            string sort;
            sort = String.Format("SELECT * FROM {0}", sTableName);
            if (sSqlTextWhere.Length != 0)
            {
                sort += String.Format(" WHERE {0}", sSqlTextWhere);
            }
            if (sSqlTextOrderBy.Length != 0)
            {
                sort += String.Format(" ORDER BY {0}", sSqlTextOrderBy);
            }

            if (bMdbOrString == 0)
            {
                SilverlightAutoLibLocal.ServiceReferenceDataSet2.WebServiceDataSet2SoapClient service = SilverlightAutoLibLocal.ServiceLib.GetServiceDataSet2();

                service.GetDataSetFromMdbCompleted += new EventHandler<SilverlightAutoLibLocal.ServiceReferenceDataSet2.GetDataSetFromMdbCompletedEventArgs>(service_GetDataSetFromMdbCompleted);

                service.GetDataSetFromMdbAsync(sFileName, sort);
            }
            else
            {
                SilverlightAutoLibLocal.ServiceReferenceDataSet2.WebServiceDataSet2SoapClient service = SilverlightAutoLibLocal.ServiceLib.GetServiceDataSet2();

                service.GetDataSetFromDsnCompleted += new EventHandler<SilverlightAutoLibLocal.ServiceReferenceDataSet2.GetDataSetFromDsnCompletedEventArgs>(service_GetDataSetFromDsnCompleted);

                service.GetDataSetFromMdbAsync(sDsnName, sort);
            }
        }

        void service_GetDataSetFromDsnCompleted(object sender, SilverlightAutoLibLocal.ServiceReferenceDataSet2.GetDataSetFromDsnCompletedEventArgs e)
        {
            if (e.Result == null) return;

            DataCompleted(e.Result);
        }

        void service_GetDataSetFromMdbCompleted(object sender, SilverlightAutoLibLocal.ServiceReferenceDataSet2.GetDataSetFromMdbCompletedEventArgs e)
        {
            if (e.Result == null) return;

            DataCompleted(e.Result);
        }

        XElement rootElement;

        void DataCompleted(string data)
        {
            XDocument xd = XDocument.Parse(data);

            //XElement root = xd.Root;

            XElement root = xd.Root;

            rootElement = root;

            m_list.Columns.Clear();
            
            bool column_maked = false;

            List<object> l = new List<object>();

            /*
            for (int i = 0; i < 100; i++)
            {
                Dictionary<string, string> s = new Dictionary<string, string>();
                s.Add("Hello", "Good");
                s.Add("By", i.ToString());
                l.Add(s);
            }*/

            m_list.AutoGenerateColumns = false;

			foreach(XElement el in root.Elements()) 
			{
                if (!column_maked)
                {
                    foreach (XElement n in el.Elements())
                    {
                        DataGridTextColumn col = new DataGridTextColumn();
                        col.Header = n.Name.LocalName;
                        indexingConverter convert = new indexingConverter();
                        col.Binding = new Binding { Converter = convert, ConverterParameter = n.Name.LocalName };
                        m_list.Columns.Add(col);
                    }
                    column_maked = true;
                }

                Dictionary<string, string> s = new Dictionary<string, string>();

                foreach (XElement n in el.Elements())
                {
                    s.Add(n.Name.LocalName, n.Value);
                }

                l.Add(s);

			}

            m_list.ItemsSource = null;
            m_list.ItemsSource = l;
		}

        class indexingConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                Dictionary<string, string> columnData = (Dictionary<string, string>)value; return columnData[parameter.ToString()];
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        } 
        
        string sClassName;

        public void SetClassName(string class_name)
        {
            sClassName = class_name;
        }
        /*
        bool GetKeepCurrentConfig()
        {
            string filename = String.Format("{0}\\Config\\Database\\Member\\{1}.inix", TotalConfig.sDirWorkProject, sClassName);

            int val = Profile.GetPrivateProfileIntW("Config", "KeepCurrent", 0, filename);

            return (val == 1);
        }

        void SetKeepCurrentConfig(bool val)
        {
            string filename = String.Format("{0}\\Config\\Database\\Member\\{1}.inix", TotalConfig.sDirWorkProject, sClassName);

            Profile.WritePrivateProfileIntW("Config", "KeepCurrent", val ? 1 : 0, filename);
        }

        bool GetUseWebConfig()
        {
            string filename = String.Format("{0}\\Config\\Database\\Member\\{1}.inix", TotalConfig.sDirWorkProject, sClassName);

            int val = Profile.GetPrivateProfileIntW("Config", "UseWebConfig", 0, filename);

            return (val == 1);
        }

        void SetUseWebConfig(bool val)
        {
            string filename = String.Format("{0}\\Config\\Database\\Member\\{1}.inix", TotalConfig.sDirWorkProject, sClassName);

            Profile.WritePrivateProfileIntW("Config", "UseWebConfig", val ? 1 : 0, filename);
        }

        void SaveClassConfigColumn()
        {
            ListWidthToConfig();

            string path = TotalConfig.sDirWorkProject + "\\Config\\Database\\Member";

            Directory.CreateDirectory(path);

            string filename = path + "\\" + sClassName;

            FIELD_VIEW_STRUCT view;
            int i;

            FileStream s = File.Open(filename, FileMode.Create);
            TextWriter writer = new StreamWriter(s);

            for (i = 0; i < blockField.Count; i++)
            {
                view = (FIELD_VIEW_STRUCT)blockField[i];
                writer.WriteLine("{0},{1},{2},{3},{4},", view.FieldName, view.DispName, view.active, view.width, view.cAlign);
            }
            writer.Close();
        }

        // 외부에서 사용
        public void SaveClassConfig()
        {
            if (GetKeepCurrentConfig()) return;		// 현재의 설정을 유지해야 하므로 저장하지 않는다.
            SaveClassConfigColumn();
        }

        void ListWidthToConfig()
        {
            FIELD_VIEW_STRUCT view;
            string column;
            for (int j = 0; j < m_list.Columns.Count; j++)
            {
                column = m_list.Columns[j].Text;

                for (int i = 0; i < blockField.Count; i++)
                {
                    view = (FIELD_VIEW_STRUCT)blockField[i];
                    if (view.DispName != column) continue;
                    view.width = m_list.Columns[j].Width;
                    break;
                }
            }
        }

        void LoadClassConfig()
        {
            string filename;

            if (GetUseWebConfig())
            {
                string path = MakeFilePath.Project("Config\\Database\\Member", sClassName);
                filename = path;
            }
            else
            {
                string path = TotalConfig.sDirWorkProject + "\\Config\\Database\\Member";
                filename = path + "\\" + sClassName;
            }

            if (!File.Exists(filename)) return;

            string one_line;
            string column = "";
            int i;
            FIELD_VIEW_STRUCT view;

            FileStream s = File.OpenRead(filename);
            TextReader reader = new StreamReader(s);
            CommaBlockString comma = new CommaBlockString();

            while (true)
            {
                one_line = reader.ReadLine();

                if (one_line == null) break;
                comma.Set(one_line);
                comma.GetString(ref column);

                for (i = 0; i < blockField.Count; i++)
                {
                    view = (FIELD_VIEW_STRUCT)blockField[i];
                    if (view.FieldName != column) continue;

                    comma.GetString(ref view.DispName);
                    comma.GetChar(ref view.active);
                    comma.GetInt(ref view.width);
                    comma.GetChar(ref view.cAlign);
                    break;
                }

            }

            reader.Close();

            bool active_flag = false;

            for (i = 0; i < (int)blockField.Count; i++)
            {
                view = (FIELD_VIEW_STRUCT)blockField[i];
                if (view.active == 1)
                {
                    active_flag = true;
                    break;
                }
            }

            // 모든 필드가 활성화가 아닐때는 모두 활성화시킨다.
            if (active_flag == false)
            {
                for (i = 0; i < blockField.Count; i++)
                {
                    view = (FIELD_VIEW_STRUCT)blockField[i];
                    view.active = 1;
                }
            }
        }*/

        public void SetSqlText(string where, string orderby)
        {
            sSqlTextWhere = where;
            sSqlTextOrderBy = orderby;
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            //m_list.FullRowSelect = bUseFullCursor;
            //m_list.GridLines = bUseGrid;

            OpenDataBase();

            timer1.Start();
        }

        /*
         * 
        private void menuItemDeleteOne_Click(object sender, System.EventArgs e)
        {

        }

        private void menuItemDeleteAll_Click(object sender, System.EventArgs e)
        {

        }

        private void menuItemOption_Click(object sender, System.EventArgs e)
        {
            FormDatabaseOption dialog = new FormDatabaseOption();

            this.ListWidthToConfig();

            dialog.blockTemp = (ArrayList)Tools.CopyObject(blockField);

            dialog.checkBoxKeepCurrentSet.Checked = this.GetKeepCurrentConfig();
            dialog.checkBoxUseWebConfig.Checked = this.GetUseWebConfig();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FIELD_VIEW_STRUCT source;
                FIELD_VIEW_STRUCT target;
                for (int i = 0; i < blockField.Count; i++)
                {
                    target = (FIELD_VIEW_STRUCT)blockField[i];
                    source = (FIELD_VIEW_STRUCT)dialog.blockTemp[i];

                    target.active = source.active;
                    target.cAlign = source.cAlign;
                    target.DispName = source.DispName;
                }

                this.SetKeepCurrentConfig(dialog.checkBoxKeepCurrentSet.Checked);
                this.SetUseWebConfig(dialog.checkBoxUseWebConfig.Checked);
                DataSetToListView();
                SaveClassConfigColumn();
            }
        }

        bool bWorkingFlag = false;

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if (bWorkingFlag) return;
            if (!bAutoUpdate) return;

            DateTime t = DateTime.Now;

            if (t.Second == nOldSec) return;

            if (t.Second > nOldSec)
            {
                nUpdateTimeCurrent += (t.Second - nOldSec);
            }
            else
            {
                nUpdateTimeCurrent += (t.Second + 60 - nOldSec);
            }

            nOldSec = t.Second;

            if (nUpdateTimeCurrent < nUpdateTime)
            {
                return;
            }

            nUpdateTimeCurrent = 0;

            ReLoad();
        }
*/
        public void ReLoad()
        {
            // TODO: Add your message handler code here and/or call default
            OpenDataBase();
        }
        
        public int GetCurSel()
        {
            if (m_list.SelectedItems.Count == 0) return -1;
            return m_list.SelectedIndex;
            //return m_list.SelectedItems[0].Index;
        }

        public void SetCurSel(int index)
        {
            //if (index >= m_list.Items.Count) return;
            m_list.SelectedIndex = index;
            
            //m_list.Items[index].Selected = true;
        }
        
        public string GetValue(int index, string col_name)
        {
            if (index == -1) return "";

            int pos = 0;
            foreach (XElement el in rootElement.Elements())
            {
                if (pos == index)
                {
                    XElement e = el.Element(col_name);
                    if (e == null) return "";
                    return e.Value;
                }
                pos++;
            }

            return "";
        }
        
        public void SetConnection(string connection, string table)
        {
            sFileName = connection;
            sTableName = table;
            OpenDataBase();
        }
        
        public void SetTable(string table)
        {
            sTableName = table;
            OpenDataBase();
        }
        /*
        public delegate void DelegateSelChange();
        public DelegateSelChange procSelChange = null;

        private void m_list_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (m_list.SelectedItems.Count == 0) return;

            if (procSelChange != null)
                procSelChange();
        }

        public void SetTextColor(Color color)
        {
            this.m_list.ForeColor = color;
        }

        public void SetBackColor(Color color)
        {
            this.m_list.BackColor = color;
        }*/
    }
}
