using System;
using NetTools;
using System.Collections;
using AutoLibLocal;
using AutoLib;
using System.IO;
using System.Data.OleDb;
using System.Data;
using NetTools.OldDefine;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace ReportBasicLib
{
	/// <summary>
	/// Summary description for MakeRunReport.
	/// </summary>
	public class MakeRunReport
	{
		public MakeRunReport()
		{
			//
			// TODO: Add constructor logic here
			// 
		}

		public async Task<REPORT_STRUCT> MakeByFile(string filename, EnumHandAuto hand_auto)
		{
			REPORT_STRUCT source = ReportFile.ReportLoad(filename, false);

			if(source == null)	return null;

			return await Make(source, hand_auto);
		}

		bool bRecurseStringValueFlag = false;	// 이값이 true이면 스트링 값이다.
		string sRecurseStringValue;
		string no_tag = "No Tag";
		int nRecurseCellY;  // TARGET RECURSE일때만 사용한다. 현재검사 중인 cell_y position
        readonly ConcurrentDictionary<string, ParsedCellCommand> _parsedCellCommandCache =
            new ConcurrentDictionary<string, ParsedCellCommand>();
        readonly ConcurrentDictionary<string, OBJECT_AI_ONE_DATA> _aiOneDataCache =
            new ConcurrentDictionary<string, OBJECT_AI_ONE_DATA>();
        readonly ConcurrentDictionary<string, OBJECT_AI_MULTI_DATA> _aiMultiDataCache =
            new ConcurrentDictionary<string, OBJECT_AI_MULTI_DATA>();
        readonly ConcurrentDictionary<string, OBJECT_MOMENT_DATA> _momentDataCache =
            new ConcurrentDictionary<string, OBJECT_MOMENT_DATA>();
        readonly ConcurrentDictionary<string, OBJECT_AI_MAX_SUM> _aiMaxSumCache =
            new ConcurrentDictionary<string, OBJECT_AI_MAX_SUM>();
        readonly ConcurrentDictionary<string, OBJECT_DI_ONE_DATA> _diOneDataCache =
            new ConcurrentDictionary<string, OBJECT_DI_ONE_DATA>();
        readonly ConcurrentDictionary<string, OBJECT_DI_MULTI_DATA> _diMultiDataCache =
            new ConcurrentDictionary<string, OBJECT_DI_MULTI_DATA>();

        public async Task<REPORT_STRUCT> Make(REPORT_STRUCT source, EnumHandAuto hand_auto)
        {
            REPORT_STRUCT target = (REPORT_STRUCT)Tools.CopyObject(source);

            if (source.TableCount > 0)
            {
                for (int l = 0; l < source.TableCount; l++)
                {
                    TABLE_STRUCT table_s = (TABLE_STRUCT)source.tableBuf[l];
                    TABLE_STRUCT table_t = (TABLE_STRUCT)target.tableBuf[l];

                    table_t.cellBuf = new ArrayList();
                    table_t.cell_y = 0;

                    int target_y = 0;
                    for (int posy = 0; posy < table_s.cell_y; posy++)
                    {
                        int maxy = 1;
                        int m = table_s.cell_x * posy;

                        //  한 행의 셀들을 병렬로 실행
                        var tasks = new List<Task<(int retny, sbyte bCalced)>>();

                        for (int posx = 0; posx < table_s.cell_x; posx++, m++)
                        {
                            CELL_STRUCT cell_s = (CELL_STRUCT)table_s.cellBuf[m];
                            int target_x = posx;

                            tasks.Add(ChangeCommandToString(table_t, table_s, cell_s, target_x, target_y, hand_auto));
                        }

                        var results = await Task.WhenAll(tasks).ConfigureAwait(false); ;

                        // 결과 적용
                        m = table_s.cell_x * posy;
                        for (int posx = 0; posx < table_s.cell_x; posx++, m++)
                        {
                            CELL_STRUCT cell_s = (CELL_STRUCT)table_s.cellBuf[m];
                            var (retny, bCalced) = results[posx];

                            if (retny > maxy) maxy = retny;
                            cell_s.bCalced = bCalced;
                        }

                        // Run Report의 Y 위치 기억
                        m = table_s.cell_x * posy;
                        for (int posx = 0; posx < table_s.cell_x; posx++, m++)
                        {
                            CELL_STRUCT cell_s = (CELL_STRUCT)table_s.cellBuf[m];
                            cell_s.wOnRunY1 = target_y;
                            cell_s.wOnRunY2 = target_y + maxy - 1;

                            for (int y = 0; y < maxy; y++)
                            {
                                CELL_STRUCT cell_t = (CELL_STRUCT)table_t.cellBuf[table_t.cell_x * (target_y + y) + posx];
                                cell_t.bCalced = cell_s.bCalced;
                            }
                        }

                        target_y += maxy;
                    }
                }
            }

            // 나머지 후처리 부분 (계산식 처리 등) 기존 그대로 유지
            if (target.TableCount > 0)
            {
                for (int l = 0; l < target.TableCount; l++)
                {
                    TABLE_STRUCT table_t = (TABLE_STRUCT)target.tableBuf[l];
                    int target_y = 0;
                    for (int m = 0, posy = 0; posy < table_t.cell_y; posy++, target_y++)
                    {
                        for (int target_x = 0, posx = 0; posx < table_t.cell_x; posx++, m++, target_x++)
                        {
                            CELL_STRUCT cell_t = (CELL_STRUCT)table_t.cellBuf[m];
                            if (cell_t.bCalced == 0)
                            {
                                double val = 0;
                                bool retn = GetValueRecurse(target, source, table_t.no, posy, cell_t.text, cell_t.text.Length, ref val);

                                cell_t.bCalced = 1;
                                if (bRecurseStringValueFlag)
                                    cell_t.text = sRecurseStringValue;
                                else
                                    MakeCellString(cell_t, val, retn);
                            }
                        }
                    }
                }
            }

            return target;
        }

        //public async Task<REPORT_STRUCT> Make(REPORT_STRUCT source, EnumHandAuto hand_auto)
        //{
        //	TABLE_STRUCT table_s, table_t;
        //	CELL_STRUCT cell_s, cell_t;
        //	int l, m;
        //	REPORT_STRUCT target;
        //	int posx, posy;
        //	int maxy, retny;
        //	int target_x, target_y;
        //	sbyte bCalced=0;
        //	bool  retn;
        //	double val=0;

        //	target = (REPORT_STRUCT)Tools.CopyObject(source);
        //	//target.tableBuf = new System.Collections.ArrayList();

        //	//HCURSOR hCursorOld = SetCursor(LoadCursor(NULL, IDC_WAIT));

        //	if(source.TableCount > 0)	
        //	{
        //		//target.tableBuf = new TABLE_STRUCT[source.nTableCount];

        //		for(l = 0; l < source.TableCount; l++) 
        //		{
        //			table_s = (TABLE_STRUCT)source.tableBuf[l];

        //			table_t = (TABLE_STRUCT)target.tableBuf[l];
        //			//table_t = (TABLE_STRUCT)Tools.CopyObject(table_s);

        //			table_t.cellBuf = new ArrayList();
        //			//table_t.cell_x = 0;
        //			table_t.cell_y = 0;

        //			target_y = 0;
        //			for(posy = 0; posy < table_s.cell_y; posy++) 
        //			{
        //				maxy = 1;
        //				m = table_s.cell_x*posy;
        //				for(posx = 0; posx < table_s.cell_x; posx++, m++) 
        //				{
        //					cell_s = (CELL_STRUCT)table_s.cellBuf[m];
        //					target_x = posx;
        //					(retny, bCalced) = await ChangeCommandToString(table_t, table_s, cell_s, target_x, target_y, hand_auto);
        //					if(retny > maxy)	maxy = retny;
        //					cell_s.bCalced = bCalced;
        //				}

        //				// Run Report의 Y의 위치를 기억해 둔다.
        //				m = table_s.cell_x*posy;
        //				for(posx = 0; posx < table_s.cell_x; posx++, m++) 
        //				{
        //					cell_s = (CELL_STRUCT)table_s.cellBuf[m];
        //					cell_s.wOnRunY1 = target_y;
        //					cell_s.wOnRunY2 = target_y+maxy-1;

        //					for(int y = 0; y < maxy; y++) 
        //					{
        //						cell_t = (CELL_STRUCT)table_t.cellBuf[table_t.cell_x*(target_y+y)+posx];
        //						cell_t.bCalced = cell_s.bCalced;
        //					}
        //				}
        //				target_y += maxy;
        //			}
        //			//target.tableBuf.Add(table_t);
        //		}
        //	}

        //	// @Line??? 의 라인 함수를 계산하고.
        //	// 평균이나 합 등의 기타 수식을 계산한다.
        //	if(target.TableCount > 0)	
        //	{
        //		for(l = 0; l < target.TableCount; l++) 
        //		{
        //			table_t = (TABLE_STRUCT)target.tableBuf[l];
        //			target_y = 0;
        //			for(m = 0, posy = 0; posy < table_t.cell_y; posy++, target_y++) 
        //			{
        //				for(target_x = 0, posx = 0; posx < table_t.cell_x; posx++, m++, target_x++) 
        //				{
        //					cell_t = (CELL_STRUCT)table_t.cellBuf[m];
        //					if(cell_t.bCalced == 0) 
        //					{
        //						retn = GetValueRecurse(target, source, table_t.no, posy, cell_t.text, cell_t.text.Length, ref val);

        //						cell_t.bCalced = 1;
        //						if(bRecurseStringValueFlag)
        //							cell_t.text = sRecurseStringValue;
        //						else
        //							MakeCellString(cell_t, val, retn);
        //					}
        //				}
        //			}
        //		}
        //	}

        //	//CopyHeadFoot(target, source); 이미 만들어 졌다) CopyObject에서 이미 복사되었다.

        //	//SetCursor(hCursorOld);

        //	return target;
        //}

        async Task<(int, sbyte bCalced)> ChangeCommandToString(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y, EnumHandAuto hand_auto)
		{
			string tag="";

			sbyte bCalced = 1;

			EnumTagType tag_type = 0;
			int[]  tag_pos=new int[1];
			double val;
	
			CELL_STRUCT cell_t;

			string org_tag="";

			if(cell_s.text.Length > 0 && cell_s.text[0] == '=') 
			{
                ParsedCellCommand parsed = GetParsedCellCommand(cell_s.text);
				EnumCommand id = parsed.CommandId;
				if(id == EnumCommand.COMMAND_AI_CURR) 
				{
                    tag = parsed.FirstArgument;
					GetOriginalTag(out org_tag, tag);
					cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);
					if(!TagLib.GetTagTypeAndPos(org_tag, ref tag_type, ref tag_pos)) 
					{
						cell_t.text = ReportConfig.sNoData;
						return (1, bCalced);
					}
					string sval;
					TagLib.GetTagValue(org_tag, out sval, out val);
					MakeNumberByFormat(ref cell_t.text, cell_t.format, val);
					return (1, bCalced);
				}
				else if(id == EnumCommand.COMMAND_AI_AVE) 
				{
					return (await ConvertAiOneData(table_t, table_s, cell_s, target_x, target_y, EnumDataType.AVE, hand_auto).ConfigureAwait(false), bCalced);
				}
				else if(id == EnumCommand.COMMAND_AI_MIN) 
				{
					return (await ConvertAiOneData(table_t, table_s, cell_s, target_x, target_y, EnumDataType.MIN, hand_auto).ConfigureAwait(false), bCalced);
				}
				else if(id == EnumCommand.COMMAND_AI_MAX) 
				{
					return (await ConvertAiOneData(table_t, table_s, cell_s, target_x, target_y, EnumDataType.MAX, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_AI_SUM) 
				{
					return (await ConvertAiOneData(table_t, table_s, cell_s, target_x, target_y, EnumDataType.SUM, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_AI_SUB) 
				{
					return (await ConvertAiOneData(table_t, table_s, cell_s, target_x, target_y, EnumDataType.SUB, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_AI_MOMENT) 
				{
					return (await ConvertAiMomentOneData(table_t, table_s, cell_s, target_x, target_y, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_AI_MAX_SUM) 
				{
					return (await ConvertAiOneMaxSum(table_t, table_s, cell_s, target_x, target_y, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_AI_MULTI_AVE) 
				{
					return (await ConvertAiMultiData(table_t, table_s, cell_s, target_x, target_y, EnumDataType.AVE, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_AI_MULTI_MIN) 
				{
					return (await ConvertAiMultiData(table_t, table_s, cell_s, target_x, target_y, EnumDataType.MIN, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_AI_MULTI_MAX) 
				{
					return (await ConvertAiMultiData(table_t, table_s, cell_s, target_x, target_y, EnumDataType.MAX, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_AI_MULTI_SUM) 
				{
					return (await ConvertAiMultiData(table_t, table_s, cell_s, target_x, target_y, EnumDataType.SUM, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_AI_MULTI_SUB) 
				{
					return (await ConvertAiMultiData(table_t, table_s, cell_s, target_x, target_y, EnumDataType.SUB, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_AI_MULTI_MOMENT) 
				{
					return (await ConvertAiMultiMomentData(table_t, table_s, cell_s, target_x, target_y, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_AI_MULTI_MAX_SUM) 
				{
					return (await ConvertAiMultiMaxSum(table_t, table_s, cell_s, target_x, target_y, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_AI_ALARM) 
				{
					return (ConvertAlarm(table_t, table_s, cell_s, target_x, target_y, hand_auto), bCalced);
                }
				else if(id == EnumCommand.COMMAND_AI_MIN_LIST) 
				{
                    return (await ConvertAiMinList(table_t, table_s, cell_s, target_x, target_y).ConfigureAwait(false), bCalced);
                }

				else if(id == EnumCommand.COMMAND_AI_MIN_TIME) 
				{
					return (await ConvertAiMinMaxTime(table_t, table_s, cell_s, target_x, target_y, EnumDataType.MIN, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_AI_MAX_TIME) 
				{
					return (await ConvertAiMinMaxTime(table_t, table_s, cell_s, target_x, target_y, EnumDataType.MAX, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_DI_CURR) 
				{
					return (ConvertDiCurr(table_t, table_s, cell_s, target_x, target_y), bCalced);
                }
				else if(id == EnumCommand.COMMAND_DI_ONTIME) 
				{
					return (await ConvertDiOneData(table_t, table_s, cell_s, target_x, target_y, EnumDataType.ONTIME, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_DI_OFFTIME) 
				{
					return (await ConvertDiOneData(table_t, table_s, cell_s, target_x, target_y, EnumDataType.OFFTIME, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_DI_ONCOUNT) 
				{
					return (await ConvertDiOneData(table_t, table_s, cell_s, target_x, target_y, EnumDataType.COUNT, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_DI_MOMENT) 
				{
					return (await ConvertDiMomentOneData(table_t, table_s, cell_s, target_x, target_y, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_DI_MULTI_ONTIME) 
				{
					return (await ConvertDiMultiData(table_t, table_s, cell_s, target_x, target_y, EnumDataType.ONTIME, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_DI_MULTI_OFFTIME) 
				{
					return (await ConvertDiMultiData(table_t, table_s, cell_s, target_x, target_y, EnumDataType.OFFTIME, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_DI_MULTI_ONCOUNT) 
				{
					return (await ConvertDiMultiData(table_t, table_s, cell_s, target_x, target_y, EnumDataType.COUNT, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_DI_MULTI_MOMENT) 
				{
					return (await ConvertDiMultiMomentData(table_t, table_s, cell_s, target_x, target_y, hand_auto).ConfigureAwait(false), bCalced);
                }
				else if(id == EnumCommand.COMMAND_DI_ONOFF_LIST) 
				{
					return (ConvertDiOnOffList(table_t, table_s, cell_s, target_x, target_y, hand_auto), bCalced);
                }
				else if(id == EnumCommand.COMMAND_DI_ONOFF_LIST_SUM) 
				{
					return (ConvertDiOnOffListSum(table_t, table_s, cell_s, target_x, target_y, hand_auto), bCalced);
                }
				else if(id == EnumCommand.COMMAND_DI_MULTI_ONOFF_LIST_SUM) 
				{
					return (ConvertDiMultiOnOffListSum(table_t, table_s, cell_s, target_x, target_y, hand_auto), bCalced);
                }
				else if(id == EnumCommand.COMMAND_DI_ALARM) 
				{
					return (ConvertAlarm(table_t, table_s, cell_s, target_x, target_y, hand_auto), bCalced);
                }
				else if(id == EnumCommand.COMMAND_ETC_MULTI_COUNT) 
				{
					return (ConvertMultiCount(table_t, table_s, cell_s, target_x, target_y, hand_auto), bCalced);
                }
				else if(id == EnumCommand.COMMAND_ETC_DATA_TIME) 
				{
					cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);
					ConvertDataTime(cell_t, hand_auto);
					return (1, bCalced);
                }
				else if(id == EnumCommand.COMMAND_ETC_TIME) 
				{
					cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);
					ConvertEtcTime(cell_t);
					return (1, bCalced);
                }
				else if(id == EnumCommand.COMMAND_ETC_MIN_LIST) 
				{
					return (ConvertEtcMinList(table_t, table_s, cell_s, target_x, target_y), bCalced);
                }
				else if(id == EnumCommand.COMMAND_ETC_DATABASE) 
				{
					return (ConvertEtcDatabase(table_t, table_s, cell_s, target_x, target_y, hand_auto), bCalced);
                }
				else if(id == EnumCommand.COMMAND_ETC_STRING_VAR) 
				{
					return (ConvertEtcStringVar(table_t, table_s, cell_s, target_x, target_y), bCalced);
                }
                else if (id == EnumCommand.COMMAND_ST_CURR)
                {
                    return (ConvertStCurr(table_t, table_s, cell_s, target_x, target_y), bCalced);
                }
				else 
				{
					cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);
			
					// 뭔지 잘 모를 때는 계산식이라고 가정하고 다음에 계산한다.
					bCalced = 0;
			
					return (1, bCalced);
				}
			}
			else 
			{
				cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);
				return (1, bCalced);
			}
		}

        ParsedCellCommand GetParsedCellCommand(string text)
        {
            return _parsedCellCommandCache.GetOrAdd(text, ParseCellCommand);
        }

        static ParsedCellCommand ParseCellCommand(string text)
        {
            ParsedCellCommand parsed = new ParsedCellCommand();
            if (string.IsNullOrEmpty(text) || text[0] != '=')
            {
                return parsed;
            }

            CommaBlockString comma = new CommaBlockString();
            string command = "";
            string firstArgument = "";

            comma.Set(text);
            comma.GetString(ref command);
            if (command.Length <= 1)
            {
                return parsed;
            }

            parsed.CommandId = ReportLib.ChangeCommandStringToId(command.Substring(1));
            comma.GetString(ref firstArgument);
            parsed.FirstArgument = firstArgument;
            return parsed;
        }

        sealed class ParsedCellCommand
        {
            public EnumCommand CommandId { get; set; }

            public string FirstArgument { get; set; }
        }

        
        bool GetValueRecurse(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string s, int size, ref double val)
        {
            return GetValueRecurse(rt, rs, cell_table, s, 0, size, ref val);
        }

        bool GetValueRecurse(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string s, int start, int size, ref double val)
        {
            if (size <= 0)
            {
                val = 0.0;
                return true;
            }

            if (s[start] == '=')
            {
                return GetValueRecurse(rt, rs, cell_table, s, start + 1, size - 1, ref val);
            }

            int open1 = 0, open2 = 0;
            int close1 = 0, close2 = 0;
            int pos = size-1;       // 뒤에서 부터 계산해야 () 없이도 정확한 계산이 나온다. 2009-7-21
            double value1 = 0, value2 = 0;

            //--------------------
            // 명령어 @cell(field)
            //--------------------

            while (true)
            {
                char current = s[start + pos];

                if (current == '(')
                {
                    open1++;
                }
                else if (current == ')')
                {
                    close1++;
                }
                else if (current == '[')
                {
                    open2++;
                }
                else if (current == ']')
                {
                    close2++;
                }
                else if (current == '+')
                {
                    if (open1 == close1 && open2 == close2)
                    {
                        if (pos < 1 || pos >= size - 1)
                        {
                            return false;
                        }
                        else
                        {
                            if (!GetValueRecurse(rt, rs, cell_table, s, start, pos, ref value1)) return false;
                            if (!GetValueRecurse(rt, rs, cell_table, s, start + pos + 1, size - pos - 1, ref value2)) return false;
                            val = value1 + value2;
                            return true;
                        }
                    }
                }
                else if (current == '-')
                {
                    if (open1 == close1 && open2 == close2)
                    {
                        if (pos == 0)
                        {
                            if (!GetValueRecurse(rt, rs, cell_table, s, start + pos + 1, size - pos - 1, ref value1)) return false;
                            val = 0 - value1;
                            return true;
                        }
                        else if (pos >= size - 1)
                        {
                            val = 0.0;
                            return true;
                        }
                        else
                        {
                            if (!GetValueRecurse(rt, rs, cell_table, s, start, pos, ref value1)) return false;
                            if (!GetValueRecurse(rt, rs, cell_table, s, start + pos + 1, size - pos - 1, ref value2)) return false;

                            val = value1 - value2;
                            return true;
                        }
                    }
                }
                else if (current == '%')
                {
                    if (open1 == close1 && open2 == close2)
                    {
                        if (pos == 0)
                        {
                            if (!GetValueRecurse(rt, rs, cell_table, s, start + pos + 1, size - pos - 1, ref value1)) return false;
                            val = 0 - value1;
                            return true;
                        }
                        else if (pos >= size - 1)
                        {
                            val = 0.0;
                            return true;
                        }
                        else
                        {
                            if (!GetValueRecurse(rt, rs, cell_table, s, start, pos, ref value1)) return false;
                            if (!GetValueRecurse(rt, rs, cell_table, s, start + pos + 1, size - pos - 1, ref value2)) return false;

                            if (value2 == 0)
                                val = 0;
                            else
                                val = (int)(value1) % (int)(value2);
                            return true;
                        }
                    }
                }
                else { }

                pos--;

                if (pos < 0)
                {
                    break;
                }
            }

            open1 = 0;
            open2 = 0;
            close1 = 0;
            close2 = 0;
            pos = size-1;

            while (true)
            {
                char current = s[start + pos];

                if (current == '(')
                {
                    open1++;
                }
                else if (current == ')')
                {
                    close1++;
                }
                else if (current == '[')
                {
                    open2++;
                }
                else if (current == ']')
                {
                    close2++;
                }
                else if (current == '*')
                {
                    if (open1 == close1 && open2 == close2)
                    {
                        if (pos < 1 || pos >= size - 1)
                        {
                            val = 0.0;
                            return true;
                        }
                        else
                        {
                            if (!GetValueRecurse(rt, rs, cell_table, s, start, pos, ref value1)) return false;
                            if (!GetValueRecurse(rt, rs, cell_table, s, start + pos + 1, size - pos - 1, ref value2)) return false;
                            val = value1 * value2;
                            return true;
                        }
                    }
                }
                else if (current == '/')
                {
                    if (open1 == close1 && open2 == close2)
                    {
                        if (pos < 1 || pos >= size - 1)
                        {
                            val = 0;
                            return true;
                        }
                        else
                        {
                            if (!GetValueRecurse(rt, rs, cell_table, s, start + pos + 1, size - pos - 1, ref value2)) return false;

                            if (value2 == 0)
                            {	// protected divice by 0
                                val = 0.0;
                                return true;
                            }
                            else
                            {
                                if (!GetValueRecurse(rt, rs, cell_table, s, start, pos, ref value1)) return false;
                                val = value1 / value2;
                                return true;
                            }
                        }
                    }
                }
                else { }

                pos--;

                if (pos < 0)
                {
                    if (s[start] == '(' && s[start + size - 1] == ')')
                    {
                        return GetValueRecurse(rt, rs, cell_table, s, start + 1, size - 2, ref val);
                    }
                    else
                    {
                        return GetValueElseFunction(rt, rs, cell_table, s, start, size, ref val);
                    }
                }
            }
        }

        /*
		bool GetValueRecurse(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string s, int size, ref double val)
		{
			if(size <= 0) 
			{
				val = 0.0;
				return true;
			}

			if(s[0] == '=') 
			{
				return GetValueRecurse(rt, rs, cell_table, s.Substring(1), size-1, ref val);
			}

			int   open1 = 0, open2 = 0;
			int   close1 = 0, close2 = 0;
			int   pos = 0;
			double value1=0, value2=0;

			//--------------------
			// 명령어 @cell(field)
			//--------------------

			while(true) 
			{
				if(s[pos] == '(')  
				{
					open1++;
				}
				else if(s[pos] == ')') 
				{
					close1++;
				}
				else if(s[pos] == '[') 
				{
					open2++;
				}
				else if(s[pos] == ']') 
				{
					close2++;
				}
				else if(s[pos] == '+') 
				{
					if(open1 == close1 && open2 == close2) 
					{
						if(pos < 1 || pos >= size-1) 
						{
							return false;
						}
						else 
						{
							if(!GetValueRecurse(rt, rs, cell_table, s, pos, ref value1))				return false;
							if(!GetValueRecurse(rt, rs, cell_table, s.Substring(pos+1), size-pos-1, ref value2))	return false;
							val = value1+value2;
							return true;
						}
					}
				}
				else if(s[pos] == '-') 
				{
					if(open1 == close1 && open2 == close2) 
					{
						if(pos == 0) 
						{
							if(!GetValueRecurse(rt, rs, cell_table, s.Substring(pos+1), size-pos-1, ref value1))	return false;
							val = 0-value1;
							return true;
						}
						else if(pos >= size-1) 
						{
							val = 0.0;
							return true;
						}
						else 
						{
							if(!GetValueRecurse(rt, rs, cell_table, s, pos, ref value1))	return false;
							if(!GetValueRecurse(rt, rs, cell_table, s.Substring(pos+1), size-pos-1, ref value2))	return false;

							val = value1-value2;
							return true;
						}
					}
				}
				else if(s[pos] == '%') 
				{
					if(open1 == close1 && open2 == close2) 
					{
						if(pos == 0) 
						{
							if(!GetValueRecurse(rt, rs, cell_table, s.Substring(pos+1), size-pos-1, ref value1))	return false;
							val = 0-value1;
							return true;
						}
						else if(pos >= size-1) 
						{
							val = 0.0;
							return true;
						}
						else 
						{
							if(!GetValueRecurse(rt, rs, cell_table, s, pos, ref value1))	return false;
							if(!GetValueRecurse(rt, rs, cell_table, s.Substring(pos+1), size-pos-1, ref value2))	return false;

							if(value2 == 0)
								val = 0;
							else 
								val = (int)(value1)%(int)(value2);
							return true;
						}
					}
				}
				else {}

				pos ++;

				if(pos >= size) 
				{
					break;
				}
			}

			open1 = 0;
			open2 = 0;
			close1 = 0;
			close2 = 0;
			pos = 0;

			while(true) 
			{
				if(s[pos] == '(')  
				{
					open1++;
				}
				else if(s[pos] == ')') 
				{
					close1++;
				}
				else if(s[pos] == '[') 
				{
					open2++;
				}
				else if(s[pos] == ']') 
				{
					close2++;
				}
				else if(s[pos] == '*') 
				{
					if(open1 == close1 && open2 == close2) 
					{
						if(pos < 1 || pos >= size-1) 
						{
							val = 0.0;
							return true;
						}
						else 
						{
							if(!GetValueRecurse(rt, rs, cell_table, s, pos, ref value1))	return false;
							if(!GetValueRecurse(rt, rs, cell_table, s.Substring(pos+1), size-pos-1, ref value2))	return false;
							val = value1*value2;
							return true;
						}
					}
				}
				else if(s[pos] == '/') 
				{
					if(open1 == close1 && open2 == close2) 
					{
						if(pos < 1 || pos >= size-1) 
						{
							val = 0;
							return true;
						}
						else 
						{
							if(!GetValueRecurse(rt, rs, cell_table, s.Substring(pos+1), size-pos-1, ref value2))	return false;

							if(value2 == 0) 
							{	// protected divice by 0
								val = 0.0;
								return true;
							}
							else 
							{
								if(!GetValueRecurse(rt, rs, cell_table, s, pos, ref value1))	return false;
								val = value1/value2;
								return true;
							}
						}
					}
				}
				else {}

				pos++;

				if(pos >= size) 
				{
					if(s[0] == '(' && s[size-1] == ')') 
					{
						return GetValueRecurse(rt, rs, cell_table, s.Substring(1), size-2, ref val);
					}
					else 
					{
						return GetValueElseFunction(rt, rs, cell_table, s, size, ref val);
					}
				}
			}
		}*/

		bool GetValueRecurse(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, int cell_y, string s, int size, ref double val)
		{
			bRecurseStringValueFlag = false;
			nRecurseCellY = cell_y;

			return GetValueRecurse(rt, rs, cell_table, s, 0, size, ref val);
		}

		void MakeCellString(CELL_STRUCT cell, double val, bool retn)
		{
			if(!retn) 
			{
				cell.text = ReportConfig.sNoData;
			}
			else 
			{
				MakeNumberByFormat(ref cell.text, cell.format, val);
			}
		}

		void GetOriginalTag(out string tar, string org_tag)
		{
			if(org_tag.Length > 0 && org_tag[0] == '$') 
			{
				if(StringVar.GetStringVar(org_tag.Substring(1), out tar))	return;
				tar = org_tag;
				return;
			}
			else 
			{
				tar = org_tag;
			}
		}

		CELL_STRUCT GetTargetCellPointer(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int x, int y)
		{
			if(y < table_t.cell_y) 
			{
				return (CELL_STRUCT)table_t.cellBuf[table_t.cell_x*y+x];
			}

			CELL_STRUCT cell_new;
			int posx, posy;

			for(posy = table_t.cell_y; posy <= y; posy++) 
			{
				for(posx = 0; posx < table_t.cell_x; posx++) 
				{	
					//FillDefaultCell(&cell_new[m]);
					cell_new = (CELL_STRUCT)Tools.CopyObject(table_s.cellBuf[posx+table_t.cell_x*(cell_s.y)]);
					cell_new.x = posx;
					cell_new.y = posy;
					cell_new.wOnRunY1 = cell_s.y;	// 원본의 파생 위치
					cell_new.wOnRunY2 = cell_s.y;
					if(cell_new.cGroup != 0) 
					{	// 보통 셀이 아니다. (Group Cell)
						cell_new.nGroupY += posy-((CELL_STRUCT)table_s.cellBuf[posx+table_t.cell_x*(cell_s.y)]).y;
					}

					table_t.cellBuf.Add(cell_new);
				}
			}

			table_t.cell_y = y+1;

			return (CELL_STRUCT)table_t.cellBuf[table_t.cell_x*y+x];
		}

        string MakeStringByFormat(DISPLAY_FORMAT_STRUCT format, string val)
        {
            if (val == null)
                return "<NULL>";

            if (format.cType == 1)  // 숫자 형식 일 때만 형식에 맞추어서 바꿔준다.
            {
                double d = ConvertTool.ToDouble(val);
                MakeNumberByFormat(ref val, format, d);
            }

            return val;
        }

		void MakeNumberByFormat(ref string target, DISPLAY_FORMAT_STRUCT format, double val)
		{
			string format_string;

			if(format.cType == 3) 
			{
				MakeTimeCountString(ref target, format, (int)val);
				return;
			}
			else if(format.cType == 1) 
			{
				if(format.bThousandComma == 0) 
				{
					format_string = String.Format("F{0}", format.cUnderPoint);
					target = val.ToString(format_string);
				}
				else 
				{
                    format_string = String.Format("N{0}", format.cUnderPoint);// "N" + format.cUnderPoint.ToString();

                    target = val.ToString(format_string);   // N5
                    /*
                    bool minus_flag = (val < 0 && val > -1) ? true : false;

					string imsi;
					string imsi2="";
					long long_value = (long)val;

                    format_string = String.Format("F{0}", format.cUnderPoint);
                    string under_string = Math.Abs(val - (long)val).ToString(format_string);

                    if (under_string[0] == '1')
                    {
                        long_value++;   // 반올림 되어서 1자리가 1.??? 가 되었다.
                    }

					imsi = String.Format("{0}", long_value);
			
					int pos = 0;
					int count = 0;
					int i;

					for(i = 0, pos = imsi.Length-1; i < imsi.Length; i++, pos--) 
					{
						imsi2 += imsi[pos];
						count++;
						if(count >= 3 && pos > 0 && imsi[pos-1] != '-') 
						{	// 3자리가 찼으나 첫번째 자리가 -이면 ,를 표시하지 않는다.
							imsi2 += ',';
							count = 0;
						}
					}

					target = "";
					for(i = 0, pos = imsi2.Length-1; i < imsi2.Length; i++, pos--) 
					{
						target += imsi2[pos];
					}

					target += under_string.Substring(1);

                    if (minus_flag) target = "-" + target;*/
				}
			}
            else if (format.cType == 4)
            {
                string fm = "{0}";

                if (format.sUserFormat != null && format.sUserFormat.Length > 0)
                    fm = format.sUserFormat;

                try
                {
                    target = String.Format(fm, val);
                }
                catch
                {
                    target = val.ToString();
                }
            }
			else 
			{
				//target = String.Format( "%.6f", val);
				target = val.ToString("F6");
				int count = target.Length;
				int i;

				for(i = count-1; i > 0; i--) 
				{
					if(target[i] == '.') 
					{
						target = target.Substring(0, i);
						break;
					}

					if(target[i] != '0')	break;

					target = target.Substring(0, i);
				}
			}
		}

		string Message알수없는시간형식(string zone)
		{
			if(Tools.IsLangKorean()) 
			{
				return String.Format("알 수 없는 시간 형식({0})", zone);
			}
			else if(Tools.IsLangChinese())
			{
				return String.Format("时间格式不正确({0})", zone);
			}
			else 
			{
				return String.Format("Unknown time format({0})", zone);
			}
		}

		async Task<int> ConvertAiOneData(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y, EnumDataType data_type, EnumHandAuto hand_auto)
		{
			OBJECT_AI_ONE_DATA obj = GetAiOneDataStruct(cell_s.text);
			CELL_STRUCT cell_t;

			cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);

			if(String.Compare(obj.time.zone, "Min") == 0) 
			{
				await ConvertAiDataMin(obj, cell_t, data_type, hand_auto).ConfigureAwait(false);
			}
			else if(String.Compare(obj.time.zone, "Hour") == 0) 
			{
                await ConvertAiDataHour(obj, cell_t, data_type, hand_auto).ConfigureAwait(false);
			}
			else if(String.Compare(obj.time.zone, "Day") == 0) 
			{
                await ConvertAiDataDay(obj, cell_t, data_type, hand_auto).ConfigureAwait(false);
			}
			else if(String.Compare(obj.time.zone, "Mon") == 0) 
			{
                await ConvertAiDataMon(obj, cell_t, data_type, hand_auto).ConfigureAwait(false);
			}
			else 
			{
				cell_t.text = Message알수없는시간형식(obj.time.zone);
			}
			return 1;
		}

        // 순시치를 읽어온다.
        async Task<(bool, double value)> Get_1MinMoment(string tag, int year, int month, int day, int hour, int minute)
        {
            TREND_AI_STRUCT trend = new TREND_AI_STRUCT();
            DataLocal data = new DataLocal();

			double value;

            bool retn = await data.LoadMinDataStructAI(tag, year, month, day, hour, minute, trend);

            if (retn)
            {
                value = trend.fCurr;
            }
            else
            {
                value = 0;
            }

            return (retn,value);
        }

        async Task<(bool, double ave)> Get_15MinAve(string tag, int year, int month, int day, int hour, int minute,  int SharpSharp)
        {
            TREND_AI_STRUCT trend = new TREND_AI_STRUCT();
            DataLocal data = new DataLocal();

            int count = 0;
            bool retn;
            double sum = 0;
			double ave;

            for (int i = 0, m = minute; i < SharpSharp; i++, m++)
            {
                retn = await data.LoadMinDataStructAI(tag, year, month, day, hour, m, trend);

                if (retn)
                {
                    count++;
                    sum += trend.fAverage;
                }
            }

            ave = 0;

            if (count == 0) return (false, ave);

            ave = sum / count;

            return (true, ave);
        }

        // 1시간을 15분 단위로 평균을 해서 그중 최대값을 구한다.
        async Task<(bool, double max)> Get_15MinAve_Max_Hour(string tag, int year, int month, int day, int hour, int SharpSharp)
        {
            //TREND_AI_STRUCT trend = new TREND_AI_STRUCT();
            //DataLocal data = new DataLocal();

            int count = 0;
            bool retn;
            double ave;
            double max = 0;

            for (int minute = 0; minute < 60; minute += SharpSharp)
            {
                (retn, ave) = await Get_15MinAve(tag, year, month, day, hour, minute, SharpSharp);

                if (retn)
                {
                    count++;
                    if (count == 1)
                    {
                        max = ave;
                    }
                    else
                    {
                        if (ave > max)
                            max = ave;
                    }
                }
            }

            if (count == 0) return (false, max);

            return (true, max);
        }

        // 하루를 15분 단위로 평균을 해서 그중 최대값을 구한다.
        async Task<(bool, double max)> Get_15MinAve_Max_Day(string tag, int year, int month, int day,  int SharpSharp)
        {
            //TREND_AI_STRUCT trend = new TREND_AI_STRUCT();
            //DataLocal data = new DataLocal();

            int count = 0;
            bool retn;
            double ave;
            double max = 0;

            for (int hour = 0; hour < 24; hour++)
            {
                for (int minute = 0; minute < 60; minute += SharpSharp)
                {
                    (retn, ave) = await Get_15MinAve(tag, year, month, day, hour, minute,  SharpSharp);

                    if (retn)
                    {
                        count++;
                        if (count == 1)
                        {
                            max = ave;
                        }
                        else
                        {
                            if (ave > max)
                                max = ave;
                        }
                    }
                }
            }

            if (count == 0) return (false, max);

            return (true, max);
        }

        // 한달을 15분 단위로 평균을 해서 그중 최대값을 구한다.  년보에서 사용한다.
        async Task<(bool, double max)> Get_15MinAve_Max_Month(string tag, int year, int month,  int SharpSharp)
        {
            //TREND_AI_STRUCT trend = new TREND_AI_STRUCT();
            //DataLocal data = new DataLocal();

            int count = 0;
            bool retn;
            double ave;
            double max = 0;

            for (int day = 1; day <= 31; day++)
            {
                for (int hour = 0; hour < 24; hour++)
                {
                    for (int minute = 0; minute < 60; minute += SharpSharp)
                    {
                        (retn, ave) = await Get_15MinAve(tag, year, month, day, hour, minute, SharpSharp);

                        if (retn)
                        {
                            count++;
                            if (count == 1)
                            {
                                max = ave;
                            }
                            else
                            {
                                if (ave > max)
                                    max = ave;
                            }
                        }
                    }
                }
            }

            if (count == 0) return (false, max);

            return (true, max);
        }

        async Task<int> ConvertAiMomentOneData(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y, EnumHandAuto hand_auto)
        {
            OBJECT_MOMENT_DATA obj = GetMomentDataStruct(cell_s.text);
            CELL_STRUCT cell_t;
            USER_SELECT_TIME user_time = new USER_SELECT_TIME();
            USER_SELECT_TIME time;
            //TREND_AI_STRUCT trend = new TREND_AI_STRUCT();
            bool retn = false;

            cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);
            GetSelectedReportTime(user_time, hand_auto);

            string org_tag = "";
            GetOriginalTag(out org_tag, obj.tag);

            double value = 0;

            // ##MinuteAve"
            // 15분간 평균값-1분평균값을 15분 모아서 평균한다.

            // ##MinuteAve_Max
            // 1분평균값을 15분 모아서 평균한 다음 각 15분 평균값 중에서 최대값을 찾는다. 
            // 일보에서는 각 시간마다 4개의 데이터중에서 최대값을 찾고
            // 월보에서는 각 일마다 시간마다 4개*24 96개의 데이터중에서 최고값을 찾는다.

            if (String.Compare(obj.time.zone, "Min") == 0)
            {
                time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
                time.min = obj.time.from;
                FitMin(time, obj.time.shift_from);

                (retn, value) = await Get_1MinMoment(org_tag, time.year, time.mon, time.day, time.hour, time.min);
            }
            else if (String.Compare(obj.time.zone, "Hour") == 0)
            {
                time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
                time.hour = obj.time.from;
                time.min = obj.min;
                FitHour(time, obj.time.shift_from);

                if (obj.data_type == "##MinuteAve")
                    (retn, value) = await Get_15MinAve(org_tag, time.year, time.mon, time.day, time.hour, time.min,  obj.nSharpSharpValue);
                else if (obj.data_type == "##MinuteAve_Max")
                    (retn, value) = await Get_15MinAve_Max_Hour(org_tag, time.year, time.mon, time.day, time.hour,  obj.nSharpSharpValue);
                else
                    (retn, value) = await Get_1MinMoment(org_tag, time.year, time.mon, time.day, time.hour, time.min);
            }
            else if (String.Compare(obj.time.zone, "Day") == 0)
            {
                time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
                time.day = obj.time.from;
                time.hour = obj.hour;
                time.min = obj.min;
                FitDay(time, obj.time.shift_from);

                if (obj.data_type == "##MinuteAve")
                    (retn, value) = await Get_15MinAve(org_tag, time.year, time.mon, time.day, time.hour, time.min,  obj.nSharpSharpValue);
                else if (obj.data_type == "##MinuteAve_Max")
                    (retn, value) = await Get_15MinAve_Max_Day(org_tag, time.year, time.mon, time.day, obj.nSharpSharpValue);
                else
                    (retn, value) = await Get_1MinMoment(org_tag, time.year, time.mon, time.day, time.hour, time.min);
            }
            else if (String.Compare(obj.time.zone, "Mon") == 0)
            {
                time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
                time.mon = obj.time.from;
                time.day = obj.day;
                time.hour = obj.hour;
                time.min = obj.min;
                FitMon(time, obj.time.shift_from);

                if (obj.data_type == "##MinuteAve")
                    (retn, value) = await Get_15MinAve(org_tag, time.year, time.mon, time.day, time.hour, time.min, obj.nSharpSharpValue);
                else if (obj.data_type == "##MinuteAve_Max")
                    (retn, value) = await Get_15MinAve_Max_Month(org_tag, time.year, time.mon,  obj.nSharpSharpValue);
                else
                    (retn, value) = await Get_1MinMoment(org_tag, time.year, time.mon, time.day, time.hour, time.min);
            }
            else
            {
                cell_t.text = Message알수없는시간형식(obj.time.zone);
            }

            if (retn == false)
            {
                cell_t.text = String.Format(ReportConfig.sNoData);
                return 1;
            }

            MakeNumberByFormat(ref cell_t.text, cell_t.format, value);

            return 1;

            // return 1 이라는 것은 성공이라는 뜻이 아니라 1줄이 증가했다는 뜻이다.
        }

		async Task<int> ConvertAiOneMaxSum(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y, EnumHandAuto hand_auto)
		{
			OBJECT_AI_MAX_SUM obj = GetAiMaxSumStruct(cell_s.text);
			CELL_STRUCT cell_t;
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			USER_SELECT_TIME time_fr, time_to;
			bool retn = false;
			double val = 0;

			cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);
			GetSelectedReportTime(user_time, hand_auto);

			if(String.Compare(obj.time.zone, "Min") == 0) 
			{
				time_fr = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);

				time_fr.min = obj.time.from;
				time_to.min = obj.time.to;
				FitMin(time_fr, obj.time.shift_from);
				FitMinWithDataTime(time_to, obj.time.shift_to, obj.time, hand_auto);

				(retn, val) = await GetMaxSumFromTo(time_fr, time_to, obj);
			}
			else if(String.Compare(obj.time.zone, "Hour") == 0) 
			{
				time_fr = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);

				time_fr.min = 0;
				time_fr.hour = obj.time.from;
		
				time_to.hour = obj.time.to;
				FitHour(time_fr, obj.time.shift_from);
				FitHourWithDataTime(time_to, obj.time.shift_to, obj.time, hand_auto);

				time_to.min = 59;

                (retn, val) = await GetMaxSumFromTo(time_fr, time_to, obj);
			}
			else if(String.Compare(obj.time.zone, "Day") == 0) 
			{
				time_fr = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);

				time_fr.min = 0;
				time_fr.hour = 0;
				time_fr.day = obj.time.from;
		
				time_to.day = obj.time.to;
				FitDay(time_fr, obj.time.shift_from);
				FitDayWithDataTime(time_to, obj.time.shift_to, obj.time, hand_auto);

				time_to.min = 59;
				time_to.hour = 23;

                (retn, val) = await GetMaxSumFromTo(time_fr, time_to, obj);
			}
			else if(String.Compare(obj.time.zone, "Mon") == 0) 
			{
				time_fr = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);

				time_fr.min = 0;
				time_fr.hour = 0;
				time_fr.day = 1;
				time_fr.mon = obj.time.from;
		
				time_to.mon = obj.time.to;
				FitMon(time_fr, obj.time.shift_from);
				FitMonWithDataTime(time_to, obj.time.shift_to, obj.time, hand_auto);

				time_to.min = 59;
				time_to.hour = 23;
				time_to.day = 31;

                (retn, val) = await GetMaxSumFromTo(time_fr, time_to, obj);
			}
			else 
			{
				cell_t.text = Message알수없는시간형식(obj.time.zone);
			}

			if(retn == false) 
			{
				cell_t.text = String.Format(ReportConfig.sNoData);
				return 1;
			}
	
			MakeNumberByFormat(ref cell_t.text, cell_t.format, val);

			return 1;

			// return 1 이라는 것은 성공이라는 뜻이 아니라 1줄이 증가했다는 뜻이다.
		}

		async Task<int> ConvertAiMultiData(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y, EnumDataType data_type, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time;
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			OBJECT_AI_MULTI_DATA obj = GetAiMultiDataStruct(cell_s.text);
			double val = 0;
			int	  i;
			CELL_STRUCT cell_t;
			int   cell_y;
			int   shift;
			bool retn = false;

			GetSelectedReportTime(user_time, hand_auto);

			string org_tag = "";
			GetOriginalTag(out org_tag, obj.tag);

			DataLocal data = new DataLocal();

			// 분 자료를 읽어온다.
			if(String.Compare(obj.time.zone, "Min") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitMin(time, shift);
						time.min = i;
						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
						(retn, val) = await data.CatDataGetAiMin(0, org_tag,  time.year, time.mon, time.day, time.hour, time.min, data_type);
						if(!retn)
						{
							cell_t.text = ReportConfig.sNoData;
						}
						else 
						{
							MakeNumberByFormat(ref cell_t.text, cell_t.format, val);
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
				// 시간 자료를 읽어온다.
			else if(String.Compare(obj.time.zone, "Hour") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitHour(time, shift);
						time.hour = i;
						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
						(retn, val) = await data.CatDataGetAiHour(0, org_tag, time.year, time.mon, time.day, time.hour, 0, data_type);
						if(!retn)
						{
							cell_t.text = ReportConfig.sNoData;
						}
						else 
						{
							MakeNumberByFormat(ref cell_t.text, cell_t.format, val);
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
				// 일 자료를 읽어온다.
			else if(String.Compare(obj.time.zone, "Day") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitDay(time, shift);
						time.day = i;
						if(TimeUtil.IsDayExist(time.year, time.mon, time.day)) 
						{
							cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
							(retn, val) = await data.CatDataGetAiDay(0, org_tag, time.year, time.mon, time.day, 0, 0, data_type);
							if(!retn)
							{
								cell_t.text = ReportConfig.sNoData;
							}
							else 
							{
								MakeNumberByFormat(ref cell_t.text, cell_t.format, val);
							}
							cell_y++;
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
				// 월 자료를 읽어온다.
			else if(String.Compare(obj.time.zone, "Mon") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitMon(time, shift);
						time.mon = i;
						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
					    (retn, val) = await data.CatDataGetAiMonth(0, org_tag, time.year, time.mon, 1, 0, 0, data_type);
						if(!retn)
						{
							cell_t.text = ReportConfig.sNoData;
						}
						else 
						{
							MakeNumberByFormat(ref cell_t.text, cell_t.format, val);
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
			else 
			{
				cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);
				cell_t.text = Message알수없는시간형식(obj.time.zone);
				return 1;
			}
		}

		async Task<int> ConvertAiMultiMomentData(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time;
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			OBJECT_MOMENT_DATA obj = new OBJECT_MOMENT_DATA();
			int	  i;
			CELL_STRUCT cell_t;
			int   cell_y;
			int   shift;
			//TREND_AI_STRUCT trend = new TREND_AI_STRUCT();

			ReportLib.ObjectStringToStruct(ref obj, cell_s.text);

			GetSelectedReportTime(user_time, hand_auto);

			string org_tag = "";
            GetOriginalTag(out org_tag, obj.tag);

			//DataLocal data = new DataLocal();
            double value = 0;
            bool retn;

			// 분 자료를 읽어온다.
			if(String.Compare(obj.time.zone, "Min") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitMin(time, shift);
						time.min = i;
						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);

                        (retn, value) = await Get_1MinMoment(org_tag, time.year, time.mon, time.day, time.hour, time.min);

                        if (!retn) 
						{
							cell_t.text = ReportConfig.sNoData;
						}
						else 
						{
                            MakeNumberByFormat(ref cell_t.text, cell_t.format, value);
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
				// 시간 자료를 읽어온다.
			else if(String.Compare(obj.time.zone, "Hour") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitHour(time, shift);
						time.hour = i;
						time.min = obj.min;
						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);

                        if (obj.data_type == "##MinuteAve")
                            (retn, value) = await Get_15MinAve(org_tag, time.year, time.mon, time.day, time.hour, time.min, obj.nSharpSharpValue);
                        else if (obj.data_type == "##MinuteAve_Max")
                            (retn, value) = await Get_15MinAve_Max_Hour(org_tag, time.year, time.mon, time.day, time.hour,  obj.nSharpSharpValue);
                        else
                            (retn, value) = await Get_1MinMoment(org_tag, time.year, time.mon, time.day, time.hour, time.min);

                        if (!retn) 
						{
							cell_t.text = ReportConfig.sNoData;
						}
						else 
						{
							MakeNumberByFormat(ref cell_t.text, cell_t.format, value);
						}
				
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
				// 일 자료를 읽어온다.
			else if(String.Compare(obj.time.zone, "Day") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitDay(time, shift);
						time.day = i;
						time.hour = obj.hour;
						time.min = obj.min;

						if(TimeUtil.IsDayExist(time.year, time.mon, time.day)) 
						{
							cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);

                            if (obj.data_type == "##MinuteAve")
                                (retn, value) = await Get_15MinAve(org_tag, time.year, time.mon, time.day, time.hour, time.min, obj.nSharpSharpValue);
                            else if (obj.data_type == "##MinuteAve_Max")
                                (retn, value) = await Get_15MinAve_Max_Day(org_tag, time.year, time.mon, time.day, obj.nSharpSharpValue);
                            else
                                (retn, value) = await Get_1MinMoment(org_tag, time.year, time.mon, time.day, time.hour, time.min);

                            if (!retn)
							{
								cell_t.text = ReportConfig.sNoData;
							}
							else 
							{
								MakeNumberByFormat(ref cell_t.text, cell_t.format, value);
							}
							cell_y++;
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
				// 월 자료를 읽어온다.
			else if(String.Compare(obj.time.zone, "Mon") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitMon(time, shift);
						time.mon = i;
						time.day = obj.day;
						time.hour = obj.hour;
						time.min = obj.min;
						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);

                        if (obj.data_type == "##MinuteAve")
                            (retn, value) = await Get_15MinAve(org_tag, time.year, time.mon, time.day, time.hour, time.min, obj.nSharpSharpValue);
                        else if (obj.data_type == "##MinuteAve_Max")
                            (retn, value) = await Get_15MinAve_Max_Month(org_tag, time.year, time.mon,  obj.nSharpSharpValue);
                        else
                            (retn, value) = await Get_1MinMoment(org_tag, time.year, time.mon, time.day, time.hour, time.min);

                        if (!retn) 
						{
							cell_t.text = ReportConfig.sNoData;
						}
						else 
						{
							MakeNumberByFormat(ref cell_t.text, cell_t.format, value);
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
			else 
			{
				cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);

				cell_t.text = Message알수없는시간형식(obj.time.zone);
				return 1;
			}
		}

		async Task<int> ConvertAiMultiMaxSum(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time_fr, time_to;
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			OBJECT_AI_MAX_SUM obj = new OBJECT_AI_MAX_SUM();
			int	  i;
			CELL_STRUCT cell_t;
			int   cell_y;
			int   shift;
			double val = 0;
			bool retn = false;

			ReportLib.ObjectStringToStruct(ref obj, cell_s.text);

			GetSelectedReportTime(user_time, hand_auto);

			// 분 자료를 읽어온다.
			if(String.Compare(obj.time.zone, "Min") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time_fr = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitMin(time_fr, shift);
						time_fr.min = i;

						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);

						(retn, val) = await GetMaxSumFromTo(time_fr, time_fr, obj);
						if(!retn)
							{
							cell_t.text = ReportConfig.sNoData;
						}
						else 
						{
							MakeNumberByFormat(ref cell_t.text, cell_t.format, val);
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
				// 시간 자료를 읽어온다.
			else if(String.Compare(obj.time.zone, "Hour") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time_fr = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitHour(time_fr, shift);
						FitHour(time_to, shift);
						time_fr.hour = i;
						time_fr.min  = 0;
						time_to.hour = i;
						time_to.min  = 59;

						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);

						(retn, val) = await GetMaxSumFromTo(time_fr, time_to, obj);
						if(!retn)
						{
							cell_t.text = ReportConfig.sNoData;
						}
						else 
						{
							MakeNumberByFormat(ref cell_t.text, cell_t.format, val);
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
				// 일 자료를 읽어온다.
			else if(String.Compare(obj.time.zone, "Day") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++) 
					{
						time_fr = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitDay(time_fr, shift);
						FitDay(time_to, shift);
						time_fr.day = i;
						time_fr.hour = 0;
						time_fr.min  = 0;
						time_to.day = i;
						time_to.hour = 23;
						time_to.min  = 59;

						if(TimeUtil.IsDayExist(time_fr.year, time_fr.mon, time_fr.day)) 
						{
							cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);

							(retn, val) = await GetMaxSumFromTo(time_fr, time_to, obj) ;
							if(!retn)
							{
								cell_t.text = ReportConfig.sNoData;
							}
							else 
							{
								MakeNumberByFormat(ref cell_t.text, cell_t.format, val);
							}
							cell_y++;
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
				// 월 자료를 읽어온다.
			else if(String.Compare(obj.time.zone, "Mon") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time_fr = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitMon(time_fr, shift);
						FitMon(time_to, shift);
						time_fr.mon = i;
						time_fr.day = 1;
						time_fr.hour = 0;
						time_fr.min  = 0;
						time_to.mon = i;
						time_to.day = 31;
						time_to.hour = 23;
						time_to.min  = 59;

						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);

						(retn, val) = await GetMaxSumFromTo(time_fr, time_to, obj);
						if(!retn)
						{
							cell_t.text = ReportConfig.sNoData;
						}
						else 
						{
							MakeNumberByFormat(ref cell_t.text, cell_t.format, val);
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
			else 
			{
				cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);
				cell_t.text = Message알수없는시간형식(obj.time.zone);
				return 1;
			}
		}

		int ConvertAlarm(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time_to, time_from;
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			OBJECT_ALARM obj = new OBJECT_ALARM();
			CELL_STRUCT cell_t;
	
			ALARM_FILE_STRUCT alarm;

			ReportLib.ObjectStringToStruct(ref obj, cell_s.text);

			GetSelectedReportTime(user_time, hand_auto);

			if(String.Compare(obj.time.zone, "Min") == 0) 
			{
				time_from = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				time_from.min = obj.time.from;
				FitMin(time_from, obj.time.shift_from);
				time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				time_to.min = obj.time.to;
				FitMin(time_to, obj.time.shift_to);

				LoadAlarmList(obj, time_from, time_to, 0);
			}
			else if(String.Compare(obj.time.zone, "Hour") == 0) 
			{
				time_from = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				time_from.hour = obj.time.from;
				FitHour(time_from, obj.time.shift_from);
				time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				time_to.hour = obj.time.to;
				FitHour(time_to, obj.time.shift_to);

				LoadAlarmList(obj, time_from, time_to, 1);
			}
			else if(String.Compare(obj.time.zone, "Day") == 0) 
			{
				time_from = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				time_from.day = obj.time.from;
				FitDay(time_from, obj.time.shift_from);
				time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				time_to.day = obj.time.to;
				FitDay(time_to, obj.time.shift_to);

				LoadAlarmList(obj, time_from, time_to, 2);
			}
			else if(String.Compare(obj.time.zone, "Mon") == 0) 
			{
				time_from = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				time_from.mon = obj.time.from;
				time_from.day = 1;
				FitMon(time_from, obj.time.shift_from);
				time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				time_to.mon = obj.time.to;
				time_to.day = TimeUtil.getmonthlimit(time_to.year, time_to.mon);
				FitMon(time_to, obj.time.shift_to);

				LoadAlarmList(obj, time_from, time_to, 3);
			}

			if(blockAlarmList.Count == 0) 
			{
				cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);
				cell_t.text = String.Format(ReportConfig.sNoData);
				return 1;
			}
			else 
			{
				// 실제 이 부분은 계산하는데는 필요하지 않다. 
				// 각줄마다 새로 만들게 되면 시간이 너무 걸리므로 필요한 라인 만큼을 미리 준비한다는 의미이다.
				// 아래 한줄을 사용하면 경보가 많을 때는 많은 시간을 절약할 수 있다.
				cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+blockAlarmList.Count-1);

				for(int l = 0; l < blockAlarmList.Count; l++) 
				{
					cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+l);
					alarm = (ALARM_FILE_STRUCT)blockAlarmList[l];
					if(obj.view == 0)		cell_t.text = String.Format("{0:0000}/{1:00}/{2:00}", alarm.t.wYear, alarm.t.wMonth, alarm.t.wDay);
					else if(obj.view == 1)	cell_t.text = String.Format("{0:00}:{1:00}:{2:00}", alarm.t.wHour, alarm.t.wMinute, alarm.t.wSecond);
					else if(obj.view == 2)	cell_t.text = String.Format("{0}", alarm.tag);
					else if(obj.view == 3)	cell_t.text = String.Format("{0}", alarm.description);
					else if(obj.view == 4)	cell_t.text = String.Format("{0}", alarm.msg);
					else if(obj.view == 5)	cell_t.text = String.Format("{0:000}", alarm.priority);
					else 					cell_t.text = String.Format("{0}", l+1);
				}
				return blockAlarmList.Count;
			}
		}

		async Task<int> ConvertAiMinListByMinute(OBJECT_AI_MIN_LIST obj, TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y)
		{
			double val = 0;
			CELL_STRUCT cell_t;
			int   cell_y;
			USER_SELECT_TIME time_fr=new USER_SELECT_TIME(), time_to = new USER_SELECT_TIME();
			int   min_gab;
			EnumDataType data_type;
			bool retn = false;

			if(obj.data_type == 0)		 data_type = EnumDataType.AVE;
			else if(obj.data_type == 1)	 data_type = EnumDataType.MIN;
			else if(obj.data_type == 2)	 data_type = EnumDataType.MAX;
			else if(obj.data_type == 3)	 data_type = EnumDataType.SUM;
			else if(obj.data_type == 4)	 data_type = EnumDataType.SUB;
            else if (obj.data_type == 5) data_type = EnumDataType.MOMENT;
			else						 data_type = EnumDataType.AVE;

			min_gab = GetVarValue(obj.min_gab, 1, 60);

			ReportConfig.GetMinListTimeFr(time_fr);
			ReportConfig.GetMinListTimeTo(time_to);

			string org_tag = "";
            GetOriginalTag(out org_tag, obj.tag);
	
			// 분 자료를 읽어온다.
			cell_y = 0;
			while(true)
			{
				if(TimeUtil.IsDayExist(time_fr.year, time_fr.mon, time_fr.day)) 
				{
					cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
					cell_y++;

					(retn, val) = await GetAiMinDataFromGab(org_tag, time_fr, min_gab, data_type);
					if (!retn)
					{
						cell_t.text = ReportConfig.sNoData;
					}
					else
					{
						MakeNumberByFormat(ref cell_t.text, cell_t.format, val);
					}
				}
				time_fr.min += min_gab;
				if(time_fr.min >= 60) 
				{
					TimeUtil.PlusHour(ref time_fr.year, ref time_fr.mon, ref time_fr.day, ref time_fr.hour);
					time_fr.min -= 60;
				}
				if(CompareTimeMin(time_fr, time_to) > 0)	break;
			}

			if(cell_y == 0) 
			{
				cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
				cell_t.text = ReportConfig.sNoData;
				return 1;
			}

			return cell_y;
		}

        async Task<int> ConvertAiMinListByHour(OBJECT_AI_MIN_LIST obj, TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y)
        {
            double val = 0;
            CELL_STRUCT cell_t;
            int cell_y;
            USER_SELECT_TIME time_fr = new USER_SELECT_TIME(), time_to = new USER_SELECT_TIME();
            int hour_gab;
            EnumDataType data_type;
			bool retn = false;

            if (obj.data_type == 0) data_type = EnumDataType.AVE;
            else if (obj.data_type == 1) data_type = EnumDataType.MIN;
            else if (obj.data_type == 2) data_type = EnumDataType.MAX;
            else if (obj.data_type == 3) data_type = EnumDataType.SUM;
            else if (obj.data_type == 4) data_type = EnumDataType.SUB;
            else if (obj.data_type == 5) data_type = EnumDataType.MOMENT;
            else data_type = EnumDataType.AVE;

            hour_gab = GetVarValue(obj.min_gab, 1, 60);

            ReportConfig.GetMinListTimeFr(time_fr);
            ReportConfig.GetMinListTimeTo(time_to);

            string org_tag = "";
            GetOriginalTag(out org_tag, obj.tag);

            // 분 자료를 읽어온다.
            cell_y = 0;
            while (true)
            {
                if (TimeUtil.IsDayExist(time_fr.year, time_fr.mon, time_fr.day))
                {
                    cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y + cell_y);
                    cell_y++;

					(retn, val) = await GetAiHourDataFromGab(org_tag, time_fr, hour_gab, data_type);
                    if(!retn)
					{
                        cell_t.text = ReportConfig.sNoData;
                    }
                    else
                    {
                        MakeNumberByFormat(ref cell_t.text, cell_t.format, val);
                    }
                }
                time_fr.hour += hour_gab;
                if (time_fr.hour >= 24)
                {
                    TimeUtil.PlusDay(ref time_fr.year, ref time_fr.mon, ref time_fr.day);
                    time_fr.hour -= 24;
                }
                if (CompareTimeHour(time_fr, time_to) > 0) break;
            }

            if (cell_y == 0)
            {
                cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y + cell_y);
                cell_t.text = ReportConfig.sNoData;
                return 1;
            }

            return cell_y;
        }

		async Task<int> ConvertAiMinList(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y)
		{
			OBJECT_AI_MIN_LIST obj = new OBJECT_AI_MIN_LIST();

			ReportLib.ObjectStringToStruct(ref obj, cell_s.text);

			if(obj.cDataUnit == 1) 
			{	// 시간자료로 구분된다.
				return await ConvertAiMinListByHour(obj, table_t, table_s, cell_s, target_x, target_y);
			}
			else 
			{	// 분자료로 구분된다.
				return await ConvertAiMinListByMinute(obj, table_t, table_s, cell_s, target_x, target_y);
			}
		}

		async Task<int> ConvertAiMinMaxTime(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y, EnumDataType data_type, EnumHandAuto hand_auto)
		{
			OBJECT_AI_ONE_DATA obj = new OBJECT_AI_ONE_DATA();
			CELL_STRUCT cell_t;

			ReportLib.ObjectStringToStruct(ref obj, cell_s.text);

			cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);

			await ConvertAiDataMinMaxTime(obj, cell_t, data_type, hand_auto);

			return 1;
		}

		int ConvertDiCurr(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y)
		{
			OBJECT_DI_CURR obj = new OBJECT_DI_CURR();
			CELL_STRUCT cell_t;
			int[] tag_pos = new int[1];
	
			ReportLib.ObjectStringToStruct(ref obj, cell_s.text);

			string org_tag = "";
			EnumTagType tag_type = 0;
            GetOriginalTag(out org_tag, obj.tag);

			cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);

			if(!TagLib.GetTagTypeAndPos(org_tag, ref tag_type, ref tag_pos)) 
			{
				cell_t.text = no_tag;
				return 1;	// 증가한 Y의 크기를 알려준다.
			}
			else 
			{
				string svalue;
				double dvalue;
				TagLib.GetTagValue(org_tag, out svalue, out dvalue);
				cell_t.text = svalue;
			}

			return 1;
		}

        int ConvertStCurr(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y)
        {
            OBJECT_ST_CURR obj = new OBJECT_ST_CURR();
            CELL_STRUCT cell_t;
            int[] tag_pos = new int[1];

            ReportLib.ObjectStringToStruct(ref obj, cell_s.text);

            string org_tag = "";
            EnumTagType tag_type = 0;
            GetOriginalTag(out org_tag, obj.tag);

            cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);

            if (!TagLib.GetTagTypeAndPos(org_tag, ref tag_type, ref tag_pos))
            {
                cell_t.text = no_tag;
                return 1;	// 증가한 Y의 크기를 알려준다.
            }
            else
            {
                string svalue;
                double dvalue;
                TagLib.GetTagValue(org_tag, out svalue, out dvalue);
                cell_t.text = svalue;
            }

            return 1;
        }

		async Task<int> ConvertDiOneData(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y, EnumDataType data_type, EnumHandAuto hand_auto)
		{
			OBJECT_DI_ONE_DATA obj = GetDiOneDataStruct(cell_s.text);
			CELL_STRUCT cell_t;

			cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);

			if(String.Compare(obj.time.zone, "Min") == 0) 
			{
				await ConvertDiDataMin(obj, cell_t, data_type, hand_auto);
			}
			else if(String.Compare(obj.time.zone, "Hour") == 0) 
			{
				await ConvertDiDataHour(obj, cell_t, data_type, hand_auto);
			}
			else if(String.Compare(obj.time.zone, "Day") == 0) 
			{
				await ConvertDiDataDay(obj, cell_t, data_type, hand_auto);
			}
			else if(String.Compare(obj.time.zone, "Mon") == 0) 
			{
				await ConvertDiDataMon(obj, cell_t, data_type, hand_auto);
			}
			else 
			{
				cell_t.text = Message알수없는시간형식(obj.time.zone);
			}
			return 1;
		}

		void GetSelectedReportTime(USER_SELECT_TIME time, EnumHandAuto hand_auto)
		{
			if(hand_auto == 0) 
			{	// 수동 인쇄 모드이거나 감시에서 사용할 때
				ReportConfig.GetHandSelectTime(time);	
			}
			else 
			{					// 지정 시간이 되면 자동으로 인쇄하는 모드일 때
				ReportConfig.GetAutoSelectTime(time);
			}
		}

		void MakeTimeString(CELL_STRUCT cell, USER_SELECT_TIME time)
		{
            if (cell.format.cType == 4)
            {
                string fm = "{0}";

                if (cell.format.sUserFormat != null && cell.format.sUserFormat.Length > 0)
                    fm = cell.format.sUserFormat;

                try
                {
                    cell.text = String.Format(fm, new DateTime(time.year, time.mon, time.day, time.hour, time.min, time.sec));
                    return;
                }
                catch
                {
                    // 그렇지 않을 때는 날짜/시간 형식을 사용한다.
                    //cell.tetarget = val.ToString();
                }
            }

			if(cell.format.cDateTime == 0)
				cell.text = String.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00}", time.year, time.mon, time.day, time.hour, time.min, time.sec);
			else if(cell.format.cDateTime == 1)	
				cell.text = String.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}", time.year, time.mon, time.day, time.hour, time.min);
			else if(cell.format.cDateTime == 2)	
				cell.text = String.Format("{0:0000}/{1:00}/{2:00} {3:00}", time.year, time.mon, time.day, time.hour);
			else if(cell.format.cDateTime == 3)	
				cell.text = String.Format("{0:0000}/{1:00}/{2:00}", time.year, time.mon, time.day);
			else if(cell.format.cDateTime == 4)	
				cell.text = String.Format("{0:0000}/{1:00}", time.year, time.mon);
			else if(cell.format.cDateTime == 5)	
				cell.text = String.Format("{0:0000}", time.year);

			else if(cell.format.cDateTime == 6)	
				cell.text = String.Format("{0:0000}년{1:00}월{2:00}일 {3:00}시{4:00}분{5:00}초", time.year, time.mon, time.day, time.hour, time.min, time.sec);
			else if(cell.format.cDateTime == 7)	
				cell.text = String.Format("{0:0000}년{1:00}월{2:00}일 {3:00}시{4:00}분", time.year, time.mon, time.day, time.hour, time.min);
			else if(cell.format.cDateTime == 8)	
				cell.text = String.Format("{0:0000}년{1:00}월{2:00}일 {3:00}시", time.year, time.mon, time.day, time.hour);
			else if(cell.format.cDateTime == 9)	
				cell.text = String.Format("{0:0000}년{1:00}월{2:00}일", time.year, time.mon, time.day);
			else if(cell.format.cDateTime == 10)	
				cell.text = String.Format("{0:0000}년{1:00}월", time.year, time.mon);
			else if(cell.format.cDateTime == 11)	
				cell.text = String.Format("{0:0000}년", time.year);

			else if(cell.format.cDateTime == 12)	
				cell.text = String.Format("{0:0000}年{1:00}月{2:00}日 {3:00}時{4:00}分{5:00}初", time.year, time.mon, time.day, time.hour, time.min, time.sec);
			else if(cell.format.cDateTime == 13)	
				cell.text = String.Format("{0:0000}年{1:00}月{2:00}日 {3:00}時{4:00}分", time.year, time.mon, time.day, time.hour, time.min);
			else if(cell.format.cDateTime == 14)	
				cell.text = String.Format("{0:0000}年{1:00}月{2:00}日 {3:00}時", time.year, time.mon, time.day, time.hour);
			else if(cell.format.cDateTime == 15)	
				cell.text = String.Format("{0:0000}年{1:00}月{2:00}日", time.year, time.mon, time.day);
			else if(cell.format.cDateTime == 16)	
				cell.text = String.Format("{0:0000}年{1:00}月", time.year, time.mon);
			else if(cell.format.cDateTime == 17)	
				cell.text = String.Format("{0:0000}年", time.year);

			else if(cell.format.cDateTime == 18)	
				cell.text = String.Format("{0:00}/{1:00} {2:00}:{3:00}:{4:00}", time.mon, time.day, time.hour, time.min, time.sec);
			else if(cell.format.cDateTime == 19)	
				cell.text = String.Format("{0:00} {1:00}:{2:00}:{3:00}", time.day, time.hour, time.min, time.sec);
			else if(cell.format.cDateTime == 20)	
				cell.text = String.Format("{0:00}:{1:00}:{2:00}", time.hour, time.min, time.sec);
			else if(cell.format.cDateTime == 21)	
				cell.text = String.Format("{0:00}:{1:00}", time.min, time.sec);
			else if(cell.format.cDateTime == 22)	
				cell.text = String.Format("{0:00}", time.sec);

			else if(cell.format.cDateTime == 23)	
				cell.text = String.Format("{0:00}월{1:00}일 {2:00}시{3:00}분{4:00}초", time.mon, time.day, time.hour, time.min, time.sec);
			else if(cell.format.cDateTime == 24)	
				cell.text = String.Format("{0:00}일 {1:00}시{2:00}분{3:00}초", time.day, time.hour, time.min, time.sec);
			else if(cell.format.cDateTime == 25)	
				cell.text = String.Format("{0:00}시{1:00}분{2:00}초", time.hour, time.min, time.sec);
			else if(cell.format.cDateTime == 26)	
				cell.text = String.Format("{0:00}분{1:00}초", time.min, time.sec);
			else if(cell.format.cDateTime == 27)	
				cell.text = String.Format("{0:00}초", time.sec);

			else if(cell.format.cDateTime == 28)	
				cell.text = String.Format("{0:00}月{1:00}日 {2:00}時{3:00}分{4:00}初", time.mon, time.day, time.hour, time.min, time.sec);
			else if(cell.format.cDateTime == 29)	
				cell.text = String.Format("{0:00}日 {1:00}時{2:00}分{3:00}初", time.day, time.hour, time.min, time.sec);
			else if(cell.format.cDateTime == 30)	
				cell.text = String.Format("{0:00}時{1:00}分{2:00}初", time.hour, time.min, time.sec);
			else if(cell.format.cDateTime == 31)	
				cell.text = String.Format("{0:00}分{1:00}初", time.min, time.sec);
			else if(cell.format.cDateTime == 32)	
				cell.text = String.Format("{0:00}初", time.sec);

			else if(cell.format.cDateTime == 33)	
				cell.text = String.Format("{0:00}/{1:00} {2:00}:{3:00}", time.mon, time.day, time.hour, time.min);
			else if(cell.format.cDateTime == 34)	
				cell.text = String.Format("{0:00} {1:00}:{2:00}", time.day, time.hour, time.min);
			else if(cell.format.cDateTime == 35)	
				cell.text = String.Format("{0:00}:{1:00}", time.hour, time.min);
			else if(cell.format.cDateTime == 36)	
				cell.text = String.Format("{0:00}", time.min);

			else if(cell.format.cDateTime == 37)	
				cell.text = String.Format("{0:00}월{1:00}일 {2:00}시{3:00}분", time.mon, time.day, time.hour, time.min);
			else if(cell.format.cDateTime == 38)	
				cell.text = String.Format("{0:00}일 {1:00}시{2:00}분", time.day, time.hour, time.min);
			else if(cell.format.cDateTime == 39)	
				cell.text = String.Format("{0:00}시{1:00}분", time.hour, time.min);
			else if(cell.format.cDateTime == 40)	
				cell.text = String.Format("{0:00}분", time.min);

			else if(cell.format.cDateTime == 41)	
				cell.text = String.Format("{0:00}月{1:00}日 {2:00}時{3:00}分", time.mon, time.day, time.hour, time.min);
			else if(cell.format.cDateTime == 42)	
				cell.text = String.Format("{0:00}日 {1:00}時{2:00}分", time.day, time.hour, time.min);
			else if(cell.format.cDateTime == 43)	
				cell.text = String.Format("{0:00}時{1:00}分", time.hour, time.min);
			else if(cell.format.cDateTime == 44)	
				cell.text = String.Format("{0:00}分", time.min);

			else if(cell.format.cDateTime == 45)	
				cell.text = String.Format("{0:00}/{1:00} {2:00}", time.mon, time.day, time.hour);
			else if(cell.format.cDateTime == 46)	
				cell.text = String.Format("{0:00} {1:00}", time.day, time.hour);
			else if(cell.format.cDateTime == 47)	
				cell.text = String.Format("{0:00}", time.hour);

			else if(cell.format.cDateTime == 48)	
				cell.text = String.Format("{0:00}월{1:00}일 {2:00}시", time.mon, time.day, time.hour);
			else if(cell.format.cDateTime == 49)	
				cell.text = String.Format("{0:00}일 {1:00}시", time.day, time.hour);
			else if(cell.format.cDateTime == 50)	
				cell.text = String.Format("{0:00}시", time.hour);

			else if(cell.format.cDateTime == 51)	
				cell.text = String.Format("{0:00}月{1:00}日 {2:00}時", time.mon, time.day, time.hour);
			else if(cell.format.cDateTime == 52)	
				cell.text = String.Format("{0:00}日 {1:00}時", time.day, time.hour);
			else if(cell.format.cDateTime == 53)	
				cell.text = String.Format("{0:00}時", time.hour);

			else if(cell.format.cDateTime == 54)	
				cell.text = String.Format("{0:00}/{1:00}", time.mon, time.day);
			else if(cell.format.cDateTime == 55)	
				cell.text = String.Format("{0:00}", time.day);

			else if(cell.format.cDateTime == 56)	
				cell.text = String.Format("{0:00}월{1:00}일", time.mon, time.day);
			else if(cell.format.cDateTime == 57)	
				cell.text = String.Format("{0:00}일", time.day);

			else if(cell.format.cDateTime == 58)	
				cell.text = String.Format("{0:00}月{1:00}日", time.mon, time.day);
			else if(cell.format.cDateTime == 59)	
				cell.text = String.Format("{0:00}日", time.day);

			else if(cell.format.cDateTime == 60)	
				cell.text = String.Format("{0:00}", time.mon);

			else if(cell.format.cDateTime == 61)	
				cell.text = String.Format("{0:00}월", time.mon);

			else if(cell.format.cDateTime == 62)	
				cell.text = String.Format("{0:00}月", time.mon);

			else 
			{
				cell.text = String.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00}", time.year, time.mon, time.day, time.hour, time.min, time.sec);
			}

			if(cell.format.bWeekAdd != 0) 
			{
				string[] sWeek;
				if(Tools.IsLangKorean()) 
				{
					sWeek = new string[7] { "일", "월", "화", "수", "목", "금", "토" };
				}
				else if(Tools.IsLangJapanese()) 
				{
					sWeek = new string[7] { "日", "月", "火", "水", "木", "金", "土" };
				}
				else if(Tools.IsLangChinese()) 
				{
					sWeek = new string[7] { "星期天", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
				}
				else 
				{
					sWeek = new string[7] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
				}
				string imsi;
				imsi = String.Format("({0})", sWeek[TimeUtil.GetWeekDay(time.year, time.mon, time.day)]);
				cell.text += imsi;
			}
		}

		void ConvertDataTime(CELL_STRUCT cell, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time = new USER_SELECT_TIME();
			OBJECT_ETC_DATA_TIME obj = new OBJECT_ETC_DATA_TIME();
			ReportLib.ObjectStringToStruct(ref obj, cell.text);

			GetSelectedReportTime(time, hand_auto);

			MakeTimeString(cell, time);
		}

		void ConvertEtcTime(CELL_STRUCT cell)
		{
			USER_SELECT_TIME time = new USER_SELECT_TIME();
			OBJECT_ETC_TIME obj = new OBJECT_ETC_TIME();
			ReportLib.ObjectStringToStruct(ref obj, cell.text);

            DateTime t = DateTimeServer.Now;

			time.year = t.Year;
			time.mon  = t.Month;
			time.day  = t.Day;
			time.hour = t.Hour;
			time.min  = t.Minute;
			time.sec  = t.Second;
	
			MakeTimeString(cell, time);
		}

		void FitMin(USER_SELECT_TIME time, int shift)
		{
			while(true) 
			{
				if(shift >= 0)	break;
				TimeUtil.MinusHour(ref time.year, ref time.mon, ref time.day, ref time.hour);
				shift++;
			}

			while(true) 
			{
				if(shift <= 0)	break;
				TimeUtil.PlusHour(ref time.year, ref time.mon, ref time.day, ref time.hour);
				shift--;
			}
		} 

		void FitHour(USER_SELECT_TIME time, int shift)
		{
			while(true) 
			{
				if(shift >= 0)	break;
				TimeUtil.MinusDay(ref time.year, ref time.mon, ref time.day);
				shift++;
			}

			while(true) 
			{
				if(shift <= 0)	break;
				TimeUtil.PlusDay(ref time.year, ref time.mon, ref time.day);
				shift--;
			}
		}

		void FitDay(USER_SELECT_TIME time, int shift)
		{
			while(true) 
			{
				if(shift >= 0)	break;
				TimeUtil.MinusMonth(ref time.year, ref time.mon);
				shift++;
			}

			while(true) 
			{
				if(shift <= 0)	break;
				TimeUtil.PlusMonth(ref time.year, ref time.mon);
				shift--;
			}
		}

		void FitMon(USER_SELECT_TIME time, int shift)
		{
			while(true) 
			{
				if(shift >= 0)	break;
				time.year--;
				shift++;
			}

			while(true) 
			{
				if(shift <= 0)	break;
				time.year++;
				shift--;
			}
		}

		void MakeTimeCountString(ref string text, DISPLAY_FORMAT_STRUCT format, int val)
		{
			if(format.cTimeCount == 0) 
			{
				text = String.Format( "{0}:{1:00}:{2:00}", val/3600, (val%3600)/60, val%60);
			}
			else if(format.cTimeCount == 1) 
			{
				text = String.Format( "{0}시간{1:00}분{2:00}초", val/3600, (val%3600)/60, val%60);
			}
			else if(format.cTimeCount == 2) 
			{
				text = String.Format( "{0}時間{1:00}分{2:00}初", val/3600, (val%3600)/60, val%60);
			}
			else if(format.cTimeCount == 3) 
			{
				text = String.Format( "{0}:{1:00}", val/60, val%60);
			}
			else if(format.cTimeCount == 4) 
			{
				text = String.Format( "{0}분{1:00}초", val/60, val%60);
			}
			else if(format.cTimeCount == 5) 
			{
				text = String.Format( "{0}分{1:00}初", val/60, val%60);
			}
			else if(format.cTimeCount == 6) 
			{
				text = String.Format( "{0}",  val);
			}
			else if(format.cTimeCount == 7) 
			{
				text = String.Format( "{0}초", val);
			}
			else if(format.cTimeCount == 8) 
			{
				text = String.Format( "{0}初", val);
			}
			else if(format.cTimeCount == 9) 
			{
				text = String.Format( "{0}시간{1:00}분", val/3600, (val%3600)/60);
			}
			else if(format.cTimeCount == 10) 
			{
				text = String.Format( "{0}時間{1:00}分", val/3600, (val%3600)/60);
			}
			else if(format.cTimeCount == 11) 
			{
				text = String.Format( "{0}:{1:00}", val/3600, (val%3600)/60);
			}
			else 
			{
				text = String.Format( "{0}시간{1:00}분{2:00}초", val/3600, (val%3600)/60, val%60);
			}
		}

		int GetFrom(CELL_TIME time, int shift)
		{
			if(shift == time.shift_from)	return time.from;
			else 
			{
				if(String.Compare(time.zone, "Day") == 0)			return 1;
				else if(String.Compare(time.zone, "Mon") == 0)		return 1;
				else										return 0;
			}
		}

		int GetToWithDataTime(CELL_TIME time, int shift, EnumHandAuto hand_auto)
		{
			if(time.bToDataTime == 1) 
			{
				if(shift == 0) 
				{	// 지정 월/일/시 가 되었다.
					USER_SELECT_TIME user_time = new USER_SELECT_TIME();
					GetSelectedReportTime(user_time, hand_auto);
					if(String.Compare(time.zone, "Min", true) == 0)			return user_time.min;
					else if(String.Compare(time.zone, "Hour", true) == 0)	return user_time.hour;
					else if(String.Compare(time.zone, "Day", true) == 0)	return user_time.day;
					else if(String.Compare(time.zone, "Mon", true) == 0)	return user_time.mon;
					else										return -1;
				}
				else if(shift < 0) 
				{
					if(String.Compare(time.zone, "Min") == 0)			return 59;
					else if(String.Compare(time.zone, "Hour") == 0)	return 23;
					else if(String.Compare(time.zone, "Day") == 0)		return 31;
					else if(String.Compare(time.zone, "Week") == 0)	return 6;
					else if(String.Compare(time.zone, "Mon") == 0)		return 12;
					else										return 10;
				}
				else 
				{	// shift > 0
					return -1;
				}
			}

			if(shift == time.shift_to)	return time.to;
			else 
			{
				if(String.Compare(time.zone, "Min") == 0)			return 59;
				else if(String.Compare(time.zone, "Hour") == 0)	return 23;
				else if(String.Compare(time.zone, "Day") == 0)		return 31;
				else if(String.Compare(time.zone, "Week") == 0)	return 6;
				else if(String.Compare(time.zone, "Mon") == 0)		return 12;
				else										return 10;
			}
		}

		int GetTo(CELL_TIME time, int shift)
		{
			if(shift == time.shift_to)	return time.to;
			else 
			{
				if(String.Compare(time.zone, "Min") == 0)			return 59;
				else if(String.Compare(time.zone, "Hour") == 0)	return 23;
				else if(String.Compare(time.zone, "Day") == 0)		return 31;
				else if(String.Compare(time.zone, "Week") == 0)	return 6;
				else if(String.Compare(time.zone, "Mon") == 0)		return 12;
				else										return 10;
			}
		}

		//-------------------------------------------------------------------------
		// From 에서 To 까지의 분자료를 계산한다.
		//-------------------------------------------------------------------------

		async Task ConvertAiDataMin(OBJECT_AI_ONE_DATA obj, CELL_STRUCT cell_t, EnumDataType data_type, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time;
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			double val, one_value;
			int	  i;
			int   shift;
			bool  read_flag = false;
			int   read_count = 0;
			string org_tag = "";

            GetOriginalTag(out org_tag, obj.tag);

			GetSelectedReportTime(user_time, hand_auto);

			//--------------------
			// 분 자료를 읽어온다.

			val = 0;
			one_value = 0;
			bool retn = false;

			DataLocal data = new DataLocal();

			if(data_type == EnumDataType.SUB) 
			{
				double value1=0, value2=0;
				time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				FitMin(time, obj.time.shift_to);
				//time.min = GetTo(obj.time, obj.time.shift_to);
				time.min = GetToWithDataTime(obj.time, obj.time.shift_to, hand_auto);
				(retn, value1) = await data.CatDataGetAiMin(0, org_tag, time.year, time.mon, time.day, time.hour, time.min, EnumDataType.MAX).ConfigureAwait(false);
				if (retn)
				{
					time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					FitMin(time, obj.time.shift_from);
					time.min = GetFrom(obj.time, obj.time.shift_from);
					TimeUtil.MinusMin(ref time.year, ref time.mon, ref time.day, ref time.hour, ref time.min);
					(retn, value2) = await data.CatDataGetAiMin(0, org_tag, time.year, time.mon, time.day, time.hour, time.min, EnumDataType.MAX).ConfigureAwait(false);
					if (retn)
					{
						read_flag = true;
						read_count = 1;

                        if (value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
                            val = (TagLib.GetTagMemberFull(org_tag) + value1 - value2);
                        else
    						val = value1-value2;
					}
				}	
			}
			else 
			{
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetToWithDataTime(obj.time, shift, hand_auto); i++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitMin(time, shift);
						time.min = i;

						(retn, one_value) = await data.CatDataGetAiMin(0, org_tag, time.year, time.mon, time.day, time.hour, time.min, data_type).ConfigureAwait(false);
						if (!retn) { continue; }

						read_flag = true;
						read_count++;

						if(data_type == EnumDataType.AVE) 
						{
							val += one_value;
						}
						else if(data_type == EnumDataType.SUM) 
						{
							val += one_value;
						}
						else if(data_type == EnumDataType.MIN) 
						{
							if(read_count == 1) 
							{	// 처음으로 읽을때
								val = one_value;
							}
							else 
							{
								if(one_value < val)	val = one_value;
							}
						}
						else if(data_type == EnumDataType.MAX) 
						{
							if(read_count == 1) 
							{	// 처음으로 읽을때
								val = one_value;
							}
							else 
							{
								if(one_value > val)	val = one_value;
							}
						}
					}
				}
			}

			if(read_flag == false) 
			{
				cell_t.text = String.Format(ReportConfig.sNoData);
				return;
			}
	
			if(data_type == EnumDataType.AVE) 
			{	// 평균치는 평균값으로 계산한다.
				val = val/read_count;
			}

			MakeNumberByFormat(ref cell_t.text, cell_t.format, val);
		}

		//-------------------------------------------------------------------------
		// From 에서 To 까지의 시간자료를 계산한다.
		//-------------------------------------------------------------------------

		async Task ConvertAiDataHour(OBJECT_AI_ONE_DATA obj, CELL_STRUCT cell_t, EnumDataType data_type, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time;
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			double val, one_value;
			int	  i;
			int   shift;
			bool  read_flag = false;
			int  read_count = 0;
			string org_tag = "";

            GetOriginalTag(out org_tag, obj.tag);

			GetSelectedReportTime(user_time, hand_auto);

			val = 0;
			one_value = 0;
			bool retn = false;

			DataLocal data = new DataLocal();

			if(data_type == EnumDataType.SUB) 
			{
				double value1=0, value2=0;
				time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				FitHour(time, obj.time.shift_to);
				//time.hour = GetTo(obj.time, obj.time.shift_to);
				time.hour = GetToWithDataTime(obj.time, obj.time.shift_to, hand_auto);
				(retn, value1) = await data.CatDataGetAiHour(0, org_tag, time.year, time.mon, time.day, time.hour, 0, EnumDataType.MAX);
				if(retn)
					{
					time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					FitHour(time, obj.time.shift_from);
					time.hour = GetFrom(obj.time, obj.time.shift_from);
					TimeUtil.MinusHour(ref time.year, ref time.mon, ref time.day, ref time.hour);
					(retn, value2) = await data.CatDataGetAiHour(0, org_tag, time.year, time.mon, time.day, time.hour, 0, EnumDataType.MAX).ConfigureAwait(false);
					if(retn)
						{
						read_flag = true;
						read_count = 1;

                        if (value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
                            val = (TagLib.GetTagMemberFull(org_tag) + value1 - value2);
                        else
                            val = value1 - value2;
					}
				}	
			}
			else 
			{
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetToWithDataTime(obj.time, shift, hand_auto); i++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitHour(time, shift);
						time.hour = i;

						(retn, one_value) = await data.CatDataGetAiHour(0, org_tag, time.year, time.mon, time.day, time.hour, 0, data_type).ConfigureAwait(false);
							if(!retn) continue;

						read_flag = true;
						read_count++;

						if(data_type == EnumDataType.AVE) 
						{
							val += one_value;
						}
						else if(data_type == EnumDataType.SUM) 
						{
							val += one_value;
						}
						else if(data_type == EnumDataType.MIN) 
						{
							if(read_count == 1) 
							{	// 처음으로 읽을때
								val = one_value;
							}
							else 
							{
								if(one_value < val)	val = one_value;
							}
						}
						else if(data_type == EnumDataType.MAX) 
						{
							if(read_count == 1) 
							{	// 처음으로 읽을때
								val = one_value;
							}
							else 
							{
								if(one_value > val)	val = one_value;
							}
						}
					}
				}
			}

			if(read_flag == false) 
			{
				cell_t.text = String.Format(ReportConfig.sNoData);
				return;
			}
	
			if(data_type == EnumDataType.AVE) 
			{	// 평균치는 평균값으로 계산한다.
				val = val/read_count;
			}

			MakeNumberByFormat(ref cell_t.text, cell_t.format, val);
		}

		//-------------------------------------------------------------------------
		// From 에서 To 까지의 일자료를 계산한다.
		//-------------------------------------------------------------------------

		async Task ConvertAiDataDay(OBJECT_AI_ONE_DATA obj, CELL_STRUCT cell_t, EnumDataType data_type, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time;
			USER_SELECT_TIME user_time=new USER_SELECT_TIME();
			double val, one_value;
			int	  i;
			int   shift;
			bool  read_flag = false;
			int  read_count = 0;
			string org_tag="";

            GetOriginalTag(out org_tag, obj.tag);

			GetSelectedReportTime(user_time, hand_auto);

			val = 0;
			one_value = 0;
			bool retn = false;

			DataLocal data = new DataLocal();

			if(data_type == EnumDataType.SUB) 
			{
				double value1=0, value2=0;
				time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				FitDay(time, obj.time.shift_to);
				//time.day = GetTo(obj.time, obj.time.shift_to);
				time.day = GetToWithDataTime(obj.time, obj.time.shift_to, hand_auto);

                // 31로 일이 지정되어 있으면 30밖에 없는 달은 데이타 없음으로 나오므로 말일로 지정해서 값을 가져온다. 2011-12-26
                int day_limit = TimeUtil.getmonthlimit(time.year, time.mon);
                if (time.day > day_limit)
                {
                    time.day = day_limit;
                }

                if (ConfigViewMain.bReportStartHourOfDayMaxSub)
                {
                    DateTime t = new DateTime(time.year, time.mon, time.day);
                    t = t.AddHours(ConfigViewMain.nReportStartHourOfDay + 23);

					(retn,value1) = await data.CatDataGetAiHour(0, org_tag,  t.Year, t.Month, t.Day, t.Hour, 0, EnumDataType.MAX).ConfigureAwait(false);
                   
					if(retn){
                        time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
                        FitDay(time, obj.time.shift_from);
                        time.day = GetFrom(obj.time, obj.time.shift_from);

                        t = new DateTime(time.year, time.mon, time.day);
                        t = t.AddHours(ConfigViewMain.nReportStartHourOfDay + 23 -24);

						(retn, value2) = await data.CatDataGetAiHour(0, org_tag, t.Year, t.Month, t.Day, t.Hour, 0, EnumDataType.MAX).ConfigureAwait(false);
                        if(retn)
							{
                            read_flag = true;
                            read_count = 1;

                            if (value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
                                val = (TagLib.GetTagMemberFull(org_tag) + value1 - value2);
                            else
                                val = value1 - value2;
                        }
                    }
                }
                else
                {
                    (retn, value1) = await data.CatDataGetAiDay(0, org_tag, time.year, time.mon, time.day, 0, 0, EnumDataType.MAX);
                   if(retn)
						{
                        time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
                        FitDay(time, obj.time.shift_from);
                        time.day = GetFrom(obj.time, obj.time.shift_from);
                        TimeUtil.MinusDay(ref time.year, ref time.mon, ref time.day);
                        (retn, value2) = await data.CatDataGetAiDay(0, org_tag,  time.year, time.mon, time.day, 0, 0, EnumDataType.MAX);
                        if(retn)
							{
                            read_flag = true;
                            read_count = 1;

                            if (value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
                                val = (TagLib.GetTagMemberFull(org_tag) + value1 - value2);
                            else
                                val = value1 - value2;
                        }
                    }
                }
			}
			else 
			{
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetToWithDataTime(obj.time, shift, hand_auto); i++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitDay(time, shift);
						time.day = i;

                        (retn, one_value) = await data.CatDataGetAiDay(0, org_tag,  time.year, time.mon, time.day, 0, 0, data_type);
							if(!retn) continue;

						read_flag = true;
						read_count++;

						if(data_type == EnumDataType.AVE) 
						{
							val += one_value;
						}
						else if(data_type == EnumDataType.SUM) 
						{
							val += one_value;
						}
						else if(data_type == EnumDataType.MIN) 
						{
							if(read_count == 1) 
							{	// 처음으로 읽을때
								val = one_value;
							}
							else 
							{
								if(one_value < val)	val = one_value;
							}
						}
						else if(data_type == EnumDataType.MAX) 
						{
							if(read_count == 1) 
							{	// 처음으로 읽을때
								val = one_value;
							}
							else 
							{
								if(one_value > val)	val = one_value;
							}
						}
					}
				}
			}

			if(read_flag == false) 
			{
				cell_t.text = String.Format(ReportConfig.sNoData);
				return;
			}
	
			if(data_type == EnumDataType.AVE) 
			{	// 평균치는 평균값으로 계산한다.
				val = val/read_count;
			}

			MakeNumberByFormat(ref cell_t.text, cell_t.format, val);
		}

		//-------------------------------------------------------------------------
		// From 에서 To 까지의 일자료를 계산한다.
		//-------------------------------------------------------------------------

		async Task ConvertAiDataMon(OBJECT_AI_ONE_DATA obj, CELL_STRUCT cell_t, EnumDataType data_type, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time;
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			double val, one_value;
			int	  i;
	
			int   shift;
			bool  read_flag = false;
			int  read_count = 0;
			string org_tag = "";

            GetOriginalTag(out org_tag, obj.tag);

			GetSelectedReportTime(user_time, hand_auto);

			val = 0;
			one_value = 0;
			bool retn = false;

			DataLocal data = new DataLocal();

			if(data_type == EnumDataType.SUB) 
			{
				double value1 = 0, value2 = 0;
				time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				FitMon(time, obj.time.shift_to);
				//time.mon = GetTo(obj.time, obj.time.shift_to);
				time.mon = GetToWithDataTime(obj.time, obj.time.shift_to, hand_auto);

                (retn, value1) = await data.CatDataGetAiMonth(0, org_tag, time.year, time.mon, 1, 0, 0, EnumDataType.MAX);
				if(retn)
					{
					time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					FitMon(time, obj.time.shift_from);
					time.mon = GetFrom(obj.time, obj.time.shift_from);
					TimeUtil.MinusMonth(ref time.year, ref time.mon);
                    (retn, value2) = await data.CatDataGetAiMonth(0, org_tag, time.year, time.mon, 1, 0, 0, EnumDataType.MAX);
					if(retn)
						{
						read_flag = true;
						read_count = 1;

                        if (value1 < value2)	// 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
                            val = (TagLib.GetTagMemberFull(org_tag) + value1 - value2);
                        else
                            val = value1 - value2;
					}
				}	
			}
			else 
			{
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetToWithDataTime(obj.time, shift, hand_auto); i++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitMon(time, shift);
						time.mon = i;

                        (retn, one_value) = await data.CatDataGetAiMonth(0, org_tag, time.year, time.mon, 1, 0, 0, data_type);
							if(!retn) continue;

						read_flag = true;
						read_count++;

						if(data_type == EnumDataType.AVE) 
						{
							val += one_value;
						}
						else if(data_type == EnumDataType.SUM) 
						{
							val += one_value;
						}
						else if(data_type == EnumDataType.MIN) 
						{
							if(read_count == 1) 
							{	// 처음으로 읽을때
								val = one_value;
							}
							else 
							{
								if(one_value < val)	val = one_value;
							}
						}
						else if(data_type == EnumDataType.MAX) 
						{
							if(read_count == 1) 
							{	// 처음으로 읽을때
								val = one_value;
							}
							else 
							{
								if(one_value > val)	val = one_value;
							}
						}
					}
				}
			}

			if(read_flag == false) 
			{
				cell_t.text = String.Format(ReportConfig.sNoData);
				return;
			}
	
			if(data_type == EnumDataType.AVE) 
			{	// 평균치는 평균값으로 계산한다.
				val = val/read_count;
			}

			MakeNumberByFormat(ref cell_t.text, cell_t.format, val);
		}

		void CopyUserSelectTime(USER_SELECT_TIME t, USER_SELECT_TIME s)
		{
			t.year = s.year;
			t.mon = s.mon;
			t.day = s.day;
			t.hour = s.hour;
			t.min = s.min;
			t.sec = s.sec;
		}	

		async Task<(int read_count, double val)> ConvertAiDataMinMaxTimeMinFromTo(string tag, int read_count,  double val, USER_SELECT_TIME time, USER_SELECT_TIME tMinMax, int min_from, int min_to, EnumDataType data_type)
		{
			int i;
			double one_value = 0;
			bool retn = false;

			DataLocal data = new DataLocal();
	
			for(i = min_from; i <= min_to; i++) 
			{
				time.min = i;
				time.sec = 0;

				(retn, one_value) = await data.CatDataGetAiMin(0, tag, time.year, time.mon, time.day, time.hour, time.min, data_type);
					if(!retn) continue;

				read_count++;

				if(data_type == EnumDataType.AVE) 
				{
					val += one_value;
				}
				else if(data_type == EnumDataType.SUM) 
				{
					val += one_value;
				}
				else if(data_type == EnumDataType.MIN) 
				{
					if(read_count == 1) 
					{	// 처음으로 읽을때
						val = one_value;
						CopyUserSelectTime(tMinMax, time);
					}
					else 
					{
						if(one_value < val) 
						{
							val = one_value;
							CopyUserSelectTime(tMinMax, time);
						}
					}
				}
				else if(data_type == EnumDataType.MAX) 
				{
					if(read_count == 1) 
					{	// 처음으로 읽을때
						val = one_value;
						CopyUserSelectTime(tMinMax, time);
					}
					else 
					{
						if(one_value > val) 
						{
							val = one_value;
							CopyUserSelectTime(tMinMax, time);
						}
					}
				}

			}
			return (read_count, val);
		}

		async Task<(int read_count, double val)> ConvertAiDataMinMaxTimeHourFromTo(string tag, int read_count,  double val, USER_SELECT_TIME time, USER_SELECT_TIME tMinMax, int hour_from, int hour_to, EnumDataType data_type)
		{
			int i;
			double one_value = 0;
			bool retn = false;

			DataLocal data = new DataLocal();
	
			for(i = hour_from; i <= hour_to; i++) 
			{
				time.hour = i;
				time.min = 0;
				time.sec = 0;

				(retn, one_value) = await data.CatDataGetAiHour(0, tag, time.year, time.mon, time.day, time.hour, 0, data_type);
					if(!retn) continue;

				read_count++;

				if(data_type == EnumDataType.AVE) 
				{
					val += one_value;
				}
				else if(data_type == EnumDataType.SUM) 
				{
					val += one_value;
				}
				else if(data_type == EnumDataType.MIN) 
				{
					if(read_count == 1) 
					{	// 처음으로 읽을 때
						val = one_value;
						CopyUserSelectTime(tMinMax, time);
					}
					else 
					{
						if(one_value < val) 
						{
							val = one_value;
							CopyUserSelectTime(tMinMax, time);
						}
					}
				}
				else if(data_type == EnumDataType.MAX) 
				{
					if(read_count == 1) 
					{	// 처음으로 읽을 때
						val = one_value;
						CopyUserSelectTime(tMinMax, time);
					}
					else 
					{
						if(one_value > val) 
						{
							val = one_value;
							CopyUserSelectTime(tMinMax, time);
						}
					}
				}

			}
			return (read_count, val);
		}

		//-------------------------------------------------------------------------
		// From 에서 To 까지의 분자료를 계산한다.
		//-------------------------------------------------------------------------

		async Task ConvertAiDataMinMaxTime(OBJECT_AI_ONE_DATA obj, CELL_STRUCT cell_t, EnumDataType data_type, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time;
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			USER_SELECT_TIME tMinMax = new USER_SELECT_TIME();
			double val;
			int   shift;
			int   read_count = 0;
			int   day;
			string org_tag = "";

            GetOriginalTag(out org_tag, obj.tag);

			GetSelectedReportTime(user_time, hand_auto);

			//--------------------
			// 분 자료를 읽어온다.
			//--------------------

			val = 0;

			if(String.Compare(obj.time.zone, "Min", true) == 0) 
			{
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					FitMin(time, shift);
					(read_count, val) = await ConvertAiDataMinMaxTimeMinFromTo(org_tag, read_count, val, time, tMinMax, GetFrom(obj.time, shift), GetTo(obj.time, shift), data_type);
				}
			}
			else if(String.Compare(obj.time.zone, "Hour", true) == 0) 
			{
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					FitHour(time, shift);

					int from = GetFrom(obj.time, shift);
					int to = GetTo(obj.time, shift);

                    (read_count, val) = await ConvertAiDataMinMaxTimeHourFromTo(org_tag, read_count,  val, time, tMinMax, from, to, data_type);
				}

				tMinMax.min = 0;
				tMinMax.sec = 0;

				if(read_count > 0) 
				{
					int min_read_count = 0;
					time = (USER_SELECT_TIME)Tools.CopyObject(tMinMax);
                    (min_read_count, val) = await ConvertAiDataMinMaxTimeMinFromTo(org_tag, min_read_count,val, time, tMinMax, 0, 59, data_type);
				}
			}
			else if(String.Compare(obj.time.zone, "Day", true) == 0) 
			{
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(day = GetFrom(obj.time, shift); day <= GetTo(obj.time, shift); day++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitDay(time, shift);
						time.day = day;
                        (read_count, val) = await ConvertAiDataMinMaxTimeHourFromTo(org_tag, read_count, val, time, tMinMax, 0, 23, data_type);
					}
				}

				tMinMax.min = 0;
				tMinMax.sec = 0;

				if(read_count > 0) 
				{
					int min_read_count = 0;
					time = (USER_SELECT_TIME)Tools.CopyObject(tMinMax);
                    (min_read_count, val) = await ConvertAiDataMinMaxTimeMinFromTo(org_tag, min_read_count, val, time, tMinMax, 0, 59, data_type);
				}
			}
            else if (String.Compare(obj.time.zone, "Mon", true) == 0)
            {
                int mon;
                for (shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++)
                {
                    for (mon = GetFrom(obj.time, shift); mon <= GetTo(obj.time, shift); mon++)
                    {
                        time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
                        FitMon(time, shift);

                        int month_limit = TimeUtil.getmonthlimit(time.year, time.mon);

                        for (day = 1; day <= month_limit; day++)
                        {
                            time.mon = mon;
                            time.day = day;
                            (read_count, val) = await ConvertAiDataMinMaxTimeHourFromTo(org_tag, read_count, val, time, tMinMax, 0, 23, data_type);
                        }
                    }
                }

                tMinMax.min = 0;
                tMinMax.sec = 0;

                // 읽은 자료가 있으면 분자료에서 더 정확하게 읽어준다.
                if (read_count > 0)
                {
                    int min_read_count = 0;
                    time = (USER_SELECT_TIME)Tools.CopyObject(tMinMax);
                    (min_read_count, val) = await ConvertAiDataMinMaxTimeMinFromTo(org_tag, min_read_count, val, time, tMinMax, 0, 59, data_type);
                }
            }

			if(read_count == 0) 
			{
				cell_t.text = String.Format(ReportConfig.sNoData);
				return;
			}
	
			MakeTimeString(cell_t, tMinMax);
		}

		async Task<int> ConvertDiMomentOneData(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y, EnumHandAuto hand_auto)
		{
			OBJECT_MOMENT_DATA obj = new OBJECT_MOMENT_DATA();
			CELL_STRUCT cell_t;
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			USER_SELECT_TIME time;
			TREND_DI_STRUCT trend = new TREND_DI_STRUCT();
			bool retn = false;

			ReportLib.ObjectStringToStruct(ref obj, cell_s.text);
			cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);
			GetSelectedReportTime(user_time, hand_auto);

			string org_tag = "";
            GetOriginalTag(out org_tag, obj.tag);

			DataLocal data = new DataLocal();

			if(String.Compare(obj.time.zone, "Min") == 0) 
			{
				time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				time.min = obj.time.from;
				FitMin(time, obj.time.shift_from);

				retn = await data.LoadMinDataStructDI(org_tag, time.year, time.mon, time.day, time.hour, time.min, trend);
			}
			else if(String.Compare(obj.time.zone, "Hour") == 0) 
			{
				time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				time.hour = obj.time.from;
				time.min = obj.min;
				FitHour(time, obj.time.shift_from);

				retn = await data.LoadMinDataStructDI(org_tag, time.year, time.mon, time.day, time.hour, time.min, trend);
			}
			else if(String.Compare(obj.time.zone, "Day") == 0) 
			{
				time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				time.day = obj.time.from;
				time.hour = obj.hour;
				time.min = obj.min;
				FitDay(time, obj.time.shift_from);

				retn = await data.LoadMinDataStructDI(org_tag, time.year, time.mon, time.day, time.hour, time.min, trend);
			}
			else if(String.Compare(obj.time.zone, "Mon") == 0) 
			{
				time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
				time.mon = obj.time.from;
				time.day = obj.day;
				time.hour = obj.hour;
				time.min = obj.min;
				FitMon(time, obj.time.shift_from);

				retn = await data.LoadMinDataStructDI(org_tag, time.year, time.mon, time.day, time.hour, time.min, trend);
			}
			else 
			{
				cell_t.text = Message알수없는시간형식(obj.time.zone);
			}

			if(retn == false) 
			{
				cell_t.text = String.Format(ReportConfig.sNoData);
				return 1;
			}
	
			MakeNumberByFormat(ref cell_t.text, cell_t.format, trend.bOnOff);

			return 1;

			// return 1 이라는 것은 성공이라는 뜻이 아니라 1줄이 증가했다는 뜻이다.
		}

		async Task<int> ConvertDiMultiMomentData(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time;
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			OBJECT_MOMENT_DATA obj = new OBJECT_MOMENT_DATA();
			int	  i;
			CELL_STRUCT cell_t;
			int   cell_y;
			int   shift;
			TREND_DI_STRUCT trend = new TREND_DI_STRUCT();
			bool retn = false;

			ReportLib.ObjectStringToStruct(ref obj, cell_s.text);

			GetSelectedReportTime(user_time, hand_auto);

			string org_tag = "";
            GetOriginalTag(out org_tag, obj.tag);

			DataLocal data = new DataLocal();

			// 분 자료를 읽어온다.
			if(String.Compare(obj.time.zone, "Min") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitMin(time, shift);
						time.min = i;
						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
						(retn) = await data.LoadMinDataStructDI(org_tag, time.year, time.mon, time.day, time.hour, time.min, trend);
						if(!retn)
						{
							cell_t.text = ReportConfig.sNoData;
						}
						else 
						{
							MakeNumberByFormat(ref cell_t.text, cell_t.format, trend.bOnOff);
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
				// 시간 자료를 읽어온다.
			else if(String.Compare(obj.time.zone, "Hour") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitHour(time, shift);
						time.hour = i;
						time.min = obj.min;
						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
						retn = await data.LoadMinDataStructDI(org_tag, time.year, time.mon, time.day, time.hour, time.min, trend);
						if (!retn)
						{
							cell_t.text = ReportConfig.sNoData;
						}
						else
						{
							MakeNumberByFormat(ref cell_t.text, cell_t.format, trend.bOnOff);
						}
				
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
				// 일 자료를 읽어온다.
			else if(String.Compare(obj.time.zone, "Day") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitDay(time, shift);
						time.day = i;
						time.hour = obj.hour;
						time.min = obj.min;

						if(TimeUtil.IsDayExist(time.year, time.mon, time.day)) 
						{
							cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
							retn = await data.LoadMinDataStructDI(org_tag, time.year, time.mon, time.day, time.hour, time.min, trend);
							if(!retn)
							{
								cell_t.text = ReportConfig.sNoData;
							}
							else 
							{
								MakeNumberByFormat(ref cell_t.text, cell_t.format, trend.bOnOff);
							}
							cell_y++;
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
				// 월 자료를 읽어온다.
			else if(String.Compare(obj.time.zone, "Mon") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitMon(time, shift);
						time.mon = i;
						time.day = obj.day;
						time.hour = obj.hour;
						time.min = obj.min;
						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
						if(!await data.LoadMinDataStructDI(org_tag, time.year, time.mon, time.day, time.hour, time.min, trend)) 
						{
							cell_t.text = ReportConfig.sNoData;
						}
						else 
						{
							MakeNumberByFormat(ref cell_t.text, cell_t.format, trend.bOnOff);
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
			else 
			{
				cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);
				cell_t.text = Message알수없는시간형식(obj.time.zone);
				return 1;
			}
		}

		public static int CompareTimeMin(USER_SELECT_TIME fr, USER_SELECT_TIME to)
		{
			if(fr.year != to.year)	return fr.year-to.year;
			if(fr.mon  != to.mon)		return fr.mon-to.mon;
			if(fr.day  != to.day)		return fr.day-to.day;
			if(fr.hour != to.hour)	return fr.hour-to.hour;
			if(fr.min  != to.min)		return fr.min-to.min;

			return 0;
		}

		int CompareTimeHour(USER_SELECT_TIME fr, USER_SELECT_TIME to)
		{
			if(fr.year != to.year)	return fr.year-to.year;
			if(fr.mon  != to.mon)		return fr.mon-to.mon;
			if(fr.day  != to.day)		return fr.day-to.day;
			if(fr.hour != to.hour)	return fr.hour-to.hour;

			return 0;
		}

		int CompareTimeDay(USER_SELECT_TIME fr, USER_SELECT_TIME to)
		{
			if(fr.year != to.year)	return fr.year-to.year;
			if(fr.mon  != to.mon)		return fr.mon-to.mon;
			if(fr.day  != to.day)		return fr.day-to.day;

			return 0;
		}

		int CompareTimeMon(USER_SELECT_TIME fr, USER_SELECT_TIME to)
		{
			if(fr.year != to.year)	return fr.year-to.year;
			if(fr.mon  != to.mon)		return fr.mon-to.mon;

			return 0;
		}

		async Task<(bool, double val)> GetMaxSumFromTo(USER_SELECT_TIME from_org, USER_SELECT_TIME to, OBJECT_AI_MAX_SUM obj)
		{
			double read_value = 0;
			TREND_AI_STRUCT trend = new TREND_AI_STRUCT();
			USER_SELECT_TIME from;
			bool retn;
			int count = 0;
			int nTerminal = 0;

			double val = 0;

			from = (USER_SELECT_TIME)Tools.CopyObject(from_org);

			string org_tag = "";
            GetOriginalTag(out org_tag, obj.tag);

			DataLocal data = new DataLocal();

			if(obj.nDataType == 0) 
			{	// 분 자료에서 최대값 더하기.
				while(CompareTimeMin(from, to) <= 0) 
				{
					retn = await data.LoadMinDataStructAI(org_tag, from.year, from.mon, from.day, from.hour, from.min, trend);
					if(retn) 
					{
						count++;
						val += trend.fMax;
					}
					TimeUtil.PlusMin(ref from.year, ref from.mon, ref from.day, ref from.hour, ref from.min);
				}
			}
			else if(obj.nDataType == 1) 
			{	// 시간 자료에서 최대값 더하기.
				while(CompareTimeHour(from, to) <= 0) 
				{
					(retn, read_value) = await data.CatDataGetAiHour(nTerminal, org_tag, from.year, from.mon, from.day, from.hour, 0, EnumDataType.MAX);
					if(retn) 
					{
						count++;
						val += read_value;
					}
					TimeUtil.PlusHour(ref from.year, ref from.mon, ref from.day, ref from.hour);
				}
			}
			else if(obj.nDataType == 2) 
			{	// 일 자료에서 최대값 더하기.
				while(CompareTimeDay(from, to) <= 0) 
				{
					(retn, read_value) = await data.CatDataGetAiDay(nTerminal, org_tag, from.year, from.mon, from.day, 0, 0, EnumDataType.MAX);
					if(retn) 
					{
						count++;
						val += read_value;
					}
					TimeUtil.PlusDay(ref from.year, ref from.mon, ref from.day);
				}
			}
			else if(obj.nDataType == 3) 
			{	// 월 자료에서 최대값 더하기.
				while(CompareTimeMon(from, to) <= 0) 
				{
					(retn, read_value) = await  data.CatDataGetAiMonth(nTerminal, org_tag, from.year, from.mon, 1, 0, 0, EnumDataType.MAX);
					if(retn) 
					{
						count++;
						val += read_value;
					}
					TimeUtil.PlusMonth(ref from.year, ref from.mon);
				}
			}

			if(count > 0) 
			{
				return (true, val);
			}

			return (false, val);
		}

		void FitMinWithDataTime(USER_SELECT_TIME time, int shift, CELL_TIME obj, EnumHandAuto hand_auto)
		{
			if(obj.bToDataTime == 1) 
			{
				GetSelectedReportTime(time, hand_auto);
			}
			else 
			{
				FitMin(time, shift);
			}
		}

		void FitHourWithDataTime(USER_SELECT_TIME time, int shift, CELL_TIME obj, EnumHandAuto hand_auto)
		{
			if(obj.bToDataTime == 1) 
			{
				GetSelectedReportTime(time, hand_auto);
			}
			else 
			{
				FitHour(time, shift);
			}
		}
		
		void FitDayWithDataTime(USER_SELECT_TIME time, int shift, CELL_TIME obj, EnumHandAuto hand_auto)
		{
			if(obj.bToDataTime == 1) 
			{
				GetSelectedReportTime(time, hand_auto);
			}
			else 
			{
				FitDay(time, shift);
			}
		}

		void FitMonWithDataTime(USER_SELECT_TIME time, int shift, CELL_TIME obj, EnumHandAuto hand_auto)
		{
			if(obj.bToDataTime == 1) 
			{
				GetSelectedReportTime(time, hand_auto);
			}
			else 
			{
				FitMon(time, shift);
			}
		}

		//-------------------------------------------------------------------------
		// From 에서 To 까지의 분자료를 계산한다.
		//-------------------------------------------------------------------------

		async Task<(bool, double val)> GetAiMinDataFromGab(string tag, USER_SELECT_TIME org_time, int min_gab, EnumDataType data_type)
		{
			USER_SELECT_TIME time;
			double one_value;
			int	  i;
			bool  read_flag = false;
			int   read_count = 0;
			bool retn = false;

			time = (USER_SELECT_TIME)Tools.CopyObject(org_time);

			//--------------------
			// 분 자료를 읽어온다.
			double val = 0;
			one_value = 0;

			DataLocal data = new DataLocal();

			if (data_type == EnumDataType.SUB)
			{
				double value1 = 0;
				double value2 = 0;

				(retn, value2) = await GetMaxValueFromMinsData(tag, time.year, time.mon, time.day, time.hour, time.min, min_gab);
				if (!retn) return (false, val);

				for (i = 0; i < min_gab; i++)
				{
					TimeUtil.MinusMin(ref time.year, ref time.mon, ref time.day, ref time.hour, ref time.min);
				}

				(retn, value1) = await GetMaxValueFromMinsData(tag, time.year, time.mon, time.day, time.hour, time.min, min_gab);
				if (!retn) return (false, val);

				if (value2 < value1)    // 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
					val = (TagLib.GetTagMemberFull(tag) + value2 - value1);
				else
					val = value2 - value1;

				return (true, val);
			}
			else if (data_type == EnumDataType.MOMENT)
			{
				(retn, one_value) = await data.CatDataGetAiMin(0, tag, time.year, time.mon, time.day, time.hour, time.min, data_type);
				if (retn)
				{
					val = one_value;
					return (true, val);
				}
				else
				{
					return (false, val);
				}
			}
			else
			{
				for (i = 0; i < min_gab; i++)
				{
					(retn, one_value) = await data.CatDataGetAiMin(0, tag, time.year, time.mon, time.day, time.hour, time.min, data_type);
					{
						read_flag = true;
						read_count++;

						if (data_type == EnumDataType.AVE)
						{
							val += one_value;
						}
						else if (data_type == EnumDataType.SUM)
						{
							val += one_value;
						}
						else if (data_type == EnumDataType.MIN)
						{
							if (read_count == 1)
							{   // 처음으로 읽을때
								val = one_value;
							}
							else
							{
								if (one_value < val) val = one_value;
							}
						}
						else if (data_type == EnumDataType.MAX)
						{
							if (read_count == 1)
							{   // 처음으로 읽을때
								val = one_value;
							}
							else
							{
								if (one_value > val) val = one_value;
							}
						}
					}
					TimeUtil.PlusMin(ref time.year, ref time.mon, ref time.day, ref time.hour, ref time.min);
				}
			}

			if(read_flag == false) 
			{
				return (false, val);
			}
	
			if(data_type == EnumDataType.AVE) 
			{	// 평균치는 평균값으로 계산한다.
				val = val/read_count;
			}

			return (true, val);
		}

        async Task<(bool, double value)> GetMaxValueFromMinsData(string tag, int year, int mon, int day, int hour, int min, int min_gab)
        {
            bool read_flag = false;
            double value2 = 0;
            DataLocal data = new DataLocal();
			bool retn = false;
			double value = 0;

            for (int i = 0; i < min_gab; i++)
            {
				(retn, value2) = await data.CatDataGetAiMin(0, tag, year, mon, day, hour, min, EnumDataType.MAX);
                if(retn)
					{
                    if (read_flag == false)
                    {	// 처음으로 읽을때
                        read_flag = true;
                        value = value2;
                    }
                    else
                    {
                        if (value2 > value) value = value2;
                    }
                }

                TimeUtil.PlusMin(ref year, ref mon, ref day, ref hour, ref min);
            }

            return (read_flag, value);
        }

        async Task<( bool, double value)> GetMaxValueFromHoursData(string tag, int year, int mon, int day, int hour, int hour_gab)
        {
            bool read_flag = false;
            double value2 = 0;
            DataLocal data = new DataLocal();
			bool retn = false;
			double value = 0;

            for (int i = 0; i < hour_gab; i++)
            {
				(retn, value2) = await data.CatDataGetAiHour(0, tag, year, mon, day, hour, 0, EnumDataType.MAX);
                if(retn)
				{
                    if (read_flag == false)
                    {	// 처음으로 읽을때
                        read_flag = true;
                        value = value2;
                    }
                    else
                    {
                        if (value2 > value) value = value2;
                    }
                }

                TimeUtil.PlusHour(ref year, ref mon, ref day, ref hour);
            }

            return (read_flag, value);
        }

		//-------------------------------------------------------------------------
		// From 에서 To 까지의 시간자료를 계산한다.
		//-------------------------------------------------------------------------

		async Task<(bool, double val)> GetAiHourDataFromGab(string tag, USER_SELECT_TIME org_time, int hour_gab, EnumDataType data_type)
		{
			USER_SELECT_TIME time;
			double one_value;
			int	  i;
			bool  read_flag = false;
			int   read_count = 0;
			bool retn = false;

			time = (USER_SELECT_TIME)Tools.CopyObject(org_time);

			//--------------------
			// 분 자료를 읽어온다.
			double val = 0;
			one_value = 0;

			DataLocal data = new DataLocal();

			if (data_type == EnumDataType.SUB)
			{
				double value1 = 0;
				double value2 = 0;

				(retn, value2) = await GetMaxValueFromHoursData(tag, time.year, time.mon, time.day, time.hour, hour_gab);
				if (!retn) return (false, val);

				for (i = 0; i < hour_gab; i++)
				{
					TimeUtil.MinusHour(ref time.year, ref time.mon, ref time.day, ref time.hour);
				}

				(retn, value1) = await GetMaxValueFromHoursData(tag, time.year, time.mon, time.day, time.hour, hour_gab);
				if (!retn) return (false, val);

				if (value2 < value1)    // 값이 -가 나오면 Tag의 Full값을 더한 다음 빼준다.
					val = (TagLib.GetTagMemberFull(tag) + value2 - value1);
				else
					val = value2 - value1;

				return (true, val);
			}
			else if (data_type == EnumDataType.MOMENT)
			{
				(retn, one_value) = await data.CatDataGetAiMin(0, tag, time.year, time.mon, time.day, time.hour, 0, data_type);
				if (retn)
				{
					val = one_value;
					return (true, val);
				}
				else
				{
					return (false, val);
				}
			}
			else
			{
				for (i = 0; i < hour_gab; i++)
				{
					(retn, one_value) = await data.CatDataGetAiHour(0, tag, time.year, time.mon, time.day, time.hour, 0, data_type);
					if (retn)
					{
						read_flag = true;
						read_count++;

						if (data_type == EnumDataType.AVE)
						{
							val += one_value;
						}
						else if (data_type == EnumDataType.SUM)
						{
							val += one_value;
						}
						else if (data_type == EnumDataType.MIN)
						{
							if (read_count == 1)
							{   // 처음으로 읽을때
								val = one_value;
							}
							else
							{
								if (one_value < val) val = one_value;
							}
						}
						else if (data_type == EnumDataType.MAX)
						{
							if (read_count == 1)
							{   // 처음으로 읽을때
								val = one_value;
							}
							else
							{
								if (one_value > val) val = one_value;
							}
						}
					}
					TimeUtil.PlusHour(ref time.year, ref time.mon, ref time.day, ref time.hour);
				}
			}

			if(read_flag == false) 
			{
				return (false, val);
			}
	
			if(data_type == EnumDataType.AVE) 
			{	// 평균치는 평균값으로 계산한다.
				val = val/read_count;
			}

			return (true, val);
		}

		int GetVarValue(string var, int min, int max)
		{
			int val;

			if(var[0] == '$') 
			{
				double float_value = 0;
                if (!StringVar.GetStringVar(var.Substring(1), out float_value)) float_value = 0;

				val = (int)float_value;
			}
			else 
			{
				val = ConvertTool.ToInt32(var);
			}

			if(val < min)	val = min;
			if(val > max)	val = max;

			return val;
		}

		

		int ConvertMultiCount(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time, user_time = new USER_SELECT_TIME();
			OBJECT_ETC_MULTI_COUNT obj = new OBJECT_ETC_MULTI_COUNT();
			CELL_STRUCT cell_t;
			int   i, cell_y;
			int shift;

			ReportLib.ObjectStringToStruct(ref obj, cell_s.text);

			GetSelectedReportTime(user_time, hand_auto);

			if(String.Compare(obj.time.zone, "Min") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitMin(time, shift);
						time.min = i;
						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
						MakeTimeString(cell_t, time);
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
			else if(String.Compare(obj.time.zone, "Hour") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitHour(time, shift);
						time.hour = i;
						time.min = 0;
						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
						MakeTimeString(cell_t, time);
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
			else if(String.Compare(obj.time.zone, "Day") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitDay(time, shift);
						time.day = i;
						time.hour = 0;
						time.min = 0;
						if(TimeUtil.IsDayExist(time.year, time.mon, time.day)) 
						{
							cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
							MakeTimeString(cell_t, time);
							cell_y++;
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
			else if(String.Compare(obj.time.zone, "Mon") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitMon(time, shift);
						time.mon = i;
						time.day = 1;
						time.hour = 0;
						time.min = 0;
						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
						MakeTimeString(cell_t, time);
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}

			return 1;
		}

		void MakeDiDataString(ref string text, DISPLAY_FORMAT_STRUCT format, EnumDataType data_type, int val)
		{
			if(data_type == EnumDataType.ONTIME) 
			{
				MakeTimeCountString(ref text, format, val);	
			}
			else if(data_type == EnumDataType.OFFTIME) 
			{
				MakeTimeCountString(ref text, format, val);	
			}
			else if(data_type == EnumDataType.COUNT) 
			{
				text = String.Format( "{0}", val);
			}
			else 
			{
				text = String.Format( "{0}", val);
			}
		}

		async Task ConvertDiDataMin(OBJECT_DI_ONE_DATA obj, CELL_STRUCT cell_t, EnumDataType data_type, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time;
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			uint val, one_value;
			int	  i;
			int   shift;
			bool  read_flag = false;
			int   read_count = 0;
			bool retn = false;

			GetSelectedReportTime(user_time, hand_auto);

			//--------------------
			// 분 자료를 읽어온다.
			val = 0;
			one_value = 0;

			string org_tag = "";
            GetOriginalTag(out org_tag, obj.tag);

			DataLocal data = new DataLocal();

			for (shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++)
			{
				for (i = GetFrom(obj.time, shift); i <= GetToWithDataTime(obj.time, shift, hand_auto); i++)
				{
					time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					FitMin(time, shift);
					time.min = i;

					(retn, one_value) = await data.CatDataGetDiMin(0, org_tag, time.year, time.mon, time.day, time.hour, time.min, data_type);
					if (!retn) continue;

					read_flag = true;
					read_count++;

					val += one_value;
				}
			}

			if(read_flag == false) 
			{
				cell_t.text = String.Format(ReportConfig.sNoData);
				return;
			}
	
			MakeDiDataString(ref cell_t.text, cell_t.format, data_type, (int)val);
		}

		async Task ConvertDiDataHour(OBJECT_DI_ONE_DATA obj, CELL_STRUCT cell_t, EnumDataType data_type, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time;
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			uint val, one_value;
			int	  i;
			int   shift;
			bool  read_flag = false;
			int   read_count = 0;
			bool retn = false;

			GetSelectedReportTime(user_time, hand_auto);

			//--------------------
			// 분 자료를 읽어온다.
			val = 0;
			one_value = 0;

			string org_tag = "";
            GetOriginalTag(out org_tag, obj.tag);

			DataLocal data = new DataLocal();

			for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
			{
				for(i = GetFrom(obj.time, shift); i <= GetToWithDataTime(obj.time, shift, hand_auto); i++) 
				{
					time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					FitHour(time, shift);
					time.hour = i;

					(retn, one_value) = await data.CatDataGetDiHour(0, org_tag, time.year, time.mon, time.day, time.hour, data_type);
						if(!retn) continue;

					read_flag = true;
					read_count++;

					val += one_value;
				}
			}

			if(read_flag == false) 
			{
				cell_t.text = String.Format(ReportConfig.sNoData);
				return;
			}
	
			MakeDiDataString(ref cell_t.text, cell_t.format, data_type, (int)val);
		}

		async Task ConvertDiDataDay(OBJECT_DI_ONE_DATA obj, CELL_STRUCT cell_t, EnumDataType data_type, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time;
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			uint val, one_value;
			int	  i;
			int   shift;
			bool  read_flag = false;
			int   read_count = 0;
			bool retn = false;

			GetSelectedReportTime(user_time, hand_auto);

			//--------------------
			// 분 자료를 읽어온다.
			val = 0;
			one_value = 0;

			string org_tag = "";
            GetOriginalTag(out org_tag, obj.tag);

			DataLocal data = new DataLocal();

			for (shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++)
			{
				for (i = GetFrom(obj.time, shift); i <= GetToWithDataTime(obj.time, shift, hand_auto); i++)
				{
					time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					FitDay(time, shift);
					time.day = i;

					(retn, one_value) = await data.CatDataGetDiDay(0, org_tag, time.year, time.mon, time.day, data_type);
					if (!retn) continue;

					read_flag = true;
					read_count++;

					val += one_value;
				}
			}

			if(read_flag == false) 
			{
				cell_t.text = String.Format(ReportConfig.sNoData);
				return;
			}

			MakeDiDataString(ref cell_t.text, cell_t.format, data_type, (int)val);
		}

		async Task ConvertDiDataMon(OBJECT_DI_ONE_DATA obj, CELL_STRUCT cell_t, EnumDataType data_type, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time;
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			uint val, one_value;
			int	  i;
			int   shift;
			bool  read_flag = false;
			int   read_count = 0;
			bool retn = false;

			GetSelectedReportTime(user_time, hand_auto);

			//--------------------
			// 분 자료를 읽어온다.
			val = 0;
			one_value = 0;

			string org_tag = "";
			GetOriginalTag(out org_tag, obj.tag);

			DataLocal data = new DataLocal();

			for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
			{
				for(i = GetFrom(obj.time, shift); i <= GetToWithDataTime(obj.time, shift, hand_auto); i++) 
				{
					time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					FitMon(time, shift);
					time.mon = i;

					(retn, one_value) = await data.CatDataGetDiMonth(0, org_tag, time.year, time.mon, data_type);
						if(!retn) continue;

					read_flag = true;
					read_count++;

					val += one_value;
				}
			}

			if(read_flag == false) 
			{
				cell_t.text = String.Format(ReportConfig.sNoData);
				return;
			}

			MakeDiDataString(ref cell_t.text, cell_t.format, data_type, (int)val);
		}

		async Task<int> ConvertDiMultiData(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y, EnumDataType data_type, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time;
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			OBJECT_DI_MULTI_DATA obj = GetDiMultiDataStruct(cell_s.text);
			uint val = 0;
			int	  i;
			CELL_STRUCT cell_t;
			int   cell_y;
			int   shift;
			bool retn = false;

			GetSelectedReportTime(user_time, hand_auto);

			string org_tag = "";
			GetOriginalTag(out org_tag, obj.tag);

			DataLocal data = new DataLocal();

			// 분 자료를 읽어온다.
			if(String.Compare(obj.time.zone, "Min") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitMin(time, shift);
						time.min = i;
						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
						(retn, val) = await data.CatDataGetDiMin(0, org_tag, time.year, time.mon, time.day, time.hour, time.min, data_type);
						if(!retn)
						{
							cell_t.text = ReportConfig.sNoData;
						}
						else 
						{
							MakeDiDataString(ref cell_t.text, cell_t.format, data_type, (int)val);
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
				// 시간 자료를 읽어온다.
			else if(String.Compare(obj.time.zone, "Hour") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitHour(time, shift);
						time.hour = i;
						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
						(retn, val) = await data.CatDataGetDiHour(0, org_tag, time.year, time.mon, time.day, time.hour, data_type);
						if(!retn)
						{
							cell_t.text = ReportConfig.sNoData;
						}
						else 
						{
							MakeDiDataString(ref cell_t.text, cell_t.format, data_type, (int)val);
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
				// 일 자료를 읽어온다.
			else if(String.Compare(obj.time.zone, "Day") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitDay(time, shift);
						time.day = i;
						if(TimeUtil.IsDayExist(time.year, time.mon, time.day)) 
						{
							cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
							(retn, val) = await data.CatDataGetDiDay(0, org_tag, time.year, time.mon, time.day, data_type);
							if(retn)
								{
								cell_t.text = ReportConfig.sNoData;
							}
							else 
							{
								MakeDiDataString(ref cell_t.text, cell_t.format, data_type, (int)val);
							}
							cell_y++;
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
				// 월 자료를 읽어온다.
			else if(String.Compare(obj.time.zone, "Mon") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitMon(time, shift);
						time.mon = i;
						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
						(retn, val) = await data.CatDataGetDiMonth(0, org_tag,time.year, time.mon, data_type);
						if(!retn){
							cell_t.text = ReportConfig.sNoData;
						}
						else 
						{
							MakeDiDataString(ref cell_t.text, cell_t.format, data_type, (int)val);
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
			else 
			{
				cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);
				cell_t.text = Message알수없는시간형식(obj.time.zone);
				return 1;
			}
		}

		void StringToTime(ref DateTime t, string str)
		{
			CommaBlockString comma = new CommaBlockString();

			comma.SetBlockCode(':');

			comma.Set(str);
			int hour=0, min=0, sec=0;
			comma.GetInt(ref hour);
			comma.GetInt(ref min);
			comma.GetInt(ref sec);

            if (hour < 0 || hour > 23) hour = 0;
            if (min < 0 || min > 59) min = 0;
            if (sec < 0 || sec > 59) sec = 0;

			t = new DateTime(t.Year, t.Month, t.Day, hour, min, sec);
		}

		void StringToDate(ref DateTime d, string str)
		{
			CommaBlockString comma = new CommaBlockString();

			comma.SetBlockCode('-');

			comma.Set(str);
			int year=0, mon=0, day=0;
			comma.GetInt(ref year);
			comma.GetInt(ref mon);
			comma.GetInt(ref day);

            if(year < 1)    year = 1;
            if (mon < 1 || mon > 12) mon = 1;
            if (day < 1 || day > 31) day = 1;

			d = new DateTime(year, mon, day, d.Hour, d.Minute, d.Second);
		}

		//--------------------------------------------------------------------------
		//	주어진 태그가 포함되는가를 검사한다. *, DI*, ??ABCDE 등 허용.
		//--------------------------------------------------------------------------

		bool IsTagInclude(string tag_org, string tag)
		{
			int i;
	
			for(i = 0; i < tag_org.Length; i++) 
			{
				if(tag_org[i] == '*')	return true;
				if(tag_org[i] == '?')	continue;
				if(tag.Length <= i)		return false;
				if(tag_org[i] != tag[i])	return false;
			}

			if(tag_org.Length != tag.Length)	return false;

			return true;
		}

		bool IsMinInclude(USER_SELECT_TIME time_from, USER_SELECT_TIME time_to, DateTime t) 
		{
			long from = TimeUtil.GetMinHap(time_from.year, time_from.mon, time_from.day, time_from.hour, time_from.min);
			long to   = TimeUtil.GetMinHap(time_to.year,   time_to.mon,   time_to.day, time_to.hour, time_to.min);
			long curr = TimeUtil.GetMinHap(t.Year, t.Month, t.Day, t.Hour, t.Minute);

			if(curr >= from && curr <= to)	return true;

			return false;
		}

		bool IsMinInclude(USER_SELECT_TIME time_from, USER_SELECT_TIME time_to, SYSTEMTIME t) 
		{
			long from = TimeUtil.GetMinHap(time_from.year, time_from.mon, time_from.day, time_from.hour, time_from.min);
			long to   = TimeUtil.GetMinHap(time_to.year,   time_to.mon,   time_to.day, time_to.hour, time_to.min);
			long curr = TimeUtil.GetMinHap(t.wYear, t.wMonth, t.wDay, t.wHour, t.wMinute);

			if(curr >= from && curr <= to)	return true;

			return false;
		}

		/*
		bool IsMinInclude(USER_SELECT_TIME time_from, USER_SELECT_TIME time_to, DateTime tSystem)
		{
			DateTime t;

			d.da_year = tSystem->wYear;
			d.da_mon = (char)tSystem->wMonth;
			d.da_day = (char)tSystem->wDay;
			t.ti_hour = (char)tSystem->wHour;
			t.ti_min = (char)tSystem->wMinute;
			t.ti_sec = (char)tSystem->wSecond;
			t.ti_hund = tSystem->wMilliseconds/10;

			return IsMinInclude(time_from, time_to, &d, &t);
		}
		*/

		bool IsHourInclude(USER_SELECT_TIME time_from, USER_SELECT_TIME time_to, DateTime t) 
		{
			long from = TimeUtil.GetHourHap(time_from.year, time_from.mon, time_from.day, time_from.hour);
			long to   = TimeUtil.GetHourHap(time_to.year,   time_to.mon,   time_to.day, time_to.hour);
			long curr = TimeUtil.GetHourHap(t.Year, t.Month, t.Day, t.Hour);

			if(curr >= from && curr <= to)	return true;

			return false;
		}

		bool IsHourInclude(USER_SELECT_TIME time_from, USER_SELECT_TIME time_to, SYSTEMTIME t) 
		{
			long from = TimeUtil.GetHourHap(time_from.year, time_from.mon, time_from.day, time_from.hour);
			long to   = TimeUtil.GetHourHap(time_to.year,   time_to.mon,   time_to.day, time_to.hour);
			long curr = TimeUtil.GetHourHap(t.wYear, t.wMonth, t.wDay, t.wHour);

			if(curr >= from && curr <= to)	return true;

			return false;
		}

		/*
		int IsHourInclude(USER_SELECT_TIME time_from, USER_SELECT_TIME time_to, Date *tSystem)
		{
			DateTime t;

			d.da_year = tSystem->wYear;
			d.da_mon = (char)tSystem->wMonth;
			d.da_day = (char)tSystem->wDay;
			t.ti_hour = (char)tSystem->wHour;
			t.ti_min = (char)tSystem->wMinute;
			t.ti_sec = (char)tSystem->wSecond;
			t.ti_hund = tSystem->wMilliseconds/10;

			return IsMinInclude(time_from, time_to, &d, &t);
		}
		*/

		bool IsDayInclude(USER_SELECT_TIME time_from, USER_SELECT_TIME time_to, DateTime d) 
		{
			long from = TimeUtil.GetDayHap(time_from.year, time_from.mon, time_from.day);
			long to   = TimeUtil.GetDayHap(time_to.year,   time_to.mon,   time_to.day);
			long curr = TimeUtil.GetDayHap(d.Year, d.Month, d.Day);

			if(curr >= from && curr <= to)	return true;

			return false;
		}

		bool IsDayInclude(USER_SELECT_TIME time_from, USER_SELECT_TIME time_to, datetime d) 
		{
			long from = TimeUtil.GetDayHap(time_from.year, time_from.mon, time_from.day);
			long to   = TimeUtil.GetDayHap(time_to.year,   time_to.mon,   time_to.day);
			long curr = TimeUtil.GetDayHap(d.da_year, d.da_mon, d.da_day);

			if(curr >= from && curr <= to)	return true;

			return false;
		}

		bool IsMonInclude(USER_SELECT_TIME time_from, USER_SELECT_TIME time_to, DateTime d) 
		{
			long from = TimeUtil.GetMonHap(time_from.year, time_from.mon);
			long to   = TimeUtil.GetMonHap(time_to.year,   time_to.mon);
			long curr = TimeUtil.GetMonHap(d.Year, d.Month);

			if(curr >= from && curr <= to)	return true;

			return false;
		}

		void AddOnOffListBlock(ArrayList block, int obj_field_view, string data, DateTime t)
		{
			CommaBlockString comma = new CommaBlockString();
			int i;
			ON_OFF_LIST_SORT item = new ON_OFF_LIST_SORT();

			if(obj_field_view == -1) 
			{
				item = new ON_OFF_LIST_SORT();
				item.imsi = String.Format("{0}", block.Count+1);
                //item.t = t;
				block.Add(item);
				return;
			}

			comma.Set(data);	
			for(i = 0; i <= obj_field_view;i++) 
			{
				item = new ON_OFF_LIST_SORT();
				comma.GetString(ref item.imsi);
			}
            item.t = t;     // 2020-6-25 정렬 기능이 동작 안되어서 확인해 보니 이부분이 빠져 있었다. 전부 같은 시간이어서 정렬이 안되었다.

			block.Add(item);
		}

		int CompareDateTimeSec(DateTime t1, DateTime t2)
		{
			if(t1.Year != t2.Year)	return t1.Year-t2.Year;
			if(t1.Month != t2.Month)	return t1.Month-t2.Month;
			if(t1.Day != t2.Day)	return t1.Day-t2.Day;
			if(t1.Hour != t2.Hour)	return t1.Hour-t2.Hour;
			if(t1.Minute != t2.Minute)	return t1.Minute-t2.Minute;
			if(t1.Second != t2.Second)	return t1.Second-t2.Second;

			return 0;
		}

		void SortOnOffListBlock(ArrayList block, int nSort)
		{
			if(nSort == 0)	return;
			if(block.Count == 0)	return;

			int l, m;
			ON_OFF_LIST_SORT item1;
			ON_OFF_LIST_SORT item2;
			ON_OFF_LIST_SORT temp;
			int big_pos;

			if(nSort == 1) 
			{
				for(l = 0; l < block.Count-1; l++) 
				{
					item1 = (ON_OFF_LIST_SORT)block[l];
					big_pos = l;
					for(m = l+1; m < block.Count; m++) 
					{
						item2 = (ON_OFF_LIST_SORT)block[m];
						if(CompareDateTimeSec(item1.t, item2.t) > 0) 
						{
							big_pos = m;
							item1 = item2;
						}
					}
					if(big_pos != l) 
					{
						temp = (ON_OFF_LIST_SORT)block[big_pos];
						block[big_pos] = block[l];
						block[l] = temp;
					}
				}
			}
			else if(nSort == 2) 
			{
				for(l = 0; l < block.Count-1; l++) 
				{
					item1 = (ON_OFF_LIST_SORT)block[l];
					big_pos = l;
					for(m = l+1; m < block.Count; m++) 
					{
						item2 = (ON_OFF_LIST_SORT)block[m];
						if(CompareDateTimeSec(item1.t, item2.t) < 0) 
						{
							big_pos = m;
							item1 = item2;
						}
					}
					if(big_pos != l) 
					{
						temp = (ON_OFF_LIST_SORT)block[big_pos];
						block[big_pos] = block[l];
						block[l] = temp;
					}
				}
			}
			else {}
		}

		void LoadOnOffList(ArrayList block, string obj_tag, int obj_field_time, int obj_field_view, int obj_nSort, USER_SELECT_TIME time_from, USER_SELECT_TIME time_to, int time_type)
		{
			string path;
			TextReader reader;
			string buf;
			string imsi = "";
			CommaBlockString comma = new CommaBlockString();
            DateTime t = DateTimeServer.Now;
			int year, mon;
			int limit_mon;

			block.Clear();

			string org_tag = "";
			GetOriginalTag(out org_tag, obj_tag);

			for(year = time_from.year; year <= time_to.year; year++) 
			{
				if(year == time_from.year)	mon = time_from.mon;
				else						mon = 1;

				if(year == time_to.year)	limit_mon = time_to.mon;
				else						limit_mon = 12;

				for(; mon <= limit_mon; mon++) 
				{
					reader = null;

					path = String.Format("{0}\\database\\{1:0000}{2:00}.datx", TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject), year, mon);
					if(File.Exists(path)) 
					{
						reader = new StreamReader(path);
					}
					else {
						path = String.Format("{0}\\database\\{1:0000}{2:00}.dat", TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject), year, mon);
						if(File.Exists(path)) 
						{
							reader = new StreamReader(path, System.Text.Encoding.Default);	
						}
					}

					if(reader == null)	continue;

					while(true) 
					{
						buf = reader.ReadLine();
						if(buf == null)	break;
						comma.Set(buf);
						comma.GetString(ref imsi);	// tag
						imsi = imsi.Trim();

						if(!IsTagInclude(org_tag, imsi))			continue;

						if(obj_field_time == 0) 
						{	// use start time
							comma.GetString(ref imsi);
							StringToTime(ref t, imsi);
							comma.GetString(ref imsi);
							StringToDate(ref t, imsi);
						}
						else 
						{		// use end time
							comma.GetString(ref imsi);
							comma.GetString(ref imsi);
							comma.GetString(ref imsi);
							StringToTime(ref t, imsi);
							comma.GetString(ref imsi);
							StringToDate(ref t, imsi);
						}

						if(time_type == 0 && IsMinInclude(time_from, time_to, t)) 
						{
							AddOnOffListBlock(block, obj_field_view, buf, t);
						}
						else if(time_type == 1 && IsHourInclude(time_from, time_to, t)) 
						{
							AddOnOffListBlock(block, obj_field_view, buf, t);
						}
						else if(time_type == 2 && IsDayInclude(time_from, time_to, t)) 
						{
							AddOnOffListBlock(block, obj_field_view, buf, t);
						}
						else if(time_type == 3 && IsMonInclude(time_from, time_to, t)) 
						{
							AddOnOffListBlock(block, obj_field_view, buf, t);
						}
					}
					reader.Close();
				}
			}

			SortOnOffListBlock(block, obj_nSort);
		}

		int ConvertDiOnOffList(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time_to = new USER_SELECT_TIME(), time_from = new USER_SELECT_TIME();
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			OBJECT_DI_ONOFF_LIST obj = new OBJECT_DI_ONOFF_LIST();
			CELL_STRUCT cell_t;
			ArrayList block = new ArrayList();
			ON_OFF_LIST_SORT item;

			ReportLib.ObjectStringToStruct(ref obj, cell_s.text);

			GetSelectedReportTime(user_time, hand_auto);

			if(obj.bUseFromToTime == 1) 
			{	// 기간 자료 시간을 사용 한다.
				ReportConfig.GetMinListTimeFr(time_from);
				ReportConfig.GetMinListTimeTo(time_to);		

				LoadOnOffList(block, obj.tag, obj.field_time, obj.field_view, obj.nSort, time_from, time_to, 0);
			}
			else 
			{
				if(String.Compare(obj.time.zone, "Min") == 0) 
				{
					time_from = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					time_from.min = obj.time.from;
					FitMin(time_from, obj.time.shift_from);
					time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					time_to.min = obj.time.to;
					FitMin(time_to, obj.time.shift_to);

					LoadOnOffList(block, obj.tag, obj.field_time, obj.field_view, obj.nSort, time_from, time_to, 0);
				}
				else if(String.Compare(obj.time.zone, "Hour") == 0) 
				{
					time_from = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					time_from.hour = obj.time.from;
					FitHour(time_from, obj.time.shift_from);
					time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					time_to.hour = obj.time.to;
					FitHour(time_to, obj.time.shift_to);

					LoadOnOffList(block, obj.tag, obj.field_time, obj.field_view, obj.nSort, time_from, time_to, 1);
				}
				else if(String.Compare(obj.time.zone, "Day") == 0) 
				{
					time_from = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					time_from.day = obj.time.from;
					FitDay(time_from, obj.time.shift_from);
					time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					time_to.day = obj.time.to;
					FitDay(time_to, obj.time.shift_to);

					LoadOnOffList(block, obj.tag, obj.field_time, obj.field_view, obj.nSort, time_from, time_to, 2);
				}
				else if(String.Compare(obj.time.zone, "Mon") == 0) 
				{
					time_from = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					time_from.mon = obj.time.from;
					FitMon(time_from, obj.time.shift_from);
					time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					time_to.mon = obj.time.to;
					FitMon(time_to, obj.time.shift_to);

					LoadOnOffList(block, obj.tag, obj.field_time, obj.field_view, obj.nSort, time_from, time_to, 3);
				}
			}

			if(block.Count == 0) 
			{
				cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);
				cell_t.text = String.Format(ReportConfig.sNoData);
				return 1;
			}
			else 
			{
				for(int l = 0; l < block.Count; l++) 
				{
					cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+l);
					item = (ON_OFF_LIST_SORT)block[l];

					if(obj.field_view == 5 && cell_t.format.cType != 0)		// 가동시간 필드이면서 표시형식이 자동이 아닐때
					{
						double sum = GetValueByTimeCount(item.imsi, item.imsi.Length);
						MakeCellString(cell_t, sum, true);	// 1 = retn OK;
					}
					else 
					{
						cell_t.text = item.imsi;
					}
				}
				return block.Count;
			}
		}

		double CalcOnOffListSum(ArrayList block)
		{
			double sum=0;
			int l;
			ON_OFF_LIST_SORT item;

			for(l = 0; l < block.Count; l++) 
			{
				item = (ON_OFF_LIST_SORT)block[l];
	
				sum += GetValueByTimeCount(item.imsi, item.imsi.Length);
			}

			return sum;
		}

		int ConvertDiOnOffListSum(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time_to = new USER_SELECT_TIME(), time_from = new USER_SELECT_TIME();
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			OBJECT_DI_ONOFF_LIST_SUM obj = new OBJECT_DI_ONOFF_LIST_SUM();
			CELL_STRUCT cell_t;
			ArrayList block = new ArrayList();

			ReportLib.ObjectStringToStruct(ref obj, cell_s.text);

			GetSelectedReportTime(user_time, hand_auto);

			if(obj.bUseFromToTime == 1) 
			{	// 기간 자료 시간을 사용 한다.
				ReportConfig.GetMinListTimeFr(time_from);
				ReportConfig.GetMinListTimeTo(time_to);		

				LoadOnOffList(block, obj.tag, obj.field_time, 5, -1, time_from, time_to, 0);
			}
			else 
			{
				if(String.Compare(obj.time.zone, "Min") == 0) 
				{
					time_from = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					time_from.min = obj.time.from;
					FitMin(time_from, obj.time.shift_from);
					time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					time_to.min = obj.time.to;
					FitMin(time_to, obj.time.shift_to);

					LoadOnOffList(block, obj.tag, obj.field_time, 5, -1, time_from, time_to, 0);
				}
				else if(String.Compare(obj.time.zone, "Hour") == 0) 
				{
					time_from = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					time_from.hour = obj.time.from;
					FitHour(time_from, obj.time.shift_from);
					time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					time_to.hour = obj.time.to;
					FitHour(time_to, obj.time.shift_to);

					LoadOnOffList(block, obj.tag, obj.field_time, 5, -1, time_from, time_to, 1);
				}
				else if(String.Compare(obj.time.zone, "Day") == 0) 
				{
					time_from = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					time_from.day = obj.time.from;
					FitDay(time_from, obj.time.shift_from);
					time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					time_to.day = obj.time.to;
					FitDay(time_to, obj.time.shift_to);

					LoadOnOffList(block, obj.tag, obj.field_time, 5, -1, time_from, time_to, 2);
				}
				else if(String.Compare(obj.time.zone, "Mon") == 0) 
				{
					time_from = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					time_from.mon = obj.time.from;
					FitMon(time_from, obj.time.shift_from);
					time_to = (USER_SELECT_TIME)Tools.CopyObject(user_time);
					time_to.mon = obj.time.to;
					FitMon(time_to, obj.time.shift_to);

					LoadOnOffList(block, obj.tag, obj.field_time, 5, -1, time_from, time_to, 3);
				}
			}

			if(block.Count == 0) 
			{
				cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);
				cell_t.text = String.Format(ReportConfig.sNoData);
				return 1;
			}
			else 
			{
				double sum=0;
				cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);

				sum = CalcOnOffListSum(block);


				MakeCellString(cell_t, sum, true);	// 1 = retn OK;

				return 1;
			}
		}

		int ConvertDiMultiOnOffListSum(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y, EnumHandAuto hand_auto)
		{
			USER_SELECT_TIME time;
			USER_SELECT_TIME user_time = new USER_SELECT_TIME();
			OBJECT_DI_ONOFF_LIST_SUM obj = new OBJECT_DI_ONOFF_LIST_SUM();
			CELL_STRUCT cell_t;
			ArrayList block = new ArrayList();
			int cell_y;
			int shift;
			int i;
			double sum=0;

			ReportLib.ObjectStringToStruct(ref obj, cell_s.text);

			GetSelectedReportTime(user_time, hand_auto);

			if(String.Compare(obj.time.zone, "Min") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitMin(time, shift);
						time.min = i;
						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);

						LoadOnOffList(block, obj.tag, obj.field_time, 5, -1, time, time, 0);

						if(block.Count == 0)
							cell_t.text = ReportConfig.sNoData;
						else 
						{
							sum = CalcOnOffListSum(block);
							MakeCellString(cell_t, sum, true);	// 1 = retn OK;
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
			else if(String.Compare(obj.time.zone, "Hour") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitHour(time, shift);
						time.hour = i;
						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);

						LoadOnOffList(block, obj.tag, obj.field_time, 5, -1, time, time, 1);

						if(block.Count == 0)
							cell_t.text = ReportConfig.sNoData;
						else 
						{
							sum = CalcOnOffListSum(block);
							MakeCellString(cell_t, sum, true);	// 1 = retn OK;
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
			else if(String.Compare(obj.time.zone, "Day") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitDay(time, shift);
						time.day = i;
						if(TimeUtil.IsDayExist(time.year, time.mon, time.day)) 
						{
							cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
					
							LoadOnOffList(block, obj.tag, obj.field_time, 5, -1, time, time, 2);

							if(block.Count == 0)
								cell_t.text = ReportConfig.sNoData;
							else 
							{
								sum = CalcOnOffListSum(block);	
								MakeCellString(cell_t, sum, true);	// 1 = retn OK;
							}

							cell_y++;
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
			else if(String.Compare(obj.time.zone, "Mon") == 0) 
			{
				cell_y = 0;
				for(shift = obj.time.shift_from; shift <= obj.time.shift_to; shift++) 
				{
					for(i = GetFrom(obj.time, shift); i <= GetTo(obj.time, shift); i++, cell_y++) 
					{
						time = (USER_SELECT_TIME)Tools.CopyObject(user_time);
						FitMon(time, shift);
						time.mon = i;
						cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);

						LoadOnOffList(block, obj.tag, obj.field_time, 5, -1, time, time, 3);

						if(block.Count == 0)
							cell_t.text = ReportConfig.sNoData;
						else 
						{
							sum = CalcOnOffListSum(block);
							MakeCellString(cell_t, sum, true);	// 1 = retn OK;
						}
					}
				}
				return cell_y;	// 증가한 Y의 크기를 알려준다.
			}
			else 
			{
				cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);
				cell_t.text = Message알수없는시간형식(obj.time.zone);
				return 1;
			}
		}

		int GetStringOrgOrVar(ref string tar, string org)
		{
			if(org.Length > 0 && org[0] == '$') 
			{
                if (StringVar.GetStringVar(org.Substring(1), out tar)) return 1;
				tar = "";
				return 0;
			}
			else 
			{
				tar = org;
			}

			return 1;
		}

		int ConvertEtcDatabase(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y, EnumHandAuto hand_auto)
		{
			OBJECT_ETC_DATABASE obj = new OBJECT_ETC_DATABASE();
			CELL_STRUCT cell_t;

			ReportLib.ObjectStringToStruct(ref obj, cell_s.text);

			string sFileName = "";
			string sTable = "";
			string sField = "";
			string sWhere = "";
			string sOrderBy = "";

			GetStringOrgOrVar(ref sFileName, obj.filename);
			GetStringOrgOrVar(ref sTable,    obj.table);
			GetStringOrgOrVar(ref sField,    obj.field);
			GetStringOrgOrVar(ref sWhere,    obj.where);
			GetStringOrgOrVar(ref sOrderBy,  obj.orderby);

			cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);

			bool bMdbOrString = false;

			if(String.Compare(sFileName, 0, "Provider=", 0, 9, true) == 0) 
			{
				bMdbOrString = true;
			}
			else 
			{	// MDB file
				bMdbOrString = false;

				if(!File.Exists(sFileName)) 
				{
					cell_t.text = String.Format("file not found.");
					return 1;	// one line	return
				}
			}

			string connection_string;

			if(bMdbOrString == false) 
			{
				connection_string = String.Format("Provider=Microsoft.JET.OLEDB.4.0;Data Source={0};", sFileName);
			}
			else
				connection_string  = sFileName;

			OleDbConnection conn;

            try
            {
                conn = new OleDbConnection(connection_string);
            }
            catch (Exception exception)
            {
                cell_t.text = "Fail";
                ErrorMessage("ConnectionString : {0}\nError={1}", connection_string, exception.Message);
                return 1;
            }

			conn.Open();
			if(conn.State != ConnectionState.Open) {
				cell_t.text = String.Format("Open failed.");
				return 1;	// one line return
			}

			if(sTable.Length == 0) 
			{
				cell_t.text = String.Format("Table name is null.");
				return 1;
			}

			DataSet ds = new DataSet();

			string sort;

            // 테이블에 SELECT문이 포함되어 있으면 그것을 쿼리로 사용한다. 2012-1-11
            if (String.Compare(sTable, 0, "SELECT ", 0, 7, true) == 0)
            {
                sort = sTable;
            }
            else
            {
                sort = String.Format("SELECT * FROM {0}", sTable);

                if (sWhere.Length > 0)
                {
                    sort += " WHERE ";
                    sort += sWhere;
                }
                if (sOrderBy.Length > 0)
                {
                    sort += " ORDER BY ";
                    sort += sOrderBy;
                }
            }

			OleDbDataAdapter adapter = new OleDbDataAdapter(sort, conn);

			try 
			{
				adapter.Fill(ds);
			}
			catch (Exception exception)
			{
				ErrorMessage("Query={0}\nError={1}", sort, exception.Message);
				conn.Close();
				
				return 1;
			}

			conn.Close();

			if(ds.Tables[0].Rows.Count == 0) 
			{
				cell_t.text = String.Format(ReportConfig.sNoData);
				return 1;
			}

			// 레코드가 있을 때는 미리 레코드 개수만큼 표를 늘려준다.
			cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+ds.Tables[0].Rows.Count-1);

			DataRow row;
			string var;

			int index_column = ds.Tables[0].Columns.IndexOf(sField);
	
			for(int l = 0; l < ds.Tables[0].Rows.Count; l++) 
			{
				cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+l);
				row = ds.Tables[0].Rows[l];

				if(index_column == -1)
					var = "";
				else 
					var = row[index_column].ToString();

                cell_t.text = MakeStringByFormat(cell_t.format, var);   // 데이터베이스인 경우도 숫자 형식인 경우 형식에 맞추어 준다. 2011-4-20
                /*
				if(var == null) 
				{
					cell_t.text = "<NULL>";
				}
				else 
				{
					cell_t.text = var;
				}*/
			}

			int count = ds.Tables[0].Rows.Count;
	
			return count;
		}

        

		int ConvertEtcStringVar(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y)
		{
			OBJECT_ETC_STRING_VAR obj = new OBJECT_ETC_STRING_VAR();
			CELL_STRUCT cell_t;

			ReportLib.ObjectStringToStruct(ref obj, cell_s.text);

			string tar = "";

            if (!StringVar.GetStringVar(obj.var, out tar)) 
			{
				tar = String.Format("Invalid var name({0})", obj.var);
			}

			cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y);

			cell_t.text = tar;
			return 1;	// one line	return
		}

		//Block blockAlarmSave(sizeof(ALARM_FILE_STRUCT));
		// 아래 지정된 변수는 경보 로드 시 같은중에는 일반적으로 같은 속성이 있으므로 두번 부르지 않게 한다.
		static USER_SELECT_TIME saveAlarmTimeFrom = new USER_SELECT_TIME();
		static USER_SELECT_TIME saveAlarmTimeTo = new USER_SELECT_TIME();
		static int saveAlarmTimeType;
		static string saveAlarmTagName;
		static ArrayList blockAlarmList = new ArrayList();

		string ALARM_FILE_EXT = "AL3";

		void LoadAlarmList(OBJECT_ALARM obj, USER_SELECT_TIME time_from, USER_SELECT_TIME time_to, int time_type)
		{
			string org_tag = "";
			GetOriginalTag(out org_tag, obj.tag);

			if((saveAlarmTimeFrom == time_from) && (saveAlarmTimeTo == time_to) && saveAlarmTimeType == time_type &&
				String.Compare(saveAlarmTagName, org_tag) == 0) 
			{	// 같은 항목이다.
				return;
			}

			saveAlarmTimeFrom = (USER_SELECT_TIME)Tools.CopyObject(time_from);
			saveAlarmTimeTo = (USER_SELECT_TIME)Tools.CopyObject(time_to);
			saveAlarmTimeType = time_type;
			saveAlarmTagName = org_tag;

			blockAlarmList.Clear();
		
			string path;
			
			datetime d = new datetime();
			ALARM_FILE_STRUCT alarm;

			for(d.da_year = time_from.year; d.da_year <= time_to.year; d.da_year++) 
			{
				for(d.da_mon = 1; d.da_mon <= 12; d.da_mon++) 
				{
					for(d.da_day = 1; d.da_day <= 31; d.da_day++) 
					{
						if(!TimeUtil.IsDayExist(d.da_year, d.da_mon, d.da_day))	continue;
						if(!IsDayInclude(time_from, time_to, d))				continue;

						path = String.Format("{0}\\alarm\\{1:0000}{2:00}{3:00}.almx", TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject), d.da_year, d.da_mon, d.da_day);

						if(File.Exists(path)) 
						{
							TextReader reader = new StreamReader(path);
							CommaBlockString comma = new CommaBlockString();
							string one_line;

							while(true) 
							{
								alarm  = new ALARM_FILE_STRUCT();

								one_line = reader.ReadLine();
								if(one_line == null)	break;

								comma.Set(one_line); 

								alarm.t.Set(comma.GetDateTime());
								comma.GetString(ref alarm.tag);	// tag
								comma.GetString(ref alarm.description);	// description
								comma.GetString(ref alarm.msg);	// msg
								comma.GetWORD(ref alarm.alarm_type);	// alarm_type
								comma.GetWORD(ref alarm.priority);	// priority
								comma.GetWORD(ref alarm.port);	// port
								comma.GetWORD(ref alarm.station);	// station
								comma.GetDWORD(ref alarm.address);	// address
								comma.GetWORD(ref alarm.alarm_sub_type);	// type

								alarm.tag = alarm.tag.Trim();

								if(!IsTagInclude(org_tag, alarm.tag))			continue;

								if(time_type == 0 && IsMinInclude(time_from, time_to, alarm.t))
								{
									blockAlarmList.Add(alarm);
								}
								else if(time_type == 1 && IsHourInclude(time_from, time_to, alarm.t))
								{
									blockAlarmList.Add(alarm);
								}
								else
								{	// 그 이외의 경우(일,월)에는 for loop 에서 검사했다.
									blockAlarmList.Add(alarm);
								}
							}

							reader.Close();
						}
						else 
						{
							FileStream stream;
							BinaryReader reader;
							
							path = String.Format("{0}\\alarm\\{1:0000}{2:00}{3:00}.{4}", TotalConfig.GetProjectDataDirectory(TotalConfig.sDirWorkProject), d.da_year, d.da_mon, d.da_day, ALARM_FILE_EXT);

							if(!File.Exists(path))	continue;
							stream = File.OpenRead(path);
							if(stream == null)	continue;
							reader = new BinaryReader(stream);
							while(true) 
							{
								alarm  = new ALARM_FILE_STRUCT();
								if(!alarm.LoadFromFile(reader))	break;

								alarm.tag = alarm.tag.Trim();

								if(!IsTagInclude(org_tag, alarm.tag))			continue;

								if(time_type == 0 && IsMinInclude(time_from, time_to, alarm.t))
								{
									blockAlarmList.Add(alarm);
								}
								else if(time_type == 1 && IsHourInclude(time_from, time_to, alarm.t))
								{
									blockAlarmList.Add(alarm);
								}
								else
								{	// 그 이외의 경우(일,월)에는 for loop 에서 검사했다.
									blockAlarmList.Add(alarm);
								}
							}
							reader.Close();
							stream.Close();
						}
					}
				}
			}
		}

		int ConvertEtcMinListByMinute(OBJECT_ETC_MIN_LIST obj, TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y)
		{
			CELL_STRUCT cell_t;
			int   cell_y;
			USER_SELECT_TIME time_fr = new USER_SELECT_TIME(), time_to = new USER_SELECT_TIME();
			int   min_gab;

			min_gab = GetVarValue(obj.min_gab, 1, 60);

			ReportConfig.GetMinListTimeFr(time_fr);
			ReportConfig.GetMinListTimeTo(time_to);
	
			// 분 자료를 읽어온다.
			cell_y = 0;
			while(true) 
			{
				if(TimeUtil.IsDayExist(time_fr.year, time_fr.mon, time_fr.day)) 
				{
					cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
					cell_y++;

					MakeTimeString(cell_t, time_fr);

					if(obj.bUseTo == 1)
						cell_t.text += String.Format("~{0:00}", (time_fr.min+min_gab-1)%60);
				}
				time_fr.min += min_gab;
				if(time_fr.min >= 60) 
				{
					TimeUtil.PlusHour(ref time_fr.year, ref time_fr.mon, ref time_fr.day, ref time_fr.hour);
					time_fr.min -= 60;
				}
				if(CompareTimeMin(time_fr, time_to) > 0)	break;
			}

			if(cell_y == 0) 
			{
				cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
				cell_t.text = ReportConfig.sNoData;
				return 1;
			}

			return cell_y;
		}

		int ConvertEtcMinListByHour(OBJECT_ETC_MIN_LIST obj, TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y)
		{
			CELL_STRUCT cell_t;
			int   cell_y;
			USER_SELECT_TIME time_fr = new USER_SELECT_TIME(), time_to = new USER_SELECT_TIME();
			int   hour_gab;

			hour_gab = GetVarValue(obj.min_gab, 1, 60);

			ReportConfig.GetMinListTimeFr(time_fr);
			ReportConfig.GetMinListTimeTo(time_to);
	
			// 분 자료를 읽어온다.
			cell_y = 0;
			while(true) 
			{
				if(TimeUtil.IsDayExist(time_fr.year, time_fr.mon, time_fr.day)) 
				{
					cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
					cell_y++;

					MakeTimeString(cell_t, time_fr);
					if(obj.bUseTo == 1)
						cell_t.text += String.Format("~{0:00}:59", (time_fr.hour+hour_gab-1)%24);
				}
				time_fr.hour += hour_gab;
				if(time_fr.hour >= 24) 
				{
					TimeUtil.PlusDay(ref time_fr.year, ref time_fr.mon, ref time_fr.day);
					time_fr.hour -= 24;
				}
				if(CompareTimeHour(time_fr, time_to) > 0)	break;
			}

			if(cell_y == 0) 
			{
				cell_t = GetTargetCellPointer(table_t, table_s, cell_s, target_x, target_y+cell_y);
				cell_t.text = ReportConfig.sNoData;
				return 1;
			}

			return cell_y;
		}

		int ConvertEtcMinList(TABLE_STRUCT table_t, TABLE_STRUCT table_s, CELL_STRUCT cell_s, int target_x, int target_y)
		{
			OBJECT_ETC_MIN_LIST obj = new OBJECT_ETC_MIN_LIST();

			ReportLib.ObjectStringToStruct(ref obj, cell_s.text);

			if(obj.cDataUnit == 1) 
			{
				return ConvertEtcMinListByHour(obj, table_t, table_s, cell_s, target_x, target_y);
			}
			else 
			{
				return ConvertEtcMinListByMinute(obj, table_t, table_s, cell_s, target_x, target_y);
			}
		}

		CELL_STRUCT GetSourcePointer(TABLE_STRUCT table_s, CELL_STRUCT cell_t)
		{
			return (CELL_STRUCT)table_s.cellBuf[cell_t.x+table_s.cell_x*cell_t.wOnRunY1];
		}

		void GetTableXY(string zone, ref int table_no, ref int x, ref int y)
		{
			x = 0;
			y = 0;

			int hap;
			string imsi = "";
			int j;
			int pos;

			// table 번호를 찾는다.
			hap = 0;
			for(pos = 0; pos < zone.Length; pos++) 
			{
				if(zone[pos] >= '0' && zone[pos] <= '9') 
				{
					imsi += zone[pos];
					hap++;
				}
				else	break;
			}

			if(hap > 0) 
			{
				table_no = ConvertTool.ToInt32(imsi);
			}

			// 필드번호를 찾는다. (A~Z, or a~z)
			hap = 0;
			imsi = "";
			for(; pos < zone.Length; pos++) 
			{
				if(zone[pos] >= 'a' && zone[pos] <= 'z') 
				{
					imsi += zone[pos];
					hap++;
				}
				else if(zone[pos] >= 'A' && zone[pos] <= 'Z') 
				{
					imsi += zone[pos];
					hap++;
				}
				else	break;
			}

			if(hap > 0) 
			{
				x = 0;
				for(j = 0; j < hap; j++) 
				{
					if(j > 0) 
					{
						x = (x+1)*26;
					}

					if(imsi[j] >= 'a' && imsi[j] <= 'z') 
					{
						x += imsi[j]-'a';
					}
					else if(imsi[j] >= 'A' && imsi[j] <= 'Z') 
					{
						x += imsi[j]-'A';
					}
				}
			}

			// 레코드 번호를 찾는다.
			y = ConvertTool.ToInt32(zone.Substring(pos));
		}

		void GetTargetZoneOnTarget(REPORT_STRUCT rt, int table_no, ref int x1, ref int y1, ref int x2, ref int y2, string zone)
		{
			x1 = 0;
			x2 = 0;
			y1 = 0;
			y2 = 0;

			CommaBlockString comma = new CommaBlockString();
			string buf = "";
			int  table_from = 0, table_to = 0;
			//int  y;

			comma.SetBlockCode(':');
			comma.Set(zone);

			comma.GetString(ref buf);
			GetTableXY(buf, ref table_from, ref x1, ref y1);
			comma.GetString(ref buf);

			if(buf.Length == 0) 
			{
				table_to = table_from;
				x2 = x1;
			}
			else 
			{
				GetTableXY(buf, ref table_to, ref x2, ref y2);
			}

			TABLE_STRUCT table_t = (TABLE_STRUCT)rt.tableBuf[table_no];

			if(x1 > x2)	Tools.Temp(ref x1, ref x2);

			if(x1 < 0)	x1 = 0;
			if(x2 < 0)	x2 = 0;

			if(x1 >= table_t.cell_x)	x1 = table_t.cell_x-1;
			if(x2 >= table_t.cell_x)	x2 = table_t.cell_x-1;

			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

		}

		void GetTargetZoneOnTargetByLineSub(REPORT_STRUCT rt, int table_no, ref int x1, ref int y1, ref int x2, ref int y2, string zone)
		{
			x1 = 0;
			x2 = 0;
			y1 = 0;
			y2 = 0;

			CommaBlockString comma = new CommaBlockString();
			string buf = "";
			int  table_from = 0, table_to = 0;
			//int  y;

			comma.SetBlockCode(':');
			comma.Set(zone);

			comma.GetString(ref buf);
			GetTableXY(buf, ref table_from, ref x1, ref y1);
			comma.GetString(ref buf);

			if(buf.Length == 0) 
			{
				table_to = table_from;
				x2 = x1;
			}
			else 
			{
				GetTableXY(buf, ref table_to, ref x2, ref y2);
			}

			TABLE_STRUCT table_t = (TABLE_STRUCT)rt.tableBuf[table_no];


			if(x1 < 0)	x1 = 0;
			if(x2 < 0)	x2 = 0;

			if(x1 >= table_t.cell_x)	x1 = table_t.cell_x-1;
			if(x2 >= table_t.cell_x)	x2 = table_t.cell_x-1;

		}

		void GetTargetZone(REPORT_STRUCT rt, REPORT_STRUCT rs, ref int table, ref int x1, ref int y1, ref int x2, ref int y2, string zone)
		{
			x1 = 0;
			x2 = 0;
			y1 = 0;
			y2 = 0;

			CommaBlockString comma = new CommaBlockString();
			string buf = "";
			int  table_from = table, table_to = table;

			comma.SetBlockCode(':');
			comma.Set(zone);

			comma.GetString(ref buf);
			GetTableXY(buf, ref table_from, ref x1, ref y1);
			comma.GetString(ref buf);

			if(buf.Length == 0) 
			{
				table_to = table_from;
				x2 = x1;
				y2 = y1;
			}
			else 
			{
				GetTableXY(buf, ref table_to, ref x2, ref y2);
			}

			if(table_from == table_to) 
			{
				table = table_from;
			}
			else 
			{
				//table = table_s->no;
			}

			if(table < 0 || table >= rs.TableCount)	table = 0;	// table over

			TABLE_STRUCT table_s = (TABLE_STRUCT)rs.tableBuf[table];

			if(x1 > x2)	Tools.Temp(ref x1, ref x2);
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);

			if(x1 < 0)	x1 = 0;
			if(y1 < 0)	y1 = 0;
			if(x2 < 0)	x2 = 0;
			if(y2 < 0)	y2 = 0;
			if(x1 >= table_s.cell_x)	x1 = table_s.cell_x-1;
			if(y1 >= table_s.cell_y)	y1 = table_s.cell_y-1;
			if(x2 >= table_s.cell_x)	x2 = table_s.cell_x-1;
			if(y2 >= table_s.cell_y)	y2 = table_s.cell_y-1;

			//if(x2 < 0)	x2 = 0;
			//if(x1 >= table_s.cell_x)	x1 = table_s.cell_x-1;

			CELL_STRUCT cell;

			cell = (CELL_STRUCT)table_s.cellBuf[x1+y1*table_s.cell_x];
			y1 = cell.wOnRunY1;
			cell = (CELL_STRUCT)table_s.cellBuf[x2+y2*table_s.cell_x];
			y2 = cell.wOnRunY2;

			// 찾은 target위치가 범위를 넘어서면 잘라준다.
			TABLE_STRUCT table_t = (TABLE_STRUCT)rt.tableBuf[table];
			if(y1 < 0)	y1 = 0;
			if(y2 < 0)	y2 = 0;
			if(y1 >= table_t.cell_y)	y1 = table_t.cell_y-1;
			if(y2 >= table_t.cell_y)	y2 = table_t.cell_y-1;
			if(y1 > y2)	Tools.Temp(ref y1, ref y2);
		}

		//--------------------------------------------------------------------------------
		//	셀이 소요시간일 때는 초로 환산한다.
		//--------------------------------------------------------------------------------

		double GetValueByTimeCount(string s, int size)
		{
			double val = 0;

			int hour = 0;
			int min = 0;
			int sec = 0;
			int i;
			string imsi = "";
			int  pos = 0;
			bool remain_flag = false;

			for(i = 0; i < size; i++) 
			{
				if(s[i] >= '0' && s[i] <= '9') 
				{
					if(remain_flag) 
					{
						pos = 0;
						hour = min;
						min = sec;
						sec = 0;
						remain_flag = false;
						imsi = "";
					}

					imsi += s[i];
					pos++;
					try 
					{
						sec = ConvertTool.ToInt32(imsi);
					}
					catch 
					{
						sec = 0;
					}
				}
				else 
				{
					if(pos > 0) 
					{
						remain_flag = true;
					}
				}
			}

			val = hour*3600+min*60+sec;

			return val;
		}

		//--------------------------------------------------------------------------------
		//	숫자일 경우 콤마를 제거한다.
		//--------------------------------------------------------------------------------

		double GetValueByElseNumber(string s, int size)
		{
			int i;
			string imsi = "";
			int  pos = 0;

			for(i = 0; i < size; i++) 
			{
				if(s[i] != ',') 
				{
					imsi += s[i];
					pos++;
					
				}
			}

			return ConvertTool.ToDouble(imsi);
		}

		bool IsNumericNoDataExpression()
		{
			int size = ReportConfig.sNoData.Length;
			if(size == 0)	return true;
	
			for(int i = 0; i < size; i++) 
			{
				if(ReportConfig.sNoData[i] < '0' || ReportConfig.sNoData[i] > '9')	return false;
			}

			return true;
		}

		bool GetValueCalcedData(CELL_STRUCT cell_t, ref double val)
		{
			val = 0;

			if(!IsNumericNoDataExpression()) 
			{	// 숫자나 공백이 아닐때만 체크
				if(String.Compare(cell_t.text, ReportConfig.sNoData) == 0) 
				{
					if(ReportConfig.bCalcNoData) 
					{
						return true;	// 데이터 없음을 0으로 계산
					}

					return false;	// 데이터 없음
				}
			}

			if(cell_t.format.cType == 3) 
			{
				val = GetValueByTimeCount(cell_t.text, cell_t.text.Length);
				return true;
			}

			val = GetValueByElseNumber(cell_t.text, cell_t.text.Length);
	
			return true;
		}

		void ErrorMessage(string str, params object[] args)
		{
			MessageDisplay.Show(str, args);
		}

		//------------------------------------------------------------------------------
		//	( 가 시작하는 위치와 ) 가 닫히는 위치를 찾는다.
		//------------------------------------------------------------------------------
		bool SeekStartEnd(string s, int size, ref int start_pos, ref int end_pos)
		{
			int i;
			int open = 0;
			int close = 0;

			for(i = 0; i < size; i++) 
			{
				if(s[i] == '(') 
				{
					if(open == 0) 
					{
						start_pos = i;
					}
					open++;
				}
				else if(s[i] == ')') 
				{
					end_pos = i;
					close++;

					if(open < close) 
					{
						ErrorMessage("can't find start (");
						return false;	// 시작하는 ( 를 찾지 못했다.
					}
					else if(open == close) 
					{
						return true;
					}
				}
				else{}
			}
			ErrorMessage("can't find end )");
			return false;	// 끝나는 ')'를 찾지 못했다.
		}

		int Function_CheckSum(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string command, string argument, ref double val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf = "";

			arg.Set(argument);

			int x1=0, y1=0, x2=0, y2=0;
			int x, y;
			double one_value = 0;
			int  count = 0;
			int table_no;
			CELL_STRUCT cell_t;
			CELL_STRUCT cell_s;
			bool retn;
			TABLE_STRUCT table_t;
			TABLE_STRUCT table_s;

			val = 0;

			while(true) 
			{ 
				arg.GetArgument(ref buf);
				if(buf.Length == 0)	break;

				table_no = cell_table;
				GetTargetZone(rt, rs, ref table_no, ref x1, ref y1, ref x2, ref y2, buf);

				table_s = (TABLE_STRUCT)rs.tableBuf[table_no];
				table_t = (TABLE_STRUCT)rt.tableBuf[table_no];

				for(y = y1; y <= y2; y++) 
				{
					for(x = x1; x <= x2; x++) 
					{
						cell_t = (CELL_STRUCT)table_t.cellBuf[x+y*table_t.cell_x];
						cell_s = GetSourcePointer(table_s, cell_t);

						if(cell_t.bCalced == 1) 
						{
							retn = GetValueCalcedData(cell_t, ref one_value);
						}
						else 
						{
							cell_t.bCalced = 1;
							retn = GetValueRecurse(rt, rs, table_no, cell_t.text, cell_t.text.Length, ref one_value);
							MakeCellString(cell_t, one_value, retn);
						}

						if(retn) 
						{
							count++;
							val += one_value;
						}
					}
				}
			}

			if(count == 0)	return 0;

			return 1;
		}

		int Function_CheckMin(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string command, string argument, ref double val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf = "";

			arg.Set(argument);

			int x1=0, y1=0, x2=0, y2=0;
			int x, y;
			double one_value = 0;
			int  count = 0;
			int table_no;
			CELL_STRUCT cell_t;
			CELL_STRUCT cell_s;
			bool retn;
			TABLE_STRUCT table_t;
			TABLE_STRUCT table_s;

			val = 0;

			while(true) 
			{ 
				arg.GetArgument(ref buf);
				if(buf.Length == 0)	break;

				table_no = cell_table;
				GetTargetZone(rt, rs, ref table_no, ref x1, ref y1, ref x2, ref y2, buf);

				table_s = (TABLE_STRUCT)rs.tableBuf[table_no];
				table_t = (TABLE_STRUCT)rt.tableBuf[table_no];

				for(y = y1; y <= y2; y++) 
				{
					for(x = x1; x <= x2; x++) 
					{
						cell_t = (CELL_STRUCT)table_t.cellBuf[x+y*table_t.cell_x];
						cell_s = GetSourcePointer(table_s, cell_t);

						if(cell_t.bCalced == 1) 
						{
							retn = GetValueCalcedData(cell_t, ref one_value);
						}
						else 
						{
							cell_t.bCalced = 1;
							retn = GetValueRecurse(rt, rs, table_no, cell_t.text, cell_t.text.Length, ref one_value);
							MakeCellString(cell_t, one_value, retn);
						}

						if(retn) 
						{
							if(count == 0)	val = one_value;
							else 
							{
								if(one_value < val)	val = one_value;
							}

							count++;
						}
					}
				}
			}

			if(count == 0)	return 0;

			return 1;
		}

		int Function_CheckMax(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string command, string argument, ref double val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf = "";

			arg.Set(argument);

			int x1=0, y1=0, x2=0, y2=0;
			int x, y;
			double one_value = 0;
			int  count = 0;
			int table_no;
			CELL_STRUCT cell_t;
			CELL_STRUCT cell_s;
			bool retn;
			TABLE_STRUCT table_t;
			TABLE_STRUCT table_s;

			val = 0;

			while(true) 
			{ 
				arg.GetArgument(ref buf);
				if(buf.Length == 0)	break;

				table_no = cell_table;
				GetTargetZone(rt, rs, ref table_no, ref x1, ref y1, ref x2, ref y2, buf);

				table_s = (TABLE_STRUCT)rs.tableBuf[table_no];
				table_t = (TABLE_STRUCT)rt.tableBuf[table_no];

				for(y = y1; y <= y2; y++) 
				{
					for(x = x1; x <= x2; x++) 
					{
						cell_t = (CELL_STRUCT)table_t.cellBuf[x+y*table_t.cell_x];
						cell_s = GetSourcePointer(table_s, cell_t);

						if(cell_t.bCalced == 1) 
						{
							retn = GetValueCalcedData(cell_t, ref one_value);
						}
						else 
						{
							cell_t.bCalced = 1;
							retn = GetValueRecurse(rt, rs, table_no, cell_t.text, cell_t.text.Length, ref one_value);
							MakeCellString(cell_t, one_value, retn);
						}

						if(retn) 
						{
							if(count == 0)	val = one_value;
							else 
							{
								if(one_value > val)	val = one_value;
							}

							count++;
						}
					}
				}
			}

			if(count == 0)	return 0;

			return 1;
		}

		int Function_CheckAve(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string command, string argument, ref double val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf = "";

			arg.Set(argument);

			int x1=0, y1=0, x2=0, y2=0;
			int x, y;
			double one_value = 0;
			int  count = 0;
			int table_no;
			CELL_STRUCT cell_t;
			CELL_STRUCT cell_s;
			bool retn;
			TABLE_STRUCT table_t;
			TABLE_STRUCT table_s;

			val = 0;

			while(true) 
			{ 
				arg.GetArgument(ref buf);
				if(buf.Length == 0)	break;

				table_no = cell_table;
				GetTargetZone(rt, rs, ref table_no, ref x1, ref y1, ref x2, ref y2, buf);

				table_s = (TABLE_STRUCT)rs.tableBuf[table_no];
				table_t = (TABLE_STRUCT)rt.tableBuf[table_no];

				for(y = y1; y <= y2; y++) 
				{
					for(x = x1; x <= x2; x++) 
					{
						cell_t = (CELL_STRUCT)table_t.cellBuf[x+y*table_t.cell_x];
						cell_s = GetSourcePointer(table_s, cell_t);

						if(cell_t.bCalced == 1) 
						{
							retn = GetValueCalcedData(cell_t, ref one_value);
						}
						else 
						{
							cell_t.bCalced = 1;
							retn = GetValueRecurse(rt, rs, table_no, cell_t.text, cell_t.text.Length, ref one_value);
							MakeCellString(cell_t, one_value, retn);
						}

						if(retn) 
						{
							count++;
							val += one_value;
						}
					}
				}
			}

			if(count == 0)	return 0;

			val /= count;

			return 1;
		}

		int Function_CheckAbsMin(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string command, string argument, ref double val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf = "";

			arg.Set(argument);

			int x1=0, y1=0, x2=0, y2=0;
			int x, y;
			double one_value = 0;
			int  count = 0;
			int table_no;
			CELL_STRUCT cell_t;
			CELL_STRUCT cell_s;
			bool retn;
			TABLE_STRUCT table_t;
			TABLE_STRUCT table_s;

			val = 0;

			while(true) 
			{ 
				arg.GetArgument(ref buf);
				if(buf.Length == 0)	break;

				table_no = cell_table;
				GetTargetZone(rt, rs, ref table_no, ref x1, ref y1, ref x2, ref y2, buf);

				table_s = (TABLE_STRUCT)rs.tableBuf[table_no];
				table_t = (TABLE_STRUCT)rt.tableBuf[table_no];

				for(y = y1; y <= y2; y++) 
				{
					for(x = x1; x <= x2; x++) 
					{
						cell_t = (CELL_STRUCT)table_t.cellBuf[x+y*table_t.cell_x];
						cell_s = GetSourcePointer(table_s, cell_t);

						if(cell_t.bCalced == 1) 
						{
							retn = GetValueCalcedData(cell_t, ref one_value);
						}
						else 
						{
							cell_t.bCalced = 1;
							retn = GetValueRecurse(rt, rs, table_no, cell_t.text, cell_t.text.Length, ref one_value);
							MakeCellString(cell_t, one_value, retn);
						}

						if(retn) 
						{
							if(count == 0)	val = one_value;
							else 
							{
								if(Math.Abs(one_value) < Math.Abs(val))	val = one_value;
							}

							count++;
						}
					}
				}
			}

			if(count == 0)	return 0;

			return 1;
		}

		int Function_CheckAbsMax(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string command, string argument, ref double val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf = "";

			arg.Set(argument);

			int x1=0, y1=0, x2=0, y2=0;
			int x, y;
			double one_value = 0;
			int  count = 0;
			int table_no;
			CELL_STRUCT cell_t;
			CELL_STRUCT cell_s;
			bool retn;
			TABLE_STRUCT table_t;
			TABLE_STRUCT table_s;

			val = 0;

			while(true) 
			{ 
				arg.GetArgument(ref buf);
				if(buf.Length == 0)	break;

				table_no = cell_table;
				GetTargetZone(rt, rs, ref table_no, ref x1, ref y1, ref x2, ref y2, buf);

				table_s = (TABLE_STRUCT)rs.tableBuf[table_no];
				table_t = (TABLE_STRUCT)rt.tableBuf[table_no];

				for(y = y1; y <= y2; y++) 
				{
					for(x = x1; x <= x2; x++) 
					{
						cell_t = (CELL_STRUCT)table_t.cellBuf[x+y*table_t.cell_x];
						cell_s = GetSourcePointer(table_s, cell_t);

						if(cell_t.bCalced == 1) 
						{
							retn = GetValueCalcedData(cell_t, ref one_value);
						}
						else 
						{
							cell_t.bCalced = 1;
							retn = GetValueRecurse(rt, rs, table_no, cell_t.text, cell_t.text.Length, ref one_value);
							MakeCellString(cell_t, one_value, retn);
						}

						if(retn) 
						{
							if(count == 0)	val = one_value;
							else 
							{
								if(Math.Abs(one_value) > Math.Abs(val))	val = one_value;
							}

							count++;
						}
					}
				}
			}

			if(count == 0)	return 0;

			return 1;
		}

		int Function_CheckAbsAve(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string command, string argument, ref double val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf = "";

			arg.Set(argument);

			int x1=0, y1=0, x2=0, y2=0;
			int x, y;
			double one_value = 0;
			int  count = 0;
			int table_no;
			CELL_STRUCT cell_t;
			CELL_STRUCT cell_s;
			bool retn;
			TABLE_STRUCT table_t;
			TABLE_STRUCT table_s;
			int plus_count = 0;
			int minus_count = 0;

			val = 0;

			while(true) 
			{ 
				arg.GetArgument(ref buf);
				if(buf.Length == 0)	break;

				table_no = cell_table;
				GetTargetZone(rt, rs, ref table_no, ref x1, ref y1, ref x2, ref y2, buf);

				table_s = (TABLE_STRUCT)rs.tableBuf[table_no];
				table_t = (TABLE_STRUCT)rt.tableBuf[table_no];

				for(y = y1; y <= y2; y++) 
				{
					for(x = x1; x <= x2; x++) 
					{
						cell_t = (CELL_STRUCT)table_t.cellBuf[x+y*table_t.cell_x];
						cell_s = GetSourcePointer(table_s, cell_t);

						if(cell_t.bCalced == 1) 
						{
							retn = GetValueCalcedData(cell_t, ref one_value);
						}
						else 
						{
							cell_t.bCalced = 1;
							retn = GetValueRecurse(rt, rs, table_no, cell_t.text, cell_t.text.Length, ref one_value);
							MakeCellString(cell_t, one_value, retn);
						}

						if(retn) 
						{
							count++;
							val += Math.Abs(one_value);
							if(one_value < 0)	minus_count++;
							else				plus_count++;
						}
					}
				}
			}

			if(count == 0)	return 0;

			val /= count;
			if(minus_count > plus_count)	val *= -1;

			return 1;
		}

		int Function_CheckAbs(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string command, string argument, ref double val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf = "";

			arg.Set(argument);

			int x1=0, y1=0, x2=0, y2=0;
			double one_value = 0;
			int table_no;
			CELL_STRUCT cell_t;
			CELL_STRUCT cell_s;
			bool retn;
			TABLE_STRUCT table_t;
			TABLE_STRUCT table_s;

			val = 0;

			arg.GetArgument(ref buf);

			table_no = cell_table;
			GetTargetZone(rt, rs, ref table_no, ref x1, ref y1, ref x2, ref y2, buf);

			table_s = (TABLE_STRUCT)rs.tableBuf[table_no];
			table_t = (TABLE_STRUCT)rt.tableBuf[table_no];

			cell_t = (CELL_STRUCT)table_t.cellBuf[x1+y1*table_t.cell_x];
			cell_s = GetSourcePointer(table_s, cell_t);

			if(cell_t.bCalced == 1) 
			{
				retn = GetValueCalcedData(cell_t, ref one_value);
			}
			else 
			{
				cell_t.bCalced = 1;
				retn = GetValueRecurse(rt, rs, table_no, cell_t.text, cell_t.text.Length, ref one_value);
				MakeCellString(cell_t, one_value, retn);
			}

			if(retn) 
			{
				val = Math.Abs(one_value);
				return 1;
			}

			return 0;
		}

		int Function_CheckLineSum(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string command, string argument, ref double val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf = "";

			arg.Set(argument);

			int x1=0, x2=0, y1=0, y2=0;
			int x;
			double one_value = 0;
			int  count = 0;
			int table_no;
			CELL_STRUCT cell_t;
			bool retn;
			TABLE_STRUCT table_t;

			val = 0;

			while(true) 
			{ 
				arg.GetArgument(ref buf);
				if(buf.Length == 0)	break;

				table_no = cell_table;
				GetTargetZoneOnTarget(rt, table_no, ref x1, ref y1, ref x2, ref y2, buf);

				table_t = (TABLE_STRUCT)rt.tableBuf[table_no];

				y1 = nRecurseCellY+y1;
				if(y1 < 0)					y1 = nRecurseCellY;
				if(y1 >= table_t.cell_y)	y1 = nRecurseCellY;

				for(x = x1; x <= x2; x++) 
				{
					cell_t = (CELL_STRUCT)table_t.cellBuf[x+y1*table_t.cell_x];
			
					if(cell_t.bCalced == 1) 
					{
						retn = GetValueCalcedData(cell_t, ref one_value);
					}
					else 
					{
						cell_t.bCalced = 1;
						retn = GetValueRecurse(rt, rs, table_no, cell_t.text, cell_t.text.Length, ref one_value);
						MakeCellString(cell_t, one_value, retn);
					}

					if(retn) 
					{
						count++;
						val += one_value;
					}
				}
			}

			if(count == 0)	return 0;

			return 1;
		}

		int Function_CheckLineMin(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string command, string argument, ref double val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf = "";

			arg.Set(argument);

			int x1=0, x2=0, y1=0, y2=0;
			int x;
			double one_value = 0;
			int  count = 0;
			int table_no;
			CELL_STRUCT cell_t;
			bool retn;
			TABLE_STRUCT table_t;

			val = 0;

			while(true) 
			{ 
				arg.GetArgument(ref buf);
				if(buf.Length == 0)	break;

				table_no = cell_table;
				GetTargetZoneOnTarget(rt, table_no, ref x1, ref y1, ref x2, ref y2, buf);

				table_t = (TABLE_STRUCT)rt.tableBuf[table_no];

				y1 = nRecurseCellY+y1;
				if(y1 < 0)					y1 = nRecurseCellY;
				if(y1 >= table_t.cell_y)	y1 = nRecurseCellY;

				for(x = x1; x <= x2; x++) 
				{
					cell_t = (CELL_STRUCT)table_t.cellBuf[x+y1*table_t.cell_x];

					if(cell_t.bCalced == 1) 
					{
						retn = GetValueCalcedData(cell_t, ref one_value);
					}
					else 
					{
						cell_t.bCalced = 1;
						retn = GetValueRecurse(rt, rs, table_no, cell_t.text, cell_t.text.Length, ref one_value);
						MakeCellString(cell_t, one_value, retn);
					}

					if(retn) 
					{
						if(count == 0)	val = one_value;
						else 
						{
							if(one_value < val)	val = one_value;
						}

						count++;
					}
				}
			}

			if(count == 0)	return 0;

			return 1;
		}

		int Function_CheckLineMax(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string command, string argument, ref double val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf = "";

			arg.Set(argument);

			int x1=0, x2=0, y1=0, y2=0;
			int x;
			double one_value = 0;
			int  count = 0;
			int table_no;
			CELL_STRUCT cell_t;
			bool retn;
			TABLE_STRUCT table_t;

			val = 0;

			while(true) 
			{ 
				arg.GetArgument(ref buf);
				if(buf.Length == 0)	break;

				table_no = cell_table;
				GetTargetZoneOnTarget(rt, table_no, ref x1, ref y1, ref x2, ref y2, buf);

				table_t = (TABLE_STRUCT)rt.tableBuf[table_no];

				y1 = nRecurseCellY+y1;
				if(y1 < 0)					y1 = nRecurseCellY;
				if(y1 >= table_t.cell_y)	y1 = nRecurseCellY;

				for(x = x1; x <= x2; x++) 
				{
					cell_t = (CELL_STRUCT)table_t.cellBuf[x+y1*table_t.cell_x];

					if(cell_t.bCalced == 1) 
					{
						retn = GetValueCalcedData(cell_t, ref one_value);
					}
					else 
					{
						cell_t.bCalced = 1;
						retn = GetValueRecurse(rt, rs, table_no, cell_t.text, cell_t.text.Length, ref one_value);
						MakeCellString(cell_t, one_value, retn);
					}

					if(retn) 
					{
						if(count == 0)	val = one_value;
						else 
						{
							if(one_value > val)	val = one_value;
						}

						count++;
					}
				}
			}

			if(count == 0)	return 0;

			return 1;
		}

		int Function_CheckLineAve(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string command, string argument, ref double val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf = "";

			arg.Set(argument);

			int x1=0, x2=0, y1=0, y2=0;
			int x;
			double one_value = 0;
			int  count = 0;
			int table_no;
			CELL_STRUCT cell_t;
			bool retn;
			TABLE_STRUCT table_t;

			val = 0;

			while(true) 
			{ 
				arg.GetArgument(ref buf);
				if(buf.Length == 0)	break;

				table_no = cell_table;
				GetTargetZoneOnTarget(rt, table_no, ref x1, ref y1, ref x2, ref y2, buf);

				table_t = (TABLE_STRUCT)rt.tableBuf[table_no];

				y1 = nRecurseCellY+y1;
				if(y1 < 0)					y1 = nRecurseCellY;
				if(y1 >= table_t.cell_y)	y1 = nRecurseCellY;

				for(x = x1; x <= x2; x++) 
				{
					cell_t = (CELL_STRUCT)table_t.cellBuf[x+y1*table_t.cell_x];

					if(cell_t.bCalced == 1) 
					{
						retn = GetValueCalcedData(cell_t, ref one_value);
					}
					else 
					{
						cell_t.bCalced = 1;
						retn = GetValueRecurse(rt, rs, table_no, cell_t.text, cell_t.text.Length, ref one_value);
						MakeCellString(cell_t, one_value, retn);
					}

					if(retn) 
					{
						count++;
						val += one_value;
					}
				}
			}

			if(count == 0)	return 0;

			val /= count;

			return 1;
		}

		int Function_CheckLineSub(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string command, string argument, ref double val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf="";

			arg.Set(argument);

			int x1=0, x2=0, y1=0, y2=0;
			double val_one = 0, val_two = 0;
			//int  count = 0;
			int table_no;
			CELL_STRUCT cell_1, cell_2;
			bool retn1, retn2;
			TABLE_STRUCT table_t;

			val = 0;

			arg.GetArgument(ref buf);

			table_no = cell_table;
			GetTargetZoneOnTargetByLineSub(rt, table_no, ref x1, ref y1, ref x2, ref y2, buf);

			table_t = (TABLE_STRUCT)rt.tableBuf[table_no];

			y1 = nRecurseCellY+y1;
			if(y1 < 0)					y1 = nRecurseCellY;
			if(y1 >= table_t.cell_y)	y1 = nRecurseCellY;

			y2 = nRecurseCellY+y2;
			if(y2 < 0)					y2 = nRecurseCellY;
			if(y2 >= table_t.cell_y)	y2 = nRecurseCellY;

			cell_1 = (CELL_STRUCT)table_t.cellBuf[x1+y1*table_t.cell_x];
			cell_2 = (CELL_STRUCT)table_t.cellBuf[x2+y2*table_t.cell_x];

			if(cell_1.bCalced == 1) 
			{
				retn1 = GetValueCalcedData(cell_1, ref val_one);
			}
			else 
			{
				cell_1.bCalced = 1;
				retn1 = GetValueRecurse(rt, rs, table_no, cell_1.text, cell_1.text.Length, ref val_one);
				MakeCellString(cell_1, val_one, retn1);
			}

			if(cell_2.bCalced == 1) 
			{
				retn2 = GetValueCalcedData(cell_2, ref val_two);
			}
			else 
			{
				cell_2.bCalced = 1;
				retn2 = GetValueRecurse(rt, rs, table_no, cell_2.text, cell_2.text.Length, ref val_two);
				MakeCellString(cell_2, val_two, retn2);
			}

			if(retn1 == false || retn2 == false) 
			{
				return 0;
			}

			val = val_one-val_two;

			return 1;
		}

		int Function_CheckMinCellText(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string command, string argument, ref double val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf = "";

			arg.Set(argument);

			int x1=0, y1=0, x2=0, y2=0;
			int dx1=0, dy1=0, dx2=0, dy2=0;	// display zone
			int minx, miny = 0;
			int x, y;
			double one_value = 0;
			int  count = 0;
			int table_no;
			CELL_STRUCT cell_t;
			CELL_STRUCT cell_s;
			bool retn;
			TABLE_STRUCT table_t = new TABLE_STRUCT();
			TABLE_STRUCT table_s;

			val = 0;

			arg.GetArgument(ref buf);
			table_no = cell_table;
			GetTargetZone(rt, rs, ref table_no, ref dx1, ref dy1, ref dx2, ref dy2, buf);

			while(true) 
			{ 
				arg.GetArgument(ref buf);
				if(buf.Length == 0)	break;

				table_no = cell_table;
				GetTargetZone(rt, rs, ref table_no, ref x1, ref y1, ref x2, ref y2, buf);

				table_s = (TABLE_STRUCT)rs.tableBuf[table_no];
				table_t = (TABLE_STRUCT)rt.tableBuf[table_no];

				for(y = y1; y <= y2; y++) 
				{
					for(x = x1; x <= x2; x++) 
					{
						cell_t = (CELL_STRUCT)table_t.cellBuf[x+y*table_t.cell_x];
						cell_s = GetSourcePointer(table_s, cell_t);

						if(cell_t.bCalced == 1) 
						{
							retn = GetValueCalcedData(cell_t, ref one_value);
						}
						else 
						{
							cell_t.bCalced = 1;
							retn = GetValueRecurse(rt, rs, table_no, cell_t.text, cell_t.text.Length, ref one_value);
							MakeCellString(cell_t, one_value, retn);
						}

						if(retn) 
						{
							if(count == 0) 
							{	
								val = one_value;
								minx = x;
								miny = y;
							}
							else 
							{
								if(one_value < val) 
								{
									val = one_value;
									minx = x;
									miny = y;
								}
							}

							count++;
						}
					}
				}
			}

			if(count == 0) 
			{
				sRecurseStringValue = "***";
				bRecurseStringValueFlag = true;
				return 0;
			}

			cell_t = (CELL_STRUCT)table_t.cellBuf[dx1+miny*table_t.cell_x];
			sRecurseStringValue = cell_t.text;
			bRecurseStringValueFlag = true;

			return 1;
		}

		int Function_CheckMaxCellText(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string command, string argument, ref double val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf = "";

			arg.Set(argument);

			int x1=0, y1=0, x2=0, y2=0;
			int dx1=0, dy1=0, dx2=0, dy2=0;	// display zone
			int minx, miny=0;
			int x, y;
			double one_value = 0;
			int  count = 0;
			int table_no;
			CELL_STRUCT cell_t;
			CELL_STRUCT cell_s;
			bool retn;
			TABLE_STRUCT table_t = new TABLE_STRUCT();
			TABLE_STRUCT table_s;

			val = 0;

			arg.GetArgument(ref buf);
			table_no = cell_table;
			GetTargetZone(rt, rs, ref table_no, ref dx1, ref dy1, ref dx2, ref dy2, buf);

			while(true) 
			{ 
				arg.GetArgument(ref buf);
				if(buf.Length == 0)	break;

				table_no = cell_table;
				GetTargetZone(rt, rs, ref table_no, ref x1, ref y1, ref x2, ref y2, buf);

				table_s = (TABLE_STRUCT)rs.tableBuf[table_no];
				table_t = (TABLE_STRUCT)rt.tableBuf[table_no];

				for(y = y1; y <= y2; y++) 
				{
					for(x = x1; x <= x2; x++) 
					{
						cell_t = (CELL_STRUCT)table_t.cellBuf[x+y*table_t.cell_x];
						cell_s = GetSourcePointer(table_s, cell_t);

						if(cell_t.bCalced == 1) 
						{
							retn = GetValueCalcedData(cell_t, ref one_value);
						}
						else 
						{
							cell_t.bCalced = 1;
							retn = GetValueRecurse(rt, rs, table_no, cell_t.text, cell_t.text.Length, ref one_value);
							MakeCellString(cell_t, one_value, retn);
						}

						if(retn) 
						{
							if(count == 0) 
							{	
								val = one_value;
								minx = x;
								miny = y;
							}
							else 
							{
								if(one_value > val) 
								{	
									val = one_value;
									minx = x;
									miny = y;
								}
							}

							count++;
						}
					}
				}
			}

			if(count == 0) 
			{
				sRecurseStringValue = "***";
				bRecurseStringValueFlag = true;
				return 0;
			}

			cell_t = (CELL_STRUCT)table_t.cellBuf[dx1+miny*table_t.cell_x];
			sRecurseStringValue = cell_t.text;
			bRecurseStringValueFlag = true;

			return 1;
		}

		int Function_CheckGetLineCount(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string command, string argument, ref double val)
		{
			ScriptArgumentString arg = new ScriptArgumentString();
			string buf = "";

			arg.Set(argument);

			int x1=0, y1=0, x2=0, y2=0;
			//int  count = 0;
			int table_no;

			val = 0;

			arg.GetArgument(ref buf);

			table_no = cell_table;
			GetTargetZone(rt, rs, ref table_no, ref x1, ref y1, ref x2, ref y2, buf);

			val = y2-y1+1;

			return 1;
		}

		int Function_Check(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string command, string argument, ref double val)
		{
			if(String.Compare(command, "sum", true) == 0) 
			{
				return Function_CheckSum(rt, rs, cell_table, command, argument, ref val);
			}
			else if(String.Compare(command, "min", true) == 0) 
			{
				return Function_CheckMin(rt, rs, cell_table, command, argument, ref val);
			}
			else if(String.Compare(command, "max", true) == 0) 
			{
				return Function_CheckMax(rt, rs, cell_table, command, argument, ref val);
			}
			else if(String.Compare(command, "ave", true) == 0) 
			{
				return Function_CheckAve(rt, rs, cell_table, command, argument, ref val);
			}
			else if(String.Compare(command, "AbsMin", true) == 0) 
			{
				return Function_CheckAbsMin(rt, rs, cell_table, command, argument, ref val);
			}
			else if(String.Compare(command, "AbsMax", true) == 0) 
			{
				return Function_CheckAbsMax(rt, rs, cell_table, command, argument, ref val);
			}
			else if(String.Compare(command, "AbsAve", true) == 0) 
			{
				return Function_CheckAbsAve(rt, rs, cell_table, command, argument, ref val);
			}
			else if(String.Compare(command, "abs", true) == 0) 
			{
				return Function_CheckAbs(rt, rs, cell_table, command, argument, ref val);
			}
			else if(String.Compare(command, "MinCellText", true) == 0) 
			{
				return Function_CheckMinCellText(rt, rs, cell_table, command, argument, ref val);
			}
			else if(String.Compare(command, "MaxCellText", true) == 0) 
			{
				return Function_CheckMaxCellText(rt, rs, cell_table, command, argument, ref val);
			}
			else if(String.Compare(command, "GetLineCount", true) == 0) 
			{
				return Function_CheckGetLineCount(rt, rs, cell_table, command, argument, ref val);
			}
			else if(String.Compare(command, "LineSum", true) == 0) 
			{
				return Function_CheckLineSum(rt, rs, cell_table, command, argument, ref val);
			}
			else if(String.Compare(command, "LineMin", true) == 0) 
			{
				return Function_CheckLineMin(rt, rs, cell_table, command, argument, ref val);
			}
			else if(String.Compare(command, "LineMax", true) == 0) 
			{
				return Function_CheckLineMax(rt, rs, cell_table, command, argument, ref val);
			}
			else if(String.Compare(command, "LineAve", true) == 0) 
			{
				return Function_CheckLineAve(rt, rs, cell_table, command, argument, ref val);
			}
			else if(String.Compare(command, "LineSub", true) == 0) 
			{
				return Function_CheckLineSub(rt, rs, cell_table, command, argument, ref val);
			}
			else{}

			return 0;	// 일치하는 함수가 없다.
		}

		bool IsNumberString(string buf, int size)
		{
			int i;

			for(i = 0; i < size; i++) 
			{
				if(buf[i] >= '0' && buf[i] <= '9') 	continue;
				if(buf[i] == '.')					continue;
				if(buf[i] == 32)					continue;

				return false;
			}
			return true;
		}

		//------------------------------------------------------------------------------
		//	스트링 값을 Long Doubld형으로 변환 읽어온다.
		//------------------------------------------------------------------------------

		double StringToFloat(string str, int size)
		{
			string stack;

			stack = str.Substring(0,size);

			return ConvertTool.ToDouble(stack);
		}

		//------------------------------------------------------------------------------
		//	하나의 요소만 남았을 때 숫자이거나 함수일 때.
		//	(예) @cell(ex), 12345, 12.3, @average 등등
		//------------------------------------------------------------------------------

        bool GetValueElseFunction(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string s, int size, ref double val)
        {
            return GetValueElseFunction(rt, rs, cell_table, s, 0, size, ref val);
        }

        T GetCachedObjectStruct<T>(ConcurrentDictionary<string, T> cache, string text, Func<string, T> parseFunc) where T : class
        {
            if (string.IsNullOrEmpty(text))
            {
                return parseFunc(text);
            }

            T cached = cache.GetOrAdd(text, parseFunc);
            return (T)Tools.CopyObject(cached);
        }

        OBJECT_AI_ONE_DATA GetAiOneDataStruct(string text)
        {
            return GetCachedObjectStruct(_aiOneDataCache, text, value =>
            {
                OBJECT_AI_ONE_DATA obj = new OBJECT_AI_ONE_DATA();
                ReportLib.ObjectStringToStruct(ref obj, value);
                return obj;
            });
        }

        OBJECT_AI_MULTI_DATA GetAiMultiDataStruct(string text)
        {
            return GetCachedObjectStruct(_aiMultiDataCache, text, value =>
            {
                OBJECT_AI_MULTI_DATA obj = new OBJECT_AI_MULTI_DATA();
                ReportLib.ObjectStringToStruct(ref obj, value);
                return obj;
            });
        }

        OBJECT_MOMENT_DATA GetMomentDataStruct(string text)
        {
            return GetCachedObjectStruct(_momentDataCache, text, value =>
            {
                OBJECT_MOMENT_DATA obj = new OBJECT_MOMENT_DATA();
                ReportLib.ObjectStringToStruct(ref obj, value);
                return obj;
            });
        }

        OBJECT_AI_MAX_SUM GetAiMaxSumStruct(string text)
        {
            return GetCachedObjectStruct(_aiMaxSumCache, text, value =>
            {
                OBJECT_AI_MAX_SUM obj = new OBJECT_AI_MAX_SUM();
                ReportLib.ObjectStringToStruct(ref obj, value);
                return obj;
            });
        }

        OBJECT_DI_ONE_DATA GetDiOneDataStruct(string text)
        {
            return GetCachedObjectStruct(_diOneDataCache, text, value =>
            {
                OBJECT_DI_ONE_DATA obj = new OBJECT_DI_ONE_DATA();
                ReportLib.ObjectStringToStruct(ref obj, value);
                return obj;
            });
        }

        OBJECT_DI_MULTI_DATA GetDiMultiDataStruct(string text)
        {
            return GetCachedObjectStruct(_diMultiDataCache, text, value =>
            {
                OBJECT_DI_MULTI_DATA obj = new OBJECT_DI_MULTI_DATA();
                ReportLib.ObjectStringToStruct(ref obj, value);
                return obj;
            });
        }

		bool GetValueElseFunction(REPORT_STRUCT rt, REPORT_STRUCT rs, int cell_table, string s, int start, int size, ref double val)
		{
            if (start != 0)
            {
                s = s.Substring(start, size);
                start = 0;
            }

			int start_pos=0, end_pos=0;
			int name_size;
			//double val1 = 0;

			if(s[0] == '@') 
			{	// 함수.
				if(!SeekStartEnd(s, size, ref start_pos, ref end_pos)) 
				{
					if(Tools.IsLangKorean()) 
					{
						ErrorMessage("함수가 () 로 닫혀있지 않음");
					}
					else 
					{
						ErrorMessage("() count mismathed of function");
					}
					return false;
				}

				name_size = start_pos-1;
				if(name_size < 1)		return false;
				if(name_size > 900)	return false;

				int arg_size = end_pos-start_pos-1;	// ()를 뺀 argument size
				string func_name;
				string func_arg;
		
				func_name = s.Substring(1, name_size);

				func_arg = s.Substring(start_pos+1, arg_size);

				int retn;

				retn = Function_Check(rt, rs, cell_table, func_name, func_arg, ref val);
				if(retn == -1)	return false;
				if(retn == 1)	return true;

				ErrorMessage("unknown function ({0})", func_name);

				return false;
			}
			else if(s[0] == '$') 
			{	// 문자열 변수
				string buf;

				buf = s.Substring(1);

                if (StringVar.GetStringVar(buf, out val)) return true;

				ErrorMessage("Undefined String Var ({0})", buf);

				return false;
			}
			else 
			{
				if(IsNumberString(s, size)) 
				{	// 숫자일 때
					val = StringToFloat(s, size);
					return true;
				}
				else 
				{							// 이 외의 경우일 때는 셀이라고 가정한다.
					TABLE_STRUCT table_s;
					TABLE_STRUCT table_t;
					CELL_STRUCT cell_t;
					CELL_STRUCT cell_s;
					int x1=0, y1=0, x2=0, y2=0;
					string buf;
					int table_no;
					bool retn;

					buf = s.Substring(0, size);

					table_no = cell_table;
					GetTargetZone(rt, rs, ref table_no, ref x1, ref y1, ref x2, ref y2, buf);

					table_t = (TABLE_STRUCT)rt.tableBuf[table_no];
					cell_t = (CELL_STRUCT)table_t.cellBuf[x1+y1*table_t.cell_x];
			
					table_s = (TABLE_STRUCT)rs.tableBuf[table_no];
					cell_s = GetSourcePointer(table_s, cell_t);

					if(cell_t.bCalced == 1) 
					{
						retn = GetValueCalcedData(cell_t, ref val);
					}
					else 
					{
						cell_t.bCalced = 1;
						retn = GetValueRecurse(rt, rs, table_no, cell_t.text, cell_t.text.Length, ref val);
						MakeCellString(cell_t, val, retn);
					}

					if(ReportConfig.bCalcNoData)
						return true;
					else
						return retn;
				}
			}
		}

		int IsStringInclude(string source, string seek)
		{
			int s_hap = source.Length;
			int t_hap = seek.Length;

			if(s_hap == 0 || t_hap == 0)	return 0;
			if(t_hap > s_hap)				return 0;	// 검사하고자 하는 스트링이 더 작다. 비교할 필요도 없다.

			//bool match = false;
			int  pos = 0;
			int  i;

			for(i = 0; i < s_hap; i++) 
			{
				if(source[i] == seek[pos]) 
				{	// yes one char matched
					pos++;
					if(pos >= t_hap)	return 1;		// yes all string matched
				}
				else 
				{
					pos = 0;
				}
			}

			return 0;
		}

	}

	class ON_OFF_LIST_SORT
	{
		//struct date d;	// 이것은 데이터를 소트하기 위해서 존재한다.
		//struct time t;	// 이것은 데이터를 소트하기 위해서 존재한다.
		public DateTime t = new DateTime();
		public string imsi;
	}
}

/*
enum {
	SOURCE_RECURSE,	// 현재 source 리포터에서 계산중이다.
	TARGET_RECURSE,	// 현재 target 리포터에서 계산중이다.
};

*/
