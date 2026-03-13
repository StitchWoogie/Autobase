using System;
using NetTools;
using System.Drawing;
using System.Windows.Forms;
using ReportBasicLib;

namespace ReportModule
{
	/// <summary>
	/// Summary description for SelectedCell.
	/// </summary>
	public class SelectedCell
	{
		public SelectedCell()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static string sTempCellText;

		static void EnumProcSetCellText(CELL_STRUCT cell)
		{
			cell.text = sTempCellText;
		}

		delegate void CallBackSelectedCell(CELL_STRUCT cell);

		public static void SelectedCellSetText(REPORT_STRUCT report, string text)
		{
			sTempCellText = text;
			EnumSelectedCell(report, new CallBackSelectedCell(EnumProcSetCellText));
		}

		static sbyte cTempCellTextFlag;

		static void EnumProcGetCellText(CELL_STRUCT cell)
		{
			if(cTempCellTextFlag == 2)	return;
			if(cTempCellTextFlag == 0) 
			{
				sTempCellText = cell.text;
				cTempCellTextFlag = 1;
			}
			if(cell.text != sTempCellText) 
			{
				cTempCellTextFlag = 2;
				sTempCellText = "";
			}
		}

		public static void SelectedCellGetText(REPORT_STRUCT report, ref string text)
		{
			sTempCellText = "";
			cTempCellTextFlag = 0;
			EnumSelectedCell(report, new CallBackSelectedCell(EnumProcGetCellText));
			text = sTempCellText;
		}

		static void EnumProcSetCellCommand(CELL_STRUCT cell)
		{
			if(cell.text.Length > 0 && cell.text[0] == '=') 
			{
				CommaBlockString comma = new CommaBlockString();
				string buf = "";

				comma.Set(cell.text);
				comma.GetString(ref buf);	// =command;
				comma.GetStringTotalRemain(ref buf);	// etc
				cell.text = String.Format("={0},{1}", sTempCellText, buf);
			}
			else 
			{
				cell.text = String.Format("={0},", sTempCellText);
			}
		}

		public static void SelectedCellSetCommand(REPORT_STRUCT report, string command)
		{
			sTempCellText = command;
			EnumSelectedCell(report, new CallBackSelectedCell(EnumProcSetCellCommand));
		}

