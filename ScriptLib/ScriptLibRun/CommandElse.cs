using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;

namespace ScriptLibRun
{
    public class CommandElse : CommandPublic
    {
        public CommandBlock blockCommand;

        public CommandElse(ScriptLibMemberMethod parent, CommandBlock parent_block) :
            base(parent, parent_block)
        {
            eCommandType = EnumCommandType.type_else;
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
                else if (command == "CommandElse")
                {
                    if (String.Compare(second_command, "END", true) == 0)
                    {
                        return;
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
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(this.ToString(), block, line);
                }
            }
        }

        public override string MakeDecompiledFile(int depth, bool bAddSemicolon)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("else");
            sb.AppendLine();
            ScriptLibClass.AppendLineWithTab(sb, depth, "{{");
            sb.Append(blockCommand.MakeDecompiledFile(depth + 1, true));
            ScriptLibClass.AppendWithTab(sb, depth, "}}");

            return sb.ToString();
        }
    }
}
