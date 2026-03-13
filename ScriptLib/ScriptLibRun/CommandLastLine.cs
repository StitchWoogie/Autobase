using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;

namespace ScriptLibRun
{
    public class CommandLastLine : CommandPublic
    {
        public CommandLastLine(ScriptLibMemberMethod parent, CommandBlock parent_block) :
            base(parent, parent_block)
        {
            eCommandType = EnumCommandType.type_lastline;
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
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(this.ToString(), block, line);
                }
            }
        }

        /*
        public override string MakeDecompiledFile(int depth, bool bAddSemicolon)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("break");
            if (bAddSemicolon)
                sb.Append(";");

            return sb.ToString();
        }*/
    }
}
