using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using AutoLibLocal;
using NetTools.OldDefine;
using NetTools;

namespace SilverlightGraphicModule
{
    public enum EnumGraphType
    {
        MultiGraph,
        MultiTrend,
    }

    public partial class ControlPublicGraph : UserControl
    {
        protected int nShowUnit = 10;
        public List<object> blockMember;
        int nPointSize = 1;
        protected int nHorzDevide = 1;
        protected int nVertDevide = 1;
        Line[] lineHorz;
        protected TextBlock[] textHorz;
        Line[] lineVert;
        public DateTime dtStartTime;
        protected int wTimeSelectOption;
        BrushPublic colorBack;
        BrushPublic colorFill;
        Color colorGuide;
        EnumDisplayFlag wDisplayFlags;
        public int nCursorX1 = 0;
        public int nCursorX2 = 0;
        bool bUseLocalRange = false;
        Rectangle rectCursor;
        protected int nHapLeftDisplay;	// 왼쪽 눈금의 개수
        int nHapRightDisplay;	        // 왼쪽 눈금의 개수
        EnumGraphType enumGraphType;
        bool bDisplayPointDate = true;	// 자료시점의 날짜를 표시해 준다.

        protected bool bAutoViewRange = false;
        public sbyte cGraphStartTimeMethod;

        public ControlPublicGraph(EnumGraphType egraphtype, int show_unit, List<object> members, int point_size, int horzdevide, int vertdevide, DateTime t, int timetype, BrushPublic back_color, BrushPublic fill_color, Color guide_color, EnumDisplayFlag displayflag, bool display_cursor_time)
        {
            InitializeComponent();

            // event등록을 XAML에 했는데 상속받은 후 Initialize때 다운이 되어서 이벤트 등록을 소스에서 직접 하게 되었음 (2009.5.28)
            gridGraph.SizeChanged += new SizeChangedEventHandler(gridGraph_SizeChanged);
            gridGraph.MouseLeftButtonDown += new MouseButtonEventHandler(canvasGraph_MouseLeftButtonDown);
            gridGraph.MouseMove += new MouseEventHandler(canvasGraph_MouseMove);
            gridGraph.MouseLeftButtonUp += new MouseButtonEventHandler(canvasGraph_MouseLeftButtonUp);

            gridTagPanel.MouseLeftButtonDown += new MouseButtonEventHandler(gridTagPanel_MouseLeftButtonDown); 

            enumGraphType = egraphtype;
            nShowUnit = show_unit;
            nPointSize = point_size;
            dtStartTime = t;
            wTimeSelectOption = timetype;
            colorBack = back_color;
            colorFill = fill_color;
            colorGuide = guide_color;
            wDisplayFlags = displayflag;
            bDisplayPointDate = display_cursor_time;

            nHorzDevide = nShowUnit / horzdevide - 1;
            lineHorz = new Line[nHorzDevide];
            textHorz = new TextBlock[nHorzDevide];

            if (!IsDesX())      // X 축의 시간 표시를 하지 않는다.
            {
                this.rowDefinitionDesX.Height = new GridLength(0);
            }

            this.gridX.Children.Clear();

            for (int i = 0; i < nHorzDevide; i++)
            {
                lineHorz[i] = new Line();

                lineHorz[i].Stroke = new SolidColorBrush(Colors.LightGray);
                lineHorz[i].StrokeThickness = 1;

                this.canvasGraph.Children.Add(lineHorz[i]);

                textHorz[i] = new TextBlock();

                textHorz[i].Foreground = new SolidColorBrush(Colors.Black);
                textHorz[i].Text = "2008-3-3";

                this.gridX.Children.Add(textHorz[i]);
            }

            nVertDevide = vertdevide - 1;
            lineVert = new Line[nVertDevide];

            for (int i = 0; i < nVertDevide; i++)
            {
                lineVert[i] = new Line();

                lineVert[i].Stroke = new SolidColorBrush(Colors.LightGray);
                lineVert[i].StrokeThickness = 1;

                this.canvasGraph.Children.Add(lineVert[i]);
            }

            blockMember = members;
            SetBlock(blockMember);

            TagFile.eventHandlerOnTagFileReaded += new EventHandler(TagFile_eventHandlerOnTagFileReaded);

            MallocAllBuf();

            PUBLIC_GRAPH_MEMBER member;

            this.canvasLevelLeft.Children.Clear();
            this.canvasLevelRight.Children.Clear();

            for (int l = 0; l < blockMember.Count; l++)
            {
                member = (PUBLIC_GRAPH_MEMBER)blockMember[l];

                MakeGraphElement(member);
                MakeLevelElement(member, l);
            }

            rectCursor = new Rectangle();
            rectCursor.Fill = new SolidColorBrush(Color.FromArgb(0x80, colorGuide.R, colorGuide.G, colorGuide.B));
            this.canvasGraph.Children.Add(rectCursor);

            MakeTagPanel();

            SetBackColor(back_color);
            this.gridGraph.Background = ObjectRectangle.MakePublicBrush(colorFill);

            UpdateValue();

            if (NetTools.Tools.IsLangKorean())
            {
                textBlockTag.Text = "태그명";
                textBlockMax.Text = "최대값";
                textBlockMin.Text = "최소값";
                textBlockCurr.Text = "현재값";
                textBlockData.Text = "자료값";
            }
        }

        // 태그를 다 읽었을 때는 태그 형식을 다시 Update한다.
        void TagFile_eventHandlerOnTagFileReaded(object sender, EventArgs e)
        {
            PUBLIC_GRAPH_MEMBER member;

            for (int i = 0; i < blockMember.Count; i++)
            {
                member = (PUBLIC_GRAPH_MEMBER)blockMember[i];

                //TagLib.GetTagTypeAndPos(member.tag, ref member.nType, ref member.nPos);
                SetBlockOne(member);            // 
                UpdateLevelText(member);        // 범위표시 글자를 다시 계산한다.
            }

            UpdateValue();      // 태그 판넬도 태그와 관련이 있다.
            //UpdateGraph();
        }

        /// <summary>
        /// 선과 점을 표시할 엘리먼트를 만든다.
        /// </summary>
        /// <param name="member"></param>
        void MakeGraphElement(PUBLIC_GRAPH_MEMBER member)
        {
            member.lines = new Line[nShowUnit];
            member.points = new FrameworkElement[nShowUnit];

            // 그래프의 선을 추가한다.
            for (int i = 0; i < nShowUnit; i++)
            {
                member.lines[i] = new Line();
                member.lines[i].Stroke = new SolidColorBrush(member.color);
                member.lines[i].StrokeThickness = member.nLineThick;

                this.canvasGraph.Children.Add(member.lines[i]);
            }

            // 그래프의 꼭지점을 추가 Point가 선의 위에 와야 하므로 나중에 추가한다.
            for (int i = 0; i < nShowUnit; i++)
            {
                Ellipse point = new Ellipse();

                point = new Ellipse();
                point.Width = nPointSize;
                point.Height = nPointSize;
                point.Fill = new SolidColorBrush(member.color);
                member.points[i] = point;

                this.canvasGraph.Children.Add(member.points[i]);
            }
        }

