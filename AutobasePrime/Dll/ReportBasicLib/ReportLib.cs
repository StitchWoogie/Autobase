using System;
using System.Drawing;
//using NetTools.OldDefine;
using System.Collections;
using NetTools;
using AutoLibLocal;
using System.IO;

namespace ReportBasicLib
{
	/// <summary>
	/// Summary description for ReportLib.
	/// </summary>
	public class ReportLib
	{
		public static COMMAND_LIST_STRUCT[] commandList = new COMMAND_LIST_STRUCT[MAX_COMMAND_LIST];

		const int MAX_COMMAND_LIST = 39;
		static int nInsertPos = 0;

		static void AddList(string str, string command, EnumCommand id, sbyte data_type)
		{
			if(nInsertPos >= MAX_COMMAND_LIST)	return;		// over
			commandList[nInsertPos].str = str;
			commandList[nInsertPos].command = command;
			commandList[nInsertPos].id = id;
			commandList[nInsertPos].data_type = data_type;
			nInsertPos++;
		}

		static ReportLib()
		{
			//
			// TODO: Add constructor logic here
			//

			// 아날로그 종류
			if(Tools.IsLangKorean()) 
			{
				AddList("AI 현재값",					"AiCurr",		EnumCommand.COMMAND_AI_CURR, 0 );
				AddList("평균값",						"AiAve",		EnumCommand.COMMAND_AI_AVE, 0 );
				AddList("최대값",						"AiMax",		EnumCommand.COMMAND_AI_MAX, 0 );
				AddList("최소값",						"AiMin",		EnumCommand.COMMAND_AI_MIN, 0 );
				AddList("적산값",						"AiSum",		EnumCommand.COMMAND_AI_SUM, 0 );
				AddList("최대값 차이",					"AiSub",		EnumCommand.COMMAND_AI_SUB, 0 );
				AddList("최대값 더하기",				"AiMaxSum",		EnumCommand.COMMAND_AI_MAX_SUM, 0 );
				AddList("AI 순시값",					"AiMoment",		EnumCommand.COMMAND_AI_MOMENT, 0 );
				AddList("여러줄 평균값",				"AiMultiAve",	EnumCommand.COMMAND_AI_MULTI_AVE, 0 );
				AddList("여러줄 최대값",				"AiMultiMax",	EnumCommand.COMMAND_AI_MULTI_MAX, 0 );
				AddList("여러줄 최소값",				"AiMultiMin",	EnumCommand.COMMAND_AI_MULTI_MIN, 0 );
				AddList("여러줄 적산값",				"AiMultiSum",	EnumCommand.COMMAND_AI_MULTI_SUM, 0 );
				AddList("여러줄 최대값 차이",			"AiMultiSub",	EnumCommand.COMMAND_AI_MULTI_SUB, 0 );
				AddList("여러줄 AI 순시값",			"AiMultiMoment",EnumCommand.COMMAND_AI_MULTI_MOMENT, 0 );
				AddList("여러줄 최대값 더하기",		"AiMultiMaxSum",EnumCommand.COMMAND_AI_MULTI_MAX_SUM, 0 );
				AddList("AI 경보",					"AiAlarm",		EnumCommand.COMMAND_AI_ALARM, 0 );
				AddList("AI 기간자료",				"AiMinList",	EnumCommand.COMMAND_AI_MIN_LIST, 0 );
				AddList("최대값 발생 시점",			"AiMaxTime",	EnumCommand.COMMAND_AI_MAX_TIME, 0 );
				AddList("최소값 발생 시점",			"AiMinTime",	EnumCommand.COMMAND_AI_MIN_TIME, 0 );

				AddList("DI 현재값",					"DiCurr",		EnumCommand.COMMAND_DI_CURR,	1 );
				AddList("ON시간",						"DiOnTime",		EnumCommand.COMMAND_DI_ONTIME,	1 );
				AddList("OFF시간",					"DiOffTime",	EnumCommand.COMMAND_DI_OFFTIME,	1 );
				AddList("ON횟수",						"DiOnCount",	EnumCommand.COMMAND_DI_ONCOUNT,	1 );
				AddList("DI 순시값",					"DiMoment",		EnumCommand.COMMAND_DI_MOMENT,	1 );
				AddList("여러줄 ON시간",				"DiMultiOnTime",EnumCommand.COMMAND_DI_MULTI_ONTIME,	1 );
				AddList("여러줄 OFF시간",				"DiMultiOffTime",EnumCommand.COMMAND_DI_MULTI_OFFTIME,	1 );
				AddList("여러줄 ON횟수",				"DiMultiOnCount",	EnumCommand.COMMAND_DI_MULTI_ONCOUNT,	1 );
				AddList("여러줄 DI 순시값",			"DiMultiMoment",	EnumCommand.COMMAND_DI_MULTI_MOMENT,	1 );
				AddList("DI 경보",					"DiAlarm",		EnumCommand.COMMAND_DI_ALARM, 1 );
				AddList("ON/OFF 리스트",				"DiOnOffList",	EnumCommand.COMMAND_DI_ONOFF_LIST,	1 );
				AddList("ON/OFF 리스트 가동시간 합산",	"DiOnOffListSum",	EnumCommand.COMMAND_DI_ONOFF_LIST_SUM,	1 );
				AddList("여러줄 ON/OFF 리스트 가동시간 합산",	"DiMultiOnOffListSum",	EnumCommand.COMMAND_DI_MULTI_ONOFF_LIST_SUM,	1 );

				AddList("데이터 시간",				"EtcDataTime",	EnumCommand.COMMAND_ETC_DATA_TIME, 2 );
				AddList("현재 시간",					"EtcTime",		EnumCommand.COMMAND_ETC_TIME, 2 );
				AddList("여러줄 순서",				"EtcMultiCount",EnumCommand.COMMAND_ETC_MULTI_COUNT, 2 );
				AddList("기간자료 순서",				"EtcMinList",	EnumCommand.COMMAND_ETC_MIN_LIST, 2 );
				AddList("데이터베이스",				"EtcDataBase",	EnumCommand.COMMAND_ETC_DATABASE, 2 );
				AddList("문자열변수",					"EtcStringVar",	EnumCommand.COMMAND_ETC_STRING_VAR, 2 );
                AddList("ST 현재값", "StCurr", EnumCommand.COMMAND_ST_CURR, 2);
			}
			else if(Tools.IsLangChinese())
			{
				AddList("AI 现在值",					"AiCurr",		EnumCommand.COMMAND_AI_CURR, 0 );
				AddList("平均值",						"AiAve",		EnumCommand.COMMAND_AI_AVE, 0 );
				AddList("最大值",						"AiMax",		EnumCommand.COMMAND_AI_MAX, 0 );
				AddList("最小值",						"AiMin",		EnumCommand.COMMAND_AI_MIN, 0 );
				AddList("累计值",						"AiSum",		EnumCommand.COMMAND_AI_SUM, 0 );
				AddList("最大值的差异",				"AiSub",		EnumCommand.COMMAND_AI_SUB, 0 );
				AddList("最大值求和",				"AiMaxSum",		EnumCommand.COMMAND_AI_MAX_SUM, 0 );
				AddList("AI 瞬时值",					"AiMoment",		EnumCommand.COMMAND_AI_MOMENT, 0 );
				AddList("多行的平均值",				"AiMultiAve",	EnumCommand.COMMAND_AI_MULTI_AVE, 0 );
				AddList("多行的最大值",				"AiMultiMax",	EnumCommand.COMMAND_AI_MULTI_MAX, 0 );
				AddList("多行的最小值",				"AiMultiMin",	EnumCommand.COMMAND_AI_MULTI_MIN, 0 );
				AddList("多行的累计值",				"AiMultiSum",	EnumCommand.COMMAND_AI_MULTI_SUM, 0 );
				AddList("多行的最大值差异",			"AiMultiSub",	EnumCommand.COMMAND_AI_MULTI_SUB, 0 );
				AddList("多行的 AI 瞬时值",			"AiMultiMoment",EnumCommand.COMMAND_AI_MULTI_MOMENT, 0 );
				AddList("多行的最大值求和",		"AiMultiMaxSum",EnumCommand.COMMAND_AI_MULTI_MAX_SUM, 0 );
				AddList("AI 警报",					"AiAlarm",		EnumCommand.COMMAND_AI_ALARM, 0 );
				AddList("AI 期间资料",				"AiMinList",	EnumCommand.COMMAND_AI_MIN_LIST, 0 );
				AddList("最大值的发生时点",			"AiMaxTime",	EnumCommand.COMMAND_AI_MAX_TIME, 0 );
				AddList("最小值的发生时点",			"AiMinTime",	EnumCommand.COMMAND_AI_MIN_TIME, 0 );

				AddList("DI 现在值",					"DiCurr",		EnumCommand.COMMAND_DI_CURR,	1 );
				AddList("ON时间",						"DiOnTime",		EnumCommand.COMMAND_DI_ONTIME,	1 );
				AddList("OFF时间",					"DiOffTime",	EnumCommand.COMMAND_DI_OFFTIME,	1 );
				AddList("ON次数",						"DiOnCount",	EnumCommand.COMMAND_DI_ONCOUNT,	1 );
				AddList("DI 瞬时值",					"DiMoment",		EnumCommand.COMMAND_DI_MOMENT,	1 );
				AddList("多行的ON时间",				"DiMultiOnTime",EnumCommand.COMMAND_DI_MULTI_ONTIME,	1 );
				AddList("多行的OFF时间",				"DiMultiOffTime",EnumCommand.COMMAND_DI_MULTI_OFFTIME,	1 );
				AddList("多行的ON次数",				"DiMultiOnCount",	EnumCommand.COMMAND_DI_MULTI_ONCOUNT,	1 );
				AddList("多行的DI 瞬时值",			"DiMultiMoment",	EnumCommand.COMMAND_DI_MULTI_MOMENT,	1 );
				AddList("DI 警报",					"DiAlarm",		EnumCommand.COMMAND_DI_ALARM, 1 );
				AddList("ON/OFF 列表",				"DiOnOffList",	EnumCommand.COMMAND_DI_ONOFF_LIST,	1 );
				AddList("合算 ON/OFF 列表中的启动时间",	"DiOnOffListSum",	EnumCommand.COMMAND_DI_ONOFF_LIST_SUM,	1 );
				AddList("多行的 合算 ON/OFF 列表中的启动时间",	"DiMultiOnOffListSum",	EnumCommand.COMMAND_DI_MULTI_ONOFF_LIST_SUM,	1 );
 
				AddList("数据时间",				"EtcDataTime",	EnumCommand.COMMAND_ETC_DATA_TIME, 2 );
				AddList("现在时间",					"EtcTime",		EnumCommand.COMMAND_ETC_TIME, 2 );
				AddList("多行顺序",				"EtcMultiCount",EnumCommand.COMMAND_ETC_MULTI_COUNT, 2 );
				AddList("期间资料顺序",				"EtcMinList",	EnumCommand.COMMAND_ETC_MIN_LIST, 2 );
				AddList("数据库",				"EtcDataBase",	EnumCommand.COMMAND_ETC_DATABASE, 2 );
				AddList("字符串变量",					"EtcStringVar",	EnumCommand.COMMAND_ETC_STRING_VAR, 2 );
                AddList("ST 现在值", "StCurr", EnumCommand.COMMAND_ST_CURR, 2);
			}
			else 
			{
				// 아날로그 종류
				AddList("AI Current",					"AiCurr",		EnumCommand.COMMAND_AI_CURR, 0 );
				AddList("AI Ave",	  					"AiAve",		EnumCommand.COMMAND_AI_AVE, 0 );
				AddList("AI Max",						"AiMax",		EnumCommand.COMMAND_AI_MAX, 0 );
				AddList("AI Min",						"AiMin",		EnumCommand.COMMAND_AI_MIN, 0 );
				AddList("AI Sum",						"AiSum",		EnumCommand.COMMAND_AI_SUM, 0 );
				AddList("AI Sub",					"AiSub",		EnumCommand.COMMAND_AI_SUB, 0 );
				AddList("AI Max Add",				"AiMaxSum",		EnumCommand.COMMAND_AI_MAX_SUM, 0 );
				AddList("AI Instantaneous",			"AiMoment",		EnumCommand.COMMAND_AI_MOMENT, 0 );
				AddList("AI Multi Ave",				"AiMultiAve",	EnumCommand.COMMAND_AI_MULTI_AVE, 0 );
				AddList("AI Multi Max",				"AiMultiMax",	EnumCommand.COMMAND_AI_MULTI_MAX, 0 );
				AddList("AI Multi Min",				"AiMultiMin",	EnumCommand.COMMAND_AI_MULTI_MIN, 0 );
				AddList("AI Multi Sum",				"AiMultiSum",	EnumCommand.COMMAND_AI_MULTI_SUM, 0 );
				AddList("AI Multi Sub",				"AiMultiSub",	EnumCommand.COMMAND_AI_MULTI_SUB, 0 );
                AddList("AI Multi Instantaneous", "AiMultiMoment", EnumCommand.COMMAND_AI_MULTI_MOMENT, 0);
				AddList("AI Multi Max Add",			"AiMultiMaxSum",EnumCommand.COMMAND_AI_MULTI_MAX_SUM, 0 );
				AddList("AI Alarm",					"AiAlarm",		EnumCommand.COMMAND_AI_ALARM, 0 );
				AddList("AI Period Data",			"AiMinList",	EnumCommand.COMMAND_AI_MIN_LIST, 0 );
				AddList("AI Event Time of Max",		"AiMaxTime",	EnumCommand.COMMAND_AI_MAX_TIME, 0 );
				AddList("AI Event Time of Min",		"AiMinTime",	EnumCommand.COMMAND_AI_MIN_TIME, 0 );

				AddList("DI Current",				"DiCurr",		EnumCommand.COMMAND_DI_CURR,	1 );
				AddList("DI ON Time",				"DiOnTime",		EnumCommand.COMMAND_DI_ONTIME,	1 );
				AddList("DI OFF Time",				"DiOffTime",	EnumCommand.COMMAND_DI_OFFTIME,	1 );
				AddList("DI ON Count",				"DiOnCount",	EnumCommand.COMMAND_DI_ONCOUNT,	1 );
                AddList("DI Instantaneous",         "DiMoment", EnumCommand.COMMAND_DI_MOMENT, 1);
				AddList("DI Multi ON Time",			"DiMultiOnTime",EnumCommand.COMMAND_DI_MULTI_ONTIME,	1 );
				AddList("DI Multi OFF Time",		"DiMultiOffTime",EnumCommand.COMMAND_DI_MULTI_OFFTIME,	1 );
				AddList("DI Multi ON Count",		"DiMultiOnCount",	EnumCommand.COMMAND_DI_MULTI_ONCOUNT,	1 );
                AddList("DI Multi Instantaneous",   "DiMultiMoment", EnumCommand.COMMAND_DI_MULTI_MOMENT, 1);
				AddList("DI Alarm",					"DiAlarm",		EnumCommand.COMMAND_DI_ALARM, 1 );
				AddList("DI ON/OFF List",			"DiOnOffList",	EnumCommand.COMMAND_DI_ONOFF_LIST,	1 );
				AddList("DI ON/OFF List Sum",		"DiOnOffListSum",	EnumCommand.COMMAND_DI_ONOFF_LIST_SUM,	1 );
				AddList("DI Multi ON/OFF List Sum",	"DiMultiOnOffListSum",	EnumCommand.COMMAND_DI_MULTI_ONOFF_LIST_SUM,	1 );

				AddList("Report Data date/time",	"EtcDataTime",	EnumCommand.COMMAND_ETC_DATA_TIME, 2 );
				AddList("Today date/time",			"EtcTime",		EnumCommand.COMMAND_ETC_TIME, 2 );
				AddList("Multi Order",				"EtcMultiCount",EnumCommand.COMMAND_ETC_MULTI_COUNT, 2 );
				AddList("Period Data Order",		"EtcMinList",	EnumCommand.COMMAND_ETC_MIN_LIST, 2 );
				AddList("Database",					"EtcDataBase",	EnumCommand.COMMAND_ETC_DATABASE, 2 );
				AddList("String Vars",				"EtcStringVar",	EnumCommand.COMMAND_ETC_STRING_VAR, 2 );
                AddList("ST current", "StCurr", EnumCommand.COMMAND_ST_CURR, 2);
			}
		}


