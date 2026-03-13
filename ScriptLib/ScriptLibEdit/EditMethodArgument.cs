using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;
using NetTools;

namespace ScriptLibEdit
{
    public class EditMethodArgument : MethodArgument
    {
        public EditMethodArgument()
        {
            pVar = new EditVariable();
        }

        public void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();
            writer.WriteByte(EnumBlockType.InOutType, (byte)eInOut);
            ((EditVariable)pVar).SaveToStream(writer, tab_depth + 1);
            parent_writer.WriteBlock(EnumBlockType.Param, writer);
        }

        public void Compile(ScriptLibMain main, EditScriptLibClass parent_class, int col_pos, int row_pos)
        {
            ((EditVariable)pVar).Compile(main, parent_class, nColumn, nRow);
        }
    }
}
