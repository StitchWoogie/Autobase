using System;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using NetTools;
using System.IO;
using System.Data.Odbc;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;

namespace AutoLibLocal
{
    [Flags]
    public enum EnumDataType
    {   // 아날로그 자료의 형태.
        AVE = 0x0001,
        MIN = 0x0002,
        MAX = 0x0004,
        SUM = 0x0008,
        SUB = 0x0010,
        CURR = 0x0020,
        MOMENT = 0x0040,        // 순시값

        ONTIME = 0x0080,    // On된 시간.
        OFFTIME = 0x0100,   // Off된 시간.
        COUNT = 0x0200, // on/off count

        AveMinMaxSum = AVE | MIN | MAX | SUM,
        CountOntime = COUNT | ONTIME,

    }

    public enum EnumDataTime : int
    {   // 아날로그 자료의 형태.
        Minute = 0, // 분자료
        Hour = 1,   // 분자료
        Day = 2,    // 분자료
        Month = 3,  // 분자료
        Week = 4,	// 주자료
        Year = 5,   // 년자료 2023-4-6 추가함.
    }
    public class TREND_AI_STRUCT
    {
        public float fSumMin;   // 1분 동안에 흘렀을 유량 (실제 적산치)
        public float fAverage;  // 1분 동안에 계측된 값의 평균 (적산 평균 아님)
        public float fMin;      // 1분 동안의 최소값
        public float fMax;      // 1분 동안의 최소값
        public float fCurr;     // 저장 당시의 현재값.

        public static readonly int struct_size = 20;
    }

    public class FILE_TREND_AI_STRUCT
    {
        public byte day;            // 일
        public byte hour;           // 월
        public byte min;            // 분
        public TREND_AI_STRUCT data = new TREND_AI_STRUCT();
        public ushort crc;          // data 의 crc

        byte[] SingleToBytes(float val)
        {
            byte[] b = new byte[4];

            MemoryStream ms = new MemoryStream(b);
            BinaryWriter br = new BinaryWriter(ms);
            br.Write(val);
            br.Close();
            return b;
        }

        public ushort CalcCRC()
        {
            ushort c;
            c = 0;

            c += day;
            c += hour;
            c += min;

            int i;
            byte[] b;
            b = SingleToBytes(data.fAverage);
            for (i = 0; i < 4; i++) c += b[i];
            b = SingleToBytes(data.fCurr);
            for (i = 0; i < 4; i++) c += b[i];
            b = SingleToBytes(data.fMax);
            for (i = 0; i < 4; i++) c += b[i];
            b = SingleToBytes(data.fMin);
            for (i = 0; i < 4; i++) c += b[i];
            b = SingleToBytes(data.fSumMin);
            for (i = 0; i < 4; i++) c += b[i];

            return c;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(day);
            writer.Write(hour);
            writer.Write(min);
            writer.Write(data.fSumMin);
            writer.Write(data.fAverage);
            writer.Write(data.fMin);
            writer.Write(data.fMax);
            writer.Write(data.fCurr);
            writer.Write(crc);
        }

        public void Read(BinaryReader reader)
        {
            day = reader.ReadByte();
            hour = reader.ReadByte();
            min = reader.ReadByte();
            data.fSumMin = reader.ReadSingle();
            data.fAverage = reader.ReadSingle();
            data.fMin = reader.ReadSingle();
            data.fMax = reader.ReadSingle();
            data.fCurr = reader.ReadSingle();
            crc = reader.ReadUInt16();
        }

        public static readonly int struct_size = TREND_AI_STRUCT.struct_size + 5;
    }

    public class HOUR_DATA_ANALOG_STRUCT
    {
        public float fSumHour;      // 한 시간 동안에 흘렀을 적산치
        public float fAveHour;      // 한 시간 동안의 계측 평균치 (적산평균치 아님)
        public float fMinHour;      // 한 시간 동안의 최소치
        public float fMaxHour;      // 한 시간 동안의 최고치
        public float fCurrSumMeter; // 시간대 마지막에 계측된 적산 계량기 눈금
        public byte flag;           // OFF이면 파일 초기화만 되어 있고 저장되지는 않았다.
        public ushort crc;

        public static readonly int struct_size = 23;

        byte[] SingleToBytes(float val)
        {
            byte[] b = new byte[4];

            MemoryStream ms = new MemoryStream(b);
            BinaryWriter br = new BinaryWriter(ms);
            br.Write(val);
            br.Close();
            return b;
        }

        public ushort CalcCRC()
        {
            ushort c = 0;

            int i;
            byte[] b;
            b = SingleToBytes(fSumHour);
            for (i = 0; i < 4; i++) c += b[i];
            b = SingleToBytes(fAveHour);
            for (i = 0; i < 4; i++) c += b[i];
            b = SingleToBytes(fMinHour);
            for (i = 0; i < 4; i++) c += b[i];
            b = SingleToBytes(fMaxHour);
            for (i = 0; i < 4; i++) c += b[i];
            b = SingleToBytes(fCurrSumMeter);
            for (i = 0; i < 4; i++) c += b[i];
            c += flag;

            return c;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(fSumHour);
            writer.Write(fAveHour);
            writer.Write(fMinHour);
            writer.Write(fMaxHour);
            writer.Write(fCurrSumMeter);
            writer.Write(flag);
            writer.Write(crc);
        }
    }

    public class HOUR_DATA_DIGITAL_STRUCT
    {
        public ushort wCountOnOff;  // 한 시간동안 ON/OFF 된 횟수.
        public uint dwOnTime;       // 접점이 ON 된 시간 (단위 sec)
        public byte flag;           // OFF이면 파일 초기화만 되어 있고 저장되지는 않았다.
        public ushort crc;

        public static readonly int struct_size = 9;

        public ushort CalcCRC()
        {
            ushort c = 0;

            c += (ushort)((wCountOnOff >> 8) & 0xFF);
            c += (ushort)((wCountOnOff >> 0) & 0xFF);
            c += (ushort)((dwOnTime >> 24) & 0xFF);
            c += (ushort)((dwOnTime >> 16) & 0xFF);
            c += (ushort)((dwOnTime >> 8) & 0xFF);
            c += (ushort)((dwOnTime >> 0) & 0xFF);
            c += flag;

            return c;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(wCountOnOff);
            writer.Write(dwOnTime);
            writer.Write(flag);
            writer.Write(crc);
        }
    }

    /*
	enum 
	{	// 디지털 자료의 형태.
		DI_DATA_TYPE_ONTIME,	// On된 시간.
		DI_DATA_TYPE_OFFTIME,	// Off된 시간.
		DI_DATA_TYPE_COUNT,		// on/off count
		DI_DATA_TYPE_CURR,
		DI_DATA_TYPE_MOMENT,	// 순시값
	}
	*/

    public class HOUR_DATA_HEAD
    {
        public byte[] id = new byte[5];         // HOUR
        public short version;       // 1
        public byte[] extra = new byte[11];     // 이전에는 태그이름으로 사용했으나 40자로 증가되면서 무의미한 배열이 됨.

        public static readonly int struct_size = 18;

        public void Write(BinaryWriter writer)
        {
            writer.Write(id);
            writer.Write(version);
            writer.Write(extra);
        }
    }

    public class TREND_DI_STRUCT
    {
        public short nCountOnOff;   // 1분동안 접점이 ON/OFF 된 횟수.
        public byte bOnOff;         // 자료 저장 시의 ON/OFF 상태
        public byte cOnTime;        // 1분동안 접점이 ON 된 시간(단위 sec)

        public static readonly int struct_size = 4;
    }

    public class FILE_TREND_DI_STRUCT
    {
        public byte day;        // 일
        public byte hour;       // 월
        public byte min;        // 분
        public TREND_DI_STRUCT data = new TREND_DI_STRUCT();
        public ushort crc;      // data 의 crc

        public static readonly int struct_size = TREND_DI_STRUCT.struct_size + 5;

        public void Write(BinaryWriter writer)
        {
            writer.Write(day);
            writer.Write(hour);
            writer.Write(min);
            writer.Write(data.nCountOnOff);
            writer.Write(data.bOnOff);
            writer.Write(data.cOnTime);
            writer.Write(crc);
        }

        public void Read(BinaryReader reader)
        {
            day = reader.ReadByte();
            hour = reader.ReadByte();
            min = reader.ReadByte();
            data.nCountOnOff = reader.ReadInt16();
            data.bOnOff = reader.ReadByte();
            data.cOnTime = reader.ReadByte();
            crc = reader.ReadUInt16();
        }

        public ushort CalcCRC()
        {
            ushort c = 0;

            c += day;
            c += hour;
            c += min;

            c += (ushort)((data.nCountOnOff) / 256);
            c += (ushort)((data.nCountOnOff) % 256);
            c += data.bOnOff;
            c += data.cOnTime;

            return c;
        }
    }

    /// <summary>
    /// Summary description for DataLocal.
    /// </summary>
    public class DataLocal_fileSystem
    {


        public DataLocal_fileSystem()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public DataSet GetTagValueList(ArrayList array)
        {
            DataSet ds = new DataSet("TAG");

            string tag;

            DataTable dt = new DataTable("TAG");
            DataColumn dc;

            dc = new DataColumn("tag", Type.GetType("System.String"));
            dc.MaxLength = -1;
            dt.Columns.Add(dc);
            dc = new DataColumn("curr", Type.GetType("System.String"));
            dc.MaxLength = -1;
            dt.Columns.Add(dc);

            DataRow row;

            string curr = "";
            bool retn;

            for (int i = 0; i < array.Count; i++)
            {
                tag = (string)array[i];

                row = dt.NewRow();
                row[0] = tag;
                curr = "";
                retn = SharedTag.GetCurr(tag, ref curr);
                row[1] = curr;
                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);

            return ds;
        }

        // 어차피 이부분은 SharedTag에 바로 보내면 되기 때문에 삭제했다. 2011-4-15
        /*
		public void WriteCurrAI(string tag, double val)
		{
			SharedTag.SetCurr(tag, val);
		}

		public void WriteCurrST(string tag, string val, string user, string ip, string computer)
		{
			SharedTag.SetCurr(tag, val, user, ip, computer);
		}

        // LocalMain으로 보낸다.
        public void WriteCurrToLocalMain(string tag, string val, string user, string ip, string computer)
        {
            SharedTag.SetCurr(tag, val, user, ip, computer);
        }

        public void WriteCurr(string tag, object val, int delay_sec, string user, string ip, string computer)
		{
			SharedTag.SetCurrDelaySec(tag, val, delay_sec, user, ip, computer);
		}*/

        //bool CatDataGetAi(string tag, ref double val, EnumDataType data_type, EnumDataTime data_time, int year, int mon, int day, int hour, int min)
        //{
        //    bool retn;

        //    if (data_time == EnumDataTime.Minute)
        //        retn = CatDataGetAiMin(0, tag, ref val, year, mon, day, hour, min, data_type);
        //    else if (data_time == EnumDataTime.Hour)
        //        retn = CatDataGetAiHour(0, tag, ref val, year, mon, day, hour, min, data_type);
        //    else if (data_time == EnumDataTime.Day)
        //        retn = CatDataGetAiDay(0, tag, ref val, year, mon, day, hour, min, data_type);
        //    else if (data_time == EnumDataTime.Week)
        //        retn = CatDataGetAiWeek(0, tag, ref val, year, mon, data_type);
        //    else if (data_time == EnumDataTime.Month)
        //        retn = CatDataGetAiMonth(0, tag, ref val, year, mon, day, hour, min, data_type);
        //    else if (data_time == EnumDataTime.Year)    // 2023-4-11 추가
        //        retn = CatDataGetAiYear(0, tag, ref val, year, mon, day, hour, min, data_type);
        //    else
        //        retn = false;

        //    if (!retn) val = 0;

        //    return retn;
        //}


        // 시작 시간이 data_gab의 배수가 아니면 데이터가 일률적이지 않고 나머지마다 달라서 일치시키려고 했으나 잠깐 보류한다.
        //void FitTimeToDataGab(EnumDataTime data_time, ref int year, ref int mon, ref int day, ref int hour, ref int min, int data_gab)
        //{
        //    if (data_gab <= 1) return;

        //    int start;
        //    int minus_gab;