		public static void FillDefaultDisplayFormat(DISPLAY_FORMAT_STRUCT format)
		{
			format.cType  = 0;		// 1 = 일반
			format.cUnderPoint = 2;	// 소숫점 이하 자릿수.
		}

		static void FillDefaultCell(CELL_STRUCT cell)
		{
			int i;
	
			cell.tcolor = Color.Black;
			cell.bcolor = Color.White;
			cell.width  = ReportConfig.InitCellWidth;
			cell.height = ReportConfig.InitCellHeight;
			cell.cAlignHorz = 1;	// hcenter
			cell.cAlignVert = 1;	// vcenter
	
			FillDefaultDisplayFormat(cell.format);

			cell.fontSize = 9;
			cell.fontName = "Gulim";

			for(i = 0; i < 6; i++) 
			{
				cell.border[i].color = Color.Black;
				cell.border[i].thick = 1;
			}
			for(i = 0; i < 4; i++) 
			{
				cell.border[i].type = 1;
			}
		}

		public static void FillDefaultTable(TABLE_STRUCT table, int sizex, int sizey)
		{
			table.cell_x = sizex;
			table.cell_y = sizey;
			table.gab_left = 0;
			table.gab_top  = 2;

			table.cellBuf = new ArrayList();
			CELL_STRUCT cell;
			int posx;
			int posy;
			int l = 0;

			//

			for(posy = 0; posy < table.cell_y; posy++) 
			{
				for(posx = 0; posx < table.cell_x; posx++, l++) 
				{
					cell = new CELL_STRUCT();
					FillDefaultCell(cell);
					cell.x = posx;
					cell.y = posy;
					table.cellBuf.Add(cell);
				}
			}
		}

