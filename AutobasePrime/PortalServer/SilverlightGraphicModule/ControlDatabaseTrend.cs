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
using AutoLibLocal;
using NetTools;
using AutoLib;
using System.Xml.Linq;

namespace SilverlightGraphicModule
{
    public class ControlDatabaseTrend : ControlPublicGraph
    {
        bool bUseLocalRange = false;

        

        public int nDataGab = 1;
        ObjectArgsDatabaseTrend objArgs;

        public ControlDatabaseTrend(int show_unit, List<object> members, int point_size, int horzdevide, int vertdevide, DateTime t, int timetype, BrushPublic back_color, BrushPublic fill_color, Color guide_color, EnumDisplayFlag displayflag, bool display_cursor_time, int data_gab, ObjectArgsDatabaseTrend args) 
            : base (EnumGraphType.MultiTrend, show_unit, members, point_size, horzdevide, vertdevide, t, timetype, back_color, fill_color, guide_color, displayflag, display_cursor_time)
        {
            objArgs = args;

            TagFile.eventHandlerOnTagFileReaded += new EventHandler(TagFile_eventHandlerOnTagFileReaded);

            nDataGab = data_gab;

            CalcStartTime();

            ReadAllPoint();
        }

        void TagFile_eventHandlerOnTagFileReaded(object sender, EventArgs e)
        {
            ReadAllPoint();
            UpdateGraph();
        }

        DateTime FitStartTime(DateTime dt)
        {
            if (objArgs.wTimeSelectOption == 0)
            {	// milli data
                return dt;
            }
            else if (objArgs.wTimeSelectOption == 1)
            {	// sec data
                return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
            }
            else if (objArgs.wTimeSelectOption == 2)
            {	// min data
                return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
            }
            else if (objArgs.wTimeSelectOption == 3)
            {	// hour data
                return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
            }
            else if (objArgs.wTimeSelectOption == 4)
            {	// day data
                return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            }
            else if (objArgs.wTimeSelectOption == 5)
            {	// month data
                return new DateTime(dt.Year, dt.Month, 1, 0, 0, 0);
            }
            else
            {	// year data
                return new DateTime(dt.Year, 1, 1, 0, 0, 0);
            }
        }

        void CalcStartTime()
		{
			if(TotalConfig.defineMode == EnumDefineMode.MODE_RUN) 
			{
				if(cGraphStartTimeMethod == 0) 
				{
					DateTime dt = DateTime.Now;

					dt = FitStartTime(dt);

					int data_cycle = objArgs.wShowUnit*objArgs.nDataCycle;

					if(objArgs.wTimeSelectOption == 0) 
					{	// milli data
						dtStartTime = dt.AddMilliseconds(-data_cycle);
					}
					else if(objArgs.wTimeSelectOption == 1) 
					{	// sec data
						dtStartTime = dt.AddSeconds(-data_cycle);
					}
					else if(objArgs.wTimeSelectOption == 2) 
					{	// min data
						dtStartTime = dt.AddMinutes(-data_cycle);
					}
					else if(objArgs.wTimeSelectOption == 3) 
					{	// hour data
						dtStartTime = dt.AddHours(-data_cycle);
					}
					else if(objArgs.wTimeSelectOption == 4) 
					{	// day data
						dtStartTime = dt.AddDays(-data_cycle);
					}
					else if(objArgs.wTimeSelectOption == 5)
					{	// month data
						dtStartTime = dt.AddMonths(-data_cycle);
					}
					else 
					{	// year data
						dtStartTime = dt.AddYears(-data_cycle);
					}
				}
				else 
				{
					dtStartTime = FitStartTime(dtStartTime);
				}
			}
			else 
			{
				DateTime dt = DateTime.Now;

				dt = FitStartTime(dt);

				int data_cycle = objArgs.wShowUnit*objArgs.nDataCycle;

				if(objArgs.wTimeSelectOption == 0) 
				{	// milli data
					dtStartTime = dt.AddMilliseconds(-data_cycle);
				}
				else if(objArgs.wTimeSelectOption == 1) 
				{	// sec data
					dtStartTime = dt.AddSeconds(-data_cycle);
				}
				else if(objArgs.wTimeSelectOption == 2) 
				{	// min data
					dtStartTime = dt.AddMinutes(-data_cycle);
				}
				else if(objArgs.wTimeSelectOption == 3) 
				{	// hour data
					dtStartTime = dt.AddHours(-data_cycle);
				}
				else if(objArgs.wTimeSelectOption == 4) 
				{	// day data
					dtStartTime = dt.AddDays(-data_cycle);
				}
				else if(objArgs.wTimeSelectOption == 5)
				{	// month data
					dtStartTime = dt.AddMonths(-data_cycle);
				}
				else 
				{	// year data
					dtStartTime = dt.AddYears(-data_cycle);
				}
			}

		}

