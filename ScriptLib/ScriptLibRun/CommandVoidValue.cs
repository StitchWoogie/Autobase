using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;

namespace ScriptLibRun
{
    public class CommandVoidValue : CommandPublic
    {
        public RecursiveValue pValue;

        public CommandVoidValue(ScriptLibMemberMethod parent, CommandPublic parent_block) :
            base(parent, parent_block)
        {
            eCommandType = EnumCommandType.type_voidvalue;
        }
        
        public void Load(byte[] buffer, ref int line)
        {
            ScriptReader reader = new ScriptReader(buffer);
            ScriptReaderBlock block;

            while (true)
            {
                block = reader.ReadBlock();

                if (block == null) break;

                if (IsPublicLoadItem(block))
                {

                }
                else if (block.type == EnumBlockType.RecursiveValue)
                {
                    pValue = new RecursiveValue(parentMethod, parentBlock);
                    pValue.Load(block.block_data, ref line);
                }
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(this.ToString(), block, line);
                }
            }
        }

        public override string MakeDecompiledFile(int depth, bool bAddSemicolon)
        {
            StringBuilder sb = new StringBuilder();

            if (pValue != null)
            {
                sb.Append(pValue.MakeDecompiledFile(depth));
            }

            if(bAddSemicolon)
                sb.Append(";");

            return sb.ToString();
        }
    }
}
