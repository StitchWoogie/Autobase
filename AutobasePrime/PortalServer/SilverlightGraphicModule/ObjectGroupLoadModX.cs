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
using NetTools;

namespace SilverlightGraphicModule
{
    public class ObjectGroupLoadModX
    {
        ObjectPublicGroupLayer parent;

        public ObjectGroupLoadModX()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public int Load(ObjectCommonProperty ocp, Canvas parent_canvas, ObjectPublicGroupLayer p, UserControl form, TextReader reader, string filename, int depth, EnumModType load_type)
        {
            parent = p;
            string one_line = "";
            string imsi = "";
            CommaTextReader commaBuf = new CommaTextReader();

            parent.FreeAllObjectBuf();

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;
                if (one_line.Length == 0) continue;
                if (one_line[0] == '[') continue;

                commaBuf.Set(one_line);
                commaBuf.GetString(ref imsi);

                if (imsi == "Resolution") { }
                else if (imsi == "DefaultLocation") { }
                else if (imsi == "Version") { }
                else if (imsi == "ModuleOptic") { }
                else if (imsi == "ModuleWindowStyle") { }
                else if (imsi == "ModuleWindowStyleFlag") { }
                else if (imsi == "BackGroundColor") { }
                else if (imsi == "BackGround") { }

                else if (imsi == "GroupRect")
                {	// Group의 사각형이다. Ver 6.20 부터 사용되었다.
                    int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                    commaBuf.GetInt(ref x1);
                    commaBuf.GetInt(ref y1);
                    commaBuf.GetInt(ref x2);
                    commaBuf.GetInt(ref y2);
                    parent.UpdateZone(x1, y1, x2, y2);
                }
                else if (imsi == "GroupSize")
                {	// Group 실제 크기이다. Ver 6.20 부터 사용되었다.
                    if (p.enumObjectType == EnumObjectType.Group)
                    {
                        commaBuf.GetInt(ref ((ObjectGroup)parent).sizeGroup.cx);
                        commaBuf.GetInt(ref ((ObjectGroup)parent).sizeGroup.cy);

                        parent_canvas.Width = ((ObjectGroup)parent).sizeGroup.cx;
                        parent_canvas.Height = ((ObjectGroup)parent).sizeGroup.cy;
                    }
                }
                else if (imsi == "ObjectGroup")
                {
                    string next_command = "";
                    commaBuf.GetString(ref next_command);
                    if (next_command == "BEGIN")
                    {
                        ObjectGroup group = new ObjectGroup(ocp, null, null, null, null);
                        group.Load(ocp, parent_canvas, form, reader, filename, depth + 1, load_type);
                        parent.AddObject(group);
                    }
                    else if (next_command == "END")
                    {
                        break;	// end of group
                    }
                    else
                    {
                        MessageBox.Show(next_command, "Unknown Group command", MessageBoxButton.OK);
                    }
                }
                else if (imsi == "ObjectLayer")
                {
                    string next_command = "";
                    commaBuf.GetString(ref next_command);
                    if (next_command == "BEGIN")
                    {
                        ObjectLayer group = new ObjectLayer(ocp, null, null, null, null);
                        group.Load(ocp, parent_canvas, form, reader, filename, depth + 1, load_type);

                        parent.AddObject(group);
                    }
                    else if (next_command == "END")
                    {
                        break;	// end of layer
                    }
                    else
                    {
                        MessageBox.Show(next_command, "Unknown Layer command", MessageBoxButton.OK);
                    }
                }
                    
                else if (imsi == "ObjectBitmap")
                {
                    LoadBitmap(ocp, parent_canvas, form, reader, imsi);
                }
                    
                else if (imsi == "ObjectButtonModule3D")
                {
                    LoadButtonModule3D(ocp, parent_canvas, reader, imsi);
                }
                else if (imsi == "ObjectButtonModuleHide")
                {
                    LoadButtonModuleHide(ocp, parent_canvas, reader, imsi);
                }
                else if (imsi == "ObjectButtonProgramm")	// 9.0.8 까지도 ObjectButtonProgramm 이었다. 
                {
                    LoadButtonProgramm(ocp, parent_canvas, reader, imsi);
                }
                else if (imsi == "ObjectButtonProgram")
                {
                    LoadButtonProgramm(ocp, parent_canvas, reader, imsi);
                }
                else if (imsi == "ObjectButtonDigitalOut")
                {
                    LoadButtonDigitalOut(ocp, parent_canvas, reader, imsi);
                }
                else if (imsi == "ObjectAnimation")
                {
                    LoadAnimation(ocp, parent_canvas, form, reader, imsi);
                }
                else if (imsi == "ObjectDigitalAnimation")
                {
                    LoadDigitalAnimation(ocp, parent_canvas, form, reader, imsi);
                }
                else if (imsi == "ObjectDigitalCircle")
                {
                    LoadDigitalCircle(ocp, parent_canvas, reader, imsi, commaBuf);
                }
                else if (imsi == "ObjectDigitalRectangle")
                {
                    LoadDigitalRectangle(ocp, parent_canvas, reader, imsi);
                }
                else if (imsi == "ObjectDigitalString")
                {
                    LoadDigitalString(ocp, parent_canvas, form, reader, imsi);
                }
                else if (imsi == "ObjectAnalogRectangle")
                {
                    LoadAnalogRectangle(ocp, parent_canvas, reader, imsi);
                }
                else if (imsi == "ObjectAnalogString")
                {
                    LoadAnalogString(ocp, parent_canvas, form, reader, imsi);
                }
                else if (imsi == "ObjectAnalogMeter")
                {
                    LoadAnalogMeter(ocp, parent_canvas, reader, imsi);
                }
                else if (imsi == "ObjectAnalogStatus")
                {
                    LoadAnalogStatus(ocp, parent_canvas, form, reader, imsi);
                }
                else if (imsi == "ObjectAnalogRotate")
                {
                    LoadAnalogRotate(ocp, parent_canvas, reader, imsi);
                }
                else if (imsi == "ObjectMultiTrend")
                {
                    LoadMultiTrend(ocp, parent_canvas, form, reader, imsi);
                }
                else if (imsi == "ObjectDatabaseTrend")
                {
                    LoadDatabaseTrend(ocp, parent_canvas, form, reader, imsi);
                }/*
                else if (imsi == "ObjectMilliDataTrend")
                {
                    LoadMilliDataTrend(ocp, form, reader, imsi);
                }*/
                else if (imsi == "ObjectMultiGraph")
                {
                    LoadMultiGraph(ocp, parent_canvas, form, reader, imsi);
                }
                else if (imsi == "ObjectStringString")
                {
                    LoadStringString(ocp, parent_canvas, reader, imsi);
                }
                else if (imsi == "ObjectText")
                {
                    LoadText(ocp, parent_canvas, reader, imsi);
                }
                else if (imsi == "ObjectClock")
                {
                    LoadClock(ocp, parent_canvas, reader, imsi);
                }
                else if (imsi == "ObjectDate")
                {
                    LoadDate(ocp, parent_canvas, reader, imsi);
                }
                /*
                else if (imsi == "ObjectChangeValueDisplay")
                {
                    LoadChangeValueDisplay(ocp, reader, imsi);
                }*/
                else if (imsi == "ObjectRectangle")
                {
                    LoadRectangle(ocp, parent_canvas, reader, imsi);
                }
                else if (imsi == "ObjectRoundRectangle")
                {
                    LoadRoundRectangle(ocp, parent_canvas, reader, imsi);
                }
                else if (imsi == "ObjectSingleText")
                {
                    LoadSingleText(ocp, parent_canvas, form, reader, imsi);
                }
                else if (imsi == "ObjectCircle")
                {
                    LoadCircle(ocp, parent_canvas, reader, imsi);
                }
                else if (imsi == "ObjectLine")
                {
                    LoadLine(ocp, parent_canvas, reader, imsi);
                }
                else if (imsi == "ObjectPoly")
                {
                    LoadPoly(ocp, parent_canvas, form, reader, imsi);
                }
                else if (imsi == "ObjectCurve")
                {
                    LoadObjectCurve(ocp, parent_canvas, form, reader, imsi);
                }
                else if (imsi == "ObjectModule")
                {
                    LoadObjectModule(ocp, parent_canvas, form, reader, imsi, filename);
                }/*
                else if (imsi == "ObjectWindowAlarm")
                {
                    LoadObjectWindowAlarm(ocp, form, reader, imsi);
                }*/
                else if (imsi == "ObjectDatabase")
                {
                    LoadObjectDatabase(ocp, parent_canvas, form, reader, imsi);
                }/*
                else if (imsi == "ObjectDemandWindow")
                {
                    LoadObjectDemandWindow(ocp, form, reader, imsi);
                }*/
                else if (imsi == "ObjectControlComboBox")
                {
                    LoadObjectControlComboBox(ocp, parent_canvas, form, reader, imsi);
                }
                else if (imsi == "ObjectControlCheckBox")
                {
                    LoadObjectControlCheckBox(ocp, parent_canvas, form, reader, imsi);
                }
                else if (imsi == "ObjectControlEditBox")
                {
                    LoadObjectControlEditBox(ocp, parent_canvas, form, reader, imsi);
                }
                else if (imsi == "ObjectControlListBox")
                {
                    LoadObjectControlListBox(ocp, parent_canvas, form, reader, imsi);
                }
                else if (imsi == "ObjectControlRadioButton")
                {
                    LoadObjectControlRadioButton(ocp, parent_canvas, form, reader, imsi);
                }
                else if (imsi == "ObjectControlDatePicker")
                {
                    LoadObjectControlDatePicker(ocp, parent_canvas, form, reader, imsi);
                }
                 /*   
                else if (imsi == "ObjectMilliData")
                {
                    LoadObjectMilliData(ocp, form, reader, imsi);
                }

                else if (String.Compare(imsi, "ObjectRealTimeTestGraph") == 0)
                {
                    LoadObjectRealTimeTestGraph(ocp, form, reader, imsi);
                }

                else if (String.Compare(imsi, "ObjectXYGraph") == 0)
                {
                    LoadObjectXYGraph(ocp, form, reader, imsi);
                }
                else if (String.Compare(imsi, "ObjectWebBrowser") == 0)
                {
                    LoadObjectWebBrowser(ocp, form, reader, imsi);
                }*/
                else if (String.Compare(imsi, "ObjectTagAnimation") == 0)
                {
                    LoadObjectTagAnimation(ocp, parent_canvas, form, reader, imsi);
                }

                else if (String.Compare(imsi, "ExpandScript") == 0) // 10.0 부터 그룹도 확장 기능을 가진다.
                {
                    EXPAND_ID_STRUCT eid = LoadObjectFromModX.LoadExpandScript(reader, imsi);
                    parent.SetExpandIdStruct(eid);
                }
                else if (String.Compare(imsi, "ClassName") == 0) // 10.0 부터 그룹도 클래스 이름을 가진다.
                {
                    LoadObjectItem.LoadClassName(commaBuf, p.objGeneral);
                }
                else if (String.Compare(imsi, "Rotation") == 0)  // 10.0 부터 그룹도 클래스 이름을 가진다.
                {
                    LoadObjectItem.LoadRotation(commaBuf, p.objGeneral);
                    p.RotationAngle = p.objGeneral.fRotateAngle; // Run시에 회전 기본 값이 fRunRotionAngle에 초기화 될 수 있도록 한다.
                }
                
                else
                {
                    LoadUnknownObject(ocp, parent_canvas, reader, imsi, commaBuf);
                }
            }


