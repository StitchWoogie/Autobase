using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace NetTools
{
    /// <summary>
    /// Summary description for Profile.
    /// </summary>
    /// 

    public class Profile
    {
        public Profile()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        class ITEM_STRUCT
        {
            public string name;
            public string val;
        }

        class SECTION_STRUCT
        {
            public string name;
            public ArrayList arrayItem = new ArrayList();
        }

        ArrayList arraySect = new ArrayList();

        public void LoadByEncodingUtf8(string filename)
        {
            LoadPublic(filename, System.Text.Encoding.UTF8);
        }

        public void LoadByEncodingDefault(string filename)
        {
            LoadPublic(filename, System.Text.Encoding.Default);
        }

        // section이 두개 이상이면 잘 안될 듯 하다. 
        // 이 함수는 modx의 objscr ini 파일을 읽어올때 사용하고 있어서 수정하지 않고 일단 두기로 했다.
        // 2012-5-25 발견
        public void LoadPublic(string filename, System.Text.Encoding encoding)
        {
            arraySect = new ArrayList();	// 새로 초기화 한다.

            if (!File.Exists(filename))
            {
                return;
            }

            TextReader reader = new StreamReader(filename, encoding);

            if (reader == null)
            {
                return;
            }

            bool ok_section = false;
            string text = "";
            CommaBlockString comma = new CommaBlockString();
            //string command = "";
            SECTION_STRUCT sects = new SECTION_STRUCT();
            ITEM_STRUCT items;

            while (true)
            {
                text = reader.ReadLine();
                if (text == null) break;

                if (text.Length == 0) continue;

                if (!ok_section)
                {
                    if (text.Length == 0) continue;
                    if (text[0] != '[') continue;
                }

                if (text[0] == '[')
                {
                    if (sects.arrayItem.Count > 0)
                    {
                        arraySect.Add(sects);
                    }
                    sects = new SECTION_STRUCT();
                    comma.Set(text);
                    comma.SetBlockCode('[');
                    comma.Skip();
                    comma.SetBlockCode(']');
                    comma.GetString(ref sects.name);

                    ok_section = true;
                }
                else
                {
                    items = new ITEM_STRUCT();
                    comma.Set(text);
                    comma.SetBlockCode('=');
                    comma.GetString(ref items.name);
                    comma.GetStringTotalRemain(ref items.val);

                    sects.arrayItem.Add(items);
                }
            }

            reader.Close();

            if (sects.arrayItem.Count > 0)
            {
                arraySect.Add(sects);
            }

            return;
        }

        public int GetIntFromReadyMemory(string section, string item, int default_value)
        {
            string s = "";
            int retn;

            GetStringFromReadyMemory(section, item, "not found item", ref s);
            if (s == "not found item")
            {
                retn = default_value;
            }
            else
            {
                try
                {
                    retn = int.Parse(s);
                }
                catch
                {
                    retn = 0;
                }
            }

            return retn;
        }

        public void GetStringFromReadyMemory(string section, string item, string default_value, ref string buf)
        {
            SECTION_STRUCT sects;
            ITEM_STRUCT items;

            for (int s = 0; s < arraySect.Count; s++)
            {
                sects = (SECTION_STRUCT)arraySect[s];

                if (String.Compare(section, sects.name, true) != 0) continue;

                for (int i = 0; i < sects.arrayItem.Count; i++)
                {
                    items = (ITEM_STRUCT)sects.arrayItem[i];

                    if (String.Compare(item, items.name, true) == 0)
                    {
                        buf = items.val;
                        return;
                    }
                }
            }

            buf = default_value;
        }

        public static int GetPrivateProfileIntA(string section, string item, int default_value, string filename)
        {
            string s = "";
            int retn;

            GetPrivateProfileStringA(section, item, "not found item", ref s, filename);
            if (s == "not found item")
            {
                retn = default_value;
            }
            else
            {
                try
                {
                    retn = int.Parse(s);
                }
                catch
                {
                    retn = 0;
                }
            }

            return retn;
        }

        public static int GetPrivateProfileIntW(string section, string item, int default_value, string filename)
        {
            string s = "";
            int retn;

            GetPrivateProfileStringW(section, item, "not found item", ref s, filename);
            if (s == "not found item")
            {
                retn = default_value;
            }
            else
            {
                try
                {
                    retn = int.Parse(s);
                }
                catch
                {
                    retn = 0;
                }
            }

            return retn;
        }

        public static bool GetPrivateProfileBoolW(string section, string item, bool default_value, string filename)
        {
            string s = "";
            bool retn;

            GetPrivateProfileStringW(section, item, "not found item", ref s, filename);
            if (s == "not found item")
            {
                retn = default_value;
            }
            else
            {
                try
                {
                    retn = Convert.ToBoolean(s);
                }
                catch
                {
                    retn = false;
                }
            }

            return retn;
        }

        static void LocalGetPrivateProfileString(TextReader reader, string section, string item, string default_value, ref string buf)
        {
            if (reader == null)
            {
                buf = default_value;
                return;
            }

            bool ok_section = false;
            string text = "";
            CommaBlockString comma = new CommaBlockString();
            string command = "";

            while (true)
            {
                text = reader.ReadLine();
                if (text == null) break;
                if (text.Length == 0) continue;

                if (!ok_section)
                {
                    if (text.Length == 0) continue;
                    if (text[0] != '[') continue;
                    comma.Set(text);
                    comma.SetBlockCode('[');
                    comma.Skip();
                    comma.SetBlockCode(']');
                    comma.GetString(ref command);
                    if (section == command)
                    {
                        ok_section = true;
                    }
                }
                else
                {
                    if (text[0] == '[')
                    {
                        ok_section = false;
                    }
                    else
                    {
                        comma.Set(text);
                        comma.SetBlockCode('=');
                        comma.GetString(ref command);
                        if (command == item)
                        {
                            comma.GetStringTotalRemain(ref buf);
                            return;
                        }
                    }
                }
            }

            buf = default_value;
            return;
        }

        static void LocalGetPrivateProfileString(string section, string item, string default_value, ref string buf, string filename, System.Text.Encoding encoding)
        {
            if (!File.Exists(filename))
            {
                buf = default_value;
                return;
            }

            TextReader reader;

            try
            {
                reader = new StreamReader(filename, encoding);
            }
            catch
            {
                return;
            }

            if (reader == null)
            {
                buf = default_value;
                return;
            }

            LocalGetPrivateProfileString(reader, section, item, default_value, ref buf);

            reader.Close();
        }

        public static void GetPrivateProfileStringA(string section, string item, string default_value, ref string buf, string filename)
        {
            LocalGetPrivateProfileString(section, item, default_value, ref buf, filename, System.Text.Encoding.Default);
        }

        public static void GetPrivateProfileStringW(string section, string item, string default_value, ref string buf, string filename)
        {
            LocalGetPrivateProfileString(section, item, default_value, ref buf, filename, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Byte[] 데이터에서 읽어온다.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="item"></param>
        /// <param name="default_value"></param>
        /// <param name="buf"></param>
        /// <param name="filedata"></param>
        public static void GetPrivateProfileString(string section, string item, string default_value, ref string buf, byte[] filedata)
        {
            if (filedata == null)
            {
                buf = default_value;
                return;
            }

            MemoryStream ms = new MemoryStream(filedata);

            TextReader reader;

            reader = new StreamReader(ms);

            LocalGetPrivateProfileString(reader, section, item, default_value, ref buf);

            reader.Close();
            ms.Close();
        }

        class SectionClass
        {
            public string section;
            public ArrayList item = new ArrayList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        /// <param name="item"></param>
        /// <param name="buf">null 이면 항목이 삭제된다.</param>
        /// <param name="filename"></param>
        /// <param name="encoding"></param>
        static void LocalWritePrivateProfileString(string section, string item, string buf, string filename, System.Text.Encoding encoding)
        {
            string path = Path.GetDirectoryName(filename);

            if (!Directory.Exists(path))	// 폴더를 만듬
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch
                {
                    return;
                }
            }

            SectionClass one_section;
            ArrayList arraySection = new ArrayList();
            CommaBlockString comma = new CommaBlockString();

            comma.SetBlockCode(']');

            if (File.Exists(filename))
            {
                TextReader reader = new StreamReader(filename, encoding);

                string one_line = "";
                one_section = new SectionClass();

                while (true)
                {
                    one_line = reader.ReadLine();
                    if (one_line == null) break;
                    if (one_line.Length == 0) continue;

                    if (one_line[0] == '[')
                    {
                        if (one_section.item.Count > 0)
                        {
                            arraySection.Add(one_section);
                        }

                        one_section = new SectionClass();
                        comma.Set(one_line.Substring(1));
                        comma.GetString(ref one_section.section);
                        continue;
                    }

                    one_section.item.Add(one_line);
                }

                if (one_section.item.Count > 0)
                {
                    arraySection.Add(one_section);
                }

                reader.Close();
            }

            comma.SetBlockCode('=');

            bool done = false;

            for (int i = 0; i < arraySection.Count; i++)
            {
                one_section = (SectionClass)arraySection[i];

                if (one_section.section == section)
                {
                    string imsi = "";
                    for (int j = 0; j < one_section.item.Count; j++)
                    {
                        comma.Set((string)one_section.item[j]);
                        comma.GetString(ref imsi);
                        if (imsi == item)
                        {
                            done = true;

                            if (buf == null)
                                one_section.item.RemoveAt(j);
                            else
                                one_section.item[j] = (item + "=" + buf);

                            goto ok;
                        }
                    }

                    one_section.item.Add(item + "=" + buf);
                    done = true;
                }
            }
        ok: ;
            if (!done && buf != null)
            {
                one_section = new SectionClass();
                one_section.section = section;
                one_section.item.Add(item + "=" + buf);
                arraySection.Add(one_section);
            }

            TextWriter writer = new StreamWriter(filename, false, encoding);
            for (int i = 0; i < arraySection.Count; i++)
            {
                one_section = (SectionClass)arraySection[i];
                writer.WriteLine("[{0}]", one_section.section);
                for (int j = 0; j < one_section.item.Count; j++)
                {
                    writer.WriteLine(one_section.item[j]);
                }
            }
            writer.Close();

            return;
        }

        public static void WritePrivateProfileStringW(string section, string item, string buf, string filename)
        {
            LocalWritePrivateProfileString(section, item, buf, filename, System.Text.Encoding.UTF8);
        }

        public static void WritePrivateProfileIntW(string section, string item, int buf, string filename)
        {
            LocalWritePrivateProfileString(section, item, buf.ToString(), filename, System.Text.Encoding.UTF8);
        }

        public static void WritePrivateProfileStringA(string section, string item, string buf, string filename)
        {
            LocalWritePrivateProfileString(section, item, buf, filename, System.Text.Encoding.Default);
        }
    }
}