        //    start = (min / data_gab) * data_gab;
        //    minus_gab = min - start;
        //    for(int i = 0; i < minus_gab; i++)
        //        TimeUtil.MinusMin(ref year, ref mon, ref day, ref hour, ref min);
        //}

        //public DataSet GetDataAi(string tag, EnumDataType value_type, EnumDataTime data_time, int year, int mon, int day, int hour, int min, int data_count, int data_gab)
        //{
        //    //if (data_gab > 1)
        //    //FitTimeToDataGab(data_time, ref year, ref mon, ref day, ref hour, ref min, data_gab);

        //    DataSet ds = new DataSet();
        //    DataTable dt;

        //    dt = new DataTable(tag);

        //    DataColumn dc;
        //    DataRow row;
        //    double val = 0;
        //    bool retn;

        //    dc = new DataColumn("Flag", Type.GetType("System.SByte"));
        //    dt.Columns.Add(dc);

        //    if ((value_type & EnumDataType.AVE) == EnumDataType.AVE)
        //    {
        //        dc = new DataColumn("AVE");
        //        dt.Columns.Add(dc);
        //    }

        //    if ((value_type & EnumDataType.MAX) == EnumDataType.MAX)
        //    {
        //        dc = new DataColumn("MAX");
        //        dt.Columns.Add(dc);
        //    }

        //    if ((value_type & EnumDataType.MIN) == EnumDataType.MIN)
        //    {
        //        dc = new DataColumn("MIN");
        //        dt.Columns.Add(dc);
        //    }

        //    if ((value_type & EnumDataType.MOMENT) == EnumDataType.MOMENT)
        //    {
        //        dc = new DataColumn("MOMENT");
        //        dt.Columns.Add(dc);
        //    }

        //    if ((value_type & EnumDataType.SUB) == EnumDataType.SUB)
        //    {
        //        dc = new DataColumn("SUB");
        //        dt.Columns.Add(dc);
        //    }

        //    if ((value_type & EnumDataType.SUM) == EnumDataType.SUM)
        //    {
        //        dc = new DataColumn("SUM");
        //        dt.Columns.Add(dc);
        //    }

        //    for (int i = 0; i < data_count; i++)
        //    {
        //        row = dt.NewRow();

        //        retn = false;

        //        if ((value_type & EnumDataType.AVE) == EnumDataType.AVE)
        //        {
        //            retn = CatDataGetAi(tag, ref val, EnumDataType.AVE, data_time, year, mon, day, hour, min);
        //            row["AVE"] = val.ToString();
        //        }
        //        if ((value_type & EnumDataType.MAX) == EnumDataType.MAX)
        //        {
        //            retn = CatDataGetAi(tag, ref val, EnumDataType.MAX, data_time, year, mon, day, hour, min);
        //            row["MAX"] = val.ToString();
        //        }
        //        if ((value_type & EnumDataType.MIN) == EnumDataType.MIN)
        //        {
        //            retn = CatDataGetAi(tag, ref val, EnumDataType.MIN, data_time, year, mon, day, hour, min);
        //            row["MIN"] = val.ToString();
        //        }
        //        if ((value_type & EnumDataType.MOMENT) == EnumDataType.MOMENT)
        //        {
        //            retn = CatDataGetAi(tag, ref val, EnumDataType.MOMENT, data_time, year, mon, day, hour, min);
        //            row["MOMENT"] = val.ToString();
        //        }
        //        if ((value_type & EnumDataType.SUB) == EnumDataType.SUB)
        //        {
        //            retn = CatDataGetAi(tag, ref val, EnumDataType.SUB, data_time, year, mon, day, hour, min);
        //            row["SUB"] = val.ToString();
        //        }
        //        if ((value_type & EnumDataType.SUM) == EnumDataType.SUM)
        //        {
        //            retn = CatDataGetAi(tag, ref val, EnumDataType.SUM, data_time, year, mon, day, hour, min);
        //            row["SUM"] = val.ToString();
        //        }

        //        if (retn)
        //        {
        //            row["Flag"] = 1;
        //        }
        //        else
        //        {
        //            row["Flag"] = 0;
        //        }

        //        dt.Rows.Add(row);

        //        for (int t = 0; t < data_gab; t++)
        //        {
        //            if (data_time == EnumDataTime.Minute)
        //                TimeUtil.PlusMin(ref year, ref mon, ref day, ref hour, ref min);
        //            else if (data_time == EnumDataTime.Hour)
        //                TimeUtil.PlusHour(ref year, ref mon, ref day, ref hour);
        //            else if (data_time == EnumDataTime.Day)
        //                TimeUtil.PlusDay(ref year, ref mon, ref day);
        //            else if (data_time == EnumDataTime.Week)
        //                mon++;
        //            else if (data_time == EnumDataTime.Month)
        //                TimeUtil.PlusMonth(ref year, ref mon);
        //            else if (data_time == EnumDataTime.Year)    // 2023-4-11 추가
        //                TimeUtil.PlusYear(ref year);
        //        }
        //    }

        //    ds.Tables.Add(dt);

        //    return ds;
        //}

        // 캐시 매니저 인스턴스 (싱글톤)
        private static DataCacheManager _cacheManager = new DataCacheManager();


        #region AI 분별 데이터 로드 (캐시 적용)
        public bool LoadMinDataStructAI(string tag, int year, int mon, int day, int hour, int min, TREND_AI_STRUCT data)
        {
            return LoadMinDataStructAI(tag, year, mon, day, hour, min, data, false); // 기본값은 캐시 사용
        }

        // 시간별 데이터 로드 메서드 (original 플래그 포함)
        public bool LoadMinDataStructAI(string tag, int year, int month, int day, int hour, int min, TREND_AI_STRUCT data, bool original)
        {
            if (original)
            {
                // 기존 방식 (매번 파일 열기)
                return LoadMinDataStructAI_Original(tag, year, month, day, hour, min, data);
            }
            else
            {
                // 캐시 방식 (시간별 무효화 적용)
                return LoadMinDataStructAI_Cached(tag, year, month, day, hour, min, data);
            }
        }

        public bool LoadMinDataStructAI(string tag, DateTime t, TREND_AI_STRUCT data)
        {
            return LoadMinDataStructAI(tag, t.Year, t.Month, t.Day, t.Hour, t.Minute, data);
        }


        public bool LoadMinDataStructAI_Original(string tag, int year, int mon, int day, int hour, int min, TREND_AI_STRUCT data)
        {
            if (day < 1 || day > 31) return false;
            if (hour < 0 || hour > 23) return false;
            if (min < 0 || min > 59) return false;

            string filename;
            string tag_file;
            string data_dir;

            tag_file = TagUtil.ConvertTagToFile(tag);
            data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
            filename = String.Format("{0}\\TREND\\{1:0000}\\MON{2:00}\\AI\\{3}", data_dir, year, mon, tag_file);

            FileStream fs;

            FILE_TREND_AI_STRUCT trend = new FILE_TREND_AI_STRUCT();

            if (!File.Exists(filename)) return false;

            // 이파일을 사용 중일때 엑셀에서 읽으면 엑셀이 다운되어서 Try Catch를 넣었다. 2012-5-30
            try
            {
                fs = File.OpenRead(filename);
            }
            catch
            {
                fs = null;
            }

            if (fs == null) return false;   // 자료 없음

            BinaryReader br = new BinaryReader(fs);

            long start = 25;
            start = start * ((day - 1) * 1440 + hour * 60L + min);

            try
            {
                fs.Seek(start, SeekOrigin.Begin);

                trend.day = br.ReadByte();
                trend.hour = br.ReadByte();
                trend.min = br.ReadByte();
                data.fSumMin = br.ReadSingle();
                data.fAverage = br.ReadSingle();
                data.fMin = br.ReadSingle();
                data.fMax = br.ReadSingle();
                data.fCurr = br.ReadSingle();
                trend.crc = br.ReadUInt16();
            }
            catch   // 파일이 깨져있을 경우 발생한다.
            {

            }

            fs.Close();

            if (trend.day != day) return false;
            if (trend.hour != hour) return false;
            if (trend.min != min) return false;
            //if(trend.crc != GetCRC16((BYTE*)&trend, sizeof(FILE_TREND_AI_STRUCT)-2))	return 0;

            return true;
        }
        

        public bool LoadMinDataStructAI_Cached(string tag, int year, int mon, int day, int hour, int min, TREND_AI_STRUCT data)
        {
            if (day < 1 || day > 31) return false;
            if (hour < 0 || hour > 23) return false;
            if (min < 0 || min > 59) return false;

            string tag_file = TagUtil.ConvertTagToFile(tag);
            string data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
            string filename = String.Format("{0}\\TREND\\{1:0000}\\MON{2:00}\\AI\\{3}", data_dir, year, mon, tag_file);

            DateTime requestTime = new DateTime(year, mon, day, hour, min, 0);

            // 1. 캐시 유효성 검사
            if (!_cacheManager.IsMinDataAICacheValid(filename, requestTime))
            {
                // 2. 캐시가 무효하거나 없으면 파일에서 로드
                if (!_cacheManager.LoadMinDataAIFile(filename, year, mon))
                {
                    return false;
                }
            }

            // 3. 캐시에서 데이터 조회
            return _cacheManager.GetMinDataAI(filename, day, hour, min, data);
        }

        #endregion



        public bool CatDataGetAiMin(int terminal, string tag, ref double val, int year, int month, int day, int hour, int min, EnumDataType data_type)
        {
            TREND_AI_STRUCT trend = new TREND_AI_STRUCT();

            val = 0.0;

            if (!LoadMinDataStructAI(tag, year, month, day, hour, min, trend))
            {
                return false;
            }

            if (data_type == EnumDataType.AVE)
            {
                val = trend.fAverage;
            }
            else if (data_type == EnumDataType.SUM)
            {
                val = trend.fSumMin;
            }
            else if (data_type == EnumDataType.MIN)
            {
                val = trend.fMin;
            }
            else if (data_type == EnumDataType.MAX)
            {
                val = trend.fMax;
            }
            else if (data_type == EnumDataType.MOMENT)
            {
                val = trend.fCurr;
            }
            else if (data_type == EnumDataType.SUB)
            {
                double value1;

                value1 = trend.fMax;

                TimeUtil.MinusMin(ref year, ref month, ref day, ref hour, ref min);
                if (!LoadMinDataStructAI(tag, year, month, day, hour, min, trend))
                {
                    return false;
                }

                if (value1 < trend.fMax)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
                    val = TagLib.GetTagMemberFull(tag) + value1 - trend.fMax;
                else
                    val = value1 - trend.fMax;
            }
            else
            {
                return false;
            }

            return true;
        }

        bool CatDataGetDi(string tag, ref uint val, EnumDataType data_type, EnumDataTime data_time, int year, int mon, int day, int hour, int min)
        {
            bool retn;

            if (data_time == EnumDataTime.Minute)
                retn = CatDataGetDiMin(0, tag, ref val, year, mon, day, hour, min, data_type);
            else if (data_time == EnumDataTime.Hour)
                retn = CatDataGetDiHour(0, tag, ref val, year, mon, day, hour, data_type);
            else if (data_time == EnumDataTime.Day)
                retn = CatDataGetDiDay(0, tag, ref val, year, mon, day, data_type);
            else if (data_time == EnumDataTime.Week)
                retn = CatDataGetDiWeek(0, tag, ref val, year, mon, data_type);
            else if (data_time == EnumDataTime.Month)
                retn = CatDataGetDiMonth(0, tag, ref val, year, mon, data_type);
            else if (data_time == EnumDataTime.Year)        // 2023-4-11 추가
                retn = CatDataGetDiYear(0, tag, ref val, year, data_type);
            else
                retn = false;

            if (!retn) val = 0;

            return retn;
        }

