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
using NetTools.OldDefine;
using AutoLibLocal;
using System.Xml.Linq;
using NetTools;
using System.Windows.Data;

namespace SilverlightGraphicModule
{
    public class ObjectArgsDatabase
    {
        public string dsn;
        public string filename;
        public string table;
        public sbyte bUseNo;
        public Color lColorText;
        public BrushPublic lColorBack = new BrushPublic();
        public int nConnectionType;
        public sbyte bAutoUpdate;
        public bool bUseGrid;
        public sbyte bUseFullCursor;
        public int nUpdateTime;
    }

    /// <summary>
    /// Summary description for ObjectDatabase.
    /// </summary>
    public class ObjectDatabase : ObjectExpand
    {
        ObjectArgsDatabase objArgs;

        WndDatabase wndChild;

        static public List<object> arrayClassList = new List<object>();

        public ObjectArgsDatabase ObjectArgs
        {
            set
            {
                objArgs = value;
            }
            get
            {
                return objArgs;
            }
        }

        public ObjectDatabase(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsDatabase args)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.Database;
            objArgs = args;

            if (objArgs.nUpdateTime < 2) objArgs.nUpdateTime = 2;	// 최소 2초

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                wndChild = new WndDatabase();
                
                wndChild.SetClassName(general.sClassName);
                wndChild.sFileName = objArgs.filename;
                wndChild.sTableName = objArgs.table;
                wndChild.sDsnName = objArgs.dsn;
                wndChild.m_bUseNo = (objArgs.bUseNo == 1);
                wndChild.bMdbOrString = objArgs.nConnectionType;
                wndChild.bAutoUpdate = (objArgs.bAutoUpdate == 1);
                wndChild.bUseGrid = objArgs.bUseGrid;
                wndChild.bUseFullCursor = (objArgs.bUseFullCursor == 1);
                wndChild.nUpdateTime = objArgs.nUpdateTime;

                //wndChild.TopLevel = false;
                //wndChild.FormBorderStyle = FormBorderStyle.None;
                //form..Controls.Add(wndChild);
                parent_canvas.Children.Add(wndChild);

                /*
                int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                GetViewZone(ref x1, ref y1, ref x2, ref y2);
                wndChild.Left = x1;
                wndChild.Top = y1;
                wndChild.Width = x2 - x1;
                wndChild.Height = y2 - y1;

                wndChild.SetTextColor(objArgs.lColorText);
                wndChild.SetBackColor(objArgs.lColorBack);*/

                //wndChild.Show();

                //wndChild.Font = MakeFont();

                arrayClassList.Add(this);

                wndChild.procSelChange = new WndDatabase.DelegateSelChange(this.OnEventSelChange);

                //base.SetToolTipOnChildWindow(wndChild.m_list);
                //OnVisible(ExpandCalcVisible());
            }

            /*
            wndChild.AutoGenerateColumns = false;
            //wndChild.ItemsSource = GetItem();// "H e l l o l".Split();// GetItem();
            
            DataGridTextColumn  col = new DataGridTextColumn();
            col.Header = "hello";
            indexingConverter convert = new indexingConverter();          
            col.Binding = new Binding { Converter = convert, ConverterParameter = "Hello" };
            //col.Binding = new System.Windows.Data.Binding("Hello");
            wndChild.Columns.Add(col);

            col = new DataGridTextColumn();
            col.Header = "i";
            //indexingConverter convert = new indexingConverter();
            col.Binding = new Binding { Converter = convert, ConverterParameter = "By" };
            //col.Binding = new System.Windows.Data.Binding("By");
            wndChild.Columns.Add(col);

            List<object> l = new List<object>();
            
            for (int i = 0; i < 100; i++)
            {
                Dictionary<string, string> s = new Dictionary<string, string>();
                s.Add("Hello", "Good");
                s.Add("By", i.ToString());
                l.Add(s);
            }

            listob = l;// GetItem();

            wndChild.ItemsSource = listob;

            TextColor = objArgs.lColorText;
            BackColor = objArgs.lColorBack;*/