        void MakeLevelElement(PUBLIC_GRAPH_MEMBER member, int l)
        {
            // 그래프의 레벨표시를 추가.
            if (member.nAxisPosition > 0)
            {
                // int pos;
                if (member.nAxisPosition == 1)
                {
                    Border border = MakeLevelBoard(member);

                    member.borderLevel = border;

                    canvasLevelLeft.Children.Add(border);
                }
                else if (member.nAxisPosition == 2)
                {
                    Border border = MakeLevelBoard(member);

                    member.borderLevel = border;

                    canvasLevelRight.Children.Add(border);
                }
                else
                {

                }
            }
        }

        public void SetBackColor(BrushPublic color)
        {
            colorBack = color;
            this.LayoutRoot.Background = ObjectRectangle.MakePublicBrush(colorBack);
        }

        StackPanel MakeOneTagPanel(PUBLIC_GRAPH_MEMBER member)
        {
            StackPanel panel = new StackPanel();

            member.textValue = new TextBlock[5];

            for (int i = 0; i < 5; i++)
            {
                Border border = new Border();
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = new SolidColorBrush(Colors.DarkGray);
                border.Background = new SolidColorBrush(Colors.LightGray);
                border.Height = 18;

                TextBlock text = new TextBlock();

                member.textValue[i] = text;

                if (i == 0)
                {
                    text.Text = GetTextTagPanelTag(member);
                    border.Background = ObjectRectangle.MakePublicBrush(colorFill);
                    text.Foreground = new SolidColorBrush(member.color);
                }
                else
                {
                    text.Text = "100";
                }

                text.HorizontalAlignment = HorizontalAlignment.Center;

                border.Child = text;

                panel.Children.Add(border);
            }

            return panel;
        }

        void MakeTagPanelOne(PUBLIC_GRAPH_MEMBER member, int i)
        {
            ColumnDefinition column;

            column = new ColumnDefinition();

            column.Width = new GridLength(100);

            this.gridTagPanel.ColumnDefinitions.Add(column);

            StackPanel panel = MakeOneTagPanel(member);
            this.gridTagPanel.Children.Add(panel);
            Grid.SetColumn(panel, i + 2);

            member.tagPanel = panel;
        }

        void MakeTagPanel()
        {
            PUBLIC_GRAPH_MEMBER member;

            this.gridTagPanel.Children.Clear();
            this.gridTagPanel.ColumnDefinitions.Clear();

            ColumnDefinition column = new ColumnDefinition();
            column.Width = new GridLength(6);
            this.gridTagPanel.ColumnDefinitions.Add(column);

            column = new ColumnDefinition();
            column.Width = new GridLength(70);
            this.gridTagPanel.ColumnDefinitions.Add(column);

            this.gridTagPanel.Children.Add(panelTagInfo);
            Grid.SetColumn(panelTagInfo, 1);

            int size = 0;
            if (IsTagColor())
            {
                size += 18;
            }
            else
            {
                this.borderTagInfoTag.Height = 0;
            }

            if (IsTagMinMax())
            {
                size += 36;
            }
            else
            {
                this.borderTagInfoMax.Height = 0;
                this.borderTagInfoMin.Height = 0;
            }

            if (IsTagCurr())
            {
                size += 18;
            }
            else
            {
                this.borderTagInfoCurr.Height = 0;
            }

            if (IsTagOldCurr())
            {
                size += 18;
            }
            else
            {
                this.borderTagInfoData.Height = 0;
            }

            if (size > 0)
            {
                this.rowDefinitionTagPanel.Height = new GridLength(size + 5);
            }
            else
            {
                this.rowDefinitionTagPanel.Height = new GridLength(0);
            }

            for (int i = 0; i < blockMember.Count; i++)
            {
                member = (PUBLIC_GRAPH_MEMBER)blockMember[i];

                MakeTagPanelOne(member, i);
            }
        }

        void UpdateLevelText(PUBLIC_GRAPH_MEMBER member)
        {
            string buf;
            double view_full;
            double view_base;
            PUBLIC_GRAPH_MEMBER memberDisplay;	// 이것은 태그의 위치에서 오른쪽버튼을 누르면 첫번째의 레벨값을 해당 태그로 바꾸어 주는 기능때문에 필요하다.
            double val;

            memberDisplay = member;

            view_full = memberDisplay.max_value;
            view_base = memberDisplay.min_value;

            // 제일 아래와 제일 위에 1개씩 추가 되어서 분할 보다 2개를 더 만든다.
            for (int i = 0; i < nVertDevide + 2; i++)
            {
                val = (view_full - view_base) * i / (nVertDevide + 1) + view_base;

                if (member.nType == 0)
                {
                    TagAiClass ai = GetRealTagAI(memberDisplay.tag, ref memberDisplay.nPos);
                    buf = TagUtil.AiValueToStringOnlyPoint(ai, TagUtil.GetDisplayValue(ai, val));
                }
                else
                {
                    TagDiClass di = GetRealTagDI(memberDisplay.tag, ref memberDisplay.nPos);
                    if (i == 0) buf = TagUtil.GetDesOFF(di);
                    else if (i == nVertDevide + 1) buf = TagUtil.GetDesON(di);
                    else buf = "";	// empty
                }

                if (member.textLevel != null)   // 아직 초기화가 안된 상태에서 태그 로딩이 먼저 되는 경우
                {
                    member.textLevel[i].Text = buf;
                }
            }
        }