        public static void EnumProcGetCellTag(CELL_STRUCT cell)
        {
            if (cell.text.Length > 0 && cell.text[0] == '=')
            {
                CommaBlockString comma = new CommaBlockString();
                string command = "";

                comma.Set(cell.text);
                comma.GetString(ref command);	// =command;
                EnumCommand id = ReportLib.ChangeCommandStringToId(command.Substring(1));

                switch (id)
                {
                    case EnumCommand.COMMAND_AI_CURR:
                        {
                            OBJECT_AI_CURR obj = new OBJECT_AI_CURR();
                            ReportLib.ObjectStringToStruct(ref obj, cell.text);
                            sTempCellText = obj.tag;
                            break;
                        }
                    case EnumCommand.COMMAND_AI_AVE:
                    case EnumCommand.COMMAND_AI_MIN:
                    case EnumCommand.COMMAND_AI_MAX:
                    case EnumCommand.COMMAND_AI_SUM:
                    case EnumCommand.COMMAND_AI_SUB:
                    case EnumCommand.COMMAND_AI_MIN_TIME:
                    case EnumCommand.COMMAND_AI_MAX_TIME:
                        {
                            OBJECT_AI_ONE_DATA obj = new OBJECT_AI_ONE_DATA();
                            ReportLib.ObjectStringToStruct(ref obj, cell.text);
                            sTempCellText = obj.tag;
                            break;
                        }
                    case EnumCommand.COMMAND_AI_MOMENT:
                    case EnumCommand.COMMAND_AI_MULTI_MOMENT:
                    case EnumCommand.COMMAND_DI_MOMENT:
                    case EnumCommand.COMMAND_DI_MULTI_MOMENT:
                        {
                            OBJECT_MOMENT_DATA obj = new OBJECT_MOMENT_DATA();
                            ReportLib.ObjectStringToStruct(ref obj, cell.text);
                            sTempCellText = obj.tag;
                            break;
                        }
                    case EnumCommand.COMMAND_AI_MAX_SUM:
                    case EnumCommand.COMMAND_AI_MULTI_MAX_SUM:
                        {
                            OBJECT_AI_MAX_SUM obj = new OBJECT_AI_MAX_SUM();
                            ReportLib.ObjectStringToStruct(ref obj, cell.text);
                            sTempCellText = obj.tag;
                            break;
                        }
                    case EnumCommand.COMMAND_AI_MULTI_AVE:
                    case EnumCommand.COMMAND_AI_MULTI_MIN:
                    case EnumCommand.COMMAND_AI_MULTI_MAX:
                    case EnumCommand.COMMAND_AI_MULTI_SUM:
                    case EnumCommand.COMMAND_AI_MULTI_SUB:
                        {
                            OBJECT_AI_MULTI_DATA obj = new OBJECT_AI_MULTI_DATA();
                            ReportLib.ObjectStringToStruct(ref obj, cell.text);
                            sTempCellText = obj.tag;
                            break;
                        }
                    case EnumCommand.COMMAND_AI_ALARM:
                    case EnumCommand.COMMAND_DI_ALARM:
                        {
                            OBJECT_ALARM obj = new OBJECT_ALARM();
                            ReportLib.ObjectStringToStruct(ref obj, cell.text);
                            sTempCellText = obj.tag;
                            break;
                        }
                    case EnumCommand.COMMAND_DI_CURR:
                        {
                            OBJECT_DI_CURR obj = new OBJECT_DI_CURR();
                            ReportLib.ObjectStringToStruct(ref obj, cell.text);
                            sTempCellText = obj.tag;
                            
                            break;
                        }
                    case EnumCommand.COMMAND_DI_ONTIME:
                    case EnumCommand.COMMAND_DI_OFFTIME:
                    case EnumCommand.COMMAND_DI_ONCOUNT:
                        {
                            OBJECT_DI_ONE_DATA obj = new OBJECT_DI_ONE_DATA();
                            ReportLib.ObjectStringToStruct(ref obj, cell.text);
                            sTempCellText = obj.tag;
                            break;
                        }
                    case EnumCommand.COMMAND_DI_MULTI_ONTIME:
                    case EnumCommand.COMMAND_DI_MULTI_OFFTIME:
                    case EnumCommand.COMMAND_DI_MULTI_ONCOUNT:
                        {
                            OBJECT_DI_MULTI_DATA obj = new OBJECT_DI_MULTI_DATA();
                            ReportLib.ObjectStringToStruct(ref obj, cell.text);
                            sTempCellText = obj.tag;
                            break;
                        }
                }
            }
        }

        public static void SelectedCellGetTag(REPORT_STRUCT report, ref string text)
        {
            sTempCellText = "";
            EnumSelectedCell(report, new CallBackSelectedCell(EnumProcGetCellTag));
            text = sTempCellText;
        }

