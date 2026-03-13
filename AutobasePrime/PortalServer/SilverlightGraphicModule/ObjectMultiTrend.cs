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
using AutoLibLocal;
using System.Collections.Generic;
using NetTools.OldDefine;
using NetTools;
using AutoLib;
using System.Xml.Linq;

namespace SilverlightGraphicModule
{
     public class ObjectArgsMultiTrend
    {
        public ObjectArgsGraphPublic pub = new ObjectArgsGraphPublic();

        public int wTimeDevide;
        public int wTimeSelectOption;

        public BrushPublic lColorBack = new BrushPublic();
        public Color lColorGuideLine;
        public Color lColorText;
        public BrushPublic lColorFill = new BrushPublic();
        public int wShowUnit;
        public int wLevelDevide;
    }

    public struct ONE_POINT_STRUCT
    {
        public double val;			// 실제값.
        public bool read_flag;		// 값을 읽었느냐?
    }

    public class PUBLIC_GRAPH_MEMBER
    {
        public string tag;
        // multi graph 에서 사용할 멤버
        public EnumTagType nType;		// tag type
        public int[] nPos;				// tag pos
        public Color color;				// 각 태그의 사용색
        public int nValueType;		// 0 - 평균치
        public int nPointType;	// 점의 형태, 0 - none, 
        public int nLineThick;	// 선굵기.
        public int nAxisPosition;	// 좌표의 위치, 0 - 표시안함, 1 - 왼쪽에 표시, 2 - 오른쪽에 표시,
        public int nLevelFrom;	// 그라프 표시의 위치 부터
        public int nLevelTo;		// 그라프 표시의 위치 까지
        public int nAxisCalcPos;	// 내부 계산에 필요한 부분 미리 그래프가 위치할 부분을 계산해 놓는다.
        public ONE_POINT_STRUCT[] point;	// 각 그래프에서 사용하는 그라프 메모리.

        public double min_value;		// 읽은 값 중에서 최소값
        public double max_value;		// 읽은 값 중에서 최대값.

        public sbyte visible;		// 현재 활성화중인가? ON=보임, OFF = 안보임
        public int nTagDisplaySize = 10;// 태그이름을 표시할 때 크기가 10으로 고정되면 모두 안보일 수 있으므로 사용자가 설정할 수 있게 하였다.

        public double view_full;
        public double view_base;

        public sbyte bReverseY;		// Y축의 좌표를 반대로 한다.
        public ushort wFlags = 0;
        public int nTimeShift = 0;      // 시간축 보정, 멤버중에서 늦게 시작하는 멤버를 일치시키기 위해서 9.3.5 부터 추가

        // 실행 시 그리기 위해 필요한 변수 저장할 필요는 없다.
        public Line[] lines;
        public FrameworkElement[] points;
        public TextBlock[] textLevel;   // 레벨을 표시해 주는 글자.
        public TextBlock[] textValue;   // 태그 판넬에 사용되는 글자 (태그,최대,최소,평균,자료값)
        public StackPanel tagPanel;
        public Border borderLevel;
        
    }

    [Flags]
    public enum EnumDisplayFlag
    {
        DESX = 0x0001,
        DESY = 0x0002,
        TAG_COLOR = 0x0004,
        GUIDELINE = 0x0008,
        TAG_CURR = 0x0010,
        TAG_OLD_CURR = 0x0020,
        TAG_MIN_MAX = 0x0040,
        BACK_BORDER = 0x0080,
        BY_DESCRIPTION = 0x0100,
    }

    /// <summary>
    /// Summary description for ObjectMultiTrend.
    /// </summary>
    public class ObjectMultiTrend : ObjectExpand
    {
        ObjectArgsMultiTrend objArgs;
        static public List<object> arrayClassList = new List<object>();

        ControlMultiTrend formChild;

        public ObjectMultiTrend(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsMultiTrend args, List<object> block)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.MultiTrend;
            objArgs = args;

            if (objArgs.pub.nLevelDisplaySize < 2) objArgs.pub.nLevelDisplaySize = 2;
            if (objArgs.pub.nLevelDisplaySize > 50) objArgs.pub.nLevelDisplaySize = 7;