		public static REPORT_STRUCT MakeNewReport()
		{
			REPORT_STRUCT report; 

			report = new REPORT_STRUCT();

			report.tableBuf = new ArrayList();
			report.wOpticRate = 100;
			report.lColorPaper = Color.White;
			report.margin_left = 10;
			report.margin_top = 10;
			report.margin_right = 10;
			report.margin_bottom = 10;
			report.margin_header = 10;
			report.margin_footer = 10;
			report.bOrientation = 0;	// portrait
			report.header.height = 100;
			report.header.blockObject = new ArrayList();
			report.footer.height = 100;
			report.footer.blockObject = new ArrayList();
			return report;
		}

		public static CELL_STRUCT GetCellStruct(TABLE_STRUCT table, int x, int y)
		{
            int pos = table.cell_x * y + x;

            if (pos < 0 || pos >= table.cellBuf.Count)
            {
                CELL_STRUCT cell = new CELL_STRUCT();
                FillDefaultCell(cell);
                cell.x = x;
                cell.y = y;
                return cell;
            }
            
			return (CELL_STRUCT)table.cellBuf[pos];
		}

		public static void GetCursorZone(REPORT_STRUCT report, out int x1, out int y1, out int x2, out int y2)
		{
			int minx, miny, maxx, maxy;
			int x, y;

			x1 = report.cursor_x1;
			y1 = report.cursor_y1;
			x2 = report.cursor_x2;
			y2 = report.cursor_y2;

			TABLE_STRUCT table = (TABLE_STRUCT)report.tableBuf[report.cursor_table];
			CELL_STRUCT cell;
			CELL_STRUCT cell2;

			if(x1 >= table.cell_x)	x1 = table.cell_x-1;
			if(x2 >= table.cell_x)	x2 = table.cell_x-1;
			if(y1 >= table.cell_y)	y1 = table.cell_y-1;
			if(y2 >= table.cell_y)	y2 = table.cell_y-1;

			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

			minx = x1;
			miny = y1;
			maxx = x2;
			maxy = y2;

			while(true) 
			{
				for(y = y1; y <= y2; y++) 
				{
					for(x = x1; x <= x2; x++) 
					{
						cell = GetCellStruct(table, x, y);
						if(cell.cGroup == 1) 
						{
							if(cell.nGroupX > maxx)	maxx = cell.nGroupX;
							if(cell.nGroupY > maxy)	maxy = cell.nGroupY;
						}
						else if(cell.cGroup == 2) 
						{
							if(cell.nGroupX < minx)	minx = cell.nGroupX;
							if(cell.nGroupY < miny)	miny = cell.nGroupY;
							cell2 = GetCellStruct(table, cell.nGroupX, cell.nGroupY);
							if(cell2.nGroupX > maxx)	maxx = cell2.nGroupX;
							if(cell2.nGroupY > maxy)	maxy = cell2.nGroupY;
						}
					}
				}

				if(minx == x1 && miny == y1 && maxx == x2 && maxy == y2) 
				{
					break;
				}

				x1 = minx;
				y1 = miny;
				x2 = maxx;
				y2 = maxy;
			}

			//x1 = minx;
			//y1 = miny;
			//x2 = maxx;
			//y2 = maxy;
		}

