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
using System.IO;
using NetTools.OldDefine;
using NetTools;
using System.Collections.Generic;
using AutoLibLocal;

namespace SilverlightGraphicModule
{
    public class LoadObjectFromModX
    {
        public List<object> graphMember = new List<object>();
        //public ArrayList graphMemberXY = new ArrayList();
        public List<object> dbTrendMember = null;
        //public ArrayList mdTrendMember = null;
        public List<object> blockPoint = new List<object>();		// Point, 를 합쳐놓음.
        public Color lTextColor;
        public BrushPublic lBackColor = new BrushPublic();
        public Color lLineColor;
        public BrushPublic lFillColor = new BrushPublic();
        public Color lGuideLineColor;
        public Color lOnColor;
        public Color lOffColor;
        public string sTagName = "";
        string sTagNameHeight;
        public RECT rRect = new RECT();

        public float fStartAngle;
        public float fEndAngle;
        public int nAngleDirection;
        public int nLocalMethod;

        int nRealUnit;
        public int nShowUnit;
        public ushort wTimeDevide;
        public ushort wLevelDevide;
        public ushort wTimeSelectOption;
        //public ushort wFlags;
        public int nBackBox;			// 밑에 배경으로 까는 박스의 형태.

        public string sFileName;
        public string sFileNameOn;
        public string sFileNameOff;

        public bool bMultiString = false;   // 이 클래스를 부르기 전에 true로 하면 여러줄을 붙여서 읽는다.
        public string sString;
        bool bStringReaded = false;  // 여러줄일 때 첫번째 줄을 읽었느냐?

        public ObjectGeneral objGeneral = new ObjectGeneral();
        public string sStringOption;	// 이속에 각각의 Object에 특화된 Option이 들어있다. 

        public int nOverlayMethod;
        public int nRotateFlip = 0;

        public MOUSE_RESPONSE_STRUCT mouseResponse = new MOUSE_RESPONSE_STRUCT();

        public EXPAND_ID_STRUCT eID;

        public ushort wLineOption;
        //public ushort wFillOption;
        public ushort wLineThick;
        //public ushort wGraphPointSize;
        public EnumWindowStyleFlags dwWindowStyle;

        public List<object> blockAnalogStatus = new List<object>();
        public LOGFONT fontStruct;
        public TEXT_ALIGN align = new TEXT_ALIGN();
        public List<object> blockButtonDoutMember = new List<object>();
        public List<object> blockListData = new List<object>();
        public List<object> blockCurve = new List<object>();

        //public ArrayList blockTwoTagMember = new ArrayList();
        //public ArrayList blockTwoTagMemberList = new ArrayList();   // TestGraph에서는 이것이 0이면 위의 blockTwoTagMember를 사용한다.


        public ScriptClass scriptLocal;

        //public int nLevelDisplaySize = 7;

        public ObjectArgsGraphPublic argsGraphPublic = new ObjectArgsGraphPublic();

        public LoadObjectFromModX()
        {
            //
            // TODO: Add constructor logic here
            //
            fStartAngle = 0.0f;
            fEndAngle = 360.0f;
            nAngleDirection = 0;	// 시계방향.
            nLocalMethod = 0;

            lTextColor = Colors.Black;
            lBackColor.basic_color = Colors.White;
            lLineColor = Colors.Black;
            lFillColor.basic_color = Colors.Red;
            lOnColor = Colors.Blue;
            lOffColor = Colors.Black;
            lGuideLineColor = Colors.DarkGray;
            rRect.left = 100;
            rRect.top = 100;
            rRect.right = 200;
            rRect.bottom = 200;
            nOverlayMethod = 0;
            sFileNameOn = "";
            sFileNameOff = "";
            sFileName = "";
            sString = "";
            nBackBox = 0;
            eID = new EXPAND_ID_STRUCT();
            eID.mod_type = EnumModType.modx;
            wLevelDevide = 2;
            wTimeSelectOption = 0;
            //wGraphPointSize = 10;
            align.x = 2;	// right
            align.y = 1;	// vcenter
            //wFlags = 0;
            dwWindowStyle = 0;

            fontStruct = new LOGFONT();
            sStringOption = "";
        }

        // 2007.6.26
        // 자신만의 클래스에서 사용하는 것들은 따로 클래스를 만들어서 사용하는 것이 속도와 구조적으로 나을 듯하다.
        protected virtual bool ChildCheck(string command, CommaTextReader comma)
        {
            return false;
        }

