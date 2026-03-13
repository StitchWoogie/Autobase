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
using NetTools;
using NetTools.OldDefine;
using AutoLibLocal;
using System.IO;
using System.Collections.Generic;
using AutoLib;

namespace SilverlightGraphicModule
{

    /// <summary>
    /// Summary description for ObjectPublic.
    /// </summary>
    ///

    //[Serializable]
    public class EXPAND_ID_STRUCT
    {
        public sbyte active;
        public uint id;
        public EnumModType mod_type;
        public ExpandScript expand = new ExpandScript();
        public bool bLineColorToTextColor = false;	// MOD파일에서는 TextColor 스크립트가 없고 LineColor 스크립트를 대신 사용했으므로 SingleText Object는 변환이 필요하다.
    };

    //[Serializable]
    public class EXPAND_SLIDER_STRUCT
    {
        public string sTag;
        public int nZoneMin;
        public int nZoneMax;
        public int nValueMin;
        public int nValueMax;
        public sbyte bUseFullBase;

        public int nWriteTimer;				// Write시 값을 변경시킨 후 
        public int nWriteTimerOldSec;
        public double fWriteValue;		    // 변경한 값이 timer가 0이 될때까지 화면상에는 이값을 보여준다.

        //[NonSerialized]
        public int[] nTagPos = new int[1];

        public void LoadFromMODX(TextReader reader, string command)
        {
            CommaBlockString comma = new CommaBlockString();
            string buf = "";

            nZoneMin = 0;
            nZoneMax = 100;
            nValueMin = 0;
            nValueMax = 100;

            while (true)
            {
                buf = reader.ReadLine();
                if (buf == null) break;

                comma.Set(buf);
                comma.GetString(ref buf);
                if (buf == "TagName")
                {
                    comma.GetString(ref sTag);
                    sTag = sTag.Trim();
                }
                else if (buf == "ZoneMin")
                {
                    comma.GetInt(ref nZoneMin);
                }
                else if (buf == "ZoneMax")
                {
                    comma.GetInt(ref nZoneMax);
                }
                else if (buf == "ValueMin")
                {
                    comma.GetInt(ref nValueMin);
                }
                else if (buf == "ValueMax")
                {
                    comma.GetInt(ref nValueMax);
                }
                else if (buf == "bUseFullBase")
                {
                    comma.GetChar(ref bUseFullBase);
                }
                else if (buf == command)
                {
                    break;
                }
            }
        }

        /*
        public void SaveFile(CommaTextWriter writer)
        {
            writer.WriteLine("TagName,{0}", sTag);
            writer.WriteLine("ZoneMin,{0}", nZoneMin);
            writer.WriteLine("ZoneMax,{0}", nZoneMax);
            writer.WriteLine("ValueMin,{0}", nValueMin);
            writer.WriteLine("ValueMax,{0}", nValueMax);
            writer.WriteLine("bUseFullBase,{0}", bUseFullBase);
        }*/
    }

    //[Serializable]
    public class EXPAND_MOUSE_ZONE
    {
        public int x1 = 5;
        public int y1 = 5;
        public int x2 = 5;
        public int y2 = 5;
        public sbyte bLock = 1;	// 이것이 체크되어 있으면 모두가 같게 반응한다.

        public void LoadFromMODX(TextReader reader, string command)
        {
            CommaBlockString comma = new CommaBlockString();
            string buf = "";

            bLock = 1;
            x1 = 5;
            y1 = 5;
            x2 = 5;
            y2 = 5;

            while (true)
            {
                buf = reader.ReadLine();
                if (buf == null) break;

                comma.Set(buf);
                comma.GetString(ref buf);
                if (buf == "Zone")
                {
                    comma.GetInt(ref x1);
                    comma.GetInt(ref y1);
                    comma.GetInt(ref x2);
                    comma.GetInt(ref y2);
                }
                else if (buf == "Lock")
                {
                    comma.GetChar(ref bLock);
                }
                else if (buf == command) break;
                else { }
            }
        }

        /*
        public void SaveFile(CommaTextWriter writer)
        {
            writer.WriteLine("Zone,{0},{1},{2},{3},", x1, y1, x2, y2);
            writer.WriteLine("Lock,{0},", bLock);
        }*/
    }

    public enum EnumExpandType
    {
        Basic = 0,
        Script = 1,
    }

    //[Serializable]
    public class ExpandBasicConversion
    {
        public string sTag;
        public int nBasicConversionType;
        public float fBasicSourceMin;
        public float fBasicSourceMax;
        public float fBasicTargetMin;
        public float fBasicTargetMax;

        public int[] tag_pos;
    }

    //[Serializable]
    public class ExpandBasicVisible
    {
        public string sTag;
        public int condition;
        public string value;

        public int[] tag_pos;
    }

    //[Serializable]
    public class ExpandBasicBlinking
    {
        public string sTag;
        public int condition;
        public string value;
        public int cycle;

        public int[] tag_pos;
    }

    //[Serializable]
    public class ExpandBasicOnlyTag
    {
        public string sTag;

        public int[] tag_pos;
    }

    //[Serializable]
    public class ExpandBasicColorMember
    {
        public bool active;
        public string sTag;
        public int condition;
        public string value = "0";
        public Color color;

        public int[] tag_pos;
    }

    //[Serializable]
    public class ExpandBasicColor
    {
        public List<object> member = new List<object>();
    }

    //[Serializable]
    public class ExpandBasicRotate
    {
        public string sTag;
        public int nRotateAxis; // 회전축
        public float fBasicSourceMin;
        public float fBasicSourceMax;
        public float fBasicTargetMin;
        public float fBasicTargetMax;
        public int nDirection;  // 0 = 시계방향, 1 = 반시계방향

        public int[] tag_pos;
    }

    //[Serializable]
    public class ExpandBasicAndScript
    {
        public EnumExpandType eExpandType = EnumExpandType.Script;  // 9 에서는 기본이 Script이므로 기본을 Script로 지정
        public object basic;
        public ScriptClass script;
    }

    //[Serializable]
    public class ExpandScript
    {
        public ExpandBasicAndScript scriptSizeWidth;
        public ExpandBasicAndScript scriptSizeHeight;
        public ExpandBasicAndScript scriptLocationX;
        public ExpandBasicAndScript scriptLocationY;
        public ExpandBasicAndScript scriptVisible;
        public ScriptClass scriptMouseDown;
        public ScriptClass scriptMouseUp;
        public ExpandBasicAndScript scriptAnimationSpeed;
        public EXPAND_SLIDER_STRUCT sliderHorz;
        public EXPAND_SLIDER_STRUCT sliderVert;
        public EXPAND_MOUSE_ZONE structMouseZone;
        public ExpandBasicAndScript scriptColorLine;
        public ExpandBasicAndScript scriptColorFill;
        public ExpandBasicAndScript scriptColorText;
        public ExpandBasicAndScript scriptColorBack;
        public ExpandBasicAndScript scriptThickLine;
        public ExpandBasicAndScript scriptBlinking;
        public ScriptClass scriptEventKeyDown;	// control
        public ScriptClass scriptEventSelChange;	// control	
        public ExpandBasicAndScript scriptRotate;   // 10.0 부터 추가된 기능
    }

    public class BrushPublic
    {
        public int brush_type = 1;      // 0 - null, 1 = solid, 2 = linear gradation, 3 = radial gradation
        public Color basic_color;   // 기존에 한가지 색상만을 사용했으므로 기존 버전과 호환성을 유지하기 위해서 기본색상을 지정해 주면 이전 버전에서도 비슷한 색상을 읽어올 수 있다.
        public Color org_basic_color = Colors.Black;
        // Runtime시 basic색상이 바뀐다. original을 기억해서 확장기능에서 이외의 색상일 경우를 대비해서 기억해 둔다. 10.1.1 부터 추가
    }

    public class BrushSolid : BrushPublic
    {
        // basic_color 를 사용한다.
        public BrushSolid(Color color)
        {
            basic_color = color;
        }

        public BrushSolid()
        {

        }
    }

    public class BrushGradient : BrushPublic
    {
        public List<BrushGradientStop> stops;
        public int spread_method;
    }

    public class BrushLinearGradient : BrushGradient
    {
        public Point pStart;
        public Point pEnd;
    }

    public class BrushGradientStop
    {
        public Color color;
        public float offset;
    }

    //[Serializable]
    public class ObjectExpand : ObjectFont
    {
        //[NonSerialized]
        bool bMouseCapture;

        EXPAND_ID_STRUCT eID = new EXPAND_ID_STRUCT();

        public ExpandScript expandScript
        {
            set
            {
                eID.expand = value;
            }
            get
            {
                return eID.expand;
            }
        }

        //[NonSerialized]
        int nSliderGabX;
        //[NonSerialized]
        int nSliderGabY;

        //[NonSerialized]
        int nRunLocationX;			// run 중일 때 위치값.
        //[NonSerialized]
        int nRunLocationY;			// run 중일 때 위치값.
        //[NonSerialized]
        float fRunPercentWidth;     //
        //[NonSerialized]
        float fRunPercentHeight;    //
        //[NonSerialized]
        protected bool bRunVisible;   		//
        //[NonSerialized]
        int nRunAnimationSpeed;	        // animation speed
        protected int nRunThickLine;	// 현재 화면상에 계산되어 있는 object의 borderThick
        //[NonSerialized]
        int nBlinkingCycle;		        // 점멸주기 
        //[NonSerialized]
        bool bVisibleByBlinking;

        //[NonSerialized]
        protected int nViewX1;		// 현재 화면상에 계산되어 있는 object의 위치
        //[NonSerialized]
        protected int nViewY1;		// 현재 화면상에 계산되어 있는 object의 위치
        //[NonSerialized]
        protected int nViewX2;		// 현재 화면상에 계산되어 있는 object의 위치
        //[NonSerialized]
        protected int nViewY2;		// 현재 화면상에 계산되어 있는 object의 위치
        //[NonSerialized]
        int nViewLineThick;	// 현재 화면상에 계산되어 있는 object의 Line thick

        Color lLineColor;
        BrushPublic lFillColor;
        Color lTextColor;
        BrushPublic lBackColor;
        int wBorderThick;			// 테두리의 두께.

        public int nLineOption;
        public int nFillOption
        {
            get
            {
                return lFillColor.brush_type;
            }
            set
            {
                lFillColor.brush_type = value;
            }
        }

        //[NonSerialized]
        protected bool bOnMouseZone;					// ON이면 마우스가 Object영역속에 들어왔다는 의미이다.
        protected int nLeft, nTop, nRight, nBottom;  	// object 좌표

        //[NonSerialized]
        Color lRunColorLine;		// run 중 line color
        //[NonSerialized]
        //BrushPublic lRunColorFill;		// run 중 fill color
        //[NonSerialized]
        Color lRunColorText;		// run 중 fill color
        //[NonSerialized]
        //BrushPublic lRunColorBack;		// run 중 fill color

        //[NonSerialized]
        float fRunRotationAngle;		                // run 중 회전각

        public float RotationAngle
        {
            get
            {
                if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                {
                    if (eID.expand.scriptRotate == null)
                    {
                        return objGeneral.fRotateAngle;
                    }
                    else
                    {
                        return fRunRotationAngle;
                    }
                }
                else
                    return objGeneral.fRotateAngle;
            }
            set
            {
                objGeneral.fRotateAngle = value;
                fRunRotationAngle = value;
            }
        }

        public virtual void Close()
        {

        }

        public Color RunColorLine
        {
            get
            {
                if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                    return lRunColorLine;
                else
                    return lLineColor;
            }
        }

        public BrushPublic RunColorFill
        {
            get
            {
                //if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                    //return lRunColorFill;
                //else
                    return lFillColor;
            }
        }

        public Color RunColorText
        {
            get
            {
                if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                    return lRunColorText;
                else
                    return lTextColor;
            }
        }

        public BrushPublic RunColorBack
        {
            get
            {
                //if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
                    //return lRunColorBack;
                //else
                    return lBackColor;
            }
        }