            if (objArgs.wLevelDevide < 2) objArgs.wLevelDevide = 2;
            if (objArgs.wLevelDevide > 100) objArgs.wLevelDevide = 100;

            if (objArgs.wTimeDevide < 1) objArgs.wTimeDevide = 10;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                arrayClassList.Add(this);
            }

            TextColor = args.lColorText;
            BackColor = args.lColorBack;
            FillColor = args.lColorFill;

            ControlMultiTrend child = new ControlMultiTrend(objArgs.wShowUnit, block, objArgs.pub.wPointSize, objArgs.wTimeDevide, objArgs.wLevelDevide, DateTime.Now, objArgs.wTimeSelectOption, args.lColorBack, args.lColorFill, args.lColorGuideLine, args.pub.wDisplayFlags, false, 1);

            formChild = child;

            parent_canvas.Children.Add(child);

            SetShapeOriginal(child);
            MoveShape();
        }

        protected override bool IsNeedMouseHitTest()
        {
            return true;
        }
        

        public override void EventTimerObject(UserControl form)
        {
            formChild.Timer();
        }

        public override object ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            if (command == "MultiTrendSetBackColor")
            {
                GetBackColor().basic_color = (Color)args[0];
                formChild.SetBackColor(GetBackColor());
            }
            else if (command == "MultiTrendSetStartTime")
            {
                DateTime t = new DateTime((int)args[0], (int)args[1], (int)args[2], (int)args[3], (int)args[4], 0);
                formChild.SetStartTime(t);
                formChild.ReadAllPoint();
            }
            else if (command == "MultiTrendSetBasicLevel")
            {
                //SetBasicLevel((int)args[0]);
            }
            else if (command == "MultiTrendGetCursorData")
            {
                return formChild.GetCursorData((int)args[0]);
            }
            else if (command == "MultiTrendClear")
            {
                formChild.MemberClear();
            }
            else if (command == "MultiTrendAddTag")
            {
                formChild.AddTag((PUBLIC_GRAPH_MEMBER)args[0]);
            }
            else if (command == "MultiTrendDeleteTag")
            {
                formChild.DeleteTag((string)args[0]);
            }
            else if (command == "MultiTrendReLoad")
            {
                formChild.ReadAllPoint();
            }
            else if (command == "MultiTrendSetDataSize")
            {
                this.SetShowUnit((int)args[0]);
            }
            else if (command == "MultiTrendGetDataSize")
            {
                return this.objArgs.wShowUnit;
            }
            else if (command == "MultiTrendGetStartTime")
            {
                return formChild.dtStartTime;
            }
            else if (command == "MultiTrendSetStartTimeMode")
            {
                formChild.SetStartTimeMode((sbyte)((int)args[0]));
            }
            else if (command == "MultiTrendGetStartTimeMode")
            {
                return formChild.cGraphStartTimeMethod;
            }
            else if (command == "MultiTrendGetCursorTime")
            {
                DateTime dt;
                string buf;
                dt = formChild.GetCursorTime(out buf);

                return dt;
            }
            else if (command == "MultiTrendGetCursorSize")
            {
                return Math.Abs(formChild.nCursorX2 - formChild.nCursorX1) + 1;
            }
            else if (command == "MultiTrendGetTimeType")
            {
                return (int)objArgs.wTimeSelectOption;
            }
            else if (command == "MultiTrendSetTimeType")
            {
                int type = (int)args[0];
                this.objArgs.wTimeSelectOption = type;

                //CONFIG_MULTI_TREND config;
                //config = LoadMultiTrendConfig();
                //config.cTimeZone = (byte)type;
                //SaveMultiTrendConfig(config);
            }
            else
            {
                return 0;
            }

            return 1;
        }

        void SetShowUnit(int unit)
        {
            if (unit == objArgs.wShowUnit) return;
            if (unit < 10) unit = 10;
            objArgs.wShowUnit = unit;
            formChild.SetShowUnit(unit);
        }



    }
         
}