		public static bool IsCursorZone(REPORT_STRUCT report, int table_no, int x, int y)
		{
			if(table_no != report.cursor_table)	return false;

			int x1, y1, x2, y2;

			GetCursorZone(report, out x1, out y1, out x2, out y2);

			if(x >= x1 && x <= x2 && y >= y1 && y <= y2)	return true;
			else											return false;
		}

		public static EnumCommand ChangeCommandStringToId(string buf)
		{
			int i;
	
			for(i = 0; i < MAX_COMMAND_LIST; i++) 
			{
				if(buf == commandList[i].command) 
				{
					return commandList[i].id;
				}
			}
			return 0;
		}

		public static void ChangeCommandIdToDes(out string str, EnumCommand id)
		{
			int i;
	
			for(i = 0; i < MAX_COMMAND_LIST; i++) 
			{
				if(id == commandList[i].id) 
				{
					str = commandList[i].str;
					return;
				}
			}

			str = String.Format("Unknown id({0})", id);
		}


		public static void ObjectStructToString(ref string str, OBJECT_AI_CURR obj)
		{
			str = String.Format("=AiCurr,{0},", obj.tag);
		}

		public static void ObjectStringToStruct(ref OBJECT_AI_CURR obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();
			string buf="";;
			comma.Set(str);
			comma.GetString(ref buf);					// command
			comma.GetString(ref obj.tag);				// tag
			obj.tag = obj.tag.Trim();
		}

		public static void ObjectStructToString(ref string str, OBJECT_AI_ONE_DATA obj)
		{
			str = String.Format("={0},{1},{2},{3},{4},{5},{6},{7},", obj.command, obj.tag, obj.time.zone, obj.time.from, obj.time.to, obj.time.shift_from, obj.time.shift_to, obj.time.bToDataTime);
		}

		public static void ObjectStringToStruct(ref OBJECT_AI_ONE_DATA obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();
			string buf="";;
			comma.Set(str);
			comma.GetString(ref buf);						// command
			obj.command = buf.Substring(1);
			comma.GetString(ref obj.tag);			// tag
			obj.tag = obj.tag.Trim();
			comma.GetString(ref obj.time.zone);// time_zone
			comma.GetInt(ref obj.time.from);							// from time
			comma.GetInt(ref obj.time.to);								// to time
			comma.GetInt(ref obj.time.shift_from);						// from time
			comma.GetInt(ref obj.time.shift_to);						// to time
			comma.GetChar(ref obj.time.bToDataTime);					// to data time
		}

		public static void ObjectStructToString(ref string str, OBJECT_AI_MULTI_DATA obj)
		{
			str = String.Format("={0},{1},{2},{3},{4},{5},{6},", obj.command, obj.tag, obj.time.zone, obj.time.from, obj.time.to, obj.time.shift_from, obj.time.shift_to);
		}

		public static void ObjectStringToStruct(ref OBJECT_AI_MULTI_DATA obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();
			string buf="";;
			comma.Set(str);
				comma.GetString(ref buf);						// command
			obj.command = buf.Substring(1);
			comma.GetString(ref obj.tag);			// tag
			obj.tag = obj.tag.Trim();
			comma.GetString(ref obj.time.zone);// time_zone
			comma.GetInt(ref obj.time.from);							// from time
			comma.GetInt(ref obj.time.to);								// to time
			comma.GetInt(ref obj.time.shift_from);						// from time
			comma.GetInt(ref obj.time.shift_to);						// to time
		}

		public static void ObjectStructToString(ref string str, OBJECT_AI_MIN_LIST obj)
		{
			str = String.Format("=AiMinList,{0},{1},{2},{3},", obj.tag, obj.data_type, obj.min_gab, obj.cDataUnit);
		}

		public static void ObjectStringToStruct(ref OBJECT_AI_MIN_LIST obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();
			string buf="";;
			comma.Set(str);
			comma.GetString(ref buf);						// command
			comma.GetString(ref obj.tag);			// tag
			obj.tag = obj.tag.Trim();
			comma.GetInt(ref obj.data_type);
			comma.GetString(ref obj.min_gab);
			comma.GetChar(ref obj.cDataUnit);
		}

		public static void ObjectStructToString(ref string str, OBJECT_ALARM obj)
		{
			str = String.Format("={0},{1},{2},{3},{4},{5},{6},{7},", obj.command, obj.tag, obj.view, obj.time.zone, obj.time.from, obj.time.to, obj.time.shift_from, obj.time.shift_to);
		}

		public static void ObjectStringToStruct(ref OBJECT_ALARM obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();
			string buf="";;
			comma.Set(str);
			comma.GetString(ref buf);
			obj.command = buf.Substring(1);
			comma.GetString(ref obj.tag);				// tag
			obj.tag = obj.tag.Trim();
			comma.GetInt(ref obj.view);									// view field
			comma.GetString(ref obj.time.zone);	// time_zone
			comma.GetInt(ref obj.time.from);								// from time
			comma.GetInt(ref obj.time.to);									// to time
			comma.GetInt(ref obj.time.shift_from);							// from time
			comma.GetInt(ref obj.time.shift_to);							// to time
		}