        public DataSet GetDataDi(string tag, EnumDataType value_type, EnumDataTime data_time, int year, int mon, int day, int hour, int min, int data_count, int data_gab)
        {


            DataSet ds = new DataSet();
            DataTable dt;

            dt = new DataTable(tag);

            DataColumn dc;
            DataRow row;
            uint val = 0;
            bool retn;

            dc = new DataColumn("Flag", Type.GetType("System.SByte"));
            dt.Columns.Add(dc);

            if ((value_type & EnumDataType.COUNT) == EnumDataType.COUNT)
            {
                dc = new DataColumn("COUNT");
                dt.Columns.Add(dc);
            }
            if ((value_type & EnumDataType.MOMENT) == EnumDataType.MOMENT)
            {
                dc = new DataColumn("MOMENT");
                dt.Columns.Add(dc);
            }
            if ((value_type & EnumDataType.OFFTIME) == EnumDataType.OFFTIME)
            {
                dc = new DataColumn("OFFTIME");
                dt.Columns.Add(dc);
            }
            if ((value_type & EnumDataType.ONTIME) == EnumDataType.ONTIME)
            {
                dc = new DataColumn("ONTIME");
                dt.Columns.Add(dc);
            }

            for (int i = 0; i < data_count; i++)
            {
                row = dt.NewRow();

                retn = false;

                if ((value_type & EnumDataType.COUNT) == EnumDataType.COUNT)
                {
                    retn = CatDataGetDi(tag, ref val, EnumDataType.COUNT, data_time, year, mon, day, hour, min);
                    row["COUNT"] = val.ToString();
                }
                if ((value_type & EnumDataType.MOMENT) == EnumDataType.MOMENT)
                {
                    if (data_time == EnumDataTime.Minute)
                        retn = CatDataGetDiMin(0, tag, ref val, year, mon, day, hour, min, EnumDataType.MOMENT);
                    else if (data_time == EnumDataTime.Hour)
                        retn = CatDataGetDiMin(0, tag, ref val, year, mon, day, hour, min, EnumDataType.MOMENT);
                    else if (data_time == EnumDataTime.Day)
                        retn = CatDataGetDiMin(0, tag, ref val, year, mon, day, hour, min, EnumDataType.MOMENT);
                    else if (data_time == EnumDataTime.Month)
                        retn = CatDataGetDiMin(0, tag, ref val, year, mon, day, hour, min, EnumDataType.MOMENT);
                    else
                        retn = false;

                    if (!retn) val = 0;
                    row["MOMENT"] = val.ToString();
                }
                if ((value_type & EnumDataType.OFFTIME) == EnumDataType.OFFTIME)
                {
                    retn = CatDataGetDi(tag, ref val, EnumDataType.OFFTIME, data_time, year, mon, day, hour, min);
                    row["OFFTIME"] = val.ToString();
                }
                if ((value_type & EnumDataType.ONTIME) == EnumDataType.ONTIME)
                {
                    retn = CatDataGetDi(tag, ref val, EnumDataType.ONTIME, data_time, year, mon, day, hour, min);
                    row["ONTIME"] = val.ToString();
                }

                if (retn)
                {
                    row["Flag"] = 1;
                }
                else
                {
                    row["Flag"] = 0;
                }

                dt.Rows.Add(row);

                for (int t = 0; t < data_gab; t++)
                {
                    if (data_time == EnumDataTime.Minute)
                        TimeUtil.PlusMin(ref year, ref mon, ref day, ref hour, ref min);
                    else if (data_time == EnumDataTime.Hour)
                        TimeUtil.PlusHour(ref year, ref mon, ref day, ref hour);
                    else if (data_time == EnumDataTime.Day)
                        TimeUtil.PlusDay(ref year, ref mon, ref day);
                    else if (data_time == EnumDataTime.Week)
                        mon++;
                    else if (data_time == EnumDataTime.Month)
                        TimeUtil.PlusMonth(ref year, ref mon);
                    else if (data_time == EnumDataTime.Year)    // 2023-4-11 추가
                        TimeUtil.PlusYear(ref year);
                }
            }

            ds.Tables.Add(dt);

            return ds;
        }

        public bool CatDataGetDiMin(int terminal, string tag, ref uint val, int year, int month, int day, int hour, int min, EnumDataType data_type)
        {
            TREND_DI_STRUCT trend = new TREND_DI_STRUCT();

            val = 0;

            if (!LoadMinDataStructDI(tag, year, month, day, hour, min, trend, false))
            {
                return false;
            }

            if (data_type == EnumDataType.ONTIME)
            {
                val = trend.cOnTime;
            }
            else if (data_type == EnumDataType.OFFTIME)
            {
                val = (uint)(60 - trend.cOnTime);
            }
            else if (data_type == EnumDataType.COUNT)
            {
                val = (uint)trend.nCountOnOff;
            }
            else if (data_type == EnumDataType.MOMENT)
            {
                val = (uint)trend.bOnOff;
            }
            else
            {
                return false;
            }

            return true;
        }

        #region DI 분별 데이터 로드 (캐시 적용)
        public bool LoadMinDataStructDI(string tag, int year, int mon, int day, int hour, int min, TREND_DI_STRUCT data)
        {      // 캐시 방식 (실시간 무효화 적용)
            return LoadMinDataStructDI_Cached(tag, year, mon, day, hour, min, data);
        }

        public bool LoadMinDataStructDI(string tag, DateTime t, TREND_DI_STRUCT data)
        {
            return LoadMinDataStructDI(tag, t.Year, t.Month, t.Day, t.Hour, t.Minute, data, false); // 기본값은 캐시 사용
        }


        // DI 데이터 로드 메서드 (original 플래그 포함)
        public bool LoadMinDataStructDI(string tag, int year, int mon, int day, int hour, int min, TREND_DI_STRUCT data, bool original)
        {
            if (original)
            {
                // 기존 방식 (매번 파일 열기)
                return LoadMinDataStructDI_Original(tag, year, mon, day, hour, min, data);
            }
            else
            {
                // 캐시 방식 (실시간 무효화 적용)
                return LoadMinDataStructDI_Cached(tag, year, mon, day, hour, min, data);
            }
        }

        public bool LoadMinDataStructDI_Original(string tag, int year, int mon, int day, int hour, int min, TREND_DI_STRUCT data)
        {
            if (day < 1 || day > 31) return false;
            if (hour < 0 || hour > 23) return false;
            if (min < 0 || min > 59) return false;

            string filename;
            string tag_file;
            string data_dir;

            tag_file = TagUtil.ConvertTagToFile(tag);
            data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
            filename = String.Format("{0}\\TREND\\{1:0000}\\MON{2:00}\\DI\\{3}", data_dir, year, mon, tag_file);

            FileStream fs;

            FILE_TREND_DI_STRUCT trend = new FILE_TREND_DI_STRUCT();

            if (!File.Exists(filename)) return false;

            // 이파일을 사용 중일때 엑셀에서 읽으면 엑셀이 다운되어서 Try Catch를 넣었다. 2012-5-30
            try
            {
                fs = File.OpenRead(filename);
            }
            catch
            {
                fs = null;
            }

            if (fs == null) return false;   // 자료 없음

            BinaryReader br = new BinaryReader(fs);

            long start = 9;
            start = start * ((day - 1) * 1440 + hour * 60L + min);

            try
            {
                fs.Seek(start, SeekOrigin.Begin);

                trend.day = br.ReadByte();
                trend.hour = br.ReadByte();
                trend.min = br.ReadByte();
                data.nCountOnOff = br.ReadInt16();
                data.bOnOff = br.ReadByte();
                data.cOnTime = br.ReadByte();
                trend.crc = br.ReadUInt16();
            }
            catch   // 파일이 깨져 있을 경우를 대비
            {

            }

            fs.Close();

            if (trend.day != day) return false;
            if (trend.hour != hour) return false;
            if (trend.min != min) return false;
            //if(trend.crc != GetCRC16((BYTE*)&trend, sizeof(FILE_TREND_DI_STRUCT)-2))	return 0;

            return true;
        }



        // DI 캐시 방식 구현
        private bool LoadMinDataStructDI_Cached(string tag, int year, int mon, int day, int hour, int min, TREND_DI_STRUCT data)
        {
            if (day < 1 || day > 31) return false;
            if (hour < 0 || hour > 23) return false;
            if (min < 0 || min > 59) return false;

            string tag_file = TagUtil.ConvertTagToFile(tag);
            string data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
            string filename = String.Format("{0}\\TREND\\{1:0000}\\MON{2:00}\\DI\\{3}", data_dir, year, mon, tag_file);

            DateTime requestTime = new DateTime(year, mon, day, hour, min, 0);

            // 1. 캐시 유효성 검사
            if (!_cacheManager.IsMinDataDICacheValid(filename, requestTime))
            {
                // 2. 캐시가 무효하거나 없으면 파일에서 로드
                if (!_cacheManager.LoadMinDataDIFile(filename, year, mon))
                {
                    return false;
                }
            }

            // 3. 캐시에서 데이터 조회
            return _cacheManager.GetMinDataDI(filename, day, hour, min, data);
        }
        #endregion


        #region AI 시간별 데이터 로드 (캐시 적용)


        public bool LoadHourDataStructAI(string tag, DateTime t, HOUR_DATA_ANALOG_STRUCT data)
        {
            return LoadHourDataStructAI(tag, t.Year, t.Month, t.Day, t.Hour, data, false);// 기본값은 캐시 사용
        }

        // 시간별 데이터 로드 메서드 (original 플래그 포함)
        public bool LoadHourDataStructAI(string tag, int year, int month, int day, int hour, HOUR_DATA_ANALOG_STRUCT data, bool original)
        {
            if (original)
            {
                // 기존 방식 (매번 파일 열기)
                return LoadHourDataStructAI_Original(tag, year, month, day, hour, data);
            }
            else
            {
                // 캐시 방식 (시간별 무효화 적용)
                return LoadHourDataStructAI_Cached(tag, year, month, day, hour, data);
            }
        }

        public bool LoadHourDataStructAI_Original(string tag, int year, int month, int day, int hour, HOUR_DATA_ANALOG_STRUCT data)
        {
            if (day < 1 || day > 31) return false;
            if (hour < 0 || hour > 23) return false;

            string tag_file;
            string data_dir;
            string filename;

            // HOUR_DATA_HEAD head; size = 18
            FileStream fs;

            tag_file = TagUtil.ConvertTagToFile(tag);
            data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
            filename = String.Format("{0}\\SUM\\{1:0000}\\MON{2:00}\\AI\\{3}", data_dir, year, month, tag_file);

            if (!File.Exists(filename)) return false;

            try
            {
                fs = File.OpenRead(filename);
            }
            catch
            {
                fs = null;
            }

            if (fs == null) return false;

            if (fs.Length != 18 + HOUR_DATA_ANALOG_STRUCT.struct_size * 24 * 31)
            {
                fs.Close();
                return false;
            }

            BinaryReader br = new BinaryReader(fs);

            try
            {
                fs.Seek(18 + ((day - 1) * 24L + hour) * 23, SeekOrigin.Begin);

                data.fSumHour = br.ReadSingle();
                data.fAveHour = br.ReadSingle();
                data.fMinHour = br.ReadSingle();
                data.fMaxHour = br.ReadSingle();
                data.fCurrSumMeter = br.ReadSingle();
                data.flag = br.ReadByte();
                data.crc = br.ReadUInt16();
            }
            catch
            {
                data.flag = 0;
            }

            fs.Close();

            if (data.flag == 0)
            {
                return false;   // 자료는 있으나 초기화 되어있는 값이다.
            }

            return true;
        }

        // 시간별 캐시 방식 구현
        private bool LoadHourDataStructAI_Cached(string tag, int year, int month, int day, int hour, HOUR_DATA_ANALOG_STRUCT data)
        {
            if (day < 1 || day > 31) return false;
            if (hour < 0 || hour > 23) return false;

            //251105 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (month < 1 || month > 12) return false;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            if (day < 1 || day > daysInMonth) return false;

            string tag_file = TagUtil.ConvertTagToFile(tag);
            string data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
            string filename = String.Format("{0}\\SUM\\{1:0000}\\MON{2:00}\\AI\\{3}", data_dir, year, month, tag_file);

            DateTime requestTime = new DateTime(year, month, day, hour, 0, 0);

            // 1. 캐시 유효성 검사
            if (!_cacheManager.IsHourDataAICacheValid(filename, requestTime))
            {
                // 2. 캐시가 무효하거나 없으면 파일에서 로드
                if (!_cacheManager.LoadHourDataAIFile(filename, year, month))
                {
                    return false;
                }
            }

            // 3. 캐시에서 데이터 조회
            return _cacheManager.GetHourDataAI(filename, day, hour, data);
        }