		static void EnumProcSetCellTag(CELL_STRUCT cell)
		{
			if(cell.text.Length > 0 && cell.text[0] == '=') 
			{
				CommaBlockString comma = new CommaBlockString();
				string command = "";

				comma.Set(cell.text);
				comma.GetString(ref command);	// =command;
				EnumCommand id = ReportLib.ChangeCommandStringToId(command.Substring(1));
		
				switch(id) 
				{
					case EnumCommand.COMMAND_AI_CURR:
					{
						OBJECT_AI_CURR obj = new OBJECT_AI_CURR();
						ReportLib.ObjectStringToStruct(ref obj, cell.text);
						obj.tag = sTempCellText;
						ReportLib.ObjectStructToString(ref cell.text, obj);
						break;
					}
					case EnumCommand.COMMAND_AI_AVE:
					case EnumCommand.COMMAND_AI_MIN:
					case EnumCommand.COMMAND_AI_MAX:
					case EnumCommand.COMMAND_AI_SUM:
					case EnumCommand.COMMAND_AI_SUB:
					case EnumCommand.COMMAND_AI_MIN_TIME:
					case EnumCommand.COMMAND_AI_MAX_TIME:

					{
						OBJECT_AI_ONE_DATA obj = new OBJECT_AI_ONE_DATA();
						ReportLib.ObjectStringToStruct(ref obj, cell.text);
						obj.tag = sTempCellText;
						ReportLib.ObjectStructToString(ref cell.text, obj);
						break;
					}
					case EnumCommand.COMMAND_AI_MOMENT:
					case EnumCommand.COMMAND_AI_MULTI_MOMENT:
					case EnumCommand.COMMAND_DI_MOMENT:
					case EnumCommand.COMMAND_DI_MULTI_MOMENT:
					{
						OBJECT_MOMENT_DATA obj = new OBJECT_MOMENT_DATA();
						ReportLib.ObjectStringToStruct(ref obj, cell.text);
						obj.tag = sTempCellText;
						ReportLib.ObjectStructToString(ref cell.text, obj);
						break;
					}
					case EnumCommand.COMMAND_AI_MAX_SUM:
					case EnumCommand.COMMAND_AI_MULTI_MAX_SUM:
					{
						OBJECT_AI_MAX_SUM obj = new OBJECT_AI_MAX_SUM();
						ReportLib.ObjectStringToStruct(ref obj, cell.text);
						obj.tag = sTempCellText;
						ReportLib.ObjectStructToString(ref cell.text, obj);
						break;
					}
					case EnumCommand.COMMAND_AI_MULTI_AVE:
					case EnumCommand.COMMAND_AI_MULTI_MIN:
					case EnumCommand.COMMAND_AI_MULTI_MAX:
					case EnumCommand.COMMAND_AI_MULTI_SUM:
					case EnumCommand.COMMAND_AI_MULTI_SUB:
					{
						OBJECT_AI_MULTI_DATA obj = new OBJECT_AI_MULTI_DATA();
						ReportLib.ObjectStringToStruct(ref obj, cell.text);
						obj.tag = sTempCellText;
						ReportLib.ObjectStructToString(ref cell.text, obj);
						break;
					}
					case EnumCommand.COMMAND_AI_ALARM:
					case EnumCommand.COMMAND_DI_ALARM:
					{
						OBJECT_ALARM obj = new OBJECT_ALARM();
						ReportLib.ObjectStringToStruct(ref obj, cell.text);
						obj.tag = sTempCellText;
						ReportLib.ObjectStructToString(ref cell.text, obj);
						break;
					}
					case EnumCommand.COMMAND_DI_CURR:
					{
						OBJECT_DI_CURR obj = new OBJECT_DI_CURR();
						ReportLib.ObjectStringToStruct(ref obj, cell.text);
						obj.tag = sTempCellText;
						ReportLib.ObjectStructToString(ref cell.text, obj);
						break;
					}
					case EnumCommand.COMMAND_DI_ONTIME:
					case EnumCommand.COMMAND_DI_OFFTIME:
					case EnumCommand.COMMAND_DI_ONCOUNT:
					{
						OBJECT_DI_ONE_DATA obj = new OBJECT_DI_ONE_DATA();
						ReportLib.ObjectStringToStruct(ref obj, cell.text);
						obj.tag = sTempCellText;
						ReportLib.ObjectStructToString(ref cell.text, obj);
						break;
					}
					case EnumCommand.COMMAND_DI_MULTI_ONTIME:
					case EnumCommand.COMMAND_DI_MULTI_OFFTIME:
					case EnumCommand.COMMAND_DI_MULTI_ONCOUNT:
					{
						OBJECT_DI_MULTI_DATA obj = new OBJECT_DI_MULTI_DATA();
						ReportLib.ObjectStringToStruct(ref obj, cell.text);
						obj.tag = sTempCellText;
						ReportLib.ObjectStructToString(ref cell.text, obj);
						break;
					}
				}
			}
			else 
			{
				//sprintf(cell.text, "=AutoData,COMMAND,{0},", sTempCellText);
			}
		}

		public static void SelectedCellSetTag(REPORT_STRUCT report, string tag)
		{
			sTempCellText = tag;
			EnumSelectedCell(report, new CallBackSelectedCell(EnumProcSetCellTag));
		}

