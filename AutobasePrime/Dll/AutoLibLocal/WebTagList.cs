using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using NetTools;

namespace AutoLibLocal
{
    public class WebTagList
    {
        public string name;
        public string description;
        public List<string> member = new List<string>();
    }

    public class WebGroupList
    {
        public static List<WebTagList> Load()
        {
            string filename = String.Format("{0}\\Web\\WebGroupList.lstx", TotalConfig.sDirWorkProject);

            List<WebTagList> array = new List<WebTagList>();

            if (!System.IO.File.Exists(filename)) return array;

            CommaBlockString comma = new CommaBlockString();
            string one_line;
            WebTagList list = new WebTagList();
            bool flag_begin = false;
            string buf = "";

            TextReader reader = new StreamReader(filename);

            if (reader == null) return array;

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;

                if (one_line.Length == 0) continue;

                comma.Set(one_line);
                comma.GetString(ref buf);
                if (!flag_begin)
                {
                    if (buf == "BEGIN")
                    {
                        flag_begin = true;
                        list = new WebTagList();
                    }
                }
                else
                {
                    if (buf == "END")
                    {
                        array.Add(list);
                        flag_begin = false;
                    }
                    else if (buf == "Name")
                    {
                        comma.GetString(ref list.name);
                    }
                    else if (buf == "Description")
                    {
                        comma.GetString(ref list.description);
                    }
                    else if (buf == "Member")
                    {
                        string tag = "";
                        comma.GetString(ref tag);
                        list.member.Add(tag);
                    }
                }
            }

            reader.Close();

            return array;
        }

        public static void Save(List<WebTagList> array)
        {
            string filename = String.Format("{0}\\Web", TotalConfig.sDirWorkProject);

            if (!Directory.Exists(filename)) Directory.CreateDirectory(filename);

            filename = String.Format("{0}\\Web\\WebGroupList.lstx", TotalConfig.sDirWorkProject);

            WebTagList list;

            TextWriter writer = new StreamWriter(filename);

            if (writer == null) return;

            for (int i = 0; i < array.Count; i++)
            {
                list = (WebTagList)array[i];

                writer.WriteLine("BEGIN");
                writer.WriteLine("Name,{0}", list.name);
                writer.WriteLine("Description,{0}", list.description);
                for (int j = 0; j < list.member.Count; j++)
                {
                    string tag = (string)list.member[j];
                    writer.WriteLine("Member,{0}", tag);
                }

                writer.WriteLine("END");
            }

            writer.Close();
        }
    }
}