        string MakeDateTimeString(EnumDbType dbtype, int year, int mon, int day, int hour, int min, int sec)
        {
            string command;
            if (objArgs.nDateColumnType == 1)
            {
                command = String.Format("'{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}'", year, mon, day, hour, min, sec);
            }
            else
            {
                command = DbTool.MakeDateTimeString(dbtype, year, mon, day, hour, min, sec);
            }

            return command;
        }

        public override void ReadAllPoint()
        {
            ClearReadFlag();

            SilverlightAutoLibLocal.ServiceReferenceDataSet2.WebServiceDataSet2SoapClient service = SilverlightAutoLibLocal.ServiceLib.GetServiceDataSet2();
            service.GetConnectionStringDbTypeCompleted += new EventHandler<SilverlightAutoLibLocal.ServiceReferenceDataSet2.GetConnectionStringDbTypeCompletedEventArgs>(service_GetConnectionStringDbTypeCompleted);
            service.GetConnectionStringDbTypeAsync(objArgs.sDsn);
        }

        void service_GetConnectionStringDbTypeCompleted(object sender, SilverlightAutoLibLocal.ServiceReferenceDataSet2.GetConnectionStringDbTypeCompletedEventArgs e)
        {
            EnumDbType dbtype = (EnumDbType)e.dbtype;

            DB_TREND_MEMBER member;
            int i;

            string time_from;
            DateTime t_to;
            string time_to;
            int data_cycle = nShowUnit * objArgs.nDataCycle;
            DataGate gate = new DataGate();

            for (i = 0; i < blockMember.Count; i++)
            {
                member = (DB_TREND_MEMBER)blockMember[i];

                if ( wTimeSelectOption == 0)
                {
                    time_from = MakeDateTimeString(dbtype, this.dtStartTime.Year, this.dtStartTime.Month, this.dtStartTime.Day, this.dtStartTime.Hour, this.dtStartTime.Minute, this.dtStartTime.Second);
                    t_to = dtStartTime.AddMilliseconds(data_cycle);
                    time_to = MakeDateTimeString(dbtype, t_to.Year, t_to.Month, t_to.Day, t_to.Hour, t_to.Minute, t_to.Second);
                }
                else if (wTimeSelectOption == 1)
                {
                    time_from = MakeDateTimeString(dbtype, this.dtStartTime.Year, this.dtStartTime.Month, this.dtStartTime.Day, this.dtStartTime.Hour, this.dtStartTime.Minute, this.dtStartTime.Second);
                    t_to = dtStartTime.AddSeconds(data_cycle);
                    time_to = MakeDateTimeString(dbtype, t_to.Year, t_to.Month, t_to.Day, t_to.Hour, t_to.Minute, t_to.Second);
                }
                else if (wTimeSelectOption == 2)
                {
                    time_from = MakeDateTimeString(dbtype, this.dtStartTime.Year, this.dtStartTime.Month, this.dtStartTime.Day, this.dtStartTime.Hour, this.dtStartTime.Minute, 0);
                    t_to = dtStartTime.AddMinutes(data_cycle);
                    time_to = MakeDateTimeString(dbtype, t_to.Year, t_to.Month, t_to.Day, t_to.Hour, t_to.Minute, 59);
                }
                else if (wTimeSelectOption == 3)
                {
                    time_from = MakeDateTimeString(dbtype, this.dtStartTime.Year, this.dtStartTime.Month, this.dtStartTime.Day, this.dtStartTime.Hour, 0, 0);
                    t_to = dtStartTime.AddHours(data_cycle);
                    time_to = MakeDateTimeString(dbtype, t_to.Year, t_to.Month, t_to.Day, t_to.Hour, 59, 59);
                }
                else if (wTimeSelectOption == 4)
                {
                    time_from = MakeDateTimeString(dbtype, this.dtStartTime.Year, this.dtStartTime.Month, this.dtStartTime.Day, 0, 0, 0);
                    t_to = dtStartTime.AddHours(data_cycle);
                    time_to = MakeDateTimeString(dbtype, t_to.Year, t_to.Month, t_to.Day, 23, 59, 59);
                }
                else if (wTimeSelectOption == 5)
                {
                    time_from = MakeDateTimeString(dbtype, this.dtStartTime.Year, this.dtStartTime.Month, 1, 0, 0, 0);
                    t_to = dtStartTime.AddDays(data_cycle);
                    time_to = MakeDateTimeString(dbtype, t_to.Year, t_to.Month, 31, 23, 59, 59);
                }
                else if (wTimeSelectOption == 6)
                {
                    time_from = MakeDateTimeString(dbtype, this.dtStartTime.Year, 1, 1, 0, 0, 0);
                    t_to = dtStartTime.AddDays(data_cycle);
                    time_to = MakeDateTimeString(dbtype, t_to.Year, 12, 31, 23, 59, 59);
                }
                else
                {
                    continue;
                }

                string command;
                string table_name;

                if (member.sTable.Length == 0)
                    table_name = objArgs.sTable;
                else
                    table_name = member.sTable;

                if (member.sWhereString.Length > 0)
                    command = String.Format("SELECT {1},{4} FROM {0} WHERE {1}>={2} AND {1}<={3} AND {5} ORDER BY {1} ASC", table_name, DbTool.Field(dbtype, objArgs.sColumnTime), time_from, time_to, DbTool.Field(dbtype, member.column), member.sWhereString);
                else
                    command = String.Format("SELECT {1},{4} FROM {0} WHERE {1}>={2} AND {1}<={3} ORDER BY {1} ASC", table_name, DbTool.Field(dbtype, objArgs.sColumnTime), time_from, time_to, DbTool.Field(dbtype, member.column));

                //string error;
                //DataSet ds = gate.GetDataSetFromDsn(objArgs.sDsn, command, out error);

                SilverlightAutoLibLocal.ServiceReferenceDataSet2.WebServiceDataSet2SoapClient service = SilverlightAutoLibLocal.ServiceLib.GetServiceDataSet2();
                service.GetDataSetFromDsnCompleted += new EventHandler<SilverlightAutoLibLocal.ServiceReferenceDataSet2.GetDataSetFromDsnCompletedEventArgs>(service_GetDataSetFromDsnCompleted);
                service.GetDataSetFromDsnAsync(objArgs.sDsn, command, i);
                /*
                if (ds == null) continue;

                int column_time = ds.Tables[0].Columns.IndexOf(objArgs.sColumnTime);
                if (column_time == -1) continue;

                int column_pos = ds.Tables[0].Columns.IndexOf(member.column);
                if (column_pos == -1) continue;

                DataRow row;
                DateTime t;
                DateTime dbt;

                t = dtStartTime;

                int k = 0;	// row_pos

                for (j = 0; j < objArgs.wShowUnit; j++)
                {
                    member.point[j].read_flag = false;
                    while (true)
                    {
                        if (k >= ds.Tables[0].Rows.Count) break;

                        row = ds.Tables[0].Rows[k];

                        dbt = GetDateTimeFromRow(ds, row, column_time);

                        if (t == dbt)
                        {
                            member.point[j].read_flag = true;
                            string s = row[column_pos].ToString();
                            member.point[j].val = s.Length == 0 ? 0 : ConvertTool.ToDouble(s);
                            k++;
                            break;
                        }
                        else if (dbt < t)	// 다음 레코드를 읽어야 한다.
                        {
                            k++;
                        }
                        else	// 
                        {
                            break;
                        }
                    }

                    if (objArgs.wTimeSelectOption == 0) t = t.AddMilliseconds(objArgs.nDataCycle);
                    else if (objArgs.wTimeSelectOption == 1) t = t.AddSeconds(objArgs.nDataCycle);
                    else if (objArgs.wTimeSelectOption == 2) t = t.AddMinutes(objArgs.nDataCycle);
                    else if (objArgs.wTimeSelectOption == 3) t = t.AddHours(objArgs.nDataCycle);
                    else if (objArgs.wTimeSelectOption == 4) t = t.AddDays(objArgs.nDataCycle);
                    else if (objArgs.wTimeSelectOption == 5) t = t.AddMonths(objArgs.nDataCycle);
                    else t = t.AddYears(objArgs.nDataCycle);

                }*/
            }


            /*
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) return;

            PUBLIC_GRAPH_MEMBER member;
            int i;
            //string sRealTag;
            int datagab;

            if (wTimeSelectOption == 0) datagab = nDataGab; // 분 자료일때만 사용?
            else datagab = 1;

            int data_count = nShowUnit / datagab;

            for (i = 0; i < blockMember.Count; i++)
            {
                member = (PUBLIC_GRAPH_MEMBER)blockMember[i];

                TagLib.GetTagTypeAndPos(member.tag, ref member.nType, ref member.nPos);

                if (member.nType == EnumTagType.AI)
                {
                    TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);

                    EnumDataType data_type = EnumDataType.AVE;
                    EnumDataTime data_time = EnumDataTime.Minute;

                    if (member.nValueType == 0)
                        data_type = EnumDataType.AVE;
                    else if (member.nValueType == 1)
                        data_type = EnumDataType.MIN;
                    else if (member.nValueType == 2)
                        data_type = EnumDataType.MAX;
                    else if (member.nValueType == 3)
                        data_type = EnumDataType.SUM;
                    else if (member.nValueType == 4)
                        data_type = EnumDataType.SUB;
                    else if (member.nValueType == 5)
                        data_type = EnumDataType.MOMENT;

                    if (wTimeSelectOption == 0)
                        data_time = EnumDataTime.Minute;
                    else if (wTimeSelectOption == 1)
                        data_time = EnumDataTime.Hour;
                    else if (wTimeSelectOption == 2)
                        data_time = EnumDataTime.Day;
                    else if (wTimeSelectOption == 3)
                        data_time = EnumDataTime.Month;

                    SilverlightAutoLibLocal.ServiceReferenceDataTag2.WebServiceDataTag2SoapClient service = SilverlightAutoLibLocal.ServiceLib.GetServiceDataTag2();
                    service.GetDataAiCompleted += new EventHandler<SilverlightAutoLibLocal.ServiceReferenceDataTag2.GetDataAiCompletedEventArgs>(service_GetDataAiCompleted);
                    service.GetDataAiAsync(ai.tag, (int)data_type, (int)data_time, dtStartTime.Year, dtStartTime.Month, dtStartTime.Day, dtStartTime.Hour, dtStartTime.Minute, data_count, datagab, i);
                }
                else if (member.nType == EnumTagType.DI)
                {
                    TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);

                    EnumDataTime data_time = EnumDataTime.Minute;
                    if (wTimeSelectOption == 0)
                        data_time = EnumDataTime.Minute;
                    else if (wTimeSelectOption == 1)
                        data_time = EnumDataTime.Hour;
                    else if (wTimeSelectOption == 2)
                        data_time = EnumDataTime.Day;
                    else if (wTimeSelectOption == 3)
                        data_time = EnumDataTime.Month;

                    SilverlightAutoLibLocal.ServiceReferenceDataTag2.WebServiceDataTag2SoapClient service = SilverlightAutoLibLocal.ServiceLib.GetServiceDataTag2();
                    service.GetDataDiCompleted += new EventHandler<SilverlightAutoLibLocal.ServiceReferenceDataTag2.GetDataDiCompletedEventArgs>(service_GetDataDiCompleted);
                    service.GetDataDiAsync(di.tag, (int)EnumDataType.MOMENT, (int)data_time, dtStartTime.Year, dtStartTime.Month, dtStartTime.Day, dtStartTime.Hour, dtStartTime.Minute, data_count, datagab, i);
                }
                else
                {

                }

            }
            */
        }

