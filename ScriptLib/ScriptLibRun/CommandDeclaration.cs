using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;
using System.Threading.Tasks;

namespace ScriptLibRun
{
    public class CommandDeclaration : CommandPublic
    {
        public Variable pVar;
        public RecursiveValue initValue = null;

        public CommandDeclaration(ScriptLibMemberMethod parent, CommandBlock parent_block) :
            base(parent, parent_block)
        {
            eCommandType = EnumCommandType.type_declaration;
        }

        public async Task<bool> RunAsync(ScriptRunConfiguration src, ClassDataStack class_stack, ClassDataStack parent_stack)
        {
            object value = null;

            if (initValue != null)
            {
                var (success, result) = await initValue.RunAsync(src, class_stack, parent_stack).ConfigureAwait(false);
                if (!success) return false;
                value = result;
            }

            parent_stack.AddItem(pVar, value);

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
                else if (block.type == EnumBlockType.Variable)
                {
                    pVar = new Variable();
                    pVar.Load(block.block_data, ref line);
                }
                else if (block.type == EnumBlockType.RecursiveValue)
                {
                    initValue = new RecursiveValue(parentMethod, parentBlock);
                    initValue.Load(block.block_data, ref line);
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

            sb.Append(pVar.MakeDecompiledFile(depth));

            if (initValue != null)
            {
                sb.Append(" = ");
                sb.Append(initValue.MakeDecompiledFile(depth));
            }
            if(bAddSemicolon)
                sb.Append(";");

            return sb.ToString();
        }

        // int a=1,b=3  과 같은 경우 첫번째는 데이터형을 쓰지만 그 다음은 데이터 형을 쓰면 안된다.
        public override string MakeDecompiledFileCommaBlock(int depth, bool bFirstComma)
        {
            StringBuilder sb = new StringBuilder();

            if (bFirstComma)
                sb.Append(pVar.MakeDecompiledFile(depth));
            else
                sb.Append(pVar.sVarName);

            if (initValue != null)
            {
                sb.Append(" = ");
                sb.Append(initValue.MakeDecompiledFile(depth));
            }

            return sb.ToString();
        }
    }
}
