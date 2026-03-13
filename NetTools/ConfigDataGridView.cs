using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace NetTools
{
    public class ConfigDataGridView
    {
        public static string GetConfigPath()
        {
            // 경로에서 버전을 삭제한다.
            string dir = Application.UserAppDataPath;

            for (int i = dir.Length - 1; i > 0; i--)
            {
                if (dir[i] == '\\')
                {
                    dir = dir.Substring(0, i);
                    break;
                }
            }

            return dir;
        }

        public static void Save(DataGridView dgv, string filename)
        {
            string cfg_dir;
            string filepath;
            DataGridViewColumn col;

            cfg_dir = GetConfigPath();

            filepath = String.Format("{0}\\DataGridView", cfg_dir);

            if (!Directory.Exists(filepath))
                Directory.CreateDirectory(filepath);

            filepath = String.Format("{0}\\DataGridView\\{1}.cfg", cfg_dir, filename);

            TextWriter writer = new StreamWriter(filepath);

            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                col = dgv.Columns[i];

                writer.WriteLine("{0},{1}", col.Name, col.Width);
            }

            writer.Flush();
            writer.Close();
        }

        public static void Load(DataGridView dgv, string filename)
        {
            string cfg_dir;
            string filepath;
            string title = "";
            int val = 0;
            int i;
            DataGridViewColumn col;
            CommaBlockString comma = new CommaBlockString();

            cfg_dir = GetConfigPath();

            filepath = String.Format("{0}\\DataGridView\\{1}.cfg", cfg_dir, filename);

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

                for (i = 0; i < dgv.Columns.Count; i++)
                {
                    col = dgv.Columns[i];

                    if (title == col.Name)
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
    }
}