            /*
            // Group Rect가 설정되어 있지 않으면 6.20 이전 버전이다. root group은 계산하지 않는다.
            if (depth > 0)
            {
                if (p.enumObjectType == EnumObjectType.Group)
                {
                    if (((ObjectGroup)parent).sizeGroup.cx == 0 && ((ObjectGroup)parent).sizeGroup.cy == 0)
                    {
                        ((ObjectGroup)parent).MakeGroupRect(form);
                    }
                }
            }*/

            return 1;
        }

        void LoadRectangle(ObjectCommonProperty ocp, Canvas parent_canvas, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsRectangle args = new ObjectArgsRectangle();

                if (load.sStringOption.Length > 0)	// 9.0.3 이상의 버전
                {
                    CommaTextReader comma = new CommaTextReader();
                    comma.Set(load.sStringOption);
                    comma.GetInt(ref args.nBorderStyle);
                }
                else	// 9.0.2 이하의 버전 
                {
                    args.nBorderStyle = load.wLineOption - 1;
                    if (args.nBorderStyle < 0) args.nBorderStyle = 0;	// 일반선

                    if (load.wLineOption > 1) load.wLineOption = 1;	// 무조건 실선.
                }

                parent.AddObject(new ObjectRectangle(ocp, parent_canvas, load.rRect, load.eID, load.objGeneral,
                    load.lLineColor, load.lFillColor, load.wLineOption, load.wLineThick, args));
            }
        }

        void LoadPoly(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                parent.AddObject(new ObjectPoly(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral,
                    load.lLineColor, load.lFillColor, load.wLineOption, load.wLineThick, load.blockPoint));
            }
        }

        void LoadObjectCurve(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                parent.AddObject(new ObjectCurve(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral,
                    load.lLineColor, load.lFillColor, load.wLineOption, load.wLineThick, load.blockCurve));
            }
        }

        void LoadCircle(ObjectCommonProperty ocp, Canvas parent_canvas, TextReader reader, string command)
        {
            LoadObjectFromCircle load = new LoadObjectFromCircle();

            if (load.run(reader, command))
            {
                parent.AddObject(new ObjectCircle(ocp, parent_canvas, load.rRect, load.eID, load.objGeneral,
                    load.lLineColor, load.lFillColor, load.wLineOption, load.wLineThick, load.objArgs));
            }
        }

        void LoadLine(ObjectCommonProperty ocp, Canvas parent_canvas, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                parent.AddObject(new ObjectLine(ocp, parent_canvas, load.rRect, load.eID, load.objGeneral,
                    load.lLineColor, load.lFillColor, load.wLineOption, load.wLineThick));
            }
        }

        void LoadRoundRectangle(ObjectCommonProperty ocp, Canvas parent_canvas, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsRoundRectangle args = new ObjectArgsRoundRectangle();
                CommaTextReader comma = new CommaTextReader();

                comma.Set(load.sStringOption);
                comma.GetInt(ref args.round_x);
                comma.GetInt(ref args.round_y);

                parent.AddObject(new ObjectRoundRectangle(ocp, parent_canvas, load.rRect, load.eID, load.objGeneral,
                    load.lLineColor, load.lFillColor, load.wLineOption, load.wLineThick, args));
            }
        }

        void LoadBitmap(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsBitmap args = new ObjectArgsBitmap();
                CommaTextReader comma = new CommaTextReader();

                args.sBitmapFile = load.sFileName;
                args.nOverlayMethod = load.nOverlayMethod;
                args.nRotateFlip = load.nRotateFlip;

                parent.AddObject(new ObjectBitmap(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral,
                    args));
            }
        }

        void LoadButtonModule3D(ObjectCommonProperty ocp, Canvas parent_canvas, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsButtonPublic args = new ObjectArgsButtonPublic();
                CommaTextReader comma = new CommaTextReader();

                args.sText = load.sString;
                args.tcolor = load.lTextColor;
                args.bcolor = load.lBackColor;

                parent.AddObject(new ObjectButtonModule3D(ocp, parent_canvas, load.rRect, load.eID, load.fontStruct, load.objGeneral,
                    args, load.sFileName));
            }
        }

        void LoadButtonModuleHide(ObjectCommonProperty ocp, Canvas parent_canvas, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsButtonPublic args = new ObjectArgsButtonPublic();
                CommaTextReader comma = new CommaTextReader();

                args.sText = load.sString;
                args.tcolor = load.lTextColor;
                args.bcolor = load.lBackColor;

                parent.AddObject(new ObjectButtonModuleHide(ocp, parent_canvas, load.rRect, load.eID, load.objGeneral, load.sFileName));
            }
        }

        void LoadButtonProgramm(ObjectCommonProperty ocp, Canvas parent_canvas, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsButtonPublic args = new ObjectArgsButtonPublic();
                CommaTextReader comma = new CommaTextReader();
                int script_type = 0;

                args.sText = load.sString;
                args.tcolor = load.lTextColor;
                args.bcolor = load.lBackColor;
                comma.Set(load.sStringOption);
                comma.GetInt(ref script_type);

                parent.AddObject(new ObjectButtonProgram(ocp, parent_canvas, load.rRect, load.eID, load.fontStruct, load.objGeneral,
                    args, script_type, load.sFileName, load.scriptLocal));
            }
        }

        void LoadButtonDigitalOut(ObjectCommonProperty ocp, Canvas parent_canvas, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsButtonPublic args = new ObjectArgsButtonPublic();
                CommaTextReader comma = new CommaTextReader();

                args.sText = load.sString;
                args.tcolor = load.lTextColor;
                args.bcolor = load.lBackColor;

                parent.AddObject(new ObjectButtonDigitalOut(ocp, parent_canvas, load.rRect, load.eID, load.fontStruct, load.objGeneral,
                    args,
                    load.mouseResponse.do_method,
                    load.blockButtonDoutMember,
                    load.mouseResponse.do_delaytime));
            }
        }

        void LoadAnimation(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsAnimation args = new ObjectArgsAnimation();
                CommaTextReader comma = new CommaTextReader();

                args.sAnimationFile = load.sFileName;
                args.nOverlayMethod = load.nOverlayMethod;
                args.nRotateFlip = load.nRotateFlip;

                parent.AddObject(new ObjectAnimation(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral,
                    args));
            }
        }

        void LoadAnalogString(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsAnalogString args = new ObjectArgsAnalogString();

                args.nBoxUse = load.nBackBox;
                args.nDisplayValue = load.nLocalMethod;
                args.colorBack = load.lBackColor;
                args.colorText = load.lTextColor;

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);
                comma.GetString(ref args.sDisplayFormat);

                parent.AddObject(new ObjectAnalogString(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    load.sTagName, load.mouseResponse, null, args));
            }
        }

        void LoadAnalogMeter(ObjectCommonProperty ocp, Canvas parent_canvas, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsAnalogMeter args = new ObjectArgsAnalogMeter();

                args.colorBack = load.lBackColor;

                args.colorGuide = load.lGuideLineColor;
                args.colorHand = load.lLineColor;
                args.colorText = load.lTextColor;
                args.thickHand = load.wLineThick;
                args.colorBorder = load.lFillColor;

                parent.AddObject(new ObjectAnalogMeter(ocp, parent_canvas, load.rRect, load.eID, load.objGeneral, load.fontStruct, load.sTagName,
                    load.mouseResponse, null, args));
            }
        }

        void LoadAnalogRectangle(ObjectCommonProperty ocp, Canvas parent_canvas, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsAnalogRectangle args = new ObjectArgsAnalogRectangle();
                CommaTextReader comma = new CommaTextReader();
                VIEW_RANGE_STRUCT view = new VIEW_RANGE_STRUCT();
                GUIDE_LINE_STRUCT guide = new GUIDE_LINE_STRUCT();
                byte r = 0, g = 0, b = 0, a = 0;

                comma.Set(load.sStringOption);

                comma.GetInt(ref args.nBarDir);

                comma.GetChar(ref view.flag);
                comma.GetDouble(ref view.fBase);
                comma.GetDouble(ref view.fFull);

                comma.GetChar(ref guide.method);
                comma.GetInt(ref guide.devideBig);
                comma.GetBYTE(ref r);
                comma.GetBYTE(ref g);
                comma.GetBYTE(ref b);
                comma.GetBYTE(ref a);
                guide.colorBig = Color.FromArgb(a, r, g, b);
                comma.GetInt(ref guide.devideSmall);
                comma.GetBYTE(ref r);
                comma.GetBYTE(ref g);
                comma.GetBYTE(ref b);
                comma.GetBYTE(ref a);
                guide.colorSmall = Color.FromArgb(a, r, g, b);

                comma.GetInt(ref guide.line_length);
                comma.GetChar(ref guide.bLevelString);

                args.colorOn = load.lFillColor;
                args.colorOff = load.lBackColor;

                parent.AddObject(new ObjectAnalogRectangle(ocp, parent_canvas, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    load.sTagName, load.mouseResponse, null, args, view, guide));
            }
        }

        void LoadAnalogRotate(ObjectCommonProperty ocp, Canvas parent_canvas, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsAnalogRotate args = new ObjectArgsAnalogRotate();

                args.bAngleDirection = (sbyte)load.nAngleDirection;
                args.lBackColor = load.lBackColor;
                args.nEndAngle = (int)load.fEndAngle;
                args.nMethod = load.nLocalMethod;
                args.nStartAngle = (int)load.fStartAngle;
                args.sFileName = load.sFileName;

                parent.AddObject(new ObjectAnalogRotate(ocp, parent_canvas, load.rRect, load.eID, load.objGeneral,
                    load.sTagName, load.mouseResponse, null, args));
            }
        }

        void LoadAnalogStatus(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsAnalogStatus args = new ObjectArgsAnalogStatus();

                parent.AddObject(new ObjectAnalogStatus(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral,
                    load.sTagName, load.mouseResponse, null, args, load.blockAnalogStatus));
            }
        }

        void LoadDigitalAnimation(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsDigitalAnimation args = new ObjectArgsDigitalAnimation();

                args.sFileOff = load.sFileNameOff;
                args.sFileOn = load.sFileNameOn;
                args.nOverlayMethod = load.nOverlayMethod;
                args.nRotateFlip = load.nRotateFlip;

                parent.AddObject(new ObjectDigitalAnimation(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral,
                    load.sTagName, load.mouseResponse, null, args));
            }
        }

        void LoadDigitalCircle(ObjectCommonProperty ocp, Canvas parent_canvas, TextReader reader, string command, CommaTextReader comma)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsDigitalCircle args = new ObjectArgsDigitalCircle();

                args.colorOff = load.lOffColor;
                args.colorOn = load.lOnColor;

                parent.AddObject(new ObjectDigitalCircle(ocp, parent_canvas, load.rRect, load.eID, load.objGeneral,
                    load.sTagName, load.mouseResponse, null, args));
            }
        }

        void LoadDigitalRectangle(ObjectCommonProperty ocp, Canvas parent_canvas, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsDigitalRectangle args = new ObjectArgsDigitalRectangle();

                args.colorOff = load.lOffColor;
                args.colorOn = load.lOnColor;

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);
                comma.GetInt(ref args.nDisplayMethod);

                parent.AddObject(new ObjectDigitalRectangle(ocp, parent_canvas, load.rRect, load.eID, load.objGeneral,
                    load.sTagName, load.mouseResponse, null, args));
            }
        }

        void LoadDigitalString(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsDigitalString args = new ObjectArgsDigitalString();

                args.colorBack = load.lBackColor;
                args.colorOff = load.lOffColor;
                args.colorOn = load.lOnColor;

                parent.AddObject(new ObjectDigitalString(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    load.sTagName, load.mouseResponse, null, args));
            }
        }

        void LoadStringString(ObjectCommonProperty ocp, Canvas parent_canvas, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsStringString args = new ObjectArgsStringString();

                args.nBoxUse = load.nBackBox;
                args.colorBack = load.lBackColor;
                args.colorText = load.lTextColor;

                parent.AddObject(new ObjectStringString(ocp, parent_canvas, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    load.sTagName, load.mouseResponse, null, args, load.align));
            }
        }

        void LoadMultiTrend(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsMultiTrend args = new ObjectArgsMultiTrend();

                args.lColorBack = load.lBackColor;
                args.lColorGuideLine = load.lGuideLineColor;
                args.lColorText = load.lTextColor;
                args.lColorFill = load.lFillColor;
                //args.pub.wDisplayFlags = (EnumDisplayFlag)load.wFlags;
                //args.pub.wPointSize = load.wGraphPointSize;
                //args.pub.nLevelDisplaySize = load.nLevelDisplaySize;
                args.pub = load.argsGraphPublic;
                args.wLevelDevide = load.wLevelDevide;
                args.wShowUnit = load.nShowUnit;
                args.wTimeDevide = load.wTimeDevide;
                args.wTimeSelectOption = load.wTimeSelectOption;

                if (load.objGeneral.sClassName.Length == 0)
                    load.objGeneral.sClassName = load.sFileName;

                parent.AddObject(new ObjectMultiTrend(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args, load.graphMember));
            }
        }

        void LoadDatabaseTrend(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsDatabaseTrend args = new ObjectArgsDatabaseTrend();

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);

                comma.GetString(ref args.sDsn);
                comma.GetString(ref args.sTable);
                comma.GetString(ref args.sColumnTime);
                comma.GetInt(ref args.nDataCycle);
                comma.GetString(ref args.sColumnMilli);
                comma.GetInt(ref args.nDateColumnType);
                comma.GetInt(ref args.nBasicSpaceLeft);
                comma.GetInt(ref args.nBasicSpaceRight);

                args.lColorBack = load.lBackColor;
                args.lColorGuideLine = load.lGuideLineColor;
                args.lColorText = load.lTextColor;
                args.lColorFill = load.lFillColor;
                //args.pub.wDisplayFlags = (EnumDisplayFlag)load.wFlags;
                args.wLevelDevide = load.wLevelDevide;
                //args.wPointSize = load.wGraphPointSize;
                args.wShowUnit = load.nShowUnit;
                args.wTimeDevide = load.wTimeDevide;
                args.wTimeSelectOption = load.wTimeSelectOption;
                //args.pub.nLevelDisplaySize = load.nLevelDisplaySize;
                args.pub = load.argsGraphPublic;

                if (load.objGeneral.sClassName.Length == 0)
                    load.objGeneral.sClassName = load.sFileName;

                parent.AddObject(new ObjectDatabaseTrend(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args, load.dbTrendMember));
            }
        }

        /*
        void LoadMilliDataTrend(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsMilliDataTrend args = new ObjectArgsMilliDataTrend();

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);

                comma.GetString(ref args.sDsn);
                comma.Skip();// comma.GetString(ref args.sTable);
                comma.Skip();// comma.GetString(ref args.sColumnTime);
                comma.GetInt(ref args.nDataCycle);
                comma.Skip();// comma.GetString(ref args.sColumnMilli);
                comma.Skip();// comma.GetInt(ref args.nDateColumnType);
                comma.GetInt(ref args.nBasicSpaceLeft);
                comma.GetInt(ref args.nBasicSpaceRight);

                args.lColorBack = load.lBackColor;
                args.lColorGuideLine = load.lGuideLineColor;
                args.lColorText = load.lTextColor;
                args.lColorFill = load.lFillColor;
                args.wDisplayFlags = (EnumDisplayFlag)load.wFlags;
                args.wLevelDevide = load.wLevelDevide;
                args.wPointSize = load.wGraphPointSize;
                args.wShowUnit = load.nShowUnit;
                args.wTimeDevide = load.wTimeDevide;
                args.wTimeSelectOption = load.wTimeSelectOption;
                args.nLevelDisplaySize = load.nLevelDisplaySize;

                if (load.objGeneral.sClassName.Length == 0)
                    load.objGeneral.sClassName = load.sFileName;

                parent.AddObject(new ObjectMilliDataTrend(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args, load.mdTrendMember));
            }
        }*/

        void LoadMultiGraph(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsMultiGraph args = new ObjectArgsMultiGraph();

                args.lColorBack = load.lBackColor;
                args.lColorGuideLine = load.lGuideLineColor;
                args.lColorText = load.lTextColor;
                args.lColorFill = load.lFillColor;
                //args.pub.wDisplayFlags = (EnumDisplayFlag)load.wFlags;
                //args.pub.nLevelDisplaySize = load.nLevelDisplaySize;
                //args.pub.wPointSize = load.wGraphPointSize;
                args.pub = load.argsGraphPublic;
                args.wLevelDevide = load.wLevelDevide;
                args.wShowUnit = load.nShowUnit;
                args.wTimeDevide = load.wTimeDevide;
                //args.wTimeSelectOption = load.wTimeSelectOption;

                if (load.objGeneral.sClassName.Length == 0)
                    load.objGeneral.sClassName = load.sFileName;

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);
                comma.GetInt(ref args.nDataTime);
                comma.GetChar(ref args.bTimeDirToLeft);
                comma.GetChar(ref args.bDisplayByTime);

                parent.AddObject(new ObjectMultiGraph(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args, load.graphMember));
            }
        }

        void LoadSingleText(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsSingleText args = new ObjectArgsSingleText();

                args.text = load.sString;
                args.textColor = load.lTextColor;

                //load.rRect.top -= 4;
                //load.rRect.bottom -= 4;

                parent.AddObject(new ObjectSingleText(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }

        void LoadText(ObjectCommonProperty ocp, Canvas parent_canvas, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();
            load.bMultiString = true;   // 여러줄 텍스트를 사용한다.

            load.align.x = 0;   // 다중라인을 지원하면서 정렬 정보가 없는 이전 버전은 왼쪽/위를 Default로 정렬한다.
            load.align.y = 0;

            if (load.run(reader, command))
            {
                ObjectArgsText args = new ObjectArgsText();

                args.text = load.sString;
                args.textColor = load.lTextColor;
                args.align = load.align;

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);
                comma.GetBool(ref args.formatFlagDirectionVertical);
                comma.GetBool(ref args.formatFlagNoWrap);

                parent.AddObject(new ObjectText(ocp, parent_canvas, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }

        void LoadDate(ObjectCommonProperty ocp, Canvas parent_canvas, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsDate args = new ObjectArgsDate();

                args.textColor = load.lTextColor;
                args.backColor = load.lBackColor;

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);
                comma.GetInt(ref args.type);

                parent.AddObject(new ObjectDate(ocp, parent_canvas, load.rRect, load.eID, load.objGeneral, load.fontStruct, args));
            }
        }

        /*
        void LoadChangeValueDisplay(ObjectCommonProperty ocp, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsChangeValueDisplay args = new ObjectArgsChangeValueDisplay();

                args.textColor = load.lTextColor;
                args.backColor = load.lBackColor;

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);
                comma.GetInt(ref args.nListCount);

                parent.AddObject(new ObjectChangeValueDisplay(ocp, load.rRect, load.eID, load.objGeneral, load.fontStruct, args));
            }
        }*/

        void LoadClock(ObjectCommonProperty ocp, Canvas parent_canvas, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsClock args = new ObjectArgsClock();

                args.textColor = load.lTextColor;
                args.backColor = load.lBackColor;

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);
                comma.GetInt(ref args.type);

                parent.AddObject(new ObjectClock(ocp, parent_canvas, load.rRect, load.eID, load.objGeneral, load.fontStruct, args));
            }
        }


        void LoadObjectControlCheckBox(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsControlCheckBox args = new ObjectArgsControlCheckBox();

                args.sTag = load.sTagName;
                args.rgbColor = load.lTextColor;

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);
                comma.GetString(ref args.sTitle);

                parent.AddObject(new ObjectControlCheckBox(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }

        void LoadObjectControlComboBox(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsControlComboBox args = new ObjectArgsControlComboBox();

                args.dwWindowStyle = load.dwWindowStyle;
                args.sTag = load.sTagName;
                args.arrayListData = load.blockListData;
                args.textColor = load.lTextColor;
                args.backColor = load.lBackColor;

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);
                comma.GetInt(ref args.nValueConvert);

                parent.AddObject(new ObjectControlComboBox(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }

        void LoadObjectControlEditBox(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsControlEditBox args = new ObjectArgsControlEditBox();

                args.dwWindowStyle = load.dwWindowStyle;
                args.sTag = load.sTagName;
                args.textColor = load.lTextColor;
                args.backColor = load.lBackColor;

                parent.AddObject(new ObjectControlEditBox(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }

        void LoadObjectControlListBox(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsControlListBox args = new ObjectArgsControlListBox();

                args.dwWindowStyle = load.dwWindowStyle;
                args.sTag = load.sTagName;
                args.arrayListData = load.blockListData;
                args.textColor = load.lTextColor;
                args.backColor = load.lBackColor;

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);
                comma.GetInt(ref args.nValueConvert);

                if (comma.IsEOS())
                {
                    args.nSelectionMode = 1;   // nSelectionMode = 9.3.7.5 부터 추가됨
                }
                else
                {
                    comma.GetInt(ref args.nSelectionMode);
                }

                parent.AddObject(new ObjectControlListBox(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }

        void LoadObjectControlRadioButton(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsControlRadioButton args = new ObjectArgsControlRadioButton();

                args.sTag = load.sTagName;
                args.rgbColor = load.lTextColor;
                args.arrayListData = load.blockListData;

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);

                parent.AddObject(new ObjectControlRadioButton(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }

        void LoadObjectControlDatePicker(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsControlDatePicker args = new ObjectArgsControlDatePicker();

                args.dwWindowStyle = load.dwWindowStyle;
                //args.sTag = load.sTagName;
                args.textColor = load.lTextColor;
                args.backColor = load.lBackColor;

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);
                comma.GetString(ref args.sFormat);

                parent.AddObject(new ObjectControlDatePicker(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }

        void LoadObjectModule(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command, string parent_file)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsModule args = new ObjectArgsModule();

                args.filename = load.sFileName;

                parent.AddObject(new ObjectModule(ocp, parent_canvas, form, parent_file, load.rRect, load.eID, load.objGeneral,
                    args));
            }
        }

        void LoadUnknownObject(ObjectCommonProperty ocp, Canvas parent_canvas, TextReader reader, string command, CommaTextReader comma)
        {
            string begin = "";

            comma.GetString(ref begin);

            if (String.Compare(begin, "BEGIN", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                LoadObjectFromModX load = new LoadObjectFromModX();

                if (load.run(reader, command))
                {
                    parent.AddObject(new ObjectUnknown(ocp, parent_canvas, command, load.rRect, load.eID, load.objGeneral));
                }
            }
            else
            {

            }
        }

        void LoadObjectDatabase(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsDatabase args = new ObjectArgsDatabase();

                args.filename = load.sFileName;
                args.lColorBack = load.lBackColor;
                args.lColorText = load.lTextColor;

                CommaTextReader comma = new CommaTextReader();

                comma.Set(load.sStringOption);
                comma.GetString(ref args.table);
                comma.GetChar(ref args.bUseNo);
                comma.GetInt(ref args.nConnectionType);
                comma.GetChar(ref args.bAutoUpdate);
                comma.GetBool(ref args.bUseGrid);
                comma.GetChar(ref args.bUseFullCursor);
                comma.GetInt(ref args.nUpdateTime);
                comma.GetString(ref args.dsn);

                parent.AddObject(new ObjectDatabase(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }
        /*
        void LoadObjectWindowAlarm(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsWindowAlarm args = new ObjectArgsWindowAlarm();

                CommaBlockString comma = new CommaBlockString();

                comma.Set(load.sStringOption);
                comma.GetInt(ref args.cIncludeMethod);
                comma.GetChar(ref args.bFlagWindowCaption);
                comma.GetChar(ref args.bFlagUseColumnHeader);

                parent.AddObject(new ObjectWindowAlarm(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }

        void LoadObjectDemandWindow(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsDemandWindow args = new ObjectArgsDemandWindow();

                CommaBlockString comma = new CommaBlockString();

                comma.Set(load.sStringOption);
                comma.GetInt(ref args.cIncludeMethod);
                comma.GetInt(ref args.bFlagWindowCaption);
                comma.GetString(ref args.demand_name);
                comma.GetInt(ref args.thick_target);


                parent.AddObject(new ObjectDemandWindow(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }

        void LoadObjectMilliData(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsMilliData args = new ObjectArgsMilliData();

                CommaBlockString comma = new CommaBlockString();

                comma.Set(load.sStringOption);
                comma.GetString(ref args.sTitle);

                parent.AddObject(new ObjectMilliData(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }

        void LoadObjectRealTimeTestGraph(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsRealTimeTestGraph args = new ObjectArgsRealTimeTestGraph();

                args.lColorBack = load.lBackColor;
                args.lColorFill = load.lFillColor;
                args.lColorGuideLine = load.lGuideLineColor;
                args.lColorText = load.lTextColor;
                args.pub.wPointSize = load.wGraphPointSize;
                args.wShowUnit = load.nShowUnit;
                args.pub.wDisplayFlags = (EnumDisplayFlag)load.wFlags;
                args.pub.nLevelDisplaySize = load.nLevelDisplaySize;
                args.wLevelDevide = load.wLevelDevide;
                args.wTimeDevide = load.wTimeDevide;
                //args.wTimeSelectOption = load.wTimeSelectOption;

                CommaBlockString comma = new CommaBlockString();
                comma.Set(load.sStringOption);
                comma.GetInt(ref args.nDataTime);
                comma.GetString(ref args.sTagStart);


                int color = 0;
                comma.GetInt(ref color);
                Color basicLineColor = Color.FromArgb(color);
                int basicLineThick = 1, basicPointType = 0;

                comma.GetInt(ref basicLineThick);
                comma.GetInt(ref basicPointType);
                comma.GetInt(ref args.nTimeDisplayType);
                comma.GetChar(ref args.bDisableCursor);
                comma.GetString(ref args.sTagRun);

                if (load.objGeneral.sClassName.Length == 0)
                    load.objGeneral.sClassName = load.sFileName;

                if (load.blockTwoTagMemberList.Count == 0)  // 9.2.0 이전에는 이항목이 없었다.
                {
                    TWO_TAG_MEMBER_LIST list = new TWO_TAG_MEMBER_LIST();
                    list.arrayMember = load.blockTwoTagMember;
                    list.color = basicLineColor;
                    list.nLineThick = basicLineThick;
                    list.nPointType = basicPointType;

                    load.blockTwoTagMemberList.Add(list);
                }

                parent.AddObject(new ObjectRealTimeTestGraph(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args, load.graphMember, load.blockTwoTagMemberList));
            }
        }

        void LoadObjectXYGraph(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsXYGraph args = new ObjectArgsXYGraph();

                args.bcolor = load.lBackColor;
                args.lColorFill = load.lFillColor;
                args.gcolor = load.lGuideLineColor;
                args.tcolor = load.lTextColor;
                args.point_size = load.wGraphPointSize;
                args.showunit = load.nShowUnit;
                args.wDisplayFlags = (EnumDisplayFlag)load.wFlags;
                args.leveldevide = load.wLevelDevide;
                args.wTimeDevide = load.wTimeDevide;

                CommaBlockString comma = new CommaBlockString();
                comma.Set(load.sStringOption);
                comma.GetInt(ref args.nDataTime);

                if (load.objGeneral.sClassName.Length == 0)
                    load.objGeneral.sClassName = load.sFileName;

                parent.AddObject(new ObjectXYGraph(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args, load.graphMemberXY));
            }
        }

        void LoadObjectWebBrowser(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX();

            if (load.run(reader, command))
            {
                ObjectArgsWebBrowser args = new ObjectArgsWebBrowser();

                CommaBlockString comma = new CommaBlockString();
                comma.Set(load.sStringOption);
                comma.GetString(ref args.url);

                UInt32 style = 0;
                comma.GetHexDWORD(ref style);

                args.styleContextMenu = (style & Tools.DWORD_MASK[0]) > 0;
                args.styleNavigation = (style & Tools.DWORD_MASK[1]) > 0;
                args.styleScrollBar = (style & Tools.DWORD_MASK[2]) > 0;

                parent.AddObject(new ObjectWebBrowser(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }*/

        void LoadObjectTagAnimation(ObjectCommonProperty ocp, Canvas parent_canvas, UserControl form, TextReader reader, string command)
        {
            LoadObjectFromTagAnimation load = new LoadObjectFromTagAnimation();

            if (load.run(reader, command))
            {
                ObjectArgsTagAnimation args = new ObjectArgsTagAnimation();

                args.nOverlayMethod = load.nOverlayMethod;
                args.nRotateFlip = load.nRotateFlip;
                args.member = load.blockTagAnimation;

                parent.AddObject(new ObjectTagAnimation(ocp, parent_canvas, form, load.rRect, load.eID, load.objGeneral, load.sTagName, load.mouseResponse, args));
            }
        }
    }
}