        void service_GetDataSetFromDsnCompleted(object sender, SilverlightAutoLibLocal.ServiceReferenceDataSet2.GetDataSetFromDsnCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled == true) return;

            if (e.Result == null)
            {
                return;
            }

            GetDataFromString(e.Result, (int)e.UserState);
        }

        DateTime GetDateTimeFromRow(string time_data)
        {
            DateTime t;

            if (objArgs.nDateColumnType == 1)
            {
                int year, mon, day, hour, min, sec;
                string s = time_data;
                string imsi;
                if (s.Length < 14)
                {
                    t = new DateTime(1, 1, 1, 0, 0, 0);
                }
                else
                {
                    imsi = s.Substring(0, 4);
                    year = ConvertTool.ToInt32(imsi);
                    imsi = s.Substring(4, 2);
                    mon = ConvertTool.ToInt32(imsi);
                    imsi = s.Substring(6, 2);
                    day = ConvertTool.ToInt32(imsi);
                    imsi = s.Substring(8, 2);
                    hour = ConvertTool.ToInt32(imsi);
                    imsi = s.Substring(10, 2);
                    min = ConvertTool.ToInt32(imsi);
                    imsi = s.Substring(12, 2);
                    sec = ConvertTool.ToInt32(imsi);

                    if (hour == 24)	// 24시로 되어있으면 0으로 해주지만 실제로 데이타를 읽어오지 못할수도 있다. 소팅순서가 틀리므로 만약 24시가 다음날 00시라면 날짜를 하루 더해야 한다.
                    {
                        hour = 0;
                    }

                    try
                    {
                        t = new DateTime(year, mon, day, hour, min, sec);
                    }
                    catch
                    {
                        t = new DateTime(1, 1, 1, 0, 0, 0);
                    }
                }
            }
            else
            {
                try
                {
                    t = ConvertTool.ToDateTime(time_data);
                }
                catch
                {
                    t = new DateTime(1, 1, 1, 0, 0, 0);
                }
            }

            return t;
        }

