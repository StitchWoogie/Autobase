using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using NetTools;
using NetTools.OldDefine;
using AutoLib;
using AutoLibLocal;
using AutoLibLocal.DemandNew;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for ObjectGroupLoadModX.
	/// </summary>
	public class ObjectGroupLoadModX
	{
        ObjectPublicGroupLayer parent;

		public ObjectGroupLoadModX()
		{
			//
			// TODO: Add constructor logic here
			//
		}
 
		public int Load(ObjectCommonProperty ocp, ObjectPublicGroupLayer p, Form form, TextReader reader, int depth, EnumModType load_type)
		{
			parent = p;
			string one_line="";
			string imsi ="";
			CommaTextReader commaBuf = new CommaTextReader();
			
			parent.FreeAllObjectBuf();

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;
				if(one_line.Length == 0)	continue;
				if(one_line[0] == '[')	continue;

				commaBuf.Set(one_line);
				commaBuf.GetString(ref imsi);

				if(imsi == "GroupRect") 
				{	// Group의 사각형이다. Ver 6.20 부터 사용되었다.
                    int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                    commaBuf.GetInt(ref x1);
                    commaBuf.GetInt(ref y1);
                    commaBuf.GetInt(ref x2);
                    commaBuf.GetInt(ref y2);
                    parent.UpdateZone(form, x1, y1, x2, y2);
				}
				else if(imsi == "GroupSize") 
				{	// Group 실제 크기이다. Ver 6.20 부터 사용되었다.
                    if (p.enumObjectType == EnumObjectType.Group)
                    {
						commaBuf.GetInt(ref ((ObjectGroup)parent).sizeGroup.cx);
                        commaBuf.GetInt(ref ((ObjectGroup)parent).sizeGroup.cy);
                    }
				}
				else if(imsi == "ObjectGroup") 
				{
					string next_command="";
					commaBuf.GetString(ref next_command);
					if(next_command == "BEGIN") 
					{
                        ObjectGroup group = new ObjectGroup(ocp, null, null, null, null);
						group.Load(ocp, form, reader, depth+1, load_type);
						parent.AddObject(group);
					}
					else if(next_command == "END") 
					{	
						break;	// end of group
					}
					else 
					{
						MessageBox.Show(next_command, "Unknown Group command");
					}
				}

                else if (imsi == "ObjectLayer")
                {
                    string next_command = "";
                    commaBuf.GetString(ref next_command);
                    if (next_command == "BEGIN")
                    {
                        ObjectLayer layer = new ObjectLayer(ocp, null, null, null, null);
                        
                        commaBuf.Skip();    // Layer flags
                        commaBuf.GetString(ref layer.objGeneral.sOnStudioTitle);
                        int r=0, g=0, b=0, a=0;
                        commaBuf.GetInt(ref r);
                        commaBuf.GetInt(ref g);
                        commaBuf.GetInt(ref b);
                        commaBuf.GetInt(ref a);

                        sbyte flag = 0;
                        commaBuf.GetChar(ref flag);
                        if (AutoLib.ConfigStudio.bSaveLayerLockStatus)  // 
                        {
                            layer.objGeneral.bOnStudioLocked = (flag == 1);
                        }

                        commaBuf.GetChar(ref flag);
                        if (AutoLib.ConfigStudio.bSaveLayerShowStatus && !ocp.bLoadOnLibraryView)
                        {
                            layer.objGeneral.bOnStudioVisible = (flag == 1);
                        }

                        layer.LayerColor = Color.FromArgb(r, g, b);    // a 는 사용하지 않는다.

                        layer.Load(ocp, form, reader, depth + 1, load_type);

                        parent.AddObject(layer);
                    }
                    else if (next_command == "END")
                    {
                        break;	// end of layer
                    }
                    else
                    {
                        MessageBox.Show(next_command, "Unknown Layer command");
                    }
                }

				else if(imsi == "ObjectBitmap")
				{
					LoadBitmap(ocp, form, reader, imsi);
				}
				else if(imsi == "ObjectMergedBitmap")
				{
					LoadMergedBitmap(ocp, reader, imsi);
				}

				else if(imsi == "ObjectButtonModule3D") 
				{
					LoadButtonModule3D(ocp, reader, imsi);
				}
				else if(imsi == "ObjectButtonModuleHide") 
				{
					LoadButtonModuleHide(ocp, reader, imsi);
				}
				else if(imsi == "ObjectButtonProgramm")	// 9.0.8 까지도 ObjectButtonProgramm 이었다. 
				{
					LoadButtonProgramm(ocp, reader, imsi);
				}
				else if(imsi == "ObjectButtonProgram") 
				{
					LoadButtonProgramm(ocp, reader, imsi);
				}
				else if(imsi == "ObjectButtonDigitalOut") 
				{
					LoadButtonDigitalOut(ocp, reader, imsi);
				}
				else if(imsi == "ObjectAnimation") 
				{
					LoadAnimation(ocp, form, reader, imsi);
				}
				else if(imsi == "ObjectDigitalAnimation") 
				{
					LoadDigitalAnimation(ocp, form, reader, imsi);
				}
				else if(imsi == "ObjectDigitalCircle") 
				{
					LoadDigitalCircle(ocp, reader, imsi, commaBuf);
				}
				else if(imsi == "ObjectDigitalRectangle") 
				{
					LoadDigitalRectangle(ocp, reader, imsi);
				}
				else if(imsi == "ObjectDigitalString") 
				{
					LoadDigitalString(ocp, form, reader, imsi);
				}
				else if(imsi == "ObjectAnalogRectangle") 
				{
					LoadAnalogRectangle(ocp, reader, imsi);
				}
				else if(imsi == "ObjectAnalogString") 
				{
					LoadAnalogString(ocp, form, reader, imsi);
				}
				else if(imsi == "ObjectAnalogMeter") 
				{
					LoadAnalogMeter(ocp, reader, imsi);
				}
				else if(imsi == "ObjectAnalogStatus") 
				{
					LoadAnalogStatus(ocp, form, reader, imsi);
				}
				else if(imsi == "ObjectAnalogRotate") 
				{
					LoadAnalogRotate(ocp, reader, imsi);
				}
                else if (imsi == "ObjectAnalogGauge") //25-02-04 hsjeong
                {
                    LoadAnalogGauge(ocp, reader, imsi);
                }
				else if(imsi == "ObjectMultiTrend") 
				{
					LoadMultiTrend(ocp, form, reader, imsi);
				}
				else if(imsi == "ObjectDatabaseTrend") 
				{
					LoadDatabaseTrend(ocp, form, reader, imsi);
				}
                else if (imsi == "ObjectMilliDataTrend")
                {
                    LoadMilliDataTrend(ocp, form, reader, imsi);
                }
				else if(imsi == "ObjectMultiGraph") 
				{
					LoadMultiGraph(ocp, form, reader, imsi);
				}
                else if (imsi == "ObjectDonutChart") //HSJEONG 25-02-04 도넛차트
                {
                    LoadDonutChart(ocp, reader, imsi);
                }
                else if (imsi == "ObjectDemandChart")
                {
                    LoadDemandChart(ocp, form, reader, imsi);
                }
                else if (imsi == "CustomChart") //25-02-24
                {
                    LoadCustomChart(ocp, form, reader, imsi);
                }
                else if (imsi == "ObjectBarcodeDisplay") //26-03-09 바코드 표시
                {
                    LoadBarcodeDisplay(ocp, form, reader, imsi);
                }
                else if (imsi == "ObjectBarcodeScanner") //26-03-09 바코드 스캐너
                {
                    LoadBarcodeScanner(ocp, form, reader, imsi);
                }
				else if(imsi == "ObjectStringString") 
				{
					LoadStringString(ocp, reader, imsi);
				}
				else if(imsi == "ObjectText") 
				{
					LoadText(ocp, reader, imsi);
				}
				else if(imsi == "ObjectClock") 
				{
					LoadClock(ocp, reader, imsi);
				}
				else if(imsi == "ObjectDate") 
				{
					LoadDate(ocp, reader, imsi);
				}
				else if(imsi == "ObjectChangeValueDisplay") 
				{
					LoadChangeValueDisplay(ocp, reader, imsi);
				}
				else if(imsi == "ObjectRectangle") 
				{
					LoadRectangle(ocp, reader, imsi);
				}
				else if(imsi == "ObjectRoundRectangle") 
				{
					LoadRoundRectangle(ocp, reader, imsi);
				}
				else if(imsi == "ObjectSingleText") 
				{
					LoadSingleText(ocp, form, reader, imsi);
				}
				else if(imsi == "ObjectCircle") 
				{
					LoadCircle(ocp, reader, imsi);
				}
				else if(imsi == "ObjectLine") 
				{
					LoadLine(ocp, reader, imsi);
				}
				else if(imsi == "ObjectPoly") 
				{
					LoadPoly(ocp, form, reader, imsi);
				}
				else if(imsi == "ObjectCurve") 
				{
					LoadObjectCurve(ocp, form, reader, imsi);
				}
				else if(imsi == "ObjectModule") 
				{
					LoadObjectModule(ocp, form, reader, imsi);
				}
				else if(imsi == "ObjectWindowAlarm") 
				{
					LoadObjectWindowAlarm(ocp, form, reader, imsi);
				}
				else if(imsi == "ObjectDatabase") 
				{
					LoadObjectDatabase(ocp, form, reader, imsi);
				}
					/*
					else if(strcmp(imsi, "ObjectVideo") == 0) {
						LoadObjectVideo(hwnd, in, imsi);
					}
					*/
				else if(imsi == "ObjectDemandWindow") 
				{
					LoadObjectDemandWindow(ocp, form, reader, imsi);
				}
					/*
					else if(strcmp(imsi, "ObjectActiveX") == 0) {
						LoadObjectActiveX(hwnd, in, imsi);
					}
					*/
				else if(imsi == "ObjectControlComboBox") 
				{
					LoadObjectControlComboBox(ocp, form, reader, imsi);
				}
				else if(imsi == "ObjectControlCheckBox") 
				{
					LoadObjectControlCheckBox(ocp, form, reader, imsi);
				}
				else if(imsi == "ObjectControlEditBox") 
				{
					LoadObjectControlEditBox(ocp, form, reader, imsi);
				}
				else if(imsi == "ObjectControlListBox") 
				{
					LoadObjectControlListBox(ocp, form, reader, imsi);
				}
				else if(imsi == "ObjectControlRadioButton") 
				{
					LoadObjectControlRadioButton(ocp, form, reader, imsi);
				}
                else if (imsi == "ObjectControlDatePicker")
                {
                    LoadObjectControlDatePicker(ocp, form, reader, imsi);
                }
                else if (imsi == "ObjectControlTabControl")
                {
                    LoadObjectControlTabControl(ocp, form, reader, imsi);
                }
                else if (imsi == "ObjectControlTreeView")
                {
                    LoadObjectControlTreeView(ocp, form, reader, imsi);
                }

				else if(imsi == "ObjectMilliData") {
					LoadObjectMilliData(ocp, form, reader, imsi);
				}

				else if(String.Compare(imsi, "ObjectRealTimeTestGraph") == 0) {
					LoadObjectRealTimeTestGraph(ocp, form, reader, imsi);
				}

				else if(String.Compare(imsi, "ObjectXYGraph") == 0) 
				{
					LoadObjectXYGraph(ocp, form, reader, imsi);
				}
                else if (String.Compare(imsi, "ObjectWebBrowser") == 0)
                {
                    LoadObjectWebBrowser(ocp, form, reader, imsi);
                }
                else if (String.Compare(imsi, "ObjectWebView") == 0)   //20240627 WebView PSU
                {
                    LoadObjectWebView(ocp, form, reader, imsi);
                }
                else if (String.Compare(imsi, "ObjectVLCAx") == 0)   //20240626 VLCAx PSU
                {
                    LoadObjectVLCAx(ocp, form, reader, imsi);
                }
                else if (imsi == "ObjectSVG") //20241024 SVG PSU
                {
                    LoadSVG(ocp, form, reader, imsi);
                }
                else if (String.Compare(imsi, "ObjectTagAnimation") == 0)
                {
                    LoadObjectTagAnimation(ocp, form, reader, imsi);
                }
                else if (String.Compare(imsi, "ObjectDataGridView") == 0)
                {
                    LoadObjectDataGridView(ocp, form, reader, imsi);
                }
                else if (String.Compare(imsi, "ExpandScript") == 0) // 10.0 부터 그룹도 확장 기능을 가진다.
                {
                    EXPAND_ID_STRUCT eid = LoadObjectFromModX.LoadExpandScript(reader, imsi);
                    parent.SetExpandIdStruct(eid);
                }
                else if (String.Compare(imsi, "ClassName") == 0) // 10.0 부터 그룹도 클래스 이름을 가진다.
                {
                    LoadObjectItem.LoadClassName(commaBuf, p.objGeneral, ocp);
                }
                else if (String.Compare(imsi, "Rotation") == 0)  // 10.0 부터 그룹도 클래스 이름을 가진다.
                {
                    LoadObjectItem.LoadRotation(commaBuf, p.objGeneral);
                    p.RotationAngle = p.objGeneral.fRotateAngle; // Run시에 회전 기본 값이 fRunRotionAngle에 초기화 될 수 있도록 한다.
                }

				else 
				{
					LoadUnknownObject(ocp, reader, imsi, commaBuf);
				}
			}
			
            /* MODX 파일은 6.2 이전 버전이 없다.
            // Group Rect가 설정되어 있지 않으면 6.20 이전 버전이다. root group은 계산하지 않는다.
			if(depth > 0) 
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

		void LoadRectangle(ObjectCommonProperty ocp, TextReader reader, string command)
		{
			LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsRectangle args = new ObjectArgsRectangle();

				if(load.sStringOption.Length > 0)	// 9.0.3 이상의 버전
				{
					CommaTextReader comma = new CommaTextReader();
					comma.Set(load.sStringOption);
					comma.GetInt(ref args.nBorderStyle);
				}
				else	// 9.0.2 이하의 버전 
				{
					args.nBorderStyle = load.wLineOption-1;
					if(args.nBorderStyle < 0)	args.nBorderStyle = 0;	// 일반선

					if(load.wLineOption > 1)	load.wLineOption = 1;	// 무조건 실선.
				}

				parent.AddObject(new ObjectRectangle(ocp, load.rRect, load.eID, load.objGeneral,
					load.lLineColor, load.lFillColor, load.wLineOption, load.wLineThick, args));
			}
		}

		void LoadPoly(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command)
		{
			LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				parent.AddObject(new ObjectPoly(ocp, form, load.rRect, load.eID, load.objGeneral,
					load.lLineColor, load.lFillColor, load.wLineOption, load.wLineThick, load.blockPoint));
			}
		}

		void LoadObjectCurve(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command)
		{
			LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				parent.AddObject(new ObjectCurve(ocp, form, load.rRect, load.eID, load.objGeneral,
					load.lLineColor, load.lFillColor, load.wLineOption, load.wLineThick, load.blockCurve));
			}
		}

		void LoadCircle(ObjectCommonProperty ocp, TextReader reader, string command)
		{
            LoadObjectFromCircle load = new LoadObjectFromCircle(ocp);

			if(load.run(reader, command)) 
			{
				parent.AddObject(new ObjectCircle(ocp, load.rRect, load.eID, load.objGeneral,
					load.lLineColor, load.lFillColor, load.wLineOption, load.wLineThick, load.objArgs));
			}
		}

		void LoadLine(ObjectCommonProperty ocp, TextReader reader, string command)
		{
			LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				parent.AddObject(new ObjectLine(ocp, load.rRect, load.eID, load.objGeneral,
					load.lLineColor, load.lFillColor, load.wLineOption, load.wLineThick));
			}
		}

		void LoadRoundRectangle(ObjectCommonProperty ocp, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsRoundRectangle args = new ObjectArgsRoundRectangle();
				CommaTextReader comma = new CommaTextReader();

				comma.Set(load.sStringOption);
				comma.GetInt(ref args.round_x);
				comma.GetInt(ref args.round_y);

				parent.AddObject(new ObjectRoundRectangle(ocp, load.rRect, load.eID, load.objGeneral,
					load.lLineColor, load.lFillColor, load.wLineOption, load.wLineThick, args));
			}
		}

		void LoadBitmap(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command)
		{
			LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsBitmap args = new ObjectArgsBitmap();
				CommaTextReader comma = new CommaTextReader();

				args.sBitmapFile = load.sFileName;
				args.nOverlayMethod = load.nOverlayMethod;
                args.nRotateFlip = load.nRotateFlip;

				parent.AddObject(new ObjectBitmap(ocp, form, load.rRect, load.eID, load.objGeneral,
					args));
			}
		}

		void LoadMergedBitmap(ObjectCommonProperty ocp, TextReader reader, string command)
		{
			LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if (load.run(reader, command))
			{
				ObjectMergedBitmap obj = new ObjectMergedBitmap(ocp, load.rRect, load.eID, load.objGeneral,
					null, 0, 0, 0);
				obj.LoadMergedData(reader, command);
				parent.AddObject(obj);
			}
		}

		void LoadButtonModule3D(ObjectCommonProperty ocp, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command))
			{
				ObjectArgsButtonPublic args = new ObjectArgsButtonPublic();
				CommaTextReader comma = new CommaTextReader();

				args.sText = load.sString;
				args.tcolor = load.lTextColor;
				args.bcolor = load.lBackColor;
                args.lcolor = load.lLineColor;
                if (load.bLineThickLoaded) args.thick = load.wLineThick;
				args.designType = (ButtonDesignType)load.nButtonDesignType;
				args.radius = load.nButtonRadius;

				parent.AddObject(new ObjectButtonModule3D(ocp, load.rRect, load.eID, load.fontStruct, load.objGeneral,
					args, load.sFileName));
			}
		}

		void LoadButtonModuleHide(ObjectCommonProperty ocp, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsButtonPublic args = new ObjectArgsButtonPublic();
				CommaTextReader comma = new CommaTextReader();

				args.sText = load.sString;
				args.tcolor = load.lTextColor;
				args.bcolor = load.lBackColor;

				parent.AddObject(new ObjectButtonModuleHide(ocp, load.rRect, load.eID, load.objGeneral, load.sFileName));
			}
		}

		void LoadButtonProgramm(ObjectCommonProperty ocp, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command))
			{
				ObjectArgsButtonPublic args = new ObjectArgsButtonPublic();
				CommaTextReader comma = new CommaTextReader();
				int script_type = 0;

				args.sText = load.sString;
				args.tcolor = load.lTextColor;
				args.bcolor = load.lBackColor;
                args.lcolor = load.lLineColor;
                if (load.bLineThickLoaded) args.thick = load.wLineThick;
				args.designType = (ButtonDesignType)load.nButtonDesignType;
				args.radius = load.nButtonRadius;

				comma.Set(load.sStringOption);
				comma.GetInt(ref script_type);

				parent.AddObject(new ObjectButtonProgram(ocp, load.rRect, load.eID, load.fontStruct, load.objGeneral,
					args, script_type, load.sFileName, load.scriptLocal));
			}
		}

		void LoadButtonDigitalOut(ObjectCommonProperty ocp, TextReader reader, string command)
		{
            LoadObjectFromButtonDigitalOut load = new LoadObjectFromButtonDigitalOut(ocp);

			if(load.run(reader, command))
			{
				ObjectArgsButtonPublic args = new ObjectArgsButtonPublic();
				CommaTextReader comma = new CommaTextReader();

				args.sText = load.sString;
				args.tcolor = load.lTextColor;
				args.bcolor = load.lBackColor;
                args.lcolor = load.lLineColor;
                if (load.bLineThickLoaded) args.thick = load.wLineThick;
				args.designType = (ButtonDesignType)load.nButtonDesignType;
				args.radius = load.nButtonRadius;

				parent.AddObject(new ObjectButtonDigitalOut(ocp, load.rRect, load.eID, load.fontStruct, load.objGeneral,
					args,
					load.mouseResponse.do_method,
					load.blockButtonDoutMember,
					load.mouseResponse.do_delaytime));
			}
		}

		void LoadAnimation(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsAnimation args = new ObjectArgsAnimation();
				CommaTextReader comma = new CommaTextReader();

				args.sAnimationFile = load.sFileName;
				args.nOverlayMethod = load.nOverlayMethod;
                args.nRotateFlip = load.nRotateFlip;

				parent.AddObject(new ObjectAnimation(ocp, form, load.rRect, load.eID, load.objGeneral,
					args));
			}
		}

		void LoadAnalogString(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsAnalogString args = new ObjectArgsAnalogString();

				args.nBoxUse = load.nBackBox;
				args.nDisplayValue = load.nLocalMethod;
				args.colorBack = load.lBackColor;
				args.colorText = load.lTextColor;

                CommaTextReader comma = new CommaTextReader();
				comma.Set(load.sStringOption);
				comma.GetString(ref args.sDisplayFormat);
                args.bDisplayUnit = (comma.GetInt() == 1);
                if (!comma.IsEOS())
                {
                    args.nHorzAlign = comma.GetInt();
                }
				
				parent.AddObject(new ObjectAnalogString(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					load.sTagName, load.mouseResponse, null, args));
			}
		}

		void LoadAnalogMeter(ObjectCommonProperty ocp, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsAnalogMeter args = new ObjectArgsAnalogMeter();

				args.colorBack = load.lBackColor;
				
				args.colorGuide = load.lGuideLineColor;
				args.colorHand = load.lLineColor;
				args.colorText = load.lTextColor;
				args.thickHand = load.wLineThick;
				args.colorBorder = load.lFillColor;

				parent.AddObject(new ObjectAnalogMeter(ocp, load.rRect, load.eID, load.objGeneral, load.fontStruct, load.sTagName, 
					load.mouseResponse, null, args));
			}
		}

		void LoadAnalogRectangle(ObjectCommonProperty ocp, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsAnalogRectangle args = new ObjectArgsAnalogRectangle();
				CommaTextReader comma = new CommaTextReader();
				VIEW_RANGE_STRUCT view = new VIEW_RANGE_STRUCT();
				GUIDE_LINE_STRUCT guide = new GUIDE_LINE_STRUCT();
				int r=0, g=0, b=0, a=0;
                
				comma.Set(load.sStringOption);
				
				comma.GetInt(ref args.nBarDir);

				comma.GetChar(ref view.flag);
				comma.GetDouble(ref view.fBase);
				comma.GetDouble(ref view.fFull);

				comma.GetChar(ref guide.method);
				comma.GetInt(ref guide.devideBig);
				comma.GetInt(ref r);
				comma.GetInt(ref g);
				comma.GetInt(ref b);
				comma.GetInt(ref a);
				guide.colorBig = Color.FromArgb(a, r, g, b);
				comma.GetInt(ref guide.devideSmall);
				comma.GetInt(ref r);
				comma.GetInt(ref g);
				comma.GetInt(ref b);
				comma.GetInt(ref a);
				guide.colorSmall = Color.FromArgb(a, r, g, b);

				comma.GetInt(ref guide.line_length);
				comma.GetChar(ref guide.bLevelString);
                comma.GetInt(ref args.cornerRadius); // 둥근 모서리 반경 저장 20250204 PSU 추가

				args.colorOn = load.lFillColor;
				args.colorOff = load.lBackColor;

				parent.AddObject(new ObjectAnalogRectangle(ocp, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					load.sTagName, load.mouseResponse, null, args, view, guide));
			}
		}

		void LoadAnalogRotate(ObjectCommonProperty ocp, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsAnalogRotate args = new ObjectArgsAnalogRotate();

				args.bAngleDirection = (sbyte)load.nAngleDirection;
				args.lBackColor = load.lBackColor;
				args.nEndAngle = (int)load.fEndAngle;
				args.nMethod = load.nLocalMethod;
				args.nStartAngle = (int)load.fStartAngle;
				args.sFileName = load.sFileName;
				
				parent.AddObject(new ObjectAnalogRotate(ocp, load.rRect, load.eID, load.objGeneral, 
					load.sTagName, load.mouseResponse, null, args));
			}
		}

		void LoadAnalogStatus(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command))
			{
				ObjectArgsAnalogStatus args = new ObjectArgsAnalogStatus();

				parent.AddObject(new ObjectAnalogStatus(ocp, form, load.rRect, load.eID, load.objGeneral, 
					load.sTagName, load.mouseResponse, null, args, load.blockAnalogStatus));
			}
		}
        //아날로그 게이지 hsjeong 25-02-04
        void LoadAnalogGauge(ObjectCommonProperty ocp, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

            if (load.run(reader, command))
            {
                ObjectArgsAnalogGauge args = new ObjectArgsAnalogGauge();
                CommaTextReader comma = new CommaTextReader();
                VIEW_RANGE_STRUCT2 view = new VIEW_RANGE_STRUCT2();
                GUIDE_LINE_STRUCT2 guide = new GUIDE_LINE_STRUCT2();
                ANALOG_GAGUE_POINT_MEMBER point = new ANALOG_GAGUE_POINT_MEMBER();
                int r = 0, g = 0, b = 0, a = 0;

                comma.Set(load.sStringOption);

                comma.GetFloat(ref args.fStartAngle);
                comma.GetFloat(ref args.fSweepAngle);

                comma.GetInt(ref args.nBarDir);
                comma.GetInt(ref args.nLineThick);

                comma.GetChar(ref view.flag);
                comma.GetDouble(ref view.fBase);
                comma.GetDouble(ref view.fFull);

                comma.GetChar(ref guide.method);
                comma.GetInt(ref guide.devideBig);
                comma.GetInt(ref r);
                comma.GetInt(ref g);
                comma.GetInt(ref b);
                comma.GetInt(ref a);
                guide.colorBig = Color.FromArgb(a, r, g, b);
                comma.GetInt(ref guide.devideSmall);
                comma.GetInt(ref r);
                comma.GetInt(ref g);
                comma.GetInt(ref b);
                comma.GetInt(ref a);
                guide.colorSmall = Color.FromArgb(a, r, g, b);

                comma.GetInt(ref guide.nSpaceGuideLine);
                comma.GetInt(ref guide.line_length);
                comma.GetChar(ref guide.bLevelString);
                comma.GetInt(ref guide.nSpaceLevelString);

                args.colorOn = load.lFillColor;
                args.colorOff = load.lBackColor;

                comma.GetInt(ref point.nPointType);
                comma.GetInt(ref r);
                comma.GetInt(ref g);
                comma.GetInt(ref b);
                comma.GetInt(ref a);
                point.color = Color.FromArgb(a, r, g, b);

                comma.GetInt(ref point.nPointSize);


                parent.AddObject(new ObjectAnalogGauge(ocp, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    load.sTagName, load.mouseResponse, null, args, view, guide, point));
            }
        }

		void LoadDigitalAnimation(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsDigitalAnimation args = new ObjectArgsDigitalAnimation();

				args.sFileOff = load.sFileNameOff;
				args.sFileOn = load.sFileNameOn;
				args.nOverlayMethod = load.nOverlayMethod;
                args.nRotateFlip = load.nRotateFlip;
				
				parent.AddObject(new ObjectDigitalAnimation(ocp, form, load.rRect, load.eID, load.objGeneral, 
					load.sTagName, load.mouseResponse, null, args));
			}
		}

		void LoadDigitalCircle(ObjectCommonProperty ocp, TextReader reader, string command, CommaTextReader comma)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsDigitalCircle args = new ObjectArgsDigitalCircle();

				args.colorOff = load.lOffColor;
				args.colorOn = load.lOnColor;
				
				parent.AddObject(new ObjectDigitalCircle(ocp, load.rRect, load.eID, load.objGeneral, 
					load.sTagName, load.mouseResponse, null, args));
			}
		}

		void LoadDigitalRectangle(ObjectCommonProperty ocp, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsDigitalRectangle args = new ObjectArgsDigitalRectangle();

				args.colorOff = load.lOffColor;
				args.colorOn = load.lOnColor;

				CommaTextReader comma = new CommaTextReader();
				comma.Set(load.sStringOption);
				comma.GetInt(ref args.nDisplayMethod);
				
				parent.AddObject(new ObjectDigitalRectangle(ocp, load.rRect, load.eID, load.objGeneral, 
					load.sTagName, load.mouseResponse, null, args));
			}
		}

		void LoadDigitalString(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsDigitalString args = new ObjectArgsDigitalString();

				args.colorBack = load.lBackColor;
				args.colorOff = load.lOffColor;
				args.colorOn = load.lOnColor;
				
				parent.AddObject(new ObjectDigitalString(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					load.sTagName, load.mouseResponse, null, args));
			}
		}

		void LoadStringString(ObjectCommonProperty ocp, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsStringString args = new ObjectArgsStringString();

				args.nBoxUse = load.nBackBox;
				args.colorBack = load.lBackColor;
				args.colorText = load.lTextColor;
				
				parent.AddObject(new ObjectStringString(ocp, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					load.sTagName, load.mouseResponse, null, args, load.align));
			}
		}

		void LoadMultiTrend(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
            LoadObjectFromMultiTrend load = new LoadObjectFromMultiTrend(ocp);

			if(load.run(reader, command)) 
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

				if(load.objGeneral.sClassName.Length == 0)
					load.objGeneral.sClassName = load.sFileName;

                args.scriptEventAfterSettings = load.scriptEventAfterSettings;

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);
                comma.GetBool(ref args.bUseMouseButtonAsZoom);
                comma.GetBool(ref args.bUseMouseButtonAsZoomWithY);
                comma.GetBool(ref args.bDontUseConfigDialog);
                args.nDataCycle = comma.GetInt();  // 2017-8-23 추가함
                if (args.nDataCycle < 1) args.nDataCycle = 1;
                comma.GetBool(ref args.bDontUseConfigAutoRange);
                comma.GetBool(ref args.bUseToolBar);  //20250306 PSU 추가.
                comma.GetInt(ref args.nToolBarPos);
                comma.GetInt(ref args.nTooolBarBtnColor);
                comma.GetBool(ref args.bHideLabelDataRange);

                int buttonsize = 16;
                comma.GetInt(ref buttonsize);
                if (buttonsize < 16) args.nToolBarButtonSize = 16;
                else args.nToolBarButtonSize = buttonsize;

                int textsize = 9;
                comma.GetInt(ref textsize);
                if (textsize < 1) args.nToolBarTextSize = 9;
                else args.nToolBarTextSize = textsize;

                args.logarithmicScale = load.logarithmicScale;
                				
				parent.AddObject(new ObjectMultiTrend(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args, load.graphMember));
			}
		}

		void LoadDatabaseTrend(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
            if (!CheckNotSupportedObject(ocp, reader, command, EnumOemTypeRight.Database))
                return;

            LoadObjectFromDatabaseTrend load = new LoadObjectFromDatabaseTrend(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsDatabaseTrend args = new ObjectArgsDatabaseTrend();

				CommaTextReader comma = new CommaTextReader();
				comma.Set(load.sStringOption);

				comma.GetString(ref args.sDsn);
				comma.GetString(ref args.sTable);
				comma.GetString(ref args.sColumnTime);
                args.nDataCycle = comma.GetInt();
				comma.GetString(ref args.sColumnMilli);
				comma.GetInt(ref args.nDateColumnType);
				comma.GetInt(ref args.nBasicSpaceLeft);
				comma.GetInt(ref args.nBasicSpaceRight);
                comma.GetBool(ref args.bDontUseConfigDialog);  //20250306 PSU 추가 .
                comma.GetBool(ref args.bUseToolBar);
                comma.GetInt(ref args.nToolBarPos);
                comma.GetInt(ref args.nTooolBarBtnColor);
                comma.GetBool(ref args.bHideLabelDataRange);

                int buttonsize = 16;  //20250317 PSU 추가.
                comma.GetInt(ref buttonsize);
                if (buttonsize < 16) args.nToolBarButtonSize = 16;
                else args.nToolBarButtonSize = buttonsize;

                int textsize = 9;
                comma.GetInt(ref textsize);
                if (textsize < 1) args.nToolBarTextSize = 9;
                else args.nToolBarTextSize = textsize;


				args.lColorBack = load.lBackColor;
				args.lColorGuideLine = load.lGuideLineColor;
                args.lColorCursor = load.lCursorColor;
				args.lColorText = load.lTextColor;
				args.lColorFill = load.lFillColor;
				//args.wDisplayFlags = (EnumDisplayFlag)load.wFlags;
				args.wLevelDevide = load.wLevelDevide;
				//args.wPointSize = load.wGraphPointSize;
				args.wShowUnit = load.nShowUnit;
				args.wTimeDevide = load.wTimeDevide;
				args.wTimeSelectOption = load.wTimeSelectOption;
				//args.nLevelDisplaySize = load.nLevelDisplaySize;
                args.pub = load.argsGraphPublic;

				if(load.objGeneral.sClassName.Length == 0)
					load.objGeneral.sClassName = load.sFileName;

                args.logarithmicScale = load.logarithmicScale;
				
				parent.AddObject(new ObjectDatabaseTrend(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args, load.dbTrendMember));
			}
		}


        void LoadMilliDataTrend(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            if (!CheckNotSupportedObject(ocp, reader, command, EnumOemTypeRight.MilliData))
                return;

            LoadObjectFromMilliDataTrend load = new LoadObjectFromMilliDataTrend(ocp);

            if (load.run(reader, command))
            {
                ObjectArgsMilliDataTrend args = new ObjectArgsMilliDataTrend();

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);

                comma.GetString(ref args.sDsn);
                comma.Skip();// comma.GetString(ref args.sTable);
                comma.Skip();// comma.GetString(ref args.sColumnTime);
                args.nDataCycle = comma.GetInt();
                comma.Skip();// comma.GetString(ref args.sColumnMilli);
                comma.Skip();// comma.GetInt(ref args.nDateColumnType);
                comma.GetInt(ref args.nBasicSpaceLeft);
                comma.GetInt(ref args.nBasicSpaceRight);
                comma.GetBool(ref args.bAutoUpdate);
                comma.GetBool(ref args.bDontUseConfigDialog); //20250306 PSU 추가.
                comma.GetBool(ref args.bUseToolBar);  
                comma.GetInt(ref args.nToolBarPos);
                comma.GetInt(ref args.nTooolBarBtnColor);
                comma.GetBool(ref args.bHideLabelDataRange);

                int buttonsize = 16;
                comma.GetInt(ref buttonsize);
                if (buttonsize < 16) args.nToolBarButtonSize = 16;
                else args.nToolBarButtonSize = buttonsize;

                int textsize = 9;
                comma.GetInt(ref textsize);
                if (textsize < 1) args.nToolBarTextSize = 9;
                else args.nToolBarTextSize = textsize;

                args.lColorBack = load.lBackColor;
                args.lColorGuideLine = load.lGuideLineColor;
                args.lColorText = load.lTextColor;
                args.lColorFill = load.lFillColor;
                //args.wDisplayFlags = (EnumDisplayFlag)load.wFlags;
                args.wLevelDevide = load.wLevelDevide;
                //args.wPointSize = load.wGraphPointSize;
                args.wShowUnit = load.nShowUnit;
                args.wTimeDevide = load.wTimeDevide;
                args.wTimeSelectOption = load.wTimeSelectOption;
                //args.nLevelDisplaySize = load.nLevelDisplaySize;
                args.pub = load.argsGraphPublic;

                if (load.objGeneral.sClassName.Length == 0)
                    load.objGeneral.sClassName = load.sFileName;

                args.logarithmicScale = load.logarithmicScale;


                parent.AddObject(new ObjectMilliDataTrend(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args, load.mdTrendMember));
            }
        }

		void LoadMultiGraph(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
            LoadObjectFromMultiGraph load = new LoadObjectFromMultiGraph(ocp);

			if(load.run(reader, command)) 
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

				if(load.objGeneral.sClassName.Length == 0)
					load.objGeneral.sClassName = load.sFileName;

				CommaTextReader comma = new CommaTextReader();
				comma.Set(load.sStringOption);
				args.nDataTime = comma.GetInt();
				comma.GetChar(ref args.bTimeDirToLeft);
				comma.GetChar(ref args.bDisplayByTime);

                args.logarithmicScale = load.logarithmicScale;
				
				parent.AddObject(new ObjectMultiGraph(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args, load.graphMember));
			}
		}

        //=== Chart 컨트롤 기반 오브젝트 로드 (25-02-24) ===

 
        void LoadCustomChart(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            LoadObjectFromChart load = new LoadObjectFromChart(ocp);

            if (load.run(reader, command))
            {
                ObjectArgsChartCustom args = new ObjectArgsChartCustom();

                args.lColorBack = load.lBackColor;
                args.lColorGuideLine = load.lGuideLineColor;
                args.lColorText = load.lTextColor;
                args.lColorFill = load.lFillColor;
                args.pub = load.argsGraphPublic;
                args.wLevelDevide = load.wLevelDevide;
                args.wShowUnit = load.nShowUnit;
                args.wTimeDivide = load.wTimeDevide;
                args.chart = load.chartCommon;
                args.logarithmicScale = load.logarithmicScale;

                if (load.objGeneral.sClassName.Length == 0)
                    load.objGeneral.sClassName = load.sFileName;

                args.chart.scriptEventAfterSettings = load.scriptEventAfterSettings;

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);
                args.bSupportPie = comma.GetBool();
                args.bSupportDoughnut = comma.GetBool();
                args.bSupportRadar = comma.GetBool();

                args.nDataTime = load.nDataTimeCustom;

                parent.AddObject(new CustomChart(ocp, form, load.rRect, load.eID,
                    load.objGeneral, load.fontStruct, args, load.chartMember, load.customPoints));
            }
        }

		void LoadSingleText(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsSingleText args = new ObjectArgsSingleText();

				args.text = load.sString;
				args.textColor = load.lTextColor;

				parent.AddObject(new ObjectSingleText(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args));
			}
		}

		void LoadText(ObjectCommonProperty ocp, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);
            load.bMultiString = true;   // 여러줄 텍스트를 사용한다.

            load.align.x = 0;   // 다중라인을 지원하면서 정렬 정보가 없는 이전 버전은 왼쪽/위를 Default로 정렬한다.
            load.align.y = 0;

			if(load.run(reader, command)) 
			{
				ObjectArgsText args = new ObjectArgsText();

				args.text = load.sString;
				args.textColor = load.lTextColor;
                args.align = load.align;

                CommaTextReader comma = new CommaTextReader();
				comma.Set(load.sStringOption);
                comma.GetBool(ref args.formatFlagDirectionVertical);
                comma.GetBool(ref args.formatFlagNoWrap);
                
				parent.AddObject(new ObjectText(ocp, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args));
			}
		}

		void LoadDate(ObjectCommonProperty ocp, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsDate args = new ObjectArgsDate();

				args.textColor = load.lTextColor;
				args.backColor = load.lBackColor;

				CommaTextReader comma = new CommaTextReader();
				comma.Set(load.sStringOption);
				comma.GetInt(ref args.type);

				parent.AddObject(new ObjectDate(ocp, load.rRect, load.eID, load.objGeneral, load.fontStruct, args));
			}
		}

		void LoadChangeValueDisplay(ObjectCommonProperty ocp, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsChangeValueDisplay args = new ObjectArgsChangeValueDisplay();

				args.textColor = load.lTextColor;
				args.backColor = load.lBackColor;

				CommaTextReader comma = new CommaTextReader();
				comma.Set(load.sStringOption);
				comma.GetInt(ref args.nListCount);

				parent.AddObject(new ObjectChangeValueDisplay(ocp, load.rRect, load.eID, load.objGeneral, load.fontStruct, args));
			}
		}

		void LoadClock(ObjectCommonProperty ocp, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsClock args = new ObjectArgsClock();

				args.textColor = load.lTextColor;
				args.backColor = load.lBackColor;

				CommaTextReader comma = new CommaTextReader();
				comma.Set(load.sStringOption);
				comma.GetInt(ref args.type);

				parent.AddObject(new ObjectClock(ocp, load.rRect, load.eID, load.objGeneral, load.fontStruct, args));
			}
		}

		void LoadObjectControlCheckBox(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsControlCheckBox args = new ObjectArgsControlCheckBox();

				args.sTag = load.sTagName;
				args.rgbColor = load.lTextColor;

				CommaTextReader comma = new CommaTextReader();
				comma.Set(load.sStringOption);
				comma.GetString(ref args.sTitle);

				parent.AddObject(new ObjectControlCheckBox(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args));
			}
		}

		void LoadObjectControlComboBox(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
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

				parent.AddObject(new ObjectControlComboBox(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args));
			}
		}

		void LoadObjectControlEditBox(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsControlEditBox args = new ObjectArgsControlEditBox();

				args.dwWindowStyle = load.dwWindowStyle;
				args.sTag = load.sTagName;
                args.textColor = load.lTextColor;
                args.backColor = load.lBackColor;

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);
                comma.GetInt(ref args.nHorzAlign);  // 10.2.4.3 부터 지원

				parent.AddObject(new ObjectControlEditBox(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args));
			}
		}

		void LoadObjectControlListBox(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
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

				parent.AddObject(new ObjectControlListBox(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args));
			}
		}

		void LoadObjectControlRadioButton(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command))
			{
				ObjectArgsControlRadioButton args = new ObjectArgsControlRadioButton();

				args.sTag = load.sTagName;
				args.rgbColor = load.lTextColor;
				args.arrayListData = load.blockListData;

				CommaTextReader comma = new CommaTextReader();
				comma.Set(load.sStringOption);

				parent.AddObject(new ObjectControlRadioButton(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct, args));
			}
		}

        void LoadObjectControlDatePicker(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

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

                parent.AddObject(new ObjectControlDatePicker(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }

        void LoadObjectControlTabControl(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

            if (load.run(reader, command))
            {
                ObjectArgsControlTabControl args = new ObjectArgsControlTabControl();

                args.dwWindowStyle = load.dwWindowStyle;
                //args.sTag = load.sTagName;
                args.textColor = load.lTextColor;
                args.backColor = load.lBackColor;

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);
                comma.GetString(ref args.sFormat);

                parent.AddObject(new ObjectControlTabControl(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }

        void LoadObjectControlTreeView(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            LoadObjectFromTreeView load = new LoadObjectFromTreeView(ocp);

            if (load.run(reader, command))
            {
                ObjectArgsControlTreeView args = new ObjectArgsControlTreeView();

                args.dwWindowStyle = load.dwWindowStyle;
                //args.sTag = load.sTagName;
                args.textColor = load.lTextColor;
                args.backColor = load.lBackColor;

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);
                comma.GetString(ref args.sFormat);

                args.scriptEventDoubleClick = load.scriptEventDoubleClick;

                parent.AddObject(new ObjectControlTreeView(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }

		void LoadObjectModule(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsModule args = new ObjectArgsModule();

				args.filename = load.sFileName;

				parent.AddObject(new ObjectModule(ocp, form, load.rRect, load.eID, load.objGeneral, 
					args));
			}
		}

		void LoadUnknownObject(ObjectCommonProperty ocp, TextReader reader, string command, CommaTextReader comma)
		{
			string begin="";

			comma.GetString(ref begin);

			if(String.Compare(begin, "BEGIN", true) == 0)  
			{
                LoadObjectFromModX load = new LoadObjectFromModX(ocp);

				if(load.run(reader, command)) 
				{
					parent.AddObject(new ObjectUnknown(ocp, command, load.rRect, load.eID, load.objGeneral));
				}
			}
			else 
			{
				
			}
		}

        // 24-12-15 추가. SCADA-Lite가 추가되면서 지원하지 않는 오브젝트는 Not Supported 로 표시한다.
        bool CheckNotSupportedObject(ObjectCommonProperty ocp, TextReader reader, string command, EnumOemTypeRight oemr)
        {
            if (TotalConfig.defineMode != EnumDefineMode.MODE_RUN)
                return true;

            if (TotalConfig.GetOemTypeRight(oemr))
                return true;

            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

            if (load.run(reader, command))
            {
                ObjectUnknown obj = new ObjectUnknown(ocp, command, load.rRect, load.eID, load.objGeneral);

                obj.SetNotSupported();
                parent.AddObject(obj);
            }

            return false;
        }

		void LoadObjectDatabase(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
            if (!CheckNotSupportedObject(ocp, reader, command, EnumOemTypeRight.Database))
                return;

            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
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
                comma.GetBool(ref args.bReverseNo);
				comma.GetString(ref args.sSqlTextWhere);
				comma.GetString(ref args.sSqlTextOrderBy);
				comma.GetInt(ref args.nRecordLimit);
				comma.GetBool(ref args.bUseAlternateRowColor);
				comma.GetBool(ref args.bUsePagination);
				comma.GetInt(ref args.nPageSize);

				parent.AddObject(new ObjectDatabase(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args));
			}
		}

		void LoadObjectWindowAlarm(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
            LoadObjectFromWindowAlarm load = new LoadObjectFromWindowAlarm(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsWindowAlarm args = new ObjectArgsWindowAlarm();

                CommaTextReader comma = new CommaTextReader();

				comma.Set(load.sStringOption);
				comma.GetInt(ref args.cIncludeMethod);
				comma.GetChar(ref args.bFlagWindowCaption);
				comma.GetChar(ref args.bFlagUseColumnHeader);
                if (!comma.IsEOS())
                {
                    comma.GetBool(ref args.bFlagUseScrollHorz);
                    comma.GetBool(ref args.bFlagUseScrollVert);
                }

                args.arrayColumns = load.arrayColumns;

				parent.AddObject(new ObjectWindowAlarm(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args));
			}
		}

        void LoadObjectDemandWindow(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

            if (load.run(reader, command))
            {
                ObjectArgsDemandWindow args = new ObjectArgsDemandWindow();

                args.lColorFill = load.lFillColor;   //20250227 PSU 추가
                args.lColorBack = load.lBackColor;
                args.lColorText = load.lTextColor;
                args.lColorGuideLine = load.lGuideLineColor;

                CommaTextReader comma = new CommaTextReader();

                comma.Set(load.sStringOption);
                comma.GetInt(ref args.cIncludeMethod);
                comma.GetInt(ref args.bFlagWindowCaption);
                comma.GetString(ref args.demand_name);
                comma.GetInt(ref args.thick_target);

                comma.GetInt(ref args.nStatusBarPos); //20250227 PSU 추가

                // 색상 설정을 위한 헬퍼 메소드를 사용하여 코드 간소화 20250227 PSU 추가
                SetColorWithDefault(comma, ref args.IColorPredictionPower, Color.Blue);
                SetColorWithDefault(comma, ref args.IColorExcessedPower, Color.Red);
                SetColorWithDefault(comma, ref args.IColorTargetPower, Color.LightGreen);
                SetColorWithDefault(comma, ref args.IColorStatusBack, Color.Black);
                SetColorWithDefault(comma, ref args.IColorStatusFill, Color.LightGray);
                SetColorWithDefault(comma, ref args.IColorStatusValue, Color.Yellow);
                SetColorWithDefault(comma, ref args.IColorTargetValue, Color.LightGreen);
                SetColorWithDefault(comma, ref args.IColorPreValue, Color.FromArgb(0, 255, 255));
                SetColorWithDefault(comma, ref args.IColorExValue, Color.Red);

                parent.AddObject(new ObjectDemandWindow(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }

        // 색상 설정을 위한 헬퍼 메소드 20250227 PSU 추가
        void SetColorWithDefault(CommaTextReader reader, ref Color colorProperty, Color defaultColor)
        {
            int r = 0, g = 0, b = 0, a = 0;
            reader.GetInt(ref r);
            reader.GetInt(ref g);
            reader.GetInt(ref b);
            reader.GetInt(ref a);

            colorProperty = (r == 0 && g == 0 && b == 0 && a == 0)
                ? defaultColor
                : Color.FromArgb(a, r, g, b);
        }

		void LoadObjectMilliData(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
            if (!CheckNotSupportedObject(ocp, reader, command, EnumOemTypeRight.MilliData))
                return;

            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsMilliData args = new ObjectArgsMilliData();

                CommaTextReader comma = new CommaTextReader();

				comma.Set(load.sStringOption);
				comma.GetString(ref args.sTitle);

				parent.AddObject(new ObjectMilliData(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args));
			}
		}

		void LoadObjectRealTimeTestGraph(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsRealTimeTestGraph args = new ObjectArgsRealTimeTestGraph();

				args.lColorBack = load.lBackColor;
				args.lColorFill = load.lFillColor;
				args.lColorGuideLine = load.lGuideLineColor;
				args.lColorText = load.lTextColor;
				//args.pub.wPointSize = load.wGraphPointSize;
				args.wShowUnit = load.nShowUnit;
				//args.pub.wDisplayFlags = (EnumDisplayFlag)load.wFlags;
				//args.pub.nLevelDisplaySize = load.nLevelDisplaySize;
                args.pub = load.argsGraphPublic;
				args.wLevelDevide = load.wLevelDevide;
				args.wTimeDevide = load.wTimeDevide;
				//args.wTimeSelectOption = load.wTimeSelectOption;

                CommaTextReader comma = new CommaTextReader();
				comma.Set(load.sStringOption);
				comma.GetInt(ref args.nDataTime);
				comma.GetString(ref args.sTagStart);
				
				//comma.GetColor(ref args.basicLineColor);	// ToArgb로 저장된것은 C++RGB방식과는 틀리다.
				int color = 0;
				comma.GetInt(ref color);
				Color basicLineColor = Color.FromArgb(color);
                int basicLineThick = 1, basicPointType = 0;

				comma.GetInt(ref basicLineThick);
				comma.GetInt(ref basicPointType);
				comma.GetInt(ref args.nTimeDisplayType);
				comma.GetChar(ref args.bDisableCursor);
				comma.GetString(ref args.sTagRun);
                int flag = 0;
                comma.GetInt(ref flag);
                args.bUseSavedData = (flag == 1);
                comma.GetString(ref args.sSavedData);
                args.bGraphDisplayWhileRunning = comma.GetBool();

				if(load.objGeneral.sClassName.Length == 0)
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
            LoadObjectFromXYGraph load = new LoadObjectFromXYGraph(ocp);

			if(load.run(reader, command)) 
			{
				ObjectArgsXYGraph args = new ObjectArgsXYGraph();

				args.bcolor = load.lBackColor;
				args.lColorFill = load.lFillColor;
				args.gcolor = load.lGuideLineColor;
				args.tcolor = load.lTextColor;
				args.point_size = load.argsGraphPublic.wPointSize;
				args.showunit = load.nShowUnit;
				args.wDisplayFlags = (EnumDisplayFlag)load.argsGraphPublic.wDisplayFlags;
				args.leveldevide = load.wLevelDevide;
				args.wTimeDevide = load.wTimeDevide;

                CommaTextReader comma = new CommaTextReader();
				comma.Set(load.sStringOption);
				args.nDataTime = comma.GetInt();

				if(load.objGeneral.sClassName.Length == 0)
					load.objGeneral.sClassName = load.sFileName;

				parent.AddObject(new ObjectXYGraph(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct, args, load.graphMemberXY));
			}
		}

        void LoadObjectWebBrowser(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

            if (load.run(reader, command))
            {
                ObjectArgsWebBrowser args = new ObjectArgsWebBrowser();

                CommaTextReader comma = new CommaTextReader();
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
        }

        //Webview 20240627 PSU
        void LoadObjectWebView(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

            if (load.run(reader, command))
            {
                ObjectArgsWebView args = new ObjectArgsWebView();

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);
                comma.GetString(ref args.url);

                parent.AddObject(new ObjectWebView(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct, args));
            }
        }


        void LoadObjectTagAnimation(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            LoadObjectFromTagAnimation load = new LoadObjectFromTagAnimation(ocp);

            if (load.run(reader, command))
            {
                ObjectArgsTagAnimation args = new ObjectArgsTagAnimation();

                args.nOverlayMethod = load.nOverlayMethod;
                args.nRotateFlip = load.nRotateFlip;
                args.member = load.blockTagAnimation;

                parent.AddObject(new ObjectTagAnimation(ocp, form, load.rRect, load.eID, load.objGeneral, load.sTagName, load.mouseResponse, args));
            }
        }

        void LoadObjectDataGridView(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            if (!CheckNotSupportedObject(ocp, reader, command, EnumOemTypeRight.Database))
                return;

            LoadObjectFromDataGrid load = new LoadObjectFromDataGrid(ocp);

            if (load.run(reader, command))
            {
                ObjectArgsDataGridView args = new ObjectArgsDataGridView();

                args.lColorBack = load.lBackColor;
                args.lColorText = load.lTextColor;

                CommaTextReader comma = new CommaTextReader();

                comma.Set(load.sStringOption);
                comma.GetString(ref args.dsn);
                comma.GetBool(ref args.bAllowUserToAddRows);
                comma.GetBool(ref args.bAllowUserToDeleteRows);

                // 새로 추가된 속성들 로드 20250304 PSU 추가
                comma.GetBool(ref args.bHideBorder);
                comma.GetBool(ref args.bHideRowHeaders);
                comma.GetBool(ref args.bHideGridLines);
                comma.GetBool(ref args.bUseAlternatingRowColors);

                // 색상 로드 시 0이면 기본값 유지
                int nAlternatingRowForeColor = 0;
                comma.GetInt(ref nAlternatingRowForeColor);
                if (nAlternatingRowForeColor != 0)
                    args.lAlternatingRowForeColor = Color.FromArgb(nAlternatingRowForeColor);

                int nAlternatingRowBackColor = 0;
                comma.GetInt(ref nAlternatingRowBackColor);
                if (nAlternatingRowBackColor != 0)
                    args.lAlternatingRowBackColor = Color.FromArgb(nAlternatingRowBackColor);

                int selBackColor = 0;
                comma.GetInt(ref selBackColor);
                if (selBackColor != 0)
                    args.lSelectionBackColor = Color.FromArgb(selBackColor);

                int selForeColor = 0;
                comma.GetInt(ref selForeColor);
                if (selForeColor != 0)
                    args.lSelectionForeColor = Color.FromArgb(selForeColor);

                int colHeadersForeColor = 0;
                comma.GetInt(ref colHeadersForeColor);
                if (colHeadersForeColor != 0)
                    args.lColumnHeadersForeColor = Color.FromArgb(colHeadersForeColor);

                int colHeadersBackColor = 0;
                comma.GetInt(ref colHeadersBackColor);
                if (colHeadersBackColor != 0)
                    args.lColumnHeadersDefaultBackColor = Color.FromArgb(colHeadersBackColor);

                int rowHeadersForeColor = 0;
                comma.GetInt(ref rowHeadersForeColor);
                if (rowHeadersForeColor != 0)
                    args.lRowHeadersDefaultForeColor = Color.FromArgb(rowHeadersForeColor);

                int rowHeadersBackColor = 0;
                comma.GetInt(ref rowHeadersBackColor);
                if (rowHeadersBackColor != 0)
                    args.lRowHeadersDefaultBackColor = Color.FromArgb(rowHeadersBackColor);

                int cellBackColor = 0;
                comma.GetInt(ref cellBackColor);
                if (cellBackColor != 0)
                    args.lCellBackColor = Color.FromArgb(cellBackColor);

                comma.GetInt(ref args.nAutoSizeRowsMode);
                int columnAutoSize = 0;
                comma.GetInt(ref columnAutoSize);
                if (columnAutoSize == 0) args.nAutoSizeColumnsMode = 1;
                else args.nAutoSizeColumnsMode = columnAutoSize;

                int columnHeaderFontSize = 10;
                comma.GetInt(ref columnHeaderFontSize);
                if (columnHeaderFontSize == 0) args.nColumnHeaderFontSize = 10;
                else args.nColumnHeaderFontSize = columnHeaderFontSize;

                args.scriptEventCellClick = load.scriptEventCellClick;
                args.scriptEventCellPainting = load.scriptEventCellPainting;
                args.scriptEventCellValueChanged = load.scriptEventCellValueChanged;


                parent.AddObject(new ObjectDataGridView(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct, args));
            }
        }

        //vlc 20240627 PSU
        void LoadObjectVLCAx(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

            if (load.run(reader, command))
            {
                ObjectArgsVLCAx args = new ObjectArgsVLCAx();

                CommaTextReader comma = new CommaTextReader();
                comma.Set(load.sStringOption);
                comma.GetString(ref args.mrl);
                comma.GetString(ref args.options);
                comma.GetBool(ref args.autoplay);
                comma.GetBool(ref args.autoloop);
                comma.GetInt(ref args.volume);

                parent.AddObject(new ObjectVLCAx(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }

        //20241024 PSU
        void LoadSVG(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

            if (load.run(reader, command))
            {
                ObjectArgsSVG args = new ObjectArgsSVG();
                CommaTextReader comma = new CommaTextReader();

                args.sSvgFile = load.sFileName;
                args.nOverlayMethod = load.nOverlayMethod;
                args.nRotateFlip = load.nRotateFlip;

                parent.AddObject(new ObjectSVG(ocp, form, load.rRect, load.eID, load.objGeneral,
                    args));
            }
        }

        //도넛 차트 hsjeong 25-02-04
        void LoadDonutChart(ObjectCommonProperty ocp, TextReader reader, string command)
        {
            LoadObjectFromDonutChart load = new LoadObjectFromDonutChart(ocp);

            if (load.run(reader, command))
            {
                ObjectArgsDonutChart args = new ObjectArgsDonutChart();
                CommaTextReader comma = new CommaTextReader();

                DONUT_CHART_TAG_MEMBER member = new DONUT_CHART_TAG_MEMBER();

                int r = 0, g = 0, b = 0, a = 0;

                comma.Set(load.sStringOption);

                comma.GetFloat(ref args.fStartAngle);
                comma.GetFloat(ref args.fSpaceAngle);

                comma.GetInt(ref args.nBarDir);
                comma.GetInt(ref args.nLineThick);

                comma.GetBYTE(ref args.bDisplayGuideLine);
                comma.GetInt(ref args.nDisplayGuideLineSize);
                comma.GetInt(ref args.nDisplayGuideLineSpace);

                comma.GetInt(ref args.nSpaceInside);
                comma.GetInt(ref args.nSpaceOutside);

                comma.GetBYTE(ref args.bDisplayTotalValue);
                comma.GetInt(ref args.nDisplayTotalSize);
                comma.GetString(ref args.sDisplayTotalFormat);

                comma.GetInt(ref a);
                comma.GetInt(ref r);
                comma.GetInt(ref g);
                comma.GetInt(ref b);
                args.color_total = Color.FromArgb(a, r, g, b);

                comma.GetInt(ref args.nTagNameSize);
                comma.GetInt(ref args.nTagNameOption);

                comma.GetInt(ref args.nValueSize);
                comma.GetInt(ref args.nValueOption);

                comma.GetInt(ref args.nPercentSize);
                comma.GetInt(ref args.nPercentOption);



                parent.AddObject(new ObjectDonutChart(ocp, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args, load.blockDonutChartMember));
            }
        }

        // DemandChart 로드
        void LoadDemandChart(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

            if (load.run(reader, command))
            {
                ObjectArgsDemandChart args = new ObjectArgsDemandChart();
                CommaTextReader comma = new CommaTextReader();
                int r = 0, g = 0, b = 0, a = 0;

                comma.Set(load.sStringOption);
                string version = "";
                comma.GetString(ref version);

                bool isV3 = (String.Compare(version, "v3", true) == 0);
                bool isV4 = (String.Compare(version, "v4", true) == 0);
                if (!isV3 && !isV4)
                {
                    Log.Write(LogLevel.WARNING, LogCategory.DATA_SAVE,
                        "DemandChart loader: unsupported StringOption format '{0}', fallback defaults applied.", version);

                    args.lColorBack = load.lBackColor;
                    args.lColorFill = load.lFillColor;
                    args.lColorText = load.lTextColor;
                    args.lColorGuideLine = load.lGuideLineColor;
                    parent.AddObject(new ObjectDemandChart(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct, args));
                    return;
                }

                // demand_block_id
                comma.GetString(ref args.demand_block_id);
                // thick_target
                comma.GetInt(ref args.thick_target);
                // nStatusBarPos
                comma.GetInt(ref args.nStatusBarPos);
                // nPercentY
                comma.GetInt(ref args.nPercentY);
                comma.GetInt(ref args.uiTheme);

                int bval = 0;
                comma.GetInt(ref bval); args.showAreaFill = (bval == 1);
                comma.GetInt(ref bval); args.showPeakLine = (bval == 1);
                comma.GetInt(ref bval); args.showForecastBand = (bval == 1);
                comma.GetInt(ref bval); args.showGridLabels = (bval == 1);
                if (!comma.IsEOS()) { comma.GetInt(ref bval); args.showStepLines = (bval == 1); }
                if (!comma.IsEOS()) { comma.GetInt(ref bval); args.showTimeZoneTarget = (bval == 1); }
                if (!comma.IsEOS()) { comma.GetInt(ref bval); args.showDetailedAxis = (bval == 1); }
                comma.GetInt(ref args.gridIntensity);
                comma.GetInt(ref args.lineThicknessTrend);
                comma.GetInt(ref args.lineThicknessForecast);
                comma.GetInt(ref args.kpiDensity);
                comma.GetInt(ref args.cornerRadius);
                comma.GetInt(ref args.kpiOpacity);

                // 9개 RGBA 색상
                comma.GetInt(ref r); comma.GetInt(ref g); comma.GetInt(ref b); comma.GetInt(ref a);
                args.lColorPrediction = Color.FromArgb(a, r, g, b);

                comma.GetInt(ref r); comma.GetInt(ref g); comma.GetInt(ref b); comma.GetInt(ref a);
                args.lColorExcess = Color.FromArgb(a, r, g, b);

                comma.GetInt(ref r); comma.GetInt(ref g); comma.GetInt(ref b); comma.GetInt(ref a);
                args.lColorTarget = Color.FromArgb(a, r, g, b);

                comma.GetInt(ref r); comma.GetInt(ref g); comma.GetInt(ref b); comma.GetInt(ref a);
                args.lColorStatusFill = Color.FromArgb(a, r, g, b);

                comma.GetInt(ref r); comma.GetInt(ref g); comma.GetInt(ref b); comma.GetInt(ref a);
                args.lColorStatusBack = Color.FromArgb(a, r, g, b);

                if (isV4 && !comma.IsEOS())
                {
                    comma.GetInt(ref r); comma.GetInt(ref g); comma.GetInt(ref b); comma.GetInt(ref a);
                    args.lColorStatusTitle = Color.FromArgb(a, r, g, b);
                }
                else
                {
                    args.lColorStatusTitle = load.lTextColor;
                }

                comma.GetInt(ref r); comma.GetInt(ref g); comma.GetInt(ref b); comma.GetInt(ref a);
                args.lColorStatusValue = Color.FromArgb(a, r, g, b);

                comma.GetInt(ref r); comma.GetInt(ref g); comma.GetInt(ref b); comma.GetInt(ref a);
                args.lColorTargetValue = Color.FromArgb(a, r, g, b);

                comma.GetInt(ref r); comma.GetInt(ref g); comma.GetInt(ref b); comma.GetInt(ref a);
                args.lColorPreValue = Color.FromArgb(a, r, g, b);

                comma.GetInt(ref r); comma.GetInt(ref g); comma.GetInt(ref b); comma.GetInt(ref a);
                args.lColorExValue = Color.FromArgb(a, r, g, b);

                comma.GetInt(ref r); comma.GetInt(ref g); comma.GetInt(ref b); comma.GetInt(ref a);
                args.lColorCardBack = Color.FromArgb(a, r, g, b);

                comma.GetInt(ref r); comma.GetInt(ref g); comma.GetInt(ref b); comma.GetInt(ref a);
                args.lColorCardBorder = Color.FromArgb(a, r, g, b);

                comma.GetInt(ref r); comma.GetInt(ref g); comma.GetInt(ref b); comma.GetInt(ref a);
                args.lColorAreaFill = Color.FromArgb(a, r, g, b);

                comma.GetInt(ref r); comma.GetInt(ref g); comma.GetInt(ref b); comma.GetInt(ref a);
                args.lColorForecastBand = Color.FromArgb(a, r, g, b);

                comma.GetInt(ref r); comma.GetInt(ref g); comma.GetInt(ref b); comma.GetInt(ref a);
                args.lColorPeakLine = Color.FromArgb(a, r, g, b);

                // 표준 색상 전달
                args.lColorBack = load.lBackColor;
                args.lColorFill = load.lFillColor;
                args.lColorText = load.lTextColor;
                args.lColorGuideLine = load.lGuideLineColor;

                parent.AddObject(new ObjectDemandChart(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
                    args));
            }
        }

        // BarcodeDisplay 로드 26-03-09
        void LoadBarcodeDisplay(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

            if (load.run(reader, command))
            {
                ObjectArgsBarcodeDisplay args = new ObjectArgsBarcodeDisplay();
                CommaTextReader comma = new CommaTextReader();

                if (!string.IsNullOrEmpty(load.sStringOption))
                {
                    comma.Set(load.sStringOption);
                    comma.GetInt(ref args.nBarcodeFormat);

                    string temp = "";
                    comma.GetString(ref temp);
                    args.sTextSource = temp;

                    temp = "";
                    comma.GetString(ref temp);
                    args.sTagBindingTemplate = temp;

                    int bval = 0;
                    comma.GetInt(ref bval);
                    args.bAutoUpdate = (bval == 1);

                    comma.GetInt(ref args.nErrorCorrectionLevel);

                    // 추가 필드 (이전 버전 파일에는 없을 수 있으나 기본값으로 처리)
                    string savePath = "";
                    comma.GetString(ref savePath);
                    args.sSavePath = savePath;

                    int bAutoSave = 0;
                    comma.GetInt(ref bAutoSave);
                    args.bAutoSave = (bAutoSave == 1);

                    int bHRT = 1;
                    comma.GetInt(ref bHRT);
                    args.bShowHumanReadableText = (bHRT == 1);
                }

                parent.AddObject(new ObjectBarcodeDisplay(ocp, form, load.rRect, load.eID, load.objGeneral, args));
            }
        }

        // BarcodeScanner 로드 26-03-09
        void LoadBarcodeScanner(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command)
        {
            LoadObjectFromModX load = new LoadObjectFromModX(ocp);

            if (load.run(reader, command))
            {
                ObjectArgsBarcodeScanner args = new ObjectArgsBarcodeScanner();
                CommaTextReader comma = new CommaTextReader();

                if (!string.IsNullOrEmpty(load.sStringOption))
                {
                    comma.Set(load.sStringOption);

                    string temp = "";
                    comma.GetString(ref temp);
                    args.sScannerName = temp;

                    temp = "";
                    comma.GetString(ref temp);
                    args.sResultTagName = temp;

                    int bval = 0;
                    comma.GetInt(ref bval);
                    args.bShowLastScan = (bval == 1);

                    bval = 0;
                    comma.GetInt(ref bval);
                    args.bShowStatus = (bval == 1);

                    comma.GetInt(ref args.nBackColor);
                    comma.GetInt(ref args.nTextColor);
                    comma.GetInt(ref args.nValidColor);
                    comma.GetInt(ref args.nInvalidColor);
                }

                parent.AddObject(new ObjectBarcodeScanner(ocp, form, load.rRect, load.eID, load.objGeneral, args));
            }
        }

    }
}