		static CELL_TIME tempCellTime=new CELL_TIME();

		static void CellTimeCopy(CELL_TIME target, CELL_TIME source)
		{
			target.bToDataTime = source.bToDataTime;
			target.from = source.from;
			target.shift_from = source.shift_from;
			target.shift_to = source.shift_to;
			target.to = source.to;
			target.zone = source.zone;
		}

		static void ChangeCellTime(CELL_TIME time)
		{
			CellTimeCopy(time, tempCellTime);
		}

		static void EnumProcSetCellTime(CELL_STRUCT cell)
		{
			if(cell.text.Length > 0 && cell.text[0] == '=') 
			{
				CommaBlockString comma = new CommaBlockString();
				string command = "";
				EnumCommand  id;

				comma.Set(cell.text);
				comma.GetString(ref command);	// =command;
				id = ReportLib.ChangeCommandStringToId(command.Substring(1));
				switch(id) 
				{
					case EnumCommand.COMMAND_AI_AVE:
					case EnumCommand.COMMAND_AI_MAX:
					case EnumCommand.COMMAND_AI_MIN:
					case EnumCommand.COMMAND_AI_SUM:
					case EnumCommand.COMMAND_AI_SUB:
					case EnumCommand.COMMAND_AI_MIN_TIME:
					case EnumCommand.COMMAND_AI_MAX_TIME:
					{
						OBJECT_AI_ONE_DATA obj = new OBJECT_AI_ONE_DATA();
						ReportLib.ObjectStringToStruct(ref obj, cell.text);
						ChangeCellTime(obj.time);
						ReportLib.ObjectStructToString(ref cell.text, obj);
					}
						break;
					case EnumCommand.COMMAND_AI_MOMENT:
					case EnumCommand.COMMAND_AI_MULTI_MOMENT:
					case EnumCommand.COMMAND_DI_MOMENT:
					case EnumCommand.COMMAND_DI_MULTI_MOMENT:
					{
						OBJECT_MOMENT_DATA obj = new OBJECT_MOMENT_DATA();
						ReportLib.ObjectStringToStruct(ref obj, cell.text);
						ChangeCellTime(obj.time);
						ReportLib.ObjectStructToString(ref cell.text, obj);
						break;
					}
					case EnumCommand.COMMAND_AI_MAX_SUM:
					case EnumCommand.COMMAND_AI_MULTI_MAX_SUM:
					{
						OBJECT_AI_MAX_SUM obj = new OBJECT_AI_MAX_SUM();
						ReportLib.ObjectStringToStruct(ref obj, cell.text);
						ChangeCellTime(obj.time);
						ReportLib.ObjectStructToString(ref cell.text, obj);
						break;
					}
					case EnumCommand.COMMAND_AI_MULTI_AVE:
					case EnumCommand.COMMAND_AI_MULTI_MAX:
					case EnumCommand.COMMAND_AI_MULTI_MIN:
					case EnumCommand.COMMAND_AI_MULTI_SUM:
					case EnumCommand.COMMAND_AI_MULTI_SUB:
					{
						OBJECT_AI_MULTI_DATA obj = new OBJECT_AI_MULTI_DATA();
						ReportLib.ObjectStringToStruct(ref obj, cell.text);
						ChangeCellTime(obj.time);
						ReportLib.ObjectStructToString(ref cell.text, obj);
					}
						break;
					case EnumCommand.COMMAND_AI_ALARM:
					case EnumCommand.COMMAND_DI_ALARM:
					{
						OBJECT_ALARM obj = new OBJECT_ALARM();
						ReportLib.ObjectStringToStruct(ref obj, cell.text);
						ChangeCellTime(obj.time);
						ReportLib.ObjectStructToString(ref cell.text, obj);
					}
						break;
					case EnumCommand.COMMAND_DI_ONTIME:
					case EnumCommand.COMMAND_DI_OFFTIME:
					case EnumCommand.COMMAND_DI_ONCOUNT:
					{
						OBJECT_DI_ONE_DATA obj = new OBJECT_DI_ONE_DATA();
						ReportLib.ObjectStringToStruct(ref obj, cell.text);
						ChangeCellTime(obj.time);
						ReportLib.ObjectStructToString(ref cell.text, obj);
					}
						break;
					case EnumCommand.COMMAND_DI_MULTI_ONTIME:
					case EnumCommand.COMMAND_DI_MULTI_OFFTIME:
					case EnumCommand.COMMAND_DI_MULTI_ONCOUNT:
					{
						OBJECT_DI_MULTI_DATA obj = new OBJECT_DI_MULTI_DATA();
						ReportLib.ObjectStringToStruct(ref obj, cell.text);
						ChangeCellTime(obj.time);
						ReportLib.ObjectStructToString(ref cell.text, obj);
					}
						break;
					case EnumCommand.COMMAND_DI_ONOFF_LIST:
					{
						OBJECT_DI_ONOFF_LIST obj = new OBJECT_DI_ONOFF_LIST();
						ReportLib.ObjectStringToStruct(ref obj, cell.text);
						ChangeCellTime(obj.time);
						ReportLib.ObjectStructToString(ref cell.text, obj);
					}
						break;
					case EnumCommand.COMMAND_ETC_MULTI_COUNT:
					{
						OBJECT_ETC_MULTI_COUNT obj = new OBJECT_ETC_MULTI_COUNT();
						ReportLib.ObjectStringToStruct(ref obj, cell.text);
						ChangeCellTime(obj.time);
						ReportLib.ObjectStructToString(ref cell.text, obj);
					}
						break;
				}
			}
			else 
			{
		
			}
		}

