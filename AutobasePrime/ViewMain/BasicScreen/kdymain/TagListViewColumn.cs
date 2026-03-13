using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;
using AutoLibLocal;

namespace BasicScreen.kdymain
{
    public class TagListViewColumn
    {
        public enum EnumTagListColumn
        {
            No = 0,
            Tag = 1,
            Description = 2,
            Value = 3,
            Unit = 4,
            Data = 5,
            Alarm = 6,
            AO_SV = 7,
            HandInput = 8,
            Port = 9,
            Address = 10,
            Out1 = 11,      // DI
            Out2 = 12,      // DI
            SV_Out1 = 13,   // DI AI가 함께 사용
        }

        public class TagListColumnItem
        {
            public EnumTagListColumn eTagListColumn;
            public string sColumnName;
            public int nWidth;
            public bool visible;
        }

        static List<TagListColumnItem> arrayPreDefinedList = new List<TagListColumnItem>();

        static TagListViewColumn()
        {
            MakeTotalColumnList();
        }

        static void AddOneItem(EnumTagListColumn e, string name, int width)
        {
            TagListColumnItem tlci = new TagListColumnItem();
            tlci.eTagListColumn = e;
            tlci.sColumnName = name;
            tlci.nWidth = width;

            arrayPreDefinedList.Add(tlci);
        }

        static void MakeTotalColumnList()
        {
            if (Tools.IsLangKorean())
            {
                AddOneItem(EnumTagListColumn.No, "순서", 80);
                AddOneItem(EnumTagListColumn.Tag, "태그이름", 100);
                AddOneItem(EnumTagListColumn.Description, "설명", 120);
                AddOneItem(EnumTagListColumn.Value, "현재 값", 130);
                AddOneItem(EnumTagListColumn.Unit, "단위", 50);
                AddOneItem(EnumTagListColumn.Data, "자료", 50);
                AddOneItem(EnumTagListColumn.Alarm, "경보", 50);
                AddOneItem(EnumTagListColumn.AO_SV, "AO SV", 80);
                AddOneItem(EnumTagListColumn.HandInput, "수동기입", 80);
                AddOneItem(EnumTagListColumn.Port, "포트", 50);
                AddOneItem(EnumTagListColumn.Address, "주소", 50);
                AddOneItem(EnumTagListColumn.Out1, "Out1", 80);
                AddOneItem(EnumTagListColumn.Out2, "Out2", 80);
                AddOneItem(EnumTagListColumn.SV_Out1, "SV/Out1", 80);
            }
            else if (Tools.IsLangJapanese())
            {
                AddOneItem(EnumTagListColumn.No, "No", 80);
                AddOneItem(EnumTagListColumn.Tag, "タグ名", 100);
                AddOneItem(EnumTagListColumn.Description, "説明", 120);
                AddOneItem(EnumTagListColumn.Value, "現在値", 130);
                AddOneItem(EnumTagListColumn.Unit, "単位", 50);
                AddOneItem(EnumTagListColumn.Data, "データ", 50);
                AddOneItem(EnumTagListColumn.Alarm, "警報", 50);
                AddOneItem(EnumTagListColumn.AO_SV, "AO SV", 80);
                AddOneItem(EnumTagListColumn.HandInput, "手動記入", 80);
                AddOneItem(EnumTagListColumn.Port, "Port", 50);
                AddOneItem(EnumTagListColumn.Address, "Address", 50);
                AddOneItem(EnumTagListColumn.Out1, "Out1", 80);
                AddOneItem(EnumTagListColumn.Out2, "Out2", 80);
                AddOneItem(EnumTagListColumn.SV_Out1, "SV/Out1", 80);
            }
            else if (Tools.IsLangChinese())
            {
                AddOneItem(EnumTagListColumn.No, "顺序", 80);
                AddOneItem(EnumTagListColumn.Tag, "标记名", 100);
                AddOneItem(EnumTagListColumn.Description, "标记描述", 120);
                AddOneItem(EnumTagListColumn.Value, "现在值", 130);
                AddOneItem(EnumTagListColumn.Unit, "单位", 50);
                AddOneItem(EnumTagListColumn.Data, "资料", 50);
                AddOneItem(EnumTagListColumn.Alarm, "警报", 50);
                AddOneItem(EnumTagListColumn.AO_SV, "AO SV", 80);
                AddOneItem(EnumTagListColumn.HandInput, "手动输入", 80);
                AddOneItem(EnumTagListColumn.Port, "Port", 50);
                AddOneItem(EnumTagListColumn.Address, "Address", 50);
                AddOneItem(EnumTagListColumn.Out1, "Out1", 80);
                AddOneItem(EnumTagListColumn.Out2, "Out2", 80);
                AddOneItem(EnumTagListColumn.SV_Out1, "SV/Out1", 80);
            }
            else if (Tools.IsLangVietnamese())
            {
                AddOneItem(EnumTagListColumn.No, "Số", 80);
                AddOneItem(EnumTagListColumn.Tag, "Tên Tag", 100);
                AddOneItem(EnumTagListColumn.Description, "Mô tả", 120);
                AddOneItem(EnumTagListColumn.Value, "Giá trị hiện tại", 130);
                AddOneItem(EnumTagListColumn.Unit, "Đơn vị", 50);
                AddOneItem(EnumTagListColumn.Data, "Data", 50);
                AddOneItem(EnumTagListColumn.Alarm, "Alarm", 50);
                AddOneItem(EnumTagListColumn.AO_SV, "AO SV", 80);
                AddOneItem(EnumTagListColumn.HandInput, "Hand Input", 80);
                AddOneItem(EnumTagListColumn.Port, "Port", 50);
                AddOneItem(EnumTagListColumn.Address, "Address", 50);
                AddOneItem(EnumTagListColumn.Out1, "Out1", 80);
                AddOneItem(EnumTagListColumn.Out2, "Out2", 80);
                AddOneItem(EnumTagListColumn.SV_Out1, "SV/Out1", 80);
            }
            else
            {
                AddOneItem(EnumTagListColumn.No, "No", 80);
                AddOneItem(EnumTagListColumn.Tag, "Tag Name", 100);
                AddOneItem(EnumTagListColumn.Description, "Description", 120);
                AddOneItem(EnumTagListColumn.Value, "Current Value", 130);
                AddOneItem(EnumTagListColumn.Unit, "Unit", 50);
                AddOneItem(EnumTagListColumn.Data, "Data", 50);
                AddOneItem(EnumTagListColumn.Alarm, "Alarm", 50);
                AddOneItem(EnumTagListColumn.AO_SV, "AO SV", 80);
                AddOneItem(EnumTagListColumn.HandInput, "Hand Input", 80);
                AddOneItem(EnumTagListColumn.Port, "Port", 50);
                AddOneItem(EnumTagListColumn.Address, "Address", 50);
                AddOneItem(EnumTagListColumn.Out1, "Out1", 80);
                AddOneItem(EnumTagListColumn.Out2, "Out2", 80);
                AddOneItem(EnumTagListColumn.SV_Out1, "SV/Out1", 80);
            }
        }