        Border MakeLevelBoard(PUBLIC_GRAPH_MEMBER member)
        {
            Border border = new Border();
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.BorderThickness = new Thickness(1);
            border.Background = ObjectRectangle.MakePublicBrush(colorFill);

            Grid grid = new Grid();
            border.Child = grid;

            PUBLIC_GRAPH_MEMBER memberDisplay;	// 이것은 태그의 위치에서 오른쪽버튼을 누르면 첫번째의 레벨값을 해당 태그로 바꾸어 주는 기능때문에 필요하다.

            memberDisplay = member;
            TagAiClass ai = null;
            TagDiClass di = null;
            double view_full;
            double view_base;

            if (member.nType == 0)
            {
                ai = GetRealTagAI(memberDisplay.tag, ref memberDisplay.nPos);

                // 여기서 GetViewFullBase를 사용해야 할 듯 하나 8.xx 소스와 같게 하기 위해서 그냥 둠 대신 
                //GetViewFullBase(ai, memberDisplay, out view_full, out view_base);
                view_full = memberDisplay.max_value;
                view_base = memberDisplay.min_value;
            }
            else
            {	// DIGITAL INPUT tag
                di = GetRealTagDI(memberDisplay.tag, ref memberDisplay.nPos);
                view_full = memberDisplay.max_value;
                view_base = memberDisplay.min_value;
            }

            //string buf;
            //double val;

            member.textLevel = new TextBlock[nVertDevide + 2];

            // 제일 아래와 제일 위에 1개씩 추가 되어서 분할 보다 2개를 더 만든다.
            for (int i = 0; i < nVertDevide + 2; i++)
            {
                //val = (view_full - view_base) * i / (nVertDevide + 1) + view_base;

                TextBlock text = new TextBlock();
                member.textLevel[i] = text;

                text.HorizontalAlignment = HorizontalAlignment.Right;
                text.Foreground = new SolidColorBrush(member.color);

                /*
                if (member.nType == 0)
                {
                    buf = TagUtil.AiValueToStringOnlyPoint(ai, TagUtil.GetDisplayValue(ai, val));
                }
                else
                {
                    if (i == 0) buf = TagUtil.GetDesOFF(di);
                    else if (i == nVertDevide+1) buf = TagUtil.GetDesON(di);
                    else buf = "";	// empty
                }
                
                text.Text = buf;*/

                grid.Children.Add(text);
            }

            UpdateLevelText(member);

            return border;
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

        // 시간글자만 바꾼다. 과거 트랜드 형
        protected virtual void UpdateTimeText(double sizex, double gab)
        {
            double x;
            DateTime t;

            int start = this.nHapLeftDisplay * 80;

            if (wTimeSelectOption == 0)
            {
                int save_day = -1;
                for (int i = 0; i < nHorzDevide; i++)
                {
                    x = ((i + 1) * gab) * sizex / (nShowUnit - 1) + start;

                    t = dtStartTime.AddMilliseconds(gab * (i + 1));
                    textHorz[i].Text = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", t.Hour, t.Minute, t.Second, t.Millisecond);

                    if (save_day != t.Day)
                    {
                        save_day = t.Day;
                        textHorz[i].Text += String.Format("\n{0}/{1}", t.Month, t.Day);
                    }

                    x -= textHorz[i].ActualWidth / 2;
                    textHorz[i].Margin = new Thickness(x, 0, 0, 0);
                }
            }
            else if (wTimeSelectOption == 1)
            {
                int save_day = -1;
                for (int i = 0; i < nHorzDevide; i++)
                {
                    x = ((i + 1) * gab) * sizex / (nShowUnit - 1) + start;

                    t = dtStartTime.AddSeconds(gab * (i + 1));
                    textHorz[i].Text = String.Format("{0:00}:{1:00}:{2:00}", t.Hour, t.Minute, t.Second);

                    if (save_day != t.Day)
                    {
                        save_day = t.Day;
                        textHorz[i].Text += String.Format("\n{0}/{1}", t.Month, t.Day);
                    }

                    x -= textHorz[i].ActualWidth / 2;
                    textHorz[i].Margin = new Thickness(x, 0, 0, 0);
                }
            }
            if (wTimeSelectOption == 2)
            {
                int save_day = -1;
                for (int i = 0; i < nHorzDevide; i++)
                {
                    x = ((i + 1) * gab) * sizex / (nShowUnit - 1) + start;

                    t = dtStartTime.AddMinutes(gab * (i + 1));
                    textHorz[i].Text = String.Format("{0:00}:{1:00}", t.Hour, t.Minute);

                    if (save_day != t.Day)
                    {
                        save_day = t.Day;
                        textHorz[i].Text += String.Format("\n{0}/{1}", t.Month, t.Day);
                    }

                    x -= textHorz[i].ActualWidth / 2;
                    textHorz[i].Margin = new Thickness(x, 0, 0, 0);
                }
            }
            else if (wTimeSelectOption == 3)    // 시간 데이터.
            {
                int save_day = 0;

                for (int i = 0; i < nHorzDevide; i++)
                {
                    x = ((i + 1) * gab) * sizex / (nShowUnit - 1) + start;

                    //textHorz[i].Margin = new Thickness(x, 0, 0, 0);
                    t = dtStartTime.AddHours(gab * (i + 1));
                    textHorz[i].Text = String.Format("{0:00}:{1:00}", t.Hour, 0);

                    if (save_day != t.Day)
                    {
                        save_day = t.Day;

                        textHorz[i].Text += String.Format("\n{0}/{1}", t.Month, t.Day);
                    }

                    x -= textHorz[i].ActualWidth / 2;
                    textHorz[i].Margin = new Thickness(x, 0, 0, 0);
                }
            }
            else if (wTimeSelectOption == 4)
            {	// 일별 데이터.
                int save_month = 0;

                for (int i = 0; i < nHorzDevide; i++)
                {
                    x = ((i + 1) * gab) * sizex / (nShowUnit - 1) + start;

                    //textHorz[i].Margin = new Thickness(x, 0, 0, 0);
                    t = dtStartTime.AddDays(gab * (i + 1));
                    textHorz[i].Text = String.Format("{0:00}:{1:00}", t.Hour, 0);

                    if (save_month != t.Month)
                    {
                        save_month = t.Month;

                        textHorz[i].Text += String.Format("\n{0}/{1}", t.Year, t.Month);
                    }

                    x -= textHorz[i].ActualWidth / 2;
                    textHorz[i].Margin = new Thickness(x, 0, 0, 0);
                }
            }
            else if (wTimeSelectOption == 5)
            {		// 월별 데이터.
                int save_year = 0;

                for (int i = 0; i < nHorzDevide; i++)
                {
                    x = ((i + 1) * gab) * sizex / (nShowUnit - 1) + start;

                    //textHorz[i].Margin = new Thickness(x, 0, 0, 0);
                    t = dtStartTime.AddMonths((int)(gab * (i + 1)));
                    textHorz[i].Text = String.Format("{0:00}:{1:00}", t.Hour, 0);

                    if (save_year != t.Year)
                    {
                        save_year = t.Year;

                        textHorz[i].Text += String.Format("\n{0}", t.Year);
                    }

                    x -= textHorz[i].ActualWidth / 2;
                    textHorz[i].Margin = new Thickness(x, 0, 0, 0);
                }
            }
        }

        void UpdateX()
        {
            double gy1 = 0, gx2 = canvasGraph.ActualWidth - 1, gy2 = canvasGraph.ActualHeight - 1;

            double sizex = canvasGraph.ActualWidth;
            double sizey = canvasGraph.ActualHeight;

            double x;
            //double gab = nShowUnit / (nHorzDevide + 1);
            double gab = (nShowUnit+nHorzDevide) / (nHorzDevide + 1);

            // 그래프 밑에 Guide라인을 그린다.
            for (int i = 0; i < nHorzDevide; i++)
            {
                //x = (i + 1) * sizex / (nHorzDevide + 1);
                x = ((i+1)*gab) * sizex / (nShowUnit-1);

                lineHorz[i].X1 = x;
                lineHorz[i].Y1 = gy1;
                lineHorz[i].X2 = x;
                lineHorz[i].Y2 = gy2;
            }

            UpdateTimeText(sizex, gab);
        }

        void UpdateY()
        {
            double gx1 = 0, gy1 = 0, gx2 = canvasGraph.ActualWidth - 1, gy2 = canvasGraph.ActualHeight - 1;

            double sizex = canvasGraph.ActualWidth;
            double sizey = canvasGraph.ActualHeight;

            double y;

            for (int i = 0; i < nVertDevide; i++)
            {
                y = (i + 1) * sizey / (nVertDevide + 1);

                lineVert[i].X1 = gx1;
                lineVert[i].Y1 = y;
                lineVert[i].X2 = gx2;
                lineVert[i].Y2 = y;
            }

            PUBLIC_GRAPH_MEMBER member;
            //ANALOG_GRAPH_MEMBER memberDisplay;	// 이것은 태그의 위치에서 오른쪽 버튼을 누르면
            int l;
            RECT rZone = new RECT();

            for (l = 0; l < blockMember.Count; l++)
            {
                member = (PUBLIC_GRAPH_MEMBER)blockMember[l];
                //memberDisplay = (ANALOG_GRAPH_MEMBER)blockMember[l];


                if (member.nAxisPosition == 0) continue;   // 디스프레이 판넬이 없다.
                if (member.nType != EnumTagType.AI && member.nType != EnumTagType.DI) continue;

                FrameworkElement levelPanel = member.borderLevel;// ((FrameworkElement)text.Parent);

                if (member.nAxisPosition == 1)
                {
                    gx2 = canvasLevelLeft.ActualWidth - 1;
                    gy2 = canvasLevelLeft.ActualHeight - 1;

                    rZone.right = (int)(gx2 - (member.nAxisCalcPos * (80)) - 0);
                    rZone.left = rZone.right - (80) + 0;
                }
                else
                {
                    gx2 = canvasLevelRight.ActualWidth - 1;
                    gy2 = canvasLevelRight.ActualHeight - 1;

                    rZone.left = (int)(gx1 + (member.nAxisCalcPos * (80)) + 0);
                    rZone.right = rZone.left + (80) - 0;
                }

                rZone.top = (int)(gy2 - (member.nLevelTo) * (gy2 - gy1) / 100);
                rZone.bottom = (int)(gy2 - (member.nLevelFrom) * (gy2 - gy1) / 100);

                Canvas.SetLeft(levelPanel, rZone.left);
                Canvas.SetTop(levelPanel, rZone.top);
                levelPanel.Width = rZone.right - rZone.left + 1;
                levelPanel.Height = rZone.bottom - rZone.top + 1;

                // 각 글자의 위치를 재정의
                for (int i = 0; i < member.textLevel.Length; i++)
                {
                    TextBlock text = member.textLevel[i];

                    if (member.bReverseY == 1)
                    {
                        if (i == 0)
                        {
                            text.VerticalAlignment = VerticalAlignment.Top;
                        }
                        else if (i == member.textLevel.Length - 1)
                        {
                            text.VerticalAlignment = VerticalAlignment.Bottom;
                        }
                        else
                        {
                            double pos = ((FrameworkElement)text.Parent).ActualHeight * i / (member.textLevel.Length - 1);
                            pos -= FontSize / 2;
                            text.Margin = new Thickness(0, pos, 0, 0);
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            text.VerticalAlignment = VerticalAlignment.Bottom;
                        }
                        else if (i == member.textLevel.Length - 1)
                        {
                            text.VerticalAlignment = VerticalAlignment.Top;
                        }
                        else
                        {
                            double pos = ((FrameworkElement)text.Parent).ActualHeight * i / (member.textLevel.Length - 1);
                            pos = ((FrameworkElement)text.Parent).ActualHeight - pos;
                            pos -= FontSize / 2;
                            text.Margin = new Thickness(0, pos, 0, 0);
                            //text.VerticalAlignment = VerticalAlignment.Center;
                        }
                    }
                }
            }
        }

        public void UpdateGraph()
        {
            double sizex, sizey;
            double x, y = 0;
            double gaby;
            ushort j;
            int l;
            ushort pos;
            PUBLIC_GRAPH_MEMBER member;
            TagAiClass ai = null;
            double view_y1;
            double view_y2;
            double view_sizey;
            bool moveto_flag = false;
            double move_x1 = 0, move_y1 = 0;
            double result_y;

            double gx1 = 0, gy1 = 0, gx2 = canvasGraph.ActualWidth - 1, gy2 = canvasGraph.ActualHeight - 1;

            sizex = canvasGraph.ActualWidth;
            sizey = canvasGraph.ActualHeight;

            double point_radios = (gy2 - gy1 + 1) * nPointSize / 1000;
            point_radios *= 2;
            if (point_radios < 1) point_radios = 1;

            for (l = 0; l < blockMember.Count; l++)
            {
                member = (PUBLIC_GRAPH_MEMBER)blockMember[l];
                if (member.visible == 0) continue;

                TagLib.GetTagTypeAndPos(member.tag, ref member.nType, ref member.nPos);

                if (member.nType == EnumTagType.AI)
                {
                    ai = GetRealTagAI(member.tag, ref member.nPos);
                    gaby = member.max_value - member.min_value;
                }
                else if (member.nType == EnumTagType.DI)
                {
                    gaby = 100;
                }
                else
                {
                    continue;
                }

                view_y1 = gy2 - (member.nLevelTo) * (gy2 - gy1) / 100;
                view_y2 = gy2 - (member.nLevelFrom) * (gy2 - gy1) / 100;
                view_sizey = view_y2 - view_y1 + 1;

                if (member.nType == EnumTagType.AI)
                {	// AI 일때만 경계치 표시
                    if ((member.wFlags & 0x0001) > 0)
                    {	// hihi
                        if (gaby == 0)	// protect divide by zero
                            y = 0;
                        else
                            y = (int)((double)view_sizey * (ai.hihi - member.min_value) / gaby);
                        if (y < 0) y = 0;
                        if (y >= view_sizey) y = view_sizey - 1;

                        if (member.bReverseY != 0)
                            y = view_y1 + y;
                        else
                            y = view_y2 - y;

                        //g.DrawLine(hPenLimit, gx1 + 1, y, gx2, y);
                    }
                    if ((member.wFlags & 0x0002) > 0)
                    {	// hihi
                        if (gaby == 0)	// protect divide by zero
                            y = 0;
                        else
                            y = (int)((double)view_sizey * (ai.high - member.min_value) / gaby);
                        if (y < 0) y = 0;
                        if (y >= view_sizey) y = view_sizey - 1;

                        if (member.bReverseY != 0)
                            y = view_y1 + y;
                        else
                            y = view_y2 - y;

                        //g.DrawLine(hPenLimit, gx1 + 1, y, gx2, y);
                    }
                    if ((member.wFlags & 0x0004) > 0)
                    {	// hihi
                        if (gaby == 0)	// protect divide by zero
                            y = 0;
                        else
                            y = (int)((double)view_sizey * (ai.low - member.min_value) / gaby);
                        if (y < 0) y = 0;
                        if (y >= view_sizey) y = view_sizey - 1;

                        if (member.bReverseY != 0)
                            y = view_y1 + y;
                        else
                            y = view_y2 - y;

                        //g.DrawLine(hPenLimit, gx1 + 1, y, gx2, y);
                    }
                    if ((member.wFlags & 0x0008) > 0)
                    {	// hihi
                        if (gaby == 0)	// protect divide by zero
                            y = 0;
                        else
                            y = (int)((double)view_sizey * (ai.lolo - member.min_value) / gaby);
                        if (y < 0) y = 0;
                        if (y >= view_sizey) y = view_sizey - 1;

                        if (member.bReverseY != 0)
                            y = view_y1 + y;
                        else
                            y = view_y2 - y;

                        //g.DrawLine(hPenLimit, gx1 + 1, y, gx2, y);
                    }
                }

                pos = 0;
                moveto_flag = false;

                for (j = 0; j < nShowUnit; j++, pos++)
                {
                    if (nShowUnit - 1 == 0)	// protect device by zero
                        x = 0;
                    else
                        x = (int)((long)sizex * j / (nShowUnit - 1));

                    if (gaby == 0)	// protect divide by zero
                        y = 0;
                    else
                    {
                        if (member.point[j].read_flag)
                        {	// 값을 읽었을 때만
                            if (member.nType == EnumTagType.AI)
                            {	// AI
                                y = (int)((double)view_sizey * (TagUtil.GetDisplayValue(ai, member.point[j].val) - member.min_value) / gaby);
                            }
                            else if (member.nType == EnumTagType.DI)
                            {
                                if (member.point[j].val > 0)
                                    y = (int)(view_sizey * 0.9);
                                else
                                    y = (int)(view_sizey * 0.1);
                            }
                            else
                            {
                                y = 1;
                            }
                        }
                    }

                    if (y < 0) y = 0;
                    if (y >= view_sizey) y = view_sizey - 1;

                    if (member.point[j].read_flag)
                    {
                        if (member.bReverseY == 1)
                            result_y = view_y1 + y;
                        else
                            result_y = view_y2 - y;

                        if (moveto_flag == false)
                        {
                            move_x1 = x + gx1;
                            move_y1 = result_y;
                            moveto_flag = true;
                            member.lines[j].Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            member.lines[j].X1 = move_x1;
                            member.lines[j].Y1 = move_y1;

                            move_x1 = x + gx1;
                            move_y1 = result_y;

                            member.lines[j].X2 = move_x1;
                            member.lines[j].Y2 = move_y1;

                            member.lines[j].Visibility = Visibility.Visible;
                        }

                        if (member.nPointType == 0)
                        {
                            member.points[j].Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            member.points[j].Visibility = Visibility.Visible;

                            Canvas.SetLeft(member.points[j], x + gx1 - point_radios / 2);
                            Canvas.SetTop(member.points[j], result_y - point_radios / 2);
                            member.points[j].Width = point_radios;
                            member.points[j].Height = point_radios;
                        }

                        if (member.nPointType == 0)
                        {
                            //g.DrawLine(pen, x + gx1, result_y, x + gx1, result_y);
                        }
                        else
                        {
                            //DisplayPoint(g, x + gx1, result_y, member.color, member.nPointType, point_radios);
                        }
                    }
                    else
                    {
                        member.points[j].Visibility = Visibility.Collapsed;
                        member.lines[j].Visibility = Visibility.Collapsed;
                        moveto_flag = false;
                    }
                }
            }

            UpdateX();
            UpdateY();

            SetCursorPos();

            UpdateCursorTime();
        }

        void UpdateCursorTime()
        {
            if (bDisplayPointDate)
            {
                rowDefinitionDisplayPointDate.Height = new GridLength(16);
            }
            else
            {
                rowDefinitionDisplayPointDate.Height = new GridLength(0);
                return;
            }

            string buf;
            
            GetCursorTime(out buf);

            this.textBlockCursorTime.Text = buf;

            int x1 = nCursorX1;
            int x2 = nCursorX2;

            if (x1 > x2) Tools.Temp(ref x1, ref x2);

            double gx1 = 0, gx2 = canvasGraph.ActualWidth - 1;

            x1 = (int)(x1 * (gx2 - gx1) / (nShowUnit - 1));

            int start = (int)(this.nHapLeftDisplay * 80+x1-textBlockCursorTime.ActualWidth/2);

            if (start < 0) start = 0;
            if (start + textBlockCursorTime.ActualWidth > gridCursorTime.ActualWidth) start = (int)(gridCursorTime.ActualWidth - textBlockCursorTime.ActualWidth);

            this.textBlockCursorTime.Margin = new Thickness(start, 0, 0, 0);
        }

        public virtual DateTime GetCursorTime(out string buf)
        {
            buf = "GetCursorTime을 오버라이드 할것";
            return DateTime.Now;
        }

        void SetCursorPos()
        {
            double gx1 = 0, gx2 = canvasGraph.ActualWidth - 1;

            rectCursor.Height = canvasGraph.ActualHeight;

            if (nCursorX1 == nCursorX2)
            {
                int x1 = (int)(nCursorX1 * (gx2 - gx1) / (nShowUnit - 1));

                Canvas.SetLeft(rectCursor, x1);
                Canvas.SetTop(rectCursor, 0);

                rectCursor.Width = 2;
            }
            else
            {
                int x1, x2;
                x1 = nCursorX1;
                x2 = nCursorX2;
                if (x1 > x2) Tools.Temp(ref x1, ref x2);

                x1 = (int)(x1 * (gx2 - gx1) / (nShowUnit - 1));
                x2 = (int)(x2 * (gx2 - gx1) / (nShowUnit - 1));

                rectCursor.Width = x2 - x1 + 1;

                Canvas.SetLeft(rectCursor, x1);
                Canvas.SetTop(rectCursor, 0);
            }
        }

        private void gridGraph_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateGraph();
        }

