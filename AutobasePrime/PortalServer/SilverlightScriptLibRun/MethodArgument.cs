using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightScriptLibRun
{
    public class MethodArgument
    {
        public EnumInOut eInOut;
        public Variable pVar = new Variable();

        /*
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

            sb.Append(pVar.MakeDecompiledFile(depth));


            return sb.ToString();
        }*/
    }
}
