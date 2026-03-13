using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DialogHoliday;
using System.Collections;
using System.Drawing;
using NetTools;
using AutoLibLocal;
using System.IO;

namespace PublicStudioLocalMain.Schedule
{
    [Serializable]
    public class NAME_STRUCT
    {
        public string title;
    }

    [Serializable]
    public class SCHEDULE_STRUCT
    {
        public string title;
        public Color color;
        public ArrayList blockName;
    }

    [Serializable]
    public class SCHEDULE_ADDITIONAL
    {
        public string title;
        public int type;
        public HOLIDAY_LIST user = new HOLIDAY_LIST();
        public string model;
    }

    [Serializable]
    public class SCHEDULE_TAG_VALUE_STRUCT
    {
        public string tag;
        public string val;
    }

    [Serializable]
    public class SCHEDULE_MODEL_ITEM_STRUCT
    {
        public int nTimeType=0; // 0=specified time, 1=sunrise, 2=sunset
        public int hour;
        public int minute;
        public int nSunBeforeAfterMinutes=0;
        public string sLocation="";    // 지역정보
        public string script;
        public ArrayList blockTag = new ArrayList();
    }

    [Serializable]
    public class SCHEDULE_MODEL_STRUCT
    {
        public string title;
        public string description;
        public ArrayList blockItem = new ArrayList();
    }

    public struct SCHEDULE_WEEK_STRUCT
    {
        public string title;

        public int block_pos;	// 이것은 실제 저장되는 것이 아니고 Week 로드시 고정스케쥴에서 찾아준다.
        public Color color;		// 이것은 실제 저장되는 것이 아니고 Week 로드시 고정스케쥴에서 찾아준다.
    }

    public class ScheduleLib
    {
        public const int MAX_SCHEDULE_WEEK = 9;

        public static ArrayList ScheduleLoadFixed()
        {
            string buf;
            TextReader reader;
            CommaBlockString comma = new CommaBlockString();
            SCHEDULE_STRUCT sc;
            NAME_STRUCT name;
            byte r = 0, g = 0, b = 0;
            ArrayList array = new ArrayList();

            reader = TotalConfig.OpenOldNew("SCHEDULE", "SCHEDULE.LST", "SCHEDULE.LSTX");

            if (reader == null)
            {
                return array;
            }
            while (true)
            {
                buf = reader.ReadLine();
                if (buf == null) break;

                comma.Set(buf);
                sc = new SCHEDULE_STRUCT();

                comma.GetString(ref sc.title);
                comma.GetBYTE(ref r);
                comma.GetBYTE(ref g);
                comma.GetBYTE(ref b);
                sc.color = Color.FromArgb(r, g, b);
                sc.blockName = new ArrayList(); ;

                while (true)
                {
                    name = new NAME_STRUCT();
                    comma.GetString(ref name.title);
                    if (name.title.Length == 0) break;
                    sc.blockName.Add(name);
                }
                array.Add(sc);
            }
            reader.Close();

            return array;
        }

        public static ArrayList ScheduleLoadAdditional()
        {
            string buf;
            TextReader reader;
            CommaBlockString comma = new CommaBlockString();
            SCHEDULE_ADDITIONAL sc;
            ArrayList array = new ArrayList();

            reader = TotalConfig.OpenOldNew("SCHEDULE", "Addition.LST", "Addition.LSTX");

            if (reader == null)
            {
                return array;
            }
            while (true)
            {
                buf = reader.ReadLine();
                if (buf == null) break;

                comma.Set(buf);
                sc = new SCHEDULE_ADDITIONAL();
                comma.GetString(ref sc.title);
                comma.GetInt(ref sc.type);
                comma.GetString(ref sc.user.title);
                comma.GetChar(ref sc.user.type);
                comma.GetInt(ref sc.user.year);
                comma.GetChar(ref sc.user.month);
                comma.GetChar(ref sc.user.day);
                comma.GetChar(ref sc.user.bSunOrMoon);
                comma.GetChar(ref sc.user.week);
                comma.GetChar(ref sc.user.weekday);
                comma.GetString(ref sc.model);

                array.Add(sc);
            }
            reader.Close();

            return array;
        }

