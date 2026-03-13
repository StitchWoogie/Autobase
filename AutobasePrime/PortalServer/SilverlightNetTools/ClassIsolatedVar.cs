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
using System.IO;
using NetTools;

namespace SilverlightNetTools
{
    public class ClassIsolatedVar
    {
        System.IO.IsolatedStorage.IsolatedStorageFile store;
        string sFilename;

        public ClassIsolatedVar(string name)
        {
            try
            {
                store = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForSite();
            }
            catch
            {
                store = null;
            }

            /*
            if(store.FileExists("SilverlightAutobaseLibrary.cache")) {
                
            }*/

            //store.OpenFile(
            //using (var store = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication())   6:     {   7:         if (!store.FileExists("DynamicXapDataGrid_CS.xap"))   8:         {   9:             WebClient c = new WebClient();  10:             c.OpenReadCompleted += new OpenReadCompletedEventHandler(c_OpenReadCompleted);  11:             c.OpenReadAsync(new Uri("DynamicXapDataGrid_CS.xap", UriKind.Relative));  12:         }  13:         else  14:         {  15:             LoadData();  16:         }  17:     }

            sFilename = name;
            Load(name);
        }

        class ITEM_STRUCT
        {
            public string name;
            public string val;
        }

        class SECTION_STRUCT
        {
            public string name;
            public List<ITEM_STRUCT> arrayItem = new List<ITEM_STRUCT>();
        }

        List<SECTION_STRUCT> arraySection = new List<SECTION_STRUCT>();

        void Load(string filename)
        {
            if (store == null) return; // 저장 공간을 사용할 수 없는 경우도 있다.

            arraySection = new List<SECTION_STRUCT>();	// 새로 초기화 한다.

            if (!store.FileExists(filename))
            {
                return;
            }

            Stream stream = store.OpenFile(filename, FileMode.Open);
            TextReader reader = new StreamReader(stream);

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
                        arraySection.Add(sects);
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
                arraySection.Add(sects);
            }

            return;
        }

        public int GetInt(string section, string item, int default_value)
        {
            string s = "";

            s = GetString(section, item, default_value.ToString());

            return ConvertTool.ToInt32(s);
        }

        public bool GetBool(string section, string item, bool default_value)
        {
            string s = "";

            s = GetString(section, item, default_value.ToString());

            return ConvertTool.ToBoolean(s);
        }

        public string GetString(string section, string item, string default_value)
        {
            SECTION_STRUCT sects;
            ITEM_STRUCT items;

            for (int s = 0; s < arraySection.Count; s++)
            {
                sects = (SECTION_STRUCT)arraySection[s];

                if (String.Compare(section, sects.name, StringComparison.CurrentCulture) != 0) continue;

                for (int i = 0; i < sects.arrayItem.Count; i++)
                {
                    items = (ITEM_STRUCT)sects.arrayItem[i];

                    if (String.Compare(item, items.name, StringComparison.CurrentCulture) == 0)
                    {
                        return items.val;
                    }
                }
            }

            return default_value;
        }

        /*
        public static int GetPrivateProfileInt(string section, string item, int default_value, string filename)
        {
            string s = "";
            int retn;

            GetPrivateProfileString(section, item, "not found item", ref s, filename);
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

        public static bool GetPrivateProfileBool(string section, string item, bool default_value, string filename)
        {
            string s = "";
            bool retn;

            GetPrivateProfileString(section, item, "not found item", ref s, filename);
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
                            reader.Close();
                            return;
                        }
                    }
                }
            }

            // reader.Close();

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

            TextReader reader = new StreamReader(filename, encoding);
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
                            reader.Close();
                            return;
                        }
                    }
                }
            }

            reader.Close();

            buf = default_value;
            return;
        }

        public static void GetPrivateProfileString(string section, string item, string default_value, ref string buf, string filename)
        {
            TextReader reader = new StreamReader(filename);

            LocalGetPrivateProfileString(reader, section, item, default_value, ref buf);

            reader.Close();
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        /// <param name="item"></param>
        /// <param name="buf">null 이면 항목이 삭제된다.</param>
        /// <param name="filename"></param>
        /// <param name="encoding"></param>
        void LocalWritePrivateProfileString(string section, string item, string buf, string filename)
        {
            string path = System.IO.Path.GetDirectoryName(filename);

            if (!Directory.Exists(path))	// 폴더를 만듬
            {
                Directory.CreateDirectory(path);
            }

            SectionClass one_section;
            ArrayList arraySection = new ArrayList();
            CommaBlockString comma = new CommaBlockString();

            comma.SetBlockCode(']');

            if (File.Exists(filename))
            {
                TextReader reader = new StreamReader(filename);

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

        public static void WritePrivateProfileString(string section, string item, string buf, string filename)
        {
            LocalWritePrivateProfileString(section, item, buf, filename);
        }*/

        public void SetString(string section, string item, string buf)
        {
            SECTION_STRUCT sects;
            ITEM_STRUCT items;

            for (int s = 0; s < arraySection.Count; s++)
            {
                sects = (SECTION_STRUCT)arraySection[s];

                if (String.Compare(section, sects.name, StringComparison.CurrentCulture) != 0) continue;

                for (int i = 0; i < sects.arrayItem.Count; i++)
                {
                    items = (ITEM_STRUCT)sects.arrayItem[i];

                    if (String.Compare(item, items.name, StringComparison.CurrentCulture) == 0)
                    {
                        items.val = buf;
                        return;
                    }
                }

                items = new ITEM_STRUCT();
                items.name = item;
                items.val = buf;
                sects.arrayItem.Add(items);
                return;
            }

            sects = new SECTION_STRUCT();
            sects.name = section;
            arraySection.Add(sects);
            
            items = new ITEM_STRUCT();
            items.name = item;
            items.val = buf;
            sects.arrayItem.Add(items);
            return;
        }

        public void SetBool(string section, string item, bool val)
        {
            SetString(section, item, val.ToString());
        }

        public void Save()
        {
            if (store == null) return; // 저장 공간을 사용할 수 없는 경우도 있다.

            Stream stream = store.OpenFile(sFilename, FileMode.Create);

            TextWriter writer = new StreamWriter(stream);

            SECTION_STRUCT sects;
            ITEM_STRUCT items;

            for (int s = 0; s < arraySection.Count; s++)
            {
                sects = (SECTION_STRUCT)arraySection[s];

                writer.WriteLine("[{0}]", sects.name);

                for (int i = 0; i < sects.arrayItem.Count; i++)
                {
                    items = (ITEM_STRUCT)sects.arrayItem[i];

                    writer.WriteLine("{0}={1}", items.name, items.val);
                }
            }

            writer.Close();
        }
    }
}