        #endregion

        //------------------------------------------------------------------------------
        //	아날로그 시간 데이터를 읽어온다.
        //------------------------------------------------------------------------------

        bool DataGetAiHourElse(int terminal, string tag, ref double val, int year, int month, int day, int hour, EnumDataType data_type)
        {
            // HOUR_DATA_HEAD head; size = 18
            HOUR_DATA_ANALOG_STRUCT data = new HOUR_DATA_ANALOG_STRUCT();

            val = 0.0;

            if (!LoadHourDataStructAI(tag, year, month, day, hour, data, false)) return false;

            //if(data.crc != GetCRC16((BYTE*)&data, sizeof(HOUR_DATA_ANALOG_STRUCT)-2)) {
            //	return 0;
            //}

            if (data_type == EnumDataType.AVE)
            {
                val = data.fAveHour;
            }
            else if (data_type == EnumDataType.SUM)
            {
                val = data.fSumHour;
            }
            else if (data_type == EnumDataType.MIN)
            {
                val = data.fMinHour;
            }
            else if (data_type == EnumDataType.MAX)
            {
                val = data.fMaxHour;
            }
            else
            {
                return false;
            }

            return true;
        }


        // 이것은 원래 인자로 들어와야 하는데 호환성때문에 이렇게 사용했다.
        public static string sMomentDataType = "";
        public static int nMomentSharpSharpValue = 15;

        //bool Get_15MinAve(string tag, int year, int month, int day, int hour, int minute, out double ave, int SharpSharp)
        //{
        //    TREND_AI_STRUCT trend = new TREND_AI_STRUCT();
        //    DataLocal data = new DataLocal();

        //    int count = 0;
        //    bool retn;
        //    double sum = 0;

        //    for (int i = 0, m = minute; i < SharpSharp; i++, m++)
        //    {
        //        retn = data.LoadMinDataStructAI(tag, year, month, day, hour, m, trend);

        //        if (retn)
        //        {
        //            count++;
        //            sum += trend.fAverage;
        //        }
        //    }

        //    ave = 0;

        //    if (count == 0) return false;

        //    ave = sum / count;

        //    return true;
        //}

        // 1시간을 15분 단위로 평균을 해서 그중 최대값을 구한다.
        //bool Get_15MinAve_Max_Hour(string tag, int year, int month, int day, int hour, out double max, int SharpSharp)
        //{
        //    //TREND_AI_STRUCT trend = new TREND_AI_STRUCT();
        //    //DataLocal data = new DataLocal();

        //    int count = 0;
        //    bool retn;
        //    double ave;
        //    max = 0;

        //    for (int minute = 0; minute < 60; minute += SharpSharp)
        //    {
        //        retn = Get_15MinAve(tag, year, month, day, hour, minute, out ave, SharpSharp);

        //        if (retn)
        //        {
        //            count++;
        //            if (count == 1)
        //            {
        //                max = ave;
        //            }
        //            else
        //            {
        //                if (ave > max)
        //                    max = ave;
        //            }
        //        }
        //    }

        //    if (count == 0) return false;

        //    return true;
        //}

        // 하루를 15분 단위로 평균을 해서 그중 최대값을 구한다.
        //bool Get_15MinAve_Max_Day(string tag, int year, int month, int day, out double max, int SharpSharp)
        //{
        //    //TREND_AI_STRUCT trend = new TREND_AI_STRUCT();
        //    //DataLocal data = new DataLocal();

        //    int count = 0;
        //    bool retn;
        //    double ave;
        //    max = 0;

        //    for (int hour = 0; hour < 24; hour++)
        //    {
        //        for (int minute = 0; minute < 60; minute += SharpSharp)
        //        {
        //            retn = Get_15MinAve(tag, year, month, day, hour, minute, out ave, SharpSharp);

        //            if (retn)
        //            {
        //                count++;
        //                if (count == 1)
        //                {
        //                    max = ave;
        //                }
        //                else
        //                {
        //                    if (ave > max)
        //                        max = ave;
        //                }
        //            }
        //        }
        //    }

        //    if (count == 0) return false;

        //    return true;
        //}

        // 한달을 15분 단위로 평균을 해서 그중 최대값을 구한다.  년보에서 사용한다.
        //bool Get_15MinAve_Max_Month(string tag, int year, int month, out double max, int SharpSharp)
        //{
        //    //TREND_AI_STRUCT trend = new TREND_AI_STRUCT();
        //    //DataLocal data = new DataLocal();

        //    int count = 0;
        //    bool retn;
        //    double ave;
        //    max = 0;

        //    for (int day = 1; day <= 31; day++)
        //    {
        //        for (int hour = 0; hour < 24; hour++)
        //        {
        //            for (int minute = 0; minute < 60; minute += SharpSharp)
        //            {
        //                retn = Get_15MinAve(tag, year, month, day, hour, minute, out ave, SharpSharp);

        //                if (retn)
        //                {
        //                    count++;
        //                    if (count == 1)
        //                    {
        //                        max = ave;
        //                    }
        //                    else
        //                    {
        //                        if (ave > max)
        //                            max = ave;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    if (count == 0) return false;

        //    return true;
        //}

        //public bool CatDataGetAiHour(int terminal, string tag, ref double val, int year, int month, int day, int hour, int min, EnumDataType data_type)
        //{
        //    if (data_type == EnumDataType.SUB)
        //    {
        //        double value1 = 0;
        //        double value2 = 0;

        //        if (!DataGetAiHourElse(terminal, tag, ref value1, year, month, day, hour, EnumDataType.MAX))
        //            return false;

        //        TimeUtil.MinusHour(ref year, ref month, ref day, ref hour);

        //        if (!DataGetAiHourElse(terminal, tag, ref value2, year, month, day, hour, EnumDataType.MAX))
        //            return false;

        //        if (value1 < value2)    // 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
        //            val = TagLib.GetTagMemberFull(tag) + value1 - value2;
        //        else
        //            val = value1 - value2;

        //        return true;
        //    }
        //    else if (data_type == EnumDataType.MOMENT)
        //    {
        //        if (sMomentDataType == "##MinuteAve")
        //        {
        //            return Get_15MinAve(tag, year, month, day, hour, min, out val, nMomentSharpSharpValue);
        //        }
        //        else if (sMomentDataType == "##MinuteAve_Max")
        //        {
        //            return Get_15MinAve_Max_Hour(tag, year, month, day, hour, out val, nMomentSharpSharpValue);
        //        }
        //        else
        //        {
        //            TREND_AI_STRUCT trend = new TREND_AI_STRUCT();

        //            val = 0.0;

        //            if (!LoadMinDataStructAI(tag, year, month, day, hour, min, trend))
        //                return false;
        //            val = trend.fCurr;

        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        return DataGetAiHourElse(terminal, tag, ref val, year, month, day, hour, data_type);
        //    }
        //}

        //20251105 PSU 데이터캐싱 누락되어 추가수정.
        bool DataGetAiDayElse(int terminal, string tag, ref double val, int year, int month, int day, EnumDataType data_type)
        {
            val = 0.0;
            //if (day < 1 || day > 31) return false;

            //251105 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (month < 1 || month > 12) return false;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            if (day < 1 || day > daysInMonth) return false;

            // ===== 캐시 사전 로딩 (for문 전에 한 번만) ===== 20251106 PUS 추가.
            string tag_file = TagUtil.ConvertTagToFile(tag);
            string data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
            string filename = String.Format("{0}\\SUM\\{1:0000}\\MON{2:00}\\AI\\{3}", data_dir, year, month, tag_file);

            DateTime requestTime = new DateTime(year, month, day, 0, 0, 0);

            // 1. 캐시 유효성 검사
            if (!_cacheManager.IsHourDataAICacheValid(filename, requestTime))
            {
                // 2. 캐시가 무효하거나 없으면 파일에서 로드
                if (!_cacheManager.LoadHourDataAIFile(filename, year, month))
                {
                    return false;
                }
            }
            // ============================================

            bool read_flag = false;
            int read_count = 0;

            for (int hour = 0; hour < 24; hour++)
            {
                HOUR_DATA_ANALOG_STRUCT data = new HOUR_DATA_ANALOG_STRUCT();

                //if (!LoadHourDataStructAI(tag, year, month, day, hour, data, false))
                //     continue;

                // 이미 캐시가 로드되어 있으므로 빠르게 동작 20251106 PSU 수정.
                if (!_cacheManager.GetHourDataAI(filename, day, hour, data))
                    continue;

                if (data.flag == 0)
                    continue;

                read_flag = true;
                read_count++;

                if (data_type == EnumDataType.AVE)
                {
                    val += data.fAveHour;
                }
                else if (data_type == EnumDataType.SUM)
                {
                    val += data.fSumHour;
                }
                else if (data_type == EnumDataType.MIN)
                {
                    if (read_count == 1)
                        val = data.fMinHour;
                    else if (data.fMinHour < val)
                        val = data.fMinHour;
                }
                else if (data_type == EnumDataType.MAX)
                {
                    if (read_count == 1)
                        val = data.fMaxHour;
                    else if (data.fMaxHour > val)
                        val = data.fMaxHour;
                }
                else
                {
                    return false;
                }
            }

            if (read_flag == false)
            {
                return false;   // 읽은 데이터가 없다.
            }

            if (data_type == EnumDataType.AVE)
            {   // 평균치는 평균값으로 계산한다.
                val = val / read_count;
            }

            return true;
        }

        //public bool CatDataGetAiDay(int terminal, string tag, ref double val, int year, int month, int day, int hour, int min, EnumDataType data_type)
        //{
        //    val = 0.0;

        //    //251016 PSU 날짜 체크 추가. 예외발생없이 처리.
        //    if (month < 1 || month > 12) return false;
        //    int daysInMonth = DateTime.DaysInMonth(year, month);
        //    if (day < 1 || day > daysInMonth) return false;

        //    if (data_type == EnumDataType.SUB)
        //    {
        //        double value1 = 0;
        //        double value2 = 0;

        //        // 하루의 시작을 다르게 설정할 때
        //        if (ConfigViewMain.bReportStartHourOfDayMaxSub)
        //        {
        //            DateTime t = new DateTime(year, month, day);

        //            t = t.AddHours(ConfigViewMain.nReportStartHourOfDay + 23);

        //            if (!DataGetAiHourElse(terminal, tag, ref value1, t.Year, t.Month, t.Day, t.Hour, EnumDataType.MAX))
        //                return false;

        //            t = t.AddHours(-24);

        //            if (!DataGetAiHourElse(terminal, tag, ref value2, t.Year, t.Month, t.Day, t.Hour, EnumDataType.MAX))
        //                return false;
        //        }
        //        else
        //        {
        //            if (!DataGetAiDayElse(terminal, tag, ref value1, year, month, day, EnumDataType.MAX))
        //                return false;
        //            TimeUtil.MinusDay(ref year, ref month, ref day);
        //            if (!DataGetAiDayElse(terminal, tag, ref value2, year, month, day, EnumDataType.MAX))
        //                return false;
        //        }

        //        if (value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
        //            val = TagLib.GetTagMemberFull(tag) + value1 - value2;
        //        else
        //            val = value1 - value2;

        //        return true;
        //    }
        //    else if (data_type == EnumDataType.MOMENT)
        //    {
        //        if (sMomentDataType == "##MinuteAve")
        //        {
        //            return Get_15MinAve(tag, year, month, day, hour, min, out val, nMomentSharpSharpValue);
        //        }
        //        else if (sMomentDataType == "##MinuteAve_Max")
        //        {
        //            return Get_15MinAve_Max_Day(tag, year, month, day, out val, nMomentSharpSharpValue);
        //        }
        //        else
        //        {
        //            TREND_AI_STRUCT trend = new TREND_AI_STRUCT();

        //            val = 0.0;

        //            if (!LoadMinDataStructAI(tag, year, month, day, hour, min, trend))
        //                return false;
        //            val = trend.fCurr;
        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        return DataGetAiDayElse(terminal, tag, ref val, year, month, day, data_type);
        //    }
        //}

