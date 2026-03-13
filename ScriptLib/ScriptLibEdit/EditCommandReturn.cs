using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;

namespace ScriptLibEdit
{
    public class EditCommandReturn : CommandReturn
    {


        public EditCommandReturn(EditScriptLibMethod parent, CommandBlock parent_block)
            : base(parent, parent_block)
        {
            
        }

        public override void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();

            SavePublic(writer, tab_depth + 1);
            if (pValue != null)
                ((EditRecursiveValue)pValue).SaveToStream(writer, tab_depth + 1);
            
            parent_writer.WriteBlock(EnumBlockType.CommandReturn, writer);
        }
        
        public override void Compile(ScriptLibMain main)
        {
            if (pValue != null)
                ((EditRecursiveValue)pValue).Compile(main);
        }
    }
}
