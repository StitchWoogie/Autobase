using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetTools.Hash;

namespace NetTools
{
    public class ProfileHash
    {
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

        List<SECTION_STRUCT> arraySect = new List<SECTION_STRUCT>();
        string sConfigFileName = "";

        public ProfileHash(string filename)
        {
            Load(filename);
        }

        readonly byte[] seed = new byte[] { 0x12, 0x57, 0x99, 0x88, 0x73 };

        void Load(string filename)
        {
            sConfigFileName = filename;

            if (!File.Exists(filename))
            {
                return;
            }

            byte[] buffer = File.ReadAllBytes(filename);

            HashBytes hash = new HashBytes();
            hash.CheckParam(hash.nRootSeed + 1, hash.nRootSeed - 2, hash.nRootSeed * 3, hash.nRootSeed / 4, hash.nRootSeed ^ 5);
            buffer = hash.Decode(buffer, seed);

            MemoryStream ms = new MemoryStream(buffer);

            if (ms == null) return;

            TextReader reader = new StreamReader(ms);

            if (reader == null)
            {
                return;
            }

            string text = "";
            CommaBlockString comma = new CommaBlockString();
            SECTION_STRUCT sects = null;
            ITEM_STRUCT items;
            string section_name;

            while (true)
            {
                text = reader.ReadLine();
                if (text == null) break;

                if (text.Length == 0) continue;

                if (text[0] == '[')
                {
                    comma.Set(text.Substring(1));
                    comma.SetBlockCode(']');
                    section_name = comma.GetString().Trim();

                    if (section_name.Length > 0)
                    {
                        sects = new SECTION_STRUCT();
                        sects.name = section_name;
                        arraySect.Add(sects);
                    }
                    else
                    {
                        // [ 로 시작되었는데 글자가 없다는 것은 문장이 이상하다.  아래의 아이템은 section이 정확하지 않기 때문에 
                        // 모두 무시한다. 취하게 되면 다른 항목이 들어올 수 있다.
                        sects = null;
                    }
                }
                else
                {
                    if (sects != null)
                    {
                        items = new ITEM_STRUCT();
                        comma.Set(text);
                        comma.SetBlockCode('=');
                        comma.GetString(ref items.name);
                        comma.GetStringTotalRemain(ref items.val);

                        items.name = items.name.Trim();
                        if (items.name.Length > 0)
                            sects.arrayItem.Add(items);
                    }
                }
            }

            reader.Close();
            ms.Close();

            return;
        }

        public void Save()
        {
            MemoryStream ms = new MemoryStream();
            TextWriter writer = new StreamWriter(ms);
            SECTION_STRUCT one_section;

            for (int i = 0; i < arraySect.Count; i++)
            {
                one_section = arraySect[i];
                
                // item이 있을때만 저장한다.
                if (one_section.arrayItem.Count <= 0) continue;

                writer.WriteLine("[{0}]", one_section.name);
                for (int j = 0; j < one_section.arrayItem.Count; j++)
                {
                    writer.WriteLine("{0}={1}", one_section.arrayItem[j].name, one_section.arrayItem[j].val);
                }
            }

            writer.Flush(); // 이것을 해야 할 듯 

            byte[] buffer = ms.ToArray();

            writer.Close();

            string dir = Path.GetDirectoryName(sConfigFileName);
            Directory.CreateDirectory(dir);

            string filename_debug = Path.ChangeExtension(sConfigFileName, "debug.txt");
            File.WriteAllBytes(filename_debug, buffer);

            HashBytes hash = new HashBytes();
            hash.CheckParam(hash.nRootSeed + 1, hash.nRootSeed - 2, hash.nRootSeed * 3, hash.nRootSeed / 4, hash.nRootSeed ^ 5);
            buffer = hash.Encode(buffer, seed);

            File.WriteAllBytes(sConfigFileName, buffer);
        }
        
        public string GetString(string section, string item, string default_value)
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
                        return items.val;
                    }
                }
            }

            return default_value;
        }

        public bool GetBool(string section, string item, bool default_value)
        {
            string val = GetString(section, item, default_value.ToString());
            return ConvertTool.ToBoolean(val);
        }

        public int GetInt(string section, string item, int default_value)
        {
            string val = GetString(section, item, default_value.ToString());
            return ConvertTool.ToInt32(val);
        }

        public void SetString(string section, string item, string value)
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
                        items.val = value;
                        return;
                    }
                }

                items = new ITEM_STRUCT();
                items.name = item;
                items.val = value;
                sects.arrayItem.Add(items);
                return;
            }

            sects = new SECTION_STRUCT();
            sects.name = section;

            items = new ITEM_STRUCT();
            items.name = item;
            items.val = value;
            sects.arrayItem.Add(items);

            arraySect.Add(sects);
        }

        public void SetBool(string section, string item, bool value)
        {
            SetString(section, item, value.ToString());
        }

        public void SetInt(string section, string item, int value)
        {
            SetString(section, item, value.ToString());
        }
    }
}
