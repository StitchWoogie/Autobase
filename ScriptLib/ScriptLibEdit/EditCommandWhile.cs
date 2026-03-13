using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;

namespace ScriptLibEdit
{
    public class EditCommandWhile : CommandWhile
    {
        public EditCommandWhile(EditScriptLibMethod parent, CommandBlock parent_block)
            : base(parent, parent_block)
        {
            
            

            pCondition = new EditRecursiveCondition(parentMethod, parent_block);
        }

        public override void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();
            SavePublic(writer, tab_depth + 1);
            ((EditRecursiveCondition)pCondition).SaveToStream(writer, tab_depth + 1);
            blockCommand.SaveToStream(writer, tab_depth + 1);
            parent_writer.WriteBlock(EnumBlockType.CommandWhile, writer);
        }

        public override void Compile(ScriptLibMain main)
        {
            ((EditRecursiveCondition)pCondition).Compile(main);
            blockCommand.Compile(main);
        }

    }
}
