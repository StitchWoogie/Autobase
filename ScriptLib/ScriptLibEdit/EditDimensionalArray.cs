using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLibRun;

namespace ScriptLibEdit
{
    public class EditDimensionalArray : DimensionalArray
    {
        public EditDimensionalArray(ScriptLibMemberMethod parent, CommandPublic parent_block)
            : base(parent, parent_block)
        {

        }

        public void Compile(ScriptLibMain main)
        {
            for (int i = 0; i < arrayValue.Count; i++)
            {
                ((EditRecursiveValue)arrayValue[i]).Compile(main);
            }
        }

        public void SaveToStream(ScriptWriter parent_writer, int tab_depth)
        {
            ScriptWriter writer = new ScriptWriter();

            for (int i = 0; i < arrayValue.Count; i++)
            {
                ((EditRecursiveValue)arrayValue[i]).SaveToStream(writer, tab_depth + 1);
            }

            parent_writer.WriteBlock(EnumBlockType.DimensinalArray, writer);
        }

        
    }
}