		public static void ObjectStructToString(ref string str, OBJECT_MOMENT_DATA obj)
		{
			str = String.Format("={0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", obj.command, obj.tag, obj.time.zone, obj.time.from, obj.time.to, obj.time.shift_from, obj.time.shift_to, obj.day, obj.hour, obj.min, obj.data_type, obj.nSharpSharpValue);
		}

		public static void ObjectStringToStruct(ref OBJECT_MOMENT_DATA obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();
			string buf="";;
			comma.Set(str);
			comma.GetString(ref buf);						// command
			obj.command = buf.Substring(1);
			comma.GetString(ref obj.tag);			// tag
			obj.tag = obj.tag.Trim();
			comma.GetString(ref obj.time.zone);// time_zone
			comma.GetInt(ref obj.time.from);							// from time
			comma.GetInt(ref obj.time.to);								// to time
			comma.GetInt(ref obj.time.shift_from);						// from time
			comma.GetInt(ref obj.time.shift_to);						// to time
			comma.GetInt(ref obj.day);
			comma.GetInt(ref obj.hour);
			comma.GetInt(ref obj.min);
            comma.GetString(ref obj.data_type);
            
            comma.GetInt(ref obj.nSharpSharpValue);                     // ## value
            if (obj.nSharpSharpValue < 2 || obj.nSharpSharpValue > 30)  // 2분에서 30분까지만 있고 기본은 15분이다.
                obj.nSharpSharpValue = 15;  
		}

		public static void ObjectStructToString(ref string str, OBJECT_AI_MAX_SUM obj)
		{
			str = String.Format("={0},{1},{2},{3},{4},{5},{6},{7},{8},", obj.command, obj.tag, obj.time.zone, obj.time.from, obj.time.to, obj.time.shift_from, obj.time.shift_to, obj.nDataType, obj.time.bToDataTime);
		}

		public static void ObjectStringToStruct(ref OBJECT_AI_MAX_SUM obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();
			string buf="";;
			comma.Set(str);
			comma.GetString(ref buf);						// command
			obj.command = buf.Substring(1);
			comma.GetString(ref obj.tag);			// tag
			obj.tag = obj.tag.Trim();
			comma.GetString(ref obj.time.zone);// time_zone
			comma.GetInt(ref obj.time.from);							// from time
			comma.GetInt(ref obj.time.to);								// to time
			comma.GetInt(ref obj.time.shift_from);						// from time
			comma.GetInt(ref obj.time.shift_to);						// to time
			comma.GetInt(ref obj.nDataType);
			comma.GetChar(ref obj.time.bToDataTime);
		}

		public static void ObjectStructToString(ref string str, OBJECT_DI_CURR obj)
		{
			str = String.Format("=DiCurr,{0},", obj.tag);
		}

		public static void ObjectStringToStruct(ref OBJECT_DI_CURR obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();
			string buf="";;
			comma.Set(str);
			comma.GetString(ref buf);			// command
			comma.GetString(ref obj.tag);			// tag
			obj.tag = obj.tag.Trim();
		}

		public static void ObjectStructToString(ref string str, OBJECT_DI_ONE_DATA obj)
		{
			str = String.Format("={0},{1},{2},{3},{4},{5},{6},{7},", obj.command, obj.tag, obj.time.zone, obj.time.from, obj.time.to, obj.time.shift_from, obj.time.shift_to, obj.time.bToDataTime);
		}

		public static void ObjectStringToStruct(ref OBJECT_DI_ONE_DATA obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();
			string buf="";;
			comma.Set(str);
			comma.GetString(ref buf);						// command
			obj.command = buf.Substring(1);
			comma.GetString(ref obj.tag);			// tag
			obj.tag = obj.tag.Trim();
			comma.GetString(ref obj.time.zone);// time_zone
			comma.GetInt(ref obj.time.from);							// from time
			comma.GetInt(ref obj.time.to);								// to time
			comma.GetInt(ref obj.time.shift_from);						// from time
			comma.GetInt(ref obj.time.shift_to);						// to time
			comma.GetChar(ref obj.time.bToDataTime);					// to data time
		}

		public static void ObjectStructToString(ref string str, OBJECT_DI_MULTI_DATA obj)
		{
			str = String.Format("={0},{1},{2},{3},{4},{5},{6},", obj.command, obj.tag, obj.time.zone, obj.time.from, obj.time.to, obj.time.shift_from, obj.time.shift_to);
		}

		public static void ObjectStringToStruct(ref OBJECT_DI_MULTI_DATA obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();
			string buf="";;
			comma.Set(str);
			comma.GetString(ref buf);							// command
			obj.command = buf.Substring(1);
			comma.GetString(ref obj.tag);				// tag
			obj.tag = obj.tag.Trim();
			comma.GetString(ref obj.time.zone);	// time_zone
			comma.GetInt(ref obj.time.from);								// from time
			comma.GetInt(ref obj.time.to);									// to time
			comma.GetInt(ref obj.time.shift_from);							// from time
			comma.GetInt(ref obj.time.shift_to);							// to time
		}

		public static void ObjectStructToString(ref string str, OBJECT_DI_ONOFF_LIST obj)
		{
			str = String.Format("=DiOnOffList,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},", obj.tag, obj.field_time, obj.field_view, obj.time.zone, obj.time.from, obj.time.to, obj.time.shift_from, obj.time.shift_to, obj.nSort, obj.bUseFromToTime);
		}

		public static void ObjectStringToStruct(ref OBJECT_DI_ONOFF_LIST obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();
			string buf="";;
			comma.Set(str);
			comma.GetString(ref buf);						// command
			comma.GetString(ref obj.tag);			// tag
			obj.tag = obj.tag.Trim();
			comma.GetInt(ref obj.field_time);							// time field pos
			comma.GetInt(ref obj.field_view);							// time field view
			//	comma.GetInt(obj.field_count);							// time field count

			comma.GetString(ref obj.time.zone);// time_zone
			comma.GetInt(ref obj.time.from);			// from time
			comma.GetInt(ref obj.time.to);				// to time
			comma.GetInt(ref obj.time.shift_from);		// from time
			comma.GetInt(ref obj.time.shift_to);		// to time
			comma.GetInt(ref obj.nSort);				// sort method 0 - none, 1 - asc, 2 - desc
			comma.GetChar(ref obj.bUseFromToTime);		// 
		}

		public static void ObjectStructToString(ref string str, OBJECT_DI_ONOFF_LIST_SUM obj)
		{
			str = String.Format("={0},{1},{2},{3},{4},{5},{6},{7},{8}", obj.command, obj.tag, obj.field_time, obj.time.zone, obj.time.from, obj.time.to, obj.time.shift_from, obj.time.shift_to, obj.bUseFromToTime);
		}