        bool CatDataGetAiWeek(int terminal, string tag, ref double val, int year, int week, EnumDataType data_type)
        {
            bool read_flag = false;
            int read_count = 0;
            //int  day;
            double day_value = 0;
            int fr_year;
            int fr_mon;
            int fr_day;
            int i;

            GetDayFromWeek(year, week, out fr_year, out fr_mon, out fr_day);

            val = 0.0;

            for (i = 0; i < 7; i++)
            {   // 한달의 데이터를 모두 읽는다.
                if (DataGetAiDayElse(terminal, tag, ref day_value, fr_year, fr_mon, fr_day, data_type))
                {
                    read_flag = true;
                    read_count++;

                    if (data_type == EnumDataType.AVE)
                    {
                        val += day_value;
                    }
                    else if (data_type == EnumDataType.SUM)
                    {
                        val += day_value;
                    }
                    else if (data_type == EnumDataType.MIN)
                    {
                        if (read_count == 1)
                        {   // 처음으로 읽을때
                            val = day_value;
                        }
                        else
                        {
                            if (day_value < val) val = day_value;
                        }
                    }
                    else if (data_type == EnumDataType.MAX)
                    {
                        if (read_count == 1)
                        {   // 처음으로 읽을때
                            val = day_value;
                        }
                        else
                        {
                            if (day_value > val) val = day_value;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                TimeUtil.PlusDay(ref fr_year, ref fr_mon, ref fr_day);
            }

            if (read_flag == false)
            {
                return false;   // 읽은 데이터가 없다.
            }

            if (data_type == EnumDataType.AVE)
            {   // 평균치는 평균값으로 계산한다.
                val = val / read_count;
            }

            return true;
        }

        bool DataGetAiMonthElse(int terminal, string tag, ref double val, int year, int month, EnumDataType data_type)
        {
            bool read_flag = false;
            int read_count = 0;
            int day;
            //float min = 0;
            //float max = 0;
            double day_value = 0;

            val = 0.0;

            //251016 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (month < 1 || month > 12) return false;
            int daysInMonth = DateTime.DaysInMonth(year, month);

            for (day = 1; day <= daysInMonth; day++) //20251105 PSU, 31->daysInMonth 로 수정.
            {   // 한달의 데이터를 모두 읽는다.
                if (!DataGetAiDayElse(terminal, tag, ref day_value, year, month, day, data_type)) continue;

                read_flag = true;
                read_count++;

                if (data_type == EnumDataType.AVE)
                {
                    val += day_value;
                }
                else if (data_type == EnumDataType.SUM)
                {
                    val += day_value;
                }
                else if (data_type == EnumDataType.MIN)
                {
                    if (read_count == 1)
                    {   // 처음으로 읽을때
                        val = day_value;
                    }
                    else
                    {
                        if (day_value < val) val = day_value;
                    }
                }
                else if (data_type == EnumDataType.MAX)
                {
                    if (read_count == 1)
                    {   // 처음으로 읽을때
                        val = day_value;
                    }
                    else
                    {
                        if (day_value > val) val = day_value;
                    }
                }
                else
                {
                    return false;
                }
            }

            if (read_flag == false)
            {
                return false;   // 읽은 데이터가 없다.
            }

            if (data_type == EnumDataType.AVE)
            {   // 평균치는 평균값으로 계산한다.
                val = val / read_count;
            }

            return true;
        }

        //public bool CatDataGetAiMonth(int terminal, string tag, ref double val, int year, int month, int day, int hour, int min, EnumDataType data_type)
        //{
        //    val = 0.0;

        //    if (data_type == EnumDataType.SUB)
        //    {
        //        double value1 = 0;
        //        double value2 = 0;

        //        if (!DataGetAiMonthElse(terminal, tag, ref value1, year, month, EnumDataType.MAX))
        //            return false;
        //        TimeUtil.MinusMonth(ref year, ref month);
        //        if (!DataGetAiMonthElse(terminal, tag, ref value2, year, month, EnumDataType.MAX))
        //            return false;

        //        if (value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
        //            val = TagLib.GetTagMemberFull(tag) + value1 - value2;
        //        else
        //            val = value1 - value2;

        //        return true;
        //    }
        //    else if (data_type == EnumDataType.MOMENT)
        //    {
        //        if (sMomentDataType == "##MinuteAve")
        //        {
        //            return Get_15MinAve(tag, year, month, day, hour, min, out val, nMomentSharpSharpValue);
        //        }
        //        else if (sMomentDataType == "##MinuteAve_Max")
        //        {
        //            return Get_15MinAve_Max_Month(tag, year, month, out val, nMomentSharpSharpValue);
        //        }
        //        else
        //        {
        //            TREND_AI_STRUCT trend = new TREND_AI_STRUCT();

        //            val = 0.0;

        //            if (!LoadMinDataStructAI(tag, year, month, day, hour, min, trend))
        //                return false;
        //            val = trend.fCurr;
        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        return DataGetAiMonthElse(terminal, tag, ref val, year, month, data_type);
        //    }
        //}


        bool DataGetAiYearElse(int terminal, string tag, ref double val, int year, EnumDataType data_type)
        {
            bool read_flag = false;
            int read_count = 0;
            int month;
            //float min = 0;
            //float max = 0;
            double day_value = 0;

            val = 0.0;

            for (month = 1; month <= 12; month++)
            {   // 한달의 데이터를 모두 읽는다.
                if (!DataGetAiMonthElse(terminal, tag, ref day_value, year, month, data_type)) continue;

                read_flag = true;
                read_count++;

                if (data_type == EnumDataType.AVE)
                {
                    val += day_value;
                }
                else if (data_type == EnumDataType.SUM)
                {
                    val += day_value;
                }
                else if (data_type == EnumDataType.MIN)
                {
                    if (read_count == 1)
                    {   // 처음으로 읽을때
                        val = day_value;
                    }
                    else
                    {
                        if (day_value < val) val = day_value;
                    }
                }
                else if (data_type == EnumDataType.MAX)
                {
                    if (read_count == 1)
                    {   // 처음으로 읽을때
                        val = day_value;
                    }
                    else
                    {
                        if (day_value > val) val = day_value;
                    }
                }
                else
                {
                    return false;
                }
            }

            if (read_flag == false)
            {
                return false;   // 읽은 데이터가 없다.
            }

            if (data_type == EnumDataType.AVE)
            {   // 평균치는 평균값으로 계산한다.
                val = val / read_count;
            }

            return true;
        }

        public bool CatDataGetAiYear(int terminal, string tag, ref double val, int year, int month, int day, int hour, int min, EnumDataType data_type)
        {
            val = 0.0;

            if (data_type == EnumDataType.SUB)
            {
                double value1 = 0;
                double value2 = 0;

                if (!DataGetAiYearElse(terminal, tag, ref value1, year, EnumDataType.MAX))
                    return false;
                TimeUtil.MinusYear(ref year);
                if (!DataGetAiYearElse(terminal, tag, ref value2, year, EnumDataType.MAX))
                    return false;

                if (value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
                    val = TagLib.GetTagMemberFull(tag) + value1 - value2;
                else
                    val = value1 - value2;

                return true;
            }
            else if (data_type == EnumDataType.MOMENT)
            {
                TREND_AI_STRUCT trend = new TREND_AI_STRUCT();

                val = 0.0;

                if (!LoadMinDataStructAI(tag, year, month, day, hour, min, trend))
                    return false;
                val = trend.fCurr;
                return true;
            }
            else
            {
                return DataGetAiYearElse(terminal, tag, ref val, year, data_type);
            }
        }


        #region DI 시간별 데이터 로드 (캐시 적용)

        public bool LoadHourDataStructDI(string tag, DateTime t, HOUR_DATA_DIGITAL_STRUCT data)
        {
            return LoadHourDataStructDI(tag, t.Year, t.Month, t.Day, t.Hour, data, false); // 기본값은 캐시 사용
        }

        // DI 시간별 데이터 로드 메서드 (original 플래그 포함)
        public bool LoadHourDataStructDI(string tag, int year, int month, int day, int hour, HOUR_DATA_DIGITAL_STRUCT data, bool original)
        {
            if (original)
            {
                // 기존 방식 (매번 파일 열기)
                return LoadHourDataStructDI_Original(tag, year, month, day, hour, data);
            }
            else
            {
                // 캐시 방식 (시간별 무효화 적용)
                return LoadHourDataStructDI_Cached(tag, year, month, day, hour, data);
            }
        }

        public bool LoadHourDataStructDI_Original(string tag, int year, int month, int day, int hour, HOUR_DATA_DIGITAL_STRUCT data)
        {
            if (day < 1 || day > 31) return false;
            if (hour < 0 || hour > 23) return false;

            string tag_file;
            string data_dir;
            string filename;
            FileStream fs;

            tag_file = TagUtil.ConvertTagToFile(tag);
            data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
            filename = String.Format("{0}\\SUM\\{1:0000}\\MON{2:00}\\DI\\{3}", data_dir, year, month, tag_file);

            if (!File.Exists(filename)) return false;

            try
            {
                fs = File.OpenRead(filename);
            }
            catch
            {
                fs = null;
            }

            if (fs == null) return false;

            if (fs.Length != 18 + HOUR_DATA_DIGITAL_STRUCT.struct_size * 24 * 31)
            {
                fs.Close();
                return false;
            }

            BinaryReader br = new BinaryReader(fs);

            try
            {
                fs.Seek(18 + ((day - 1) * 24L + hour) * 9, SeekOrigin.Begin);

                data.wCountOnOff = br.ReadUInt16();
                data.dwOnTime = br.ReadUInt32();
                data.flag = br.ReadByte();
                data.crc = br.ReadUInt16();
            }
            catch
            {
                data.flag = 0;
            }

            fs.Close();


            if (data.flag == 0)
            {
                return false;   // 자료는 있으나 초기화 되어있는 값이다.
            }

            return true;
        }


        // DI 시간별 캐시 방식 구현
        private bool LoadHourDataStructDI_Cached(string tag, int year, int month, int day, int hour, HOUR_DATA_DIGITAL_STRUCT data)
        {
            if (day < 1 || day > 31) return false;
            if (hour < 0 || hour > 23) return false;

            //251016 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (month < 1 || month > 12) return false;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            if (day < 1 || day > daysInMonth) return false;

            string tag_file = TagUtil.ConvertTagToFile(tag);
            string data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
            string filename = String.Format("{0}\\SUM\\{1:0000}\\MON{2:00}\\DI\\{3}", data_dir, year, month, tag_file);

            DateTime requestTime = new DateTime(year, month, day, hour, 0, 0);

            // 1. 캐시 유효성 검사
            if (!_cacheManager.IsHourDataDICacheValid(filename, requestTime))
            {
                // 2. 캐시가 무효하거나 없으면 파일에서 로드
                if (!_cacheManager.LoadHourDataDIFile(filename, year, month))
                {
                    return false;
                }
            }

            // 3. 캐시에서 데이터 조회
            return _cacheManager.GetHourDataDI(filename, day, hour, data);
        }
        #endregion

        #region 캐시 관리 메서드

        /// <summary>
        /// 모든 캐시를 정리합니다.
        /// </summary>
        public void ClearAllDataCache()
        {
            _cacheManager.ClearAllCache();
        }

        /// <summary>
        /// 실시간 캐시를 강제로 무효화합니다.
        /// </summary>
        public void InvalidateRealTimeCache()
        {
            _cacheManager.InvalidateRealTimeCache();
        }

        /// <summary>
        /// 특정 파일의 캐시를 제거합니다.
        /// </summary>
        public void RemoveFromCache(string filename)
        {
            _cacheManager.RemoveSpecificCache(filename);
        }



        /// <summary>
        /// 캐시 개수를 조회합니다.
        /// </summary>
        public int GetTotalCacheCount()
        {
            return _cacheManager.GetTotalCacheCount();
        }

        /// <summary>
        /// 추정 메모리 사용량을 조회합니다. (MB 단위)
        /// </summary>
        public double GetEstimatedMemoryUsage()
        {
            return _cacheManager.GetEstimatedMemoryUsage();
        }

        /// <summary>
        /// 자동 캐시 정리 (백그라운드에서 주기적으로 호출)
        /// </summary>
        public void AutoCleanupCache()
        {
            // 1시간 이상 사용되지 않은 캐시 정리
            int removedCount = _cacheManager.CleanupOldCaches(60);

            // 로그 출력 (옵션)
            if (removedCount > 0)
            {
                Debug.WriteLine($"자동 캐시 정리 완료: {removedCount}개 제거");
            }
        }

        #endregion



        //------------------------------------------------------------------------------
        //	아날로그 시간 데이터를 읽어온다.
        //------------------------------------------------------------------------------

        public bool CatDataGetDiHour(int terminal, string tag, ref uint val, int year, int month, int day, int hour, EnumDataType data_type)
        {
            HOUR_DATA_DIGITAL_STRUCT data = new HOUR_DATA_DIGITAL_STRUCT();

            val = 0;

            //251016 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (month < 1 || month > 12)
                return false;

            int daysInMonth = DateTime.DaysInMonth(year, month);
            if (day < 1 || day > daysInMonth)
                return false;

            if (!LoadHourDataStructDI(tag, year, month, day, hour, data, false)) return false;

            if (data_type == EnumDataType.ONTIME)
            {
                val = data.dwOnTime;
            }
            else if (data_type == EnumDataType.OFFTIME)
            {
                val = 3600 - data.dwOnTime;
            }
            else if (data_type == EnumDataType.COUNT)
            {
                val = data.wCountOnOff;
            }
            else
            {
                return false;
            }

            return true;
        }

        //20251106 PSU 수정
        public bool CatDataGetDiDay(int terminal, string tag, ref uint val, int year, int month, int day, EnumDataType data_type)
        {
            val = 0;

            //251016 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (month < 1 || month > 12)
                return false;

            int daysInMonth = DateTime.DaysInMonth(year, month);
            if (day < 1 || day > daysInMonth)
                return false;

            // ===== 캐시 사전 로딩 (for문 전에 한 번만) ===== 20251106 PUS 추가.

            string tag_file = TagUtil.ConvertTagToFile(tag);
            string data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
            string filename = String.Format("{0}\\SUM\\{1:0000}\\MON{2:00}\\DI\\{3}", data_dir, year, month, tag_file);

            DateTime requestTime = new DateTime(year, month, day, 0, 0, 0);

            // 1. 캐시 유효성 검사
            if (!_cacheManager.IsHourDataDICacheValid(filename, requestTime))
            {
                // 2. 캐시가 무효하거나 없으면 파일에서 로드
                if (!_cacheManager.LoadHourDataDIFile(filename, year, month))
                {
                    return false;
                }
            }
            // ============================================

            bool read_flag = false;
            int read_count = 0;

            for (int hour = 0; hour < 24; hour++)
            {
                HOUR_DATA_DIGITAL_STRUCT data = new HOUR_DATA_DIGITAL_STRUCT();

                // 이미 캐시가 로드되어 있으므로 빠르게 동작 20251106 PSU 수정.
                if (!_cacheManager.GetHourDataDI(filename, day, hour, data))
                    continue;

                if (data.flag == 1)
                {
                    read_flag = true;
                    read_count++;

                    if (data_type == EnumDataType.ONTIME)
                    {
                        val += data.dwOnTime;
                    }
                    else if (data_type == EnumDataType.OFFTIME)
                    {
                        val += data.dwOnTime;
                    }
                    else if (data_type == EnumDataType.COUNT)
                    {
                        val += data.wCountOnOff;
                    }
                    else
                    {

                        return false;
                    }
                }
            }

            if (read_flag == false)
            {
                return false;   // 읽은 데이터가 없다.
            }

            if (data_type == EnumDataType.OFFTIME)
            {
                val = (uint)(read_count * 3600 - val);
            }

            return true;
        }

        void GetDayFromWeek(int syear, int week, out int fr_year, out int fr_mon, out int fr_day)
        {
            DateTime dt = new DateTime(syear, 1, 1);

            int weekday = (int)dt.DayOfWeek;

            DateTime dtFr = dt.AddDays(-weekday);

            if (week > 1)
            {
                dtFr = dtFr.AddDays(7 * (week - 1));
            }

            fr_year = dtFr.Year;
            fr_mon = dtFr.Month;
            fr_day = dtFr.Day;
        }

        bool CatDataGetDiWeek(int terminal, string tag, ref uint val, int year, int week, EnumDataType data_type)
        {
            int fr_year;
            int fr_mon;
            int fr_day;

            GetDayFromWeek(year, week, out fr_year, out fr_mon, out fr_day);

            val = 0;

            bool read_flag = false;
            int read_count = 0;
            uint imsi_val = 0;

            for (int i = 0; i < 7; i++)
            {
                if (CatDataGetDiDay(0, tag, ref imsi_val, fr_year, fr_mon, fr_day, data_type))
                {
                    read_flag = true;
                    read_count++;
                    val += imsi_val;
                }
                TimeUtil.PlusDay(ref fr_year, ref fr_mon, ref fr_day);
            }

            if (read_flag == false)
            {
                return false;   // 읽은 데이터가 없다.
            }

            return true;
        }

        // 캐시를 활용한 CatDataGetDiMonth 메서드 개선
        public bool CatDataGetDiMonth(int terminal, string tag, ref uint val, int year, int month, EnumDataType data_type)
        {
            return CatDataGetDiMonth(terminal, tag, ref val, year, month, data_type, false);
        }

        public bool CatDataGetDiMonth(int terminal, string tag, ref uint val, int year, int month,
            EnumDataType data_type, bool original)
        {
            val = 0;

            if (original)
            {
                // 기존 방식 (전체 파일을 한번에 읽어서 루프)
                return CatDataGetDiMonth_Original(terminal, tag, ref val, year, month, data_type);
            }
            else
            {
                // 캐시 방식 (캐시된 데이터에서 월별 데이터 수집)
                bool read_flag = false;
                int read_count = 0;

                // 31일 * 24시간 = 744회 캐시에서 읽기 (기존 방식보다 훨씬 빠름)
                for (int day = 1; day <= 31; day++)
                {
                    for (int hour = 0; hour < 24; hour++)
                    {
                        HOUR_DATA_DIGITAL_STRUCT data = new HOUR_DATA_DIGITAL_STRUCT();

                        // 캐시된 시간별 데이터에서 읽기 (초고속)
                        if (LoadHourDataStructDI(tag, year, month, day, hour, data, false))
                        {
                            if (data.flag == 1)
                            {
                                read_flag = true;
                                read_count++;

                                if (data_type == EnumDataType.ONTIME)
                                {
                                    val += data.dwOnTime;
                                }
                                else if (data_type == EnumDataType.OFFTIME)
                                {
                                    val += data.dwOnTime;
                                }
                                else if (data_type == EnumDataType.COUNT)
                                {
                                    val += data.wCountOnOff;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }

                if (read_flag == false)
                {
                    return false; // 읽은 데이터가 없다.
                }

                if (data_type == EnumDataType.OFFTIME)
                {
                    val = (uint)(read_count * 3600 - val);
                }

                return true;
            }
        }



        public bool CatDataGetDiMonth_Original(int terminal, string tag, ref uint val, int year, int month, EnumDataType data_type)
        {
            string tag_file;
            string data_dir;
            string filename;
            //HOUR_DATA_HEAD head;
            HOUR_DATA_DIGITAL_STRUCT data = new HOUR_DATA_DIGITAL_STRUCT();
            bool read_flag = false;
            int read_count = 0;
            int i;
            FileStream fs;

            val = 0;

            tag_file = TagUtil.ConvertTagToFile(tag);
            data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);
            filename = String.Format("{0}\\SUM\\{1:0000}\\MON{2:00}\\DI\\{3}", data_dir, year, month, tag_file);

            if (!File.Exists(filename)) return false;

            try
            {
                fs = File.OpenRead(filename);
            }
            catch
            {
                fs = null;
            }

            if (fs == null) return false;
            if (fs.Length != 18 + HOUR_DATA_DIGITAL_STRUCT.struct_size * 24 * 31)
            {
                fs.Close();
                return false;
            }

            BinaryReader br = new BinaryReader(fs);

            fs.Seek(18, SeekOrigin.Begin);  // head

            for (i = 0; i < 24 * 31; i++)
            {
                data.wCountOnOff = br.ReadUInt16();
                data.dwOnTime = br.ReadUInt32();
                data.flag = br.ReadByte();
                data.crc = br.ReadUInt16();

                if (data.flag == 1)
                {
                    read_flag = true;
                    read_count++;

                    if (data_type == EnumDataType.ONTIME)
                    {
                        val += data.dwOnTime;
                    }
                    else if (data_type == EnumDataType.OFFTIME)
                    {
                        val += data.dwOnTime;
                    }
                    else if (data_type == EnumDataType.COUNT)
                    {
                        val += data.wCountOnOff;
                    }
                    else
                    {
                        fs.Close();
                        return false;
                    }
                }
            }

            fs.Close();

            if (read_flag == false)
            {
                return false;   // 읽은 데이터가 없다.
            }

            if (data_type == EnumDataType.OFFTIME)
            {
                val = (uint)(read_count * 3600 - val);
            }

            return true;
        }

        public bool CatDataGetDiYear(int terminal, string tag, ref uint val, int year, EnumDataType data_type)
        {
            int month;
            bool read_flag = false;
            uint imsi = 0;

            val = 0;

            for (month = 1; month <= 12; month++)
            {
                if (CatDataGetDiMonth(terminal, tag, ref imsi, year, month, data_type))
                {
                    read_flag = true;
                    val += imsi;        //
                }
            }

            return read_flag;
        }

        public bool CheckUserName(out string err_msg, string username, string passcode, string passcode256)
        {
            string userfile = String.Format("{0}\\Users\\{1}.user", TotalConfig.sDirWorkProject, UserInfoStruct.EncodeUserFilename(username));

            UserInfoStruct info = new UserInfoStruct();

            if (!info.LoadUser(out err_msg, userfile, username)) return false;

            if (ConfigVarTotal.bLocalFlag && info.bUseAutoLockOnPasswordMismatched)
            {
                int nAutoLockMismatchedCount = UserProtectConfig.AutoLockMismatchedCount;

                if (nAutoLockMismatchedCount > 0)
                {
                    if (info.nPasswordMismatchedCount >= nAutoLockMismatchedCount)
                    {

                        if (Tools.IsLangKorean()) // 23-11-20 log 추가 hsjeong
                        {
                            err_msg = String.Format("계정이 중지되었습니다. (암호가 {0}번 이상 틀렸습니다. 관리자가 새 암호를 부여해야 합니다.)", nAutoLockMismatchedCount);
                            Log.Write(LogLevel.ERROR, LogCategory.SECURITY_ACCOUNT_LOCK, "사용자=''{0}'', 계정 중지", username);
                            
                        }
                        else
                        {
                            err_msg = String.Format("Account is Locked. (Password mismatch > {0})", nAutoLockMismatchedCount);
                            Log.Write(LogLevel.ERROR, LogCategory.SECURITY_ACCOUNT_LOCK, "User=''{0}'', Account is locked.", username);
                        }
                        return false;
                    }
                }
            }

            if (passcode256 != null && info.sHashCode256.Length > 0)
            {
                if (info.sHashCode256 == passcode256)
                {
                    info.nPasswordMismatchedCount = 0;
                    info.SaveUser(username);

                    return true;
                }
            }
            else
            {
                if (info.sPassCode == passcode)
                {
                    info.nPasswordMismatchedCount = 0;
                    info.SaveUser(username);

                    return true;
                }
            }

            if (ConfigVarTotal.bLocalFlag && info.bUseAutoLockOnPasswordMismatched)
            {
                int nAutoLockMismatchedCount = UserProtectConfig.AutoLockMismatchedCount;

                if (nAutoLockMismatchedCount > 0)
                {
                    info.nPasswordMismatchedCount++;

                    info.SaveUser(username);

                    if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
                    {
                        if (NetTools.Tools.IsLangKorean())
                            err_msg = "사용자 이름이 존재하지 않거나 암호가 틀립니다.";
                        else if (NetTools.Tools.IsLangChinese())
                            err_msg = "用户名不存在或密码不正确。";
                        else
                            err_msg = "Invalid Username or Password.";
                    }
                    else
                    {
                        if (Tools.IsLangKorean())
                            err_msg = String.Format("암호가 맞지 않습니다. {0}번이상 틀리면 계정이 중지됩니다.", nAutoLockMismatchedCount - info.nPasswordMismatchedCount);
                        else
                            err_msg = String.Format("Password Mismatched. Account will stop if {0} or more times wrong.", nAutoLockMismatchedCount - info.nPasswordMismatchedCount);
                    }

                    return false;
                }
            }

            if (TotalConfig.eOemType == EnumOemType.MBSENGSCADA)
            {
                if (NetTools.Tools.IsLangKorean())
                    err_msg = "사용자 이름이 존재하지 않거나 암호가 틀립니다.";
                else if (NetTools.Tools.IsLangChinese())
                    err_msg = "用户名不存在或密码不正确。";
                else
                    err_msg = "Invalid Username or Password.";
            }
            else
            {
                if (NetTools.Tools.IsLangKorean())
                    err_msg = "암호가 틀립니다.";
                else
                    err_msg = "Password Mismatched.";
            }
            return false;
        }

        public bool DefaultUserCheck(out string err_msg, out string username)
        {
            username = TotalConfigW.GetDefaultUser(TotalConfig.sDirWorkProject);

            string userfile = String.Format("{0}\\Users\\{1}.user", TotalConfig.sDirWorkProject, UserInfoStruct.EncodeUserFilename(username));

            UserInfoStruct info = new UserInfoStruct();

            bool retn = info.LoadUser(out err_msg, userfile, username);

            return retn;
        }

        class FileInfoCompare : IComparer
        {
            int IComparer.Compare(object o1, object o2)
            {
                FileInfo f1 = (FileInfo)o1;
                FileInfo f2 = (FileInfo)o2;

                return String.Compare(f1.Name, f2.Name);
            }
        }

        #region Log & Alarm
        public DataSet GetLogLists()
        {
            string data_dir = TotalConfig.GetProjectDataLogDirectory(TotalConfig.sDirWorkProject);

            string path = String.Format("{0}\\Log", data_dir);

            if (!Directory.Exists(path)) return null;

            DirectoryInfo info = new DirectoryInfo(path);

            DataSet ds = new DataSet();
            DataTable dt = new DataTable("LOG");
            DataColumn dc = new DataColumn("FileName", Type.GetType("System.String"));
            dt.Columns.Add(dc);

            DataRow row;

            FileInfo[] fis;

            fis = info.GetFiles("*.LOG?");
            Array.Sort(fis, new FileInfoCompare());
            foreach (FileInfo fi in fis)
            {
                row = dt.NewRow();
                row[0] = fi.Name;
                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);

            return ds;
        }

        string DecodeLogOneLine(string source)
        {
            NetTools.Hash.HashString hash = new NetTools.Hash.HashString();

            hash.CheckParam(hash.nRootSeed + 1, hash.nRootSeed - 2, hash.nRootSeed * 3, hash.nRootSeed / 4, hash.nRootSeed ^ 5);

            string target = hash.Decode(source, "LOG");

            return target;
        }

        DataSet GetLogFileOemSbas(string log_name)
        {
            string data_dir = TotalConfig.GetProjectDataLogDirectory(TotalConfig.sDirWorkProject);

            string path = String.Format("{0}\\LOG\\{1}", data_dir, log_name);

            if (!File.Exists(path)) return null;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable("LOG");
            DataColumn dc = new DataColumn("Description", Type.GetType("System.String"));
            dc.MaxLength = -1;// 256;
            dt.Columns.Add(dc);

            DataRow row;
            string one_line = "";

            BinaryReader reader;

            string ext = Path.GetExtension(path);

            FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            reader = new BinaryReader(fs);

            int ch;
            StringBuilder sb = new StringBuilder();

            while (true)
            {
                ch = reader.Read();

                if (ch == -1) break;

                if (ch == 1)
                {
                    row = dt.NewRow();

                    one_line = DecodeLogOneLine(sb.ToString());

                    row[0] = one_line;

                    dt.Rows.Add(row);

                    sb.Remove(0, sb.Length);
                }
                else
                {
                    sb.Append((char)(ch + 46));
                }
            }

            reader.Close();
            ds.Tables.Add(dt);

            return ds;
        }

        public DataSet GetLogFile(string log_name)
        {
            if (TotalConfig.eOemType == EnumOemType.SBAS)
            {
                return GetLogFileOemSbas(log_name);
            }

            string data_dir = TotalConfig.GetProjectDataLogDirectory(TotalConfig.sDirWorkProject);

            string path = String.Format("{0}\\LOG\\{1}", data_dir, log_name);

            if (!File.Exists(path)) return null;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable("LOG");
            DataColumn dc = new DataColumn("Description", Type.GetType("System.String"));
            dc.MaxLength = -1;// 256;
            dt.Columns.Add(dc);

            DataRow row;
            string one_line = "";

            TextReader reader;

            string ext = Path.GetExtension(path);

            if (String.Compare(ext, ".logx", true) == 0)
                reader = new StreamReader(path);
            else
                reader = new StreamReader(path, System.Text.Encoding.Default);

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;

                row = dt.NewRow();
                if (TotalConfig.eOemType == EnumOemType.SBAS)
                {
                    //one_line = DecodeLogOneLine(one_line);
                }

                row[0] = one_line;

                dt.Rows.Add(row);
            }

            reader.Close();
            ds.Tables.Add(dt);

            return ds;
        }

        public DataSet GetAlarmLists()
        {
            string data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);

            string path = String.Format("{0}\\ALARM", data_dir);

            if (!Directory.Exists(path)) return null;

            DirectoryInfo info = new DirectoryInfo(path);

            DataSet ds = new DataSet();
            DataTable dt = new DataTable("ALARM");
            DataColumn dc = new DataColumn("FileName", Type.GetType("System.String"));
            dt.Columns.Add(dc);
            dc = new DataColumn("AlarmCount", Type.GetType("System.Int32"));
            dt.Columns.Add(dc);

            DataRow row;

            FileInfo[] fis;

            fis = info.GetFiles("*.AL3");
            Array.Sort(fis, new FileInfoCompare());

            foreach (FileInfo fi in fis)
            {
                row = dt.NewRow();
                row[0] = fi.Name;
                row[1] = fi.Length / ALARM_FILE_STRUCT.struct_size;
                dt.Rows.Add(row);
            }

            fis = info.GetFiles("*.ALMX");
            Array.Sort(fis, new FileInfoCompare());
            foreach (FileInfo fi in fis)
            {
                row = dt.NewRow();
                row[0] = fi.Name;
                row[1] = Tools.GetLineHap(fi.FullName);
                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);

            return ds;
        }

        void MakeAlarmHeader(DataTable dt)
        {
            DataColumn dc;
            dc = new DataColumn("alarm_datetime", Type.GetType("System.DateTime"));
            dt.Columns.Add(dc);
            dc = new DataColumn("tag", Type.GetType("System.String"));
            dc.MaxLength = -1;  // 40 40글자자 넘는 경우가 있어서 크기 제한을 두지 않는다. 10.1.1 
            dt.Columns.Add(dc);
            dc = new DataColumn("description", Type.GetType("System.String"));
            dc.MaxLength = -1;  // 80;
            dt.Columns.Add(dc);
            dc = new DataColumn("message", Type.GetType("System.String"));
            dc.MaxLength = -1;// 80;
            dt.Columns.Add(dc);
            dc = new DataColumn("alarm_type", Type.GetType("System.UInt16"));
            dt.Columns.Add(dc);
            dc = new DataColumn("priority", Type.GetType("System.UInt16"));
            dt.Columns.Add(dc);
            dc = new DataColumn("port", Type.GetType("System.UInt16"));
            dt.Columns.Add(dc);
            dc = new DataColumn("station", Type.GetType("System.UInt16"));
            dt.Columns.Add(dc);
            dc = new DataColumn("address", Type.GetType("System.UInt32"));
            dt.Columns.Add(dc);
            dc = new DataColumn("type", Type.GetType("System.UInt16"));
            dt.Columns.Add(dc);

            // 10.2.1 부터 추가되었다.
            dc = new DataColumn("user", Type.GetType("System.String"));
            dt.Columns.Add(dc);
            dc = new DataColumn("ip", Type.GetType("System.String"));
            dt.Columns.Add(dc);
            dc = new DataColumn("computer", Type.GetType("System.String"));
            dt.Columns.Add(dc);
        }

        bool IsBoolFilterInclude(bool[] filter, int index)
        {
            if (index < 0) return false;        // range over
            if (index >= filter.Length) return false;   // range over

            return filter[index];
        }

        void AddAlarmFile(DataTable dt, string path, Regex filterTag, bool[] filterType, bool[] filterPort, bool check_time, DateTime tFrom, DateTime tTo)
        {
            DataRow row;

            string ext = Path.GetExtension(path);

            if (String.Compare(ext, ".almx", true) == 0)	// 텍스트 방식의 파일
            {
                TextReader reader = new StreamReader(path);
                CommaBlockString comma = new CommaBlockString();
                string one_line;
                string imsi = "";
                ushort uval16 = 0;
                uint uval32 = 0;
                DateTime t;

                while (true)
                {
                    one_line = reader.ReadLine();
                    if (one_line == null) break;

                    comma.Set(one_line);

                    t = comma.GetDateTime();

                    // 시간도 필터 범위에 포함한다.
                    if (check_time)
                    {
                        if (t < tFrom || t > tTo) continue;
                    }

                    row = dt.NewRow();

                    row[0] = t;

                    comma.GetString(ref imsi);	// tag
                    if (filterTag != null && !filterTag.IsMatch(imsi)) continue;    // 태그 필터에 걸리지 않는다.
                    row[1] = imsi;

                    comma.GetString(ref imsi);	// description
                    row[2] = imsi;
                    comma.GetString(ref imsi);	// msg
                    row[3] = imsi;
                    comma.GetWORD(ref uval16);	// alarm_type

                    if (filterType != null && !IsBoolFilterInclude(filterType, uval16)) continue;    // 필터에 포함되지 않는다.                    

                    row[4] = uval16;
                    comma.GetWORD(ref uval16);	// priority
                    row[5] = uval16;
                    comma.GetWORD(ref uval16);	// port

                    if (filterPort != null && !IsBoolFilterInclude(filterPort, uval16)) continue;    // 필터에 포함되지 않는다.                    

                    row[6] = uval16;
                    comma.GetWORD(ref uval16);	// station
                    row[7] = uval16;
                    comma.GetDWORD(ref uval32);	// address
                    row[8] = uval32;
                    comma.GetWORD(ref uval16);	// alarm_sub_type
                    row[9] = uval16;

                    comma.GetString(ref imsi);  // user
                    row[10] = imsi;
                    comma.GetString(ref imsi);  // ip
                    row[11] = imsi;
                    comma.GetString(ref imsi);  // computer
                    row[12] = imsi;

                    dt.Rows.Add(row);
                }

                reader.Close();
            }
            else // 옛날 방식의 구조체 방식
            {
                ALARM_FILE_STRUCT alarm = new ALARM_FILE_STRUCT();
                FileStream fs = File.OpenRead(path);
                BinaryReader reader = new BinaryReader(fs);
                DateTime t;

                int alarm_count = (int)(fs.Length / ALARM_FILE_STRUCT.struct_size);

                for (int i = 0; i < alarm_count; i++)
                {
                    alarm.LoadFromFile(reader);

                    t = alarm.t.ToDateTime();

                    // 시간도 필터 범위에 포함한다.
                    if (check_time)
                    {
                        if (t < tFrom || t > tTo) continue;
                    }

                    if (filterTag != null && !filterTag.IsMatch(alarm.tag)) continue;                   // 태그 필터에 걸리지 않는다.
                    if (filterType != null && !IsBoolFilterInclude(filterType, alarm.alarm_type)) continue;    // 필터에 포함되지 않는다.                    
                    if (filterPort != null && !IsBoolFilterInclude(filterPort, alarm.port)) continue;    // 필터에 포함되지 않는다.                    

                    row = dt.NewRow();
                    //row[0] = one_line;
                    row[0] = t;
                    row[1] = alarm.tag;
                    row[2] = alarm.description;
                    row[3] = alarm.msg;
                    row[4] = alarm.alarm_type;
                    row[5] = alarm.priority;
                    row[6] = alarm.port;
                    row[7] = alarm.station;
                    row[8] = alarm.address;
                    row[9] = alarm.alarm_sub_type;
                    dt.Rows.Add(row);
                }

                fs.Close();
            }
        }

        public DataSet GetAlarmFile(string alarm_file)
        {
            string data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);

            string path = String.Format("{0}\\ALARM\\{1}", data_dir, alarm_file);

            // 2007.10.29 추가. 주어진 파일이 없으면 다음 버전이나 이전버전을 읽는다.
            if (!File.Exists(path))
            {
                string ext = Path.GetExtension(alarm_file);

                if (String.Compare(ext, ".ALMX", true) == 0)
                {
                    path = String.Format("{0}\\ALARM\\{1}.AL3", data_dir, Path.GetFileNameWithoutExtension(alarm_file));
                }
                else
                {
                    path = String.Format("{0}\\ALARM\\{1}.ALMX", data_dir, Path.GetFileNameWithoutExtension(alarm_file));
                }

                if (!File.Exists(path)) return null;
            }

            DataSet ds = new DataSet();
            DataTable dt = new DataTable("ALARM");

            MakeAlarmHeader(dt);
            AddAlarmFile(dt, path, null, null, null, false, DateTime.Now, DateTime.Now);

            ds.Tables.Add(dt);

            return ds;
        }

        bool IsFileInclude(string filename, int dayhapfr, int dayhapto)
        {
            string name = Path.GetFileNameWithoutExtension(filename);
            if (name.Length != 8) return false;// 날짜형 이름이 아니다.

            int y = ConvertTool.ToInt32(name.Substring(0, 4));
            int m = ConvertTool.ToInt32(name.Substring(4, 2));
            int d = ConvertTool.ToInt32(name.Substring(6, 2));

            int hap = (int)TimeUtil.GetDayHap(y, m, d);

            if (hap < dayhapfr) return false;
            if (hap > dayhapto) return false;

            return true;
        }

        public DataSet GetAlarmFileByScript(DateTime tFrom, DateTime tTo, string option, bool checktime)
        {
            string data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);

            string path = String.Format("{0}\\ALARM", data_dir);

            if (!Directory.Exists(path)) return null;

            CommaTextReader comma = new CommaTextReader();
            string buf = "";
            comma.Set(option);
            Regex filterTag = null;
            bool[] filterPort = null;
            bool[] filterType = null;

            while (!comma.IsEOS())
            {
                comma.GetString(ref buf);

                if (String.Compare(buf, 0, "Tag=", 0, 4) == 0)
                {
                    try
                    {
                        filterTag = new Regex(buf.Substring(4), RegexOptions.IgnoreCase);
                    }
                    catch
                    {
                        filterTag = null;
                    }
                }
                else if (String.Compare(buf, 0, "Port=", 0, 5) == 0)
                {
                    filterPort = new bool[256];

                    CommaBlockString c2 = new CommaBlockString();
                    c2.Set(buf.Substring(5));
                    string sport = "";
                    while (!c2.IsEOS())
                    {
                        c2.GetString(ref sport);
                        if (sport.Length == 0) continue;

                        int port = ConvertTool.ToInt32(sport);
                        if (port >= 0 && port <= 255)
                        {
                            filterPort[port] = true;
                        }
                    }
                }
                else if (String.Compare(buf, 0, "Type=", 0, 5) == 0)
                {
                    filterType = new bool[AlarmClass.MAX_ALARM_MSG_TYPE];

                    CommaBlockString c2 = new CommaBlockString();
                    c2.Set(buf.Substring(5));
                    string stype = "";
                    while (!c2.IsEOS())
                    {
                        c2.GetString(ref stype);
                        if (stype.Length == 0) continue;

                        int type = ConvertTool.ToInt32(stype);
                        if (type >= 0 && type < AlarmClass.MAX_ALARM_MSG_TYPE)
                        {
                            filterType[type] = true;
                        }
                    }
                }
            }

            DataSet ds = new DataSet();
            DataTable dt = new DataTable("ALARM");

            MakeAlarmHeader(dt);

            DirectoryInfo info = new DirectoryInfo(path);

            int dayhapfr = (int)TimeUtil.GetDayHap(tFrom.Year, tFrom.Month, tFrom.Day);
            int dayhapto = (int)TimeUtil.GetDayHap(tTo.Year, tTo.Month, tTo.Day);

            foreach (FileInfo fi in info.GetFiles("*.AL3"))
            {
                if (IsFileInclude(fi.Name, dayhapfr, dayhapto))
                    AddAlarmFile(dt, fi.FullName, filterTag, filterType, filterPort, checktime, tFrom, tTo);
            }

            foreach (FileInfo fi in info.GetFiles("*.ALMX"))
            {
                if (IsFileInclude(fi.Name, dayhapfr, dayhapto))
                    AddAlarmFile(dt, fi.FullName, filterTag, filterType, filterPort, checktime, tFrom, tTo);
            }

            ds.Tables.Add(dt);

            return ds;
        }

        #endregion Log & Alarm
        public DataSet GetOnOffList(int year, int mon)
        {
            TextReader reader = null;

            string path = String.Format("{0}\\database\\{1:0000}{2:00}.datx", TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject), year, mon);
            if (File.Exists(path))
            {
                reader = new StreamReader(path);
            }
            else
            {
                path = String.Format("{0}\\database\\{1:0000}{2:00}.dat", TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject), year, mon);
                if (File.Exists(path))
                {
                    reader = new StreamReader(path, System.Text.Encoding.Default);
                }
            }

            if (reader == null) return null;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable("OnOffList");
            DataColumn dc;

            dc = new DataColumn("tag", Type.GetType("System.String"));
            dc.MaxLength = -1;// 80;
            dt.Columns.Add(dc);

            dc = new DataColumn("start_time", Type.GetType("System.String"));
            dc.MaxLength = 11;
            dt.Columns.Add(dc);
            dc = new DataColumn("start_date", Type.GetType("System.String"));
            dc.MaxLength = 11;
            dt.Columns.Add(dc);

            dc = new DataColumn("end_time", Type.GetType("System.String"));
            dc.MaxLength = 11;
            dt.Columns.Add(dc);
            dc = new DataColumn("end_date", Type.GetType("System.String"));
            dc.MaxLength = 11;
            dt.Columns.Add(dc);

            dc = new DataColumn("oper_time", Type.GetType("System.String"));
            dc.MaxLength = 15;
            dt.Columns.Add(dc);

            DataRow row;

            string one_line;

            ArrayList array = new ArrayList();
            CommaBlockString comma = new CommaBlockString();
            string imsi = "";

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;

                array.Clear();
                comma.Set(one_line);
                while (true)
                {
                    if (comma.IsEOS()) break;
                    comma.GetString(ref imsi);
                    array.Add(imsi);
                }

                if (array.Count == 0) continue;

                for (int c = dt.Columns.Count; c < array.Count; c++)
                {
                    dc = new DataColumn("Item" + c, Type.GetType("System.String"));
                    dc.MaxLength = -1;// 256;
                    dt.Columns.Add(dc);
                }

                row = dt.NewRow();

                for (int i = 0; i < array.Count; i++)
                {
                    row[i] = array[i];
                }
                dt.Rows.Add(row);
            }

            reader.Close();

            ds.Tables.Add(dt);

            return ds;
        }

