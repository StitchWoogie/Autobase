using System;
using System.IO;
using System.Drawing;
using System.Collections;
using NetTools;

namespace GraphicModule
{
	/// <summary>
	/// Summary description for SaveObjectItem.
	/// </summary>
	public class SaveObjectItem
	{
		public SaveObjectItem()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        static void SaveBrush(CommaTextWriter writer, BrushPublic pub)
        {
            writer.Write("{0},{1},{2},{3},{4},", pub.basic_color.R, pub.basic_color.G, pub.basic_color.B, pub.basic_color.A, pub.brush_type);

            if (pub.brush_type == 2)
            {
                BrushLinearGradient brush = (BrushLinearGradient)pub;
                string members = "";

                for (int i = 0; i < brush.stops.Count; i++)
                {
                    // �⺻�������� ���� 2018-3-23
                    members += string.Format(CultureTool.ciKR, "{0}:{1:X08}:", brush.stops[i].offset, brush.stops[i].color.ToArgb());
                }

                writer.Write("{0},{1},{2},{3},{4},{5},{6},", brush.spread_method, brush.stops.Count, members, brush.pStart.X, brush.pStart.Y, brush.pEnd.X, brush.pEnd.Y);
            }

            writer.WriteLine();
        }
		
		public static void TextColor(CommaTextWriter writer, Color color)
		{
			writer.WriteLine("\tTextColor,{0},{1},{2},{3},", color.R, color.G, color.B, color.A);
		}

        public static void BackColor(CommaTextWriter writer, BrushPublic pub)
		{
            writer.Write("\tBackColor,");
            SaveBrush(writer, pub);
            writer.WriteLine();
			//writer.WriteLine("\tBackColor,{0},{1},{2},{3},", color.R, color.G, color.B, color.A);
		}

		public static void LineColor(CommaTextWriter writer, Color color)
		{
			writer.WriteLine("\tLineColor,{0},{1},{2},{3},", color.R, color.G, color.B, color.A);
		}

        /*
		public static void FillColor(CommaTextWriter writer, Color color)
		{
			writer.WriteLine("\tFillColor,{0},{1},{2},{3},", color.R, color.G, color.B, color.A);
		}*/

        public static void FillColor(CommaTextWriter writer, BrushPublic pub)
        {
            writer.Write("\tFillColor,");
            SaveBrush(writer, pub);
            writer.WriteLine();
        }

		public static void GuideLineColor(CommaTextWriter writer, Color color)
		{
			writer.WriteLine("\tGuideLineColor,{0},{1},{2},{3},", color.R, color.G, color.B, color.A);
		}

        public static void CursorColor(CommaTextWriter writer, Color color)
        {
            writer.WriteLine("\tCursorColor,{0},{1},{2},{3},", color.R, color.G, color.B, color.A);
        }

		public static void OnColor(CommaTextWriter writer, Color color)
		{
			writer.WriteLine("\tOnColor,{0},{1},{2},{3},", color.R, color.G, color.B, color.A);
		}

		public static void OffColor(CommaTextWriter writer, Color color)
		{
			writer.WriteLine("\tOffColor,{0},{1},{2},{3},", color.R, color.G, color.B, color.A);
		}

		public static void TagName(CommaTextWriter writer, string tag)
		{
			writer.WriteLine("\tTagName,{0},", tag);
		}
		
		public static void Rect(CommaTextWriter writer, int x1, int y1, int x2, int y2)
		{
			writer.WriteLine("\tRect,{0},{1},{2},{3},", x1, y1, x2, y2);
		}

		public static void LocalMethod(CommaTextWriter writer, int method)
		{
			writer.WriteLine("\tLocalMethod,{0},", method);
		}

		public static void FileName(CommaTextWriter writer, string filename)
		{
			writer.WriteLine("\tFileName,{0},", filename);
		}

		public static void StartAngle(CommaTextWriter writer, int angle)
		{
			writer.WriteLine("\tStartAngle,{0},", angle);
		}

		public static void EndAngle(CommaTextWriter writer, int angle)
		{
			writer.WriteLine("\tEndAngle,{0},", angle);
		}

		public static void AngleDirection(CommaTextWriter writer, int dir)
		{
			writer.WriteLine("\tAngleDirection,{0},", dir);
		}

		/*
		public static void TagMouseResponse(TextWriter writer, int res)
		{
			writer.WriteLine("\tTagMouseResponse,{0},", res);
		}
		*/

		public static void BackBox(CommaTextWriter writer, int option)
		{
			writer.WriteLine("\tBackBox,{0},", option);		
		}

		public static void OverlayMethod(CommaTextWriter writer, int option, int rotate_flip)
		{
			writer.WriteLine("\tOverlayMethod,{0},{1},", option, rotate_flip);
		}

