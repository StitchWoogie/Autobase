using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace GraphicModule
{
    class JsonTool
    {

        // {},{} 를 분리한다.
        static List<string> SplitClass(string s, int pos_s, int pos_e)
        {
            List<string> array = new List<string>();

            int curly_open = 0; // {}

            int start = 0, end = 0;
            bool string_start = false;  // ""
            int ch;
            bool slash_flag = false;

            for (int i = pos_s; i <= pos_e; i++)
            {
                ch = s[i];

                if (string_start)
                {
                    if (slash_flag)
                    {
                        slash_flag = false;
                    }
                    else
                    {
                        if (ch == '"')
                        {
                            string_start = false;
                        }
                        else if (ch == '\\')
                        {
                            slash_flag = true;
                        }
                    }
                }
                else
                {
                    if (ch == '{')
                    {
                        curly_open++;
                        start = i;
                    }
                    else if (ch == '}')
                    {
                        curly_open--;
                        end = i;
                    }
                    else if (ch == '"')
                    {
                        string_start = true;
                    }
                    else if (ch == ',')
                    {
                        if (curly_open == 0)
                        {
                            array.Add(s.Substring(start, end - start + 1));
                            start = 0;
                            end = 0;
                        }
                    }
                }
            }

            if (start != end)
            {
                array.Add(s.Substring(start, end - start + 1));
            }

            return array;
        }

        // { "name1" : "value1", "name2" : value2 }  를 분리한다.
        static List<string> SplitMember(string s)
        {
            int pos_s = 0;
            int pos_e = s.Length - 1;

            // 바깥에 있는 중괄호를 제거한다.
            if (s[pos_s] == '{' && s[pos_e] == '}')
            {
                pos_s++;
                pos_e--;
            }

            List<string> array = new List<string>();

            int curly_open = 0; // {}

            int start = -1;
            bool string_start = false;  // ""
            int ch;

            for (int i = pos_s; i <= pos_e; i++)
            {
                ch = s[i];

                if (string_start)
                {
                    if (ch == '"')
                    {
                        string_start = false;
                    }
                }
                else
                {
                    if (ch == '{')
                    {
                        curly_open++;
                        //start = i;
                    }
                    else if (ch == '}')
                    {
                        curly_open--;
                        //end = i;
                    }
                    else if (ch == '"')
                    {
                        string_start = true;
                    }
                    else if (ch == ',')
                    {
                        if (curly_open == 0)
                        {
                            array.Add(s.Substring(start, i - start));
                            start = -1;
                            //end = 0;
                        }
                    }
                    else
                    {
                        if (start == -1)
                            start = i;
                    }
                }
            }

            if (start != -1)
            {
                array.Add(s.Substring(start, pos_e - start + 1));
            }

            return array;
        }


        static string RemoveSlashString(string s)
        {
            StringBuilder sb = new StringBuilder();

            bool slash_flag = false;

            for (int i = 0; i < s.Length; i++)
            {
                if (slash_flag)
                {
                    slash_flag = false;
                    sb.Append(s[i]);
                }
                else
                {
                    if (s[i] == '\\')
                    {
                        slash_flag = true;
                    }
                    else
                    {
                        sb.Append(s[i]);
                    }
                }
            }

            return sb.ToString();
        }

        // My Source
        public static DataTable ToDataTable(string s)
        {
            // s = [{ "_id" : ObjectId("5ce794e3de3ac84a842aeb9e"), "PersonDate" : "2019-05-25T01:23:45.000Z", "PersonAge" : 9998 }, { "_id" : ObjectId("5cecd166de3ac8225cee60c9"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cecd475de3ac8543851dfb7"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cecdf22de3ac8567ce0e5d2"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cecdf40de3ac87ad84fba26"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cecdf53de3ac87ad84fba27"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cecdf62de3ac87ad84fba28"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cecdf83de3ac86a3489b9c1"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cecdf8bde3ac86a3489b9c2"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cece01ade3ac838d8200573"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cece208de3ac86ff4a573c7"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cece569de3ac87954c4d3d2"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cecea3bde3ac835ec286dcd"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf0b839de3ac868cc2bf10b"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf0b851de3ac865a06d6b42"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf0b9c5de3ac8650c8b8ad2"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf0bacade3ac8650c8b8ad3"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf0bb0ede3ac84b4085fba9"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf0bc72de3ac84b4085fbaa"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf0bcd4de3ac866e4b511b5"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf0bdb3de3ac87780163b4c"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf0bdfdde3ac8436ca9b000"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf0be1dde3ac81fec2e119f"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf0be40de3ac85cf8f65ce2"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf733e4de3ac8480c02a670"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf7590cde3ac828e8e48f4d"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf75968de3ac81b2868b7e7"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf75c2fde3ac87280372318"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf76880de3ac8261086c8fb"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf768a0de3ac86ec8a51794"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf769d0de3ac82fd8598f27"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf769fede3ac87120b876e5"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf76abdde3ac854c8cd46b4"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf76b5ede3ac863f85c652d"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf76c38de3ac838b0989245"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf76d2fde3ac8338466ffe4"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf76e0fde3ac848e06ef4f8"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf76ea7de3ac873c86d9337"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf76ef6de3ac873c86d9338"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf76ef8de3ac873c86d9339"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf76efdde3ac873c86d933a"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf76f76de3ac84eb095e2a2"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf77140de3ac85bf406a0f4"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf7721ade3ac85bf406a0f5"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf77315de3ac86078a219f5"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf77354de3ac84be889630c"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cf77366de3ac85f085f0a35"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cfcafcdde3ac866c4aa25de"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cfcb00bde3ac8730818aea7"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cfcb0c6de3ac859b851e936"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cfcb11ede3ac81200859ad3"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cfcb194de3ac894040d6530"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cfcb1c4de3ac868bc26b759"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cfcb205de3ac850285dd6f1"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cfda088de3ac80fc4ef81f5"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }, { "_id" : ObjectId("5cfda0a2de3ac859d84449f0"), "PersonDate" : ISODate("2019-05-25T01:23:45Z"), "PersonAge" : 9998 }]
            // row = 56
            DataTable dt = new DataTable();

            int pos_s = 0;
            int pos_e = s.Length - 1;

            // 처음에 있는 대괄호를 제거한다.
            if (s[pos_s] == '[' && s[pos_e] == ']')
            {
                pos_s++;
                pos_e--;
            }

            List<string> array = SplitClass(s, pos_s, pos_e);

            List<string> array_member;
            List<string> ColumnsName = new List<string>();

            for (int i = 0; i < array.Count; i++)
            {
                array_member = SplitMember(array[i]);

                for (int j = 0; j < array_member.Count; j++)
                {
                    string ColumnsNameData = array_member[j];
                    int idx = ColumnsNameData.IndexOf(":");
                    string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                    ColumnsNameString = ColumnsNameString.Trim();
                    if (!ColumnsName.Contains(ColumnsNameString))
                    {
                        ColumnsName.Add(ColumnsNameString);
                    }
                }
            }

            // 먼저 컬럼을 모두 만들어야 한다.
            for (int i = 0; i < ColumnsName.Count; i++)
            {
                dt.Columns.Add(ColumnsName[i]);
            }

            for (int i = 0; i < array.Count; i++)
            {
                array_member = SplitMember(array[i]);

                DataRow row = dt.NewRow();

                for (int j = 0; j < array_member.Count; j++)
                {
                    string ColumnsNameData = array_member[j];
                    int idx = ColumnsNameData.IndexOf(":");
                    string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                    ColumnsNameString = ColumnsNameString.Trim();
                    string RowDataString = ColumnsNameData.Substring(idx + 1);
                    RowDataString = RowDataString.Trim();
                    if (RowDataString.Length >= 2 && RowDataString[0] == '"' && RowDataString[RowDataString.Length - 1] == '"')
                    {
                        RowDataString = RowDataString.Substring(1, RowDataString.Length - 2);
                    }

                    row[ColumnsNameString] = RemoveSlashString(RowDataString);
                }

                dt.Rows.Add(row);
            }

            return dt;
        }

        // 인터넷 공개 소스
        public static DataTable JsonStringToDataTable(string jsonString)
        {
            DataTable dt = new DataTable();
            string[] jsonStringArray = System.Text.RegularExpressions.Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "}, {");
            List<string> ColumnsName = new List<string>();
            foreach (string jSA in jsonStringArray)
            {
                string[] jsonStringData = System.Text.RegularExpressions.Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                foreach (string ColumnsNameData in jsonStringData)
                {
                    try
                    {
                        int idx = ColumnsNameData.IndexOf(":");
                        string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "");
                        if (!ColumnsName.Contains(ColumnsNameString))
                        {
                            ColumnsName.Add(ColumnsNameString);
                        }
                    }
                    catch
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
                    }
                }
                break;
            }
            foreach (string AddColumnName in ColumnsName)
            {
                dt.Columns.Add(AddColumnName);
            }
            foreach (string jSA in jsonStringArray)
            {
                string[] RowData = System.Text.RegularExpressions.Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
                DataRow nr = dt.NewRow();
                foreach (string rowData in RowData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string RowColumns = rowData.Substring(0, idx - 1).Replace("\"", "");
                        string RowDataString = rowData.Substring(idx + 1).Replace("\"", "");
                        nr[RowColumns] = RowDataString;
                    }
                    catch
                    {
                        continue;
                    }
                }
                dt.Rows.Add(nr);
            }
            return dt;
        }
    }
}