        public virtual void ObjectSave(CommaTextWriter writer)
        {
            MessageBox.Show("상속클래스에서 ObjectSave를 override 해야 합니다.\n(Class=" + this.ToString() + ")", "프로그램 오류", MessageBoxButton.OK);
        }

        public bool ExpandActive
        {
            get
            {
                return (eID.active == 1);
            }
            set
            {
                if (value) eID.active = 1;
                else eID.active = 0;
            }
        }

        public void SetExpandIdStruct(EXPAND_ID_STRUCT eid)
        {
            eID = eid;
        }

        public Color LineColor
        {
            set
            {
                SetLineColor(value);
            }
        }

        public BrushPublic FillColor
        {
            set
            {
                SetFillColor(value);
            }
        }

        public Color TextColor
        {
            set
            {
                SetTextColor(value);
            }
        }

        public BrushPublic BackColor
        {
            set
            {
                SetBackColor(value);
            }
        }

        public int LineThick
        {
            set
            {
                SetBorderThick(value);
            }
        }

        public ObjectGeneral objGeneral = new ObjectGeneral();

        public ObjectExpand(ObjectCommonProperty ocp, RECT rect, EXPAND_ID_STRUCT eid, LOGFONT lf, ObjectGeneral general)
            : base(ocp, lf)
        {
            if (general == null)
                objGeneral = new ObjectGeneral();
            else
                objGeneral = general;

            if (rect == null)
            {
                nLeft = 0;
                nTop = 0;
                nRight = 100;
                nBottom = 100;
            }
            else
            {
                nLeft = rect.left;
                nTop = rect.top;
                nRight = rect.right;
                nBottom = rect.bottom;
            }

            if (eid != null)
            {
                eID = eid;
            }
            else
            {
                eID.active = 0;
                eID.id = 0;
                eID.mod_type = EnumModType.mod;
            }

            //bMouseCapture = false;

            nRunLocationX = 0;			    // run 중일 때 위치 값.
            nRunLocationY = 0;
            fRunPercentWidth = 0;
            fRunPercentHeight = 0;
            bRunVisible = true;
            lRunColorLine = Colors.Black;
            //lRunColorFill = new BrushSolid(Colors.White);
            //lRunColorBack = new BrushSolid(Colors.White);
            lRunColorText = Colors.Black;
            nRunThickLine = 1;
            nBlinkingCycle = 0;	            // 점멸주기 
            bVisibleByBlinking = true;	    // 점멸옵션을 사용할 때 상태.
            bOnMouseZone = false;			// ON이면 마우스가 Object영역속에 들어왔다는 의미이다.

            wBorderThick = 1;

            lLineColor = Colors.Black;
            lFillColor = new BrushSolid(Colors.Red);
            lBackColor = new BrushSolid(Colors.White);

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                SetUseScriptLocationX((eID.active == 1 && eID.expand.scriptLocationX != null) ? true : false);
                SetUseScriptLocationY((eID.active == 1 && eID.expand.scriptLocationY != null) ? true : false);
            }

            nViewX1 = 0;
            nViewY1 = 0;
            nViewX2 = 0;
            nViewY2 = 0;
            nViewLineThick = 1;

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                EnumRunChangedType changed_type;
                ExpandRun(null, out changed_type);
            }

            ExpandCalcObjectRect();

