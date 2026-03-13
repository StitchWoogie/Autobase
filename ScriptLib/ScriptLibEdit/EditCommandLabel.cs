using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;

namespace ScriptLibEdit
{
    public class EditCommandLabel : CommandLabel
    {
        public EditCommandLabel(EditScriptLibMethod parent, CommandBlock parent_block)
            : base(parent, parent_block)
        {

        }

        public override void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();

            SavePublic(writer, tab_depth + 1);
            writer.WriteName(sNameLabel);

            parent_writer.WriteBlock(EnumBlockType.CommandLabel, writer);
        }
    }
}