		public static void SelectedCellSetTime(REPORT_STRUCT report, CELL_TIME time)
		{
			CellTimeCopy(tempCellTime, time);
	
			EnumSelectedCell(report, new CallBackSelectedCell(EnumProcSetCellTime));
		}

		static sbyte cTempCellAlign;
		static sbyte cTempCellAlignHorz;
		static sbyte cTempCellAlignVert;

		static void EnumProcSetCellAlignHorz(CELL_STRUCT cell)
		{
			cell.cAlignHorz = cTempCellAlign;
		}

		public static void SelectedCellSetAlignHorz(REPORT_STRUCT report, sbyte align)
		{
			cTempCellAlign = align;
			EnumSelectedCell(report, new CallBackSelectedCell(EnumProcSetCellAlignHorz));
		}

		static void EnumProcSetCellAlign(CELL_STRUCT cell)
		{
			cell.cAlignHorz = cTempCellAlignHorz;
			cell.cAlignVert = cTempCellAlignVert;
		}

		public static void SelectedCellSetAlign(REPORT_STRUCT report, sbyte horz, sbyte vert)
		{
			cTempCellAlignHorz = horz;
			cTempCellAlignVert = vert;
			EnumSelectedCell(report, new CallBackSelectedCell(EnumProcSetCellAlign));
		}

		//static int nTempCellWidth;
		//static int nTempCellHeight;

		static bool bEnumSelectedFirst = false;

		static DISPLAY_FORMAT_STRUCT tempDisplayFormat;

		static void EnumProcSetFormat(CELL_STRUCT cell)
		{
			cell.format = tempDisplayFormat;
		}

		public static void SelectedCellSetFormat(REPORT_STRUCT report, DISPLAY_FORMAT_STRUCT format)
		{
			tempDisplayFormat = format;
			EnumSelectedCell(report, new CallBackSelectedCell(EnumProcSetFormat));
		}

		static int nEnumSelectedCellX1;	// enum child 에서 영역을 필요로 하는 곳도 있다.
		static int nEnumSelectedCellY1;
		static int nEnumSelectedCellX2;
		static int nEnumSelectedCellY2;

		static bool IsEnumSelectedFirst()
		{
			return bEnumSelectedFirst;
		}

		static void EnumSelectedCell(REPORT_STRUCT report, CallBackSelectedCell lpfnCallBackSelectCell)
		{
			if(report.TableCount == 0)						return;
			if(report.cursor_table >= report.TableCount)		return;

			int x1, y1, x2, y2;
			ReportLib.GetCursorZone(report, out x1, out y1, out x2, out y2);

			nEnumSelectedCellX1 = x1;
			nEnumSelectedCellY1 = y1;
			nEnumSelectedCellX2 = x2;
			nEnumSelectedCellY2 = y2;

			TABLE_STRUCT table;
			int x, y;

			table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];

