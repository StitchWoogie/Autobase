using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;

namespace ScriptLibEdit
{
    class EditCommandMethodArg : CommandMethodArg
    {
        public EditCommandMethodArg(EditScriptLibMethod parent, CommandPublic parent_block)
            : base(parent, parent_block)
        {
            value = new EditRecursiveValue(parent, parent_block);
        }

        public void Compile(ScriptLibMain main)
        {
            ((EditRecursiveValue)value).Compile(main);
        }

        public void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();
            writer.WriteByte(EnumBlockType.InOutType, (byte)eInOut);
            ((EditRecursiveValue)value).SaveToStream(writer, tab_depth + 1);
            parent_writer.WriteBlock(EnumBlockType.CommandMethodArg, writer);
        }
    }
}
