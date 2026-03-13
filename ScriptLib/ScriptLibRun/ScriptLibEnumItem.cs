using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptLibRun
{
    public class ScriptLibEnumItem : ScriptLibMemberPublic
    {
        public string sName;
        public int value;

        public ScriptLibEnumItem()
        {
            eMember = EnumScriptLibMember.EnumItem;
        }

        public override string GetName()
        {
            return sName;
        }

        public override bool GetStaticValue(ScriptRunConfiguration src, out object retnvalue)
        {
            retnvalue = value;

            return true;
        }

        public override bool SetStaticValue(ScriptRunConfiguration src, object value)
        {
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

                if (block.type == EnumBlockType.Name)
                {
                    sName = block.ReadString();
                }
                else if (block.type == EnumBlockType.EnumValue)
                {
                    value = block.ReadInt();
                }
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(this.ToString(), block, line);
                    //return;
                }
            }
        }

        public override string MakeDecompiledFile(int depth)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(sName);
            sb.Append(" = ");
            sb.Append(value.ToString());
            return sb.ToString();
        }
    }
}
