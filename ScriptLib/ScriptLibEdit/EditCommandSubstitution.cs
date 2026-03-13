using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;

namespace ScriptLibEdit
{
    public class EditCommandSubstitution : CommandSubstitution
    {
        public EditCommandSubstitution(EditScriptLibMethod parent, CommandBlock parent_block)
            : base(parent, parent_block)
        {
            valueTarget = new EditRecursiveValue(parentMethod, parent_block);
            valueSource = new EditRecursiveValue(parentMethod, parent_block);
        }

        public override void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();
            SavePublic(writer, tab_depth + 1);
            ((EditRecursiveValue)valueTarget).SaveToStream(writer, tab_depth + 1);
            ((EditRecursiveValue)valueSource).SaveToStream(writer, tab_depth + 1);
            parent_writer.WriteBlock(EnumBlockType.CommandSubstitution, writer);
        }

        public override void Compile(ScriptLibMain main)
        {
            ((EditRecursiveValue)valueTarget).Compile(main);
            ((EditRecursiveValue)valueSource).Compile(main);

            ((EditRecursiveValue)valueTarget).CheckSetable(main);
        }
    }
}
