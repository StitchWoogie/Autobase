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

namespace SilverlightGraphicModule
{
    public class OptionMultiGraph
    {
        public sbyte bTimeDirToLeft;
        public sbyte bDisplayByTime;
        public int nDataTime;	// milli sec
    }

    public class ControlMultiGraph : ControlPublicGraph
    {
        bool bRunFlag = true;
        int nOldMilliSec = -1;
        int nRemainMilliSec = 0;
        DateTime dtLastData = DateTime.Now;
        OptionMultiGraph optionMultiGraph;

        public ControlMultiGraph(int show_unit, List<object> members, int point_size, int horzdevide, int vertdevide, DateTime t, int timetype, BrushPublic back_color, BrushPublic fill_color, Color guide_color, EnumDisplayFlag displayflag, OptionMultiGraph option, bool display_cursor_time) 
            : base (EnumGraphType.MultiGraph, show_unit, members, point_size, horzdevide, vertdevide, t, timetype, back_color, fill_color, guide_color, displayflag, display_cursor_time)
        {
            optionMultiGraph = option;
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

            if (!bRunFlag) return;

            DateTime dt = DateTime.Now;

            if (nOldMilliSec == -1)
            {	// 최초 시작
                dtLastData = new DateTime(dt.Ticks);
                nOldMilliSec = dt.Second * 1000 + dt.Millisecond;
                nRemainMilliSec = 0;
                //nBufPos = 0;

                //FillData();
                UpdateGraph();
                return;
            }

            int curMilliSec = dt.Second * 1000 + dt.Millisecond;

            if (curMilliSec < nOldMilliSec)
            {	// 분이 바뀌었다.
                nRemainMilliSec += (curMilliSec + 60000) - nOldMilliSec;
            }
            else
            {
                nRemainMilliSec += curMilliSec - nOldMilliSec;
            }

            nOldMilliSec = curMilliSec;

            int data_count = nRemainMilliSec / optionMultiGraph.nDataTime;

            if (data_count == 0) return;

            nRemainMilliSec -= optionMultiGraph.nDataTime * data_count;

            if (optionMultiGraph.bTimeDirToLeft == 1)
            {
                for (int l = 0; l < blockMember.Count; l++)
                {
                    member = (PUBLIC_GRAPH_MEMBER)blockMember[l];

                    for (i = 0; i < nShowUnit - data_count; i++)
                    {
                        member.point[i] = member.point[i + data_count];
                    }

                    double curr;

                    if (member.nType == 0)
                    {
                        // AI TAG    
                        TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.nPos);
                        curr = ai.curr;
                    }
                    else
                    {
                        TagDiClass di = TagLib.GetStructDI(member.tag, ref member.nPos);
                        curr = di.curr;
                    }

                    for (i = 0; i < data_count; i++)
                    {
                        member.point[nShowUnit - 1 - i].read_flag = true;
                        member.point[nShowUnit - 1 - i].val = curr;
                    }
                }
            }
            else
            {
                for (int l = 0; l < blockMember.Count; l++)
                {
                    member = (PUBLIC_GRAPH_MEMBER)blockMember[l];

                    for (i = nShowUnit - 1; i >= data_count; i--)
                    {
                        member.point[i] = member.point[i - data_count];
                        //member.point[i].val = member.point[i - data_count].val;
                    }

                    double curr;

                    if (member.nType == 0)
                    {
                        // AI TAG    
                        TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.nPos);
                        curr = ai.curr;
                    }
                    else
                    {
                        TagDiClass di = TagLib.GetStructDI(member.tag, ref member.nPos);
                        curr = di.curr;
                    }

                    for (i = 0; i < data_count; i++)
                    {
                        member.point[i].read_flag = true;
                        member.point[i].val = curr;
                    }
                }
            }

