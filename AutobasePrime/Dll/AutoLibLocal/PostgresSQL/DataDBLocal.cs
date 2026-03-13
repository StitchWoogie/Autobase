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
using Npgsql;

namespace AutoLibLocal
{
     /// <summary>
    /// PostgreSQL DB를 사용하는 DataLocal 클래스 (캐시 시스템 포함)
    /// </summary>
    public class DataLocal
    {

        private readonly DataPostgres _db;
        private static DBDataCacheManager _cacheManager = new DBDataCacheManager();

        public DataLocal()
        {
            //
            // TODO: Add constructor logic here
            //
            _db = DataPostgres.Instance;
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



        #region AI 분별 데이터 로드 (캐시 적용)
        public async Task<bool> LoadMinDataStructAI(string tag, DateTime t, TREND_AI_STRUCT data)
        {
            return await LoadMinDataStructAI(tag, t.Year, t.Month, t.Day, t.Hour, t.Minute, data).ConfigureAwait(false);
        }

        public async Task<bool> LoadMinDataStructAI(string tag, int year, int mon, int day, int hour, int min, TREND_AI_STRUCT data)
        {
            if (day < 1 || day > 31) return false;
            if (hour < 0 || hour > 23) return false;
            if (min < 0 || min > 59) return false;

            return await _cacheManager.GetMinDataAI(tag, year, mon, day, hour, min, data).ConfigureAwait(false);
        }

        #endregion


        #region DI 분별 데이터 로드 (캐시 적용)
        public async Task<bool> LoadMinDataStructDI(string tag, DateTime t, TREND_DI_STRUCT data)
        {
            return await LoadMinDataStructDI(tag, t.Year, t.Month, t.Day, t.Hour, t.Minute, data).ConfigureAwait(false);
        }

        // DI 캐시 방식 구현
        public async Task<bool> LoadMinDataStructDI(string tag, int year, int mon, int day, int hour, int min, TREND_DI_STRUCT data)
        {
            if (day < 1 || day > 31) return false;
            if (hour < 0 || hour > 23) return false;
            if (min < 0 || min > 59) return false;

            return await _cacheManager.GetMinDataDI(tag, year, mon, day, hour, min, data).ConfigureAwait(false); 
        }

        #endregion


        #region AI 시간별 데이터 로드 (캐시 적용)
        // 시간별 데이터 로드 메서드 (original 플래그 포함)
        public async Task<bool> LoadHourDataStructAI(string tag, DateTime t, HOUR_DATA_ANALOG_STRUCT data)
        {
            return  await LoadHourDataStructAI(tag, t.Year, t.Month, t.Day, t.Hour, data);
        }

        // 시간별 캐시 방식 구현
        private async Task<bool> LoadHourDataStructAI(string tag, int year, int month, int day, int hour, HOUR_DATA_ANALOG_STRUCT data)
        {
            if (day < 1 || day > 31) return false;
            if (hour < 0 || hour > 23) return false;

            //251016 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (month < 1 || month > 12) return false;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            if (day < 1 || day > daysInMonth) return false;

            return await _cacheManager.GetHourDataAI(tag, year, month, day, hour, data).ConfigureAwait(false);
        }
        #endregion



        #region DI 시간별 데이터 로드 (캐시 적용)

        public async Task<bool> LoadHourDataStructDI(string tag, DateTime t, HOUR_DATA_DIGITAL_STRUCT data)
        {
            return await LoadHourDataStructDI(tag, t.Year, t.Month, t.Day, t.Hour, data).ConfigureAwait(false);
        }

        // DI 시간별 캐시 방식 구현
        private async Task<bool> LoadHourDataStructDI(string tag, int year, int month, int day, int hour, HOUR_DATA_DIGITAL_STRUCT data)
        {
            if (day < 1 || day > 31) return false;
            if (hour < 0 || hour > 23) return false;

            //251016 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (month < 1 || month > 12) return false;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            if (day < 1 || day > daysInMonth) return false;

            return await _cacheManager.GetHourDataDI(tag, year, month, day, hour, data).ConfigureAwait(false);
        }
        #endregion


        #region AI 데이터 조회 메서드


        async Task<(bool, double val)> CatDataGetAi(string tag,  EnumDataType data_type, EnumDataTime data_time, int year, int mon, int day, int hour, int min)
        {
            bool retn;
            double val = 0;

            //251016 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (mon < 1 || mon > 12) return (false, val);
            int daysInMonth = DateTime.DaysInMonth(year, mon);
            if (day < 1 || day > daysInMonth) return (false, val);

            if (data_time == EnumDataTime.Minute)
                (retn, val) = await CatDataGetAiMin(0, tag, year, mon, day, hour, min, data_type).ConfigureAwait(false);
            else if (data_time == EnumDataTime.Hour)
                (retn, val) = await CatDataGetAiHour(0, tag, year, mon, day, hour, min, data_type).ConfigureAwait(false);
            else if (data_time == EnumDataTime.Day)
                (retn, val) = await CatDataGetAiDay(0, tag, year, mon, day, hour, min, data_type).ConfigureAwait(false);
            else if (data_time == EnumDataTime.Week)
                (retn, val) = await CatDataGetAiWeek(0, tag,  year, mon, data_type).ConfigureAwait(false);
            else if (data_time == EnumDataTime.Month)
                (retn, val) = await CatDataGetAiMonth(0, tag, year, mon, day, hour, min, data_type).ConfigureAwait(false);
            else if (data_time == EnumDataTime.Year)    // 2023-4-11 추가
                (retn, val) = await CatDataGetAiYear(0, tag, year, mon, day, hour, min, data_type).ConfigureAwait(false);
            else
                retn = false;

            if (!retn) val = 0;

            return (retn, val);
        }

        public async Task< DataSet> GetDataAi(string tag, EnumDataType value_type, EnumDataTime data_time, int year, int mon, int day, int hour, int min, int data_count, int data_gab)
        {
            //if (data_gab > 1)
            //FitTimeToDataGab(data_time, ref year, ref mon, ref day, ref hour, ref min, data_gab);

            DataSet ds = new DataSet();
            DataTable dt;

            dt = new DataTable(tag);

            DataColumn dc;
            DataRow row;
            double val = 0;
            bool retn;

            dc = new DataColumn("Flag", Type.GetType("System.SByte"));
            dt.Columns.Add(dc);

            if ((value_type & EnumDataType.AVE) == EnumDataType.AVE)
            {
                dc = new DataColumn("AVE");
                dt.Columns.Add(dc);
            }

            if ((value_type & EnumDataType.MAX) == EnumDataType.MAX)
            {
                dc = new DataColumn("MAX");
                dt.Columns.Add(dc);
            }

            if ((value_type & EnumDataType.MIN) == EnumDataType.MIN)
            {
                dc = new DataColumn("MIN");
                dt.Columns.Add(dc);
            }

            if ((value_type & EnumDataType.MOMENT) == EnumDataType.MOMENT)
            {
                dc = new DataColumn("MOMENT");
                dt.Columns.Add(dc);
            }

            if ((value_type & EnumDataType.SUB) == EnumDataType.SUB)
            {
                dc = new DataColumn("SUB");
                dt.Columns.Add(dc);
            }

            if ((value_type & EnumDataType.SUM) == EnumDataType.SUM)
            {
                dc = new DataColumn("SUM");
                dt.Columns.Add(dc);
            }

            for (int i = 0; i < data_count; i++)
            {
                row = dt.NewRow();

                retn = false;

                if ((value_type & EnumDataType.AVE) == EnumDataType.AVE)
                {
                    (retn, val) = await CatDataGetAi(tag, EnumDataType.AVE, data_time, year, mon, day, hour, min).ConfigureAwait(false);
                    row["AVE"] = val.ToString();
                }
                if ((value_type & EnumDataType.MAX) == EnumDataType.MAX)
                {
                    (retn, val) = await CatDataGetAi(tag, EnumDataType.MAX, data_time, year, mon, day, hour, min).ConfigureAwait(false);
                    row["MAX"] = val.ToString();
                }
                if ((value_type & EnumDataType.MIN) == EnumDataType.MIN)
                {
                    (retn, val) = await CatDataGetAi(tag,EnumDataType.MIN, data_time, year, mon, day, hour, min).ConfigureAwait(false);
                    row["MIN"] = val.ToString();
                }
                if ((value_type & EnumDataType.MOMENT) == EnumDataType.MOMENT)
                {
                    (retn, val) = await CatDataGetAi(tag, EnumDataType.MOMENT, data_time, year, mon, day, hour, min).ConfigureAwait(false);
                    row["MOMENT"] = val.ToString();
                }
                if ((value_type & EnumDataType.SUB) == EnumDataType.SUB)
                {
                    (retn, val) = await  CatDataGetAi(tag, EnumDataType.SUB, data_time, year, mon, day, hour, min).ConfigureAwait(false);
                    row["SUB"] = val.ToString();
                }
                if ((value_type & EnumDataType.SUM) == EnumDataType.SUM)
                {
                    (retn, val) = await CatDataGetAi(tag,  EnumDataType.SUM, data_time, year, mon, day, hour, min).ConfigureAwait(false);
                    row["SUM"] = val.ToString();
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


        public async Task<(bool, double val)> CatDataGetAiMin(int terminal, string tag,  int year, int month, int day, int hour, int min, EnumDataType data_type)
        {
            TREND_AI_STRUCT trend = new TREND_AI_STRUCT();

            double val = 0.0;

            //251016 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (month < 1 || month > 12) return (false, val);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            if (day < 1 || day > daysInMonth) return (false, val);

            if (!await LoadMinDataStructAI(tag, year, month, day, hour, min, trend).ConfigureAwait(false))
            {
                return (false, val);
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
                if (!await LoadMinDataStructAI(tag, year, month, day, hour, min, trend).ConfigureAwait(false))
                {
                    return ( false, val);
                }

                if (value1 < trend.fMax)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
                    val = TagLib.GetTagMemberFull(tag) + value1 - trend.fMax;
                else
                    val = value1 - trend.fMax;
            }
            else
            {
                return (false, val);
            }

            return (true, val);
        }




        /// <summary>
        /// 아날로그 시간 데이터를 읽어온다.
        /// </summary>
        async Task<( bool, double val)> DataGetAiHourElse(int terminal, string tag, int year, int month, int day, int hour, EnumDataType data_type)
        {
            // HOUR_DATA_HEAD head; size = 18
            HOUR_DATA_ANALOG_STRUCT data = new HOUR_DATA_ANALOG_STRUCT();

            double val = 0.0;

            if (!await LoadHourDataStructAI(tag, year, month, day, hour, data).ConfigureAwait(false)) return (false, val);

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
                return (false, val);
            }

            return (true, val);
        }

        public async Task<(bool, double val)> CatDataGetAiHour(int terminal, string tag, int year, int month, int day, int hour, int min, EnumDataType data_type)
        {
            double val = 0;
            bool retn = false;

            //251016 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (month < 1 || month > 12) return (false, val);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            if (day < 1 || day > daysInMonth) return (false, val);

            if (data_type == EnumDataType.SUB)
            {
                double value1 = 0;
                double value2 = 0;

                (retn, value1) = await DataGetAiHourElse(terminal, tag, year, month, day, hour, EnumDataType.MAX).ConfigureAwait(false);
                if (!retn)
                    return (false, val);

                TimeUtil.MinusHour(ref year, ref month, ref day, ref hour);

                (retn, value2) = await DataGetAiHourElse(terminal, tag, year, month, day, hour, EnumDataType.MAX).ConfigureAwait(false);
                if (!retn)
                    return (false, val);

                if (value1 < value2)    // 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
                    val = TagLib.GetTagMemberFull(tag) + value1 - value2;
                else
                    val = value1 - value2;

                return (true, val);
            }
            else if (data_type == EnumDataType.MOMENT)
            {
                if (sMomentDataType == "##MinuteAve")
                {
                    return await Get_15MinAve(tag, year, month, day, hour, min, nMomentSharpSharpValue).ConfigureAwait(false);
                }
                else if (sMomentDataType == "##MinuteAve_Max")
                {
                    return await Get_15MinAve_Max_Hour(tag, year, month, day, hour, nMomentSharpSharpValue).ConfigureAwait(false);
                }
                else
                {
                    TREND_AI_STRUCT trend = new TREND_AI_STRUCT();

                    val = 0.0;

                    if (!await LoadMinDataStructAI(tag, year, month, day, hour, min, trend).ConfigureAwait(false))
                        return (false, val);
                    val = trend.fCurr;

                    return (true, val);
                }
            }
            else
            {
                return await DataGetAiHourElse(terminal, tag, year, month, day, hour, data_type).ConfigureAwait(false);
            }
        }

        async Task<(bool, double val)> DataGetAiDayElse(int terminal, string tag, int year, int month, int day, EnumDataType data_type)
        {
            double  val = 0.0;
            if (day < 1 || day > 31) return (false, val);

            bool read_flag = false;
            int read_count = 0;

            for (int hour = 0; hour < 24; hour++)
            {
                HOUR_DATA_ANALOG_STRUCT data = new HOUR_DATA_ANALOG_STRUCT();

                if (! await LoadHourDataStructAI(tag, year, month, day, hour, data).ConfigureAwait(false))
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
                    return (false, val);
                }
            }

            if (!read_flag)
                return (false, val);

            if (data_type == EnumDataType.AVE)
                val = val / read_count;

            return (true, val);
        }

        public async Task<(bool, double val)> CatDataGetAiDay(int terminal, string tag, int year, int month, int day, int hour, int min, EnumDataType data_type)
        {
            double val = 0.0;
            bool retn = false;

            //251016 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (month < 1 || month > 12) return (false, val);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            if (day < 1 || day > daysInMonth) return (false, val);

            if (data_type == EnumDataType.SUB)
            {
                double value1 = 0;
                double value2 = 0;

                // 하루의 시작을 다르게 설정할 때
                if (ConfigViewMain.bReportStartHourOfDayMaxSub)
                {
                    DateTime t = new DateTime(year, month, day);

                    t = t.AddHours(ConfigViewMain.nReportStartHourOfDay + 23);

                    (retn, value1) = await DataGetAiHourElse(terminal, tag, t.Year, t.Month, t.Day, t.Hour, EnumDataType.MAX).ConfigureAwait(false);
                    if (!retn)
                        return (false, val);

                    t = t.AddHours(-24);

                    (retn, value2) = await DataGetAiHourElse(terminal, tag, t.Year, t.Month, t.Day, t.Hour, EnumDataType.MAX).ConfigureAwait(false);
                    if (!retn) return (false, val);
                }
                else
                {
                    (retn, value1) = await DataGetAiDayElse(terminal, tag, year, month, day, EnumDataType.MAX).ConfigureAwait(false);
                    if (!retn) return (false, val);
                    TimeUtil.MinusDay(ref year, ref month, ref day);
                    (retn, value2) = await DataGetAiDayElse(terminal, tag, year, month, day, EnumDataType.MAX).ConfigureAwait(false);
                    if (!retn) return (false, val);
                }

                if (value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
                    val = TagLib.GetTagMemberFull(tag) + value1 - value2;
                else
                    val = value1 - value2;

                return (true, val);
            }
            else if (data_type == EnumDataType.MOMENT)
            {
                if (sMomentDataType == "##MinuteAve")
                {
                    return await Get_15MinAve(tag, year, month, day, hour, min, nMomentSharpSharpValue).ConfigureAwait(false);
                }
                else if (sMomentDataType == "##MinuteAve_Max")
                {
                    return await Get_15MinAve_Max_Day(tag, year, month, day, nMomentSharpSharpValue).ConfigureAwait(false);
                }
                else
                {
                    TREND_AI_STRUCT trend = new TREND_AI_STRUCT();

                    val = 0.0;

                    if (!await LoadMinDataStructAI(tag, year, month, day, hour, min, trend).ConfigureAwait(false))
                        return (false, val);
                    val = trend.fCurr;
                    return (true, val);
                }
            }
            else
            {
                return await DataGetAiDayElse(terminal, tag, year, month, day, data_type).ConfigureAwait(false);
            }
        }

        async Task<(bool, double val)> CatDataGetAiWeek(int terminal, string tag,  int year, int week, EnumDataType data_type)
        {
            bool read_flag = false;
            int read_count = 0;
            //int  day;
            double day_value = 0;
            int fr_year;
            int fr_mon;
            int fr_day;
            int i;
            bool retn = false;
            GetDayFromWeek(year, week, out fr_year, out fr_mon, out fr_day);

            double val = 0.0;

            for (i = 0; i < 7; i++)
            {   // 한달의 데이터를 모두 읽는다.
                (retn, day_value) = await DataGetAiDayElse(terminal, tag, fr_year, fr_mon, fr_day, data_type).ConfigureAwait(false);

                if(retn)
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
                        return (false, val);
                    }
                }

                TimeUtil.PlusDay(ref fr_year, ref fr_mon, ref fr_day);
            }

            if (read_flag == false)
            {
                return (false, val);   // 읽은 데이터가 없다.
            }

            if (data_type == EnumDataType.AVE)
            {   // 평균치는 평균값으로 계산한다.
                val = val / read_count;
            }

            return (true, val);
        }

        async Task<(bool, double val)> DataGetAiMonthElse(int terminal, string tag, int year, int month, EnumDataType data_type)
        {
            bool read_flag = false;
            int read_count = 0;
            int day;
            //float min = 0;
            //float max = 0;
            double day_value = 0;
            bool retn = false;

            double val = 0.0;

            //251016 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (month < 1 || month > 12) return (false, val);
            int daysInMonth = DateTime.DaysInMonth(year, month);

            for (day = 1; day <= daysInMonth; day++)
            {   // 한달의 데이터를 모두 읽는다.

                (retn, day_value) = await DataGetAiDayElse(terminal, tag, year, month, day, data_type).ConfigureAwait(false);
                if(!retn) continue;

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
                    return (false, val);
                }
            }

            if (read_flag == false)
            {
                return (false, val);   // 읽은 데이터가 없다.
            }

            if (data_type == EnumDataType.AVE)
            {   // 평균치는 평균값으로 계산한다.
                val = val / read_count;
            }

            return (true, val);
        }