        public bool run(TextReader reader, string command)
        {
            string buf = "";
            string one_line;
            byte r = 0, g = 0, b = 0, a = 0;
            CommaTextReader comma = new CommaTextReader();

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) return false;		// file end. file destroyed
                if (one_line.Length == 0) continue;

                comma.Set(one_line);
                comma.GetString(ref buf);

                if (String.Compare(buf, command) == 0)
                {
                    comma.GetString(ref buf);
                    if (String.Compare(buf, "END") != 0) return false;	// file destroyed

                    
                    // ObjectTag에서 사용하는 것을 전체 클래스에서 사용하도록 이동함 2007.6.13
                    if (mouseResponse.bImsiResponseOnVisible == 1)
                    {
                        objGeneral.bResponseOnVisible = true;
                    }
                    return true;
                }
                else if (ChildCheck(buf, comma))
                {
                    // 해당 명령어가 있다.
                }
                else if (String.Compare(buf, "TextColor") == 0)
                {
                    comma.GetBYTE(ref r);
                    comma.GetBYTE(ref g);
                    comma.GetBYTE(ref b);
                    comma.GetBYTE(ref a);
                    lTextColor = Color.FromArgb(a, r, g, b);
                }
                else if (String.Compare(buf, "BackColor") == 0)
                {
                    /*
                    comma.GetBYTE(ref r);
                    comma.GetBYTE(ref g);
                    comma.GetBYTE(ref b);
                    comma.GetBYTE(ref a);
                    lBackColor = Color.FromArgb(a, r, g, b);*/

                    lBackColor = LoadBrush(comma);
                }
                else if (String.Compare(buf, "LineColor") == 0)
                {
                    comma.GetBYTE(ref r);
                    comma.GetBYTE(ref g);
                    comma.GetBYTE(ref b);
                    comma.GetBYTE(ref a);
                    lLineColor = Color.FromArgb(a, r, g, b);
                }
                else if (String.Compare(buf, "FillColor") == 0)
                {
                    /*
                    comma.GetBYTE(ref r);
                    comma.GetBYTE(ref g);
                    comma.GetBYTE(ref b);
                    comma.GetBYTE(ref a);
                    lFillColor = Color.FromArgb(a, r, g, b);*/

                    lFillColor = LoadBrush(comma);
                }
                else if (String.Compare(buf, "GuideLineColor") == 0)
                {
                    comma.GetBYTE(ref r);
                    comma.GetBYTE(ref g);
                    comma.GetBYTE(ref b);
                    comma.GetBYTE(ref a);
                    lGuideLineColor = Color.FromArgb(a, r, g, b);
                }
                else if (String.Compare(buf, "OnColor") == 0)
                {
                    comma.GetBYTE(ref r);
                    comma.GetBYTE(ref g);
                    comma.GetBYTE(ref b);
                    comma.GetBYTE(ref a);
                    lOnColor = Color.FromArgb(a, r, g, b);
                }
                else if (String.Compare(buf, "OffColor") == 0)
                {
                    comma.GetBYTE(ref r);
                    comma.GetBYTE(ref g);
                    comma.GetBYTE(ref b);
                    comma.GetBYTE(ref a);
                    lOffColor = Color.FromArgb(a, r, g, b);
                }

                    // 아래는 태그 관련 object에서 사용하는 명령.
                else if (String.Compare(buf, "TagName") == 0)
                {
                    comma.GetString(ref sTagName);
                    sTagName = sTagName.Trim();
                }
                else if (String.Compare(buf, "TagNameHeight") == 0)
                {
                    comma.GetString(ref sTagNameHeight);
                }
                else if (String.Compare(buf, "TagMouseResponse") == 0)
                {
                    comma.GetInt(ref mouseResponse.mouse_response);
                    comma.GetString(ref mouseResponse.scriptFilename);
                    comma.GetChar(ref mouseResponse.bImsiResponseOnVisible);

                    comma.GetChar(ref mouseResponse.cUserControlBoxUse);
                    comma.GetString(ref mouseResponse.sUserControlBoxModuleFile);
                    comma.GetString(ref mouseResponse.sUserControlBoxTitle);
                    comma.GetString(ref mouseResponse.sUserControlBoxDescription);
                }
                else if (String.Compare(buf, "TagDigitalOutputMethod") == 0)
                {
                    comma.GetInt(ref mouseResponse.do_method);
                }
                else if (String.Compare(buf, "TagDigitalOutputDelayTime") == 0)
                {
                    comma.GetInt(ref mouseResponse.do_delaytime);
                }
                else if (String.Compare(buf, "Rect") == 0)
                {
                    comma.GetInt(ref rRect.left);
                    comma.GetInt(ref rRect.top);
                    comma.GetInt(ref rRect.right);
                    comma.GetInt(ref rRect.bottom);
                }