			CELL_STRUCT cell;

			bEnumSelectedFirst = true;
			for(y = y1; y <= y2; y++) 
			{
				for(x = x1; x <= x2; x++) 
				{
					cell = (CELL_STRUCT)table.cellBuf[y*table.cell_x+x];
					if(cell.cGroup != 2) 
					{
						lpfnCallBackSelectCell(cell);
						bEnumSelectedFirst = false;
					}
				}
			}
		}

		static Color tempColorChange;

		static void EnumProcSetTextColor(CELL_STRUCT cell)
		{
			cell.tcolor = tempColorChange;
		}

		public static void SelectedCellSetTextColor(REPORT_STRUCT report, Color color)
		{
			tempColorChange = color;
			EnumSelectedCell(report, new CallBackSelectedCell(EnumProcSetTextColor));
		}

		static void EnumProcSetBackColor(CELL_STRUCT cell)
		{
			cell.bcolor = tempColorChange;
		}

		public static void SelectedCellSetBackColor(REPORT_STRUCT report, Color color)
		{
			tempColorChange = color;
			EnumSelectedCell(report, new CallBackSelectedCell(EnumProcSetBackColor));
		}

		static BORDER_STRUCT[] tempBorder = new BORDER_STRUCT[8];
		static TABLE_STRUCT tempTable;

		public static void SelectedCellSetBorder(REPORT_STRUCT report, BORDER_STRUCT[] border)
		{
			if(report.cursor_table >= report.tableBuf.Count)	return;

			tempBorder = border;
			tempTable = (TABLE_STRUCT)report.tableBuf[report.cursor_table];
			EnumSelectedCell(report, new CallBackSelectedCell(EnumProcSetCellBorder));
		}

		static void EnumProcSetCellBorder(CELL_STRUCT cell)
		{
			CELL_STRUCT side_cell;

			// left setting
			if(cell.x == nEnumSelectedCellX1) 
			{
				cell.border[0] = tempBorder[3];
				if(cell.x != 0) 
				{	// 선택되지 않은 side cell도 SET 한다.
					side_cell = (CELL_STRUCT)tempTable.cellBuf[cell.y*tempTable.cell_x+(cell.x-1)];
					side_cell.border[2] = tempBorder[3];
				}
			}
			else
				cell.border[0] = tempBorder[4];

			// top setting
			if(cell.y == nEnumSelectedCellY1) 
			{
				cell.border[1] = tempBorder[0];
				if(cell.y != 0) 
				{	// 선택되지 않은 side cell도 SET 한다.
					side_cell = (CELL_STRUCT)tempTable.cellBuf[(cell.y-1)*tempTable.cell_x+cell.x];
					side_cell.border[3] = tempBorder[0];
				}
			}
			else
				cell.border[1] = tempBorder[1];

			// right setting
			if(cell.x == nEnumSelectedCellX2) 
			{
				cell.border[2] = tempBorder[5];
				if(cell.x < tempTable.cell_x-1) 
				{	// 선택되지 않은 side cell도 SET 한다.
					side_cell = (CELL_STRUCT)tempTable.cellBuf[cell.y*tempTable.cell_x+(cell.x+1)];
					side_cell.border[0] = tempBorder[5];
				}
			}
			else
				cell.border[2] = tempBorder[4];

			// bottom setting
			if(cell.y == nEnumSelectedCellY2) 
			{
				cell.border[3] = tempBorder[2];
				if(cell.y < tempTable.cell_y-1) 
				{	// 선택되지 않은 side cell도 SET 한다.
					side_cell = (CELL_STRUCT)tempTable.cellBuf[(cell.y+1)*tempTable.cell_x+cell.x];
					side_cell.border[1] = tempBorder[2];
				}
			}
			else
				cell.border[3] = tempBorder[1];

			// slash setting
			cell.border[4] = tempBorder[6];
			// non slash setting
			cell.border[5] = tempBorder[7];
		}