        public DataSet GetDataSetFromMdb(string filename, string command, out string error)
        {
            if (!File.Exists(filename))
            {
                error = "파일을 찾을 수 없습니다.";
                return null;
            }

            OleDbConnection conn = new OleDbConnection();

            conn.ConnectionString = String.Format("Provider=Microsoft.JET.OLEDB.4.0;Data Source={0};", filename);

            try
            {
                conn.Open();
            }
            catch (Exception exception)
            {
                error = String.Format("GetDataSetFromMdb Connection.Open() error\n{0}", exception.Message);
                return null;
            }

            OleDbDataAdapter adapter = new OleDbDataAdapter(command, conn);

            DataSet ds = new DataSet();

            try
            {
                adapter.Fill(ds, "TableTest");
            }
            catch
            {
            }
            finally
            {
                conn.Close();
            }

            error = "";
            return ds;
        }

        public DataSet GetDataSetFromDsn(string dsn, string command, out string error)
        {
            error = "";
            ConnectionString con_str;

            con_str = DbTool.GetConnectionString(dsn);

            if (con_str == null)
            {
                if (Tools.IsLangKorean())
                    error = String.Format("{0} 목록을 찾을 수 없습니다.", dsn);
                else
                    error = String.Format("Can't find {0} list at connection string", dsn);
                return null;
            }

            CommonDbConnection conn = new CommonDbConnection(con_str.dbConnectionType, con_str.dsn, con_str.bAddCommitAfterCommand);
            //conn.ConnectionString = con_str.dsn;

            try
            {
                conn.Open();
            }
            catch (Exception exception)
            {
                error = String.Format("Connection.Open() error\n{0}", exception.Message);
                return null;
            }
            CommonDbDataAdapter adapter = new CommonDbDataAdapter(command, conn);

            DataSet ds = new DataSet();

            try
            {
                adapter.Fill(ds, "TableTest");
            }
            catch (Exception exception)
            {
                error = String.Format("adapter.Fill Error\nCommand={0}\nError Message={1}", command, exception.Message);
                ds = null;
            }
            finally
            {
                conn.Close();
            }

            return ds;
        }