		public static void ObjectStringToStruct(ref OBJECT_DI_ONOFF_LIST_SUM obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();

			comma.Set(str);
			comma.GetString(ref obj.command);						// command
			obj.command = obj.command.Substring(1);

			comma.GetString(ref obj.tag);			// tag
			obj.tag = obj.tag.Trim();
			comma.GetInt(ref obj.field_time);							// time field pos
			//comma.GetInt(obj.field_view);							// time field view
			//	comma.GetInt(obj.field_count);							// time field count

			comma.GetString(ref obj.time.zone);// time_zone
			comma.GetInt(ref obj.time.from);			// from time
			comma.GetInt(ref obj.time.to);				// to time
			comma.GetInt(ref obj.time.shift_from);		// from time
			comma.GetInt(ref obj.time.shift_to);		// to time
			//comma.GetInt(obj.nSort);				// sort method 0 - none, 1 - asc, 2 - desc
			comma.GetChar(ref obj.bUseFromToTime);		// 
		}

		/*
		public static void ObjectStructToString(ref string str, OBJECT_DI_MULTI_ONOFF_LIST_SUM obj)
		{
			str = String.Format("=DiMultiOnOffListSum,{0},{1},{2},{3},{4},{5},{6},{7},", obj.tag, obj.field_time, obj.time.zone, obj.time.from, obj.time.to, obj.time.shift_from, obj.time.shift_to, obj.bUseFromToTime);
		}

		public static void ObjectStringToStruct(ref OBJECT_DI_MULTI_ONOFF_LIST_SUM obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();
			string buf="";;
			comma.Set(str);
			comma.GetString(ref buf);						// command
			comma.GetString(ref obj.tag);			// tag
			obj.tag = obj.tag.Trim();
			comma.GetInt(ref obj.field_time);							// time field pos
			//comma.GetInt(obj.field_view);							// time field view
			//	comma.GetInt(obj.field_count);							// time field count

			comma.GetString(ref obj.time.zone);// time_zone
			comma.GetInt(ref obj.time.from);			// from time
			comma.GetInt(ref obj.time.to);				// to time
			comma.GetInt(ref obj.time.shift_from);		// from time
			comma.GetInt(ref obj.time.shift_to);		// to time
			//comma.GetInt(obj.nSort);				// sort method 0 - none, 1 - asc, 2 - desc
			comma.GetChar(ref obj.bUseFromToTime);		// 
		}
		*/

		public static void ObjectStructToString(ref string str, OBJECT_ETC_DATA_TIME obj)
		{
			str = String.Format("=EtcDataTime,");
		}

		public static void ObjectStringToStruct(ref OBJECT_ETC_DATA_TIME obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();
			string buf="";;
			comma.Set(str);
			comma.GetString(ref buf);	// command
			//comma.GetInt(obj.style);
			//comma.GetInt(obj.display);
		}

		public static void ObjectStructToString(ref string str, OBJECT_ETC_TIME obj)
		{
			str = String.Format("=EtcTime,");
		}

		public static void ObjectStringToStruct(ref OBJECT_ETC_TIME obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();
			string buf="";;
			comma.Set(str);
				comma.GetString(ref buf);	// command
			//comma.GetInt(obj.style);
			//comma.GetInt(obj.display);
		}

		public static void ObjectStructToString(ref string str, OBJECT_ETC_MULTI_COUNT obj)
		{
			str = String.Format("=EtcMultiCount,{0},{1},{2},{3},{4},", obj.time.zone, obj.time.from, obj.time.to, obj.time.shift_from, obj.time.shift_to);
		}

		public static void ObjectStringToStruct(ref OBJECT_ETC_MULTI_COUNT obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();
			string buf="";;
			comma.Set(str);
			comma.GetString(ref buf);			// command
			comma.GetString(ref obj.time.zone);// time_zone
			comma.GetInt(ref obj.time.from);	// from time
			comma.GetInt(ref obj.time.to);		// to time
			comma.GetInt(ref obj.time.shift_from);	// from time
			comma.GetInt(ref obj.time.shift_to);		// to time
		}

		public static void ObjectStructToString(ref string str, OBJECT_ETC_MIN_LIST obj)
		{
			str = String.Format("=EtcMinList,{0},{1},{2}", obj.min_gab, obj.cDataUnit, obj.bUseTo);
		}

		public static void ObjectStringToStruct(ref OBJECT_ETC_MIN_LIST obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();
			string buf="";
			comma.Set(str);
				comma.GetString(ref buf);						// command
			comma.GetString(ref obj.min_gab);
			comma.GetChar(ref obj.cDataUnit);

			if(comma.IsEOS()) 
			{
				obj.bUseTo = 1;	
			}
			else 
			{
				comma.GetChar(ref obj.bUseTo);
			}
		}

		public static void ObjectStructToString(ref string str, OBJECT_ETC_DATABASE obj)
		{
			string temp;
			temp = String.Format("=EtcDataBase,{0},{1},{2},{3},{4},", obj.filename, obj.table, obj.field, obj.where, obj.orderby);

			str = temp;
		}

		public static void ObjectStringToStruct(ref OBJECT_ETC_DATABASE obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();
			string buf="";;
			comma.Set(str);
			comma.GetString(ref buf);						// command
			comma.GetString(ref obj.filename);						
			comma.GetString(ref obj.table);						
			comma.GetString(ref obj.field);						
			comma.GetString(ref obj.where);						
			comma.GetString(ref obj.orderby);						
			//comma.GetString(ref obj.min_gab);
			//comma.GetChar(ref obj.cDataUnit);
		}

		public static void ObjectStructToString(ref string str, OBJECT_ETC_STRING_VAR obj)
		{
			string temp;
			temp = String.Format("=EtcStringVar,{0},", obj.var);

			str = temp;
		}

		public static void ObjectStringToStruct(ref OBJECT_ETC_STRING_VAR obj, string str)
		{
			CommaBlockString comma = new CommaBlockString();
			string buf="";;
			comma.Set(str);
			comma.GetString(ref buf);						// command
			comma.GetString(ref obj.var);						
		}

        public static void ObjectStructToString(ref string str, OBJECT_ST_CURR obj)
        {
            str = String.Format("=StCurr,{0},", obj.tag);
        }

        public static void ObjectStringToStruct(ref OBJECT_ST_CURR obj, string str)
        {
            CommaBlockString comma = new CommaBlockString();
            string buf = ""; ;
            comma.Set(str);
            comma.GetString(ref buf);			// command
            comma.GetString(ref obj.tag);			// tag
            obj.tag = obj.tag.Trim();
        }

