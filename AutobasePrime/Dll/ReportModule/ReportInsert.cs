using System;
using System.Windows.Forms;
using DialogTag;
using NetTools;
using ReportBasicLib;

namespace ReportModule
{
	/// <summary>
	/// Summary description for ReportInsert.
	/// </summary>
	public class ReportInsert
	{
		public ReportInsert()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static bool SelectTagAI(out string tag)
		{
			tag = "";
			FormSelectTag dialog = new FormSelectTag();
			dialog.bUseTagAI = true;

            if (dialog.ShowDialog(Form.ActiveForm) != DialogResult.OK) return false;
			tag = dialog.sTag;

			return true;
		}

		static bool SelectTagDI(out string tag)
		{
			tag = "";
			FormSelectTag dialog = new FormSelectTag();
			dialog.bUseTagDI = true;
            if (dialog.ShowDialog(Form.ActiveForm) != DialogResult.OK) return false;
			tag = dialog.sTag;

			return true;
		}

        static bool SelectTagST(out string tag)
        {
            tag = "";
            FormSelectTag dialog = new FormSelectTag();
            dialog.bUseTagST = true;
            if (dialog.ShowDialog(Form.ActiveForm) != DialogResult.OK) return false;
            tag = dialog.sTag;

            return true;
        }

		static bool SelectTimeZone(CELL_TIME cell_time, bool flag)
		{
			PropertySheetPublic sheet = new PropertySheetPublic();

			PropertySelectTime time = new PropertySelectTime();

			time.SetTime(cell_time);
			//time.UseToDataTime = false;

			sheet.AddPage(time);
            sheet.StartPosition = FormStartPosition.CenterParent;

            if (sheet.ShowDialog(Form.ActiveForm) == DialogResult.OK)
			{
				time.GetTime(cell_time);

				return true;
			}

			return false;
		}

        static string MakeRelativeColumnId(int cell_x)
        {
            if (cell_x < 26)
            {
                return String.Format("{0}", (char)(cell_x + 'A'));
            }

            return String.Format("{0}{1}", (char)((cell_x / 26) - 1 + 'A'), (char)((cell_x % 26) + 'A'));
        }

        static string MakeRelativeCellId(int cell_x, int cell_y)
        {
            return String.Format("{0}{1}", MakeRelativeColumnId(cell_x), cell_y);
        }

        public static string MakeRelativeCellIdPublic(int cell_x, int cell_y)
        {
            return MakeRelativeCellId(cell_x, cell_y);
        }

        static void GetSelectedFunctionRange(FormReportChild form, out string cellId, out string rangeId, out string lineRangeId, out string displayColumnId)
        {
            cellId = "A0";
            rangeId = "A0:A3";
            lineRangeId = "A:C";
            displayColumnId = "A";

            REPORT_STRUCT report = form.GetReportStruct();
            if (report == null || report.TableCount <= 0 || report.cursor_table >= report.TableCount)
            {
                return;
            }

            int x1, y1, x2, y2;
            ReportLib.GetCursorZone(report, out x1, out y1, out x2, out y2);

            cellId = MakeRelativeCellId(x1, y1);
            rangeId = x1 == x2 && y1 == y2
                ? cellId
                : String.Format("{0}:{1}", MakeRelativeCellId(x1, y1), MakeRelativeCellId(x2, y2));
            lineRangeId = x1 == x2
                ? MakeRelativeColumnId(x1)
                : String.Format("{0}:{1}", MakeRelativeColumnId(x1), MakeRelativeColumnId(x2));
            displayColumnId = MakeRelativeColumnId(x1);
        }

        public static string MakeFunctionBySelection(FormReportChild form, string sampleFunction)
        {
            string cellId;
            string rangeId;
            string lineRangeId;
            string displayColumnId;
            GetSelectedFunctionRange(form, out cellId, out rangeId, out lineRangeId, out displayColumnId);

            switch (sampleFunction)
            {
                case "@ave(A0:A3)":
                    return String.Format("@ave({0})", rangeId);
                case "@max(A0:A3)":
                    return String.Format("@max({0})", rangeId);
                case "@min(A0:A3)":
                    return String.Format("@min({0})", rangeId);
                case "@sum(A0:A3)":
                    return String.Format("@sum({0})", rangeId);
                case "@LineAve(A:C)":
                    return String.Format("@LineAve({0})", lineRangeId);
                case "@LineMax(A:C)":
                    return String.Format("@LineMax({0})", lineRangeId);
                case "@LineMin(A:C)":
                    return String.Format("@LineMin({0})", lineRangeId);
                case "@LineSum(A:C)":
                    return String.Format("@LineSum({0})", lineRangeId);
                case "@LineSub(A:C)":
                    return String.Format("@LineSub({0})", lineRangeId);
                case "@AbsAve(A0:A3)":
                    return String.Format("@AbsAve({0})", rangeId);
                case "@AbsMax(A0:A3)":
                    return String.Format("@AbsMax({0})", rangeId);
                case "@AbsMin(A0:A3)":
                    return String.Format("@AbsMin({0})", rangeId);
                case "@abs(A0)":
                    return String.Format("@abs({0})", cellId);
                case "@MinCellText(A, B0:B3)":
                    return String.Format("@MinCellText({0}, {1})", displayColumnId, rangeId);
                case "@MaxCellText(A, B0:B3)":
                    return String.Format("@MaxCellText({0}, {1})", displayColumnId, rangeId);
                case "@GetLineCount(B1:B1)":
                    return String.Format("@GetLineCount({0})", rangeId);
            }

            return sampleFunction;
        }

