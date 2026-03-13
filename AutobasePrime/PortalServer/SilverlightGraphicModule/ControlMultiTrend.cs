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
    public class ControlMultiTrend : ControlPublicGraph
    {
        bool bUseLocalRange = false;

        public int nDataGab = 1;

        public ControlMultiTrend(int show_unit, List<object> members, int point_size, int horzdevide, int vertdevide, DateTime t, int timetype, BrushPublic back_color, BrushPublic fill_color, Color guide_color, EnumDisplayFlag displayflag, bool display_cursor_time, int data_gab) 
            : base (EnumGraphType.MultiTrend, show_unit, members, point_size, horzdevide, vertdevide, t, timetype+2, back_color, fill_color, guide_color, displayflag, display_cursor_time)
        {
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

        void CalcStartTime()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (cGraphStartTimeMethod == 0)
                {
                    DateTime dt = DateTime.Now;

                    if (wTimeSelectOption == 2)
                    {	// min data
                        dtStartTime = TimeUtil.MinusMin(dt, nShowUnit);
                    }
                    else if (wTimeSelectOption == 3)
                    {	// hour data
                        dtStartTime = TimeUtil.MinusHour(dt, nShowUnit);
                    }
                    else if (wTimeSelectOption == 4)
                    {	// day data
                        dtStartTime = TimeUtil.MinusDay(dt, nShowUnit);
                    }
                    else if (wTimeSelectOption == 5)
                    {	// month data
                        dtStartTime = TimeUtil.MinusMonth(dt, nShowUnit);
                    }
                    else
                    {	// min data

                    }
                }
                else
                {

                }
            }
            else
            {
                DateTime dt = DateTime.Now;

                if (wTimeSelectOption == 2)
                {	// min data
                    dtStartTime = TimeUtil.MinusMin(dt, nShowUnit);
                }
                else if (wTimeSelectOption == 3)
                {	// hour data
                    dtStartTime = TimeUtil.MinusHour(dt, nShowUnit);
                }
                else if (wTimeSelectOption == 4)
                {	// day data
                    dtStartTime = TimeUtil.MinusDay(dt, nShowUnit);
                }
                else if (wTimeSelectOption == 5)
                {	// month data
                    dtStartTime = TimeUtil.MinusMonth(dt, nShowUnit);
                }
                else
                {	// min data

                }
            }
        }

        public override void ReadAllPoint()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT) return;

            PUBLIC_GRAPH_MEMBER member;
            int i;
            //string sRealTag;
            int datagab;

            if (wTimeSelectOption == 2) datagab = nDataGab; // 분 자료일때만 사용?
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

                    if (wTimeSelectOption == 2)
                        data_time = EnumDataTime.Minute;
                    else if (wTimeSelectOption == 3)
                        data_time = EnumDataTime.Hour;
                    else if (wTimeSelectOption == 4)
                        data_time = EnumDataTime.Day;
                    else if (wTimeSelectOption == 5)
                        data_time = EnumDataTime.Month;

                    SilverlightAutoLibLocal.ServiceReferenceDataTag2.WebServiceDataTag2SoapClient service = SilverlightAutoLibLocal.ServiceLib.GetServiceDataTag2();
                    service.GetDataAiCompleted += new EventHandler<SilverlightAutoLibLocal.ServiceReferenceDataTag2.GetDataAiCompletedEventArgs>(service_GetDataAiCompleted);
                    service.GetDataAiAsync(ai.tag, (int)data_type, (int)data_time, dtStartTime.Year, dtStartTime.Month, dtStartTime.Day, dtStartTime.Hour, dtStartTime.Minute, data_count, datagab, i);
                }
                else if (member.nType == EnumTagType.DI)
                {
                    TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);

                    EnumDataTime data_time = EnumDataTime.Minute;
                    if (wTimeSelectOption == 2)
                        data_time = EnumDataTime.Minute;
                    else if (wTimeSelectOption == 3)
                        data_time = EnumDataTime.Hour;
                    else if (wTimeSelectOption == 4)
                        data_time = EnumDataTime.Day;
                    else if (wTimeSelectOption == 5)
                        data_time = EnumDataTime.Month;

                    SilverlightAutoLibLocal.ServiceReferenceDataTag2.WebServiceDataTag2SoapClient service = SilverlightAutoLibLocal.ServiceLib.GetServiceDataTag2();
                    service.GetDataDiCompleted += new EventHandler<SilverlightAutoLibLocal.ServiceReferenceDataTag2.GetDataDiCompletedEventArgs>(service_GetDataDiCompleted);
                    service.GetDataDiAsync(di.tag, (int)EnumDataType.MOMENT, (int)data_time, dtStartTime.Year, dtStartTime.Month, dtStartTime.Day, dtStartTime.Hour, dtStartTime.Minute, data_count, datagab, i);
                }
                else
                {

                }

            }

        }

        void service_GetDataDiCompleted(object sender, SilverlightAutoLibLocal.ServiceReferenceDataTag2.GetDataDiCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled == true) return;

            if (e.Result == null)
            {
                return;
            }

            GetDataFromString(e.Result, (int)e.UserState);
        }

        void service_GetDataAiCompleted(object sender, SilverlightAutoLibLocal.ServiceReferenceDataTag2.GetDataAiCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled == true) return;

            if (e.Result == null)
            {
                return;
            }

            GetDataFromString(e.Result, (int)e.UserState);
        }

        void GetDataFromString(string data, int member_pos)
        {
            PUBLIC_GRAPH_MEMBER member;

            member = (PUBLIC_GRAPH_MEMBER)blockMember[member_pos];

            int datagab;

            if (wTimeSelectOption == 2) datagab = nDataGab;
            else datagab = 1;

            int data_count = nShowUnit / datagab;

            XDocument xd = XDocument.Parse(data);
            XElement root = xd.Root;

            int pos = 0;

            foreach (XElement el in root.Elements())
            {
                bool flag = false;
                double val = 0;

                foreach (XElement n in el.Elements())
                {
                    if (String.Compare(n.Name.LocalName, "Flag", StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        flag = n.Value == "1" ? true : false;
                    }
                    else
                    {
                        val = ConvertTool.ToDouble(n.Value);
                    }
                }

                for (int i = 0; i < datagab; i++)
                {
                    member.point[pos+i].read_flag = flag;
                    member.point[pos+i].val = val;
                }

                pos+=datagab;
            }

            string sRealTag;

            for (int i = 0; i < blockMember.Count; i++)
            {
                member = (PUBLIC_GRAPH_MEMBER)blockMember[i];
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

            if (wTimeSelectOption == 2)
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