		public static string[] GetReportListsLocal()
		{
			ArrayList array = new ArrayList();

			string root_path = String.Format("{0}\\Report", TotalConfig.sDirWorkProject);

			if(!Directory.Exists(root_path))	return null;

			DirectoryInfo info = new DirectoryInfo(root_path);

			REPORT_LIST item;

			foreach(FileInfo fi in info.GetFiles("*.rpt?")) 
			{
				item = new REPORT_LIST();
				item.filename = fi.Name;
				item.description = ReportFile.GetReportDescription(fi.FullName);
				
				array.Add(item);
			}

			if(array.Count == 0)	return null;

			string[] val = new string[array.Count*2];
			for(int i = 0; i < array.Count; i++) 
			{
				item = (REPORT_LIST)array[i];
				val[i*2+0] = item.filename;
				val[i*2+1] = item.description;
			}
			return val;
		}
	}

	[Serializable]
	public struct BORDER_STRUCT
	{
		public int			colorCalc;	// 여러셀의 공통된 특성을 얻어오기 위해서 SelectedCellGetBorder에서만 사용하는 변수 (편집기에서만 사용)
		public Color		color;
		public sbyte		type;	// 0 = 없음, 1 = 단일선, 2 = 이중선
		public sbyte		thick;	// default = 1
	}

	[Serializable]
	public struct DISPLAY_FORMAT_STRUCT 
	{
		public sbyte		cType;			// 표시 형식 
		public sbyte		cUnderPoint;	// 소숫점 이하 자릿수.
		public sbyte		bThousandComma;	// 천단위 콤마 구분 
		public sbyte		cDateTime;		// 날짜 시간 구분 
		public sbyte		cTimeCount;		// 소요 시간	
		public sbyte		bWeekAdd;		// 요일을 추가할것인가의 여부.
        public string sUserFormat;    // 사용자 정의 포맷
		//char		extra[9];
	}

	[Serializable]
	public class CELL_STRUCT 
	{
		public int			x;					// table에서 cell좌표
		public int			y;					// table에서 cell좌표
		public Color		tcolor;
		public Color		bcolor;
		public BORDER_STRUCT[] border = new BORDER_STRUCT[6];		// 0 - left, 1 - top, 2 - right, 3 - bottom, 4 - slash, 5 - slash
		public sbyte		cAlignHorz;			// 수평 정렬 0 - Left, 1 = Center, 2 = Right
		public sbyte		cAlignVert;			// 수직 정렬 0 - Top, 1 = Center, 2 = Bottom
		//public LOGFONT		logfont;			// CELL Font
		public string		fontName;
		public float		fontSize;
		public FontStyle	fontStyle;
		public string		text="";			// text
		public int			width;
		public int			height;

		public sbyte		cGroup;				// 셀이 병합 되었는가. 0 = 보통 셀, 1 = 병합 기준셀, 2 = 숨기는 cell
		public int			nGroupX;			// 기준셀에서는 병합 되었을 때 Cell의 마지막 위치이고 숨기는 셀은 기준셀의 이름을 말한다.
		public int			nGroupY;			// 기준셀에서는 병합 되었을 때 Cell의 마지막 위치이고 숨기는 셀은 기준셀의 이름을 말한다.
		public int			wOnRunY1;			// Run Mode 일때 셀이 들어가는 위치 일반적일때는 y1==y2 여러셀을 파생시킬때는 y1!=y2
		public int			wOnRunY2;			// Run Mode 일때 셀이 들어가는 위치
		public sbyte		bCalced;			// 이 셀의 수식은 계산이 완료 되었다.

		public DISPLAY_FORMAT_STRUCT format;	// display format

		//char		extra[33];
	}

	[Serializable]
	public class TABLE_STRUCT
	{
		public int  no;		// table no
		public int cell_x;
		public int cell_y;
		public int gab_left;
		public int gab_top;
		public ArrayList	  cellBuf = new ArrayList();
		//char extra[80];
	}

	public enum DrawType 
	{
		Line = 1,
		Text = 2,
	}

	[Serializable]
	public class REPORT_STRUCT 
	{
		public float fVersionFileMajor;
		public float fVersionFileMinor;
		public string sVersionProgram;
		
		public int margin_left;
		public int margin_top;
		public int margin_right;
		public int margin_bottom;
		public int margin_header;	// 머리글 위치
		public int margin_footer;	// 꼬리글 위치

		public sbyte bOrientation;	// 0 = Portrait, 1 = landscape

		public int wOpticRate;
		public int cursor_table;
		public int cursor_x1;
		public int cursor_y1;
		public int cursor_x2;
		public int cursor_y2;
		//public int nTableCount;
		public int TableCount 
		{
			get 
			{
				return tableBuf.Count;
			}
		}
		public string description = "";
		public Color lColorPaper;
		public ArrayList tableBuf = new ArrayList();
		public HEAD_FOOT_STRUCT header = new HEAD_FOOT_STRUCT();
		public HEAD_FOOT_STRUCT footer = new HEAD_FOOT_STRUCT();
		//char	extra[80];
	}

	[Serializable]
	public class HEAD_FOOT_STRUCT 
	{
		public int		height;
		public uint	count;		// 일반적으로는 쓰지 않고 저장하고 불러오는 부분에서만 쓴다.
		public ArrayList blockObject = new ArrayList();
	}

	[Serializable]
	public class CELL_TIME
	{
		public string zone;
		public int	 from;
		public int  to;
		public int  shift_from;
		public int  shift_to;
		public sbyte bToDataTime;		// 이것이 체크되어 있으면 리포터에서 설정한 시간까지만 계산한다.
	}

	public enum EnumCommand
	{
		COMMAND_UNKNOWN,
		COMMAND_AI_CURR,
		COMMAND_AI_AVE,
		COMMAND_AI_MAX,
		COMMAND_AI_MIN,
		COMMAND_AI_SUM,
		COMMAND_AI_SUB,
		COMMAND_AI_MOMENT,			// 순시값
		COMMAND_AI_MAX_SUM,			// 최대값 더하기
		COMMAND_AI_MULTI_AVE,
		COMMAND_AI_MULTI_MAX,
		COMMAND_AI_MULTI_MIN,
		COMMAND_AI_MULTI_SUM,
		COMMAND_AI_MULTI_SUB,
		COMMAND_AI_MULTI_MOMENT,	// 여러줄 순시값
		COMMAND_AI_MULTI_MAX_SUM,	// 여러줄 최대값 더하기
		COMMAND_AI_ALARM,
		COMMAND_AI_MIN_LIST,
		COMMAND_AI_MAX_TIME,		// 최대값 발생 시점
		COMMAND_AI_MIN_TIME,		// 최소값 발생 시점

