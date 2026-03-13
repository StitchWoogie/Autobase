using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;
using System.Threading.Tasks;

namespace ScriptLibRun
{
    public class CommandIf : CommandPublic
    {
        public RecursiveCondition pCondition;
        public CommandBlock blockCommand;
        public bool bElseIf = false;    // else if 인 경우 플래그를 살려준다.

        public CommandIf(ScriptLibMemberMethod parent, CommandPublic parent_block) :
            base(parent, parent_block)
        {
            eCommandType = EnumCommandType.type_if;
        }

        public async Task<(bool success, bool value)> CheckConditionAsync(ScriptRunConfiguration src, ClassDataStack class_stack, ClassDataStack parent_stack)
        {
            return await (pCondition.CheckConditionAsync(src, class_stack, parent_stack)).ConfigureAwait(false);
        }
        /*

        public void Load(TextReader reader, ref int line)
        {
            string one_line = "";
            string command = "";
            string second_command = "";
            CommaTextReader comma = new CommaTextReader();

            while (true)
            {
                one_line = reader.ReadLine();
                if (one_line == null) break;

                line++;

                comma.Set(one_line);
                comma.GetString(ref command);
                comma.GetString(ref second_command);

                if (IsPublicLoadItem(command, second_command, comma))
                {

                }
                else if (command == "CommandIf")
                {
                    if (String.Compare(second_command, "END", true) == 0)
                    {
                        return;
                    }
                }
                else if (command == "Condition")
                {
                    if (String.Compare(second_command, "BEGIN", true) == 0)
                    {
                        pCondition = new RecursiveCondition(parentMethod, parentBlock);
                        pCondition.Load(reader, ref line);
                    }
                }
                else if (command == "CommandBlock")
                {
                    if (String.Compare(second_command, "BEGIN", true) == 0)
                    {
                        blockCommand = new CommandBlock(parentMethod, parentBlock);
                        blockCommand.Load(reader, null, ref line);
                    }
                }
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(reader, this.ToString(), command, second_command, line);
                    return;
                }

                
            }
        }*/

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
                else if (block.type == EnumBlockType.CommandBlock)
                {
                    blockCommand = new CommandBlock(parentMethod, parentBlock); // 초기화 문에서 변수가 선언될 수 있으므로 parent_block은 pCommandBefore를 사용한다.
                    blockCommand.Load(block.block_data, null, ref line);
                }
                else if (block.type == EnumBlockType.Condition)
                {
                    pCondition = new RecursiveCondition(parentMethod, parentBlock); // 초기화 문에서 변수가 선언될 수 있으므로 parent_block은 pCommandBefore를 사용한다.
                    pCondition.Load(block.block_data, ref line);
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

            if(bElseIf)
                sb.Append("else if(");
            else
                sb.Append("if(");

            sb.Append(pCondition.MakeDecompiledFile(depth));
            sb.Append(")");
            sb.AppendLine();
            ScriptLibClass.AppendLineWithTab(sb, depth, "{{");
            sb.Append(blockCommand.MakeDecompiledFile(depth+1, true));
            ScriptLibClass.AppendWithTab(sb, depth, "}}");

            return sb.ToString();
        }
    }
}