        void GetDataFromString(string data, int member_pos)
        {
            if (member_pos < 0 || member_pos >= blockMember.Count) return;  // range over

            DB_TREND_MEMBER member;

            member = (DB_TREND_MEMBER)blockMember[member_pos];

            int datagab;

            if (wTimeSelectOption == 0) datagab = nDataGab;
            else datagab = 1;

            int data_count = nShowUnit / datagab;

            XDocument xd = XDocument.Parse(data);
            XElement root = xd.Root;

            int pos = 0;

            long hap;

            if (wTimeSelectOption == 0) hap = TimeUtil.GetMilliSecHap(dtStartTime);
            else if (wTimeSelectOption == 1) hap = TimeUtil.GetSecHap(dtStartTime);
            else if (wTimeSelectOption == 2) hap = TimeUtil.GetMinHap(dtStartTime);
            else if (wTimeSelectOption == 3) hap = TimeUtil.GetHourHap(dtStartTime);
            else if (wTimeSelectOption == 4) hap = TimeUtil.GetDayHap(dtStartTime);
            else if (wTimeSelectOption == 5) hap = TimeUtil.GetMonHap(dtStartTime);
            else hap = dtStartTime.Year / objArgs.nDataCycle;

            foreach (XElement el in root.Elements())
            {
                bool flag_time = false;
                bool flag_data = false;
                string value_data="";
                string value_time="";

                foreach (XElement n in el.Elements())
                {
                    if (String.Compare(n.Name.LocalName, objArgs.sColumnTime, StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        flag_time = true;// n.Value == "1" ? true : false;
                        value_time = n.Value;
                    }
                    else if (String.Compare(n.Name.LocalName, member.column, StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        flag_data = true;// n.Value == "1" ? true : false;
                        value_data = n.Value;
                    }

                    if (flag_time && flag_data)
                    {
                        DateTime t = GetDateTimeFromRow(value_time);

                        if (wTimeSelectOption == 0)
                        {
                            pos = (int)(TimeUtil.GetMilliSecHap(t) / objArgs.nDataCycle - hap);
                        }
                        else if (wTimeSelectOption == 1)
                        {
                            pos = (int)(TimeUtil.GetSecHap(t) / objArgs.nDataCycle - hap);
                        }
                        else if (wTimeSelectOption == 2)
                        {
                            pos = (int)(TimeUtil.GetMinHap(t) / objArgs.nDataCycle - hap);
                        }
                        else if (wTimeSelectOption == 3)
                        {
                            pos = (int)(TimeUtil.GetHourHap(t) / objArgs.nDataCycle - hap);
                        }
                        else if (wTimeSelectOption == 4)
                        {
                            pos = (int)(TimeUtil.GetDayHap(t) / objArgs.nDataCycle - hap);
                        }
                        else if (wTimeSelectOption == 5)
                        {
                            pos = (int)(TimeUtil.GetMonHap(t) / objArgs.nDataCycle - hap);
                        }
                        else 
                        {
                            pos = (int)(t.Year / objArgs.nDataCycle - hap);
                        }

                        if (pos >= 0 && pos < nShowUnit)
                        {
                            if (member.point[pos].read_flag == false)   // 이미 읽은 경우는 처음의 데이터가 맞을 확률이 높다. (시간정렬 & 시간이 정확히 맞아 떨어진다.)
                            {
                                member.point[pos].read_flag = true;
                                member.point[pos].val = ConvertTool.ToDouble(value_data);
                            }
                        }
                        
                        break;
                    }
                }

                pos+=datagab;
            }

            string sRealTag;

            for (int i = 0; i < blockMember.Count; i++)
            {
                member = (DB_TREND_MEMBER)blockMember[i];
                if (member.nType == EnumTagType.AI)
                {
                    TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
                    GetViewFullBase(ai, member, out member.max_value, out member.min_value);
                }
                else
                {
                    member.min_value = 0;
                    member.max_value = 100;
                }

                if (member.nType == EnumTagType.AI)
                {	// AI
                    TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
                    sRealTag = ai.tag;
                }
                else if (member.nType == EnumTagType.DI)
                {	// DI
                    TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);
                    sRealTag = di.tag;
                }
                else { }

                if (bAutoViewRange)
                {
                    int read_count = 0;

                    for (int j = 0; j < nShowUnit; j++)
                    {
                        if (member.point[j].read_flag)
                        {
                            double val = member.point[j].val;

                            if (read_count == 0)
                            {
                                member.min_value = val;
                                member.max_value = val;
                            }
                            else
                            {
                                if (val < member.min_value) member.min_value = val;
                                if (val > member.max_value) member.max_value = val;
                            }
                            read_count++;
                        }
                    }
                }
            }

            UpdateGraph();
        }

        TagAiClass GetRealTagAI(string tag, ref int[] tag_pos)
        {
            TagAiClass ai = TagLib.GetStructAI(tag, ref tag_pos);

            if (ai.cTagLinkType == 3)
            {	// 간접태그
                if (ai.assign != null && ai.assign.pos[0] != TagLib.TAG_NOT_FOUND)
                {	// assign tag가 있을 때
                    TagAiClass ai2 = TagLib.GetStructAI(ai.assign.tag, ref ai.assign.pos);
                    return ai2;
                }
            }

            return ai;
        }

        TagDiClass GetRealTagDI(string tag, ref int[] tag_pos)
        {
            TagDiClass di = TagLib.GetStructDI(tag, ref tag_pos);

            if (di.cTagLinkType == 3)
            {	// 간접태그
                if (di.assign != null && di.assign.pos[0] != TagLib.TAG_NOT_FOUND)
                {	// assign tag가 있을 때
                    TagDiClass di2 = TagLib.GetStructDI(di.assign.tag, ref di.assign.pos);
                    return di2;
                }
            }

            return di;
        }

        void GetViewFullBase(TagAiClass ai, PUBLIC_GRAPH_MEMBER member, out double view_full, out double view_base)
        {
            if (bUseLocalRange)
            {
                view_full = member.view_full;
                view_base = member.view_base;
            }
            else
            {
                view_full = ai.view_full;
                view_base = ai.view_base;
            }
        }

        public override DateTime GetCursorTime(out string buf)
        {
            DateTime dt = new DateTime(dtStartTime.Ticks);

            int x1 = nCursorX1;
            int x2 = nCursorX2;

            if (x1 > x2) Tools.Temp(ref x1, ref x2);

            if (wTimeSelectOption == 0)
            {	// min data
                dt = dt.AddMinutes(x1);
                buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
            }
            else if (wTimeSelectOption == 1)
            {	// min data
                dt = dt.AddMinutes(x1);
                buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
            }
            else if (wTimeSelectOption == 2)
            {	// min data
                dt = dt.AddMinutes(x1);
                buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);
            }
            else if (wTimeSelectOption == 3)
            {	// hour data
                dt = dt.AddHours(x1);
                if (Tools.IsLangKorean())
                {
                    buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}시", dt.Year, dt.Month, dt.Day, dt.Hour);
                }
                else if (Tools.IsLangJapanese())
                {
                    buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}時", dt.Year, dt.Month, dt.Day, dt.Hour);
                }
                else if (Tools.IsLangChinese())
                {
                    buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}时", dt.Year, dt.Month, dt.Day, dt.Hour);
                }
                else
                {
                    buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}H", dt.Year, dt.Month, dt.Day, dt.Hour);
                }
            }
            else if (wTimeSelectOption == 4)
            {	// day data
                dt = dt.AddDays(x1);
                buf = String.Format("{0:0000}/{1:00}/{2:00}", dt.Year, dt.Month, dt.Day);
            }
            else if (wTimeSelectOption == 5)
            {	// mon data
                dt = dt.AddMonths(x1);
                if (Tools.IsLangKorean())
                {
                    buf = String.Format("{0:0000}년 {1:00}월", dt.Year, dt.Month);
                }
                else if (Tools.IsLangJapanese())
                {
                    buf = String.Format("{0:0000}年 {1:00}月", dt.Year, dt.Month);
                }
                else if (Tools.IsLangChinese())
                {
                    buf = String.Format("{0:0000}年 {1:00}月", dt.Year, dt.Month);
                }
                else
                {
                    buf = String.Format("{0:0000}/{1:00}", dt.Year, dt.Month);
                }
            }
            else
            {
                buf = "Time time unknown";
            }

            return dt;
        }

        public override void Timer()
        {
            TagPublicClass pub;

            PUBLIC_GRAPH_MEMBER member;
            int i;

            for (i = 0; i < blockMember.Count; i++)
            {
                member = (PUBLIC_GRAPH_MEMBER)blockMember[i];
                pub = TagLib.GetStructPublic(member.tag, ref member.nPos);
                pub.NeedDataCurr = true;
            }

            base.Timer();
        }

        public void SetStartTimeMode(sbyte mode)
        {
            cGraphStartTimeMethod = mode;
            CalcStartTime();
            SetStartTime(dtStartTime);
        }
    }
}