        public List<TagListColumnItem> arrayColumnList = new List<TagListColumnItem>();
        
        public void AddColumn(EnumTagListColumn e, bool visible)
        {
            for (int i = 0; i < arrayPreDefinedList.Count; i++)
            {
                if (arrayPreDefinedList[i].eTagListColumn == e)
                {
                    TagListColumnItem tlci = new TagListColumnItem();
                    tlci.eTagListColumn = e;
                    tlci.sColumnName = arrayPreDefinedList[i].sColumnName;
                    tlci.nWidth = arrayPreDefinedList[i].nWidth;
                    tlci.visible = visible;

                    arrayColumnList.Add(tlci);
                }
            }
        }

        /*
        TagListColumnItem SeekColumnByEnumString(string e)
        {
            for (int i = 0; i < arrayColumnList.Count; i++)
            {
                if (e == arrayColumnList[i].eTagListColumn.ToString())
                {
                    return arrayColumnList[i];
                }
            }

            return null;
        }

        public void AutoBaseListCtrlConfigLoad(string filename, string section)
        {
            string cfg_dir;
            string filepath;

            CommaBlockString comma = new CommaBlockString();

            cfg_dir = TotalConfig.AutoBaseIniGetConfigDirectory();
            filepath = String.Format("{0}\\UserListCtrlConfig\\{1}_{2}", cfg_dir, filename, section);

            if (!File.Exists(filepath)) return;

            string buf;

            TextReader reader = new StreamReader(filepath);
            if (reader == null) return;

            TagListColumnItem tlci;

            string e;
            
            while (true)
            {
                buf = reader.ReadLine();
                if (buf == null) break;

                comma.Set(buf);
                e = comma.GetString();

                tlci = SeekColumnByEnumString(e);

                if (tlci != null)
                {
                    tlci.nWidth = comma.GetInt();
                    tlci.visible = comma.GetBool();

                    if (tlci.nWidth != 0)
                    {
                        if (tlci.nWidth > 1000) tlci.nWidth = 50;
                        if (tlci.nWidth < 10) tlci.nWidth = 10;
                    }
                }
            }
            reader.Close();
        }

        public void AutoBaseListCtrlConfigSave(string filename, string section)
        {
            string cfg_dir;
            string filepath;
            int i;
            CommaBlockString comma = new CommaBlockString();

            cfg_dir = TotalConfig.AutoBaseIniGetConfigDirectory();

            filepath = String.Format("{0}\\UserListCtrlConfig", cfg_dir);
            Directory.CreateDirectory(filepath);
            filepath = String.Format("{0}\\UserListCtrlConfig\\{1}_{2}", cfg_dir, filename, section);

            TextWriter writer = new StreamWriter(filepath);
            if (writer == null) return;

            TagListColumnItem tlci;

            for (i = 0; i < arrayColumnList.Count; i++)
            {
                tlci = arrayColumnList[i];
                writer.WriteLine("{0},{1},{2}", tlci.eTagListColumn.ToString(), tlci.nWidth, tlci.visible);
            }

            writer.Close();

        }*/
    }
}