        bool IsDesX() { return ((wDisplayFlags & EnumDisplayFlag.DESX) == 0 ? false : true); } // X축의 설명이 있는냐?
        bool IsDesY() { return ((wDisplayFlags & EnumDisplayFlag.DESY) == 0 ? false : true); } // Y축의 설명이 있는냐?
        bool IsGuideLine() { return ((wDisplayFlags & EnumDisplayFlag.GUIDELINE) == 0 ? false : true); }	// Guide Line이 있느냐.
        bool IsTagColor() { return ((wDisplayFlags & EnumDisplayFlag.TAG_COLOR) == 0 ? false : true); }	// 태그 안내판 표시.
        bool IsTagCurr() { return ((wDisplayFlags & EnumDisplayFlag.TAG_CURR) == 0 ? false : true); }
        bool IsTagOldCurr() { return ((wDisplayFlags & EnumDisplayFlag.TAG_OLD_CURR) == 0 ? false : true); }
        bool IsTagMinMax() { return ((wDisplayFlags & EnumDisplayFlag.TAG_MIN_MAX) == 0 ? false : true); }
        bool IsBackBorder() { return ((wDisplayFlags & EnumDisplayFlag.BACK_BORDER) == 0 ? false : true); }
        bool IsByDescription() { return ((wDisplayFlags & EnumDisplayFlag.BY_DESCRIPTION) == 0 ? false : true); }