            if (data_count > 0)
            {
                dtLastData = new DateTime(dt.Ticks);
                SetStartTime(dtLastData);
                UpdateGraph();
            }
        }

        /*
        void FillData()
        {
            nBufPos++;
            nBufPos %= nShowUnit;

            int l;
            ANALOG_GRAPH_MEMBER member;

            for (l = 0; l < blockMember.Count; l++)
            {
                member = (ANALOG_GRAPH_MEMBER)blockMember[l];

                if (member.nType == 0)
                { 	// AI TAG
                    TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.nPos);
                    GetViewFullBase(ai, member, out member.max_value, out member.min_value);
                }
                else
                {
                    member.min_value = 0;
                    member.max_value = 100;
                }

                if (member.point != null)
                {
                    if (member.nType == 0)
                    { 	// AI TAG
                        TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.nPos);
                        member.point[nBufPos].read_flag = true;
                        member.point[nBufPos].val = ai.curr;
                    }
                    else
                    {
                        TagDiClass di = TagLib.GetStructDI(member.tag, ref member.nPos);
                        member.point[nBufPos].read_flag = true;
                        member.point[nBufPos].val = di.curr;
                    }
                }
            }

        }

        void GetViewFullBase(TagAiClass ai, ANALOG_GRAPH_MEMBER member, out double view_full, out double view_base)
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
        }*/

        // 시간글자만 바꾼다.
        protected override void UpdateTimeText(double sizex, double gab)
        {
            string buf;
            double x;

            if (optionMultiGraph.bTimeDirToLeft == 0)
            {
                DateTime t;

                int start = this.nHapLeftDisplay * 80;

                for (int i = 0; i < nHorzDevide; i++)
                {
                    x = ((i + 1) * gab) * sizex / (nShowUnit - 1) + start;

                    if (optionMultiGraph.bDisplayByTime == 1)
                    {
                        t = dtStartTime.AddMilliseconds(-gab * (i + 1) * optionMultiGraph.nDataTime);
                        buf = String.Format("{0}:{1:00}:{2:00}", t.Hour, t.Minute, t.Second);
                    }
                    else
                    {
                        buf = String.Format("{0}", (gab * (i + 1)) * optionMultiGraph.nDataTime / 1000);
                    }

                    textHorz[i].Text = buf;

                    x -= textHorz[i].ActualWidth / 2;
                    textHorz[i].Margin = new Thickness(x, 0, 0, 0);
                }

            }
            else
            {
                DateTime t;

                int start = this.nHapLeftDisplay * 80;

                for (int i = 0; i < nHorzDevide; i++)
                {
                    x = ((i + 1) * gab) * sizex / (nShowUnit - 1) + start;

                    if (optionMultiGraph.bDisplayByTime == 1)
                    {
                        t = dtStartTime.AddMilliseconds(-gab * (nHorzDevide - i) * optionMultiGraph.nDataTime);
                        buf = String.Format("{0}:{1:00}:{2:00}", t.Hour, t.Minute, t.Second);
                    }
                    else
                    {
                        buf = String.Format("{0}", (gab * (nHorzDevide - i)) * optionMultiGraph.nDataTime / 1000);
                    }

                    textHorz[i].Text = buf;

                    x -= textHorz[i].ActualWidth / 2;
                    textHorz[i].Margin = new Thickness(x, 0, 0, 0);
                }
            }
        }

        public override DateTime GetCursorTime(out string buf)
        {
            DateTime dt = new DateTime(dtStartTime.Ticks);

            int x1 = nCursorX1;
            int x2 = nCursorX2;

            if (x1 > x2) Tools.Temp(ref x1, ref x2);

            if (optionMultiGraph.bTimeDirToLeft == 0)
            {
                dt = dtStartTime.AddMilliseconds(-x1* optionMultiGraph.nDataTime);

            }
            else
            {
                dt = dtStartTime.AddMilliseconds(x1 * optionMultiGraph.nDataTime);
            }

            buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);

            /*
            if (wTimeSelectOption == 0)
            {	// min data
                dt = dt.AddMinutes(x1);
                buf = String.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);
            }
            else if (wTimeSelectOption == 1)
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
            else if (wTimeSelectOption == 2)
            {	// day data
                dt = dt.AddDays(x1);
                buf = String.Format("{0:0000}/{1:00}/{2:00}", dt.Year, dt.Month, dt.Day);
            }
            else if (wTimeSelectOption == 3)
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
            }*/

            return dt;
        }
    }
}