		public static void LineOption(CommaTextWriter writer, int option)
		{
			writer.WriteLine("\tLineOption,{0},", option);		
		}

        // 10.0.2 이전버전은 FillOption을 사용했다. 0이나 1 둘중의 하나만 존재하므로 0이 아니면 모두 1이다.
		public static void FillOption(CommaTextWriter writer, int option)
		{
			writer.WriteLine("\tFillOption,{0},", option == 0 ? 0 : 1); // 
		}

		public static void LineThick(CommaTextWriter writer, int option)
		{
			writer.WriteLine("\tLineThick,{0},", option);
		}

		public static void ButtonDesignType(CommaTextWriter writer, int designType)
		{
			writer.WriteLine("\tButtonDesignType,{0},", designType);
		}

		public static void ButtonRadius(CommaTextWriter writer, int radius)
		{
			writer.WriteLine("\tButtonRadius,{0},", radius);
		}

		public static void String(CommaTextWriter writer, string str)
		{
			writer.WriteLine("\tString,{0},", str);
		}

		public static void TimeSelectOption(CommaTextWriter writer, int option)
		{
			writer.WriteLine("\twTimeSelectOption,{0},", option);		
		}

		public static void GraphMember(CommaTextWriter writer, ArrayList block)
		{
			int l;
			ANALOG_GRAPH_MEMBER member;

			for(l = 0; l < block.Count; l++) 
			{
				member = (ANALOG_GRAPH_MEMBER)block[l];
				writer.Write("\tGraphMember,{0},{1},{2},{3},{4},{5},{6},{7},", 
					member.tag, 
					member.color.R, member.color.G, member.color.B, member.color.A,
					member.nValueType,
					member.nPointType,
					member.nLineThick);

				writer.Write("{0},", member.nAxisPosition);
				writer.Write("{0},", member.nLevelFrom);
				writer.Write("{0},", member.nLevelTo);
				writer.Write("{0},", member.nTagDisplaySize);
				writer.Write("{0},", member.bReverseY);
				writer.Write("{0:X04},", member.wFlags);
                writer.Write("{0},", member.nTimeShift);
                writer.Write("{0},", member.nGraphType);
                writer.Write("{0},", member.wFlags);
				writer.WriteLine("");
			}
		}

		public static void DbTrendMember(CommaTextWriter writer, ArrayList block)
		{
			int l;
			DB_TREND_MEMBER member;

			for(l = 0; l < block.Count; l++) 
			{
				member = (DB_TREND_MEMBER)block[l];
				writer.Write("\tDbTrendMember,{0},{1},{2},{3},{4},{5},{6},{7},{8},", 
					member.tag, member.column,
					member.color.R, member.color.G, member.color.B, member.color.A,
					member.nValueType,
					member.nPointType,
					member.nLineThick);

				writer.Write("{0},", member.nAxisPosition);
				writer.Write("{0},", member.nLevelFrom);
				writer.Write("{0},", member.nLevelTo);
				writer.Write("{0},", member.nTagDisplaySize);
				writer.Write("{0},", member.nReverseY);
				writer.Write("{0:X04},", member.wFlags);
				writer.Write("{0},", member.nGraphType);
				writer.Write("{0},", member.sTable);
				writer.Write("{0},", member.sWhereString);
				writer.Write("{0},", member.sDescription);
				writer.WriteLine("");
			}
		}

        public static void MdTrendMember(CommaTextWriter writer, ArrayList block)
        {
            int l;
            MD_TREND_MEMBER member;

            for (l = 0; l < block.Count; l++)
            {
                member = (MD_TREND_MEMBER)block[l];
                writer.Write("\tMdTrendMember,{0},{1},{2},{3},{4},{5},{6},{7},{8},",
                    member.tag, member.column,
                    member.color.R, member.color.G, member.color.B, member.color.A,
                    member.nValueType,
                    member.nPointType,
                    member.nLineThick);

                writer.Write("{0},", member.nAxisPosition);
                writer.Write("{0},", member.nLevelFrom);
                writer.Write("{0},", member.nLevelTo);
                writer.Write("{0},", member.nTagDisplaySize);
                writer.Write("{0},", member.nReverseY);
                writer.Write("{0:X04},", member.wFlags);
                writer.Write("{0},", member.nGraphType);
                writer.Write("{0},", "");//member.sTable);
                writer.Write("{0},", "");//member.sWhereString);
                writer.Write("{0},", member.sDescription);
                writer.WriteLine("");
            }
        }

