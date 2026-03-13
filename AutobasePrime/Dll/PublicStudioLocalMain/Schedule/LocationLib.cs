using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AutoLibLocal;
using NetTools;
using System.Windows.Forms;

namespace PublicStudioLocalMain.Schedule
{
    public class LocationItem
    {
        public string sCity;
        public string sNationalCode;    // ISO National code KR, US etc
        public string sLatitude;
        public string sLongitude;
    }

    public class LocationLib
    {
        public static List<LocationItem> arrayLocation = new List<LocationItem>();

        static LocationLib()
        {
            arrayLocation = LoadProjectLocationList();

            // 프로그램에서 오류가 날 수 있으므로 읽지 못한때는 기본 할당을 해준다.
            if (arrayLocation == null)
                arrayLocation = new List<LocationItem>();
        }

        /*
        static void AddOneLocation(string city, string national_code, string latitude, string longitude)
        {
            LocationItem item;

            item = new LocationItem();
            item.sCity = city;
            item.sNationalCode = national_code;
            item.sLongitude = longitude;
            item.sLatitude = latitude;

            arrayLocation.Add(item);
        }
        
        static void MakeDefaultLocation()
        {
            AddOneLocation("Busan", "KR", "35:06:00.00", "129:03:00.00");
            AddOneLocation("Daegu", "KR", "35:52:00.00", "128:35:00.00");
            AddOneLocation("Daejeon", "KR", "36:20:00.00", "127:26:00.00");
            AddOneLocation("Gwangju", "KR", "35:09:00.00", "125:54:00.00");
            AddOneLocation("Incheon", "KR", "37:28:00.00", "126:38:00.00");
            AddOneLocation("Jeju", "KR", "33:31:00.00", "126:32:00.00");
            AddOneLocation("Seoul", "KR", "37:30:00.00", "127:00:00.00");
            AddOneLocation("Ulsan", "KR", "35:34:00.00", "129:19:00.00");
        }*/

        public static List<LocationItem> LoadDefaultLocationList()
        {
            string filename = String.Format("{0}\\SCHEDULE\\DefaultLocation.lstx", Application.StartupPath);

            return LoadLocationList(filename);
        }

        public static List<LocationItem> LoadProjectLocationList()
        {
            string filename = String.Format("{0}\\SCHEDULE\\Location.lstx", TotalConfig.sDirWorkProject);

            List<LocationItem> array = LoadLocationList(filename);

            if (array == null)
            {
                return LoadDefaultLocationList();
            }

            return array;
        }

        static List<LocationItem> LoadLocationList(string filename)
        {
            List<LocationItem> array = new List<LocationItem>();

            if (!File.Exists(filename))
            {
                return null;
            }

            TextReader reader = new StreamReader(filename);

            if (reader == null)
            {
                return null;
            }

            CommaBlockString comma = new CommaBlockString();
            string buf;
            LocationItem item;

            while (true)
            {
                buf = reader.ReadLine();
                if (buf == null) break;
                if (buf.Length == 0) continue;

                item = new LocationItem();

                comma.Set(buf);
                item.sCity = comma.GetString().Trim();
                item.sNationalCode = comma.GetString().Trim();
                item.sLatitude = comma.GetString().Trim();
                item.sLongitude = comma.GetString().Trim();

                array.Add(item);
            }

            return array;
        }

        public static void SaveLocationList()
        {
            string filename;
            TextWriter writer;

            filename = String.Format("{0}\\SCHEDULE", TotalConfig.sDirWorkProject);
            Directory.CreateDirectory(filename);
            filename = String.Format("{0}\\SCHEDULE\\Location.lstx", TotalConfig.sDirWorkProject);

            writer = new StreamWriter(filename);
            if (writer == null) return;

            LocationItem item;

            for (int i = 0; i < arrayLocation.Count; i++)
            {
                item = arrayLocation[i];

                writer.WriteLine("{0},{1},{2},{3},", item.sCity, item.sNationalCode, item.sLatitude, item.sLongitude);
            }
            writer.Close();
        }

        static void CalcLocationInfomation(LocationItem item, out double latitude, out double longitude)
        {
            CommaBlockString comma = new CommaBlockString();

            if (item.sLatitude.IndexOf(':') != -1)
            {
                int hour=0;
                int minute = 0;
                double second = 0;

                comma.SetBlockCode(':');
                comma.Set(item.sLatitude);
                hour = comma.GetInt();
                minute = comma.GetInt();
                second = comma.GetDouble();

                latitude = hour + (minute / 60.0) + (second / 3600.0);
            }
            else if (item.sLatitude.IndexOf(' ') != -1)
            {
                int hour = 0;
                int minute = 0;
                double second = 0;

                comma.SetBlockCode(' ');
                comma.Set(item.sLatitude);
                hour = comma.GetInt();
                minute = comma.GetInt();
                second = comma.GetDouble();

                latitude = hour + (minute / 60.0) + (second / 3600.0);
            }
            else
            {
                latitude = ConvertTool.ToDouble(item.sLatitude);
            }

            if (item.sLongitude.IndexOf(':') != -1)
            {
                int hour = 0;
                int minute = 0;
                double second = 0;

                comma.SetBlockCode(':');
                comma.Set(item.sLongitude);
                hour = comma.GetInt();
                minute = comma.GetInt();
                second = comma.GetDouble();

                longitude = hour + (minute / 60.0) + (second / 3600.0);
            }
            else if (item.sLongitude.IndexOf(' ') != -1)
            {
                int hour = 0;
                int minute = 0;
                double second = 0;

                comma.SetBlockCode(' ');
                comma.Set(item.sLongitude);
                hour = comma.GetInt();
                minute = comma.GetInt();
                second = comma.GetDouble();

                longitude = hour + (minute / 60.0) + (second / 3600.0);
            }
            else
            {
                longitude = ConvertTool.ToDouble(item.sLongitude);
            }
        }

        public static bool CalcLocationInfomationByTitle(List<LocationItem> array, string title, out double latitude, out double longitude)
        {
            for (int i = 0; i < array.Count; i++)
            {
                if (array[i].sCity == title)
                {
                    LocationItem item = array[i];
                    CalcLocationInfomation(item, out latitude, out longitude);
                    return true;
                }
            }

            latitude = 0;
            longitude = 0;

            return false;
        }
    }
}
