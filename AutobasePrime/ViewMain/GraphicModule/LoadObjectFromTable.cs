using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using NetTools;
using System.IO;

namespace GraphicModule
{
    /// <summary>
    /// ObjectTable 로딩 클래스.
    /// </summary>
    class LoadObjectFromTable : LoadObjectFromModX
    {
        public ScriptClass scriptEventCellClick;

        // 테이블 관련 데이터
        public List<float> columnWidths = new List<float>();
        public List<float> rowHeights = new List<float>();
        public List<List<TableCellData>> cells = new List<List<TableCellData>>();

        public LoadObjectFromTable(ObjectCommonProperty ocp)
            : base(ocp)
        {

        }

        protected override bool ChildCheck(TextReader reader, string command, CommaTextReader comma)
        {
            if (String.Compare(command, "ScriptEventCellClick") == 0)
            {
                scriptEventCellClick = LoadOneScript(reader, command);
            }
            else if (String.Compare(command, "TableColumnWidths") == 0)
            {
                float val = 0;
                while (comma.GetFloat(ref val))
                {
                    columnWidths.Add(val);
                    val = 0;
                }
            }
            else if (String.Compare(command, "TableRowHeights") == 0)
            {
                float val = 0;
                while (comma.GetFloat(ref val))
                {
                    rowHeights.Add(val);
                    val = 0;
                }
            }
            else if (String.Compare(command, "TableCell") == 0)
            {
                int row = 0, col = 0;
                comma.GetInt(ref row);
                comma.GetInt(ref col);

                TableCellData cell = new TableCellData();
                string text = "";
                comma.GetString(ref text);
                cell.sText = ObjectTable.UnescapeCellText(text);

                string tagName = "";
                comma.GetString(ref tagName);
                cell.sTagName = ObjectTable.UnescapeCellText(tagName);

                comma.GetInt(ref cell.nColSpan);
                comma.GetInt(ref cell.nRowSpan);
                comma.GetInt(ref cell.nTextAlign);

                int foreColor = 0, backColor = 0;
                comma.GetInt(ref foreColor);
                comma.GetInt(ref backColor);
                if (foreColor != 0) cell.lForeColor = System.Drawing.Color.FromArgb(foreColor);
                if (backColor != 0) cell.lBackColor = System.Drawing.Color.FromArgb(backColor);

                int bold = 0;
                comma.GetInt(ref bold);
                cell.bBold = (bold == 1);

                int merged = 0;
                comma.GetInt(ref merged);
                cell.bMergedChild = (merged == 1);

                // 행 리스트 확장
                while (cells.Count <= row)
                    cells.Add(new List<TableCellData>());

                while (cells[row].Count <= col)
                    cells[row].Add(new TableCellData());

                cells[row][col] = cell;
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
