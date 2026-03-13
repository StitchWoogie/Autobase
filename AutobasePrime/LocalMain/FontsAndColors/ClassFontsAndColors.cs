using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using AutoLibLocal;
using NetTools;

namespace LocalMain
{
    [Serializable]
    public class ClassFontsAndColors
    {
        public List<FontsAndColorsGroup> arrayGroup = new List<FontsAndColorsGroup>();

        void AddGroup(FontsAndColorsGroup group)
        {
            arrayGroup.Add(group);
        }

        public void Add(FontsAndColorsItemPublic item, string group_name, string item_name)
        {
            item.LoadConfig(group_name, item_name);

            for (int i = 0; i < arrayGroup.Count; i++)
            {
                if (arrayGroup[i].group_name == group_name)
                {
                    arrayGroup[i].AddItem(item, item_name);
                    
                    return;
                }
            }

            FontsAndColorsGroup group = new FontsAndColorsGroup();
            group.group_name = group_name;
            group.AddItem(item, item_name);

            arrayGroup.Add(group);
        }

        public FontsAndColorsGroup GetGroup(string group_name)
        {
            for (int i = 0; i < arrayGroup.Count; i++)
            {
                if (arrayGroup[i].group_name == group_name)
                {
                    return arrayGroup[i];
                }
            }

            return null;
        }

        public void SaveConfig()
        {
            for (int i = 0; i < arrayGroup.Count; i++)
            {
                arrayGroup[i].SaveConfig();
            }
        }
    }

    [Serializable]
    public class FontsAndColorsGroup
    {
        public string group_name;
        public List<FontsAndColorsItemPublic> arrayItem = new List<FontsAndColorsItemPublic>();

        public void AddItem(FontsAndColorsItemPublic item, string item_name)
        {
            item.item_name = item_name;
            arrayItem.Add(item);
        }

        public FontsAndColorsItemPublic GetItem(string item_name)
        {
            for (int i = 0; i < arrayItem.Count; i++)
            {
                if (arrayItem[i].item_name == item_name)
                {
                    return arrayItem[i];
                }
            }

            return null;
        }

        public void SaveConfig()
        {
            for (int i = 0; i < arrayItem.Count; i++)
            {
                arrayItem[i].SaveConfig(group_name, arrayItem[i].item_name);
            }
        }
    }

    [Serializable]
    public class FontsAndColorsItemPublic
    {
        public string item_name;

        public virtual void LoadConfig(string group_name, string item_name)
        {

        }

        public virtual void SaveConfig(string group_name, string item_name)
        {

        }

        public virtual void Copy(FontsAndColorsItemPublic source)
        {

        }
    }

    [Serializable]
    public class FontsAndColorsItemFont : FontsAndColorsItemPublic
    {
        Font font = null;
        public string FontName;
        public float FontSize;
        public bool FontBold;

        // 폰트를 필요할 때 만들어서 쓰는것이 전체적인 속도에 도움이 될 듯 하다.
        public Font GetFont()
        {
            if (font == null)
            {
                FontStyle style = FontStyle.Regular;

                if (FontBold) style |= FontStyle.Bold;

                font = new Font(FontName, FontSize, style);
            }

            return font;
        }
        
        public override void Copy(FontsAndColorsItemPublic source)
        {
            FontsAndColorsItemFont s = (FontsAndColorsItemFont)source;
            FontName = s.FontName;
            FontSize = s.FontSize;
            FontBold = s.FontBold;

            font = null;
        }
                
        public override void LoadConfig(string group_name, string item_name)
        {
            string fonts = TotalConfig.LoadRegAutoBaseConfig("FontsAndColors", group_name + "\\" + item_name, "Font", "");

            if (fonts.Length == 0)
            {
                Font fontimsi = AutoLibLocal.ConfigViewMain.MakeDefaultFont();

                FontName = fontimsi.Name;
                FontSize = fontimsi.Size;
                FontBold = fontimsi.Bold;
            }
            else
            {
                CommaBlockString comma = new CommaBlockString();
                comma.Set(fonts);
                FontName = comma.GetString();
                FontSize = comma.GetFloat();
                FontBold = comma.GetBool();
            }
        }

        public override void SaveConfig(string group_name, string item_name)
        {
            string fonts = String.Format("{0},{1},{2}", FontName, FontSize, FontBold);

            TotalConfig.SaveRegAutoBaseConfig("FontsAndColors", group_name + "\\" + item_name, "Font", fonts);
        }
    }
}