        public static void InsertFunctionSample(FormReportChild form, string sampleFunction)
        {
            if (form == null) return;

            string imsi = String.Format("={0}", MakeFunctionBySelection(form, sampleFunction));
            SelectedCell.SelectedCellSetText(form.GetReportStruct(), imsi);
            form.Invalidate();
            form.DrawCellInfo(form.GetReportStruct());
            form.SetChangeFlag();
        }

		public static void InsertAutoData(FormReportChild form)
		{
			FormDialogInsertAutoData dialog = new FormDialogInsertAutoData();
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (dialog.ShowDialog(Form.ActiveForm) == DialogResult.OK) 
			{
				string command = dialog.sSelectedCommand;
				EnumCommand eCommand = ReportLib.ChangeCommandStringToId(command);
				string sDescription;
				
				ReportLib.ChangeCommandIdToDes(out sDescription, eCommand);

				string buf = "";
				
				switch(eCommand) 
				{
					case EnumCommand.COMMAND_AI_CURR:
					{
						OBJECT_AI_CURR obj = new OBJECT_AI_CURR();

						if(!SelectTagAI(out obj.tag))	return;

						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}

					case EnumCommand.COMMAND_AI_AVE:
					case EnumCommand.COMMAND_AI_MAX:
					case EnumCommand.COMMAND_AI_MIN:
					case EnumCommand.COMMAND_AI_SUM:
					case EnumCommand.COMMAND_AI_SUB: 
					{
						OBJECT_AI_ONE_DATA obj = new OBJECT_AI_ONE_DATA();
						obj.command = command;
						if(!SelectTagAI(out obj.tag))		return;
						if(!SelectTimeZone(obj.time, false))	return;
						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
					case EnumCommand.COMMAND_AI_MOMENT: 
					case EnumCommand.COMMAND_AI_MULTI_MOMENT: 
					case EnumCommand.COMMAND_DI_MOMENT: 
					case EnumCommand.COMMAND_DI_MULTI_MOMENT: 
					{
						OBJECT_MOMENT_DATA obj = new OBJECT_MOMENT_DATA();
						obj.day = 1;
						obj.command = command;
						if(eCommand == EnumCommand.COMMAND_AI_MOMENT || 
							eCommand == EnumCommand.COMMAND_AI_MULTI_MOMENT) 
						{
							if(!SelectTagAI(out obj.tag))		return;
						}
						else 
						{
							if(!SelectTagDI(out obj.tag))		return;
						}
						if(!SelectTimeZone(obj.time, false))	return;
						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
					case EnumCommand.COMMAND_AI_MAX_SUM: 
					case EnumCommand.COMMAND_AI_MULTI_MAX_SUM: 
					{
						OBJECT_AI_MAX_SUM obj = new OBJECT_AI_MAX_SUM();
						obj.nDataType = 1;
						obj.command = command;
						if(!SelectTagAI(out obj.tag))		return;
						if(!SelectTimeZone(obj.time, false))	return;
						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
					case EnumCommand.COMMAND_AI_MULTI_AVE:
					case EnumCommand.COMMAND_AI_MULTI_MAX:
					case EnumCommand.COMMAND_AI_MULTI_MIN:
					case EnumCommand.COMMAND_AI_MULTI_SUM:
					case EnumCommand.COMMAND_AI_MULTI_SUB: 
					{
						OBJECT_AI_MULTI_DATA obj = new OBJECT_AI_MULTI_DATA();
						obj.command = command;
						if(!SelectTagAI(out obj.tag))		return;
						if(!SelectTimeZone(obj.time, false))	return;
						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
					case EnumCommand.COMMAND_AI_ALARM: 
					{
						OBJECT_ALARM obj = new OBJECT_ALARM();
						obj.command = command;
						if(!ReportEditorProperty.ConfigAiAlarm(obj, sDescription))		return;
						//if(!SelectTimeZone(obj.time, false))	return;
						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
					case EnumCommand.COMMAND_AI_MIN_LIST: 
					{
						OBJECT_AI_MIN_LIST obj = new OBJECT_AI_MIN_LIST();
						obj.min_gab = "5";
						//if(!SelectTagAI(out obj.tag))			return;
						if(!ReportEditorProperty.ConfigAiMinList(obj, sDescription))	return;
						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
					case EnumCommand.COMMAND_DI_CURR:
					{
						OBJECT_DI_CURR obj = new OBJECT_DI_CURR();
						if(!SelectTagDI(out obj.tag))	return;
						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
					case EnumCommand.COMMAND_DI_ONTIME:
					case EnumCommand.COMMAND_DI_OFFTIME:
					case EnumCommand.COMMAND_DI_ONCOUNT:
					{
						OBJECT_DI_ONE_DATA obj = new OBJECT_DI_ONE_DATA();
						obj.command = command;
						if(!SelectTagDI(out obj.tag))		return;
						if(!SelectTimeZone(obj.time, false))	return;
						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
					case EnumCommand.COMMAND_DI_MULTI_ONTIME:
					case EnumCommand.COMMAND_DI_MULTI_OFFTIME:
					case EnumCommand.COMMAND_DI_MULTI_ONCOUNT:
					{
						OBJECT_DI_MULTI_DATA obj = new OBJECT_DI_MULTI_DATA();
						obj.command = command;
						if(!SelectTagDI(out obj.tag))		return;
						if(!SelectTimeZone(obj.time, false))	return;
						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
					case EnumCommand.COMMAND_DI_ONOFF_LIST:
					{
						OBJECT_DI_ONOFF_LIST obj = new OBJECT_DI_ONOFF_LIST();
						obj.field_time = 0;

						if(!ReportEditorProperty.ConfigOnOffList(obj, sDescription))		return;
						//if(!SelectTimeZone(obj.time, false))	return;
						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
					case EnumCommand.COMMAND_DI_ONOFF_LIST_SUM:
					case EnumCommand.COMMAND_DI_MULTI_ONOFF_LIST_SUM:
					{
						OBJECT_DI_ONOFF_LIST_SUM obj = new OBJECT_DI_ONOFF_LIST_SUM();
						DISPLAY_FORMAT_STRUCT format = new DISPLAY_FORMAT_STRUCT();

						obj.command = command;
						obj.field_time = 0;
						ReportLib.FillDefaultDisplayFormat(format);
						format.cType = 3;

						if(!ReportEditorProperty.ConfigOnOffListSum(obj, sDescription))		return;
						//if(!SelectTimeZone(obj.time, false))	return;
						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						SelectedCell.SelectedCellSetFormat(form.GetReportStruct(), format);
						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
					case EnumCommand.COMMAND_DI_ALARM: 
					{
						OBJECT_ALARM obj = new OBJECT_ALARM();

						obj.command = command;
						if(!ReportEditorProperty.ConfigDiAlarm(obj, sDescription))		return;
						//if(!SelectTimeZone(obj.time, false))	return;
						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
					case EnumCommand.COMMAND_ETC_DATA_TIME:
					{
						OBJECT_ETC_DATA_TIME obj = new OBJECT_ETC_DATA_TIME();
						DISPLAY_FORMAT_STRUCT format = new DISPLAY_FORMAT_STRUCT();

						ReportLib.FillDefaultDisplayFormat(format);
						format.cType = 2;
						//if(!SelectDataTimeStyle(&obj))		return;

						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						SelectedCell.SelectedCellSetFormat(form.GetReportStruct(), format);
						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
					case EnumCommand.COMMAND_ETC_TIME:
					{
						OBJECT_ETC_TIME obj = new OBJECT_ETC_TIME();
						DISPLAY_FORMAT_STRUCT format = new DISPLAY_FORMAT_STRUCT();
					
						ReportLib.FillDefaultDisplayFormat(format);
						format.cType = 2;
					
						// if(!SelectTimeStyle(&obj))		return;
						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						SelectedCell.SelectedCellSetFormat(form.GetReportStruct(), format);
						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
					case EnumCommand.COMMAND_ETC_MULTI_COUNT:
					{
						OBJECT_ETC_MULTI_COUNT obj = new OBJECT_ETC_MULTI_COUNT();
						DISPLAY_FORMAT_STRUCT format = new DISPLAY_FORMAT_STRUCT();

						if(!SelectTimeZone(obj.time, false))	return;
						ReportLib.ObjectStructToString(ref buf, obj);
						ReportLib.FillDefaultDisplayFormat(format);
						format.cType = 2;

                        if (Tools.IsLangKorean())
                        {
                            if (String.Compare(obj.time.zone, "Min") == 0)
                                format.cDateTime = 39;
                            else if (String.Compare(obj.time.zone, "Hour") == 0)
                                format.cDateTime = 49;
                            else if (String.Compare(obj.time.zone, "Day") == 0)
                                format.cDateTime = 56;
                            else if (String.Compare(obj.time.zone, "Mon") == 0)
                                format.cDateTime = 61;
                        }
                        else
                        {
                            if (String.Compare(obj.time.zone, "Min") == 0)
                                format.cDateTime = 1;
                            else if (String.Compare(obj.time.zone, "Hour") == 0)
                                format.cDateTime = 2;
                            else if (String.Compare(obj.time.zone, "Day") == 0)
                                format.cDateTime = 3;
                            else if (String.Compare(obj.time.zone, "Mon") == 0)
                                format.cDateTime = 4;
                        }

						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						SelectedCell.SelectedCellSetFormat(form.GetReportStruct(), format);
						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
					case EnumCommand.COMMAND_ETC_MIN_LIST: 
					{
						OBJECT_ETC_MIN_LIST obj = new OBJECT_ETC_MIN_LIST();
						DISPLAY_FORMAT_STRUCT format = new DISPLAY_FORMAT_STRUCT();

						ReportLib.FillDefaultDisplayFormat(format);
						format.cType = 2;
						format.cDateTime = 1;

						obj.min_gab = "5";
						if(!ReportEditorProperty.ConfigEtcMinList(obj, sDescription))	return;
						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						SelectedCell.SelectedCellSetFormat(form.GetReportStruct(), format);
						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
					case EnumCommand.COMMAND_AI_MAX_TIME:
					case EnumCommand.COMMAND_AI_MIN_TIME:
					{
						OBJECT_AI_ONE_DATA obj = new OBJECT_AI_ONE_DATA();
						DISPLAY_FORMAT_STRUCT format = new DISPLAY_FORMAT_STRUCT();


						obj.command = command;
						if(!SelectTagAI(out obj.tag))		return;
						if(!SelectTimeZone(obj.time, false))	return;

						ReportLib.FillDefaultDisplayFormat(format);
						format.cType = 2;

						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						SelectedCell.SelectedCellSetFormat(form.GetReportStruct(), format);

						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
					case EnumCommand.COMMAND_ETC_DATABASE:
					{
						OBJECT_ETC_DATABASE obj = new OBJECT_ETC_DATABASE();

						if(!ReportEditorProperty.ConfigEtcDatabase(obj, sDescription))	return;

						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);

						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
					case EnumCommand.COMMAND_ETC_STRING_VAR:
					{
						OBJECT_ETC_STRING_VAR obj = new OBJECT_ETC_STRING_VAR();

						FormDialogSelectVars dialogv = new FormDialogSelectVars();
                        dialogv.StartPosition = FormStartPosition.CenterParent;

                        if (dialogv.ShowDialog(Form.ActiveForm) != DialogResult.OK) return;

						obj.var = dialogv.sSelectedItem;

						ReportLib.ObjectStructToString(ref buf, obj);
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);

						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						form.SetChangeFlag();
						break;
					}
                case EnumCommand.COMMAND_ST_CURR:
                    {
                        OBJECT_ST_CURR obj = new OBJECT_ST_CURR();
                        if (!SelectTagST(out obj.tag)) return;
                        ReportLib.ObjectStructToString(ref buf, obj);
                        SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
                        form.Invalidate();
                        form.DrawCellInfo(form.GetReportStruct());
                        form.SetChangeFlag();
                        break;
                    }
					default:
						if(Tools.IsLangKorean()) 
						{
							buf = String.Format("새로운 명령어({0})", (int)eCommand);
						}
						else 
						{
							buf = String.Format("New command({0})", (int)eCommand);
						}
						SelectedCell.SelectedCellSetText(form.GetReportStruct(), buf);
						form.Invalidate();
						form.DrawCellInfo(form.GetReportStruct());
						break;

				}
			}
		}

		public static void InsertFunctionData(FormReportChild form)
		{
			FormDialogInsertFunction dialog = new FormDialogInsertFunction();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.FunctionPreviewResolver = sample => MakeFunctionBySelection(form, sample);

            if (dialog.ShowDialog(Form.ActiveForm) != DialogResult.OK) return;

            InsertFunctionSample(form, dialog.sFunction);
		}
	}
}


/*
 



*/ 
