using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using NetTools;
using NetTools.OldDefine;
using AutoLib;
using System.Collections;
using AutoLibLocal;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for ObjectGroupLoadMod.
	/// </summary>
	public class ObjectGroupLoadMod
	{
		ObjectGroup parent;

		public ObjectGroupLoadMod()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public int Load(ObjectCommonProperty ocp, ObjectGroup p, Form form, TextReader reader, int depth, EnumModType load_type)
		{
			parent = p;
			string one_line="";
			string imsi ="";
			CommaBlockString commaBuf = new CommaBlockString();
			
			//int resolution_flag = OFF;
			//int tile_flag = OFF;

			parent.FreeAllObjectBuf();

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;
				if(one_line.Length == 0)	continue;
				if(one_line[0] == '[')	continue;

				commaBuf.Set(one_line);
				commaBuf.GetString(ref imsi);

				if(imsi == "Resolution") {}
				else if(imsi == "DefaultLocation") {}
				else if(imsi == "Version")	{}
				else if(imsi == "ModuleOptic")	{}
				else if(imsi == "ModuleWindowStyle")	{}
				else if(imsi == "ModuleWindowStyleFlag"){}
				else if(imsi == "BackGroundColor"){}
				else if(imsi == "BackGround"){}
				else if(imsi == "GroupRect") 
				{	
                    // Group의 사각형이다. Ver 6.20 부터 사용되었다.
                    int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                    commaBuf.GetInt(ref x1);
                    commaBuf.GetInt(ref y1);
                    commaBuf.GetInt(ref x2);
                    commaBuf.GetInt(ref y2);
                    parent.UpdateZone(form, x1, y1, x2, y2);
					//commaBuf.GetInt(ref parent.rGroupZone.left);
					//commaBuf.GetInt(ref parent.rGroupZone.top);
					//commaBuf.GetInt(ref parent.rGroupZone.right);
					//commaBuf.GetInt(ref parent.rGroupZone.bottom);
				}
				else if(imsi == "GroupSize") 
				{	// Group 실제 크기이다. Ver 6.20 부터 사용되었다.
					commaBuf.GetInt(ref parent.sizeGroup.cx);
					commaBuf.GetInt(ref parent.sizeGroup.cy);
				}
		
				else if(imsi == "Group") 
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

				else if(imsi == "Bitmap") 
				{
					LoadBitmap(ocp, form, reader, imsi, commaBuf);
				}
				
				else if(imsi == "ButtonModule3D") 
				{
					LoadButtonModule3D(ocp, reader, imsi, commaBuf);
				}
				else if(imsi == "ButtonModuleHide") 
				{
					RECT r = new RECT();
					string mod_file="";

					commaBuf.GetInt(ref r.left);
					commaBuf.GetInt(ref r.top);
					commaBuf.GetInt(ref r.right);
					commaBuf.GetInt(ref r.bottom);
					commaBuf.GetString(ref mod_file);

					ObjectGeneral general = new ObjectGeneral();
					general.sClassName = "ModuleHide1";

					parent.AddObject(new ObjectButtonModuleHide(ocp, r, null, general,
						mod_file));
				}
				else if(imsi == "ButtonProgramm") 
				{
					LoadButtonProgramm(ocp, reader, imsi, commaBuf);
				}
				else if(imsi == "ButtonDigitalOut") 
				{
					LoadButtonDigitalOut(ocp, reader, imsi, commaBuf);
				}
				else if(imsi == "Animation") 
				{
					LoadAnimation(ocp, form, reader, imsi, commaBuf);
				}
				else if(imsi == "DigitalAnimation") 
				{
					LoadDigitalAnimation(ocp, form, reader, imsi, commaBuf);
				}
				else if(imsi == "DigitalCircle") 
				{
					LoadDigitalCircle(ocp, reader, imsi, commaBuf);
					
				}
				else if(imsi == "DigitalRectangle") 
				{
					LoadDigitalRectangle(ocp, reader, imsi, commaBuf);
				}
				else if(imsi == "DigitalString") 
				{
					LoadDigitalString(ocp, form, reader, imsi, commaBuf);
				}
				else if(imsi == "AnalogRectangle") 
				{
					LoadAnalogRectangle(ocp, reader, imsi, commaBuf);
				}
				else if(imsi == "AnalogString") 
				{
					LoadAnalogString(ocp, form, reader, imsi, commaBuf);
				}
				else if(imsi == "AnalogGuage") 
				{
					LoadAnalogGuage(ocp, commaBuf);
				}
				else if(imsi == "AnalogMeter") 
				{
					LoadAnalogMeter(ocp, reader, imsi);
				}
				else if(imsi == "AnalogStatus") 
				{
					LoadAnalogStatus(ocp, form, reader, imsi, commaBuf);
				}
				else if(imsi == "AnalogTotalCurrent") 
				{
					LoadChangeValueDisplay(ocp, reader, imsi, commaBuf);
				}
				/*
				else if(strncmp(imsi, "AnalogTotalCurrent", 18) == 0) {
					WORD display;
					float level;
			
					LOGFONT font;

					MakeDefaultLogFont(&font);
            
					commaBuf.GetInt(x1);
					commaBuf.GetInt(y1);
					commaBuf.GetInt(x2);
					commaBuf.GetInt(y2);
					commaBuf.GetWORD(display);
					commaBuf.GetFloat(level);
					commaBuf.GetLong(tcolor);
					commaBuf.GetLong(bcolor);

					commaBuf.GetString(font.lfFaceName, sizeof(font.lfFaceName));
					commaBuf.GetLong(font.lfHeight);

					AddObject(new ObjectAnalogTotalCurrent(hdc, nTerminal, x1, y1, x2, y2, display, level, tcolor, bcolor, &font));
				}
				*/
				else if(imsi == "AnalogRotate") 
				{
					LoadAnalogRotate(ocp, reader, imsi);
				}
				else if(imsi == "AnalogGraph")
				{
					LoadAnalogGraph(ocp, form, reader, imsi);					
				}
				else if(imsi == "AnalogTrend")
				{
					LoadAnalogTrend(ocp, form, reader, imsi);
				}
				else if(imsi == "AnalogData")
				{
					LoadAnalogData(ocp, form, reader, imsi);
				}

				else if(imsi == "MultiTrend") 
				{
					LoadMultiTrend(ocp, form, reader, imsi);
				}
				else if(imsi == "ObjectMultiGraph") 
				{
					LoadMultiGraph(ocp, form, reader, imsi);
				}
				else if(imsi == "StringTag") 
				{
					LoadStringString(ocp, reader, imsi);
				}
				else if(imsi == "Text") 
				{
					LoadText(ocp, reader, imsi, commaBuf);
				}
				else if(imsi == "Clock") 
				{
					LoadClock(ocp, reader, imsi, commaBuf);
				}
				else if(imsi == "Date") 
				{
					LoadDate(ocp, reader, imsi, commaBuf);
				}
				else if(imsi == "Rectangle") 
				{
					LoadRectangle(ocp, reader, imsi);
				}
				else if(imsi == "ObjectRoundRectangle") 
				{
					LoadRoundRectangle(ocp, reader, imsi);
				}
				else if(imsi == "SingleText") 
				{
					LoadSingleText(ocp, form, reader, imsi);
				}
				else if(imsi == "Circle") 
				{
					LoadCircle(ocp, reader, imsi);
				}
				else if(imsi == "Line") 
				{
					LoadLine(ocp, reader, imsi);
				}
				else if(imsi == "Poly") 
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
				else if(imsi == "ObjectMilliData") 
				{
					LoadObjectMilliData(ocp, form, reader, imsi);
				}
				else if(String.Compare(imsi, "ObjectRealTimeTestGraph") == 0) 
				{
					LoadObjectRealTimeTestGraph(ocp, form, reader, imsi);
				}
				else if(String.Compare(imsi, "ObjectXYGraph") == 0) 
				{
					LoadObjectXYGraph(ocp, form, reader, imsi);
				}
				else 
				{
					LoadUnknownObject(ocp, reader, imsi, commaBuf);
				}
			}

			// Group Rect가 설정되어 있지 않으면 6.20 이전 버전이다. root group은 계산하지 않는다.
			if(depth > 0) 
			{
				if(parent.sizeGroup.cx == 0 && parent.sizeGroup.cy == 0) 
				{
					parent.MakeGroupRect(form);
				}
			}

			return 1;
		}

		void LoadRectangle(ObjectCommonProperty ocp, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				ObjectArgsRectangle args = new ObjectArgsRectangle();

				args.nBorderStyle = load.wLineOption-1;
				if(args.nBorderStyle < 0)	args.nBorderStyle = 0;	// 일반선

				if(load.wLineOption > 1)	load.wLineOption = 1;	// 무조건 실선.

				parent.AddObject(new ObjectRectangle(ocp, load.rRect, load.eID, load.objGeneral,
					load.lLineColor, load.lFillColor, load.wLineOption, load.wLineThick, args));
			}
		}

		void LoadPoly(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				parent.AddObject(new ObjectPoly(ocp, form, load.rRect, load.eID, load.objGeneral,
					load.lLineColor, load.lFillColor, load.wLineOption, load.wLineThick, load.blockPoint));
			}
		}

		void LoadObjectCurve(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				parent.AddObject(new ObjectCurve(ocp, form, load.rRect, load.eID, load.objGeneral,
					load.lLineColor, load.lFillColor, load.wLineOption, load.wLineThick, load.blockCurve));
			}
		}

		void LoadCircle(ObjectCommonProperty ocp, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				parent.AddObject(new ObjectCircle(ocp, load.rRect, load.eID, load.objGeneral,
					load.lLineColor, load.lFillColor, load.wLineOption, load.wLineThick, new ObjectArgsCircle()));
			}
		}

		void LoadLine(ObjectCommonProperty ocp, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				parent.AddObject(new ObjectLine(ocp, load.rRect, load.eID, load.objGeneral,
					load.lLineColor, load.lFillColor, load.wLineOption, load.wLineThick));
			}
		}

		void LoadRoundRectangle(ObjectCommonProperty ocp, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				ObjectArgsRoundRectangle args = new ObjectArgsRoundRectangle();
				CommaBlockString comma = new CommaBlockString();

				comma.Set(load.sStringOption);
				comma.GetInt(ref args.round_x);
				comma.GetInt(ref args.round_y);

				parent.AddObject(new ObjectRoundRectangle(ocp, load.rRect, load.eID, load.objGeneral,
					load.lLineColor, load.lFillColor, load.wLineOption, load.wLineThick, args));
			}
		}

		void LoadBitmap(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command, CommaBlockString comma)
		{
			string buf="";

			comma.GetString(ref buf);
			if(String.Compare(buf, "BEGIN") != 0) 
			{
				ObjectArgsBitmap args = new ObjectArgsBitmap();
				RECT r = new RECT();

				args.sBitmapFile = buf;
				args.nOverlayMethod = 0;

				comma.GetInt(ref r.left);
				comma.GetInt(ref r.top);

				if(comma.IsEOS()) 
				{
					r.right = -9999;
					r.bottom = -9999;
				}
				else 
				{
					comma.GetInt(ref r.right);
					comma.GetInt(ref r.bottom);
				}

				ObjectGeneral general = new ObjectGeneral();
				general.sClassName = "Bitmap1";

				parent.AddObject(new ObjectBitmap(ocp, form, r, null, general, args));
			}
			else 
			{
				LoadObjectFromMod load = new LoadObjectFromMod();

				if(load.run(reader, command)) 
				{
					ObjectArgsBitmap args = new ObjectArgsBitmap();

					args.sBitmapFile = load.sFileName;
					args.nOverlayMethod = load.nOverlayMethod;

					parent.AddObject(new ObjectBitmap(ocp, form, load.rRect, load.eID, load.objGeneral,
						args));
				}
			}
		}

		void LoadButtonModule3D(ObjectCommonProperty ocp, TextReader reader, string command, CommaBlockString comma)
		{
			string buf="";

			comma.GetString(ref buf);
			if(String.Compare(buf, "BEGIN") != 0) 
			{
				LOGFONT font = new LOGFONT();
				ObjectArgsButtonPublic args = new ObjectArgsButtonPublic();
				RECT r = new RECT();
				long font_height=0;
				string sFileName = "";

				r.left = ConvertTool.ToInt32(buf);
				comma.GetInt(ref r.top);
				comma.GetInt(ref r.right);
				comma.GetInt(ref r.bottom);
				comma.GetString(ref args.sText);
				comma.GetColor(ref args.tcolor);
				comma.GetColor(ref args.bcolor.basic_color);
				comma.GetString(ref font.lfFaceName);
				comma.GetLong(ref font_height);
				comma.GetString(ref sFileName);

				font.style = FontStyle.Regular;

				font.lfHeight = (int)((-font_height*(Double)72.0/96.0)+0.5);

				ObjectGeneral general = new ObjectGeneral();
				general.sClassName = "ButtonModule3D1";

				parent.AddObject(new ObjectButtonModule3D(ocp, r, null, font, general, args, sFileName));
			}
			else 
			{
				LoadObjectFromMod load = new LoadObjectFromMod();

				if(load.run(reader, command)) 
				{
					ObjectArgsButtonPublic args = new ObjectArgsButtonPublic();

					args.sText = load.sString;
					args.tcolor = load.lTextColor;
					args.bcolor = load.lBackColor;

					parent.AddObject(new ObjectButtonModule3D(ocp, load.rRect, load.eID, load.fontStruct, load.objGeneral,
						args, load.sFileName));
				}
			}
		}

		void LoadButtonProgramm(ObjectCommonProperty ocp, TextReader reader, string command, CommaBlockString comma)
		{
			string buf="";

			comma.GetString(ref buf);
			if(String.Compare(buf, "BEGIN") != 0) 
			{
				LOGFONT font = new LOGFONT();
				ObjectArgsButtonPublic args = new ObjectArgsButtonPublic();
				RECT r = new RECT();
				long font_height=0;
				string sFileName = "";

				r.left = ConvertTool.ToInt32(buf);
				comma.GetInt(ref r.top);
				comma.GetInt(ref r.right);
				comma.GetInt(ref r.bottom);
				comma.GetString(ref args.sText);
				comma.GetColor(ref args.tcolor);
				comma.GetColor(ref args.bcolor.basic_color);
				comma.GetString(ref font.lfFaceName);
				comma.GetLong(ref font_height);
				comma.GetString(ref sFileName);

				font.style = FontStyle.Regular;

				font.lfHeight = (int)((-font_height*(Double)72.0/96.0)+0.5);

				ObjectGeneral general = new ObjectGeneral();
				general.sClassName = "ButtonProgram1";

				parent.AddObject(new ObjectButtonProgram(ocp, r, null, font, general, args, 0, sFileName, null));
			}
			else 
			{
				LoadObjectFromMod load = new LoadObjectFromMod();

				if(load.run(reader, command)) 
				{
					ObjectArgsButtonPublic args = new ObjectArgsButtonPublic();

					args.sText = load.sString;
					args.tcolor = load.lTextColor;
					args.bcolor = load.lBackColor;

					parent.AddObject(new ObjectButtonProgram(ocp, load.rRect, load.eID, load.fontStruct, load.objGeneral,
						args, 0, load.sFileName, null));
				}
			}
		}

		void LoadButtonDigitalOut(ObjectCommonProperty ocp, TextReader reader, string command, CommaBlockString comma)
		{
			string buf="";

			comma.GetString(ref buf);
			if(String.Compare(buf, "BEGIN") != 0) 
			{
				LOGFONT font = new LOGFONT();
				ObjectArgsButtonPublic args = new ObjectArgsButtonPublic();
				RECT r = new RECT();
				long font_height=0;
				int do_method = 0;
				int do_delaytime = 0;
				ArrayList block = new ArrayList();
				BUTTON_DOUT_STRUCT member;

				r.left = ConvertTool.ToInt32(buf);
				comma.GetInt(ref r.top);
				comma.GetInt(ref r.right);
				comma.GetInt(ref r.bottom);
				comma.GetString(ref args.sText);
				comma.GetColor(ref args.tcolor);
				comma.GetColor(ref args.bcolor.basic_color);
				comma.GetString(ref font.lfFaceName);
				comma.GetLong(ref font_height);
				comma.GetInt(ref do_method);
				for(int i = 0; i < 16; i++) 
				{
					member = new BUTTON_DOUT_STRUCT();
					comma.GetString(ref member.tag);
					member.tag = member.tag.Trim();
					if(member.tag.Length > 0) 
					{
						block.Add(member);
					}
				}
				comma.GetInt(ref do_delaytime);

				font.style = FontStyle.Regular;

				font.lfHeight = (int)((-font_height*(Double)72.0/96.0)+0.5);

				ObjectGeneral general = new ObjectGeneral();
				general.sClassName = "ButtonDigitalOut1";

				parent.AddObject(new ObjectButtonDigitalOut(ocp, r, null, font, general,
					args, 
					do_method,
					block,
					do_delaytime));
			}
			else 
			{
				LoadObjectFromMod load = new LoadObjectFromMod();

				if(load.run(reader, command)) 
				{
					ObjectArgsButtonPublic args = new ObjectArgsButtonPublic();

					args.sText = load.sString;
					args.tcolor = load.lTextColor;
					args.bcolor = load.lBackColor;

					parent.AddObject(new ObjectButtonDigitalOut(ocp, load.rRect, load.eID, load.fontStruct, load.objGeneral,
						args, 
						load.mouseResponse.do_method,
						load.blockButtonDoutMember,
						load.mouseResponse.do_delaytime));
				}
			}
		}

		void LoadAnimation(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command, CommaBlockString comma)
		{
			string buf="";

			comma.GetString(ref buf);
			if(String.Compare(buf, "BEGIN") != 0) 
			{
				RECT rect = new RECT();
				ObjectArgsAnimation args = new ObjectArgsAnimation();

				args.sAnimationFile = buf;

				comma.GetInt(ref rect.left);
				comma.GetInt(ref rect.top);

				if(comma.IsEOS()) 
				{
					rect.right = -9999;
					rect.bottom = -9999;
				}
				else 
				{
					comma.GetInt(ref rect.right);
					comma.GetInt(ref rect.bottom);
					comma.GetInt(ref args.nOverlayMethod);
				}

				ObjectGeneral general = new ObjectGeneral();
				general.sClassName = "Animation1";

				parent.AddObject(new ObjectAnimation(ocp, form, rect, null, general, args));
			}
			else 
			{
				LoadObjectFromMod load = new LoadObjectFromMod();

				if(load.run(reader, command)) 
				{
					ObjectArgsAnimation args = new ObjectArgsAnimation();

					args.sAnimationFile = load.sFileName;
					args.nOverlayMethod = load.nOverlayMethod;

					parent.AddObject(new ObjectAnimation(ocp, form, load.rRect, load.eID, load.objGeneral,
						args));
				}
			}
		}

		void LoadAnalogString(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command, CommaBlockString comma)
		{
			string buf="";

			comma.GetString(ref buf);
			if(String.Compare(buf, "BEGIN") != 0) 
			{
				int style = 0;
				LOGFONT font = new LOGFONT();
				RECT r = new RECT();
				RECT rTag = new RECT();
				long font_height=0;
				ObjectArgsAnalogString args = new ObjectArgsAnalogString();
				EXPAND_ID_STRUCT eid = new EXPAND_ID_STRUCT();
				eid.mod_type = EnumModType.mod;

				comma.GetInt(ref rTag.left);
				comma.GetInt(ref rTag.top);
				comma.GetInt(ref rTag.right);
				comma.GetInt(ref rTag.bottom);
				comma.GetInt(ref r.left);
				comma.GetInt(ref r.top);
				r.right = -9999;
				r.bottom = -9999;
				comma.GetColor(ref args.colorText);
				comma.GetColor(ref args.colorBack.basic_color);
				comma.GetInt(ref style);	// 사용안함
				comma.GetString(ref font.lfFaceName);
				comma.GetLong(ref font_height);
				comma.GetInt(ref args.nBoxUse);
				comma.GetInt(ref args.nDisplayValue);

				font.style = FontStyle.Regular;
				font.lfHeight = (int)((-font_height*(Double)72.0/96.0)+0.5);

				ObjectGeneral general = new ObjectGeneral();
				general.sClassName = "AnalogString1";

				ObjectAnalogString obj = new ObjectAnalogString(ocp, form, r, eid, general, font,
					buf.Trim(), null, rTag, args);
				
				parent.AddObject(obj);
			}
			else 
			{
				LoadObjectFromMod load = new LoadObjectFromMod();

				if(load.run(reader, command)) 
				{
					ObjectArgsAnalogString args = new ObjectArgsAnalogString();

					args.nBoxUse = load.nBackBox;
					args.nDisplayValue = load.nLocalMethod;
					args.colorBack = load.lBackColor;
					args.colorText = load.lTextColor;
				
					parent.AddObject(new ObjectAnalogString(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
						load.sTagName, load.mouseResponse, load.rTagRect, args));
				}
			}
		}

		void LoadAnalogGuage(ObjectCommonProperty ocp, CommaBlockString comma)
		{
			string tag = "";
			int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
			comma.GetString(ref tag);	// tag
			tag = tag.Trim();	// 10글자로 스페이스가 들어가 있다.
			comma.GetInt(ref x1);
			comma.GetInt(ref y1);
			comma.GetInt(ref x2);
			comma.GetInt(ref y2);

			ObjectArgsAnalogMeter args = new ObjectArgsAnalogMeter();

			args.colorBack.basic_color = Color.White;
			args.colorGuide = Color.Black;
			args.colorHand = Color.Red;
			args.colorText = Color.LightGray;
			args.thickHand = 3;
			args.colorBorder.basic_color = Color.LightGray;

			RECT rt = new RECT();
			rt.left = x1;
			rt.top = y1;
			rt.right = x2;
			rt.bottom = y2;

			ObjectGeneral general = new ObjectGeneral();
			general.sClassName = "OldMeter";
                			
			parent.AddObject(new ObjectAnalogMeter(ocp, rt, null, general, null, tag,
				null, null, args));
		}

		void LoadAnalogMeter(ObjectCommonProperty ocp, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{

				ObjectArgsAnalogMeter args = new ObjectArgsAnalogMeter();

				args.colorBack = load.lBackColor;
				
				args.colorGuide = load.lGuideLineColor;
				args.colorHand = load.lLineColor;
				args.colorText = load.lTextColor;
				args.thickHand = load.wLineThick;

				CommaBlockString comma = new CommaBlockString();
				int r=0, g=0, b=0;
				comma.Set(load.sStringOption);
				comma.GetInt(ref r);
				comma.GetInt(ref g);
				comma.GetInt(ref b);
				args.colorBorder.basic_color = Color.FromArgb(r, g, b);

				RECT rt = new RECT();
				rt.left = load.rTagRect.left;
				rt.top = load.rTagRect.top;
				rt.right = load.rTagRect.right;
				rt.bottom = load.rTagRect.bottom;
                				
				parent.AddObject(new ObjectAnalogMeter(ocp, rt, load.eID, load.objGeneral, load.fontStruct, load.sTagName,
					load.mouseResponse, load.rTagRect, args));
			}
		}

		void LoadChangeValueDisplay(ObjectCommonProperty ocp, TextReader reader, string command, CommaBlockString comma)
		{
			ObjectArgsChangeValueDisplay args = new ObjectArgsChangeValueDisplay();

			args.nListCount = 3;	// 이전에는 무조건 3이었다.

			RECT r = new RECT();
			
			LOGFONT lf = new LOGFONT();
			lf.style = FontStyle.Regular;
			long font_height=0;
			
			comma.GetInt(ref r.left);
			comma.GetInt(ref r.top);
			comma.GetInt(ref r.right);
			comma.GetInt(ref r.bottom);
			comma.Skip();//display
			comma.Skip();//level
			comma.GetColor(ref args.textColor);
			comma.GetColor(ref args.backColor.basic_color);
			comma.GetString(ref lf.lfFaceName);
			comma.GetLong(ref font_height);

			// 다시 복원
			//size = ::MulDiv(-lf->lfHeight, 72, ::GetDeviceCaps(hdc, LOGPIXELSY));
			lf.lfHeight = (int)((-font_height*(Double)72.0/96.0)+0.5);

			ObjectGeneral general = new ObjectGeneral();
			general.sClassName = "ChangeValueDisplay1";

			parent.AddObject(new ObjectChangeValueDisplay(ocp, r, null, general, lf, args));
		}

		void LoadAnalogRectangle(ObjectCommonProperty ocp, TextReader reader, string command, CommaBlockString comma)
		{
			string buf="";

			comma.GetString(ref buf);
			if(String.Compare(buf, "BEGIN") != 0) 
			{
				RECT rect = new RECT();
				RECT rTag = new RECT();
				int style=0;
				VIEW_RANGE_STRUCT view = new VIEW_RANGE_STRUCT();
				GUIDE_LINE_STRUCT guide = new GUIDE_LINE_STRUCT();
				int r=0, g=0, b=0;
				MOUSE_RESPONSE_STRUCT mr = new MOUSE_RESPONSE_STRUCT();
				ObjectArgsAnalogRectangle args = new ObjectArgsAnalogRectangle();
				EXPAND_ID_STRUCT eid = new EXPAND_ID_STRUCT();
				eid.mod_type = EnumModType.mod;

				comma.GetInt(ref rTag.left);
				comma.GetInt(ref rTag.top);
				comma.GetInt(ref rTag.right);
				comma.GetInt(ref rTag.bottom);
				comma.GetInt(ref rect.left);
				comma.GetInt(ref rect.top);
				comma.GetInt(ref rect.right);
				comma.GetInt(ref rect.bottom);
				comma.GetColor(ref args.colorOff.basic_color);
				comma.GetColor(ref args.colorOn.basic_color);
				comma.GetInt(ref style);	// 사용하지 않는 것으로 밝혀짐 (2003.9.24 8.3.0 개발중)	
				comma.GetInt(ref mr.mouse_response);
				comma.GetInt(ref args.nBarDir);

				comma.GetChar(ref view.flag);
				comma.GetDouble(ref view.fBase);
				comma.GetDouble(ref view.fFull);

				comma.GetChar(ref guide.method);
				comma.GetInt(ref guide.devideBig);
				comma.GetInt(ref r);
				comma.GetInt(ref g);
				comma.GetInt(ref b);
				guide.colorBig = Color.FromArgb(r, g, b);
				comma.GetInt(ref guide.devideSmall);
				comma.GetInt(ref r);
				comma.GetInt(ref g);
				comma.GetInt(ref b);
				guide.colorSmall = Color.FromArgb(r, g, b);

				comma.GetInt(ref guide.line_length);
				comma.GetChar(ref guide.bLevelString);
				comma.GetString(ref mr.scriptFilename);

				ObjectGeneral general = new ObjectGeneral();
				general.sClassName = "AnalogRectangle1";

				parent.AddObject(new ObjectAnalogRectangle(ocp, rect, eid, general, null, 
					buf.Trim(), mr, rTag, args, view, guide));
			}
			else 
			{
				LoadObjectFromMod load = new LoadObjectFromMod();

				if(load.run(reader, command)) 
				{
					ObjectArgsAnalogRectangle args = new ObjectArgsAnalogRectangle();
					//CommaBlockString comma = new CommaBlockString();
					string tag="";
					RECT rt = new RECT();
					RECT rMouse = new RECT();
					VIEW_RANGE_STRUCT view = new VIEW_RANGE_STRUCT();
					GUIDE_LINE_STRUCT guide = new GUIDE_LINE_STRUCT();
					int r=0, g=0, b=0;
					MOUSE_RESPONSE_STRUCT mouse_response = new MOUSE_RESPONSE_STRUCT();
                
					comma.Set(load.sStringOption);
					comma.GetString(ref tag);	// tag
					tag = tag.Trim();
					comma.GetInt(ref rMouse.left);
					comma.GetInt(ref rMouse.top);
					comma.GetInt(ref rMouse.right);
					comma.GetInt(ref rMouse.bottom);
					comma.GetInt(ref rt.left);
					comma.GetInt(ref rt.top);
					comma.GetInt(ref rt.right);
					comma.GetInt(ref rt.bottom);

					comma.GetColor(ref args.colorOff.basic_color);
					comma.GetColor(ref args.colorOn.basic_color);

					int reserved = 0;
					comma.GetInt(ref reserved);	//comma.GetInt(ref args.nStyle);

					comma.GetInt(ref mouse_response.mouse_response);
					comma.GetInt(ref args.nBarDir);

					comma.GetChar(ref view.flag);
					comma.GetDouble(ref view.fBase);
					comma.GetDouble(ref view.fFull);

					comma.GetChar(ref guide.method);
					comma.GetInt(ref guide.devideBig);
					comma.GetInt(ref r);
					comma.GetInt(ref g);
					comma.GetInt(ref b);
					guide.colorBig = Color.FromArgb(r, g, b);
					comma.GetInt(ref guide.devideSmall);
					comma.GetInt(ref r);
					comma.GetInt(ref g);
					comma.GetInt(ref b);
					guide.colorSmall = Color.FromArgb(r, g, b);

					comma.GetInt(ref guide.line_length);
					comma.GetChar(ref guide.bLevelString);

					ObjectGeneral general = new ObjectGeneral();
					general.sClassName = "AnalogRectangle1";

					parent.AddObject(new ObjectAnalogRectangle(ocp, rt, load.eID, general, load.fontStruct,
						tag, mouse_response, rMouse, args, view, guide));
				}
			}
		}

		void LoadAnalogRotate(ObjectCommonProperty ocp, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

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
					load.sTagName, load.mouseResponse, load.rTagRect, args));
			}
		}

		void LoadAnalogStatus(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command, CommaBlockString comma)
		{
			string buf="";

			comma.GetString(ref buf);
			if(String.Compare(buf, "BEGIN") != 0) 
			{
				ArrayList block = new ArrayList();
				ANALOG_STATUS_STRUCT status = new ANALOG_STATUS_STRUCT();
				RECT r = new RECT();
				RECT rTag = new RECT();
				ObjectArgsAnalogStatus args = new ObjectArgsAnalogStatus();
				EXPAND_ID_STRUCT eid = new EXPAND_ID_STRUCT();
				eid.mod_type = EnumModType.mod;

				comma.GetInt(ref rTag.left);
				comma.GetInt(ref rTag.top);
				comma.GetInt(ref rTag.right);
				comma.GetInt(ref rTag.bottom);
				comma.GetInt(ref r.left);
				comma.GetInt(ref r.top);
				r.right = -9999;
				r.bottom = -9999;

				for(int i = 0; i < 16; i++) 
				{
					status = new ANALOG_STATUS_STRUCT();
					comma.GetInt(ref status.type);
					comma.GetString(ref status.filename);
					block.Add(status);
				}

				ObjectGeneral general = new ObjectGeneral();
				general.sClassName = "AnalogStatus1";

				parent.AddObject(new ObjectAnalogStatus(ocp, form, r, eid, general, 
					buf.Trim(), null, rTag, args, block));
			}
			else 
			{
				LoadObjectFromMod load = new LoadObjectFromMod();

				if(load.run(reader, command))
				{
					ObjectArgsAnalogStatus args = new ObjectArgsAnalogStatus();

					parent.AddObject(new ObjectAnalogStatus(ocp, form, load.rRect, load.eID, load.objGeneral, 
						load.sTagName, load.mouseResponse, load.rTagRect, args, load.blockAnalogStatus));
				}
			}
		}

		void LoadDigitalAnimation(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command, CommaBlockString comma)
		{
			string buf="";

			comma.GetString(ref buf);
			if(String.Compare(buf, "BEGIN") != 0) 
			{
				RECT r = new RECT();
				RECT rTag = new RECT();
				MOUSE_RESPONSE_STRUCT mr = new MOUSE_RESPONSE_STRUCT();
				ObjectArgsDigitalAnimation args = new ObjectArgsDigitalAnimation();
				EXPAND_ID_STRUCT eid = new EXPAND_ID_STRUCT();
				eid.mod_type = EnumModType.mod;

				comma.GetInt(ref rTag.left);
				comma.GetInt(ref rTag.top);
				comma.GetInt(ref rTag.right);
				comma.GetInt(ref rTag.bottom);
				comma.GetInt(ref r.left);
				comma.GetInt(ref r.top);
				r.right = -9999;
				r.bottom = -9999;
				comma.GetString(ref args.sFileOff);
				comma.GetString(ref args.sFileOn);
				comma.GetInt(ref mr.mouse_response);
				comma.GetInt(ref mr.do_method);
				comma.GetInt(ref mr.do_delaytime);
				comma.GetInt(ref args.nOverlayMethod);

				ObjectGeneral general = new ObjectGeneral();
				general.sClassName = "DigitalAnimation1";

				parent.AddObject(new ObjectDigitalAnimation(ocp, form, r, eid, general, 
					buf.Trim(), mr, rTag, args));
			}
			else 
			{
				LoadObjectFromMod load = new LoadObjectFromMod();

				if(load.run(reader, command)) 
				{
					ObjectArgsDigitalAnimation args = new ObjectArgsDigitalAnimation();

					args.sFileOff = load.sFileNameOff;
					args.sFileOn = load.sFileNameOn;
					args.nOverlayMethod = load.nOverlayMethod;
				
					parent.AddObject(new ObjectDigitalAnimation(ocp, form, load.rRect, load.eID, load.objGeneral, 
						load.sTagName, load.mouseResponse, load.rTagRect, args));
				}
			}
		}

		void LoadDigitalCircle(ObjectCommonProperty ocp, TextReader reader, string command, CommaBlockString comma)
		{
			RECT r = new RECT();
			RECT rMouse = new RECT();

			ObjectArgsDigitalCircle args = new ObjectArgsDigitalCircle();

			string tag="";
			MOUSE_RESPONSE_STRUCT mouseResponse = new MOUSE_RESPONSE_STRUCT();
			EXPAND_ID_STRUCT eid = new EXPAND_ID_STRUCT();
			eid.mod_type = EnumModType.mod;

			comma.GetString(ref tag);	// tag
			tag = tag.Trim();
			comma.GetInt(ref rMouse.left);
			comma.GetInt(ref rMouse.top);
			comma.GetInt(ref rMouse.right);
			comma.GetInt(ref rMouse.bottom);
			comma.GetInt(ref r.left);
			comma.GetInt(ref r.top);
			comma.GetInt(ref r.right);
			comma.GetInt(ref r.bottom);

			comma.GetColor(ref args.colorOff);
			comma.GetColor(ref args.colorOn);

			comma.GetInt(ref mouseResponse.mouse_response);
			comma.GetInt(ref mouseResponse.do_method);
			comma.GetInt(ref mouseResponse.do_delaytime);

			ObjectGeneral general = new ObjectGeneral();
			general.sClassName = "DigitalCircle1";

			parent.AddObject(new ObjectDigitalCircle(ocp, r, eid, general, 
				tag, mouseResponse, rMouse, args));
		}

		void LoadDigitalRectangle(ObjectCommonProperty ocp, TextReader reader, string command, CommaBlockString comma)
		{
			RECT r = new RECT();
			RECT rMouse = new RECT();

			ObjectArgsDigitalRectangle args = new ObjectArgsDigitalRectangle();
            
			string tag="";
			MOUSE_RESPONSE_STRUCT mouseResponse = new MOUSE_RESPONSE_STRUCT();
			EXPAND_ID_STRUCT eid = new EXPAND_ID_STRUCT();
			eid.mod_type = EnumModType.mod;

			comma.GetString(ref tag);	// tag
			tag = tag.Trim();
			comma.GetInt(ref rMouse.left);
			comma.GetInt(ref rMouse.top);
			comma.GetInt(ref rMouse.right);
			comma.GetInt(ref rMouse.bottom);
			comma.GetInt(ref r.left);
			comma.GetInt(ref r.top);
			comma.GetInt(ref r.right);
			comma.GetInt(ref r.bottom);

			comma.GetColor(ref args.colorOff);
			comma.GetColor(ref args.colorOn);

			comma.GetInt(ref mouseResponse.mouse_response);
			comma.GetInt(ref mouseResponse.do_method);
			comma.GetInt(ref mouseResponse.do_delaytime);

			comma.GetInt(ref args.nDisplayMethod);

			ObjectGeneral general = new ObjectGeneral();
			general.sClassName = "DigitalRectangle1";

			parent.AddObject(new ObjectDigitalRectangle(ocp, r, eid, general, 
				tag, mouseResponse, rMouse, args));
		}

		void LoadDigitalString(ObjectCommonProperty ocp, System.Windows.Forms.Form form, TextReader reader, string command, CommaBlockString comma)
		{
			string buf="";

			comma.GetString(ref buf);
			if(String.Compare(buf, "BEGIN") != 0) 
			{
				LOGFONT font = new LOGFONT();
				RECT r = new RECT();
				RECT rTag = new RECT();
				long font_height=0;
				MOUSE_RESPONSE_STRUCT mr = new MOUSE_RESPONSE_STRUCT();
				ObjectArgsDigitalString args = new ObjectArgsDigitalString();
				EXPAND_ID_STRUCT eid = new EXPAND_ID_STRUCT();
				eid.mod_type = EnumModType.mod;

				comma.GetInt(ref rTag.left);
				comma.GetInt(ref rTag.top);
				comma.GetInt(ref rTag.right);
				comma.GetInt(ref rTag.bottom);
				comma.GetInt(ref r.left);
				comma.GetInt(ref r.top);
				r.right = -9999;
				r.bottom = -9999;
				comma.GetColor(ref args.colorOff);
				comma.GetColor(ref args.colorOn);
				comma.GetColor(ref args.colorBack.basic_color);
				comma.GetString(ref font.lfFaceName);
				comma.GetLong(ref font_height);
				comma.GetInt(ref mr.mouse_response);
				comma.GetInt(ref mr.do_method);
				comma.GetInt(ref mr.do_delaytime);

				font.style = FontStyle.Regular;
				font.lfHeight = (int)((-font_height*(Double)72.0/96.0)+0.5);

				ObjectGeneral general = new ObjectGeneral();
				general.sClassName = "DigitalString1";

				parent.AddObject(new ObjectDigitalString(ocp, form, r, eid, general, font,
					buf.Trim(), mr, rTag, args));
			}
			else 
			{
				LoadObjectFromMod load = new LoadObjectFromMod();

				if(load.run(reader, command)) 
				{
					ObjectArgsDigitalString args = new ObjectArgsDigitalString();

					args.colorBack = load.lBackColor;
					args.colorOff = load.lOffColor;
					args.colorOn = load.lOnColor;
				
					parent.AddObject(new ObjectDigitalString(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
						load.sTagName, load.mouseResponse, load.rTagRect, args));
				}
			}
		}

		void LoadStringString(ObjectCommonProperty ocp, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				ObjectArgsStringString args = new ObjectArgsStringString();

				args.nBoxUse = load.nBackBox;
				args.colorBack = load.lBackColor;
				args.colorText = load.lTextColor;
				
				parent.AddObject(new ObjectStringString(ocp, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					load.sTagName, load.mouseResponse, load.rTagRect, args, load.align));
			}
		}

		void LoadMultiTrend(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				ObjectArgsMultiTrend args = new ObjectArgsMultiTrend();

				args.lColorBack.basic_color = Color.LightGray;
				args.lColorFill.basic_color = load.lBackColor.basic_color;
				args.lColorGuideLine = load.lGuideLineColor;
				args.lColorText = load.lTextColor;
				args.pub.wDisplayFlags = (EnumDisplayFlag)load.wFlags;
				args.pub.wPointSize = load.wGraphPointSize;
				args.pub.nLevelDisplaySize = load.nLevelDisplaySize;								
				args.wLevelDevide = load.wLevelDevide;
				args.wShowUnit = load.nShowUnit;
				args.wTimeDevide = load.wTimeDevide;
				args.wTimeSelectOption = load.wTimeSelectOption;

				if(load.objGeneral.sClassName.Length == 0)
					load.objGeneral.sClassName = load.sFileName;
				
				parent.AddObject(new ObjectMultiTrend(ocp, form, load.rTagRect, load.eID, load.objGeneral, load.fontStruct,
					args, load.graphMember));
			}
		}

		// AnalogTrend는 9.0부터는 없어졌지만 MultiTrend로 대체한다.
		void LoadAnalogTrend(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				ObjectArgsMultiTrend args = new ObjectArgsMultiTrend();

				args.lColorBack.basic_color = Color.LightGray;
				args.lColorFill.basic_color = load.lBackColor.basic_color;
				args.lColorGuideLine = load.lGuideLineColor;
				args.lColorText = load.lTextColor;
				args.pub.wDisplayFlags = (EnumDisplayFlag)load.wFlags;
				args.wLevelDevide = load.wLevelDevide;
				args.pub.wPointSize = load.wGraphPointSize;
				args.wShowUnit = load.nShowUnit;
				args.wTimeDevide = load.wTimeDevide;
				args.wTimeSelectOption = load.wTimeSelectOption;

				if(load.objGeneral.sClassName.Length == 0)
					load.objGeneral.sClassName = load.sFileName;
				
				parent.AddObject(new ObjectMultiTrend(ocp, form, load.rTagRect, load.eID, load.objGeneral, load.fontStruct,
					args, load.graphMember));
			}

			/*
			 
			LoadObject load;

	if(load.run(in, "AnalogTrend")) {
		PUBLIC_GRAPH_STRUCT pub;

		memset(&pub, 0, sizeof(PUBLIC_GRAPH_STRUCT));
		pub.bcolor = load.lBackColor;
		pub.gcolor = load.lGuideLineColor;
		pub.tcolor = load.lTextColor;
		pub.point_size = load.wGraphPointSize;
		pub.showunit = load.nShowUnit;
		pub.wDisplayFlags = load.wFlags;
		pub.leveldevide = load.wLevelDevide;

		AddObject(new ObjectAnalogTrend(hdc, nTerminal,
										 load.graphMember,
										 load.rTagRect.left,  load.rTagRect.top, load.rTagRect.right, load.rTagRect.bottom,
										 &pub,
										 load.wTimeDevide,
										 load.fontStruct));
	}
			
			*/ 
		}

		// AnalogData는 9.0부터는 없어졌지만 MultiTrend로 대체한다.
		void LoadAnalogData(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				ObjectArgsMultiTrend args = new ObjectArgsMultiTrend();

				args.lColorBack.basic_color = Color.LightGray;
				args.lColorFill.basic_color = load.lBackColor.basic_color;
				args.lColorGuideLine = load.lGuideLineColor;
				args.lColorText = load.lTextColor;
				args.pub.wDisplayFlags = (EnumDisplayFlag)load.wFlags;
				args.pub.wDisplayFlags |= EnumDisplayFlag.BACK_BORDER;
				args.wLevelDevide = load.wLevelDevide;
				args.pub.wPointSize = load.wGraphPointSize;
				args.wShowUnit = load.nShowUnit;
				args.wTimeDevide = load.wTimeDevide;
				args.wTimeSelectOption = load.wTimeSelectOption;

				if(load.objGeneral.sClassName.Length == 0)
					load.objGeneral.sClassName = load.sFileName;

				// 시,일,월,분 을 분,시,일,월으로 바꾼다.
				args.wTimeSelectOption = (load.wTimeSelectOption == 3) ? 0 : load.wTimeSelectOption+1;
				
				parent.AddObject(new ObjectMultiTrend(ocp, form, load.rTagRect, load.eID, load.objGeneral, load.fontStruct,
					args, load.graphMember));
			}

		}

		void LoadMultiGraph(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				ObjectArgsMultiGraph args = new ObjectArgsMultiGraph();

				args.lColorBack.basic_color = Color.LightGray;
				args.lColorFill.basic_color = load.lBackColor.basic_color;
				args.lColorGuideLine = load.lGuideLineColor;
				args.lColorText = load.lTextColor;
				args.pub.wDisplayFlags = (EnumDisplayFlag)load.wFlags;
				args.pub.wPointSize = load.wGraphPointSize;
				args.pub.nLevelDisplaySize = load.nLevelDisplaySize;								
				args.wLevelDevide = load.wLevelDevide;
				args.wShowUnit = load.nShowUnit;
				args.wTimeDevide = load.wTimeDevide;
				//args.wTimeSelectOption = load.wTimeSelectOption;

				if(load.objGeneral.sClassName.Length == 0)
					load.objGeneral.sClassName = load.sFileName;

				CommaBlockString comma = new CommaBlockString();
				comma.Set(load.sStringOption);
                args.nDataTime = comma.GetInt();
				comma.GetChar(ref args.bTimeDirToLeft);
				comma.GetChar(ref args.bDisplayByTime);
				
				parent.AddObject(new ObjectMultiGraph(ocp, form, load.rTagRect, load.eID, load.objGeneral, load.fontStruct,
					args, load.graphMember));
			}
		}

		// AnalogGraph는 9.0부터는 없어졌지만 MultiGraph로 대체한다.
		void LoadAnalogGraph(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				ObjectArgsMultiGraph args = new ObjectArgsMultiGraph();

				args.lColorBack.basic_color = Color.LightGray;
				args.lColorFill.basic_color = load.lBackColor.basic_color;
				args.lColorGuideLine = load.lGuideLineColor;
				args.lColorText = load.lTextColor;
				args.pub.wDisplayFlags = (EnumDisplayFlag)load.wFlags;
				args.wLevelDevide = load.wLevelDevide;
				args.pub.wPointSize = load.wGraphPointSize;
				args.wShowUnit = load.nShowUnit;
				args.wTimeDevide = load.wTimeDevide;
				//args.wTimeSelectOption = load.wTimeSelectOption;

				if(load.objGeneral.sClassName.Length == 0)
					load.objGeneral.sClassName = load.sFileName;

				CommaBlockString comma = new CommaBlockString();
				comma.Set(load.sStringOption);
				
				comma.GetChar(ref args.bTimeDirToLeft);

				args.pub.wDisplayFlags -= EnumDisplayFlag.BY_DESCRIPTION;
				args.pub.wDisplayFlags -= EnumDisplayFlag.TAG_MIN_MAX;
				args.pub.wDisplayFlags -= EnumDisplayFlag.TAG_CURR;
				args.pub.wDisplayFlags -= EnumDisplayFlag.TAG_OLD_CURR;

				ANALOG_GRAPH_MEMBER graph;

				for(int i = 0; i < load.graphMember.Count; i++) 
				{
					graph = (ANALOG_GRAPH_MEMBER)load.graphMember[i];
					graph.nPointType = 0;
					graph.nLineThick = 1;
					graph.nLevelFrom = 0;
					graph.nLevelTo = 100;
					graph.nAxisPosition = 0;
					graph.nValueType = 0;
				}
			
				parent.AddObject(new ObjectMultiGraph(ocp, form, load.rTagRect, load.eID, load.objGeneral, load.fontStruct,
					args, load.graphMember));
			}


		}
		

		void LoadSingleText(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				ObjectArgsSingleText args = new ObjectArgsSingleText();

				args.text = load.sString;
				args.textColor = load.lTextColor;

				load.eID.bLineColorToTextColor = true;

				parent.AddObject(new ObjectSingleText(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args));
			}
		}

		void LoadText(ObjectCommonProperty ocp, TextReader reader, string command, CommaBlockString comma)
		{
			string buf="";

			comma.GetString(ref buf);
			if(String.Compare(buf, "BEGIN") != 0) 
			{
				LOGFONT font = new LOGFONT();
				ObjectArgsText args = new ObjectArgsText();
				RECT r = new RECT();
				Color bcolor=Color.White;
				long font_height=0;

				r.left = ConvertTool.ToInt32(buf);
				comma.GetInt(ref r.top);
				comma.GetInt(ref r.right);
				comma.GetInt(ref r.bottom);
				comma.GetString(ref args.text);
				comma.GetColor(ref args.textColor);
				comma.GetColor(ref bcolor);
				comma.GetString(ref font.lfFaceName);
				comma.GetLong(ref font_height);
				font.style = FontStyle.Regular;

				font.lfHeight = (int)((-font_height*(Double)72.0/96.0)+0.5);

				ObjectGeneral general = new ObjectGeneral();
				general.sClassName = "Text1";

				parent.AddObject(new ObjectText(ocp, r, null, general, font, args));
			}
			else 
			{
				LoadObjectFromMod load = new LoadObjectFromMod();

				if(load.run(reader, command)) 
				{
					ObjectArgsText args = new ObjectArgsText();

					args.text = load.sString;
					args.textColor = load.lTextColor;
                    args.align.x = 0;
                    args.align.y = 0;

					parent.AddObject(new ObjectText(ocp, load.rRect, load.eID, load.objGeneral, load.fontStruct,
						args));
				}
			}
		}

		void LoadDate(ObjectCommonProperty ocp, TextReader reader, string command, CommaBlockString comma)
		{
			ObjectArgsDate args = new ObjectArgsDate();

			RECT r = new RECT();
			
			//Color tcolor=Color.Black, bcolor=Color.White;
			LOGFONT lf = new LOGFONT();
			lf.style = FontStyle.Regular;
			long font_height=0;
			
			comma.GetInt(ref r.left);
			comma.GetInt(ref r.top);
			comma.GetInt(ref r.right);
			comma.GetInt(ref r.bottom);
			comma.GetColor(ref args.textColor);
			comma.GetColor(ref args.backColor.basic_color);
			comma.GetString(ref lf.lfFaceName);
			comma.GetLong(ref font_height);
			comma.GetInt(ref args.type);

			// 다시 복원
			//size = ::MulDiv(-lf->lfHeight, 72, ::GetDeviceCaps(hdc, LOGPIXELSY));
			lf.lfHeight = (int)((-font_height*(Double)72.0/96.0)+0.5);

			ObjectGeneral general = new ObjectGeneral();
			general.sClassName = "Date1";

			parent.AddObject(new ObjectDate(ocp, r, null, general, lf, args));
		}

		void LoadClock(ObjectCommonProperty ocp, TextReader reader, string command, CommaBlockString comma)
		{
			ObjectArgsClock args = new ObjectArgsClock();

			RECT r = new RECT();
			
			//Color tcolor=Color.Black, bcolor=Color.White;
			LOGFONT lf = new LOGFONT();
			//Tools.MakeDefaultLogFont(ref lf);
			lf.style = FontStyle.Regular;
			long font_height=0;
			
			comma.GetInt(ref r.left);
			comma.GetInt(ref r.top);
			comma.GetInt(ref r.right);
			comma.GetInt(ref r.bottom);
			comma.GetColor(ref args.textColor);
			comma.GetColor(ref args.backColor.basic_color);
			comma.GetString(ref lf.lfFaceName);
			comma.GetLong(ref font_height);
			comma.GetInt(ref args.type);

			// 다시 복원
			//size = ::MulDiv(-lf->lfHeight, 72, ::GetDeviceCaps(hdc, LOGPIXELSY));
			lf.lfHeight = (int)((-font_height*(Double)72.0/96.0)+0.5);

			ObjectGeneral general = new ObjectGeneral();
			general.sClassName = "Clock1";

			parent.AddObject(new ObjectClock(ocp, r, null, general, lf, args));
		}

		void LoadObjectControlCheckBox(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				ObjectArgsControlCheckBox args = new ObjectArgsControlCheckBox();

				args.sTag = load.sTagName;
				args.rgbColor = load.lTextColor;

				CommaBlockString comma = new CommaBlockString();
				comma.Set(load.sStringOption);
				comma.GetString(ref args.sTitle);

				parent.AddObject(new ObjectControlCheckBox(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args));
			}
		}

		void LoadObjectControlComboBox(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				ObjectArgsControlComboBox args = new ObjectArgsControlComboBox();

				args.dwWindowStyle = load.dwWindowStyle;
				args.sTag = load.sTagName;
				args.arrayListData = load.blockListData;

				CommaBlockString comma = new CommaBlockString();
				comma.Set(load.sStringOption);
				comma.GetInt(ref args.nValueConvert);

				parent.AddObject(new ObjectControlComboBox(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args));
			}
		}

		void LoadObjectControlEditBox(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				ObjectArgsControlEditBox args = new ObjectArgsControlEditBox();

				args.dwWindowStyle = load.dwWindowStyle;
				args.sTag = load.sTagName;

				parent.AddObject(new ObjectControlEditBox(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args));
			}
		}

		void LoadObjectControlListBox(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				ObjectArgsControlListBox args = new ObjectArgsControlListBox();

				args.dwWindowStyle = load.dwWindowStyle;
				args.sTag = load.sTagName;
				args.arrayListData = load.blockListData;

				CommaBlockString comma = new CommaBlockString();
				comma.Set(load.sStringOption);
				comma.GetInt(ref args.nValueConvert);

				parent.AddObject(new ObjectControlListBox(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args));
			}
		}

		void LoadObjectControlRadioButton(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				ObjectArgsControlRadioButton args = new ObjectArgsControlRadioButton();

				args.sTag = load.sTagName;
				args.rgbColor = load.lTextColor;
				args.arrayListData = load.blockListData;

				CommaBlockString comma = new CommaBlockString();
				comma.Set(load.sStringOption);

				parent.AddObject(new ObjectControlRadioButton(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args));
			}
		}

		void LoadObjectModule(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				ObjectArgsModule args = new ObjectArgsModule();

				args.filename = load.sFileName;

				parent.AddObject(new ObjectModule(ocp, form, load.rRect, load.eID, load.objGeneral, 
					args));
			}
		}

		void LoadUnknownObject(ObjectCommonProperty ocp, TextReader reader, string command, CommaBlockString comma)
		{
			string begin="";

			comma.GetString(ref begin);

			if(String.Compare(begin, "BEGIN") == 0)  
			{
				LoadObjectFromMod load = new LoadObjectFromMod();

				if(load.run(reader, command)) 
				{
					if(load.bReadedTagRectFlag)
						parent.AddObject(new ObjectUnknown(ocp, command, load.rTagRect, load.eID, load.objGeneral));
					else
						parent.AddObject(new ObjectUnknown(ocp,command, load.rRect, load.eID, load.objGeneral));
				}
			}
			else 
			{
				//AddObject(new ObjectUnknown(sharedData, nTerminal, command, load.rRect, load.eID, load.sClassName));
			}
		}

		void LoadObjectDatabase(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				ObjectArgsDatabase args = new ObjectArgsDatabase();

				//args.dwWindowStyle = load.dwWindowStyle;
				//args.sTag = load.sTagName;
				args.dsn = load.sFileName;
				args.filename = load.sFileName;
				args.lColorBack = load.lBackColor;
				args.lColorText = load.lTextColor;

				CommaBlockString comma = new CommaBlockString();

				comma.Set(load.sStringOption);
				comma.GetString(ref args.table);
				comma.GetChar(ref args.bUseNo);
				comma.GetInt(ref args.nConnectionType);
				comma.GetChar(ref args.bAutoUpdate);
				int reserved = 0;
				comma.GetInt(ref reserved); // args.bDeleteMenu);
				comma.GetChar(ref args.bUseFullCursor);
				comma.GetInt(ref args.nUpdateTime);

				parent.AddObject(new ObjectDatabase(ocp, form, load.rRect, load.eID, load.objGeneral, load.fontStruct,
					args));
			}
		}

		void LoadObjectWindowAlarm(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
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
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
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
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
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
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				ObjectArgsRealTimeTestGraph args = new ObjectArgsRealTimeTestGraph();

				args.lColorBack.basic_color = Color.LightGray;
				args.lColorFill.basic_color = load.lBackColor.basic_color;
				args.lColorGuideLine = load.lGuideLineColor;
				args.lColorText = load.lTextColor;
				args.pub.wPointSize = load.wGraphPointSize;
				args.wShowUnit = load.nShowUnit;
				args.pub.wDisplayFlags = (EnumDisplayFlag)load.wFlags;
				args.pub.nLevelDisplaySize = load.nLevelDisplaySize;
				args.wLevelDevide = load.wLevelDevide;
				args.wTimeDevide = load.wTimeDevide;

                TWO_TAG_MEMBER_LIST list = new TWO_TAG_MEMBER_LIST();

				CommaBlockString comma = new CommaBlockString();
				comma.Set(load.sStringOption);
				comma.GetInt(ref args.nDataTime);
				comma.GetString(ref args.sTagStart);
				comma.GetColor(ref list.color);
				comma.GetInt(ref list.nLineThick);
				comma.GetInt(ref list.nPointType);
				comma.GetInt(ref args.nTimeDisplayType);
				comma.GetChar(ref args.bDisableCursor);
				comma.GetString(ref args.sTagRun);

				if(load.objGeneral.sClassName.Length == 0)
					load.objGeneral.sClassName = load.sFileName;

                list.arrayMember = load.blockTwoTagMember;

                ArrayList array = new ArrayList();
                array.Add(list);
				
				parent.AddObject(new ObjectRealTimeTestGraph(ocp, form, load.rTagRect, load.eID, load.objGeneral, load.fontStruct,
                    args, load.graphMember, array));
			}
		}

		void LoadObjectXYGraph(ObjectCommonProperty ocp, Form form, TextReader reader, string command)
		{
			LoadObjectFromMod load = new LoadObjectFromMod();

			if(load.run(reader, command)) 
			{
				ObjectArgsXYGraph args = new ObjectArgsXYGraph();

				args.bcolor.basic_color = Color.LightGray;
				args.lColorFill.basic_color = load.lBackColor.basic_color;
				args.gcolor = load.lGuideLineColor;
				args.tcolor = load.lTextColor;
				args.point_size = load.wGraphPointSize;
				args.showunit = load.nShowUnit;
				args.wDisplayFlags = (EnumDisplayFlag)load.wFlags;
				args.leveldevide = load.wLevelDevide;
				args.wTimeDevide = load.wTimeDevide;

				CommaBlockString comma = new CommaBlockString();
				comma.Set(load.sStringOption);
                args.nDataTime = comma.GetInt();

				if(load.objGeneral.sClassName.Length == 0)
					load.objGeneral.sClassName = load.sFileName;

				parent.AddObject(new ObjectXYGraph(ocp, form, load.rTagRect, load.eID, load.objGeneral, load.fontStruct,
					args, load.graphMemberXY));
			}
		} 
	}
}
