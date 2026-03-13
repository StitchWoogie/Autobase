using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;
using System.Threading.Tasks;

namespace ScriptLibRun
{
    public class CommandSubstitution : CommandPublic
    {
        //public string sTagetVarName;
        public RecursiveValue valueTarget;
        public RecursiveValue valueSource;

        public CommandSubstitution(ScriptLibMemberMethod parent, CommandBlock parent_block) :
            base(parent, parent_block)
        {
            eCommandType = EnumCommandType.type_substitution;
        }

        public async Task<bool> RunAsync(ScriptRunConfiguration src, ClassDataStack class_stack, ClassDataStack parent_stack)
        {
            (bool success, object value) = await valueSource.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
            if (!success) return false;

            success = await valueTarget.ChangeVariableAsync(src, class_stack, parent_stack, value).ConfigureAwait(false);
            if (!success) return false;

            return true;
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
                    if (valueTarget == null)
                    {
                        valueTarget = new RecursiveValue(parentMethod, parentBlock);
                        valueTarget.Load(block.block_data, ref line);
                    }
                    else if (valueSource == null)
                    {
                        valueSource = new RecursiveValue(parentMethod, parentBlock);
                        valueSource.Load(block.block_data, ref line);
                    }
                }
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(this.ToString(), block, line);
                    //return;
                }
            }
        }

        public override string MakeDecompiledFile(int depth, bool bAddSemicolon)
        {
            StringBuilder sb = new StringBuilder();

            //ScriptLibClass.AppendWithTab(sb, depth, "");
            sb.Append(valueTarget.MakeDecompiledFile(depth));
            sb.Append(" = ");
            sb.Append(valueSource.MakeDecompiledFile(depth));
            if(bAddSemicolon)
                sb.Append(";");

            return sb.ToString();
        }
    }
}