		COMMAND_DI_CURR,
		COMMAND_DI_ONTIME,
		COMMAND_DI_OFFTIME,
		COMMAND_DI_ONCOUNT,
		COMMAND_DI_MOMENT,			// 순시값
		COMMAND_DI_MULTI_ONTIME,
		COMMAND_DI_MULTI_OFFTIME,
		COMMAND_DI_MULTI_ONCOUNT,
		COMMAND_DI_MULTI_MOMENT,	// 순시값
		COMMAND_DI_ONOFF_LIST,
		COMMAND_DI_ONOFF_LIST_SUM,	// 가동시간 합산
		COMMAND_DI_MULTI_ONOFF_LIST_SUM,	// 가동시간 합산
		COMMAND_DI_ALARM,

		COMMAND_ETC_DATA_TIME,
		COMMAND_ETC_TIME,
		COMMAND_ETC_MULTI_COUNT,
		COMMAND_ETC_MIN_LIST,
		COMMAND_ETC_DATABASE,
		COMMAND_ETC_STRING_VAR,

        COMMAND_ST_CURR,
	}

	public class OBJECT_AI_CURR
	{
		public string tag;
	}

	public class OBJECT_AI_ONE_DATA
	{
		public string tag;
		public string command;
		public CELL_TIME time = new CELL_TIME();
	}

	public class OBJECT_AI_MULTI_DATA
	{
		public string tag;
		public string command;
		public CELL_TIME time = new CELL_TIME();
	}

	public class OBJECT_ALARM
	{
		public string command;
		public string tag;
		public int	 view;
		public CELL_TIME time = new CELL_TIME();
	}

	public class OBJECT_AI_MIN_LIST
	{
		public string tag;
		public int  data_type;
		public string min_gab;			// 시간 간격
		public sbyte cDataUnit;
	}

	public class OBJECT_MOMENT_DATA
	{
		public string tag;
		public string command;
		public CELL_TIME time = new CELL_TIME();
		public int  day;
		public int  hour;
		public int  min;
        public string data_type = "";
        public int nSharpSharpValue = 15;    // DataType의 ##에 들어가는 값
	}

	public class OBJECT_AI_MAX_SUM
	{
		public string tag;
		public string command;
		public CELL_TIME time = new CELL_TIME();
		public int  nDataType;
	}

	public class OBJECT_DI_CURR
	{
		public string tag;
	}

	public class OBJECT_DI_ONE_DATA
	{
		public string tag;
		public string command;
		public CELL_TIME time = new CELL_TIME();
	}

	public class OBJECT_DI_MULTI_DATA
	{
		public string tag;
		public string command;
		public CELL_TIME time = new CELL_TIME();
	}

	public class OBJECT_DI_ONOFF_LIST
	{
		public string tag;
		public int  field_time;
		public int  field_view;
		public int	 nSort;
		//int  field_count;
		public CELL_TIME time = new CELL_TIME();
		public sbyte bUseFromToTime;	// 기간 자료 시간을 사용한다.
	}

	public class OBJECT_DI_ONOFF_LIST_SUM
	{
		public string command;
		public string tag;
		public int  field_time;
		//int  field_view;
		//int	 nSort;
		//int  field_count;
		public CELL_TIME time = new CELL_TIME();
		public sbyte bUseFromToTime;	// 기간 자료 시간을 사용한다.
	}

	/*
	public class OBJECT_DI_MULTI_ONOFF_LIST_SUM
	{
		public string tag;
		public int  field_time;
		//int  field_view;
		//int	 nSort;
		//int  field_count;
		public CELL_TIME time = new CELL_TIME();
		public sbyte bUseFromToTime;	// 기간 자료 시간을 사용한다.
	}
	*/

	public class OBJECT_ETC_DATA_TIME
	{
		public sbyte ex;
	}

	public class OBJECT_ETC_TIME
	{
		public sbyte ex;
	}

	public class OBJECT_ETC_MULTI_COUNT
	{
		public CELL_TIME time = new CELL_TIME();
	}

	public class OBJECT_ETC_MIN_LIST 
	{
		public string min_gab;			// 시간 간격
		public sbyte cDataUnit;
		public sbyte bUseTo;		// ~ 이후 표시안함
	}

	public class OBJECT_ETC_DATABASE
	{
		public string filename;
		public string table;
		public string field;
		public string where;
		public string orderby;
	}

	public class OBJECT_ETC_STRING_VAR
	{
		public string var;
	}

    public class OBJECT_ST_CURR
    {
        public string tag;
    }

	public struct COMMAND_LIST_STRUCT
	{
		public string str;
		public string command;
		public EnumCommand  id;
		public sbyte data_type;
	}

	[Serializable]
	public class DRAW_ONE_OBJECT
	{
		public EnumDrawType	type;
		public object	p;
	} 

	[Serializable]
	public class DRAW_OBJECT_LINE
	{
		public int x1;
		public int y1;
		public int x2;
		public int y2;
		public int thick;
		public int	style;
		public Color color;
		//char extra[16];
	}

	[Serializable]
	public class DRAW_OBJECT_TEXT
	{
		public int x1;
		public int y1;
		public int x2;
		public int y2;
		public int			align;
		public string		fontName;
		public float		fontSize;
		public FontStyle	fontStyle;
		public string text;
		public Color color;
		//char extra[16];
	}

	public enum EnumDrawType
	{
		LINE = 1,
		TEXT = 2,
	}

	[Serializable]
	public class USER_SELECT_TIME
	{
		public	int  year;
		public	int  mon;
		public	int  day;
		public	int  hour;
		public	int  min;
		public	int  sec;

		public static bool operator==(USER_SELECT_TIME a, USER_SELECT_TIME b)
		{
			if(a.year != b.year)	return false;
			if(a.mon != b.mon)		return false;
			if(a.day != b.day)		return false;
			if(a.hour != b.hour)	return false;
			if(a.min != b.min)		return false;
			if(a.sec != b.sec)		return false;
			return true;
		}

		public static bool operator!=(USER_SELECT_TIME a, USER_SELECT_TIME b)
		{
			if(a.year != b.year)	return true;
			if(a.mon != b.mon)		return true;
			if(a.day != b.day)		return true;
			if(a.hour != b.hour)	return true;
			if(a.min != b.min)		return true;
			if(a.sec != b.sec)		return true;
			return false;
		}

		public override bool Equals(object o)
		{
			return ((USER_SELECT_TIME)o == this);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	
	} 

	[Serializable]
	public class REPORT_LIST
	{
		public string filename;
		public string description;
	}

	public enum EnumHandAuto
	{
		HAND_MODE,
		AUTO_MODE,
	}
}


