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
using NetTools.OldDefine;
using AutoLibLocal;

namespace SilverlightGraphicModule
{
    public class ObjectArgsGraphPublic
    {
        public EnumDisplayFlag wDisplayFlags;
        public int wPointSize = 10;
        public int nLevelDisplaySize = 7;

        //public Color HIHI = Color.FromArgb(0x80, 0, 0);
        //public Color HIGH = Color.FromArgb(255, 0, 0);
        //public Color LOW = Color.FromArgb(0, 0, 255);
        //public Color LOLO = Color.FromArgb(0, 0, 0x80);
        public Color colorHiHi = Color.FromArgb(255, 0x80, 0, 0);
        public Color colorHigh = Color.FromArgb(255, 255, 0, 0);
        public Color colorLow = Color.FromArgb(255, 0, 0, 255);
        public Color colorLoLo = Color.FromArgb(255, 0, 0, 0x80);
    }

    public class ObjectArgsMultiGraph
    {
        public ObjectArgsGraphPublic pub = new ObjectArgsGraphPublic();
        public BrushPublic lColorFill = new BrushPublic();
        public Color lColorText;
        public BrushPublic lColorBack = new BrushPublic();
        public Color lColorGuideLine;

        public int wShowUnit;

        public int wLevelDevide;

        public int nDataTime;	// milli sec
        public int wTimeDevide;		// time devide unit

        public string sTitle;
        public sbyte bTimeDirToLeft;
        public sbyte bDisplayByTime;
    }

    public class ObjectMultiGraph : ObjectExpand
    {
        

        ObjectArgsMultiGraph objArgs;

        // 등록된 클래스 리스트
        //[NonSerialized]
        static public List<object> arrayClassList = new List<object>();

        

        ControlPublicGraph formChild;

        public ObjectMultiGraph(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, RECT rect, EXPAND_ID_STRUCT eid, ObjectGeneral general, LOGFONT lf, ObjectArgsMultiGraph args, List<object> block)
            : base(ocp, rect, eid, lf, general)
        {
            //
            // TODO: Add constructor logic here
            //
            enumObjectType = EnumObjectType.MultiGraph;
            objArgs = args;
            

            if (objArgs.wLevelDevide < 2) objArgs.wLevelDevide = 2;
            if (objArgs.wLevelDevide > 100) objArgs.wLevelDevide = 100;

            

            if (objArgs.pub.nLevelDisplaySize < 2) objArgs.pub.nLevelDisplaySize = 2;
            if (objArgs.pub.nLevelDisplaySize > 50) objArgs.pub.nLevelDisplaySize = 7;

            if (objArgs.wTimeDevide < 1) objArgs.wTimeDevide = 10;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                arrayClassList.Add(this);
            }

            if (objArgs.nDataTime < 1) objArgs.nDataTime = 1000;

            TextColor = args.lColorText;
            BackColor = args.lColorBack;
            FillColor = args.lColorFill;
            OptionMultiGraph option = new OptionMultiGraph();
            option.bTimeDirToLeft = objArgs.bTimeDirToLeft;
            option.bDisplayByTime = objArgs.bDisplayByTime;
            option.nDataTime = objArgs.nDataTime;

            ControlMultiGraph child = new ControlMultiGraph(objArgs.wShowUnit, block, objArgs.pub.wPointSize, objArgs.wTimeDevide, objArgs.wLevelDevide, DateTime.Now, 2, args.lColorBack, args.lColorFill, args.lColorGuideLine, args.pub.wDisplayFlags, option, false);

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
            if (command == "MultiGraphSetBackColor")
            {
                GetBackColor().basic_color = (Color)args[0];
                formChild.SetBackColor(GetBackColor());
            }
            else if (command == "MultiGraphSetBasicLevel")
            {
                //SetBasicLevel((int)args[0]);
            }
                
            else if (command == "MultiGraphGetCursorData")
            {
                return formChild.GetCursorData((int)args[0]);
            }
            else if (command == "MultiGraphClear")
            {
                formChild.MemberClear();
            }
            else if (command == "MultiGraphAddTag")
            {
                formChild.AddTag((PUBLIC_GRAPH_MEMBER)args[0]);
            }
            else if (command == "MultiGraphDeleteTag")
            {
                formChild.DeleteTag((string)args[0]);
            }
            else if (command == "MultiGraphSetDataSize")
            {
                this.SetShowUnit((int)args[0]);
            }
            else if (command == "MultiGraphGetDataSize")
            {
                return this.objArgs.wShowUnit;
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