        public async Task<(bool, double val)> CatDataGetAiMonth(int terminal, string tag, int year, int month, int day, int hour, int min, EnumDataType data_type)
        {
            double val = 0.0;
            bool retn = false;

            if (data_type == EnumDataType.SUB)
            {
                double value1 = 0;
                double value2 = 0;

                (retn, value1) = await DataGetAiMonthElse(terminal, tag, year, month, EnumDataType.MAX).ConfigureAwait(false);
                if (!retn) return (false, val);
                TimeUtil.MinusMonth(ref year, ref month);
                (retn, value2) = await DataGetAiMonthElse(terminal, tag, year, month, EnumDataType.MAX).ConfigureAwait(false);
                if (!retn) return (false, val);

                if (value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
                    val = TagLib.GetTagMemberFull(tag) + value1 - value2;
                else
                    val = value1 - value2;

                return (true, val);
            }
            else if (data_type == EnumDataType.MOMENT)
            {
                if (sMomentDataType == "##MinuteAve")
                {
                    return await Get_15MinAve(tag, year, month, day, hour, min, nMomentSharpSharpValue).ConfigureAwait(false);
                }
                else if (sMomentDataType == "##MinuteAve_Max")
                {
                    return await Get_15MinAve_Max_Month(tag, year, month, nMomentSharpSharpValue).ConfigureAwait(false);
                }
                else
                {
                    TREND_AI_STRUCT trend = new TREND_AI_STRUCT();

                    val = 0.0;

                    if (!await LoadMinDataStructAI(tag, year, month, day, hour, min, trend).ConfigureAwait(false))
                        return (false, val);
                    val = trend.fCurr;
                    return (true, val);
                }
            }
            else
            {
                return await DataGetAiMonthElse(terminal, tag, year, month, data_type).ConfigureAwait(false);
            }
        }


        async Task<(bool, double val)> DataGetAiYearElse(int terminal, string tag,  int year, EnumDataType data_type)
        {
            bool read_flag = false;
            int read_count = 0;
            int month;
            //float min = 0;
            //float max = 0;
            double day_value = 0;

            bool retn = false;
            double val = 0.0;

            for (month = 1; month <= 12; month++)
            {   // 한달의 데이터를 모두 읽는다.
                (retn, day_value) = await DataGetAiMonthElse(terminal, tag, year, month, data_type).ConfigureAwait(false);
                if(!retn)    continue;

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
                    return (false, val);
                }
            }

            if (read_flag == false)
            {
                return (false, val);   // 읽은 데이터가 없다.
            }

            if (data_type == EnumDataType.AVE)
            {   // 평균치는 평균값으로 계산한다.
                val = val / read_count;
            }

            return (true, val);
        }