                else if (String.Compare(buf, "StartAngle") == 0)
                {
                    comma.GetFloat(ref fStartAngle);
                }
                else if (String.Compare(buf, "EndAngle") == 0)
                {
                    comma.GetFloat(ref fEndAngle);
                }
                else if (String.Compare(buf, "AngleDirection") == 0)
                {
                    comma.GetInt(ref nAngleDirection);
                }
                else if (String.Compare(buf, "LocalMethod") == 0)
                {
                    comma.GetInt(ref nLocalMethod);
                }
                else if (String.Compare(buf, "FileName") == 0)
                {
                    comma.GetString(ref sFileName);
                }
                    
                else if (String.Compare(buf, "GraphMember") == 0)
                {
                    PUBLIC_GRAPH_MEMBER member = new PUBLIC_GRAPH_MEMBER();
                    byte cr = 0, cg = 0, cb = 0, ca = 0;

                    comma.GetString(ref member.tag);
                    member.tag = member.tag.Trim();
                    comma.GetBYTE(ref cr);
                    comma.GetBYTE(ref cg);
                    comma.GetBYTE(ref cb);
                    comma.GetBYTE(ref ca);
                    member.color = Color.FromArgb(ca, cr, cg, cb);
                    comma.GetInt(ref member.nValueType);
                    comma.GetInt(ref member.nPointType);
                    comma.GetInt(ref member.nLineThick);
                    if (member.nLineThick < 1) member.nLineThick = 1;

                    comma.GetInt(ref member.nAxisPosition);
                    comma.GetInt(ref member.nLevelFrom);
                    comma.GetInt(ref member.nLevelTo);
                    comma.GetInt(ref member.nTagDisplaySize);
                    comma.GetChar(ref member.bReverseY);
                    comma.GetHexWORD(ref member.wFlags);
                    comma.GetInt(ref member.nTimeShift);

                    graphMember.Add(member);
                }/*
                else if (String.Compare(buf, "GraphMemberXY") == 0)
                {
                    XY_GRAPH_MEMBER member = new XY_GRAPH_MEMBER();
                    comma.GetString(ref member.tagX);
                    comma.GetString(ref member.tagY);
                    comma.GetInt(ref r);
                    comma.GetInt(ref g);
                    comma.GetInt(ref b);
                    member.color = Color.FromArgb(r, g, b);
                    comma.Skip();//comma.GetChar(ref member.n.cValueType);
                    comma.GetChar(ref member.cPointType);
                    comma.GetChar(ref member.cLineThick);
                    if (member.cLineThick < 1) member.cLineThick = 1;

                    comma.GetChar(ref member.cAxisPositionX);
                    comma.GetInt(ref member.nLevelFromX);
                    comma.GetInt(ref member.nLevelToX);
                    comma.GetChar(ref member.cAxisPositionY);
                    comma.GetInt(ref member.nLevelFromY);
                    comma.GetInt(ref member.nLevelToY);
                    comma.GetInt(ref member.nTagDisplaySize);
                    comma.GetChar(ref member.bReverseX);
                    comma.GetChar(ref member.bReverseY);

                    graphMemberXY.Add(member);
                }*/
                else if (String.Compare(buf, "DbTrendMember") == 0)
                {
                    DB_TREND_MEMBER member = new DB_TREND_MEMBER();
                    byte cr = 0, cg = 0, cb = 0, ca = 0;

                    comma.GetString(ref member.tag);
                    member.tag = member.tag.Trim();
                    comma.GetString(ref member.column);
                    member.column = member.column.Trim();

                    comma.GetBYTE(ref cr);
                    comma.GetBYTE(ref cg);
                    comma.GetBYTE(ref cb);
                    comma.GetBYTE(ref ca);
                    member.color = Color.FromArgb(ca, cr, cg, cb);
                    comma.GetInt(ref member.nValueType);
                    comma.GetInt(ref member.nPointType);
                    comma.GetInt(ref member.nLineThick);
                    if (member.nLineThick < 1) member.nLineThick = 1;

                    comma.GetInt(ref member.nAxisPosition);
                    comma.GetInt(ref member.nLevelFrom);
                    comma.GetInt(ref member.nLevelTo);
                    comma.GetInt(ref member.nTagDisplaySize);
                    comma.GetChar(ref member.bReverseY);
                    comma.GetHexWORD(ref member.wFlags);

                    comma.GetInt(ref member.nGraphType);
                    comma.GetString(ref member.sTable);
                    comma.GetString(ref member.sWhereString);
                    comma.GetString(ref member.sDescription);

                    if (dbTrendMember == null) dbTrendMember = new List<object>();

                    dbTrendMember.Add(member);
                } /*
                else if (String.Compare(buf, "MdTrendMember") == 0) // Milli Data Trend Member
                {
                    MD_TREND_MEMBER member = new MD_TREND_MEMBER();
                    int cr = 0, cg = 0, cb = 0, ca = 0;

                    comma.GetString(ref member.tag);
                    member.tag = member.tag.Trim();
                    comma.GetString(ref member.column);
                    member.column = member.column.Trim();

                    comma.GetInt(ref cr);
                    comma.GetInt(ref cg);
                    comma.GetInt(ref cb);
                    comma.GetInt(ref ca);
                    member.color = Color.FromArgb(ca, cr, cg, cb);
                    comma.GetInt(ref member.nValueType);
                    comma.GetInt(ref member.nPointType);
                    comma.GetInt(ref member.nLineThick);
                    if (member.nLineThick < 1) member.nLineThick = 1;

                    comma.GetInt(ref member.nAxisPosition);
                    comma.GetInt(ref member.nLevelFrom);
                    comma.GetInt(ref member.nLevelTo);
                    comma.GetInt(ref member.nTagDisplaySize);
                    comma.GetInt(ref member.nReverseY);
                    comma.GetHexWORD(ref member.wFlags);
                    comma.GetInt(ref member.nGraphType);
                    comma.Skip();//.GetString(ref member.sTable);
                    comma.Skip();//.GetString(ref member.sWhereString);
                    comma.GetString(ref member.sDescription);

                    if (mdTrendMember == null) mdTrendMember = new ArrayList();

                    mdTrendMember.Add(member);
                }
                else if (String.Compare(buf, "DemandMember") == 0)
                {

                }*/
                else if (String.Compare(buf, "MemberAnalogStatus") == 0)
                {
                    ANALOG_STATUS_STRUCT status = new ANALOG_STATUS_STRUCT();
                    comma.GetInt(ref status.type);
                    comma.GetString(ref status.filename);
                    blockAnalogStatus.Add(status);
                }
                else if (String.Compare(buf, "RealUnit") == 0)
                {
                    comma.GetInt(ref nRealUnit);
                }
                else if (String.Compare(buf, "ShowUnit") == 0)
                {
                    comma.GetInt(ref nShowUnit);
                }
                else if (String.Compare(buf, "wFlags") == 0)
                {
                    ushort flags = 0;
                    comma.GetWORD(ref flags);
                    argsGraphPublic.wDisplayFlags = (EnumDisplayFlag)flags;
                }
                else if (String.Compare(buf, "wTimeDevide") == 0)
                {
                    comma.GetWORD(ref wTimeDevide);
                }
                else if (String.Compare(buf, "wLevelDevide") == 0)
                {
                    comma.GetWORD(ref wLevelDevide);
                }
                else if (String.Compare(buf, "wTimeSelectOption") == 0)
                {
                    comma.GetWORD(ref wTimeSelectOption);
                }
                else if (String.Compare(buf, "FileNameOn") == 0)
                {
                    comma.GetString(ref sFileNameOn);
                }
                else if (String.Compare(buf, "FileNameOff") == 0)
                {
                    comma.GetString(ref sFileNameOff);
                }
                else if (String.Compare(buf, "OverlayMethod") == 0)
                {
                    comma.GetInt(ref nOverlayMethod);
                    comma.GetInt(ref nRotateFlip);
                }
                else if (String.Compare(buf, "LogFont") == 0)
                {
                    int imsi = 0;
                    comma.GetString(ref fontStruct.lfFaceName);
                    comma.GetFloat(ref fontStruct.lfHeight);

                    comma.GetInt(ref imsi);
                    fontStruct.Bold = (imsi == 1);
                    comma.GetInt(ref imsi);
                    fontStruct.Italic = (imsi == 1);
                    comma.GetInt(ref imsi);
                    fontStruct.Strikeout = (imsi == 1);
                    comma.GetInt(ref imsi);
                    fontStruct.Underline = (imsi == 1);

                    /*
                    fontStruct.style = 0;
                    comma.GetInt(ref imsi);
                    if (imsi == 1) fontStruct.style |= FontStyle.Bold;
                    comma.GetInt(ref imsi);
                    if (imsi == 1) fontStruct.style |= FontStyle.Italic;
                    comma.GetInt(ref imsi);
                    if (imsi == 1) fontStruct.style |= FontStyle.Strikeout;
                    comma.GetInt(ref imsi);
                    if (imsi == 1) fontStruct.style |= FontStyle.Underline;*/
                }
                else if (String.Compare(buf, "BackBox") == 0)
                {
                    comma.GetInt(ref nBackBox);
                }
                    