        public bool DataSetCommand(string dsn, string command, out string error)
        {
            ConnectionString con_str;

            con_str = DbTool.GetConnectionString(dsn);

            if (con_str == null)
            {
                if (Tools.IsLangKorean())
                    error = String.Format("{0} 목록을 찾을 수 없습니다.", dsn);
                else
                    error = String.Format("Can't find {0} list at connection string", dsn);

                return false;
            }

            CommonDbConnection conn = new CommonDbConnection(con_str.dbConnectionType, con_str.dsn, con_str.bAddCommitAfterCommand);

            //conn.ConnectionString = con_str.dsn;

            try
            {
                conn.Open();
            }
            catch (Exception exception)
            {
                error = exception.Message;
                return false;
            }

            CommonDbCommand dbcom = new CommonDbCommand(command, conn);

            CommonDbTransaction transaction = new CommonDbTransaction();
            if (conn.bAddCommitAfterCommand)
            {
                transaction.BeginTransaction(conn);
                dbcom.Transaction = transaction;
            }

            try
            {
                dbcom.ExecuteNonQuery();

                if (conn.bAddCommitAfterCommand)
                {
                    transaction.Commit();
                }
            }
            catch (Exception exception)
            {
                if (conn.bAddCommitAfterCommand)
                {
                    transaction.Rollback();
                }

                conn.Close();
                error = exception.Message;
                return false;
            }

            conn.Close();

            error = "";
            return true;
        }