        public async Task<(bool, double val)> CatDataGetAiYear(int terminal, string tag, int year, int month, int day, int hour, int min, EnumDataType data_type)
        {
            double val = 0.0;
            bool retn = false;

            if (data_type == EnumDataType.SUB)
            {
                double value1 = 0;
                double value2 = 0;

                (retn, value1) = await DataGetAiYearElse(terminal, tag, year, EnumDataType.MAX).ConfigureAwait(false);
                if (!retn) return (false, val);
                TimeUtil.MinusYear(ref year);
                (retn, value2) = await DataGetAiYearElse(terminal, tag, year, EnumDataType.MAX).ConfigureAwait(false);
                if (!retn) return (false, val);

                if (value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
                    val = TagLib.GetTagMemberFull(tag) + value1 - value2;
                else
                    val = value1 - value2;

                return (true, val);
            }
            else if (data_type == EnumDataType.MOMENT)
            {
                TREND_AI_STRUCT trend = new TREND_AI_STRUCT();

                val = 0.0;

                if (!await LoadMinDataStructAI(tag, year, month, day, hour, min, trend).ConfigureAwait(false))
                    return (false, val);
                val = trend.fCurr;
                return (true, val);
            }
            else
            {
                return await DataGetAiYearElse(terminal, tag, year, data_type).ConfigureAwait(false);
            }
        }



        #endregion


        #region DI 데이터 조회 메서드

        /// <summary>
        ///  해당 데이터 1개 조회.
        /// </summary>
        async Task<(bool, uint val)> CatDataGetDi(string tag, EnumDataType data_type, EnumDataTime data_time, int year, int mon, int day, int hour, int min)
        {
            bool retn;
            uint val = 0;

            //251016 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (mon < 1 || mon > 12) return (false, val);
            int daysInMonth = DateTime.DaysInMonth(year, mon);
            if (day < 1 || day > daysInMonth) return (false, val);

            if (data_time == EnumDataTime.Minute)
                (retn, val) = await CatDataGetDiMin(0, tag, year, mon, day, hour, min, data_type).ConfigureAwait(false);
            else if (data_time == EnumDataTime.Hour)
                (retn, val) = await CatDataGetDiHour(0, tag, year, mon, day, hour, data_type).ConfigureAwait(false);
            else if (data_time == EnumDataTime.Day)
                (retn, val) = await CatDataGetDiDay(0, tag, year, mon, day, data_type).ConfigureAwait(false);
            else if (data_time == EnumDataTime.Week)
                (retn, val) = await CatDataGetDiWeek(0, tag, year, mon, data_type).ConfigureAwait(false);
            else if (data_time == EnumDataTime.Month)
                (retn, val) = await CatDataGetDiMonth(0, tag, year, mon, data_type).ConfigureAwait(false);
            else if (data_time == EnumDataTime.Year)        // 2023-4-11 추가
                (retn, val) = await CatDataGetDiYear(0, tag, year, data_type).ConfigureAwait(false);
            else
                retn = false;

            if (!retn) val = 0;

            return (retn, val);
        }


        /// <summary>
        ///  DataSet 반환
        /// </summary>
        public async Task<DataSet> GetDataDi(string tag, EnumDataType value_type, EnumDataTime data_time, int year, int mon, int day, int hour, int min, int data_count, int data_gab)
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
                    (retn, val) = await  CatDataGetDi(tag, EnumDataType.COUNT, data_time, year, mon, day, hour, min).ConfigureAwait(false);
                    row["COUNT"] = val.ToString();
                }
                if ((value_type & EnumDataType.MOMENT) == EnumDataType.MOMENT)
                {
                    if (data_time == EnumDataTime.Minute)
                        (retn, val) = await CatDataGetDiMin(0, tag, year, mon, day, hour, min, EnumDataType.MOMENT).ConfigureAwait(false);
                    else if (data_time == EnumDataTime.Hour)
                        (retn, val) = await CatDataGetDiMin(0, tag, year, mon, day, hour, min, EnumDataType.MOMENT).ConfigureAwait(false);
                    else if (data_time == EnumDataTime.Day)
                        (retn, val) = await CatDataGetDiMin(0, tag, year, mon, day, hour, min, EnumDataType.MOMENT).ConfigureAwait(false);
                    else if (data_time == EnumDataTime.Month)
                        (retn, val) = await CatDataGetDiMin(0, tag, year, mon, day, hour, min, EnumDataType.MOMENT).ConfigureAwait(false);
                    else
                        retn = false;

                    if (!retn) val = 0;
                    row["MOMENT"] = val.ToString();
                }
                if ((value_type & EnumDataType.OFFTIME) == EnumDataType.OFFTIME)
                {
                    (retn, val) = await CatDataGetDi(tag, EnumDataType.OFFTIME, data_time, year, mon, day, hour, min).ConfigureAwait(false);
                    row["OFFTIME"] = val.ToString();
                }
                if ((value_type & EnumDataType.ONTIME) == EnumDataType.ONTIME)
                {
                    (retn, val) = await CatDataGetDi(tag, EnumDataType.ONTIME, data_time, year, mon, day, hour, min).ConfigureAwait(false);
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

        public async Task<(bool, uint val)> CatDataGetDiMin(int terminal, string tag, int year, int month, int day, int hour, int min, EnumDataType data_type)
        {
            TREND_DI_STRUCT trend = new TREND_DI_STRUCT();

            uint val = 0;

            //251016 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (month < 1 || month > 12) return (false, val);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            if (day < 1 || day > daysInMonth) return (false, val);

            if (!await LoadMinDataStructDI(tag, year, month, day, hour, min, trend).ConfigureAwait(false))
            {
                return (false, val);
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
                return (false, val);
            }

            return (true, val);
        }


        public async Task<(bool, uint val)> CatDataGetDiHour(int terminal, string tag, int year, int month, int day, int hour, EnumDataType data_type)
        {
            HOUR_DATA_DIGITAL_STRUCT data = new HOUR_DATA_DIGITAL_STRUCT();
            uint val = 0;

            //251016 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (month < 1 || month > 12)
                return (false, val);

            int daysInMonth = DateTime.DaysInMonth(year, month);
            if (day < 1 || day > daysInMonth) 
                return (false, val);

            if (! await LoadHourDataStructDI(tag, year, month, day, hour, data).ConfigureAwait(false)) return (false, val);

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
                return (false, val);
            }

            return (true, val);
        }