                else if (String.Compare(buf, "ExpandScript") == 0)
                {
                    eID = LoadExpandScript(reader, buf);
                } 
                else if (String.Compare(buf, "LocalScript") == 0)
                {
                    scriptLocal = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "TagScript") == 0)
                {
                    mouseResponse.scriptInclude = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "LineOption") == 0)
                {
                    comma.GetWORD(ref wLineOption);
                }
                else if (String.Compare(buf, "FillOption") == 0)
                {
                    if (!bBrushVersion)  // 10.0.2 이전 버전인 경우만 채움을 확인한다.
                        comma.GetInt(ref lFillColor.brush_type);
                }
                else if (String.Compare(buf, "LineThick") == 0)
                {
                    comma.GetWORD(ref wLineThick);
                }
                else if (String.Compare(buf, "String") == 0)
                {
                    if (bMultiString)
                    {
                        if (this.bStringReaded)
                        {
                            string add = comma.GetStringWithSpace();
                            sString += "\r\n" + add;
                        }
                        else
                        {
                            sString = comma.GetStringWithSpace();
                            this.bStringReaded = true;
                        }
                    }
                    else
                    {
                        sString = comma.GetStringWithSpace();
                    }
                }
                else if (String.Compare(buf, "Point") == 0)
                {
                    int x = 0;
                    int y = 0;

                    comma.GetInt(ref x);
                    comma.GetInt(ref y);

                    Point point = new Point(x, y);

                    blockPoint.Add(point);
                }
                else if (String.Compare(buf, "Curve") == 0)
                {
                    CURVE_STRUCT curve = new CURVE_STRUCT();

                    ushort type = 0;

                    comma.GetHexWORD(ref type);
                    curve.type = (EnumCurveType)type;
                    comma.GetInt(ref curve.x[0]);
                    comma.GetInt(ref curve.y[0]);
                    comma.GetInt(ref curve.x[1]);
                    comma.GetInt(ref curve.y[1]);
                    comma.GetInt(ref curve.x[2]);
                    comma.GetInt(ref curve.y[2]);

                    blockCurve.Add(curve);
                }
                else if (String.Compare(buf, "wGraphPointSize") == 0)
                {
                    comma.GetInt(ref argsGraphPublic.wPointSize);
                }
                else if (String.Compare(buf, "TextAlign") == 0)
                {
                    comma.GetBYTE(ref align.x);
                    comma.GetBYTE(ref align.y);
                }
                else if (String.Compare(buf, "StringOption") == 0)
                {
                    comma.GetStringTotalRemain(ref sStringOption);
                }
                else if (String.Compare(buf, "ClassName") == 0)
                {
                    LoadObjectItem.LoadClassName(comma, objGeneral);
                }
                else if (String.Compare(buf, "Rotation") == 0)
                {
                    LoadObjectItem.LoadRotation(comma, objGeneral);
                }
                else if (String.Compare(buf, "ButtonDoutMember") == 0)
                {
                    BUTTON_DOUT_STRUCT member = new BUTTON_DOUT_STRUCT();

                    comma.GetString(ref member.tag);
                    member.tag = member.tag.Trim();
                    blockButtonDoutMember.Add(member);
                }/*
                // 9.2.0 이전에 RealTimeTestGraph에서 한개만 기준 그래프 한개만 사용할 때
                else if (String.Compare(buf, "TwoTagMember") == 0)
                {
                    TWO_TAG_MEMBER member = new TWO_TAG_MEMBER();

                    comma.GetString(ref member.sTag1);
                    comma.GetString(ref member.sTag2);
                    blockTwoTagMember.Add(member);
                }
                // 9.2.0 이후에 사용하는 형식 
                else if (String.Compare(buf, "TwoTagMemberList") == 0)
                {
                    TWO_TAG_MEMBER_LIST list = new TWO_TAG_MEMBER_LIST();

                    comma.GetColorFromARGB(ref list.color);
                    comma.GetInt(ref list.nLineThick);
                    comma.GetInt(ref list.nPointType);
                    comma.Skip();
                    comma.Skip();
                    comma.Skip();
                    comma.Skip();

                    while (true)
                    {
                        if (comma.IsEOS()) break;

                        TWO_TAG_MEMBER member = new TWO_TAG_MEMBER();

                        comma.GetString(ref member.sTag1);
                        if (member.sTag1 == "") break;  // end of x,y member 뒤에 추가할 수 있으므로 일단 끝을 만들어 놓았다.
                        comma.GetString(ref member.sTag2);
                        list.arrayMember.Add(member);
                    }
                    blockTwoTagMemberList.Add(list);
                }*/
                else if (String.Compare(buf, "WindowStyle") == 0)
                {
                    uint flags = 0;
                    comma.GetHexDWORD(ref flags);
                    dwWindowStyle = (EnumWindowStyleFlags)flags;
                }
                else if (String.Compare(buf, "ListData") == 0)
                {
                    string data = "";
                    comma.GetString(ref data);
                    blockListData.Add(data);
                }
                else if (String.Compare(buf, "LevelDisplaySize") == 0)
                {
                    comma.GetInt(ref argsGraphPublic.nLevelDisplaySize);
                }
                else if (String.Compare(buf, "GraphPublicArgs") == 0)
                {
                    ushort flags = 0;
                    comma.GetWORD(ref flags);
                    argsGraphPublic.wDisplayFlags = (EnumDisplayFlag)flags;
                    comma.GetInt(ref argsGraphPublic.wPointSize);
                    comma.GetInt(ref argsGraphPublic.nLevelDisplaySize);

                    uint argb = 0;
                    comma.GetHexDWORD(ref argb);
                    argsGraphPublic.colorHiHi = NetFunction.FromArgb((int)argb);
                    comma.GetHexDWORD(ref argb);
                    argsGraphPublic.colorHigh = NetFunction.FromArgb((int)argb);
                    comma.GetHexDWORD(ref argb);
                    argsGraphPublic.colorLow = NetFunction.FromArgb((int)argb);
                    comma.GetHexDWORD(ref argb);
                    argsGraphPublic.colorLoLo = NetFunction.FromArgb((int)argb);

                    //writer.WriteLine("\tGraphPublicArgs,{0},{1},{2},{3:X08},{4:X08},{5:X08},{6:X08}", (int)pub.wDisplayFlags, pub.wPointSize, pub.nLevelDisplaySize, pub.colorHiHi, pub.colorHigh, pub.colorLow, pub.colorLoLo);

                }
                else
                {

                }
            }

            //return true;
        }

        bool bBrushVersion = false;

        BrushPublic LoadBrush(CommaTextReader comma)
        {
            byte r = 0, g = 0, b = 0, a = 0;
            int type = 1;

            comma.GetBYTE(ref r);
            comma.GetBYTE(ref g);
            comma.GetBYTE(ref b);
            comma.GetBYTE(ref a);

            if (comma.IsEOS())
            {
                BrushSolid brush = new BrushSolid();
                brush.basic_color = Color.FromArgb(a, r, g, b);
                brush.brush_type = type;
                return brush;
            }

            bBrushVersion = true;
            comma.GetInt(ref type);

            if (type == 2)
            {
                BrushLinearGradient brush = new BrushLinearGradient();
                brush.basic_color = Color.FromArgb(a, r, g, b);
                brush.brush_type = type;

                comma.GetInt(ref brush.spread_method);
                int count = 0;
                comma.GetInt(ref count);
                string members = "";
                comma.GetString(ref members);

                float x = 0, y = 0;

                comma.GetFloat(ref x);
                comma.GetFloat(ref y);
                brush.pStart = new Point(x, y);
                comma.GetFloat(ref x);
                comma.GetFloat(ref y);
                brush.pEnd = new Point(x, y);

                brush.stops = new System.Collections.Generic.List<BrushGradientStop>();
                CommaBlockString commastop = new CommaBlockString();
                commastop.SetBlockCode(':');
                commastop.Set(members);

                for (int i = 0; i < count; i++)
                {
                    BrushGradientStop stop = new BrushGradientStop();
                    commastop.GetFloat(ref stop.offset);
                    uint argb = 0;
                    commastop.GetHexDWORD(ref argb);
                    stop.color = NetFunction.FromArgb((int)argb);

                    brush.stops.Add(stop);
                }

                return brush;
            }
            else
            {
                BrushSolid brush = new BrushSolid();
                brush.basic_color = Color.FromArgb(a, r, g, b);
                brush.brush_type = type;
                return brush;
            }
        }
        
        public static EXPAND_ID_STRUCT LoadExpandScript(TextReader reader, string command)
        {
            EXPAND_ID_STRUCT eID;

            eID = new EXPAND_ID_STRUCT();
            eID.mod_type = EnumModType.modx;

            string one_line;
            string buf = "";
            CommaBlockString comma = new CommaBlockString();

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;

                comma.Set(one_line);
                comma.GetString(ref buf);
                if (String.Compare(command, buf, StringComparison.CurrentCultureIgnoreCase) == 0) // END
                {
                    break;
                }
                else if (String.Compare(buf, "Active", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    comma.GetChar(ref eID.active);
                }
                    
                else if (String.Compare(buf, "ScriptSizeWidth", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    if (eID.expand.scriptSizeWidth == null)
                    {
                        eID.expand.scriptSizeWidth = new ExpandBasicAndScript();
                    }

                    eID.expand.scriptSizeWidth.script = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptSizeHeight", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    if (eID.expand.scriptSizeHeight == null)
                    {
                        eID.expand.scriptSizeHeight = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptSizeHeight.script = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptLocationX", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    if (eID.expand.scriptLocationX == null)
                    {
                        eID.expand.scriptLocationX = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptLocationX.script = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptLocationY", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    if (eID.expand.scriptLocationY == null)
                    {
                        eID.expand.scriptLocationY = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptLocationY.script = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ExpandSizeWidth", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptSizeWidth);
                }
                else if (String.Compare(buf, "ExpandSizeHeight", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptSizeHeight);
                }
                else if (String.Compare(buf, "ExpandLocationX", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptLocationX);
                }
                else if (String.Compare(buf, "ExpandLocationY", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptLocationY);
                }
                    
                else if (String.Compare(buf, "ScriptEventKeyDown", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    eID.expand.scriptEventKeyDown = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptEventSelChange", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    eID.expand.scriptEventSelChange = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptMouseDown", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    eID.expand.scriptMouseDown = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptMouseUp", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    eID.expand.scriptMouseUp = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptMouseZone", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    eID.expand.structMouseZone = LoadOneMouseZone(reader, buf);
                }
                else if (String.Compare(buf, "ScriptVisible", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    if (eID.expand.scriptVisible == null)
                    {
                        eID.expand.scriptVisible = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptVisible.script = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptBlinking", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    if (eID.expand.scriptBlinking == null)
                    {
                        eID.expand.scriptBlinking = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptBlinking.script = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ExpandVisible", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptVisible);
                }
                else if (String.Compare(buf, "ExpandBlinking", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptBlinking);
                }

                else if (String.Compare(buf, "ScriptSliderHorz", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    eID.expand.sliderHorz = LoadOneSlider(reader, buf);
                }
                else if (String.Compare(buf, "ScriptSliderVert", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    eID.expand.sliderVert = LoadOneSlider(reader, buf);
                }
                    
                else if (String.Compare(buf, "ScriptColorLine", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    if (eID.expand.scriptColorLine == null)
                    {
                        eID.expand.scriptColorLine = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptColorLine.script = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptColorFill", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    if (eID.expand.scriptColorFill == null)
                    {
                        eID.expand.scriptColorFill = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptColorFill.script = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptColorText", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    if (eID.expand.scriptColorText == null)
                    {
                        eID.expand.scriptColorText = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptColorText.script = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptColorBack", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    if (eID.expand.scriptColorBack == null)
                    {
                        eID.expand.scriptColorBack = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptColorBack.script = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptThickLine", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    if (eID.expand.scriptThickLine == null)
                    {
                        eID.expand.scriptThickLine = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptThickLine.script = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ScriptAnimationSpeed", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    if (eID.expand.scriptAnimationSpeed == null)
                    {
                        eID.expand.scriptAnimationSpeed = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptAnimationSpeed.script = LoadOneScript(reader, buf);
                }

                else if (String.Compare(buf, "ExpandColorLine", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptColorLine);
                }
                else if (String.Compare(buf, "ExpandColorFill", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptColorFill);
                }
                else if (String.Compare(buf, "ExpandColorText", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptColorText);
                }
                else if (String.Compare(buf, "ExpandColorBack", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptColorBack);
                }
                else if (String.Compare(buf, "ExpandThickLine", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptThickLine);
                }
                else if (String.Compare(buf, "ExpandAnimationSpeed", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptAnimationSpeed);
                }

                    
                else if (String.Compare(buf, "ScriptRotate", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    if (eID.expand.scriptRotate == null)
                    {
                        eID.expand.scriptRotate = new ExpandBasicAndScript();
                    }
                    eID.expand.scriptRotate.script = LoadOneScript(reader, buf);
                }
                else if (String.Compare(buf, "ExpandRotate", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    LoadOneConversion(reader, buf, ref eID.expand.scriptRotate);
                }

                else
                {

                }
            }

            return eID;
        }

        static void LoadOneConversion(TextReader reader, string command, ref ExpandBasicAndScript expand)
        {
            if (expand == null)
            {
                expand = new ExpandBasicAndScript();
            }

            string one_line;
            string buf = "";
            CommaBlockString comma = new CommaBlockString();

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;

                comma.Set(one_line);
                comma.GetString(ref buf);
                if (String.Compare(command, buf, StringComparison.CurrentCultureIgnoreCase) == 0) // END
                {
                    break;
                }
                else if (String.Compare(buf, "ExpandType", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    int type = 0;
                    comma.GetInt(ref type);

                    if (type == 0) expand.eExpandType = EnumExpandType.Basic;
                    else
                    {
                        expand.eExpandType = EnumExpandType.Script;
                    }
                }
                else if (String.Compare(buf, "ExpandBasicConversion", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    ExpandBasicConversion con = new ExpandBasicConversion();

                    comma.GetString(ref con.sTag);
                    comma.GetInt(ref con.nBasicConversionType);
                    comma.GetFloat(ref con.fBasicSourceMin);
                    comma.GetFloat(ref con.fBasicSourceMax);
                    comma.GetFloat(ref con.fBasicTargetMin);
                    comma.GetFloat(ref con.fBasicTargetMax);

                    expand.basic = con;
                }
                else if (String.Compare(buf, "ExpandBasicVisible", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    ExpandBasicVisible con = new ExpandBasicVisible();

                    comma.GetString(ref con.sTag);
                    comma.GetInt(ref con.condition);
                    comma.GetString(ref con.value);

                    expand.basic = con;
                }
                else if (String.Compare(buf, "ExpandBasicBlinking", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    ExpandBasicBlinking con = new ExpandBasicBlinking();

                    comma.GetString(ref con.sTag);
                    comma.GetInt(ref con.condition);
                    comma.GetString(ref con.value);
                    comma.GetInt(ref con.cycle);

                    expand.basic = con;
                }
                else if (String.Compare(buf, "ExpandBasicOnlyTag", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    ExpandBasicOnlyTag con = new ExpandBasicOnlyTag();

                    comma.GetString(ref con.sTag);

                    expand.basic = con;
                }
                else if (String.Compare(buf, "ExpandBasicColorMember", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    if (expand.basic == null)
                    {
                        expand.basic = new ExpandBasicColor();
                    }

                    ExpandBasicColor con = (ExpandBasicColor)expand.basic;

                    ExpandBasicColorMember member = new ExpandBasicColorMember();
                    byte flag = 0;

                    comma.GetBYTE(ref flag);
                    member.active = (flag == 1);
                    comma.Skip();   // bDefault
                    comma.GetString(ref member.sTag);
                    comma.GetInt(ref member.condition);
                    comma.GetString(ref member.value);

                    byte cr = 0, cg = 0, cb = 0, ca = 0;

                    comma.GetBYTE(ref cr);
                    comma.GetBYTE(ref cg);
                    comma.GetBYTE(ref cb);
                    comma.GetBYTE(ref ca);
                    member.color = Color.FromArgb(ca, cr, cg, cb);

                    con.member.Add(member);
                }
                else if (String.Compare(buf, "ExpandBasicRotate", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    ExpandBasicRotate con = new ExpandBasicRotate();

                    comma.GetString(ref con.sTag);
                    comma.GetInt(ref con.nRotateAxis);
                    comma.GetFloat(ref con.fBasicSourceMin);
                    comma.GetFloat(ref con.fBasicSourceMax);
                    comma.GetFloat(ref con.fBasicTargetMin);
                    comma.GetFloat(ref con.fBasicTargetMax);
                    comma.GetInt(ref con.nDirection);

                    expand.basic = con;
                }
                else
                {

                }
            }
        }

        public static ScriptClass LoadOneScript(TextReader reader, string command)
        {
            ScriptClass script = new ScriptClass();

            script.LoadFromMODX(reader, command);

            if (script.Script.Length == 0)
                return null;

            return script;
        }

        static EXPAND_MOUSE_ZONE LoadOneMouseZone(TextReader reader, string command)
        {
            EXPAND_MOUSE_ZONE script = new EXPAND_MOUSE_ZONE();

            script.LoadFromMODX(reader, command);

            return script;
        }

        static EXPAND_SLIDER_STRUCT LoadOneSlider(TextReader reader, string command)
        {
            EXPAND_SLIDER_STRUCT script = new EXPAND_SLIDER_STRUCT();

            script.LoadFromMODX(reader, command);

            return script;
        }
        
    }
}