		public static void GraphMemberXY(CommaTextWriter writer, ArrayList block)
		{
			int l;
			XY_GRAPH_MEMBER member;

			for(l = 0; l < block.Count; l++) 
			{
				member = (XY_GRAPH_MEMBER)block[l];
				writer.Write("\tGraphMemberXY,{0},{1},{2},{3},{4},{5},{6},{7},", 
					member.tagX, member.tagY,
					member.color.R, member.color.G, member.color.B,
					0,//member.cValueType,
					member.cPointType,
					member.cLineThick);
				writer.Write("{0},", member.cAxisPositionX);
				writer.Write("{0},", member.nLevelFromX);
				writer.Write("{0},", member.nLevelToX);
				writer.Write("{0},", member.cAxisPositionY);
				writer.Write("{0},", member.nLevelFromY);
				writer.Write("{0},", member.nLevelToY);
				writer.Write("{0},", member.nTagDisplaySize);
				writer.Write("{0},", member.bReverseX);
				writer.Write("{0},", member.bReverseY);
                writer.Write("{0},", member.nGraphType);
				writer.WriteLine("");
			}
		}

		public static void GraphPointSize(CommaTextWriter writer, int point_size)
		{
			writer.WriteLine("\twGraphPointSize,{0},", point_size);		
		}

        public static void LevelDisplaySize(CommaTextWriter writer, int size)
        {
            writer.WriteLine("\tLevelDisplaySize,{0},", size);
        }

        // ObjectArgsGraphPublic 인자를 한번에 기록한다. 
        public static void GraphPublicArgs(CommaTextWriter writer, ObjectArgsGraphPublic pub)
        {
            // ObjectArgsGraphPublic 인자를 한번에 기록한다. 10.2.1 부터 추가
            //writer.WriteLine("\tGraphPublicArgs,{0},{1},{2},{3:X08},{4:X08},{5:X08},{6:X08}", (int)pub.wDisplayFlags, pub.wPointSize, pub.nLevelDisplaySize,
            //    pub.colorHiHi.ToArgb(), pub.colorHigh.ToArgb(), pub.colorLow.ToArgb(), pub.colorLoLo.ToArgb());

            //20250204 PSU 패널배경,글자색상 추가
            writer.WriteLine("\tGraphPublicArgs,{0},{1},{2},{3:X08},{4:X08},{5:X08},{6:X08},{7:X08},{8:X08}", (int)pub.wDisplayFlags, pub.wPointSize, pub.nLevelDisplaySize,
                pub.colorHiHi.ToArgb(), pub.colorHigh.ToArgb(), pub.colorLow.ToArgb(), pub.colorLoLo.ToArgb(), pub.colorPanelBack.ToArgb(), pub.colorPanelText.ToArgb()); 

            // 이전 버전의 호환성을 위해서 기록해 준다. Upgrade가 되면 이부분을 삭제하도록 한다.
            writer.WriteLine("\twFlags,{0},", (int)pub.wDisplayFlags);
            writer.WriteLine("\twGraphPointSize,{0},", pub.wPointSize);
            writer.WriteLine("\tLevelDisplaySize,{0},", pub.nLevelDisplaySize);
        }

		public static void TextAlign(CommaTextWriter writer, TEXT_ALIGN align)
		{
			writer.WriteLine("\tTextAlign,{0},{1},", align.x, align.y);
		}

		public static void ClassName(CommaTextWriter writer, ObjectGeneral general)
		{
            int flag_lock = 0;
            int flag_show = 1;
            
            if (AutoLib.ConfigStudio.bSaveLayerLockStatus)  // 
            {
                flag_lock = general.bOnStudioLocked ? 1 : 0;
            }
            if (AutoLib.ConfigStudio.bSaveLayerShowStatus)
            {
                flag_show = general.bOnStudioVisible ? 1 : 0;
            }

			writer.WriteLine("\tClassName,{0},{1},{2},{3},{4},{5},{6}", general.sClassName, general.bUseToolTip, general.sObjectDescription, general.bResponseOnVisible ? 1 : 0, 
                                    general.sOnStudioTitle, flag_lock, flag_show);
            

            if (general.fRotateAngle != 0)  // 회전이 적용된 경우만 저장한다.
            {
                writer.WriteLine("\tRotation,{0},", general.fRotateAngle);
            }
		}

		public static void WindowStyle(CommaTextWriter writer, EnumWindowStyleFlags style)
		{
			writer.WriteLine("\tWindowStyle,{0:X08},", (ulong)style);
		}


        // 2018-6-20 지원
        public static void LogarithmicScale(CommaTextWriter writer, LogarithmicScale ls)
        {
            if (!ls.bUse) return;   // 사용하지 않으면 파일이 커지는 것을 방지하기 위해 저장하지 않는다.

            writer.WriteLine("\tLogarithmicScale,{0},{1},", ls.bUse, ls.fBase);
        }
	}
}

