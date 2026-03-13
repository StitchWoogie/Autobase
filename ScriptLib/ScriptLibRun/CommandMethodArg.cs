using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;
using System.Threading.Tasks;

namespace ScriptLibRun
{
    public class CommandMethodArg
    {
        public EnumInOut eInOut;
        public RecursiveValue value;

        ScriptLibMemberMethod parentMethod;
        CommandPublic parentBlock;

        public CommandMethodArg(ScriptLibMemberMethod parent, CommandPublic parent_block)
        {
            parentMethod = parent;
            parentBlock = parent_block;    
        }

        public async Task<(bool success, object val)> RunAsync(ScriptRunConfiguration src, ClassDataStack class_stack, ClassDataStack parent_stack)
        {
            return await value.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
        }

        public void Load(byte[] buffer, ref int line)
        {
            ScriptReader reader = new ScriptReader(buffer);
            ScriptReaderBlock block;

            while (true)
            {
                block = reader.ReadBlock();

                if (block == null) break;

                if (block.type == EnumBlockType.RecursiveValue)
                {
                    value = new RecursiveValue(parentMethod, parentBlock);
                    value.Load(block.block_data, ref line);
                }
                else if (block.type == EnumBlockType.InOutType)
                {
                    eInOut = (EnumInOut)block.ReadByte();
                }
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(this.ToString(), block, line);
                    //return;
                }
            }
        }

        public string MakeDecompiledFile(int depth)
        {
            StringBuilder sb = new StringBuilder();
            if (eInOut == EnumInOut.Out)
                sb.Append("out ");
            else if(eInOut == EnumInOut.Ref)
                sb.Append("ref ");
            else if (eInOut == EnumInOut.Params)
                sb.Append("params ");
            else
            {

            }

            sb.Append(value.MakeDecompiledFile(depth));

            return sb.ToString();
        }
    }
}