        public static void ScheduleSaveAdditional(ArrayList array)
        {
            string filename;
            TextWriter writer;
            SCHEDULE_ADDITIONAL sc;
            int l;

            filename = String.Format("{0}\\SCHEDULE", TotalConfig.sDirWorkProject);
            Directory.CreateDirectory(filename);
            filename = String.Format("{0}\\SCHEDULE\\Addition.LSTX", TotalConfig.sDirWorkProject);
            writer = new StreamWriter(filename);
            if (writer == null) return;

            for (l = 0; l < array.Count; l++)
            {
                sc = (SCHEDULE_ADDITIONAL)array[l];

                writer.Write("{0},", sc.title);
                writer.Write("{0},", sc.type);
                writer.Write("{0},", sc.user.title);
                writer.Write("{0},", sc.user.type);
                writer.Write("{0},", sc.user.year);
                writer.Write("{0},", sc.user.month);
                writer.Write("{0},", sc.user.day);
                writer.Write("{0},", sc.user.bSunOrMoon);
                writer.Write("{0},", sc.user.week);
                writer.Write("{0},", sc.user.weekday);
                writer.Write("{0},", sc.model);
                writer.WriteLine();
            }
            writer.Close();
        }

        public static void ScheduleSaveFixed(ArrayList array)
        {
            string filename;
            TextWriter writer;
            SCHEDULE_STRUCT sc;
            int l, m;
            NAME_STRUCT name;

            filename = String.Format("{0}\\SCHEDULE", TotalConfig.sDirWorkProject);
            Directory.CreateDirectory(filename);
            filename = String.Format("{0}\\SCHEDULE\\SCHEDULE.LSTX", TotalConfig.sDirWorkProject);
            writer = new StreamWriter(filename);
            if (writer == null) return;

            for (l = 0; l < array.Count; l++)
            {
                sc = (SCHEDULE_STRUCT)array[l];
                writer.Write("{0},", sc.title);
                writer.Write("{0},{1},{2},", sc.color.R, sc.color.G, sc.color.B);
                for (m = 0; m < sc.blockName.Count; m++)
                {
                    name = (NAME_STRUCT)sc.blockName[m];
                    writer.Write("{0},", name.title);
                }

                writer.WriteLine();
            }
            writer.Close();
        }

        public static SCHEDULE_WEEK_STRUCT[] ScheduleLoadWeek()
        {
            string buf;
            TextReader reader;
            CommaBlockString comma = new CommaBlockString();
            int no = 0;
            SCHEDULE_WEEK_STRUCT[] scheduleWeek = new SCHEDULE_WEEK_STRUCT[ScheduleLib.MAX_SCHEDULE_WEEK];

            reader = TotalConfig.OpenOldNew("SCHEDULE", "WEEK.LST", "WEEK.LSTX");

            if (reader == null)
            {
                return scheduleWeek;
            }
            while (true)
            {
                buf = reader.ReadLine();
                if (buf == null) break;

                comma.Set(buf);
                comma.GetInt(ref no);
                if (no >= 0 && no < ScheduleLib.MAX_SCHEDULE_WEEK)
                {
                    comma.GetString(ref scheduleWeek[no].title);
                }
            }
            reader.Close();

            return scheduleWeek;
        }

        public static void MakeViewString(SCHEDULE_MODEL_ITEM_STRUCT item, out string buf)
        {
            buf = "";

            string imsi;

            if (item.blockTag != null)
            {
                int l;
                SCHEDULE_TAG_VALUE_STRUCT tag;

                for (l = 0; l < item.blockTag.Count; l++)
                {
                    tag = (SCHEDULE_TAG_VALUE_STRUCT)item.blockTag[l];
                    imsi = String.Format("{0}={1}; ", tag.tag, tag.val);

                    buf += imsi;

                }
            }

            if (item.script != null)
            {
                buf += item.script;
            }
        }

