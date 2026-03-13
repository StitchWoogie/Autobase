using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;

namespace ScriptLibRun
{
    public class CommandGoto : CommandPublic
    {
        public string sNameLabel;

        public CommandGoto(ScriptLibMemberMethod parent, CommandBlock parent_block) :
            base(parent, parent_block)
        {
            eCommandType = EnumCommandType.type_goto;
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
                else if (command == "CommandGoto")
                {
                    if (String.Compare(second_command, "END", true) == 0)
                    {
                        return;
                    }
                }
                else if(command == "NameLabel") {
                    sNameLabel = second_command;
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
                else if (block.type == EnumBlockType.Name)
                {
                    sNameLabel = block.ReadString();
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

            sb.Append("goto ");
            sb.Append(sNameLabel);
            if (bAddSemicolon)
                sb.Append(";");

            return sb.ToString();
        }
    }
}
