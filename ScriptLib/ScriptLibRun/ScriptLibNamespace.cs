using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetTools;
using System.IO;

namespace ScriptLibRun
{
    public class ScriptLibNamespace
    {
        public string sNameNamespace;

        public List<ScriptLibMemberPublic> arrayMember = new List<ScriptLibMemberPublic>();     // 이름을 arrayClass로 하지 않은 이유는 class, enum 등 다양하기 때문에 arrayMember로 했다.

        public ScriptLibLibrary parentLibrary;

        public ScriptLibNamespace(ScriptLibLibrary parent)
        {
            parentLibrary = parent;
        }

        public virtual bool Split(ScriptLibMain main, object file, string sBody, int col_pos, int row_pos)
        {
            return false;
        }

        public virtual void Compile(ScriptLibMain main)
        {

        }

        public void SetBreakPoints(string filename, List<int> array)
        {
            for (int i = 0; i < arrayMember.Count; i++)
            {
                arrayMember[i].SetBreakPoints(filename, array);
            }
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
                    sNameNamespace = block.ReadString();
                }
                else if (block.type == EnumBlockType.ClassBlock)
                {
                    ScriptLibClass slc = new ScriptLibClass(this);
                    slc.Load(block.block_data, ref line);
                    arrayMember.Add(slc);
                }
                else if (block.type == EnumBlockType.EnumBlock)
                {
                    ScriptLibEnum sle = new ScriptLibEnum(this);
                    sle.Load(block.block_data, ref line);
                    arrayMember.Add(sle);
                }
                else
                {
                    ScriptLibClass.ShowErrorLoadUnknownCommand(this.ToString(), block, line);
                }
            }
        }

        public string MakeDecompiledFile()
        {
            int depth = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("namespace {0}", sNameNamespace));
            sb.AppendLine("{");
            for (int i = 0; i < arrayMember.Count; i++)
            {
                if (i > 0) sb.AppendLine(); // 두번째 부터는 한칸 띄워준다.

                sb.AppendLine(arrayMember[i].MakeDecompiledFile(depth+1));
            }
            sb.AppendLine("}");

            return sb.ToString();
        }

    }
}