        string GetTextTagPanelTag(PUBLIC_GRAPH_MEMBER member)
        {
            string buf;

            if (IsByDescription())
            {
                if (member.nType == EnumTagType.AI)
                {
                    TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
                    if (ai.description.Length == 0)
                        buf = ai.tag;
                    else
                        buf = ai.description;
                }
                else if (member.nType == EnumTagType.DI)
                {
                    TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);
                    if (di.description.Length == 0)
                        buf = di.tag;
                    else
                        buf = di.description;
                }
                else
                {
                    buf = member.tag;
                }
            }
            else
            {
                if (member.nType == EnumTagType.AI)
                {
                    TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
                    buf = ai.tag;
                }
                else if (member.nType == EnumTagType.DI)
                {
                    TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);
                    buf = di.tag;
                }
                else
                {
                    buf = member.tag;
                }
            }

            return buf;
        }

        void UpdateValue()
        {
            int l;
            PUBLIC_GRAPH_MEMBER member;
            string buf;
            double val;
            double min, max;

            for (l = 0; l < blockMember.Count; l++)
            {
                member = (PUBLIC_GRAPH_MEMBER)blockMember[l];

                if (IsTagColor())
                {
                    buf = GetTextTagPanelTag(member);
                    /*
                    if (IsByDescription())
                    {
                        if (member.nType == EnumTagType.AI)
                        {
                            TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
                            if (ai.description.Length == 0)
                                buf = ai.tag;
                            else
                                buf = ai.description;
                        }
                        else if (member.nType == EnumTagType.DI)
                        {
                            TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);
                            if (di.description.Length == 0)
                                buf = di.tag;
                            else
                                buf = di.description;
                        }
                        else
                        {
                            buf = member.tag;
                        }
                    }
                    else
                    {
                        if (member.nType == EnumTagType.AI)
                        {
                            TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
                            buf = ai.tag;
                        }
                        else if (member.nType == EnumTagType.DI)
                        {
                            TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);
                            buf = di.tag;
                        }
                        else
                        {
                            buf = member.tag;
                        }
                    }*/

                    member.textValue[0].Text = buf;
                }
                if (IsTagMinMax())
                {
                    if (member.nType == 0)
                    {
                        TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
                        if (nCursorX1 == nCursorX2)
                        {
                            GetViewFullBase(ai, member, out max, out min);
                            buf = TagUtil.AiValueToString(ai, max);
                        }
                        else
                        {
                            GetCursorValue(member, out val, out min, out max);
                            buf = TagUtil.AiValueToString(ai, max);
                        }
                    }
                    else
                    {
                        buf = "";
                    }

                    member.textValue[1].Text = buf;

                    if (member.nType == 0)
                    {
                        TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
                        if (nCursorX1 == nCursorX2)
                        {
                            GetViewFullBase(ai, member, out max, out min);
                            buf = TagUtil.AiValueToString(ai, min);
                        }
                        else
                        {
                            GetCursorValue(member, out val, out min, out max);
                            buf = TagUtil.AiValueToString(ai, min);
                        }
                    }
                    else
                    {
                        buf = "";
                    }

                    member.textValue[2].Text = buf;
                }
                if (IsTagCurr())
                {
                    if (member.nType == EnumTagType.AI)
                    {
                        TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
                        buf = TagUtil.AiValueToString(ai, ai.curr);
                    }
                    else if (member.nType == EnumTagType.DI)
                    {
                        TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);
                        if (di.curr == 1) buf = TagUtil.GetDesON(di);
                        else buf = TagUtil.GetDesOFF(di);


                    }
                    else
                    {
                        buf = "???";
                    }

                    member.textValue[3].Text = buf;
                }
                if (IsTagOldCurr())
                {
                    if (GetCursorValue(member, out val, out min, out max))
                    {
                        if (member.nType == EnumTagType.AI)
                        {
                            TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
                            buf = TagUtil.AiValueToString(ai, val);
                        }
                        else
                        {
                            TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);
                            if (val > 0.5) buf = TagUtil.GetDesON(di);
                            else buf = TagUtil.GetDesOFF(di);
                        }
                    }
                    else
                    {
                        buf = "***";
                    }

                    member.textValue[4].Text = buf;
                }
            }
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

        bool GetCursorValue(PUBLIC_GRAPH_MEMBER member, out double curr, out double min, out double max)
        {
            curr = 0;
            min = 0;
            max = 100;

            //if(member.point == null)			return false;

            int x1, x2;

            x1 = nCursorX1;
            x2 = nCursorX2;

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (x1 < 0 || x1 >= nShowUnit) return false;
            if (x2 < 0 || x2 >= nShowUnit) return false;

            return GetZoneValue(member, out curr, out min, out max, x1, x2);
        }

        bool GetZoneValue(PUBLIC_GRAPH_MEMBER member, out double curr, out double min, out double max, int from, int to)
        {
            curr = 0;
            min = 0;
            max = 100;

            if (member.nType == EnumTagType.AI)
            {
                TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
                GetViewFullBase(ai, member, out max, out min);
            }

            // if(member.point == null)			return false;
            //if(dsData.Tables.IndexOf(member.tag) == -1)	return;

            bool retn = false;

            if (from == to)
            {
                int pos;

                pos = from;

                if (pos < 0 || pos >= nShowUnit) return false;

                curr = member.point[pos].val;
                retn = member.point[pos].read_flag;
            }
            else
            {
                int count = 0;
                int i;

                if (from > to) Tools.Temp(ref from, ref to);
                if (from < 0 || from >= nShowUnit) return false;
                if (to < 0 || to >= nShowUnit) return false;

                //DataRow row;
                float val;

                for (i = from; i <= to; i++)
                {
                    //row = member.ds.Tables[0].Rows[i];

                    if (!member.point[i].read_flag) continue;
                    val = (float)member.point[i].val;
                    if (count == 0)
                    {
                        curr = val;
                        min = curr;
                        max = curr;
                    }
                    else
                    {

                        curr += val;
                        if (val < min) min = val;
                        if (val > max) max = val;
                    }
                    count++;
                }

                if (count > 0)
                {
                    curr = curr / count;
                    retn = true;
                }
            }

            return retn;
        }

        public virtual void Timer()
        {
            int l;
            PUBLIC_GRAPH_MEMBER member;
            string buf;

            if (IsTagCurr())
            {

                for (l = 0; l < blockMember.Count; l++)
                {
                    member = (PUBLIC_GRAPH_MEMBER)blockMember[l];

                    if (member.nType == EnumTagType.AI)
                    {
                        TagAiClass ai = GetRealTagAI(member.tag, ref member.nPos);
                        buf = TagUtil.AiValueToString(ai, ai.curr);
                    }
                    else if (member.nType == EnumTagType.DI)
                    {
                        TagDiClass di = GetRealTagDI(member.tag, ref member.nPos);
                        if (di.curr == 1) buf = TagUtil.GetDesON(di);
                        else buf = TagUtil.GetDesOFF(di);


                    }
                    else
                    {
                        buf = "???";
                    }

                    if (member.textValue[3].Text != buf)
                        member.textValue[3].Text = buf;
                }
            }
        }

        private void canvasGraph_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (bCaptureMouse) return;
            Point p = e.GetPosition(gridGraph);

            int mx = (int)p.X;
            int my = (int)p.Y;
            double gx1 = 0, gx2 = canvasGraph.ActualWidth - 1;

            int pos = (int)((float)(nShowUnit - 1) * (mx - gx1) / (gx2 - gx1) + 0.5);
            nCursorX1 = pos;
            nCursorX2 = pos;
            //InvalidateObject(form);
            bCaptureMouse = true;
            gridGraph.CaptureMouse();
            SetCursorPos();
            UpdateValue();
            UpdateCursorTime();
        }

        bool bCaptureMouse = false;

        private void canvasGraph_MouseMove(object sender, MouseEventArgs e)
        {
            if (!bCaptureMouse) return;

            Point p = e.GetPosition(gridGraph);

            int mx = (int)p.X;
            int my = (int)p.Y;
            double gx1 = 0, gx2 = canvasGraph.ActualWidth - 1;

            int pos = (int)((float)(nShowUnit - 1) * (mx - gx1) / (gx2 - gx1) + 0.5);

            if (pos < 0) pos = 0;
            if (pos >= nShowUnit) pos = nShowUnit - 1;

            nCursorX2 = pos;
            SetCursorPos();
            UpdateValue();
            UpdateCursorTime();
        }

        private void canvasGraph_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!bCaptureMouse) return;
            bCaptureMouse = false;
            gridGraph.ReleaseMouseCapture();
        }

        public double GetCursorData(int pos)
        {
            if (pos < 0) return 0;
            if (pos >= (int)blockMember.Count) return 0;

            PUBLIC_GRAPH_MEMBER member;
            double value, min, max;

            member = (PUBLIC_GRAPH_MEMBER)blockMember[pos];

            if (GetCursorValue(member, out value, out min, out max))
            {
                return value;
            }
            else
            {
                return 0;
            }
        }

        void RemoveOneGraphElement(PUBLIC_GRAPH_MEMBER member)
        {
            for (int i = 0; i < nShowUnit; i++)
            {
                this.canvasGraph.Children.Remove(member.lines[i]);
                this.canvasGraph.Children.Remove(member.points[i]);
            }

            this.gridTagPanel.Children.Remove(member.tagPanel);

            this.canvasLevelLeft.Children.Remove(member.borderLevel);   // 왼쪽 오른쪽 모두 제거
            this.canvasLevelRight.Children.Remove(member.borderLevel);  // 왼쪽 오른쪽 모두 제거
        }

        public void MemberClear()
        {
            PUBLIC_GRAPH_MEMBER member;

            for (int l = 0; l < blockMember.Count; l++)
            {
                member = (PUBLIC_GRAPH_MEMBER)blockMember[l];

                RemoveOneGraphElement(member);
            }

            blockMember.Clear();
            CalcAxisPosition();

            //this.gridLevelLeft.ColumnDefinitions.Clear();
            //this.canvasLevelLeft.Children.Clear();

            //this.gridLevelRight.ColumnDefinitions.Clear();
            //this.canvasLevelRight.Children.Clear();
        }

        public void AddTag(PUBLIC_GRAPH_MEMBER member)
        {
            SetBlockOne(member);

            blockMember.Add(member);

            MakeGraphElement(member);
            MakeTagPanelOne(member, blockMember.Count - 1);
            MakeLevelElement(member, blockMember.Count - 1);

            SetBlockOne(member);
            MallocAllBuf();
            CalcAxisPosition();
        }

        public void DeleteTag(string tag)
        {
            PUBLIC_GRAPH_MEMBER member;

            for (int i = 0; i < blockMember.Count; i++)
            {
                member = (PUBLIC_GRAPH_MEMBER)blockMember[i];

                if (tag == member.tag)
                {
                    RemoveOneGraphElement(member);
                    blockMember.RemoveAt(i);
                    CalcAxisPosition();
                    return;
                }
            }
        }

        //static int nSignGraphNo = 0;

        void MallocOneBuf(PUBLIC_GRAPH_MEMBER member)
        {
            member.point = new ONE_POINT_STRUCT[nShowUnit];
        }

        void MallocAllBuf()
        {
            PUBLIC_GRAPH_MEMBER member;
            for (int i = 0; i < blockMember.Count; i++)
            {
                member = (PUBLIC_GRAPH_MEMBER)blockMember[i];
                MallocOneBuf(member);
            }
        }

        protected void ClearReadFlag()
        {
            PUBLIC_GRAPH_MEMBER member;
            for (int i = 0; i < blockMember.Count; i++)
            {
                member = (PUBLIC_GRAPH_MEMBER)blockMember[i];
                for (int j = 0; j < nShowUnit; j++)
                {
                    member.point[j].read_flag = false;
                }

            }
        }

        void CalcAxisPosition()
        {
            int lstart = 0, lend = 0;
            int rstart = 0, rend = 0;
            int lpos = 0;
            int rpos = 0;
            int l;
            PUBLIC_GRAPH_MEMBER member;

            for (l = 0; l < blockMember.Count; l++)
            {
                member = (PUBLIC_GRAPH_MEMBER)blockMember[l];

                if (member.nAxisPosition == 1)
                {	// left position
                    if (lstart == 0 && lend == 0)
                    {
                        member.nAxisCalcPos = lpos;
                        lstart = member.nLevelFrom;
                        lend = member.nLevelTo;
                    }
                    else
                    {
                        if (member.nLevelFrom >= lend || member.nLevelTo <= lstart)
                        {
                            member.nAxisCalcPos = lpos;
                            lstart = 0;
                            lend = 0;
                            lpos++;
                        }
                        else
                        {
                            lpos++;
                            member.nAxisCalcPos = lpos;
                            lstart = member.nLevelFrom;
                            lend = member.nLevelTo;
                        }
                    }
                }
                else if (member.nAxisPosition == 2)
                {	// right position
                    if (rstart == 0 && rend == 0)
                    {
                        member.nAxisCalcPos = rpos;
                        rstart = member.nLevelFrom;
                        rend = member.nLevelTo;
                    }
                    else
                    {
                        if (member.nLevelFrom >= rend || member.nLevelTo <= rstart)
                        {
                            member.nAxisCalcPos = rpos;
                            rstart = 0;
                            rend = 0;
                            rpos++;
                        }
                        else
                        {
                            rpos++;
                            member.nAxisCalcPos = rpos;
                            rstart = member.nLevelFrom;
                            rend = member.nLevelTo;
                        }
                    }
                }
            }

            if (lstart == 0 && lend == 0 && lpos == 0)
            {
                nHapLeftDisplay = 0;	// 왼쪽 눈금의 개수
            }
            else
            {
                nHapLeftDisplay = lpos + 1;	// 왼쪽 눈금의 개수
            }
            if (rstart == 0 && rend == 0 && rpos == 0)
            {
                nHapRightDisplay = 0;		// 오른쪽 눈금의 개수
            }
            else
            {
                nHapRightDisplay = rpos + 1;	// 오른쪽 눈금의 개수
            }

            gridLevelAndGraph.ColumnDefinitions[0].Width = new GridLength(nHapLeftDisplay * 80);
            gridLevelAndGraph.ColumnDefinitions[2].Width = new GridLength(nHapRightDisplay * 80);
        }

        void SetBlockOne(PUBLIC_GRAPH_MEMBER member)
        {
            TagLib.GetTagTypeAndPos(member.tag, ref member.nType, ref member.nPos);
            if (member.nLevelFrom < 0) member.nLevelFrom = 0;
            if (member.nLevelTo > 100) member.nLevelTo = 100;
            if (member.nLevelFrom >= member.nLevelTo)
            {
                member.nLevelFrom = 0;
                member.nLevelTo = 100;
            }
            member.visible = 1;
            if (member.nTagDisplaySize < 1 || member.nTagDisplaySize > 40)
            {
                member.nTagDisplaySize = 10;
            }

            if (member.nType == EnumTagType.AI)
            {
                TagAiClass ai = TagLib.GetStructAI(member.tag, ref member.nPos);
                member.max_value = ai.view_full;
                member.min_value = ai.view_base;
                member.view_base = ai.view_base;
                member.view_full = ai.view_full;
            }
        }

        void SetBlock(List<object> block)
        {
            int l;
            PUBLIC_GRAPH_MEMBER member;

            if (block == null)
                blockMember = new List<object>();
            else
                blockMember = block;

            for (l = 0; l < blockMember.Count; l++)
            {
                member = (PUBLIC_GRAPH_MEMBER)blockMember[l];
                SetBlockOne(member);
            }


            CalcAxisPosition();
        }

        public void SetShowUnit(int unit)
        {
            PUBLIC_GRAPH_MEMBER member;

            for (int l = 0; l < blockMember.Count; l++)
            {
                member = (PUBLIC_GRAPH_MEMBER)blockMember[l];
                for (int i = 0; i < nShowUnit; i++)
                {
                    this.canvasGraph.Children.Remove(member.lines[i]);
                    this.canvasGraph.Children.Remove(member.points[i]);
                }
            }

            nShowUnit = unit;
            if (nCursorX1 < 0) nCursorX1 = 0;
            if (nCursorX1 >= nShowUnit) nCursorX1 = nShowUnit - 1;
            if (nCursorX2 < 0) nCursorX2 = 0;
            if (nCursorX2 >= nShowUnit) nCursorX2 = nShowUnit - 1;
            MallocAllBuf();

            for (int l = 0; l < blockMember.Count; l++)
            {
                member = (PUBLIC_GRAPH_MEMBER)blockMember[l];

                MakeGraphElement(member);
            }

            UpdateGraph();
        }

        public void SetStartTime(DateTime t)
        {
            dtStartTime = t;
        }

        public int GetMemberFlags(int pos)
        {
            if (pos >= this.blockMember.Count) return 0;
            if (pos < 0) return 0;

            DB_TREND_MEMBER member = (DB_TREND_MEMBER)this.blockMember[pos];
            return member.wFlags;
        }

        public int SetMemberFlags(int pos, ushort flags)
        {
            if (pos >= this.blockMember.Count) return 0;
            if (pos < 0) return 0;

            DB_TREND_MEMBER member = (DB_TREND_MEMBER)this.blockMember[pos];
            member.wFlags = flags;
            //InvalidateObject(formParent);
            return 1;
        }

        // public bool bUseTimeChange = true;

        private void gridTagPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DialogGraph page = new DialogGraph();
            DialogCommon dialog = new DialogCommon(page);

            if(Tools.IsLangKorean())
                dialog.Text = "그래프 설정";
            else
                dialog.Text = "Graph";

            dialog.RegisterCancelButton(page.buttonCancel);
            dialog.RegisterOkButton(page.buttonOK, new DialogCommon.DelegateOnOK(OnOK));

            page.checkBoxUseDisplayPointTime.IsChecked = this.bDisplayPointDate;
            //page.checkBoxUseAutoRange.IsChecked = this.bAutoViewRange;
            page.radioButtonStartType0.IsChecked = (this.cGraphStartTimeMethod == 0);
            page.radioButtonStartType1.IsChecked = (this.cGraphStartTimeMethod == 1);
            page.textBoxYear.Text = this.dtStartTime.Year.ToString();
            page.textBoxMonth.Text = this.dtStartTime.Month.ToString();
            page.textBoxDay.Text = this.dtStartTime.Day.ToString();
            page.textBoxHour.Text = this.dtStartTime.Hour.ToString();
            page.textBoxMinute.Text = this.dtStartTime.Minute.ToString();

            page.nShowUnit = nShowUnit;
            page.nTimeType = wTimeSelectOption;

            dialog.Show(SilverlightDialogControl.DialogStyle.Modal);
        }

        bool OnOK(UserControl control)
        {
            DialogGraph page = (DialogGraph)control;

            this.bDisplayPointDate = (bool)page.checkBoxUseDisplayPointTime.IsChecked;
            //this.bAutoViewRange = (bool)page.checkBoxUseAutoRange.IsChecked;    // 잘 안됨

            if(page.radioButtonStartType0.IsChecked == true)    this.cGraphStartTimeMethod = 0;
            else                                                this.cGraphStartTimeMethod = 1;

            if (this.cGraphStartTimeMethod == 1)
            {
                DateTime t = new DateTime(ConvertTool.ToInt32(page.textBoxYear.Text),
                    ConvertTool.ToInt32(page.textBoxMonth.Text),
                    ConvertTool.ToInt32(page.textBoxDay.Text),
                    ConvertTool.ToInt32(page.textBoxHour.Text),
                    ConvertTool.ToInt32(page.textBoxMinute.Text),
                    0);
                SetStartTime(t);
            }

            ReadAllPoint();
            UpdateGraph();

            return true;
        }

        public virtual void ReadAllPoint()
        {

        }
    }
}