        public async Task<(bool, uint val)> CatDataGetDiDay(int terminal, string tag,  int year, int month, int day, EnumDataType data_type)
        {
            uint val = 0;

            if (day < 1 || day > 31) return (false, val);

            //251016 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (month < 1 || month > 12) return (false, val);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            if (day < 1 || day > daysInMonth) return (false, val);

            bool read_flag = false;
            int read_count = 0;

            for (int hour = 0; hour < 24; hour++)
            {
                HOUR_DATA_DIGITAL_STRUCT data = new HOUR_DATA_DIGITAL_STRUCT();

                if (!await LoadHourDataStructDI(tag, year, month, day, hour, data).ConfigureAwait(false))
                    continue;

                if (data.flag == 0)
                    continue;

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
                    return (false, val);
                }
            }

            if (!read_flag)
                return (false, val);

            if (data_type == EnumDataType.OFFTIME)
                val = (uint)(read_count * 3600 - val);

            return (true, val);
        }


        async Task<(bool, uint val)> CatDataGetDiWeek(int terminal, string tag, int year, int week, EnumDataType data_type)
        {
            int fr_year;
            int fr_mon;
            int fr_day;

            GetDayFromWeek(year, week, out fr_year, out fr_mon, out fr_day);

            uint val = 0;

            bool read_flag = false;
            int read_count = 0;
            uint imsi_val = 0;
            bool retn = false;

            for (int i = 0; i < 7; i++)
            {
                (retn, imsi_val) = await CatDataGetDiDay(0, tag, fr_year, fr_mon, fr_day, data_type).ConfigureAwait(false);

                if (retn)
                {
                    read_flag = true;
                    read_count++;
                    val += imsi_val;
                }
                TimeUtil.PlusDay(ref fr_year, ref fr_mon, ref fr_day);
            }

            if (read_flag == false)
            {
                return (false, val);   // 읽은 데이터가 없다.
            }

            return (true, val);
        }