        public static ArrayList ModelLoad()
        {
            string buf;
            TextReader reader;
            CommaBlockString comma = new CommaBlockString();
            ArrayList block = new ArrayList();

            reader = TotalConfig.OpenOldNew("SCHEDULE", "MODEL.LST", "MODEL.LSTX");

            if (reader == null)
            {
                return block;
            }
            while (true)
            {
                buf = reader.ReadLine();
                if (buf == null) break;

                comma.Set(buf);
                comma.GetString(ref buf);
                if (String.Compare(buf, "MODEL", true) == 0)
                {
                    CommaScriptFileModel scr = new CommaScriptFileModel();
                    scr.Execute(reader, buf, comma);
                    block.Add(scr.model);
                }
            }

            reader.Close();

            return block;
        }

        public static void ModelSave(ArrayList block)
        {
            string filename;
            TextWriter writer;
            SCHEDULE_MODEL_STRUCT model;
            SCHEDULE_MODEL_ITEM_STRUCT item;
            SCHEDULE_TAG_VALUE_STRUCT tagValue;
            int l, m, n;

            filename = String.Format("{0}\\SCHEDULE", TotalConfig.sDirWorkProject);
            Directory.CreateDirectory(filename);
            filename = String.Format("{0}\\SCHEDULE\\MODEL.LSTX", TotalConfig.sDirWorkProject);
            writer = new StreamWriter(filename);
            if (writer == null) return;

            for (l = 0; l < block.Count; l++)
            {
                model = (SCHEDULE_MODEL_STRUCT)block[l];
                writer.WriteLine("MODEL,BEGIN,");
                writer.WriteLine("Title,{0},", model.title);
                writer.WriteLine("Description,{0},", model.description);

                for (m = 0; m < model.blockItem.Count; m++)
                {
                    item = (SCHEDULE_MODEL_ITEM_STRUCT)model.blockItem[m];

                    writer.WriteLine("Member,Begin,");
                    writer.WriteLine("TimeType,{0},", item.nTimeType);
                    writer.WriteLine("Time,{0},{1},", item.hour, item.minute);
                    writer.WriteLine("SunControl,{0},{1},", item.nSunBeforeAfterMinutes, item.sLocation);
                    if (item.blockTag != null)
                    {
                        for (n = 0; n < item.blockTag.Count; n++)
                        {
                            tagValue = (SCHEDULE_TAG_VALUE_STRUCT)item.blockTag[n];
                            writer.WriteLine("TagValue,{0},{1},", tagValue.tag, tagValue.val);
                        }
                    }
                    if (item.script != null)
                    {
                        writer.WriteLine("Script,Begin,");
                        writer.WriteLine("{0}", item.script);
                        writer.WriteLine("Script,End,");
                    }
                    writer.WriteLine("Member,End,");
                }

                writer.WriteLine("MODEL,END,");
            }
            writer.Close();
        }

        public static void MakeStringScheduleAdditional(out string buf, SCHEDULE_ADDITIONAL add)
        {
            if (add.type == 0)
            {
                if (Tools.IsLangKorean())
                {
                    buf = "특정일운전";
                }
                else if (Tools.IsLangJapanese())
                {
                    buf = "特定日運転";
                }
                else if (Tools.IsLangChinese())
                {
                    buf = "操作于特定日";
                }
                else
                {
                    buf = "Special Day Control";
                }
            }
            else if (add.type == 1)
            {
                if (Tools.IsLangKorean())
                {
                    buf = "공휴일운전";
                }
                else if (Tools.IsLangJapanese())
                {
                    buf = "公休日運転";
                }
                else if (Tools.IsLangChinese())
                {
                    buf = "操作于公休日";
                }
                else
                {
                    buf = "Holiday Control";
                }
            }
            else if (add.type == 3)
            {
                if (Tools.IsLangKorean())
                {
                    buf = "매일 운전";
                }
                else
                {
                    buf = "Everyday Control";
                }
            }
            else
                Holiday.MakeString(out buf, add.user);
        }

        
    }
}
