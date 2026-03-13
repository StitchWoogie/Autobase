using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;

namespace ScriptLibRun
{
    public class MethodArgument : ColumnRowInfo
    {
        public EnumInOut eInOut;
        public Variable pVar = new Variable();

        public void Load(byte[] buffer, ref int line)
        {
            ScriptReader reader = new ScriptReader(buffer);
            ScriptReaderBlock block;

            while (true)
            {
                block = reader.ReadBlock();

                if (block == null) break;

                if (block.type == EnumBlockType.InOutType)
                {
                    eInOut = (EnumInOut)block.ReadByte();
                }
                else if (block.type == EnumBlockType.Variable)
                {
                    pVar.Load(block.block_data, ref line);
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
            else if (eInOut == EnumInOut.Ref)
                sb.Append("ref ");
            else if (eInOut == EnumInOut.Params)
                sb.Append("params ");
            else
            {

            }

            sb.Append(pVar.MakeDecompiledFile(depth));

            return sb.ToString();
        }
    }
}