        public async Task<(bool, uint val)> CatDataGetDiMonth(int terminal, string tag, int year, int month, EnumDataType data_type)
        {
             uint val = 0;
            bool read_flag = false;
            int read_count = 0;

            //251016 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (month < 1 || month > 12) return (false, val);
            int daysInMonth = DateTime.DaysInMonth(year, month);

            // 31일 * 24시간 = 744회 캐시에서 읽기 (기존 방식보다 훨씬 빠름)
            for (int day = 1; day <= daysInMonth; day++)
            {

                for (int hour = 0; hour < 24; hour++)
                {
                    HOUR_DATA_DIGITAL_STRUCT data = new HOUR_DATA_DIGITAL_STRUCT();

                    // 캐시된 시간별 데이터에서 읽기 (초고속)
                    if (await LoadHourDataStructDI(tag, year, month, day, hour, data).ConfigureAwait(false))
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
                                return (false, val);
                            }
                        }
                    }
                }
            }

            if (read_flag == false)
            {
                return (false, val); // 읽은 데이터가 없다.
            }

            if (data_type == EnumDataType.OFFTIME)
            {
                val = (uint)(read_count * 3600 - val);
            }

            return (true, val);

        }



        public async Task<(bool, uint val)> CatDataGetDiYear(int terminal, string tag, int year, EnumDataType data_type)
        {
            int month;
            bool read_flag = false;
            uint imsi = 0;
            bool success = false;

            uint val = 0;

            for (month = 1; month <= 12; month++)
            {
                (success, imsi) =  await CatDataGetDiMonth(terminal, tag,  year, month, data_type).ConfigureAwait(false);
                if(success)
                {
                    read_flag = true;
                    val += imsi;        //
                }
            }

            return (read_flag, val);
        }


        #endregion


        // 이것은 원래 인자로 들어와야 하는데 호환성때문에 이렇게 사용했다.
        public static string sMomentDataType = "";
        public static int nMomentSharpSharpValue = 15;

        async Task<(bool, double ave)> Get_15MinAve(string tag, int year, int month, int day, int hour, int minute, int SharpSharp)
        {
            TREND_AI_STRUCT trend = new TREND_AI_STRUCT();
            DataLocal data = new DataLocal();

            int count = 0;
            bool retn;
            double sum = 0;

            for (int i = 0, m = minute; i < SharpSharp; i++, m++)
            {
                retn = await data.LoadMinDataStructAI(tag, year, month, day, hour, m, trend).ConfigureAwait(false);

                if (retn)
                {
                    count++;
                    sum += trend.fAverage;
                }
            }

            double ave = 0;

            if (count == 0) return (false, ave);

            ave = sum / count;

            return (true, ave);
        }

        // 1시간을 15분 단위로 평균을 해서 그중 최대값을 구한다.
        async Task<(bool, double max)> Get_15MinAve_Max_Hour(string tag, int year, int month, int day, int hour, int SharpSharp)
        {
            //TREND_AI_STRUCT trend = new TREND_AI_STRUCT();
            //DataLocal data = new DataLocal();

            int count = 0;
            bool retn;
            double ave;
            double max = 0;

            for (int minute = 0; minute < 60; minute += SharpSharp)
            {
                (retn, ave) = await Get_15MinAve(tag, year, month, day, hour, minute, SharpSharp).ConfigureAwait(false);

                if (retn)
                {
                    count++;
                    if (count == 1)
                    {
                        max = ave;
                    }
                    else
                    {
                        if (ave > max)
                            max = ave;
                    }
                }
            }

            if (count == 0) return (false,max);

            return (true, max);
        }

        // 하루를 15분 단위로 평균을 해서 그중 최대값을 구한다.
        async Task<( bool, double max)> Get_15MinAve_Max_Day(string tag, int year, int month, int day, int SharpSharp)
        {
            //TREND_AI_STRUCT trend = new TREND_AI_STRUCT();
            //DataLocal data = new DataLocal();

            int count = 0;
            bool retn;
            double ave;
            double max = 0;

            for (int hour = 0; hour < 24; hour++)
            {
                for (int minute = 0; minute < 60; minute += SharpSharp)
                {
                    (retn, ave) = await Get_15MinAve(tag, year, month, day, hour, minute, SharpSharp).ConfigureAwait(false);

                    if (retn)
                    {
                        count++;
                        if (count == 1)
                        {
                            max = ave;
                        }
                        else
                        {
                            if (ave > max)
                                max = ave;
                        }
                    }
                }
            }

            if (count == 0) return (false, max);

            return (true, max);
        }

        // 한달을 15분 단위로 평균을 해서 그중 최대값을 구한다.  년보에서 사용한다.
        async Task<(bool, double max)> Get_15MinAve_Max_Month(string tag, int year, int month, int SharpSharp)
        {
            //TREND_AI_STRUCT trend = new TREND_AI_STRUCT();
            //DataLocal data = new DataLocal();

            int count = 0;
            bool retn;
            double ave;
            double max = 0;

            //251016 PSU 날짜 체크 추가. 예외발생없이 처리.
            if (month < 1 || month > 12) return (false, max);
            int daysInMonth = DateTime.DaysInMonth(year, month);

            for (int day = 1; daysInMonth <= 31; day++)
            {
                for (int hour = 0; hour < 24; hour++)
                {
                    for (int minute = 0; minute < 60; minute += SharpSharp)
                    {
                        (retn, ave) = await Get_15MinAve(tag, year, month, day, hour, minute, SharpSharp).ConfigureAwait(false);

                        if (retn)
                        {
                            count++;
                            if (count == 1)
                            {
                                max = ave;
                            }
                            else
                            {
                                if (ave > max)
                                    max = ave;
                            }
                        }
                    }
                }
            }

            if (count == 0) return (false, max);

            return (true, max);
        }


        #region 캐시 관리 메서드

        /// <summary>
        /// 모든 캐시를 정리합니다.
        /// </summary>
        public void ClearAllDataCache()
        {
            _cacheManager.ClearAllCache();
        }

        /// <summary>
        /// 자동 캐시 정리 (백그라운드에서 주기적으로 호출)
        /// </summary>
        public void AutoCleanupCache()
        {
            // 현재 시간 캐시만 무효화 (과거 데이터는 변경되지 않으므로 유지)
            _cacheManager.InvalidateCurrentMonthCache();

            Debug.WriteLine("자동 캐시 정리 완료 - 현재 시간 캐시만 무효화");
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
        #endregion



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

        //public DataSet GetLogLists()
        //{
        //    string data_dir = TotalConfig.GetProjectDataLogDirectory(TotalConfig.sDirWorkProject);

        //    string path = String.Format("{0}\\Log", data_dir);

        //    if (!Directory.Exists(path)) return null;

        //    DirectoryInfo info = new DirectoryInfo(path);

        //    DataSet ds = new DataSet();
        //    DataTable dt = new DataTable("LOG");
        //    DataColumn dc = new DataColumn("FileName", Type.GetType("System.String"));
        //    dt.Columns.Add(dc);

        //    DataRow row;

        //    FileInfo[] fis;

        //    fis = info.GetFiles("*.LOG?");
        //    Array.Sort(fis, new FileInfoCompare());
        //    foreach (FileInfo fi in fis)
        //    {
        //        row = dt.NewRow();
        //        row[0] = fi.Name;
        //        dt.Rows.Add(row);
        //    }

        //    ds.Tables.Add(dt);

        //    return ds;
        //}

        /// <summary>
        /// 날짜별 로그 목록 조회 (GetLogLists 대체)
        /// </summary>
        public async Task<DataSet> GetLogLists()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("LOG");
            dt.Columns.Add("LogDate", typeof(string));
            dt.Columns.Add("TotalCount", typeof(int));
            dt.Columns.Add("DebugCount", typeof(int));
            dt.Columns.Add("InfoCount", typeof(int));
            dt.Columns.Add("WarningCount", typeof(int));
            dt.Columns.Add("ErrorCount", typeof(int));
            dt.Columns.Add("CriticalCount", typeof(int));
            dt.Columns.Add("FatalCount", typeof(int));

            try
            {
                using (var connection = new NpgsqlConnection(ConfigDataDB.sConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string query = @"
                        SELECT 
                            TO_CHAR(log_datetime AT TIME ZONE @timezone, 'YYYY-MM-DD') as log_date, 
                            COUNT(*) as total_count,
                            COUNT(*) FILTER (WHERE level = 0) as debug_count,
                            COUNT(*) FILTER (WHERE level = 1) as info_count,
                            COUNT(*) FILTER (WHERE level = 2) as warning_count,
                            COUNT(*) FILTER (WHERE level = 3) as error_count,
                            COUNT(*) FILTER (WHERE level = 4) as critical_count,
                            COUNT(*) FILTER (WHERE level = 5) as fatal_count
                        FROM operational.logs
                        GROUP BY TO_CHAR(log_datetime AT TIME ZONE @timezone, 'YYYY-MM-DD')
                        ORDER BY log_date DESC";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@timezone", ConfigDataDB.sPostgresTimezone);

                        using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                DataRow row = dt.NewRow();

                                row["LogDate"] = reader.GetString(0);      // YYYY-MM-DD 형식
                                row["TotalCount"] = reader.GetInt64(1);
                                row["DebugCount"] = reader.GetInt64(2);
                                row["InfoCount"] = reader.GetInt64(3);
                                row["WarningCount"] = reader.GetInt64(4);
                                row["ErrorCount"] = reader.GetInt64(5);
                                row["CriticalCount"] = reader.GetInt64(6);
                                row["FatalCount"] = reader.GetInt64(7);

                                dt.Rows.Add(row);
                            }
                        }
                    }

                    ds.Tables.Add(dt);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"로그 목록 조회 오류: {ex.Message}");
            }

            return ds;
        }

        /// <summary>
        /// 특정 날짜의 로그 조회 (GetLogFile 대체)
        /// </summary>
        public async Task<DataSet> GetLogFile(string logDateStr)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Logs");
            dt.Columns.Add("Id", typeof(long));
            dt.Columns.Add("log_datetime", typeof(DateTime));
            dt.Columns.Add("Level", typeof(string));
            dt.Columns.Add("LevelValue", typeof(int));
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("CategoryValue", typeof(int));
            dt.Columns.Add("Message", typeof(string));
            dt.Columns.Add("Username", typeof(string));
            dt.Columns.Add("IpAddress", typeof(string));
            dt.Columns.Add("MachineName", typeof(string));
            dt.Columns.Add("HasDetail", typeof(bool));

            try
            {
                DateTime logDate = DateTime.Parse(logDateStr);
                DateTime startTime = logDate.Date;
                DateTime endTime = logDate.Date.AddDays(1);

                using (var connection = new NpgsqlConnection(ConfigDataDB.sConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string query = @"
                    SELECT 
                        id,
                        log_datetime,
                        level,
                        category,
                        message,
                        username,
                        ip_address,
                        machine_name,
                        CASE WHEN detail IS NOT NULL THEN true ELSE false END as has_detail
                    FROM operational.logs
                    WHERE log_datetime >= @startTime AND log_datetime < @endTime
                    ORDER BY log_datetime ASC, id ASC";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        // Local 시간을 UTC로 변환
                        DateTime startTimeUtc = DateTime.SpecifyKind(startTime, DateTimeKind.Local).ToUniversalTime();
                        DateTime endTimeUtc = DateTime.SpecifyKind(endTime, DateTimeKind.Local).ToUniversalTime();

                        command.Parameters.AddWithValue("startTime", startTimeUtc);
                        command.Parameters.AddWithValue("endTime", endTimeUtc);

                        using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                DataRow row = dt.NewRow();

                                row["Id"] = reader.GetInt64(0);

                                DateTime logDateTime = reader.GetDateTime(1);
                                row["log_datetime"] = logDateTime.ToLocalTime();

                                short levelValue = reader.GetInt16(2);
                                row["LevelValue"] = levelValue;
                                row["Level"] = ((LogLevel)levelValue).ToString();

                                int categoryValue = reader.GetInt32(3);
                                row["CategoryValue"] = categoryValue;
                                row["Category"] = LogCategory.GetCategoryName(categoryValue);

                                row["Message"] = reader.IsDBNull(4) ? "" : reader.GetString(4);
                                row["Username"] = reader.IsDBNull(5) ? "" : reader.GetString(5);
                                row["IpAddress"] = reader.IsDBNull(6) ? "" : reader.GetString(6);
                                row["MachineName"] = reader.IsDBNull(7) ? "" : reader.GetString(7);
                                row["HasDetail"] = reader.GetBoolean(8);

                                dt.Rows.Add(row);
                            }
                        }
                    }
                }

                ds.Tables.Add(dt);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"로그 파일 조회 오류: {ex.Message}");
            }

            return ds;
        }

        /// <summary>
        /// 기간별 로그 조회
        /// </summary>
        public async Task<DataTable> GetLogsByPeriod(
            DateTime startTime,
            DateTime endTime,
            LogLevel? minLevel = null,
            int? category = null,
            string searchText = null,
            string username = null,
            int maxRecords = 10000)
        {
            var dt = new DataTable("Logs");
            dt.Columns.Add("Id", typeof(long));
            dt.Columns.Add("log_datetime", typeof(DateTime));
            dt.Columns.Add("Level", typeof(string));
            dt.Columns.Add("LevelValue", typeof(int));
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("CategoryValue", typeof(int));
            dt.Columns.Add("Message", typeof(string));
            dt.Columns.Add("Username", typeof(string));
            dt.Columns.Add("IpAddress", typeof(string));
            dt.Columns.Add("MachineName", typeof(string));

            try
            {
                using (var connection = new NpgsqlConnection(ConfigDataDB.sConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    var queryBuilder = new System.Text.StringBuilder(@"
                SELECT 
                    id,
                    log_datetime,
                    level,
                    category,
                    message,
                    username,
                    ip_address,
                    machine_name
                FROM operational.logs
                WHERE log_datetime >= @startTime AND log_datetime <= @endTime");

                    var parameters = new List<NpgsqlParameter>
            {
                new NpgsqlParameter("startTime", DateTime.SpecifyKind(startTime, DateTimeKind.Local).ToUniversalTime()),
                new NpgsqlParameter("endTime", DateTime.SpecifyKind(endTime, DateTimeKind.Local).ToUniversalTime())
            };

                    // 레벨 필터
                    if (minLevel.HasValue)
                    {
                        queryBuilder.Append(" AND level >= @minLevel");
                        parameters.Add(new NpgsqlParameter("minLevel", (short)minLevel.Value));
                    }

                    // 카테고리 필터
                    if (category.HasValue)
                    {
                        if (category.Value % 100 == 0)
                        {
                            // 그룹 전체
                            queryBuilder.Append(" AND category >= @categoryMin AND category < @categoryMax");
                            parameters.Add(new NpgsqlParameter("categoryMin", category.Value));
                            parameters.Add(new NpgsqlParameter("categoryMax", category.Value + 100));
                        }
                        else
                        {
                            // 정확한 카테고리
                            queryBuilder.Append(" AND category = @category");
                            parameters.Add(new NpgsqlParameter("category", category.Value));
                        }
                    }

                    // 검색어 필터
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        queryBuilder.Append(" AND message ILIKE @searchText");
                        parameters.Add(new NpgsqlParameter("searchText", "%" + searchText + "%"));
                    }

                    // 사용자 필터
                    if (!string.IsNullOrEmpty(username))
                    {
                        queryBuilder.Append(" AND username = @username");
                        parameters.Add(new NpgsqlParameter("username", username));
                    }

                    queryBuilder.Append(" ORDER BY log_datetime DESC, id DESC LIMIT @maxRecords");
                    parameters.Add(new NpgsqlParameter("maxRecords", maxRecords));

                    using (var command = new NpgsqlCommand(queryBuilder.ToString(), connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());

                        using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                DataRow row = dt.NewRow();

                                row["Id"] = reader.GetInt64(0);

                                DateTime logDateTime = reader.GetDateTime(1);
                                row["log_datetime"] = logDateTime.ToLocalTime();

                                short levelValue = reader.GetInt16(2);
                                row["LevelValue"] = levelValue;
                                row["Level"] = ((LogLevel)levelValue).ToString();

                                int categoryValue = reader.GetInt32(3);
                                row["CategoryValue"] = categoryValue;
                                row["Category"] = LogCategory.GetCategoryName(categoryValue);

                                row["Message"] = reader.IsDBNull(4) ? "" : reader.GetString(4);
                                row["Username"] = reader.IsDBNull(5) ? "" : reader.GetString(5);
                                row["IpAddress"] = reader.IsDBNull(6) ? "" : reader.GetString(6);
                                row["MachineName"] = reader.IsDBNull(7) ? "" : reader.GetString(7);

                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"로그 조회 오류: {ex.Message}");
            }

            return dt;
        }

        /// <summary>
        /// 특정 로그의 상세 정보 조회 (detail 포함)
        /// </summary>
        public async Task<LogDetailInfo> GetLogDetail(long logId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConfigDataDB.sConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string query = @"
                SELECT 
                    id,
                    log_datetime,
                    level,
                    category,
                    message,
                    username,
                    ip_address,
                    machine_name,
                    detail,
                    created_at
                FROM operational.logs
                WHERE id = @id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("id", logId);

                        using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            if (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                return new LogDetailInfo
                                {
                                    Id = reader.GetInt64(0),
                                    LogDateTime = reader.GetDateTime(1).ToLocalTime(),
                                    Level = ((LogLevel)reader.GetInt16(2)).ToString(),
                                    LevelValue = reader.GetInt16(2),
                                    Category = LogCategory.GetCategoryName(reader.GetInt32(3)),
                                    CategoryValue = reader.GetInt32(3),
                                    Message = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    Username = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                    IpAddress = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                    MachineName = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                    Detail = reader.IsDBNull(8) ? "" : reader.GetString(8),
                                    CreatedAt = reader.GetDateTime(9).ToLocalTime()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"로그 상세 조회 오류: {ex.Message}");
            }

            return null;
        }


        /// <summary>
        /// 카테고리별 로그 통계
        /// </summary>
        public async Task<DataTable> GetLogStatisticsByCategory(DateTime startTime, DateTime endTime)
        {
            var dt = new DataTable("Statistics");
            dt.Columns.Add("CategoryGroup", typeof(string));
            dt.Columns.Add("CategoryValue", typeof(int));
            dt.Columns.Add("TotalCount", typeof(long));
            dt.Columns.Add("DebugCount", typeof(long));
            dt.Columns.Add("InfoCount", typeof(long));
            dt.Columns.Add("WarningCount", typeof(long));
            dt.Columns.Add("ErrorCount", typeof(long));
            dt.Columns.Add("CriticalCount", typeof(long));
            dt.Columns.Add("FatalCount", typeof(long));

            try
            {
                using (var connection = new NpgsqlConnection(ConfigDataDB.sConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string query = @"
                SELECT 
                    (category / 100) * 100 as category_group,
                    COUNT(*) as total_count,
                    COUNT(*) FILTER (WHERE level = 0) as debug_count,
                    COUNT(*) FILTER (WHERE level = 1) as info_count,
                    COUNT(*) FILTER (WHERE level = 2) as warning_count,
                    COUNT(*) FILTER (WHERE level = 3) as error_count,
                    COUNT(*) FILTER (WHERE level = 4) as critical_count,
                    COUNT(*) FILTER (WHERE level = 5) as fatal_count
                FROM operational.logs
                WHERE log_datetime >= @startTime AND log_datetime <= @endTime
                GROUP BY category_group
                ORDER BY category_group";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("startTime",
                            DateTime.SpecifyKind(startTime, DateTimeKind.Local).ToUniversalTime());
                        command.Parameters.AddWithValue("endTime",
                            DateTime.SpecifyKind(endTime, DateTimeKind.Local).ToUniversalTime());

                        using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                DataRow row = dt.NewRow();

                                int categoryValue = reader.GetInt32(0);
                                row["CategoryValue"] = categoryValue;
                                row["CategoryGroup"] = LogCategory.GetCategoryName(categoryValue);
                                row["TotalCount"] = reader.GetInt64(1);
                                row["DebugCount"] = reader.GetInt64(2);
                                row["InfoCount"] = reader.GetInt64(3);
                                row["WarningCount"] = reader.GetInt64(4);
                                row["ErrorCount"] = reader.GetInt64(5);
                                row["CriticalCount"] = reader.GetInt64(6);
                                row["FatalCount"] = reader.GetInt64(7);

                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"로그 통계 조회 오류: {ex.Message}");
            }

            return dt;
        }


        /// <summary>
        /// 최근 로그 조회 (실시간 모니터링용)
        /// </summary>
        public async Task<DataTable> GetRecentLogs(int count = 100, LogLevel? minLevel = null)
        {
            var dt = new DataTable("RecentLogs");
            dt.Columns.Add("Id", typeof(long));
            dt.Columns.Add("log_datetime", typeof(DateTime));
            dt.Columns.Add("Level", typeof(string));
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("Message", typeof(string));
            dt.Columns.Add("Username", typeof(string));

            try
            {
                using (var connection = new NpgsqlConnection(ConfigDataDB.sConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string query = @"
                SELECT 
                    id,
                    log_datetime,
                    level,
                    category,
                    message,
                    username
                FROM operational.logs";

                    if (minLevel.HasValue)
                    {
                        query += " WHERE level >= @minLevel";
                    }

                    query += " ORDER BY log_datetime DESC, id DESC LIMIT @count";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        if (minLevel.HasValue)
                        {
                            command.Parameters.AddWithValue("minLevel", (short)minLevel.Value);
                        }
                        command.Parameters.AddWithValue("count", count);

                        using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                DataRow row = dt.NewRow();

                                row["Id"] = reader.GetInt64(0);
                                row["log_datetime"] = reader.GetDateTime(1).ToLocalTime();
                                row["Level"] = ((LogLevel)reader.GetInt16(2)).ToString();
                                row["Category"] = LogCategory.GetCategoryName(reader.GetInt32(3));
                                row["Message"] = reader.IsDBNull(4) ? "" : reader.GetString(4);
                                row["Username"] = reader.IsDBNull(5) ? "" : reader.GetString(5);

                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"최근 로그 조회 오류: {ex.Message}");
            }

            return dt;
        }

        /// <summary>
        /// 로그 상세 정보 클래스
        /// </summary>
        public class LogDetailInfo
        {
            public long Id { get; set; }
            public DateTime LogDateTime { get; set; }
            public string Level { get; set; }
            public short LevelValue { get; set; }
            public string Category { get; set; }
            public int CategoryValue { get; set; }
            public string Message { get; set; }
            public string Username { get; set; }
            public string IpAddress { get; set; }
            public string MachineName { get; set; }
            public string Detail { get; set; }
            public DateTime CreatedAt { get; set; }
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

        //public DataSet GetLogFile(string log_name)
        //{
        //    if (TotalConfig.eOemType == EnumOemType.SBAS)
        //    {
        //        return GetLogFileOemSbas(log_name);
        //    }

        //    string data_dir = TotalConfig.GetProjectDataLogDirectory(TotalConfig.sDirWorkProject);

        //    string path = String.Format("{0}\\LOG\\{1}", data_dir, log_name);

        //    if (!File.Exists(path)) return null;

        //    DataSet ds = new DataSet();
        //    DataTable dt = new DataTable("LOG");
        //    DataColumn dc = new DataColumn("Description", Type.GetType("System.String"));
        //    dc.MaxLength = -1;// 256;
        //    dt.Columns.Add(dc);

        //    DataRow row;
        //    string one_line = "";

        //    TextReader reader;

        //    string ext = Path.GetExtension(path);

        //    if (String.Compare(ext, ".logx", true) == 0)
        //        reader = new StreamReader(path);
        //    else
        //        reader = new StreamReader(path, System.Text.Encoding.Default);

        //    while (true)
        //    {
        //        one_line = reader.ReadLine();
        //        if (one_line == null) break;

        //        row = dt.NewRow();
        //        if (TotalConfig.eOemType == EnumOemType.SBAS)
        //        {
        //            //one_line = DecodeLogOneLine(one_line);
        //        }

        //        row[0] = one_line;

        //        dt.Rows.Add(row);
        //    }

        //    reader.Close();
        //    ds.Tables.Add(dt);

        //    return ds;
        //}

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


        /// <summary>
        /// 경보 목록 조회 (날짜별 그룹화)
        /// </summary>
        public async Task<DataSet> GetAlarmListsAsync()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("ALARM");

            try
            {
                // 컬럼 정의
                dt.Columns.Add("FileName", typeof(string));      // 날짜 형식으로 변경 (예: 2025-10-15)
                dt.Columns.Add("AlarmCount", typeof(int));

                using (var connection = new NpgsqlConnection(ConfigDataDB.sConnectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand(@"
                        SELECT 
                            TO_CHAR(alarm_datetime AT TIME ZONE @timezone, 'YYYY-MM-DD') AS alarm_date,
                            COUNT(*) AS alarm_count
                            FROM operational.alarms
                            GROUP BY TO_CHAR(alarm_datetime AT TIME ZONE @timezone, 'YYYY-MM-DD')
                            ORDER BY alarm_date DESC", connection))
                    {
                        command.Parameters.AddWithValue("@timezone", ConfigDataDB.sPostgresTimezone);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                DataRow row = dt.NewRow();
                                row["FileName"] = reader.GetString(0);      // YYYY-MM-DD 형식
                                row["AlarmCount"] = reader.GetInt32(1);
                                dt.Rows.Add(row);
                            }
                        }
                    }
                }

                ds.Tables.Add(dt);
                Debug.WriteLine($"경보 목록 조회 완료: {dt.Rows.Count}개 날짜");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("GetAlarmLists 오류: {0}", ex.Message));
            }

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

        async Task<System.Data.DataTable> AddAlarmFiltered(DateTime startTime,
            DateTime endTime,
            Regex filterTag = null,
            bool[] filterType = null,
            bool[] filterPort = null,
            short? priority = null,
              int? limit = null)
        {
            return await _db.GetAlarmsAdvanced(startTime, endTime, filterTag, filterType, filterPort, priority, limit);
        }

        //void AddAlarmFile(DataTable dt, string path, Regex filterTag, bool[] filterType, bool[] filterPort, bool check_time, DateTime tFrom, DateTime tTo)
        //{
        //    DataRow row;

        //    string ext = Path.GetExtension(path);

        //    if (String.Compare(ext, ".almx", true) == 0)	// 텍스트 방식의 파일
        //    {
        //        TextReader reader = new StreamReader(path);
        //        CommaBlockString comma = new CommaBlockString();
        //        string one_line;
        //        string imsi = "";
        //        ushort uval16 = 0;
        //        uint uval32 = 0;
        //        DateTime t;

        //        while (true)
        //        {
        //            one_line = reader.ReadLine();
        //            if (one_line == null) break;

        //            comma.Set(one_line);

        //            t = comma.GetDateTime();

        //            // 시간도 필터 범위에 포함한다.
        //            if (check_time)
        //            {
        //                if (t < tFrom || t > tTo) continue;
        //            }

        //            row = dt.NewRow();

        //            row[0] = t;

        //            comma.GetString(ref imsi);	// tag
        //            if (filterTag != null && !filterTag.IsMatch(imsi)) continue;    // 태그 필터에 걸리지 않는다.
        //            row[1] = imsi;

        //            comma.GetString(ref imsi);	// description
        //            row[2] = imsi;
        //            comma.GetString(ref imsi);	// msg
        //            row[3] = imsi;
        //            comma.GetWORD(ref uval16);	// alarm_type

        //            if (filterType != null && !IsBoolFilterInclude(filterType, uval16)) continue;    // 필터에 포함되지 않는다.                    

        //            row[4] = uval16;
        //            comma.GetWORD(ref uval16);	// priority
        //            row[5] = uval16;
        //            comma.GetWORD(ref uval16);	// port

        //            if (filterPort != null && !IsBoolFilterInclude(filterPort, uval16)) continue;    // 필터에 포함되지 않는다.                    

        //            row[6] = uval16;
        //            comma.GetWORD(ref uval16);	// station
        //            row[7] = uval16;
        //            comma.GetDWORD(ref uval32);	// address
        //            row[8] = uval32;
        //            comma.GetWORD(ref uval16);	// alarm_sub_type
        //            row[9] = uval16;

        //            comma.GetString(ref imsi);  // user
        //            row[10] = imsi;
        //            comma.GetString(ref imsi);  // ip
        //            row[11] = imsi;
        //            comma.GetString(ref imsi);  // computer
        //            row[12] = imsi;

        //            dt.Rows.Add(row);
        //        }

        //        reader.Close();
        //    }
        //    else // 옛날 방식의 구조체 방식
        //    {
        //        ALARM_FILE_STRUCT alarm = new ALARM_FILE_STRUCT();
        //        FileStream fs = File.OpenRead(path);
        //        BinaryReader reader = new BinaryReader(fs);
        //        DateTime t;

        //        int alarm_count = (int)(fs.Length / ALARM_FILE_STRUCT.struct_size);

        //        for (int i = 0; i < alarm_count; i++)
        //        {
        //            alarm.LoadFromFile(reader);

        //            t = alarm.t.ToDateTime();

        //            // 시간도 필터 범위에 포함한다.
        //            if (check_time)
        //            {
        //                if (t < tFrom || t > tTo) continue;
        //            }

        //            if (filterTag != null && !filterTag.IsMatch(alarm.tag)) continue;                   // 태그 필터에 걸리지 않는다.
        //            if (filterType != null && !IsBoolFilterInclude(filterType, alarm.alarm_type)) continue;    // 필터에 포함되지 않는다.                    
        //            if (filterPort != null && !IsBoolFilterInclude(filterPort, alarm.port)) continue;    // 필터에 포함되지 않는다.                    

        //            row = dt.NewRow();
        //            //row[0] = one_line;
        //            row[0] = t;
        //            row[1] = alarm.tag;
        //            row[2] = alarm.description;
        //            row[3] = alarm.msg;
        //            row[4] = alarm.alarm_type;
        //            row[5] = alarm.priority;
        //            row[6] = alarm.port;
        //            row[7] = alarm.station;
        //            row[8] = alarm.address;
        //            row[9] = alarm.alarm_sub_type;
        //            dt.Rows.Add(row);
        //        }

        //        fs.Close();
        //    }
        //}

        //public DataSet GetAlarmFile(string alarm_file)
        //{
        //    string data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);

        //    string path = String.Format("{0}\\ALARM\\{1}", data_dir, alarm_file);

        //    // 2007.10.29 추가. 주어진 파일이 없으면 다음 버전이나 이전버전을 읽는다.
        //    if (!File.Exists(path))
        //    {
        //        string ext = Path.GetExtension(alarm_file);

        //        if (String.Compare(ext, ".ALMX", true) == 0)
        //        {
        //            path = String.Format("{0}\\ALARM\\{1}.AL3", data_dir, Path.GetFileNameWithoutExtension(alarm_file));
        //        }
        //        else
        //        {
        //            path = String.Format("{0}\\ALARM\\{1}.ALMX", data_dir, Path.GetFileNameWithoutExtension(alarm_file));
        //        }

        //        if (!File.Exists(path)) return null;
        //    }

        //    DataSet ds = new DataSet();
        //    DataTable dt = new DataTable("ALARM");

        //    MakeAlarmHeader(dt);
        //    AddAlarmFile(dt, path, null, null, null, false, DateTime.Now, DateTime.Now);

        //    ds.Tables.Add(dt);

        //    return ds;
        //}

        /// <summary>
        /// 특정 날짜의 경보 조회 (GetAlarmFile 대체)
        /// </summary>
        public async Task<DataSet> GetAlarmFileAsync(string alarmDateStr)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("ALARM");

            // 경보 테이블 구조 정의
            dt.Columns.Add("Id", typeof(long));
            dt.Columns.Add("alarm_datetime", typeof(DateTime));
            dt.Columns.Add("tag_name", typeof(string));
            dt.Columns.Add("description", typeof(string));
            dt.Columns.Add("message", typeof(string));
            dt.Columns.Add("alarm_type", typeof(int));
            dt.Columns.Add("priority", typeof(int));
            dt.Columns.Add("port", typeof(int));
            dt.Columns.Add("station", typeof(int));
            dt.Columns.Add("address", typeof(long));
            dt.Columns.Add("sub_type", typeof(int));
            dt.Columns.Add("username", typeof(string));
            dt.Columns.Add("ip_address", typeof(string));
            dt.Columns.Add("computer_name", typeof(string));

            try
            {
                DateTime alarmDate = DateTime.Parse(alarmDateStr);
                DateTime startTime = alarmDate.Date;
                DateTime endTime = alarmDate.Date.AddDays(1);

                using (var connection = new NpgsqlConnection(ConfigDataDB.sConnectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    string query = @"
                        SELECT 
                            id,
                            alarm_datetime,
                            tag_name,
                            description,
                            message,
                            alarm_type,
                            priority,
                            port,
                            station,
                            address,
                            sub_type,
                            username,
                            ip_address,
                            computer_name
                        FROM operational.alarms
                        WHERE alarm_datetime >= @startTime AND alarm_datetime < @endTime
                        ORDER BY alarm_datetime ASC, id ASC";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        // Local 시간을 UTC로 변환
                        DateTime startTimeUtc = DateTime.SpecifyKind(startTime, DateTimeKind.Local).ToUniversalTime();
                        DateTime endTimeUtc = DateTime.SpecifyKind(endTime, DateTimeKind.Local).ToUniversalTime();

                        command.Parameters.AddWithValue("startTime", startTimeUtc);
                        command.Parameters.AddWithValue("endTime", endTimeUtc);

                        using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                DataRow row = dt.NewRow();

                                row["Id"] = reader.GetInt64(0);

                                DateTime alarmDateTime = reader.GetDateTime(1);
                                row["alarm_datetime"] = alarmDateTime.ToLocalTime();

                                row["tag_name"] = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                row["description"] = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                row["message"] = reader.IsDBNull(4) ? "" : reader.GetString(4);

                                int alarmType = reader.GetInt32(5);
                                row["alarm_type"] = alarmType;
                                row["priority"] = reader.GetInt32(6);
                                row["port"] = reader.GetInt32(7);
                                row["station"] = reader.GetInt32(8);
                                row["address"] = reader.GetInt64(9);
                                row["sub_type"] = reader.GetInt32(10);

                                row["username"] = reader.IsDBNull(11) ? "" : reader.GetString(11);
                                row["ip_address"] = reader.IsDBNull(12) ? "" : reader.GetString(12);
                                row["computer_name"] = reader.IsDBNull(13) ? "" : reader.GetString(13);

                                dt.Rows.Add(row);
                            }
                        }
                    }
                }

                ds.Tables.Add(dt);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("경보 파일 조회 오류: {0}", ex.Message));
            }

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

        public async Task<DataSet> GetAlarmFileByScript(DateTime tFrom, DateTime tTo, string option, bool checktime)
        {
            //string data_dir = TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject);

            //string path = String.Format("{0}\\ALARM", data_dir);
            //if (!Directory.Exists(path)) return null;

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

           // DirectoryInfo info = new DirectoryInfo(path);

            //int dayhapfr = (int)TimeUtil.GetDayHap(tFrom.Year, tFrom.Month, tFrom.Day);
            //int dayhapto = (int)TimeUtil.GetDayHap(tTo.Year, tTo.Month, tTo.Day);

            //foreach (FileInfo fi in info.GetFiles("*.AL3"))
            //{
            //    if (IsFileInclude(fi.Name, dayhapfr, dayhapto))
            //        AddAlarmFile(dt, fi.FullName, filterTag, filterType, filterPort, checktime, tFrom, tTo);
            //}

            //foreach (FileInfo fi in info.GetFiles("*.ALMX"))
            //{
            //    if (IsFileInclude(fi.Name, dayhapfr, dayhapto))
            //        AddAlarmFile(dt, fi.FullName, filterTag, filterType, filterPort, checktime, tFrom, tTo);
            //}

            dt = await AddAlarmFiltered(tFrom, tTo, filterTag, filterType, filterPort, null, null);

            ds.Tables.Add(dt);

            return ds;
        }

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

            // DefaultDB (PostgreSQL) via DataGate relay
            if (dsn == "__defaultdb__")
            {
                return GetDataSetFromDefaultDB(command, out error);
            }

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

        DataSet GetDataSetFromDefaultDB(string command, out string error)
        {
            error = "";
            string connStr = ConfigDataDB.sConnectionString;
            if (string.IsNullOrEmpty(connStr))
            {
                error = "Default DB connection string is not configured.";
                return null;
            }

            DataSet ds = new DataSet();
            try
            {
                using (var conn = new NpgsqlConnection(connStr))
                {
                    conn.Open();
                    using (var adapter = new NpgsqlDataAdapter(command, conn))
                    {
                        adapter.Fill(ds, "TableTest");
                    }
                }
            }
            catch (Exception ex)
            {
                error = String.Format("DefaultDB Error\nQuery={0}\n{1}", command, ex.Message);
                return null;
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

        /*
		public bool DataSetCommand(string dsn, string command, out string error)
		{
			ConnectionString con_str;

			con_str = DbTool.GetConnectionString(dsn);

			if(con_str == null) 
			{
				if(Tools.IsLangKorean())
					error = String.Format("{0} 목록을 찾을 수 없습니다.", dsn);
				else  
					error = String.Format("Can't find {0} list at connection string", dsn);

				return false;
			}

			CommonDbConnection conn = new CommonDbConnection(con_str.dbConnectionType);

			conn.ConnectionString = con_str.dsn;

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

			try 
			{
				dbcom.ExecuteNonQuery();
			}
			catch (Exception exception) 
			{
				conn.Close();
				error = exception.Message;
				return false;
			}

			conn.Close();

			error = "";
			return true;
		}*/

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