            //MakeMouseZone(ocp.rootCanvas);
        }

        public Color GetLineColor() { return lLineColor; }
        public BrushPublic GetFillColor() { return lFillColor; }
        public Color GetTextColor() { return lTextColor; }
        public BrushPublic GetBackColor() { return lBackColor; }
        public int GetBorderThick() { return wBorderThick; }

        int ExpandCalcSizeWidth(int width)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (eID.active == 0) return width;
                if (eID.expand.scriptSizeWidth == null) return width;

                return (int)((long)width * fRunPercentWidth / 100);
            }
            else
            {
                return width;
            }
        }

        int ExpandCalcSizeHeight(int height)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (eID.active == 0) return height;
                if (eID.expand.scriptSizeHeight == null) return height;

                return (int)((long)height * fRunPercentHeight / 100);
            }
            else
            {
                return height;
            }
        }

        int ExpandCalcLocationX(int x)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (eID.active == 0) return x;
                if (eID.expand.scriptLocationX == null && eID.expand.sliderHorz == null) return x;
                return nRunLocationX;
            }
            else
            {
                return x;
            }
        }

        int ExpandCalcLocationY(int y)
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (eID.active == 0) return y;
                if (eID.expand.scriptLocationY == null && eID.expand.sliderVert == null) return y;

                return nRunLocationY;
            }
            else
            {
                return y;
            }
        }

        protected bool ExpandCalcVisible()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (bRunVisible && bVisibleByBlinking) return true;

                return false;
            }
            else
            {
                return true;
            }
        }
        
        //------------------------------------------------------------------------------
        //	각 object에서 직접 이 함수를 부른다.
        //	내부에서는 사용하지 않는다.
        //  사용자 script에서는 rpm 값으로 온다.
        //------------------------------------------------------------------------------

        public int ExpandGetAnimationSpeed()
        {
            if (eID.active == 0) return 60;
            if (eID.expand.scriptAnimationSpeed == null) return 60;	// 1초에 1회전 한다.

            return nRunAnimationSpeed;
        }

        //------------------------------------------------------------------------------
        //	각 object에서 직접 이 함수를 부른다.
        //	내부에서는 사용하지 않는다.
        // 애니매이션 속도를 사용하는가 ?.
        //------------------------------------------------------------------------------

        public bool ExpandIsUseAnimationSpeed()
        {
            if (eID.active == 0) return false;

            if (eID.expand.scriptAnimationSpeed != null) return true;
            else return false;
        }

        int GetPercent(float curr, float v_base, float v_full, float fBase, float fFull)
        {
            float val;

            if ((v_full - v_base) == 0)
            {
                if (fBase < fFull) val = fBase;
                else val = fFull;
            }
            else
            {
                if (fBase < fFull)
                {
                    val = (float)(((double)(curr - v_base) * (fFull - fBase)) / (v_full - v_base));
                    val += fBase;
                }
                else
                {	// 결과값이 반대로 꺼꾸로 되어 있을 때
                    val = (float)(((double)(curr - v_base) * (fBase - fFull)) / (v_full - v_base));
                    val = (fBase - fFull) - val;
                    val += fFull;
                }
            }

            // 범위를 초과하는 값을 잘라낸다.
            if (fBase < fFull)
            {
                if (val < fBase) val = fBase;
                if (val > fFull) val = fFull;
            }
            else
            {
                if (val > fBase) val = fBase;
                if (val < fFull) val = fFull;
            }

            return (int)val;
        }

        protected enum EnumRunChangedType
        {
            None = 0x0000,
            Size = 0x0001,
            Location = 0x0002,
            Thick = 0x0004,
            Rotation = 0x0008,   // 10.0 부터 추가
        }

        // protected virtual void TextColorChanged() { }
        // protected virtual void BackColorChanged() { }

        double GetTagValue(string tag, ref int[] tag_pos)
        {
            double val = 0;
            TagPublicClass tp = TagLib.GetStructPublic(tag, ref tag_pos);

            tp.bNeedDataCurr = true;

            if (tp.enumTagType == EnumTagType.AI)
            {
                TagAiClass ai = (TagAiClass)tp;
                val = ai.curr;
            }
            else if (tp.enumTagType == EnumTagType.DI)
            {
                TagDiClass di = (TagDiClass)tp;
                val = di.curr;
            }
            else { }

            return val;
        }
       
        bool RunExpandBasicAndScript(UserControl form, ExpandBasicAndScript expand, out double retn, ref int conversion_type)
        {
            retn = 0;

            if (expand == null) return false;

            if (expand.eExpandType == EnumExpandType.Basic)
            {
                if (expand.basic.GetType() == typeof(ExpandBasicConversion))
                {
                    ExpandBasicConversion conv = (ExpandBasicConversion)expand.basic;

                    double val = GetTagValue(conv.sTag, ref conv.tag_pos);

                    retn = (val - conv.fBasicSourceMin) * (conv.fBasicTargetMax - conv.fBasicTargetMin) / (conv.fBasicSourceMax - conv.fBasicSourceMin) + conv.fBasicTargetMin;

                    conversion_type = conv.nBasicConversionType;
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicVisible))
                {
                    ExpandBasicVisible conv = (ExpandBasicVisible)expand.basic;

                    if (ObjectTagAnimation.IsCondition(conv.sTag, ref conv.tag_pos, conv.condition, conv.value))
                        retn = 1;
                    else
                        retn = 0;
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicBlinking))
                {
                    ExpandBasicBlinking conv = (ExpandBasicBlinking)expand.basic;

                    if (ObjectTagAnimation.IsCondition(conv.sTag, ref conv.tag_pos, conv.condition, conv.value))
                        retn = conv.cycle;
                    else
                        retn = 0;
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicOnlyTag))
                {
                    ExpandBasicOnlyTag conv = (ExpandBasicOnlyTag)expand.basic;

                    retn = GetTagValue(conv.sTag, ref conv.tag_pos);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicRotate))
                {
                    ExpandBasicRotate conv = (ExpandBasicRotate)expand.basic;

                    double val = GetTagValue(conv.sTag, ref conv.tag_pos);

                    retn = (val - conv.fBasicSourceMin) * (conv.fBasicTargetMax - conv.fBasicTargetMin) / (conv.fBasicSourceMax - conv.fBasicSourceMin) + conv.fBasicTargetMin;

                    conversion_type = conv.nRotateAxis;

                    if (conv.nDirection == 1) retn *= -1;   // 반시계방향
                }
                else
                {
                    MessageBox.Show("ObjectExpand.RunExpandBasicAndScript 에서 알수 없는 Type", expand.basic.ToString(), MessageBoxButton.OK);
                }
            }
            else
            {
                if (expand.script == null) return false;
                expand.script.Run(form);
                retn = (double)expand.script.GetReturnValue();
            }

            return true;
        }

        bool RunExpandBasicAndScript(UserControl form, ExpandBasicAndScript expand, ref Color retn)
        {
            if (expand == null) return false;

            if (expand.eExpandType == EnumExpandType.Basic)
            {
                if (expand.basic.GetType() == typeof(ExpandBasicColor))
                {
                    ExpandBasicColor conv = (ExpandBasicColor)expand.basic;

                    ExpandBasicColorMember member;

                    for (int i = 0; i < conv.member.Count; i++)
                    {
                        member = (ExpandBasicColorMember)conv.member[i];

                        if (member.active == false) continue;

                        
                        if (ObjectTagAnimation.IsCondition(member.sTag, ref member.tag_pos, member.condition, member.value))
                        {
                            retn = member.color;
                            return true;
                        }
                    }

                    return true;    // false가 아니다 넘어온 기본 색상값을 변경하지 않고 true를 반환한다.
                }
                else
                {
                    MessageBox.Show("ObjectExpand.RunExpandBasicAndScript에서 알수 없는 Type", expand.basic.ToString(), MessageBoxButton.OK);
                }
            }
            else
            {
                if (expand.script == null) return false;
                expand.script.Run(form);
                retn = expand.script.GetReturnValueColor();
            }

            return true;
        }

        int nBasicExpandOptionSizeWidth = 0;
        int nBasicExpandOptionSizeHeight = 0;
        int nBasicExpandOptionLocationX = 0;
        int nBasicExpandOptionLocationY = 0;
        protected int nBasicExpandOptionRotate = 0;   // 0 = 중심이 회전축, 1=LT, 2=RT, 3=LB, 4=RB

        protected virtual void LineColorChanged(Color color) { }
        protected virtual void FillColorChanged(Color color) { }
        protected virtual void TextColorChanged(Color color) { }
        protected virtual void BackColorChanged(Color color) { }
        protected virtual void LineThickChanged(double thick) { }
        
        protected bool ExpandRun(UserControl form, out EnumRunChangedType changed_type)
        {
            bool change_flag = false;
            changed_type = EnumRunChangedType.None;

            if (eID.active == 1)
            {	// 확장 옵션을 사용한다.
                double retn;
                int option = 0; // 옵션이 없는 경우

                if (RunExpandBasicAndScript(form, eID.expand.scriptSizeWidth, out retn, ref nBasicExpandOptionSizeWidth))
                {
                    if (fRunPercentWidth != (float)retn)
                    {
                        fRunPercentWidth = (float)retn;
                        change_flag = true;
                        changed_type |= EnumRunChangedType.Size;
                    }
                }
                if (RunExpandBasicAndScript(form, eID.expand.scriptSizeHeight, out retn, ref nBasicExpandOptionSizeHeight))
                {
                    if (fRunPercentHeight != (float)retn)
                    {
                        fRunPercentHeight = (float)retn;
                        change_flag = true;
                        changed_type |= EnumRunChangedType.Size;
                    }
                }
                if (RunExpandBasicAndScript(form, eID.expand.scriptLocationX, out retn, ref nBasicExpandOptionLocationX))
                {
                    if (nRunLocationX != (int)retn)
                    {
                        nRunLocationX = (int)retn;
                        change_flag = true;
                        changed_type |= EnumRunChangedType.Location;
                    }
                }

                if (RunExpandBasicAndScript(form, eID.expand.scriptLocationY, out retn, ref nBasicExpandOptionLocationY))
                {
                    if (nRunLocationY != (int)retn)
                    {
                        nRunLocationY = (int)retn;
                        change_flag = true;
                        changed_type |= EnumRunChangedType.Location;
                    }
                }

                if (RunExpandBasicAndScript(form, eID.expand.scriptVisible, out retn, ref option))
                {
                    bool visible = retn == 0 ? false : true;

                    if (bRunVisible != visible)
                    {
                        bRunVisible = visible;
                        change_flag = true;
                    }
                }

                if (RunExpandBasicAndScript(form, eID.expand.scriptBlinking, out retn, ref option))
                {
                    if (nBlinkingCycle != (int)retn)
                    {
                        nBlinkingCycle = (int)retn;
                        change_flag = true;

                        if (nBlinkingCycle == 0)
                        {	// 점멸을 사용하지 않는다.
                            bVisibleByBlinking = true;
                        }
                    }
                }

                if (RunExpandBasicAndScript(form, eID.expand.scriptAnimationSpeed, out retn, ref option))
                {
                    nRunAnimationSpeed = (int)retn;
                }

                if (eID.expand.sliderHorz != null)
                {
                    if (eID.expand.sliderHorz.nTagPos[0] != TagLib.TAG_NOT_FOUND)
                    {
                        TagAiClass ai = TagLib.GetStructAI(eID.expand.sliderHorz.sTag, ref eID.expand.sliderHorz.nTagPos);

                        double val = ai.curr;

                        if (eID.expand.sliderHorz.nWriteTimer > 0)
                        {
                            if (val == eID.expand.sliderHorz.fWriteValue)
                            {
                                eID.expand.sliderHorz.nWriteTimer = 0;	// 값이 확인되었다.
                            }
                            else
                            {
                                DateTime t = DateTime.Now;
                                if (t.Second != eID.expand.sliderHorz.nWriteTimerOldSec)
                                {
                                    eID.expand.sliderHorz.nWriteTimerOldSec = t.Second;
                                    eID.expand.sliderHorz.nWriteTimer--;
                                }
                                if (eID.expand.sliderHorz.nWriteTimer > 0)
                                    val = eID.expand.sliderHorz.fWriteValue;
                            }
                        }

                        if (eID.expand.sliderHorz.bUseFullBase == 1)
                        {
                            val = GetPercent((float)val, ai.fBase, ai.fFull,
                                (float)(-eID.expand.sliderHorz.nZoneMin), (float)eID.expand.sliderHorz.nZoneMax);
                        }
                        else
                        {
                            val = GetPercent((float)val, (float)eID.expand.sliderHorz.nValueMin, (float)eID.expand.sliderHorz.nValueMax,
                                (float)(-eID.expand.sliderHorz.nZoneMin), (float)eID.expand.sliderHorz.nZoneMax);
                        }

                        val += (nLeft < nRight ? nLeft : nRight);

                        if (nRunLocationX != val)
                        {
                            nRunLocationX = (int)val;
                            change_flag = true;
                            changed_type |= EnumRunChangedType.Location;
                        }
                    }
                }
                if (eID.expand.sliderVert != null)
                {
                    if (eID.expand.sliderVert.nTagPos[0] != TagLib.TAG_NOT_FOUND)
                    {
                        TagAiClass ai = TagLib.GetStructAI(eID.expand.sliderVert.sTag, ref eID.expand.sliderVert.nTagPos);

                        double val = ai.curr;

                        if (eID.expand.sliderVert.nWriteTimer > 0)
                        {
                            if (val == eID.expand.sliderVert.fWriteValue)
                            {
                                eID.expand.sliderVert.nWriteTimer = 0;	// 값이 확인되었다.
                            }
                            else
                            {
                                DateTime t = DateTime.Now;
                                if (t.Second != eID.expand.sliderVert.nWriteTimerOldSec)
                                {
                                    eID.expand.sliderVert.nWriteTimerOldSec = t.Second;
                                    eID.expand.sliderVert.nWriteTimer--;
                                }
                                if (eID.expand.sliderVert.nWriteTimer > 0)
                                    val = eID.expand.sliderVert.fWriteValue;
                            }
                        }

                        if (eID.expand.sliderVert.bUseFullBase == 1)
                        {
                            val = GetPercent((float)val, ai.fBase, ai.fFull,
                                (float)(-eID.expand.sliderVert.nZoneMin), (float)eID.expand.sliderVert.nZoneMax);
                        }
                        else
                        {
                            val = GetPercent((float)val, (float)eID.expand.sliderVert.nValueMin, (float)eID.expand.sliderVert.nValueMax,
                                (float)(-eID.expand.sliderVert.nZoneMin), (float)eID.expand.sliderVert.nZoneMax);
                        }
                        val += (nTop < nBottom ? nTop : nBottom);

                        if (nRunLocationY != val)
                        {
                            nRunLocationY = (int)val;
                            change_flag = true;
                            changed_type |= EnumRunChangedType.Location;
                        }
                    }
                }

                if (RunExpandBasicAndScript(form, eID.expand.scriptThickLine, out retn, ref option))
                {
                    if (nRunThickLine != (int)retn)
                    {
                        nRunThickLine = (int)retn;
                        change_flag = true;
                        changed_type |= EnumRunChangedType.Thick;
                    }
                }

                Color color;

                color = lLineColor; // 해당 조건이 없을 때는 default 색상을 돌려준다.
                if (RunExpandBasicAndScript(form, eID.expand.scriptColorLine, ref color))
                {
                    if (lRunColorLine != color)
                    {
                        lRunColorLine = color;
                        change_flag = true;

                        LineColorChanged(lRunColorLine);
                        //this.shapeOriginal.Stroke = new SolidColorBrush(lRunColorLine);
                    }
                }

                color = lFillColor.org_basic_color; // 해당 조건이 없을 때는 default 색상을 돌려준다.
                if (RunExpandBasicAndScript(form, eID.expand.scriptColorFill, ref color))
                {
                    if (lFillColor.basic_color != color)
                    {
                        lFillColor.basic_color = color;
                        change_flag = true;

                        FillColorChanged(lFillColor.basic_color);
                        //this.shapeOriginal.Fill = new SolidColorBrush(lRunColorFill);
                    }
                }

                color = lTextColor; // 해당 조건이 없을 때는 default 색상을 돌려준다.
                if (RunExpandBasicAndScript(form, eID.expand.scriptColorText, ref color))
                {
                    if (lRunColorText != color)
                    {
                        lRunColorText = color;
                        change_flag = true;

                        TextColorChanged(lRunColorText);
                    }
                }

                color = lBackColor.org_basic_color; // 해당 조건이 없을 때는 default 색상을 돌려준다.
                if (RunExpandBasicAndScript(form, eID.expand.scriptColorBack, ref color))
                {
                    if (lBackColor.basic_color != color)
                    {
                        lBackColor.basic_color = color;
                        change_flag = true;
                        BackColorChanged(lBackColor.basic_color);
                    }
                }

                if (RunExpandBasicAndScript(form, eID.expand.scriptRotate, out retn, ref nBasicExpandOptionRotate))
                {
                    if (fRunRotationAngle != (float)retn)
                    {
                        fRunRotationAngle = (float)retn;
                        change_flag = true;
                        changed_type |= EnumRunChangedType.Rotation;
                    }
                }
            }

            return change_flag;
        }

        // 두께가 영역 안쪽으로 굵어지는 특성을 가진 오브젝트는 보정해야 할 크기를 알려준다.
        protected virtual int ApplyGabByThick(int thick)
        {
            return 0;
        }

        protected void ExpandCalcObjectRect()
        {
            int x1, y1, x2, y2;
            int oldx1 = nViewX1;
            int oldy1 = nViewY1;
            int oldx2 = nViewX2;
            int oldy2 = nViewY2;
            int oldthick = nViewLineThick;

            // LocationX, Width 계산
            x1 = nLeft;
            x2 = nRight;
            y1 = nTop;
            y2 = nBottom;

            int width = Math.Abs(nRight - nLeft) + 1;
            int height = Math.Abs(nBottom - nTop) + 1;

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            // LocationX
            if (nBasicExpandOptionLocationX == 1)   // 오른쪽 방향
            {
                x1 = x1 + ExpandCalcLocationX(x1);
                x2 = x1 + (width - 1);
            }
            else if (nBasicExpandOptionLocationX == 2)   // 왼쪽 방향
            {
                x1 = x1 - ExpandCalcLocationX(x1);
                x2 = x1 + (width - 1);
            }
            else // 화면 좌표
            {
                x1 = ExpandCalcLocationX(x1);
                x2 = x1 + (width - 1);
            }

            // Size Width
            if (nBasicExpandOptionSizeWidth == 1)   // 중간에서
            {
                int calc_width = ExpandCalcSizeWidth(width);
                x1 = x1 + width / 2 - calc_width / 2;
                x2 = x1 + (calc_width - 1);
            }
            else if (nBasicExpandOptionSizeWidth == 2)   // 오른쪽에서
            {
                x1 = x2 - (ExpandCalcSizeWidth(width) - 1);
            }
            else // 왼쪽에서 - 기본 모드
            {
                x2 = x1 + (ExpandCalcSizeWidth(width) - 1);
            }

            // LocationY
            if (nBasicExpandOptionLocationY == 1)   // 아래쪽 방향
            {
                y1 = y1 + ExpandCalcLocationY(y1);
                y2 = y1 + (height - 1);
            }
            else if (nBasicExpandOptionLocationY == 2)   // 위쪽 방향
            {
                y1 = y1 - ExpandCalcLocationY(y1);
                y2 = y1 + (height - 1);
            }
            else // 화면 좌표
            {
                y1 = ExpandCalcLocationY(y1);
                y2 = y1 + (height - 1);
            }

            // Size Height
            if (nBasicExpandOptionSizeHeight == 1)   // 중간에서
            {
                int calc_height = ExpandCalcSizeHeight(height);
                y1 = y1 + height / 2 - calc_height / 2;
                y2 = y1 + (calc_height - 1);
            }
            else if (nBasicExpandOptionSizeHeight == 2)   // 밑에서
            {
                y1 = y2 - (ExpandCalcSizeHeight(height) - 1);
            }
            else // 위에서 - 기본 모드
            {
                y2 = y1 + (ExpandCalcSizeHeight(height) - 1);
            }

            nViewLineThick = GetViewSize(nRunThickLine);

            if (nViewLineThick > 3)
            {
                //int a = 10;
            }
            // 두께가 안쪽으로 좁아지는 사각형,원 같은 요소는 좌표의 보정이 필요하다.
            int gab = ApplyGabByThick(nViewLineThick);
            if (gab > 0)
            {
                x1 -= gab;
                y1 -= gab;
                x2 += gab;
                y2 += gab;
            }

            // 원래좌표의 방향대로 바꾸어 준다.
            if (nLeft > nRight) Tools.Temp(ref x1, ref x2);
            if (nTop > nBottom) Tools.Temp(ref y1, ref y2);

            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (enumObjectType == EnumObjectType.Group) // 10.0 부터 추가
                {
                    if (eID.active == 1)
                    {
                        ObjectGroup grp = (ObjectGroup)this;

                        RECT r = new RECT();

                        r.left = x1;
                        r.top = y1;
                        r.right = x2;
                        r.bottom = y2;

                        //grp.SetZoneAtPercent100(r);
                    }
                }
            }

            x1 = GetViewPosX(x1);
            y1 = GetViewPosY(y1);
            x2 = GetViewPosX(x2);
            y2 = GetViewPosY(y2);

            nViewX1 = x1;
            nViewY1 = y1;
            nViewX2 = x2;
            nViewY2 = y2;
            nViewLineThick = GetViewSize(nRunThickLine);

            /*
            // 변한것이 있으면 OnMove메시지를 날려준다.
            if ((oldx1 != nViewX1) || (oldy1 != nViewY1) || (oldx2 != nViewX2) || (oldy2 != nViewY2) || oldthick != nViewLineThick)
            {
                nViewOldX1 = oldx1;
                nViewOldY1 = oldy1;
                nViewOldX2 = oldx2;
                nViewOldY2 = oldy2;
                nViewOldLineThick = oldthick;
                OnMove(nViewX1, nViewY1, nViewX2, nViewY2);

                return true;
            }*/

            if ((oldx1 != nViewX1) || (oldy1 != nViewY1) || (oldx2 != nViewX2) || (oldy2 != nViewY2))
            {
                OnMove(nViewX1, nViewY1, nViewX2, nViewY2);
            }
            if (oldthick != nViewLineThick)
            {
                LineThickChanged(nViewLineThick);
            }

        }

        //int nViewOldX1 = 0, nViewOldY1 = 0, nViewOldX2 = 0, nViewOldY2 = 0, nViewOldLineThick = 0; // 좌표가 바뀌기 전의 값

        FrameworkElement shapeOriginal=null;

        RotateTransform transformRotate = null;

        protected void SetShapeOriginal(FrameworkElement child)
        {
            shapeOriginal = child;

            if (!IsNeedMouseHitTest()) shapeOriginal.IsHitTestVisible = false;

            OnVisible(ExpandCalcVisible());     // 초기 상태의 Visible을 설정한다. 이것을 하지 않으면 초기에 대부분이 Visble상태가 된다.

            child.RenderTransform = new TransformGroup();

            if (eID.expand.scriptRotate != null || objGeneral.fRotateAngle != 0)
            {
                transformRotate = new RotateTransform();
                ((TransformGroup)child.RenderTransform).Children.Add(transformRotate);
            }
            
            if(RotationAngle != 0) {    // 편집 상태에서 부터 회전이 적용되었다.
                ApplyRotateTransform();   
            }
        }

        protected FrameworkElement GetShapeOriginal()
        {
            return shapeOriginal;
        }

        public virtual void OnMove(int x1, int y1, int x2, int y2)
        {
            if (shapeOriginal == null) return;

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            Canvas.SetLeft(shapeOriginal, x1);
            Canvas.SetTop(shapeOriginal, y1);
            shapeOriginal.Width = x2-x1+1;
            shapeOriginal.Height = y2-y1+1;
        }

        public void MoveShape()
        {
            int x1 = nViewX1;
            int y1 = nViewY1;
            int x2 = nViewX2;
            int y2 = nViewY2;

            OnMove(x1, y1, x2, y2);
        }

        /// <summary>
        /// 마우스 응답을 해야하는 경우는 override 해서 true를 반환한다.
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsNeedMouseHitTest()
        {
            if (eID.active == 1)
            {
                if (eID.expand.structMouseZone != null) return true;
                if (eID.expand.scriptMouseDown != null) return true;
                if (eID.expand.scriptMouseUp != null) return true;
                if (eID.expand.sliderHorz != null) return true;
                if (eID.expand.sliderVert != null) return true;
            }

            return false;
        }

        /// <summary>
        /// 원래 ObjectTag에 있는 함수인데 9.3.5 부터 이쪽으로 이전
        /// </summary>
        /// <returns></returns>
        protected bool CheckResponseOnVisible()
        {
            if (objGeneral.bResponseOnVisible == false) return true;

            if (bRunVisible) return true;

            return false;
        }

        public virtual bool WmLeftButtonDown(UserControl form, MouseEventArgs e)
        {
            if (shapeOriginal == null) return false;    // 맨처음에 있는 Layer는 검사할 필요 없다.

            if (!CheckResponseOnVisible()) return false;

            int sx, sy;
            int vx1, vx2, vy1, vy2;

            Canvas parent = (Canvas)this.GetShapeOriginal().Parent;

            //System.Windows.Point position = e.GetPosition(form);
            System.Windows.Point position = e.GetPosition((Canvas)this.GetShapeOriginal().Parent);//objCommonProperty.rootPage.LayoutRoot);    // Transform이 적용된 오브젝트인 경우 해당 canvas를 사용한다.

            sx = (int)position.X;
            sy = (int)position.Y;

            vx1 = nViewX1;
            vy1 = nViewY1;
            vx2 = nViewX2;
            vy2 = nViewY2;

            if (vx1 > vx2) Tools.Temp(ref vx1, ref vx2);
            if (vy1 > vy2) Tools.Temp(ref vy1, ref vy2);

            if (sx < vx1 || sy < vy1 || sx > vx2 || sy > vy2) return false;

            if (eID.active == 1 && eID.expand.scriptMouseDown != null)
            {
                eID.expand.scriptMouseDown.SetHandOperation();	// 수동으로 출력한다.
                eID.expand.scriptMouseDown.Run(form);
                if (eID.expand.scriptMouseDown.IsError())
                {
                    string message;
                    message = eID.expand.scriptMouseDown.GetError();
                    MessageBox.Show(message, "Mouse Down Script Error", MessageBoxButton.OK);
                }

                if (eID.expand.scriptMouseUp != null)
                {	  // 마우스 Up 스크립트가 있을 때만.
                    parent.CaptureMouse();
                    bMouseCapture = true;
                }

                return true;
            }
            else
            {
                if (eID.active == 1 && eID.expand.scriptMouseUp != null)
                {
                    parent.CaptureMouse();
                    bMouseCapture = true;
                    return true;
                }
            }

            bool retn = false;

            if (eID.active == 1 && eID.expand.sliderHorz != null)
            {
                TagAiClass ai = TagLib.GetStructAI(eID.expand.sliderHorz.sTag, ref eID.expand.sliderHorz.nTagPos);
                
                nSliderGabX = sx - vx1;
                nSliderGabY = sy - vy1;

                parent.CaptureMouse();
                bMouseCapture = true;
                ai.bScrollBarMoving = true;
                bHorizontalScrollDataChanged = false;

                retn = true;
            }
            if (eID.active == 1 && eID.expand.sliderVert != null)
            {
                TagAiClass ai = TagLib.GetStructAI(eID.expand.sliderVert.sTag, ref eID.expand.sliderVert.nTagPos);

                nSliderGabX = sx - vx1;
                nSliderGabY = sy - vy1;

                parent.CaptureMouse();
                bMouseCapture = true;
                ai.bScrollBarMoving = true;
                bVerticalScrollDataChanged = false;

                retn = true;
            }

            return retn;
        }

        public virtual bool WmLeftButtonUp(UserControl form, MouseEventArgs e)
        {
            if (bMouseCapture == false) return false;

            Canvas parent = (Canvas)this.GetShapeOriginal().Parent;

            if (eID.active == 1 && eID.expand.scriptMouseUp != null)
            {
                eID.expand.scriptMouseUp.SetHandOperation();	// 수동으로 출력한다.
                eID.expand.scriptMouseUp.Run(form);
                if (eID.expand.scriptMouseUp.IsError())
                {
                    string message;
                    message = eID.expand.scriptMouseUp.GetError();
                }
            }

            if (eID.active == 1 && eID.expand.sliderHorz != null)
            {
                TagAiClass ai = TagLib.GetStructAI(eID.expand.sliderHorz.sTag, ref eID.expand.sliderHorz.nTagPos);

                ai.bScrollBarMoving = false;

                if (bHorizontalScrollDataChanged)
                {
                    TagWrite.WriteCurrAI(eID.expand.sliderHorz.sTag, ai, ai.curr, true);
                }
            }
            if (eID.active == 1 && eID.expand.sliderVert != null)
            {
                TagAiClass ai = TagLib.GetStructAI(eID.expand.sliderVert.sTag, ref eID.expand.sliderVert.nTagPos);

                ai.bScrollBarMoving = false;

                if (bVerticalScrollDataChanged)
                {
                    TagWrite.WriteCurrAI(eID.expand.sliderVert.sTag, ai, ai.curr, true);
                }
            }

            
            bMouseCapture = false;
            parent.ReleaseMouseCapture();

            return true;
        }

        Grid canvas_mousezone = null;

        void MakeMouseZone(Canvas parent)
        {
            if (eID.active == 1 && eID.expand.structMouseZone != null)
            {
                Grid canvas = new Grid();

                int vx1, vx2, vy1, vy2;

                vx1 = nViewX1;
                vy1 = nViewY1;
                vx2 = nViewX2;
                vy2 = nViewY2;

                if (vx1 > vx2) Tools.Temp(ref vx1, ref vx2);
                if (vy1 > vy2) Tools.Temp(ref vy1, ref vy2);

                vx1 -= eID.expand.structMouseZone.x1 + 1;
                vy1 -= eID.expand.structMouseZone.y1 + 1;
                vx2 += eID.expand.structMouseZone.x2 + 1;
                vy2 += eID.expand.structMouseZone.y2 + 1;

                canvas.Width = vx2 - vx1 + 1;
                canvas.Height = vy2 - vy1 + 1;

                Canvas.SetLeft(canvas, vx1);
                Canvas.SetTop(canvas, vy1);

                Rectangle r = new Rectangle();
                r.Margin = new Thickness(0, 0, 0, 0);
                r.Stroke = new SolidColorBrush(Colors.Black);
                canvas.Children.Add(r);

                r = new Rectangle();
                r.Margin = new Thickness(1, 1, 1, 1);
                r.Stroke = new SolidColorBrush(Colors.White);
                canvas.Children.Add(r);

                r = new Rectangle();
                r.Margin = new Thickness(2, 2, 2, 2);
                r.Stroke = new SolidColorBrush(Colors.Black);
                canvas.Children.Add(r);

                canvas_mousezone = canvas;

                canvas.IsHitTestVisible = false;
                parent.Children.Add(canvas);
            }
        }

        bool bHorizontalScrollDataChanged = false;
        bool bVerticalScrollDataChanged = false;

        void ScrollTagValueChanged(TagAiClass ai, long value, ref bool changed)
        {
            if ((double)value != ai.curr)
            {
                ai.curr = value;
                changed = true;
            }
        }

        public virtual bool WmMouseMove(UserControl form, MouseEventArgs e)
        {
            if (shapeOriginal == null) return false;    // 맨처음에 있는 Layer는 검사할 필요 없다.
            // ToolTipCheck(form, e);	// ToolTip 체크

            //System.Windows.Point position = e.GetPosition(form);
            //System.Windows.Point position = e.GetPosition(objCommonProperty.rootPage.LayoutRoot);    // Transform이 적용된 오브젝트인 경우 해당 canvas를 사용한다.
            System.Windows.Point position = e.GetPosition((Canvas)(this.GetShapeOriginal().Parent));    // Transform이 적용된 오브젝트인 경우 해당 canvas를 사용한다.

            int sx = (int)position.X;
            int sy = (int)position.Y;

            if (eID.active == 1 && eID.expand.structMouseZone != null)
            {
                int vx1, vx2, vy1, vy2;

                vx1 = nViewX1;
                vy1 = nViewY1;
                vx2 = nViewX2;
                vy2 = nViewY2;

                if (vx1 > vx2) Tools.Temp(ref vx1, ref vx2);
                if (vy1 > vy2) Tools.Temp(ref vy1, ref vy2);

                if (sx >= vx1 && sy >= vy1 && sx <= vx2 && sy <= vy2)
                {
                    if (!bOnMouseZone)
                    {
                        bOnMouseZone = true;
                        //MakeMouseZone(objCommonProperty.rootPage.LayoutRoot);
                        MakeMouseZone((Canvas)this.GetShapeOriginal().Parent);
                    }
                }
                else
                {
                    if (bOnMouseZone)
                    {
                        bOnMouseZone = false;
                        //objCommonProperty.rootPage.LayoutRoot.Children.Remove(canvas_mousezone);
                        ((Canvas)this.GetShapeOriginal().Parent).Children.Remove(canvas_mousezone);
                        canvas_mousezone = null;
                    }
                }
            }

            /*
            position = e.GetPosition((Canvas)(this.GetShapeOriginal().Parent));    // Transform이 적용된 오브젝트인 경우 해당 canvas를 사용한다.
            sx = (int)position.X;
            sy = (int)position.Y;*/

            if (bMouseCapture == false) return false;

            if (eID.active == 1 && eID.expand.sliderHorz != null)
            {
                if (eID.expand.sliderHorz.nTagPos[0] != TagLib.TAG_NOT_FOUND)
                {
                    int left = nLeft < nRight ? nLeft : nRight;
                    int x1 = GetViewPosX(left - eID.expand.sliderHorz.nZoneMin) + nSliderGabX;
                    int x2 = GetViewPosX(left + eID.expand.sliderHorz.nZoneMax) + nSliderGabX;

                    long val;

                    TagAiClass ai = TagLib.GetStructAI(eID.expand.sliderHorz.sTag, ref eID.expand.sliderHorz.nTagPos);

                    if (eID.expand.sliderHorz.bUseFullBase == 1)
                        val = GetPercent((float)sx, (float)x1, (float)x2, (float)ai.fBase, (float)ai.fFull);
                    else
                        val = GetPercent((float)sx, (float)x1, (float)x2, (float)eID.expand.sliderHorz.nValueMin, (float)eID.expand.sliderHorz.nValueMax);

                    if (SharedData.userInfo.HaveRightsHandOperationAndMsgAtScript(ai))
                    {
                        //TagWrite.WriteCurrAI(eID.expand.sliderHorz.sTag, ai, val, true);
                        ScrollTagValueChanged(ai, val, ref bHorizontalScrollDataChanged);

                        DateTime t = DateTime.Now;
                        eID.expand.sliderHorz.nWriteTimer = 5;
                        eID.expand.sliderHorz.nWriteTimerOldSec = t.Second;
                        eID.expand.sliderHorz.fWriteValue = val;

                        EventTimer(form);	// 메모리 태그일 경우 바의 위치를 바로 결정하기 위해서 한번 실행

                    }
                }
            }

            if (eID.active == 1 && eID.expand.sliderVert != null)
            {
                if (eID.expand.sliderVert.nTagPos[0] != TagLib.TAG_NOT_FOUND)
                {
                    int top = nTop < nBottom ? nTop : nBottom;
                    int y1 = GetViewPosY(top - eID.expand.sliderVert.nZoneMin) + nSliderGabY;
                    int y2 = GetViewPosY(top + eID.expand.sliderVert.nZoneMax) + nSliderGabY;

                    long val;

                    TagAiClass ai = TagLib.GetStructAI(eID.expand.sliderVert.sTag, ref eID.expand.sliderVert.nTagPos);

                    if (eID.expand.sliderVert.bUseFullBase == 1)
                        val = GetPercent((float)sy, (float)y1, (float)y2, (float)ai.fBase, (float)ai.fFull);
                    else
                        val = GetPercent((float)sy, (float)y1, (float)y2, (float)eID.expand.sliderVert.nValueMin, (float)eID.expand.sliderVert.nValueMax);

                    if (SharedData.userInfo.HaveRightsHandOperationAndMsgAtScript(ai))
                    {
                        //TagWrite.WriteCurrAI(eID.expand.sliderVert.sTag, ai, val, true);
                        ScrollTagValueChanged(ai, val, ref bVerticalScrollDataChanged);

                        DateTime t = DateTime.Now;
                        eID.expand.sliderVert.nWriteTimer = 5;
                        eID.expand.sliderVert.nWriteTimerOldSec = t.Second;
                        eID.expand.sliderVert.fWriteValue = val;

                        EventTimer(form);	// 메모리 태그일 경우 바의 위치를 바로 결정하기 위해서 한번 실행
                    }
                }
            }

            return true;
        }

        /*
        static bool bToolTipCheckStart = false;
        static object pToolTipOwner = null;
        static bool bToolTipViewStatus = false;
        static System.Windows.Controls.Primitives.Popup oToolTip = new System.Windows.Controls.Primitives.Popup();
        //static UserControl pToolTipForm = null;

        public static void ToolTipCheckStart()
        {
            //if (!ConfigViewMain.bDisplayToolTip) return;

            bToolTipCheckStart = false;
        }

        public static void ToolTipCheckEnd()
        {
            //if (!ConfigViewMain.bDisplayToolTip) return;

            if (bToolTipCheckStart == false) // 툴팁 디스프레이가 해당되는 오브젝트가 없다.
            {
                //if (pToolTipForm != null)
                //{
                    oToolTip.Visibility = Visibility.Collapsed;
                    //oToolTip.SetToolTip(pToolTipForm, "");
                //}
                bToolTipViewStatus = false;
            }
        }

        static ScriptClass scriptCalc = new ScriptClass();

        public virtual string GetToolTipString()
        {
            if (TotalConfig.defineMode == EnumDefineMode.MODE_RUN)
            {
                if (this.objGeneral.sObjectDescription.Length > 0 && this.objGeneral.sObjectDescription[0] == '=')
                {
                    object val;
                    scriptCalc.GetValueRecurse(this.objGeneral.sObjectDescription.Substring(1), out val);

                    return scriptCalc.GetValueString(val);
                }
                else
                {
                    return this.objGeneral.sObjectDescription;
                }
            }
            else
            {
                return this.objGeneral.sObjectDescription;
            }
        }

        /// <summary>
        /// 자체윈도우가 있는 요소는 해당되는 차일드 폼을 연결해 준다.
        /// </summary>
        public void SetToolTipOnChildWindow(Control child)
        {
            //oToolTip.InitialDelay = 0;
            //oToolTip.ShowAlways = true;

            //oToolTip.SetToolTip(child, GetToolTipString());
        }

        protected void ToolTipCheck(UserControl form, MouseEventArgs e)
        {
            //if (!ConfigViewMain.bDisplayToolTip) return;

            if (bToolTipCheckStart == false && this.objGeneral.bUseToolTip == 1)	// 툴팁 보여주기
            {
                int vx1, vx2, vy1, vy2;

                vx1 = nViewX1;
                vy1 = nViewY1;
                vx2 = nViewX2;
                vy2 = nViewY2;

                if (vx1 > vx2) Tools.Temp(ref vx1, ref vx2);
                if (vy1 > vy2) Tools.Temp(ref vy1, ref vy2);

                Point point = e.GetPosition(objCommonProperty.rootCanvas);
                int sx = (int)point.X;
                int sy = (int)point.Y;

                if (sx>= vx1 && sy >= vy1 && sx <= vx2 && sy <= vy2)
                {
                    if (!bToolTipViewStatus)
                    {
                        bToolTipViewStatus = true;
                        oToolTip.Visibility = Visibility.Visible;
                        TextBlock t = new TextBlock();
                        t.Text = GetToolTipString();
                        oToolTip.Child = t;
                        //oToolTip.SetToolTip(form, GetToolTipString());
                        //oToolTip.InitialDelay = 0;
                        //oToolTip.ShowAlways = true;
                        pToolTipOwner = this;
                        //pToolTipForm = form;
                    }
                    else
                    {
                        if (pToolTipOwner != this)	// 오브젝트가 바뀌었다.
                        {
                            //oToolTip.SetToolTip(form, this.GetToolTipString());
                            pToolTipOwner = this;
                            //pToolTipForm = form;
                        }
                    }

                    bToolTipCheckStart = true;
                }
                else
                {

                }
            }
        }*/

        

        /*
        public virtual bool WmRightButtonDown(System.Windows.Forms.Form form, MouseEventArgs e)
        {
            return false;
        }

        public virtual bool WmRightButtonUp(System.Windows.Forms.Form form, MouseEventArgs e)
        {
            return false;
        }
        */
        public void GetZone(ref RECT r)
        {
            r.left = nLeft;
            r.top = nTop;
            r.right = nRight;
            r.bottom = nBottom;
        }
        /*
        public virtual void GetZone(ref int x1, ref int y1, ref int x2, ref int y2)
        {
            x1 = nLeft;
            y1 = nTop;
            x2 = nRight;
            y2 = nBottom;
        }
        */
        public void GetViewZone(ref int x1, ref int y1, ref int x2, ref int y2)
        {
            x1 = nViewX1;
            y1 = nViewY1;
            x2 = nViewX2;
            y2 = nViewY2;
        }

        /*
        // 회전이 적용된 ViewZone
        protected void GetViewZoneWithRotation(ref int x1, ref int y1, ref int x2, ref int y2)
        {
            if (RotationAngle == 0)
            {
                x1 = nViewX1;
                y1 = nViewY1;
                x2 = nViewX2;
                y2 = nViewY2;
            }
            else
            {
                x1 = nViewX1 - nRotationCenterX;
                y1 = nViewY1 - nRotationCenterY;
                x2 = nViewX2 - nRotationCenterX;
                y2 = nViewY2 - nRotationCenterY;
            }
        }
        */
        public virtual void UpdateZone(int x1, int y1, int x2, int y2)
        {
            nLeft = x1;
            nTop = y1;
            nRight = x2;
            nBottom = y2;

            ExpandCalcObjectRect();	// 보여줄 사각형을 미리 계산해 놓는다.
        }

        public void OnVisible(bool flag) 	// 이것은 파생 클래스가 Child윈도우로 구성될 때 Window를 Show/Hide 하기위해서 필요하다.
        {
            this.shapeOriginal.Visibility = flag ? Visibility.Visible : Visibility.Collapsed;
        }

        protected virtual void ApplyRotateTransform()
        {
            if (transformRotate == null) return;

            int cx;
            int cy;

            CalcRotationCenter(out cx, out cy);

            cx = nViewX1 > nViewX2 ? cx - nViewX2 : cx - nViewX1;
            cy = nViewY1 > nViewY2 ? cy - nViewY2 : cy - nViewY1;

            nRotationCenterX = cx;
            nRotationCenterY = cy;

            RotateTransform rotate = transformRotate;// new RotateTransform();
            rotate.CenterX = cx;
            rotate.CenterY = cy;
            rotate.Angle = this.RotationAngle;
            //this.shapeOriginal.RenderTransform = rotate;
        }
         
        public virtual void EventTimer(UserControl form)
        {
            if (eID.active == 1)
            {
                bool old_visible = ExpandCalcVisible();
                EnumRunChangedType changed_type;

                if (ExpandRun(form, out changed_type))
                {	// 변화가 있었다.
                    // 각 변화별로 속도를 개선하기 위해서 필요한 동작만 한다.
                    if ((changed_type & EnumRunChangedType.Location) > 0 ||
                        (changed_type & EnumRunChangedType.Size) > 0 ||
                        (changed_type & EnumRunChangedType.Thick) > 0)
                    {
                        ExpandCalcObjectRect();
                    }

                    if ((changed_type & EnumRunChangedType.Rotation) > 0)
                    {
                        ApplyRotateTransform();
                        /*
                        int cx;
                        int cy;

                        CalcRotationCenter(out cx, out cy);

                        cx = nViewX1 > nViewX2 ? cx - nViewX2 : cx - nViewX1;
                        cy = nViewY1 > nViewY2 ? cy - nViewY2 : cy - nViewY1;

                        nRotationCenterX = cx;
                        nRotationCenterY = cy;

                        RotateTransform rotate = new RotateTransform();
                        rotate.CenterX = cx;
                        rotate.CenterY = cy;
                        rotate.Angle = this.RotationAngle;
                        this.shapeOriginal.RenderTransform = rotate;*/
                    }

                    if ((changed_type & EnumRunChangedType.Thick) > 0)
                    {
                        //this.shapeOriginal.StrokeThickness = nRunThickLine;
                    }
                }

                if (old_visible != ExpandCalcVisible())
                {	
                    OnVisible(ExpandCalcVisible());
                }

                if (eID.expand.scriptBlinking != null)
                {	// 점멸 옵션을 사용중일때는 각 주기가 되면 점멸을 시도한다.
                    if (nBlinkingCycle > 0 && bRunVisible)
                    {
                        DateTime time = new DateTime();
                        int mili;
                        bool visible;

                        time = DateTime.Now;

                        mili = time.Millisecond + time.Second * 1000 + time.Minute * 1000 * 60 + time.Hour * 1000 * 60 * 60;
                        mili %= nBlinkingCycle;

                        if ((int)mili < nBlinkingCycle / 2) visible = true;
                        else visible = false;

                        if (bVisibleByBlinking != visible)
                        {
                            bVisibleByBlinking = visible;
                            OnVisible(ExpandCalcVisible());
                        }
                    }
                }

            }

            EventTimerObject(form);
        }

        public virtual void EventTimerObject(UserControl form) { }

        /*
        public void OnEventKeyDown(int code)
        {
            if (eID.active == 1 && eID.expand.scriptEventKeyDown != null)
            {
                eID.expand.scriptEventKeyDown.nKeyValue = code;
                eID.expand.scriptEventKeyDown.SetHandOperation();	// 수동으로 출력한다.
                eID.expand.scriptEventKeyDown.Run(TotalConfig.formMain);
                if (eID.expand.scriptEventKeyDown.IsError())
                {
                    string msg = eID.expand.scriptEventKeyDown.GetError();
                    MessageBox.Show(msg, "Error:EventKeyDown");
                }
            }
        }
        */
        public void OnEventSelChange()
        {
            if (eID.active == 1 && eID.expand.scriptEventSelChange != null)
            {
                eID.expand.scriptEventSelChange.SetHandOperation();	// 수동으로 출력한다.
                eID.expand.scriptEventSelChange.Run(objCommonProperty.rootPage);
                if (eID.expand.scriptEventSelChange.IsError())
                {
                    string msg = eID.expand.scriptEventSelChange.GetError();
                    MessageBox.Show(msg, "Error:EventSelChange", MessageBoxButton.OK);
                }
            }
        }
        /*
        void DrawMouseZone(Graphics g, int x1, int y1, int x2, int y2)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            x1 -= eID.expand.structMouseZone.x1;
            y1 -= eID.expand.structMouseZone.y1;
            x2 += eID.expand.structMouseZone.x2;
            y2 += eID.expand.structMouseZone.y2;

            Pen hPenWhite = new Pen(Color.White, 1);
            Pen hPenBlack = new Pen(Color.Black, 1);

            g.DrawLine(hPenWhite, x1, y1, x2, y1);
            g.DrawLine(hPenWhite, x2, y1, x2, y2);
            g.DrawLine(hPenWhite, x2, y2, x1, y2);
            g.DrawLine(hPenWhite, x1, y2, x1, y1);

            g.DrawLine(hPenBlack, x1 - 1, y1 - 1, x2 + 1, y1 - 1);
            g.DrawLine(hPenBlack, x2 + 1, y1 - 1, x2 + 1, y2 + 1);
            g.DrawLine(hPenBlack, x2 + 1, y2 + 1, x1 - 1, y2 + 1);
            g.DrawLine(hPenBlack, x1 - 1, y2 + 1, x1 - 1, y1 - 1);

            g.DrawLine(hPenBlack, x1 + 1, y1 + 1, x2 - 1, y1 + 1);
            g.DrawLine(hPenBlack, x2 - 1, y1 + 1, x2 - 1, y2 - 1);
            g.DrawLine(hPenBlack, x2 - 1, y2 - 1, x1 + 1, y2 - 1);
            g.DrawLine(hPenBlack, x1 + 1, y2 - 1, x1 + 1, y1 + 1);
        }

        public bool IsMouseInViewZone(MouseEventArgs e)
        {
            int x1 = nViewX1;
            int x2 = nViewX2;
            int y1 = nViewY1;
            int y2 = nViewY2;

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            if (e.X < x1) return false;
            if (e.Y < y1) return false;
            if (e.X > x2) return false;
            if (e.Y > y2) return false;

            return true;
        }

        protected virtual bool IsNeedPaint(Rectangle r, int x1, int y1, int x2, int y2)
        {
            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            if (x1 > r.Right) return false;
            if (y1 > r.Bottom) return false;
            if (x2 < r.Left) return false;
            if (y2 < r.Top) return false;

            return true;
        }
         */

        private int nRotationCenterX = 0;
        private int nRotationCenterY = 0;

        protected virtual void CalcRotationCenter(out int cx, out int cy)
        {
            if (this.nBasicExpandOptionRotate == 1) // 회전축이 Left,Top
            {
                cx = (nViewX1 < nViewX2) ? nViewX1 : nViewX2;
                cy = (nViewY1 < nViewY2) ? nViewY1 : nViewY2;
            }
            else if (this.nBasicExpandOptionRotate == 2) // 회전축이 RIght,Top 
            {
                cx = (nViewX1 < nViewX2) ? nViewX2 : nViewX1;
                cy = (nViewY1 < nViewY2) ? nViewY1 : nViewY2;
            }
            else if (this.nBasicExpandOptionRotate == 3)// 회전축이 Left,Bottom
            {
                cx = (nViewX1 < nViewX2) ? nViewX1 : nViewX2;
                cy = (nViewY1 < nViewY2) ? nViewY2 : nViewY1;
            }
            else if (this.nBasicExpandOptionRotate == 4)// 회전축이 RIght,Bottom 
            {
                cx = (nViewX1 < nViewX2) ? nViewX2 : nViewX1;
                cy = (nViewY1 < nViewY2) ? nViewY2 : nViewY1;
            }
            else // 회전축이 중심
            {
                int w = Math.Abs(nViewX2 - nViewX1) + 1;
                int h = Math.Abs(nViewY2 - nViewY1) + 1;

                cx = (nViewX1 > nViewX2) ? nViewX2 + w / 2 : nViewX1 + w / 2;
                cy = (nViewY1 > nViewY2) ? nViewY2 + h / 2 : nViewY1 + h / 2;
            }
        }

        /*
        public virtual void Display(Graphics g, Rectangle rcPaint, int originx, int originy)
        {
            if (!ExpandCalcVisible()) return;

            if (RotationAngle != 0) // 회전이 적용 되었다.
            {
                int cx;
                int cy;

                CalcRotationCenter(out cx, out cy);

                nRotationCenterX = cx;
                nRotationCenterY = cy;

                g.TranslateTransform(cx, cy);
                g.RotateTransform(RotationAngle);

                DisplayObject(g, nViewX1 - cx, nViewY1 - cy, nViewX2 - cx, nViewY2 - cy, nViewLineThick);

                if (eID.active == 1 && eID.expand.structMouseZone != null && bOnMouseZone)
                {
                    DrawMouseZone(g, nViewX1 - cx, nViewY1 - cy, nViewX2 - cx, nViewY2 - cy);
                }
                g.ResetTransform();
            }
            else
            {
                DisplayObject(g, nViewX1 - originx, nViewY1 - originy, nViewX2 - originx, nViewY2 - originy, nViewLineThick);

                if (eID.active == 1 && eID.expand.structMouseZone != null && bOnMouseZone)
                {
                    DrawMouseZone(g, nViewX1 - originx, nViewY1 - originy, nViewX2 - originx, nViewY2 - originy);
                }
            }
        }

        public override void SetBasePoint(int x, int y)
        {
            base.SetBasePoint(x, y);

            ExpandCalcObjectRect();
        }

        public override void SetScreenSize(int x, int y)
        {
            base.SetScreenSize(x, y);

            ExpandCalcObjectRect();
        }

        public override void SetOpticRate(int rate)
        {
            base.SetOpticRate(rate);

            ExpandCalcObjectRect();
        }
        */
        public override void SetZoneAtPercent100(SIZE size, RECT r)
        {
            base.SetZoneAtPercent100(size, r);

            ExpandCalcObjectRect();
        }
        /*
        public virtual void DisplayObject(Graphics g, int x1, int y1, int x2, int y2, int bthick)
        {

        }
        */
        public void SetBorderThick(int thick)
        {
            wBorderThick = thick;
            nRunThickLine = thick;
            ExpandCalcObjectRect();
        }
        /*
        public void InvalidateOldObject(System.Windows.Forms.Form form)
        {
            int gab = nViewOldLineThick / 2;
            int x1, y1, x2, y2;

            x1 = nViewOldX1;
            y1 = nViewOldY1;
            x2 = nViewOldX2;
            y2 = nViewOldY2;

            if (x1 > x2) Tools.Temp(ref x1, ref x2);
            if (y1 > y2) Tools.Temp(ref y1, ref y2);

            x1 -= gab;
            y1 -= gab;
            x2 += gab;
            y2 += gab;

            if (eID.active == 1 && eID.expand.structMouseZone != null)
            {
                x1 -= (eID.expand.structMouseZone.x1 + 1);
                y1 -= (eID.expand.structMouseZone.y1 + 1);
                x2 += (eID.expand.structMouseZone.x2 + 1);
                y2 += (eID.expand.structMouseZone.y2 + 1);
            }

            Rectangle rc = new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);

            form.Invalidate(rc);
        }
        */
        public void InvalidateObject(UserControl form)
        {
            /*
            if (RotationAngle == 0)
            {
                int gab = nViewLineThick / 2;
                int x1, y1, x2, y2;

                x1 = nViewX1;
                y1 = nViewY1;
                x2 = nViewX2;
                y2 = nViewY2;

                if (x1 > x2) Tools.Temp(ref x1, ref x2);
                if (y1 > y2) Tools.Temp(ref y1, ref y2);

                x1 -= gab;
                y1 -= gab;
                x2 += gab;
                y2 += gab;

                if (eID.active == 1 && eID.expand.structMouseZone != null)
                {
                    x1 -= (eID.expand.structMouseZone.x1 + 1);
                    y1 -= (eID.expand.structMouseZone.y1 + 1);
                    x2 += (eID.expand.structMouseZone.x2 + 1);
                    y2 += (eID.expand.structMouseZone.y2 + 1);
                }

                Rectangle rc = new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);

                form.Invalidate(rc);
            }
            else
            {
                form.Invalidate();  // 전체를 한다.
            }*/
        }

        public void SetLineColor(Color color)
        {
            lLineColor = color;
            lRunColorLine = color;
        }

        public void SetFillColor(BrushPublic color)
        {
            lFillColor = color;
            lFillColor.org_basic_color = lFillColor.basic_color;    // run time시 basic_color가 바뀌므로 기본색상을 기억해 둔다.
            //lRunColorFill = color;
        }

        public void SetTextColor(Color color)
        {
            lTextColor = color;
            lRunColorText = color;
        }

        public void SetBackColor(BrushPublic color)
        {
            lBackColor = color;
            lBackColor.org_basic_color = lBackColor.basic_color;    // run time시 basic_color가 바뀌므로 기본색상을 기억해 둔다.
            //lRunColorBack = color;
        }
        /*
        ScriptClass LoadExpandScriptFromMod(Profile profile, string ext, uint id)
        {
            if (id == 0) return null;

            ScriptClass control;

            control = null;

            if (profile.GetIntFromReadyMemory(ext, "Active", 0) == 0) return null;

            string scriptfile = "";

            MakeScriptPath(ref scriptfile, ext, id);

            if (!File.Exists(scriptfile)) return null;

            control = new ScriptClass();
            if (control == null) return null;

            control.LoadFromFile(scriptfile);

            return control;
        }

        // MOD 파일은 스크립트 밖에 없다.
        ExpandBasicAndScript LoadExpandBasicAndScriptFromMod(Profile profile, string ext, uint id)
        {
            ExpandBasicAndScript expand = new ExpandBasicAndScript();

            expand.eExpandType = EnumExpandType.Script; // MOD 파일은 항상 스크립트 형이다.
            expand.script = LoadExpandScriptFromMod(profile, ext, id);

            return expand;
        }

        EXPAND_SLIDER_STRUCT LoadExpandSlider(Profile profile, string ext, uint id)
        {
            if (id == 0) return null;

            EXPAND_SLIDER_STRUCT slider;

            slider = null;

            if (profile.GetIntFromReadyMemory(ext, "Active", 0) == 0) return null;

            string filename = "";

            MakeScriptPath(ref filename, ext, id);

            if (!File.Exists(filename)) return null;

            slider = new EXPAND_SLIDER_STRUCT();
            if (slider == null) return null;

            ExpandLoadSliderFile(filename, slider);

            return slider;
        }

        EXPAND_MOUSE_ZONE LoadExpandMouseZone(Profile profile, string ext, uint id)
        {
            if (id == 0) return null;

            EXPAND_MOUSE_ZONE zone;

            zone = null;

            if (profile.GetIntFromReadyMemory(ext, "Active", 0) == 0) return null;

            string filename = "";

            MakeScriptPath(ref filename, ext, id);

            if (!File.Exists(filename)) return null;

            zone = new EXPAND_MOUSE_ZONE();
            if (zone == null) return null;

            ExpandLoadMouseZoneFile(filename, zone);

            return zone;
        }*/

        // MOD버전에서.
        // 태그를 사용하는 오브젝트에서 Mouse 반응을 하기위해서 존재한다.
        protected void AddMouseZone(int x1, int y1, int x2, int y2)
        {
            eID.expand.structMouseZone = new EXPAND_MOUSE_ZONE();
            eID.expand.structMouseZone.x1 = x1;
            eID.expand.structMouseZone.y1 = y1;
            eID.expand.structMouseZone.x2 = x2;
            eID.expand.structMouseZone.y2 = y2;
            eID.expand.structMouseZone.bLock = 1;
            eID.active = 1;
        }

        /*
        public virtual void EventTag(System.Windows.Forms.Form form, COMM_EVENT_STRUCT tagevent)
        {

        }

        void MakeScriptPath(ref string path, string ext, uint id)
        {
            string file;
            file = String.Format("OBJ{0:00000}.{1}", id, ext);

            if (TotalConfig.defineMode == EnumDefineMode.MODE_EDIT)
            {
                if (GraphicTool.bLoadOnLibrary)
                {
                    path = GraphicTool.sLoadOnLibraryDir + "\\" + file;
                }
                else
                {

                    path = MakeFilePath.Graphic_ObjScr(file);
                }
            }
            else
            {

                path = MakeFilePath.Graphic_ObjScr(file);
            }
        }

        void ExpandLoadMouseZoneFile(string filename, EXPAND_MOUSE_ZONE zone)
        {

            CommaBlockString comma = new CommaBlockString();
            string buf = "";

            zone.bLock = 1;
            zone.x1 = 5;
            zone.y1 = 5;
            zone.x2 = 5;
            zone.y2 = 5;

            TextReader reader = new StreamReader(filename, System.Text.Encoding.Default);
            if (reader == null) return;

            while (true)
            {
                buf = reader.ReadLine();
                if (buf == null) break;

                comma.Set(buf);
                comma.GetString(ref buf);
                if (buf == "Zone")
                {
                    comma.GetInt(ref zone.x1);
                    comma.GetInt(ref zone.y1);
                    comma.GetInt(ref zone.x2);
                    comma.GetInt(ref zone.y2);
                }
                else if (buf == "Lock")
                {
                    comma.GetChar(ref zone.bLock);
                }
                else { }
            }

            reader.Close();
        }

        void ExpandLoadSliderFile(string filename, EXPAND_SLIDER_STRUCT slider)
        {
            CommaBlockString comma = new CommaBlockString();
            string buf = "";

            slider.nZoneMin = 0;
            slider.nZoneMax = 100;
            slider.nValueMin = 0;
            slider.nValueMax = 100;

            TextReader reader = new StreamReader(filename, System.Text.Encoding.Default);
            if (reader == null) return;

            while (true)
            {
                buf = reader.ReadLine();
                if (buf == null) break;

                comma.Set(buf);
                comma.GetString(ref buf);
                if (buf == "TagName")
                {
                    comma.GetString(ref slider.sTag);
                    slider.sTag = slider.sTag.Trim();
                }
                else if (buf == "ZoneMin")
                {
                    comma.GetInt(ref slider.nZoneMin);
                }
                else if (buf == "ZoneMax")
                {
                    comma.GetInt(ref slider.nZoneMax);
                }
                else if (buf == "ValueMin")
                {
                    comma.GetInt(ref slider.nValueMin);
                }
                else if (buf == "ValueMax")
                {
                    comma.GetInt(ref slider.nValueMax);
                }
            }

            reader.Close();

            EnumTagType tag_type = EnumTagType.none;

            if (TagLib.GetTagTypeAndPos(slider.sTag, ref tag_type, ref slider.nTagPos))
            {
            }
        }*/

        public virtual object ExecuteClassName(bool bHandOperation, string command, params object[] args)
        {
            return 0;
        }
        
        public object ExecuteClassNameOnlyObject(string command, params object[] args)
        {
            if (command == "ObjectSetBackColor")
            {
                Color color = NetFunction.FromArgb((int)args[0]);
                SetBackColor(new BrushSolid(color));
                BackColorChanged(color);
                return 1;
            }
            else if (command == "ObjectSetTextColor")
            {
                Color color = NetFunction.FromArgb((int)args[0]);
                SetTextColor(color);
                TextColorChanged(color);
                return 1;
            }
            else if (command == "ObjectSetLineColor")
            {
                Color color = NetFunction.FromArgb((int)args[0]);
                SetLineColor(color);
                LineColorChanged(color);
                return 1;
            }
            else if (command == "ObjectSetFillColor")
            {
                Color color = NetFunction.FromArgb((int)args[0]);
                SetFillColor(new BrushSolid(color));
                FillColorChanged(color);
                return 1;
            }
            else if (command == "ObjectSetLineThick")
            {
                int thick = (int)args[0];
                SetBorderThick(thick);
                return 1;
            }
            else if (command == "ObjectSetLineOption")
            {
                int option = (int)args[0];
                nLineOption = option;
                return 1;
            }
            else if (command == "ObjectSetFillOption")
            {
                int option = (int)args[0];
                nFillOption = option;
                return 1;
            }
            else if (command == "ObjectSetToolTipText")
            {
                string option = (string)args[0];

                objGeneral.sObjectDescription = option;

                return 1;
            }
            else if (command == "ObjectSetText")
            {
                string option = (string)args[0];

                OnObjectSetText(option);

                return 1;
            }
            else if (command == "ObjectSetRect")
            {
                nLeft = (int)args[1];
                nTop = (int)args[2];
                nRight = (int)args[3];
                nBottom = (int)args[4];

                ExpandCalcObjectRect();
                /*
                if (ExpandCalcObjectRect()) // 변화가 있으면
                {
                    InvalidateOldObject(objCommonProperty.form);  // 오브젝트의 이전 위치를 갱신

                    if (enumObjectType == EnumObjectType.Group) // 10.0 부터 추가
                    {
                        objCommonProperty.form.Invalidate();  // 부분적인 Invalidate가 잘 되지 않는다.
                    }
                }
                InvalidateObject(objCommonProperty.form);*/

                return 1;
            }

            return 0;
        }

        protected virtual void OnObjectSetText(string text) { }
        
        public virtual string ExecuteClassNameStringReturn(string command, params object[] args)
        {
            return "";
        }
        /*
        public void GetExpandIdStruct(EXPAND_ID_STRUCT eid)
        {
            eid.active = eID.active;
            eid.id = eID.id;
        }

        void SaveScriptToModX(CommaTextWriter writer, string name, ScriptClass script)
        {
            if (script == null) return;

            writer.WriteLine("\t{0},BEGIN", name);
            script.SaveFile(writer);
            writer.WriteLine("\t{0},END", name);
        }

        void SaveScriptToModX(CommaTextWriter writer, string name, EXPAND_MOUSE_ZONE script)
        {
            if (script == null) return;

            writer.WriteLine("\t{0},BEGIN", name);
            script.SaveFile(writer);
            writer.WriteLine("\t{0},END", name);
        }

        void SaveScriptToModX(CommaTextWriter writer, string name, EXPAND_SLIDER_STRUCT script)
        {
            if (script == null) return;

            writer.WriteLine("\t{0},BEGIN", name);
            script.SaveFile(writer);
            writer.WriteLine("\t{0},END", name);
        }

        void SaveExpandToModX(CommaTextWriter writer, string name, ExpandBasicAndScript expand)
        {
            if (expand == null) return;

            if (expand.basic != null)   // 설정되어 있을때만 사용한다.
            {
                writer.WriteLine("\t{0},BEGIN", "Expand" + name);
                writer.WriteLine("\t\tExpandType,{0},", (int)expand.eExpandType);
                if (expand.basic.GetType() == typeof(ExpandBasicConversion))
                {
                    ExpandBasicConversion con = (ExpandBasicConversion)expand.basic;
                    writer.WriteLine("\t\tExpandBasicConversion,{0},{1},{2},{3},{4},{5},", con.sTag, con.nBasicConversionType, con.fBasicSourceMin, con.fBasicSourceMax, con.fBasicTargetMin, con.fBasicTargetMax);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicVisible))
                {
                    ExpandBasicVisible con = (ExpandBasicVisible)expand.basic;
                    writer.WriteLine("\t\tExpandBasicVisible,{0},{1},{2},", con.sTag, con.condition, con.value);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicBlinking))
                {
                    ExpandBasicBlinking con = (ExpandBasicBlinking)expand.basic;
                    writer.WriteLine("\t\tExpandBasicBlinking,{0},{1},{2},{3}", con.sTag, con.condition, con.value, con.cycle);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicOnlyTag))
                {
                    ExpandBasicOnlyTag con = (ExpandBasicOnlyTag)expand.basic;
                    writer.WriteLine("\t\tExpandBasicOnlyTag,{0},", con.sTag);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicColor))
                {
                    ExpandBasicColor con = (ExpandBasicColor)expand.basic;

                    ExpandBasicColorMember member;
                    for (int i = 0; i < con.member.Count; i++)
                    {
                        member = (ExpandBasicColorMember)con.member[i];
                        writer.Write("\t\tExpandBasicColorMember,{0},", member.active ? 1 : 0);
                        writer.Write("{0},", "");   // imsi tag 애니매이션과 일치하기 위해서
                        writer.Write("{0},", member.sTag);
                        writer.Write("{0},", member.condition);
                        writer.Write("{0},", member.value);
                        writer.Write("{0},{1},{2},{3},", member.color.R, member.color.G, member.color.B, member.color.A);
                        writer.WriteLine();
                    }
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicRotate))
                {
                    ExpandBasicRotate con = (ExpandBasicRotate)expand.basic;
                    writer.WriteLine("\t\tExpandBasicRotate,{0},{1},{2},{3},{4},{5},{6},", con.sTag, con.nRotateAxis, con.fBasicSourceMin, con.fBasicSourceMax, con.fBasicTargetMin, con.fBasicTargetMax, con.nDirection);
                }
                else
                {
                    MessageBox.Show("unknown type at ObjectExpand.SaveExpandToModX", expand.basic.ToString());
                }
                writer.WriteLine("\t{0},END", "Expand" + name);
            }

            SaveScriptToModX(writer, "Script" + name, expand.script);
        }

        public void SaveExpandScript(CommaTextWriter writer)
        {
            writer.WriteLine("\tExpandScript,BEGIN");
            writer.WriteLine("\t\tActive,{0}", eID.active);
            //SaveScriptToModX(writer, "ScriptSizeWidth", eID.expand.scriptSizeWidth);
            SaveExpandToModX(writer, "SizeWidth", eID.expand.scriptSizeWidth);
            SaveExpandToModX(writer, "SizeHeight", eID.expand.scriptSizeHeight);
            SaveExpandToModX(writer, "LocationX", eID.expand.scriptLocationX);
            SaveExpandToModX(writer, "LocationY", eID.expand.scriptLocationY);
            SaveScriptToModX(writer, "ScriptEventKeyDown", eID.expand.scriptEventKeyDown);
            SaveScriptToModX(writer, "ScriptEventSelChange", eID.expand.scriptEventSelChange);
            SaveScriptToModX(writer, "ScriptMouseDown", eID.expand.scriptMouseDown);
            SaveScriptToModX(writer, "ScriptMouseUp", eID.expand.scriptMouseUp);
            SaveScriptToModX(writer, "ScriptMouseZone", eID.expand.structMouseZone);
            SaveExpandToModX(writer, "Visible", eID.expand.scriptVisible);
            SaveExpandToModX(writer, "Blinking", eID.expand.scriptBlinking);
            SaveExpandToModX(writer, "AnimationSpeed", eID.expand.scriptAnimationSpeed);
            SaveScriptToModX(writer, "ScriptSliderHorz", eID.expand.sliderHorz);
            SaveScriptToModX(writer, "ScriptSliderVert", eID.expand.sliderVert);
            SaveExpandToModX(writer, "ColorLine", eID.expand.scriptColorLine);
            SaveExpandToModX(writer, "ColorFill", eID.expand.scriptColorFill);
            SaveExpandToModX(writer, "ColorText", eID.expand.scriptColorText);
            SaveExpandToModX(writer, "ColorBack", eID.expand.scriptColorBack);
            SaveExpandToModX(writer, "ThickLine", eID.expand.scriptThickLine);
            SaveExpandToModX(writer, "Rotate", eID.expand.scriptRotate);
            writer.WriteLine("\tExpandScript,END");
        }

        public virtual void GetFamilyFile(List<object> block)
        {

        }

        public virtual void ChangeFamilyFile(List<object> block)
        {

        }
        */

        void GetAllTagListOfOneScript(List<object> block, ScriptClass script, string used_position)
        {
            if (script == null) return;
            script.GetMultiSelectTagList(block, objCommonProperty.sModuleName, EnumTagUsedType.GraphicObject, this, used_position);
        }

        void GetAllTagListOfOneScript(List<object> block, EXPAND_SLIDER_STRUCT script, string used_position)
        {
            if (script == null) return;
            TagUtil.AddTagList(block, script.sTag, EnumTagType.AI, objCommonProperty.sModuleName, EnumTagUsedType.GraphicObject, this, used_position);
        }

        void GetAllTagListOfOneExpand(List<object> block, ExpandBasicAndScript expand, string position)
        {
            if (expand == null) return;

            if (expand.basic != null)
            {
                if (expand.basic.GetType() == typeof(ExpandBasicConversion))
                {
                    ExpandBasicConversion con = (ExpandBasicConversion)expand.basic;
                    TagUtil.AddTagList(block, con.sTag, EnumTagType.AI, objCommonProperty.sModuleName, EnumTagUsedType.GraphicObject, this, position);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicVisible))
                {
                    ExpandBasicVisible con = (ExpandBasicVisible)expand.basic;
                    TagUtil.AddTagList(block, con.sTag, EnumTagType.DI, objCommonProperty.sModuleName, EnumTagUsedType.GraphicObject, this, position);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicBlinking))
                {
                    ExpandBasicBlinking con = (ExpandBasicBlinking)expand.basic;
                    TagUtil.AddTagList(block, con.sTag, EnumTagType.DI, objCommonProperty.sModuleName, EnumTagUsedType.GraphicObject, this, position);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicOnlyTag))
                {
                    ExpandBasicOnlyTag con = (ExpandBasicOnlyTag)expand.basic;
                    TagUtil.AddTagList(block, con.sTag, EnumTagType.AI, objCommonProperty.sModuleName, EnumTagUsedType.GraphicObject, this, position);
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicColor))
                {
                    ExpandBasicColor con = (ExpandBasicColor)expand.basic;

                    ExpandBasicColorMember member;
                    for (int i = 0; i < con.member.Count; i++)
                    {
                        member = (ExpandBasicColorMember)con.member[i];
                        TagUtil.AddTagList(block, member.sTag, EnumTagType.AI, objCommonProperty.sModuleName, EnumTagUsedType.GraphicObject, this, position);
                    }
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicRotate))
                {
                    ExpandBasicRotate con = (ExpandBasicRotate)expand.basic;
                    TagUtil.AddTagList(block, con.sTag, EnumTagType.AI, objCommonProperty.sModuleName, EnumTagUsedType.GraphicObject, this, position);
                }
                else
                {
                }

            }

            GetAllTagListOfOneScript(block, expand.script, position);
        }

        public virtual void GetMultiSelectTagList(List<object> block)
        {
            GetAllTagListOfOneExpand(block, eID.expand.scriptSizeWidth, "Expand:Width");
            GetAllTagListOfOneExpand(block, eID.expand.scriptSizeHeight, "Expand:Height");
            GetAllTagListOfOneExpand(block, eID.expand.scriptLocationX, "Expand:X");
            GetAllTagListOfOneExpand(block, eID.expand.scriptLocationY, "Expand:Y");
            GetAllTagListOfOneScript(block, eID.expand.scriptEventKeyDown, "Expand:EventKeyDown");
            GetAllTagListOfOneScript(block, eID.expand.scriptEventSelChange, "Expand:EventSelChange");
            GetAllTagListOfOneScript(block, eID.expand.scriptMouseDown, "Expand:MouseDown");
            GetAllTagListOfOneScript(block, eID.expand.scriptMouseUp, "Expand:MouseUp");
            GetAllTagListOfOneExpand(block, eID.expand.scriptVisible, "Expand:Visible");
            GetAllTagListOfOneExpand(block, eID.expand.scriptBlinking, "Expand:Blinking");
            GetAllTagListOfOneExpand(block, eID.expand.scriptAnimationSpeed, "Expand:AnimationSpeed");
            GetAllTagListOfOneScript(block, eID.expand.sliderHorz, "Expand:SliderHorz");
            GetAllTagListOfOneScript(block, eID.expand.sliderVert, "Expand:SliderVert");
            GetAllTagListOfOneExpand(block, eID.expand.scriptColorLine, "Expand:ColorLine");
            GetAllTagListOfOneExpand(block, eID.expand.scriptColorFill, "Expand:ColorFill");
            GetAllTagListOfOneExpand(block, eID.expand.scriptColorText, "Expand:ColorText");
            GetAllTagListOfOneExpand(block, eID.expand.scriptColorBack, "Expand:ColorBack");
            GetAllTagListOfOneExpand(block, eID.expand.scriptThickLine, "Expand:LineThick");
            GetAllTagListOfOneExpand(block, eID.expand.scriptRotate, "Expand:Rotate");
        }
        
        void SetAllTagListOfOneScript(List<object> block, ScriptClass script)
        {
            if (script == null) return;
            script.SetMultiSelectTagList(block);
        }

        void SetAllTagListOfOneScript(List<object> block, EXPAND_SLIDER_STRUCT script)
        {
            if (script == null) return;

            string tag = script.sTag;
            if (ObjectGroup.IsNeedUpdateTag(block, ref tag))
                script.sTag = tag;
        }

        void SetAllTagListOfOneExpand(List<object> block, ExpandBasicAndScript expand)
        {
            if (expand == null) return;

            string tag;
            if (expand.basic != null)
            {
                if (expand.basic.GetType() == typeof(ExpandBasicConversion))
                {
                    ExpandBasicConversion con = (ExpandBasicConversion)expand.basic;

                    tag = con.sTag;
                    if (ObjectGroup.IsNeedUpdateTag(block, ref tag))
                        con.sTag = tag;
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicVisible))
                {
                    ExpandBasicVisible con = (ExpandBasicVisible)expand.basic;
                    tag = con.sTag;
                    if (ObjectGroup.IsNeedUpdateTag(block, ref tag))
                        con.sTag = tag;
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicBlinking))
                {
                    ExpandBasicBlinking con = (ExpandBasicBlinking)expand.basic;
                    tag = con.sTag;
                    if (ObjectGroup.IsNeedUpdateTag(block, ref tag))
                        con.sTag = tag;
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicOnlyTag))
                {
                    ExpandBasicOnlyTag con = (ExpandBasicOnlyTag)expand.basic;
                    tag = con.sTag;
                    if (ObjectGroup.IsNeedUpdateTag(block, ref tag))
                        con.sTag = tag;
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicColor))
                {
                    ExpandBasicColor con = (ExpandBasicColor)expand.basic;

                    ExpandBasicColorMember member;
                    for (int i = 0; i < con.member.Count; i++)
                    {
                        member = (ExpandBasicColorMember)con.member[i];
                        tag = member.sTag;
                        if (ObjectGroup.IsNeedUpdateTag(block, ref tag))
                            member.sTag = tag;
                    }
                }
                else if (expand.basic.GetType() == typeof(ExpandBasicRotate))
                {
                    ExpandBasicRotate con = (ExpandBasicRotate)expand.basic;
                    tag = con.sTag;
                    if (ObjectGroup.IsNeedUpdateTag(block, ref tag))
                        con.sTag = tag;
                }
                else
                {
                }
            }

            SetAllTagListOfOneScript(block, expand.script);
        }

        public virtual void SetMultiSelectTagList(List<object> block)
        {
            SetAllTagListOfOneExpand(block, eID.expand.scriptSizeWidth);
            SetAllTagListOfOneExpand(block, eID.expand.scriptSizeHeight);
            SetAllTagListOfOneExpand(block, eID.expand.scriptLocationX);
            SetAllTagListOfOneExpand(block, eID.expand.scriptLocationY);
            SetAllTagListOfOneScript(block, eID.expand.scriptEventKeyDown);
            SetAllTagListOfOneScript(block, eID.expand.scriptEventSelChange);
            SetAllTagListOfOneScript(block, eID.expand.scriptMouseDown);
            SetAllTagListOfOneScript(block, eID.expand.scriptMouseUp);
            SetAllTagListOfOneExpand(block, eID.expand.scriptVisible);
            SetAllTagListOfOneExpand(block, eID.expand.scriptBlinking);
            SetAllTagListOfOneExpand(block, eID.expand.scriptAnimationSpeed);
            SetAllTagListOfOneScript(block, eID.expand.sliderHorz);
            SetAllTagListOfOneScript(block, eID.expand.sliderVert);
            SetAllTagListOfOneExpand(block, eID.expand.scriptColorLine);
            SetAllTagListOfOneExpand(block, eID.expand.scriptColorFill);
            SetAllTagListOfOneExpand(block, eID.expand.scriptColorText);
            SetAllTagListOfOneExpand(block, eID.expand.scriptColorBack);
            SetAllTagListOfOneExpand(block, eID.expand.scriptThickLine);
        }
        /*
        public virtual void EventTimerObjectOnPreview(System.Windows.Forms.Form form)
        {
            EventTimerObject(form);
        }*/

        protected bool bPreviewMode = false;
        /*
        public virtual void EventTimerOnPreview(System.Windows.Forms.Form form)
        {
            bPreviewMode = true;
            EventTimerObjectOnPreview(form);
        }

        public virtual void AddObjectInfo(TreeNode parent)
        {
            TreeNode node = new TreeNode(Path.GetExtension(this.ToString()).Substring(1));
            parent.Nodes.Add(node);
        }

        public delegate void DelegateCallBackObject(object obj);
         */

        public virtual void OnTagFileReaded()
        {

        }
    }
}