		static void BorderRegister(BORDER_STRUCT s, ref BORDER_STRUCT t)
		{
			if(t.thick == -1) 
			{	// first set
				t.thick = s.thick;	
			}
			else if(t.thick == -2) 
			{	// multi select

			}
			else 
			{
				if(t.thick != s.thick)	t.thick = -2;
			}

			if(t.colorCalc == -1) 
			{	// first set
				t.color = s.color;	
				t.colorCalc = 0;
			}
			else if(t.colorCalc == -2) 
			{	// multi select

			}
			else 
			{
				if(t.color != s.color) 
				{
					t.colorCalc = -2;
				}
			}

			if(t.type == -1) 
			{	// first set
				t.type = s.type;	
			}
			else if(t.type == -2) 
			{	// multi select

			}
			else 
			{
				if(t.type != s.type)	t.type = -2;
			}
		}

		static void EnumProcGetCellBorder(CELL_STRUCT cell)
		{
			// left setting
			if(cell.x == nEnumSelectedCellX1) 
				BorderRegister(cell.border[0], ref tempBorder[3]);
			else
				BorderRegister(cell.border[0], ref tempBorder[4]);

			// top setting
			if(cell.y == nEnumSelectedCellY1) 
				BorderRegister(cell.border[1], ref tempBorder[0]);
			else
				BorderRegister(cell.border[1], ref tempBorder[1]);

			// right setting
			if(cell.x == nEnumSelectedCellX2) 
				BorderRegister(cell.border[2], ref tempBorder[5]);
			else
				BorderRegister(cell.border[2], ref tempBorder[4]);

			// bottom setting
			if(cell.y == nEnumSelectedCellY2) 
				BorderRegister(cell.border[3], ref tempBorder[2]);
			else
				BorderRegister(cell.border[3], ref tempBorder[1]);

			// slash setting
			BorderRegister(cell.border[4], ref tempBorder[6]);
			// non slash setting
			BorderRegister(cell.border[5], ref tempBorder[7]);
		}

		public static BORDER_STRUCT[] SelectedCellGetBorder(REPORT_STRUCT report)
		{
			int i;
	
			for(i = 0; i < 8; i++) 
			{
				tempBorder[i].colorCalc = -1;
				tempBorder[i].thick = -1;
				tempBorder[i].type  = -1;
			}

			EnumSelectedCell(report, new CallBackSelectedCell(EnumProcGetCellBorder));

			for(i = 0; i < 8; i++) 
			{
				if(tempBorder[i].colorCalc < 0)	tempBorder[i].color = Color.Black;
				if(tempBorder[i].thick < 0)	tempBorder[i].thick = 1;
				if(tempBorder[i].type  < 0)	tempBorder[i].type  = 1;
			}

			return tempBorder;
		}

		public static Font SelectedCellGetFont(REPORT_STRUCT report)
		{
			tempFontName = "Gulim";
			tempFontSize = 9;
			tempFontStyle = FontStyle.Regular;

			EnumSelectedCell(report, new CallBackSelectedCell(EnumProcGetCellFont));

			return new Font(tempFontName, tempFontSize, tempFontStyle);
		}

		static string tempFontName;
		static FontStyle tempFontStyle;
		static float tempFontSize;

		static void EnumProcGetCellFont(CELL_STRUCT cell)
		{
			if(IsEnumSelectedFirst()) 
			{
				tempFontName  = cell.fontName;
				tempFontStyle = cell.fontStyle;
				tempFontSize  = cell.fontSize;
			}
		}

		public static void SelectedCellSetFont(REPORT_STRUCT report, Font font)
		{
			tempFontName = font.Name;
			tempFontSize = font.Size;
			tempFontStyle = font.Style;
			EnumSelectedCell(report, new CallBackSelectedCell(EnumProcSetCellFont));
		}

		static void EnumProcSetCellFont(CELL_STRUCT cell)
		{
			cell.fontName  = tempFontName;
			cell.fontStyle = tempFontStyle;
			cell.fontSize  = tempFontSize;
		}
	}
}


