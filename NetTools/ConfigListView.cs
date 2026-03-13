using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace NetTools
{
    public class ConfigListView
    {
        public static void Load(ListView m_list, string filename)
        {
            string cfg_dir;
            string filepath;
            string title = "";
            int val = 0;
            int i;
            ColumnHeader col;
            CommaBlockString comma = new CommaBlockString();

            cfg_dir = ConfigDataGridView.GetConfigPath();
            filepath = String.Format("{0}\\ListView\\{1}.cfg", cfg_dir, filename);

            if (!File.Exists(filepath)) return;

            string buf;

            TextReader reader = new StreamReader(filepath);
            if (reader == null) return;
            while (true)
            {
                buf = reader.ReadLine();
                if (buf == null) break;
                comma.Set(buf);
                comma.GetString(ref title);

                for (i = 0; i < m_list.Columns.Count; i++)
                {
                    col = m_list.Columns[i];
                    if (title == col.Text)
                    {
                        comma.GetInt(ref val);
                        if (val != 0)
                        {
                            if (val > 1000) val = 50;
                            if (val < 10) val = 10;
                            col.Width = val;
                        }
                        break;
                    }
                }
            }
            reader.Close();
        }

        public static void Save(ListView m_list, string filename)
        {
            string cfg_dir;
            string filepath;
            int i;
            ColumnHeader col;
            CommaBlockString comma = new CommaBlockString();

            cfg_dir = ConfigDataGridView.GetConfigPath();

            filepath = String.Format("{0}\\ListView", cfg_dir);
            Directory.CreateDirectory(filepath);
            filepath = String.Format("{0}\\ListView\\{1}.cfg", cfg_dir, filename);

            TextWriter writer = new StreamWriter(filepath);
            if (writer == null) return;

            for (i = 0; i < m_list.Columns.Count; i++)
            {
                col = m_list.Columns[i];
                writer.WriteLine("{0},{1},", col.Text, col.Width);
            }

            writer.Close();

        }
    }
}