            SetShapeOriginal(wndChild);
            MoveShape();
        }

        //List<object> listob;

        public override void EventTimerObject(UserControl form)
        {
            //listob[0].i = listob[0].i+1;

            //wndChild.ItemsSource = null;
            //wndChild.ItemsSource = listob;
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

        public override void Close()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                //wndChild.SaveClassConfig();
                arrayClassList.Remove(this);
            }
        }

        /*
        public override void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT)
            {
                if (x1 > x2) Tools.Temp(ref x1, ref x2);
                if (y1 > y2) Tools.Temp(ref y1, ref y2);

                Font font = MakeFont();

                int cyChar = (int)font.GetHeight() + 1;
                int cxChar = (int)(font.GetHeight() / 2);

                RECT r = new RECT();

                DrawClass.PopBox2(g, x1, y1, x2, y2, RunColorBack);

                string buf;
                int i, y;

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Near;
                format.FormatFlags |= StringFormatFlags.NoWrap;
                Brush brush = new SolidBrush(RunColorText);

                for (i = 0, y = y1; y < y2; y += cyChar, i++)
                {
                    r.left = x1;
                    r.top = y;
                    r.right = x2;
                    r.bottom = y2;

                    if (objArgs.bUseNo == 1)
                    {
                        buf = String.Format("{0:000}", i + 1);
                        DrawClass.DrawText(g, buf, font, brush, r, format);
                        r.left += cxChar * 4;
                    }

                    if (r.left >= r.right) continue;

                    buf = String.Format("2003-04-22");
                    DrawClass.DrawText(g, buf, font, brush, r, format);
                    r.left += cxChar * 11;

                    if (r.left >= r.right) continue;

                    buf = String.Format("15:50:00");
                    DrawClass.DrawText(g, buf, font, brush, r, format);
                    r.left += cxChar * 9;

                    if (r.left >= r.right) continue;

                    buf = String.Format("Column1_Row{0:000}", i);
                    DrawClass.DrawText(g, buf, font, brush, r, format);
                    r.left += cxChar * 17;

                    if (r.left >= r.right) continue;

                    buf = String.Format("Column2_Row{0:000}", i);
                    DrawClass.DrawText(g, buf, font, brush, r, format);
                }
            }
        }

        public override void OnMove(int x1, int y1, int x2, int y2)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (wndChild != null)
                {
                    Font font = MakeFont();
                    wndChild.Font = font;

                    wndChild.Left = x1;
                    wndChild.Top = y1;
                    wndChild.Width = x2 - x1;
                    wndChild.Height = y2 - y1;
                }
            }
        }
        */
        public override object ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            if (command == "DatabaseSetFilter")
            {
                wndChild.SetSqlText((string)args[0], (string)args[1]);
                wndChild.ReLoad();
                return 1;
            }
            else if (command == "DatabaseReLoad")
            {
                wndChild.ReLoad();
                return 1;
            }
            else if (command == "DatabaseGetCurSel")
            {
                return wndChild.GetCurSel();
            }
            else if (command == "DatabaseSetCurSel")
            {
                wndChild.SetCurSel((int)args[0]);
                return 1;
            }

            else if (command == "DatabaseSetConnection")
            {
                wndChild.SetConnection((string)args[0], (string)args[1]);
                return 1;
            }
            else if (command == "DatabaseSetTable")
            {
                wndChild.SetTable((string)args[0]);
                return 1;
            }

            return 0;
        }

        public override string ExecuteClassNameStringReturn(string command, params object[] args)
        {
            if (command == "DatabaseGetValue")
            {
                return wndChild.GetValue((int)args[0], (string)args[1]);
            }

            return "";
        }

        /*
        public override void ObjectSave(CommaTextWriter writer)
        {
            ObjectSaveFont(writer);

            SaveObjectItem.FileName(writer, objArgs.filename);
            SaveObjectItem.BackColor(writer, GetBackColor());
            SaveObjectItem.TextColor(writer, GetTextColor());

            writer.Write("\tStringOption,");
            writer.Write("{0},", objArgs.table);
            writer.Write("{0},", objArgs.bUseNo);
            writer.Write("{0},", objArgs.nConnectionType);
            writer.Write("{0},", objArgs.bAutoUpdate);
            writer.Write("{0},", objArgs.bUseGrid);
            writer.Write("{0},", objArgs.bUseFullCursor);
            writer.Write("{0},", objArgs.nUpdateTime);
            writer.Write("{0},", objArgs.dsn);
            writer.WriteLine();
        }

        protected override void TextColorChanged()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) return;

            wndChild.SetTextColor(this.RunColorText);
        }

        protected override void BackColorChanged()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) return;

            wndChild.SetBackColor(this.RunColorBack);
        }

        public override void OnVisible(bool flag)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                this.wndChild.Visible = flag;
            }

        }
        */

        protected override bool IsNeedMouseHitTest()
        {
            return true;
        }

    }
}
