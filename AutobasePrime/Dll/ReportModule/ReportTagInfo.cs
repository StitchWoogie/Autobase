using System;
using System.Collections.Generic;
using System.Text;
using ReportBasicLib;
using System.Collections;
using AutoLibLocal;

namespace ReportModule
{
    public class ReportTagInfo
    {
        string sFileName;

        public void GetMultiSelectTagList(string filename, ArrayList block)
        {
            sFileName = filename;
            REPORT_STRUCT report = ReportFile.ReportLoad(filename, false);

            // TODO: add draw code for native data here
            TABLE_STRUCT table;
            int l;

            for (l = 0; l < report.TableCount; l++)
            {
                table = (TABLE_STRUCT)report.tableBuf[l];

                GetFromOneTable(block, table);
            }
        }

        void GetFromOneTable(ArrayList block, TABLE_STRUCT table)
        {
            int posx, posy;
            CELL_STRUCT cell = null;
            int cell_pos;

            cell_pos = 0;

            for (posy = 0; posy < table.cell_y; posy++)
            {
                cell_pos = posy * table.cell_x;

                for (posx = 0; posx < table.cell_x; posx++, cell_pos++)
                {
                    cell = (CELL_STRUCT)table.cellBuf[cell_pos];
                    GetFromOneCell(block, table.no, cell);
                }
            }
        }

        void GetFromOneCell(ArrayList block, int table_no, CELL_STRUCT cell)
        {
            SelectedCell.sTempCellText = "";
            SelectedCell.EnumProcGetCellTag(cell);

            if (SelectedCell.sTempCellText.Length > 0)
            {
                string id;
                FormReportChild.MakeCellIdString(out id, table_no, cell.x, cell.y);
                TagUtil.AddTagList(block, SelectedCell.sTempCellText, EnumTagType.none, sFileName, EnumTagUsedType.String, cell.text, id);
            }
        }
    }
}