        public DataSet GetDataSetFromOdbc(string dsn, string command, out string error)
        {
            OdbcConnection conn = new OdbcConnection();

            conn.ConnectionString = dsn;

            try
            {
                conn.Open();
            }
            catch (Exception exception)
            {
                if (Tools.IsLangKorean())
                    error = String.Format("{0} 에 연결할 수 없습니다.\nError={1}", dsn, exception.Message);
                else
                    error = String.Format("Can't connect to {0}\nError={1}", dsn, exception.Message);

                return null;
            }

            OdbcDataAdapter adapter = new OdbcDataAdapter(command, conn);

            DataSet ds = new DataSet();

            try
            {
                adapter.Fill(ds, "TableTest");
            }
            catch (Exception exception)
            {
                conn.Close();
                error = String.Format("Query Error={0}", exception.Message);
                return null;
            }

            conn.Close();

            error = "";
            return ds;
        }

        public bool DataSetOdbcCommand(string dsn, string command, out string error)
        {
            OdbcConnection conn = new OdbcConnection();

            conn.ConnectionString = dsn;

            try
            {
                conn.Open();
            }
            catch (Exception exception)
            {
                if (Tools.IsLangKorean())
                    error = String.Format("{0} 에 연결할 수 없습니다.\nError={1}", dsn, exception.Message);
                else
                    error = String.Format("Can't connect to {0}\nError={1}", dsn, exception.Message);

                return false;
            }

            OdbcCommand dbcom = new OdbcCommand(command, conn);

            try
            {
                dbcom.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                error = exception.Message;
                conn.Close();
                return false;
            }

            conn.Close();

            error = "";
            return true;
        }
    }
}

