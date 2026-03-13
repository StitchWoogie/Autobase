using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;

namespace ScriptLibRun
{
    public class CommandContinue : CommandPublic
    {
        public RecursiveValue value;

        public CommandContinue(ScriptLibMemberMethod parent, CommandBlock parent_block) :
            base(parent, parent_block)
        {
            eCommandType = EnumCommandType.type_continue;
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
                else if (command == "CommandContinue")
                {
                    if (String.Compare(second_command, "END", true) == 0)
                    {
                        return;
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

            sb.Append("continue");
            if (bAddSemicolon)
                sb.Append(";");

            return sb.ToString();
        }
    }
}
