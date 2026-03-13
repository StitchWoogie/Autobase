using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;

namespace ScriptLibRun
{
    public class ColumnRowInfo
    {
        public int nColumn, nRow;

        public void SetColRow(int column, int row)
        {
            nColumn = column;
            nRow = row;
        }

        /*
        // Run에서 사용
        protected bool IsPublicLoadItem(string command, string second_command, CommaTextReader comma)
        {
            if (command == "ColRow")
            {
                nColumn = ConvertTool.ToInt32(second_command);
                nRow = comma.GetInt();
                return true;
            }

            return false;
        }*/
        
        /*
        // Edit 모드에서 사용
        protected void SavePublic(NetTools.CommaTextWriter writer, int tab_depth)
        {
            writer.WriteLineWithTab(tab_depth, "ColRow,{0},{1}", nColumn, nRow);
        }*/

        protected bool IsPublicLoadItem(ScriptReaderBlock block)
        {
            if (block.type == EnumBlockType.ColRow)
            {
                block.ReadColRow(out nColumn, out nRow);
                return true;
            }

            return false;
        }

        // Edit 모드에서 사용
        protected void SavePublic(ScriptWriter writer, int tab_depth)
        {
            writer.WriteColRow(nColumn, nRow);
        }

        
    }
}
